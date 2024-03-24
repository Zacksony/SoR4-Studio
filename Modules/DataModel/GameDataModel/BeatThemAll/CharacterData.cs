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
                "None", //0
                "HoldForward", //1
                "TapForward", //2
                "ForwardForward", //3
                "HoldDown", //4
                "DownDown", //5
                "Hadoken", //6
                "Up", //7
                "Backward", //8
                "DiagonalUp", //9
                "DiagonalDown", //10
                "BackwardDiagonalUp", //11
                "BackwardDiagonalDown", //12
                "AiSpecialJump", //13
                "AiSpecialJumpForward", //14
                "AiSpecialJumpBackward", //15
                "AllyThrown" //16
            ];

            public static List<string> TrigerButtonDef { get; } =
            [
                "None", //0
                "PressX", //1
                "PressY", //2
                "PressB", //3
                "FuryX", //4
                "FuryY", //5
                "FuryB", //6
                "FuryAny", //7
                "ForwardDash", //8
                "BackDash", //9
                "Parry", //10
                "Taunt", //11
                "GrabFront", //12
                "GrabBack", //13
                "HoldOnlyForward", //14
                "Always", //15
                "GrabContinue", //16
                "PressA", //17
                "HoldBack", //18
                "AiTargetInDetector", //19
                "SecondVaultOrPressA", //20
                "GrabFrontOrBack", //21
                "StarMove", //22
                "HitWall", //23
                "HitWallStrict", //24
                "GrabAirFront", //25
                "GrabAirBack", //26
                "GrabAirFrontOrBack", //27
                "HoldAndReleaseX", //28
                "TouchEnemy", //29
                "DepthDashUp", //30
                "DepthDashDown" //31
            ];

            public static List<string> ArmorTypeDef { get; } =
            [
                "None", //0
                "Armor before first hit", //1
                "Armor whole move", //2
                "I-frames before first hit", //3
                "I-frames whole move", //4
                "U-frames before first hit", //5
                "U-frames whole move", //6
            ];

            public SoR4_DirectString MoveID => this[1]!;
            public SoR4_Int32 TrigerButton => this[2]!;
            public SoR4_Int32 MoveCondition => this[3]!; // enum: 0, 1, 2, 4; 0 – must be performed on the ground, 1 – must be performed in the air, rest is unknown
            public SoR4_Int32 TrigerDirection => this[4]!;
            public SoR4_Bool CanCancelIntoNormalHit => this[14]!;
            public SoR4_Bool CanCancelIntoSpecialHit => this[15]!;
            public SoR4_Int32 ArmorType => this[19]!;
            public SoR4_Scaled CancelWindowOpenBasicMoves => this[20, 1]!;
            public SoR4_Scaled GreenHpCost => this[21, 1]!;
            public SoR4_Bool CanCancelSpecialMoves => this[27]!;
            public SoR4_Scaled IncomeDamageMultipDuringMove => this[35, 1]!;
            public SoR4_Scaled CancelWindowOpenSelf => this[55, 1]!;
            public SoR4_DirectString? AlternativeMoveID => this[66, 2];
            public SoR4_Bool AntiFall => this[68]!;
            public ExtenderList<HitClass>? Hits => this[6];
            public ExtenderList<MoveCancelClass>? MoveCancels => this[7];

            internal class HitClass : FieldExtenderBase
            {
                public SoR4_Scaled Damage => this[1, 1]!;
                public SoR4_Int32 XForceGrounded => this[2, 1]!;
                public SoR4_Int32 YForceGrounded => this[3, 1]!;
                public SoR4_Int32 HitStop => this[4]!;
                public SoR4_Int32 HitStun => this[5]!;
                public SoR4_Int32 TargetLandAnimeCount => this[7]!;
                public SoR4_Bool FlipTargetOnLaunch => this[9]!;
                public SoR4_Bool IsMultiHit => this[10]!;
                public SoR4_Int32 EnableCollision => this[11]!; // 99 = true, 0 = false // 碰撞
                public SoR4_Scaled? CollisionDamage => this[12, 1, 1];
                public SoR4_Int32? CollisionXForce => this[12, 2, 1];
                public SoR4_Int32? CollisionYForce => this[12, 3, 1];
                public SoR4_Bool PushBackDirection => this[13]!; // 1 = push, 0 = pull
                public SoR4_Int32 YForceAerial => this[15, 1]!;
                public SoR4_Bool CanRecoverGreenHp => this[16]!;
                public SoR4_Int32 GroundBounceCount => this[17]!;
                public SoR4_Int32 XForceAerial => this[20, 1]!;
                public SoR4_DirectString FeedBackEffect => this[21, 1]!;
                public SoR4_Scaled ZDepth => this[24, 1]!;
                public SoR4_Scaled RecoveryFrames => this[28]!;
                public SoR4_Scaled WallBounceDamage => this[29, 2, 1]!;
                public SoR4_Scaled HitGravity => this[32, 1]!;
                public SoR4_Bool IsIgnoreWeight => this[35]!;
                public SoR4_Bool IsAlwaysOTG => this[37]!;
                public SoR4_Bool KnockDownOnLaunch => this[41]!;
                public SoR4_Bool EnableOTG => this[51]!; // 倒地追击
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
