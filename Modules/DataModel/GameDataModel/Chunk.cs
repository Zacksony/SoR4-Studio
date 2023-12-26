using SoR4_Studio.Modules.Utils.Protobuf;
using SoR4_Studio.Modules.Utils.Protobuf.ProtoBinary;
using System;
using System.IO;

namespace SoR4_Studio.Modules.DataModel.GameDataModel;

internal class Chunk(Stream stream, long protoPosition, int protoLength) : IDisposable
{
    public Stream Stream => stream;

    public long ProtoPosition => protoPosition;

    public int ProtoLength => protoLength;

    public bool IsDecoded => _root is not null;

    private ProtoField? _root;
    public ProtoField Root => _root ??= ProtoField.CreateMessage(ProtoDecoder.Decode(stream, protoPosition, protoLength));

    public ProtoField? this[params int[] address] => Root[address];

    #region IDisposable

    private bool _disposedValue = false;

    public void Dispose()
    {
        if (_disposedValue)
        {
            return;
        }

        _root?.Dispose();
        _root = null;

        _disposedValue = true;

        GC.SuppressFinalize(this);
    }

    #endregion
}
