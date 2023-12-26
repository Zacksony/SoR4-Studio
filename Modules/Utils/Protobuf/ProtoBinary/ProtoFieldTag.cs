namespace SoR4_Studio.Modules.Utils.Protobuf.ProtoBinary;

internal struct ProtoFieldTag
{
    public ProtoFieldTag(Varint baseValue) => BaseValue = baseValue;

    public ProtoFieldTag(int id, WireType wireType)
    {
        BaseValue = Varint.FromDecoded(((ulong)id << 3) | (ulong)wireType);
    }

    public Varint BaseValue { get; private set; }

    public int ID
    {
        readonly get => (int)(BaseValue.Decoded >> 3);

        set => BaseValue = Varint.FromDecoded((value << 3) | (int)WireType);
    }

    public WireType WireType
    {
        readonly get => (WireType)(int)(BaseValue.Decoded & 0x07);

        set => BaseValue = Varint.FromDecoded((ID << 3) | (int)value);
    }
}
