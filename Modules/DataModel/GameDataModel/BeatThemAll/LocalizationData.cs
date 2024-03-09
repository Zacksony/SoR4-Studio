using SoR4_Studio.Modules.DataModel.GameDataModel.BuiltIns;
using SoR4_Studio.Modules.DataModel.GameDataModel.FieldDescriber;
using SoR4_Studio.Modules.Utils.Protobuf.ProtoBinary;
using System;
using System.Linq;

namespace SoR4_Studio.Modules.DataModel.GameDataModel.BeatThemAll;

internal class LocalizationData(GameData gameData) : FieldExtenderBase(gameData, new(MainKeys.LocalizationData))
{
    public ExtenderList<LanguageClass> LanguageList => new(this[1]);

    #region StringManager

    public ExtenderList<LanguageClass.StringKeyValueClass>? CurrentStringDictionary
    {
        get
        {
            foreach (var dict in from language in LanguageList
                                 where language.LanguageName.Value == Properties.UserSettings.Default.ModdingLanguage
                                 select language.StringDictionary)
            {
                return dict;
            }

            return null;
        }
    }

    public void UnifyLanguage()
    {
        foreach (var language in LanguageList)
        {
            if (language.LanguageName.Value == Properties.UserSettings.Default.ModdingLanguage)
            {
                continue;
            }

            language.StringDictionary.Clear();
            foreach (var keyValue in CurrentStringDictionary ?? new(new()))
            {
                language.StringDictionary.Add(keyValue.DeepClone<LanguageClass.StringKeyValueClass>());
            }
        }
    }

    public string AddValue(string value)
    {
        string newKey = "_" + Convert.ToString(DateTime.UtcNow.Ticks, 16);

        if (TryGetValue(newKey) is not null) // key存在
        {
            return AddValue(value);
        }
        else // key不存在
        {
            AddValue(newKey, value);
        }

        return newKey;
    }

    public void AddValue(string key, string value)
    {
        ProtoField newField = ProtoField.CreateMessage([
            (1, ProtoField.CreateString(key)),
            (2, ProtoField.CreateString(value))
        ]);
        CurrentStringDictionary!.BaseList.Add(newField);
    }

    public void SetValue(string key, string value)
    {
        foreach (var keyValue in from keyValue in CurrentStringDictionary
                                 where keyValue.Key.Value == key
                                 select keyValue)
        {
            keyValue.Value.Value = value;
            return;
        }

        AddValue(key, value);
    }

    public string GetValue(string key)
    {
        return TryGetValue(key) ?? "*" + key;
    }

    private string? TryGetValue(string key)
    {
        foreach (var keyValue in from keyValue in CurrentStringDictionary
                                 where keyValue.Key.Value == key
                                 select keyValue)
        {
            return keyValue.Value.Value;
        }

        return null;
    }

    #endregion

    internal class LanguageClass : FieldExtenderBase
    {
        public SoR4_DirectString LanguageName => this[1]!;
        public ExtenderList<StringKeyValueClass> StringDictionary => new(this[2]);

        internal class StringKeyValueClass : FieldExtenderBase
        {
            public SoR4_DirectString Key => this[1]!;
            public SoR4_DirectString Value => this[2]!;
        }
    }
}
