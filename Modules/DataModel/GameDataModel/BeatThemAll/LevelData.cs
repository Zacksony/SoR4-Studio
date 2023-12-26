using SoR4_Studio.Modules.DataModel.GameDataModel.BuiltIns;
using SoR4_Studio.Modules.DataModel.GameDataModel.FieldDescriber;
using SoR4_Studio.Modules.Utils.Protobuf.ProtoBinary;
using System.Collections.Generic;
using System.Linq;

namespace SoR4_Studio.Modules.DataModel.GameDataModel.BeatThemAll;

internal class LevelData(GameData gameData)
{
    public LevelDataClass this[string levelID] => new(gameData, levelID);

    internal class LevelDataClass(GameData gameData, string levelID) : FieldExtenderBase(gameData, new(MainKeys.LevelData, levelID))
    {
        public string LevelID => levelID;
        public SoR4_DirectString DecorID => this[1, 1]!;
        public SoR4_Bool EnableRetroFilter => this[9]!;
        public SoR4_Bool EnableEnemyInfighting => this[11]!;
        public ExtenderList<WaveClass> Waves => this[101]!;

        internal class WaveClass : FieldExtenderBase
        {
            public SoR4_DirectString WaveName => this[13, 1]!;

            /*
             * 0 - Need to clear section
             * 1 - Can leave section without cleaning
             * 2 - After clear it's softlock
             */
            public SoR4_Int32 CameraLockType => this[13, 7]!;
            public SoR4_Int32 MaxEnemiesOnScreen => this[13, 17]!;// -1 for endless
            public ExtenderList<SpawnerClass>? SpawnerList => this[13, 6];

            internal class SpawnerClass : FieldExtenderBase
            {
                public SoR4_Int32 PosX => this[2, 1, 1]!;
                public SoR4_Int32 PosY => this[2, 2, 1]!;
                public SoR4_DirectString? OnGroundObjectID => this[10, 1, 1];
                public SoR4_DirectString? BreakableID => this[11, 1, 1];
                public SoR4_DirectString? InBreakablePickupID => this[11, 2, 1];
                public SoR4_Bool? IsBreakableMirrored => this[11, 4];
                public ExtenderList<EnemyClass>? Enemies => this[9, 1];
                public SoR4_Int32? EnemySpwanMultip => this[9, 5]; // 0 for default
                public SoR4_Int32? EnemyDifficultySpecificSpwan => this[9, 8]; // -1 for universal

                private List<SoR4_DirectString>? pickupUnnormal;
                public List<SoR4_DirectString> PickupUnnormal => pickupUnnormal ??= GetUnnormalPickupIDs();
                public List<SoR4_DirectString> GetUnnormalPickupIDs()
                {
                    List<SoR4_DirectString> ids = [];

                    foreach (ProtoField field in BaseField.EnumerateAllField())
                    {
                        if (field.Match(ProtoFieldType.String) && GameData.PickupIDs.Contains(field.String))
                        {
                            if ((OnGroundObjectID is not null) && (field == OnGroundObjectID.Field))
                            {
                                continue;
                            }
                            if ((InBreakablePickupID is not null) && (field == InBreakablePickupID.Field))
                            {
                                continue;
                            }

                            ids.Add(new(new(this, field)));
                        }
                    }

                    return ids;
                }
                private List<SoR4_DirectString>? breakableUnnormal;
                public List<SoR4_DirectString> BreakableUnnormal => breakableUnnormal ??= GetUnnormalBreakableIDs();
                public List<SoR4_DirectString> GetUnnormalBreakableIDs()
                {
                    List<SoR4_DirectString> ids = [];

                    foreach (ProtoField field in BaseField.EnumerateAllField())
                    {
                        if (field.Match(ProtoFieldType.String) && GameData.DestroyableIDs.Contains(field.String))
                        {
                            if ((BreakableID is not null) && (field == BreakableID.Field))
                            {
                                continue;
                            }

                            ids.Add(new(new(this, field)));
                        }
                    }

                    return ids;
                }
                private List<SoR4_DirectString>? enemiesUnnormal;
                public List<SoR4_DirectString> EnemiesUnnormal => enemiesUnnormal ??= GetUnnormalEnemyIDs();
                public List<SoR4_DirectString> GetUnnormalEnemyIDs()
                {
                    List<SoR4_DirectString> ids = [];

                    foreach (ProtoField field in BaseField.EnumerateAllField())
                    {
                        if (field.Match(ProtoFieldType.String) && GameData.CharacterIDs.Contains(field.String))
                        {
                            if (Enemies?.BaseList.Any((f) => (f[2, 44, 1, 1] is not null) && (f[2, 44, 1, 1] == field)) ?? false)
                            {
                                continue;
                            }

                            ids.Add(new(new(this, field)));
                        }
                    }

                    return ids;
                }

                internal class EnemyClass : FieldExtenderBase
                {
                    public SoR4_DirectString? EnemyID => this[2, 44, 1, 1];
                    public SoR4_DirectString? HoldWeaponID => this[2, 44, 2, 1];
                    public SoR4_Int32? HP => this[2, 44, 17]; // -1 for global
                }
            }
        }
    }
}
