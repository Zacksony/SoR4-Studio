using SoR4_Studio.Modules.Utils.Protobuf.ProtoBinary;

namespace SoR4_Studio.Modules.DataModel.GameDataModel.FieldDescriber;

internal readonly struct FieldDescriber(FieldExtenderBase messageExtender, ProtoField? result)
{
    public GameData GameData => messageExtender.GameData;
    public ProtoField? Result { get; init; } = result;

    public SoR4_Int32 Int32 => new(this);
    public SoR4_Bool Bool => new(this);
    public SoR4_Float Float => new(this);
    public SoR4_Scaled Scaled => new(this);
    public SoR4_DirectString DirectString => new(this);
    public SoR4_LocalizedString LocalizedString => new(this);

    public static implicit operator SoR4_Int32?(FieldDescriber v) => v.Result is null ? null : new(v);
    public static implicit operator SoR4_Bool?(FieldDescriber v) => v.Result is null ? null : new(v);
    public static implicit operator SoR4_Float?(FieldDescriber v) => v.Result is null ? null : new(v);
    public static implicit operator SoR4_Scaled?(FieldDescriber v) => v.Result is null ? null : new(v);
    public static implicit operator SoR4_DirectString?(FieldDescriber v) => v.Result is null ? null : new(v);
    public static implicit operator SoR4_LocalizedString?(FieldDescriber v) => v.Result is null ? null : new(v);
}

internal class SoR4_Int32(FieldDescriber describer)
{
    public ProtoField Field => describer.Result!;

    public int Value
    {
        get => describer.Result!.Int32;
        set => describer.Result!.Int32 = value;
    }
    public override string ToString() => Value.ToString();
    public static implicit operator int(SoR4_Int32 v) => v.Value;
}

internal class SoR4_Bool(FieldDescriber describer)
{
    public ProtoField Field => describer.Result!;

    public bool Value
    {
        get => describer.Result!.Int32 != 0;
        set => describer.Result!.Int32 = value ? 1 : 0;
    }
    public override string ToString() => Value.ToString();
    public static implicit operator bool(SoR4_Bool v) => v.Value;
}

internal class SoR4_Float(FieldDescriber describer)
{
    public ProtoField Field => describer.Result!;

    public float Value
    {
        get => describer.Result!.Single;
        set => describer.Result!.Single = value;
    }
    public override string ToString() => Value.ToString();
    public static implicit operator float(SoR4_Float v) => v.Value;
}

internal class SoR4_Scaled(FieldDescriber describer)
{
    public ProtoField Field => describer.Result!;

    public float Value
    {
        get => float.Round(describer.Result!.Int32 / 65536f, 3);
        set => describer.Result!.Int32 = (int)(value * 65536);
    }
    public override string ToString() => Value.ToString();
    public static implicit operator float(SoR4_Scaled v) => v.Value;
}

internal class SoR4_DirectString(FieldDescriber describer)
{
    public ProtoField Field => describer.Result!;

    public string Value
    {
        get => describer.Result!.String;
        set => describer.Result!.String = value;
    }
    public override string ToString() => Value.ToString();
    public static implicit operator string(SoR4_DirectString v) => v.Value;
}

internal class SoR4_LocalizedString(FieldDescriber describer)
{
    public ProtoField Field => describer.Result!;

    public string Value
    {
        get => describer.GameData.LocalizationData.GetValue(describer.Result!.String);
        set => describer.Result!.String = describer.GameData.LocalizationData.AddValue(value ?? "");
    }
    public override string ToString() => Value.ToString();
    public static implicit operator string(SoR4_LocalizedString v) => v.Value;
}
