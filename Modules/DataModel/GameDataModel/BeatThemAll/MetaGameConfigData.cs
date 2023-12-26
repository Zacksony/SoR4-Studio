using SoR4_Studio.Modules.DataModel.GameDataModel.BuiltIns;
using SoR4_Studio.Modules.DataModel.GameDataModel.FieldDescriber;

namespace SoR4_Studio.Modules.DataModel.GameDataModel.BeatThemAll;

internal class MetaGameConfigData(GameData gameData) : FieldExtenderBase(gameData, new(MainKeys.MetaGameConfigData))
{
    public ExtenderList<LevelDirectorClass> MainCampaignDirectors => this[14, 0, 1]!;
    public ExtenderList<DifficultyClass> Difficulties => this[29]!;

    internal class LevelDirectorClass : FieldExtenderBase
    {
        public ExtenderList<LevelIDClass> LevelIDs => this[1]!;
        public SoR4_Int32 Int01 => this[5]!;
        public SoR4_Int32 Int02 => this[6]!;
        public SoR4_Bool Bool01 => this[7]!;

        internal class LevelIDClass : FieldExtenderBase
        {
            public SoR4_DirectString LevelID => this[1]!;
        }
    }

    internal class DifficultyClass : FieldExtenderBase
    {
        private string DifficultyDescriptionKey
            => this[1].DirectString.Value + "_DESC";
        public string DifficultyName
        {
            get => this[1].LocalizedString.Value;

            set
            {
                string oldDesc = DifficultyDescription;
                this[1].LocalizedString.Value = value;
                GameData.LocalizationData.SetValue(DifficultyDescriptionKey, oldDesc);
            }
        }
        public string DifficultyDescription
        {
            get => GameData.LocalizationData.GetValue(DifficultyDescriptionKey);

            set
            {
                string oldName = DifficultyName;
                DifficultyName = oldName;
                GameData.LocalizationData.SetValue(DifficultyDescriptionKey, value);
            }
        }
        public SoR4_Int32 StartingLivesStoryMode => this[4, 1]!;
        public SoR4_Int32 MaxAttacker => this[4, 3]!;
        public SoR4_Scaled EnemyHpMultip => this[4, 4, 1]!;
        public SoR4_Scaled GlobalSpawnMultip => this[4, 5, 1]!;
        public SoR4_Int32 StartingStars => this[4, 6]!;
        public SoR4_Int32 AggroSlots => this[4, 7]!;
        public SoR4_Int32 EnemyActivity => this[4, 8]!;
        public SoR4_Scaled GreenHpUsageMultip => this[4, 11, 1]!;
        public SoR4_Int32 StartingLivesArcade => this[4, 13]!;
        public SoR4_Float RankScoreMultip => this[4, 14]!;
        public SoR4_Scaled EnemySpeedBoost => this[4, 16, 1]!;
        public SoR4_Scaled IncomeDamageMultip => this[4, 18, 1]!;
        public SoR4_Int32 ExtraLifePointsStoryMode => this[4, 19]!;
        public SoR4_Int32 ExtraLifePointsArcade => this[4, 20]!;
    }
}
