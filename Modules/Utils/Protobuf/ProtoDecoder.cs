using SoR4_Studio.Modules.Utils.Protobuf.ProtoBinary;
using System;
using System.Buffers;
using System.IO;
using System.Text;

namespace SoR4_Studio.Modules.Utils.Protobuf;

internal class ProtoDecoder
{
    public static Message Decode(Stream stream, long position, long count)
    {
        stream.Position = position;
        using BinaryReader reader = new(stream, Encoding.UTF8, true);
        return new ProtoDecoder(reader).TryParseMessage(position + count - 1)
            ?? throw ProtobufExceptions.ProtoMessageParseFailedException;
    }

    private ProtoDecoder(BinaryReader reader) => (_reader, _stream) = (reader, reader.BaseStream);

    private readonly BinaryReader _reader;

    private readonly Stream _stream;

    private Message? TryParseMessage(long endPosition)
    {
        Message message = [];

        while (endPosition - _stream.Position + 1 > 0)
        {
            if (TryParseFieldTag(endPosition) is not ProtoFieldTag tag)
            {
                DisposeMessage(message);
                return null;
            }

            if (TryParseFieldContent(endPosition, tag.WireType) is not ProtoField field)
            {
                DisposeMessage(message);
                return null;
            }

            if (message.Find((v) => v.ID == tag.ID).Field is ProtoField previous)
            {
                previous.UpgradeToRepeated().Repeated.Add(field);
            }
            else
            {
                message.Add((tag.ID, field));
            }
        }

        return message;
    }

    private ProtoFieldTag? TryParseFieldTag(long endPosition)
    {
        long inputLength = endPosition - _stream.Position + 1;
        int varintLength = Varint.GetVarintLength(_stream);

        // 为什么有等号：Tag不能落单

        if (inputLength <= varintLength)
        {
            return null;
        }

        byte[] tagBytes = ArrayPool<byte>.Shared.Rent(varintLength);
        _stream.Read(tagBytes, 0, varintLength);
        ProtoFieldTag tag = new(new(tagBytes));
        ArrayPool<byte>.Shared.Return(tagBytes);

        // field number 范围: [1, 2^29-1]

        if (tag.ID is < 1 or > 536_870_911)
        {
            _stream.Position -= varintLength;
            return null;
        }

        if (!Enum.IsDefined(typeof(WireType), tag.WireType))
        {
            _stream.Position -= varintLength;
            return null;
        }

        return tag;
    }

    private ProtoField? TryParseFieldContent(long endPosition, WireType wireType)
    {
        long inputLength = endPosition - _stream.Position + 1;

        switch (wireType)
        {
            case WireType.Varint:
                {
                    int readLength = Varint.GetVarintLength(_stream);

                    if (inputLength < readLength)
                    {
                        return null;
                    }

                    byte[] varintBytes = ArrayPool<byte>.Shared.Rent(readLength);
                    _stream.Read(varintBytes, 0, readLength);
                    Varint varint = new(varintBytes);
                    ArrayPool<byte>.Shared.Return(varintBytes);
                    return ProtoField.CreateVarint(varint.Decoded);
                }

            case WireType.Fixed32:
                {
                    int readLength = 4;

                    if (inputLength < readLength)
                    {
                        return null;
                    }

                    return ProtoField.CreateFixed32(_reader.ReadInt32());
                }

            case WireType.Fixed64:
                {
                    int readLength = 8;

                    if (inputLength < readLength)
                    {
                        return null;
                    }

                    return ProtoField.CreateFixed64(_reader.ReadInt64());
                }

            case WireType.LengthDelimited:
                {
                    // 1

                    int varintLength = Varint.GetVarintLength(_stream);

                    if (inputLength < varintLength)
                    {
                        return null;
                    }

                    // 2

                    byte[] varintBytes = ArrayPool<byte>.Shared.Rent(varintLength);
                    _stream.Read(varintBytes, 0, varintLength);
                    int readLength = new Varint(varintBytes).DecodedInt32;
                    ArrayPool<byte>.Shared.Return(varintBytes);

                    if (inputLength < varintLength + readLength || readLength < 0)
                    {
                        _stream.Position -= varintLength;
                        return null;
                    }

                    if (readLength == 0)
                    {
                        return ProtoField.CreateString("");
                    }

                    /*
                    | *如果发现内容都是ASC-II的可打印字符，
                    |  那么直接判定为string
                    */

                    if (_stream.IsDisplayables(readLength))
                    {
                        return ProtoField.CreateString(Encoding.UTF8.GetString(_reader.ReadBytes(readLength)));
                    }

                    // Is it SubMessage?

                    long oldPosition = _stream.Position;

                    if (TryParseMessage(_stream.Position + readLength - 1) is Message message)
                    {
                        return ProtoField.CreateMessage(message);
                    }
                    else
                    {
                        _stream.Position = oldPosition;
                    }

                    // Not SubMessage

                    return ProtoField.CreateString(Encoding.UTF8.GetString(_reader.ReadBytes((int)readLength)));
                }

            default:
                return null;
        }
    }

    private static void DisposeMessage(Message message)
    {
        foreach (var (_, Field) in message)
        {
            Field.Dispose();
        }

        message.Clear();
    }
}
