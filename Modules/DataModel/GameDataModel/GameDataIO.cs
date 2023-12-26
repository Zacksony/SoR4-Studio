using SoR4_Studio.Modules.Utils;
using SoR4_Studio.Modules.Utils.Protobuf;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace SoR4_Studio.Modules.DataModel.GameDataModel;

internal class GameDataIO : IDisposable
{
    /*
     * +-------------------+
     * | Bigfile Structure |
     * +-------------------+
     * _______________________________
     *  TYPE      |  NAME
     * -----------+-------------------
     *  Int32     |  TotalChunkCount
     *  Chunk[]   |  Chunks
     * 
     * +-----------------+
     * | Chunk Structure |
     * +-----------------+
     * _______________________________
     *  TYPE      |  NAME
     * -----------+-------------------
     *  Varint    |  MainKeyLength
     *  UTF16     |  MainKeyString
     *  Varint    |  SubKeyLength
     *  UTF16     |  SubKeyString
     *  Int32     |  ProtobufLength
     *  byte[]    |  ProtobufMessage
     */

    public GameDataIO(Stream BigFileStream, bool isCompressed)
    {
        TempFileManager.CleanUp();
        GameDataStream = TempFileManager.GenNewTempFile();

        if (isCompressed)
        {
            using DeflateStream deflateStream = new(BigFileStream, CompressionMode.Decompress);

            try
            {
                deflateStream.CopyTo(GameDataStream);
            }
            catch (IOException)
            {
                throw;
            }
            finally
            {
                BigFileStream.Close();
            }
        }
        else // uncompressed
        {
            BigFileStream.CopyTo(GameDataStream);
            BigFileStream.Close();
        }
    }

    public Stream GameDataStream { get; }

    private GameData? _gameData;
    public GameData GameData => _gameData ??= new(PreScanChunks());

    public void Save(string outputFileName, bool doCompress = true, CompressionLevel compressionLevel = CompressionLevel.Fastest)
    {
        Stream outputStream;

        if (doCompress)
        {
            outputStream = new DeflateStream(File.Create(outputFileName), compressionLevel);
        }
        else
        {
            outputStream = File.Create(outputFileName);
        }

        Output(outputStream);

        outputStream.Close();

        TempFileManager.CleanUp();
    }

    public void Output(Stream outputStream)
    {
        using Stream inputStream = RawDataOutput();
        inputStream.CopyTo(outputStream);
        if (outputStream.CanSeek) { outputStream.Position = 0; }
    }

    private Chunks PreScanChunks()
    {
        GameDataStream.Position = 0;

        Chunks chunks = [];

        HashSet<string> titleSet = []; // 用来检测重复

        using BinaryReader reader = new(GameDataStream, Encoding.UTF8, leaveOpen: true);

        int totalChunkCount = reader.ReadInt32();

        for (int i = 0; i < totalChunkCount; i++)
        {
            string mainKeyString = Encoding.Unicode.GetString(Encoding.UTF8.GetBytes(reader.ReadString()));
            string subKeyString = Encoding.Unicode.GetString(Encoding.UTF8.GetBytes(reader.ReadString()));
            int protoLength = reader.ReadInt32();
            long protoPosition = reader.BaseStream.Position;
            reader.BaseStream.Position += protoLength;

            if (!titleSet.Add($"{mainKeyString}{subKeyString}"))
            {
                continue;
            }

            if (!chunks.TryGetValue(mainKeyString, out Dictionary<string, Chunk>? value))
            {
                chunks.Add(mainKeyString, value = []);
            }

            value.Add(subKeyString, new Chunk(GameDataStream, protoPosition, protoLength));
        }

        return chunks;
    }

    private Stream RawDataOutput()
    {
        using BinaryWriter outputWriter = new(TempFileManager.GenNewTempFile(), Encoding.Unicode, leaveOpen: true);

        outputWriter.Write(GameData.TotalChunkCount);

        foreach (var iMainPair in GameData.Chunks)
        {
            foreach (var iSubPair in iMainPair.Value)
            {
                outputWriter.Write(iMainPair.Key);
                outputWriter.Write(iSubPair.Key);

                ChunkOutput(outputWriter, iSubPair.Value);
            }
        }

        outputWriter.BaseStream.Position = 0;
        return outputWriter.BaseStream;
    }

    private static void ChunkOutput(BinaryWriter outputWriter, Chunk chunk)
    {
        if (chunk.IsDecoded)
        {
            using MemoryStream tempMemorySteam = new();
            ProtoEncoder.EncodeMessage(tempMemorySteam, chunk.Root.Message);

            outputWriter.Write((int)tempMemorySteam.Length);
            tempMemorySteam.Position = 0;
            tempMemorySteam.CopyTo(outputWriter.BaseStream);
        }
        else
        {
            outputWriter.Write(chunk.ProtoLength);

            byte[] buffer = ArrayPool<byte>.Shared.Rent(chunk.ProtoLength);

            chunk.Stream.Position = chunk.ProtoPosition;
            chunk.Stream.Read(buffer, 0, chunk.ProtoLength);
            outputWriter.Write(buffer, 0, chunk.ProtoLength);

            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    #region IDisposable

    private bool _disposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            _gameData?.Dispose();
        }

        GameDataStream.Close();
        TempFileManager.CleanUp();
        _gameData = null;

        _disposedValue = true;
    }

    ~GameDataIO()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
