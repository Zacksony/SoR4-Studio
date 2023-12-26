using SoR4_Studio.Modules.DataModel.GameDataModel.BuiltIns;
using SoR4_Studio.Modules.DataModel.GameDataModel.FieldDescriber;
using SoR4_Studio.Modules.Utils.Protobuf.ProtoBinary;
using System.Collections.Generic;

namespace SoR4_Studio.Modules.DataModel.GameDataModel.BeatThemAll;

internal class DecorData(GameData gameData)
{
    public DecorDataClass this[string decorID] => new(gameData, decorID);

    internal class DecorDataClass : FieldExtenderBase
    {
        public DecorDataClass(GameData gameData, string decorID) : base(gameData, new(MainKeys.DecorData, decorID))
        {
            InitialAreas();
        }

        public HashSet<List<(int x, int y)>> Areas { get; init; } = [];

        private void InitialAreas()
        {
            foreach (ProtoField message_8 in this[8].Result?.UpgradeToRepeated().Repeated ?? [])
            {
                foreach (ProtoField message_8_i_5 in message_8[5]?.UpgradeToRepeated().Repeated ?? [])
                {
                    if (message_8_i_5[14, 2, 1] is not ProtoField nodeListField)
                    {
                        continue;
                    }

                    List<(int, int)> nodeList = [];

                    foreach (ProtoField nodeField in nodeListField.UpgradeToRepeated().Repeated)
                    {
                        nodeList.Add((nodeField[1, 1]!.Int32, nodeField[2, 1]!.Int32));
                    }

                    Areas.Add(nodeList);
                }
            }
        }
    }
}
