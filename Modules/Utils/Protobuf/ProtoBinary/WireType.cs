namespace SoR4_Studio.Modules.Utils.Protobuf.ProtoBinary;

internal enum WireType : uint
{
    // Varint int32, int64, uint32, uint64, sint32, sint64, bool, enum
    Varint = 0,

    // 64-bit fixed64, sfixed64, double
    Fixed64 = 1,

    // Length-delimited string, bytes, embedded messages, packed repeated fields
    LengthDelimited = 2,

    // 32-bit fixed32, sfixed32, float
    Fixed32 = 5
}
