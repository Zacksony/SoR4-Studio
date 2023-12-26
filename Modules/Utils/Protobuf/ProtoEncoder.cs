using SoR4_Studio.Modules.Utils.Protobuf.ProtoBinary;
using System.IO;
using System.Text;

namespace SoR4_Studio.Modules.Utils.Protobuf;

internal static class ProtoEncoder
{
    public static void EncodeMessage(Stream stream, Message message)
    {
        using BinaryWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        foreach (var (ID, Field) in message)
        {
            EncodeField(writer, ID, Field);
        }
    }

    private static void EncodeField(BinaryWriter writer, int id, ProtoField field)
    {
        ProtoFieldTag tag;
        if (field.Match(ProtoFieldType.Fixed32))
        {
            tag = new(id, WireType.Fixed32);
            writer.Write(tag.BaseValue.EncodedBytes);
            writer.Write(field.Int32);
        }
        else if (field.Match(ProtoFieldType.Fixed64))
        {
            tag = new(id, WireType.Fixed64);
            writer.Write(tag.BaseValue.EncodedBytes);
            writer.Write(field.Int64);
        }
        else if (field.Match(ProtoFieldType.Varint))
        {
            tag = new(id, WireType.Varint);
            writer.Write(tag.BaseValue.EncodedBytes);
            writer.Write(Varint.FromDecoded(field.UInt64).EncodedBytes);
        }
        else if (field.Match(ProtoFieldType.String))
        {
            tag = new(id, WireType.LengthDelimited);
            writer.Write(tag.BaseValue.EncodedBytes);
            writer.Write(Varint.FromDecoded(field.ByteSize).EncodedBytes);
            writer.Write(Encoding.UTF8.GetBytes(field.String));
        }
        else if (field.Match(ProtoFieldType.Message))
        {
            tag = new(id, WireType.LengthDelimited);
            writer.Write(tag.BaseValue.EncodedBytes);

            using MemoryStream tempMemoryStream = new();
            EncodeMessage(tempMemoryStream, field.Message);

            writer.Write(Varint.FromDecoded((ulong)tempMemoryStream.Length).EncodedBytes);
            tempMemoryStream.Position = 0;
            tempMemoryStream.CopyTo(writer.BaseStream);
        }
        else if (field.Match(ProtoFieldType.Repeated))
        {
            foreach (ProtoField iField in field.Repeated)
            {
                EncodeField(writer, id, iField);
            }
        }
    }
}
