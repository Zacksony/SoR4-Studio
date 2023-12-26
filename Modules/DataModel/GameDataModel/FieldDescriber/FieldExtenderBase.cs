using SoR4_Studio.Modules.Utils.Protobuf.ProtoBinary;

namespace SoR4_Studio.Modules.DataModel.GameDataModel.FieldDescriber;

internal abstract class FieldExtenderBase
{
    public FieldExtenderBase() { }
    public FieldExtenderBase(GameData gameData, FieldAddress address)
    {
        _gameData = gameData;
        _baseField = gameData[address]!;
    }

    private GameData? _gameData;
    public GameData GameData => _gameData!;

    private ProtoField? _baseField;
    public ProtoField BaseField => _baseField!;

    public FieldDescriber this[params int[] address] => new(this, BaseField![address]);

    public T DeepClone<T>() where T : FieldExtenderBase, new()
    {
        return new() { _gameData = GameData, _baseField = BaseField.DeepClone() };
    }

    public static T MakeChild<T>(GameData gameData, ProtoField baseField) where T : FieldExtenderBase, new()
    {
        return new() { _gameData = gameData, _baseField = baseField };
    }
}
