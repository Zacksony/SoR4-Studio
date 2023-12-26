namespace SoR4_Studio.Modules.Utils.Protobuf.ProtoBinary;

internal enum ProtoFieldType : byte
{
    Unknown = 0b0000_0000,

    Fixed32 = 0b0000_0001,

    Fixed64 = 0b0000_0010,

    Varint = 0b0000_0100,

    String = 0b0000_1000,

    Message = 0b0001_0000,

    Repeated = 0b0010_0000
}
