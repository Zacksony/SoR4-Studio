using SoR4_Studio.Modules.DataModel.GameDataModel.BuiltIns;
using SoR4_Studio.Modules.DataModel.GameDataModel.FieldDescriber;
using System.Collections.Generic;

namespace SoR4_Studio.Modules.DataModel.GameDataModel.BeatThemAll;

internal class CharacterData(GameData gameData)
{
    public CharacterDataClass this[string characterID] => new(gameData, characterID);

    internal class CharacterDataClass(GameData gameData, string characterID) : FieldExtenderBase(gameData, new(MainKeys.CharacterData, characterID))
    {
        public SoR4_Int32 SpeedX => this[4, 1, 1]!;
        public SoR4_Int32 SpeedY => this[4, 2, 1]!;
        public SoR4_Scaled HP => this[9, 1]!;
        public SoR4_DirectString AIID => this[10, 1, 1]!;
        public SoR4_DirectString DeathScreamID => this[12, 1]!;
        public SoR4_LocalizedString DisplayName => this[13]!;
        public SoR4_DirectString IconID => this[14, 1]!;
        public SoR4_Bool IsBoss => this[25]!;
        public SoR4_Int32 Team => this[29]!;
        public SoR4_Bool IsPermaArmor => this[104]!;
        public SoR4_Bool DoCleanAfterDeath => this[105]!;
        public ExtenderList<UsableWeaponClass>? UsableWeapones => this[34];
        public ExtenderList<MoveClass> Moves => this[99]!;

        internal class UsableWeaponClass : FieldExtenderBase
        {
            public SoR4_DirectString WeaponID => this[1]!;
        }

        internal class MoveClass : FieldExtenderBase
        {
            public static List<string> TrigerDirectionDef { get; } =
            [
                "None",
                "HoldForward",
                "TapForward",
                "ForwardForward",
                "HoldDown",
                "DownDown",
                "Hadoken",
                "Up",
                "Backward",
                "DiagonalUp",
                "DiagonalDown",
                "BackwardDiagonalUp",
                "BackwardDiagonalDown",
                "AiSpecialJump",
                "AiSpecialJumpForward",
                "AiSpecialJumpBackward",
                "AllyThrown"
            ];

            public static List<string> TrigerButtonDef { get; } =
            [
                "None",
                "PressX",
                "PressY",
                "PressB",
                "FuryX",
                "FuryY",
                "FuryB",
                "FuryAny",
                "ForwardDash",
                "BackDash",
                "Parry",
                "Taunt",
                "GrabFront",
                "GrabBack",
                "HoldOnlyForward",
                "Always",
                "GrabContinue",
                "PressA",
                "HoldBack",
                "AiTargetInDetector",
                "SecondVaultOrPressA",
                "GrabFrontOrBack",
                "StarMove",
                "HitWall",
                "HitWallStrict",
                "GrabAirFront",
                "GrabAirBack",
                "GrabAirFrontOrBack",
                "HoldAndReleaseX",
                "TouchEnemy",
                "DepthDashUp",
                "DepthDashDown"
            ];

            public static List<string> ArmorTypeDef { get; } =
            [
                "None",
                "Armor before first hit",
                "Armor whole move",
                "I-frames before first hit",
                "I-frames whole move",
                "U-frames before first hit",
                "U-frames whole move",
            ];

            public SoR4_DirectString MoveID => this[1]!;
            public SoR4_Int32 TrigerButton => this[2]!;
            public SoR4_Int32 MoveCondition => this[3]!; // 0 – must be performed on the ground, 1 – must be performed in the air, rest is unknown
            public SoR4_Int32 TrigerDirection => this[4]!;
            public SoR4_Bool CanCancelIntoNormalHit => this[14]!;
            public SoR4_Bool CanCancelIntoSpecialHit => this[15]!;
            public SoR4_Int32 ArmorType => this[19]!;
            public SoR4_Scaled FrameLength => this[20, 1]!;
            public SoR4_Scaled GreenHpCost => this[21, 1]!;
            public SoR4_Bool CanCancelSpecialHit => this[27]!;
            public SoR4_Scaled IncomeDamageMultipDuringMove => this[35, 1]!;
            public SoR4_Scaled MinFrameLengthToCancel => this[55, 1]!;
            public SoR4_DirectString? AlternativeMoveID => this[66, 2];
            public ExtenderList<HitClass>? Hits => this[6];
            public ExtenderList<MoveCancelClass>? MoveCancels => this[7];

            internal class HitClass : FieldExtenderBase
            {
                public SoR4_Scaled Damage => this[1, 1]!;
                public SoR4_Int32 XForce => this[2, 1]!;
                public SoR4_Int32 YForce => this[3, 1]!;
                public SoR4_Int32 HitStop => this[4]!;
                public SoR4_Int32 HitStun => this[5]!;
                public SoR4_Int32 Seizures => this[7]!;
                public SoR4_Bool WillTurnAroundOnHit => this[9]!;
                public SoR4_Bool IsMultiHit => this[10]!;
                public SoR4_Int32 EnableCollision => this[11]!; // 99 = true, 0 = false // 碰撞
                public SoR4_Scaled? CollisionDamage => this[12, 1, 1];
                public SoR4_Int32? CollisionXForce => this[12, 2, 1];
                public SoR4_Int32? CollisionYForce => this[12, 3, 1];
                public SoR4_Bool PushBackDirection => this[13]!; // 1 = push, 0 = pull
                public SoR4_Int32 AirJuggleLaunchHeight => this[15, 1]!;
                public SoR4_Bool CanRecoverGreenHp => this[16]!;
                public SoR4_Int32 GroundBounceCount => this[17]!;
                public SoR4_Int32 PushbackStrengthForAirborne => this[20, 1]!;
                public SoR4_DirectString FeedBackEffect => this[21, 1]!;
                public SoR4_Scaled HitDepth => this[24, 1]!;
                public SoR4_Scaled WallBounceDamage => this[29, 2, 1]!;
                public SoR4_Scaled HitGravity => this[32, 1]!;
                public SoR4_Bool IsIgnoreWeight => this[35]!;
                public SoR4_Bool IsIgnoreOTG => this[37]!;
                public SoR4_Bool AllowEnemyLieDown => this[41]!;
                public SoR4_Bool AllowOTGPursuit => this[51]!; // 倒地追击
                public SoR4_Bool CanHitAirborne => this[59]!;
            }

            internal class MoveCancelClass : FieldExtenderBase
            {
                public SoR4_DirectString MoveName => this[1]!;
                public SoR4_Int32 TrigerButton => this[2]!;
                public SoR4_Int32 TrigerDirection => this[3]!;
                public SoR4_Scaled StartFrame => this[4, 1]!;
                public SoR4_Scaled EndFrame => this[5, 1]!;
                public SoR4_Bool CannotCancelOnWhiff => this[6]!;
                public SoR4_Bool TurnAround => this[9]!;
            }
        }
    }
}
