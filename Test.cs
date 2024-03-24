using SoR4_Studio.Modules.DataModel.GameDataModel;
using SoR4_Studio.Modules.DataModel.GameDataModel.BuiltIns;
using SoR4_Studio.Modules.DataModel.GameDataModel.FieldDescriber;
using SoR4_Studio.Modules.Utils.Protobuf.ProtoBinary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Mvmb = SoR4_Studio.Modules.ViewModel.ModdingViewModelBase;

namespace SoR4_Studio;

internal static class Test
{
    public static string gameBigfileName = @"D:\SteamLibrary\steamapps\common\Streets of Rage 4\data\bigfile";
    
    public static void Main()
    {
        
    }

    public static void Raining()
    {
        GameDataIO modIO = new(File.OpenRead(gameBigfileName), isCompressed: true);
        GameData mod = modIO.GameData;

        FieldAddress addressSprList = new(MainKeys.DecorData, @"decors/main_campaign/stage_1/lvl_1/dcr_stage1_1", [8, 6, 5, 0, 5]);
        int[] addressX = [11, 2, 1];
        int[] addressY = [11, 2, 2];
        int[] addressZ = [11, 2, 3];

        Repeated sprList = mod[addressSprList]!.UpgradeToRepeated().Repeated;
        ProtoField initSpr = sprList[0].DeepClone();

        sprList.Clear();

        for (int x = -16384; x < 131072; x += 186)
        {
            for (int y = 4096; y > -4096; y -= 1024)
            {
                ProtoField newSpr = initSpr.DeepClone();
                newSpr[addressX]!.Int32 = x;
                newSpr[addressY]!.Int32 = y;
                newSpr[addressZ]!.Int32 = 400;
                sprList.Add(newSpr);
            }
        }

        //modIO.OutputToFile(gameBigfileName);
    }

    public static void FindField()
    {
        GameDataIO modIO = new(File.OpenRead(gameBigfileName), isCompressed: true);
        GameData mod = modIO.GameData;

        foreach (string id in mod.GuiNodeIDs)
        {
            foreach (ProtoField field in mod[new(MainKeys.GuiNodeData, id)]!.EnumerateAllField())
            {
                if (field.Type == ProtoFieldType.String && field.String.Contains(@"survival", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"{id} -> {field.String}");
                }
                else
                {
                    //Console.WriteLine($"{charaID} -> {field}");
                }
            }
        }
    }

    public static void ModdingComboLabel()
    {
        GameDataIO modIO = new(File.OpenRead(gameBigfileName), isCompressed: true);
        GameData mod = modIO.GameData;

        Repeated comboInfo = mod[new(MainKeys.GameplayConfigData)]![103]!.Repeated;
        comboInfo[1][1]!.Int32 = 60;
        comboInfo[1][8, 1]!.Int32 = 255;
        comboInfo[1][8, 2]!.Int32 = 245;
        comboInfo[1][8, 3]!.Int32 = 0;

        comboInfo[2][1]!.Int32 = 120;
        comboInfo[2][8, 1]!.Int32 = 255;
        comboInfo[2][8, 2]!.Int32 = 96;
        comboInfo[2][8, 3]!.Int32 = 0;

        comboInfo[3][1]!.Int32 = 300;
        comboInfo[3][8, 1]!.Int32 = 127;
        comboInfo[3][8, 2]!.Int32 = 255;
        comboInfo[3][8, 3]!.Int32 = 0;

        comboInfo[4][1]!.Int32 = 600;
        comboInfo[4][8, 1]!.Int32 = 0;
        comboInfo[4][8, 2]!.Int32 = 255;
        comboInfo[4][8, 3]!.Int32 = 226;

        comboInfo[5][1]!.Int32 = 1000;
        comboInfo[5][8, 1]!.Int32 = 0;
        comboInfo[5][8, 2]!.Int32 = 99;
        comboInfo[5][8, 3]!.Int32 = 255;

        comboInfo[6][1]!.Int32 = 2000;
        comboInfo[6][8, 1]!.Int32 = 135;
        comboInfo[6][8, 2]!.Int32 = 0;
        comboInfo[6][8, 3]!.Int32 = 255;

        comboInfo[7][1]!.Int32 = 4000;
        comboInfo[7][8, 1]!.Int32 = 255;
        comboInfo[7][8, 2]!.Int32 = 0;
        comboInfo[7][8, 3]!.Int32 = 110;

        comboInfo[8][1]!.Int32 = 6000;
        comboInfo[8][8, 1]!.Int32 = 255;
        comboInfo[8][8, 2]!.Int32 = 0;
        comboInfo[8][8, 3]!.Int32 = 0;

        comboInfo[9][1]!.Int32 = 8000;
        comboInfo[9][8, 1]!.Int32 = 237;
        comboInfo[9][8, 2]!.Int32 = 192;
        comboInfo[9][8, 3]!.Int32 = 192;

        modIO.OutputToFile(gameBigfileName);
    }

    public static void Modding2()
    {
        string v8FileName = @"D:\Games\SOR4_MOD_PROJ\v8\original";
        string xpFileName = @"D:\Games\SOR4_MOD_PROJ\v8\xp\Publish\xp-v1.1\bigfile";
        GameDataIO modIO = new(File.OpenRead(gameBigfileName), isCompressed: true);
        GameData mod = modIO.GameData;
        GameDataIO v8IO = new(File.OpenRead(v8FileName), isCompressed: true);
        GameData v8 = v8IO.GameData;
        GameDataIO xpIO = new(File.OpenRead(xpFileName), isCompressed: true);
        GameData xp = xpIO.GameData;

        FieldAddress characterAddress = new(MainKeys.CharacterData, "characters/sor4_playables/chrsor4shivaspecialforward");
        FieldAddress spriteAdress = new(MainKeys.AnimatedSpriteData, "animatedsprites/sor4/playables/sprsor4max_playable");

        (ProtoField move, ProtoField anime) FindMove(GameData gameData, FieldAddress characterAddress, FieldAddress spriteAddress, string moveName)
        {
            ProtoField move = new();
            ProtoField anime = new();

            foreach (ProtoField iMove in gameData[characterAddress]![99]!.Repeated)
            {
                if (iMove[1]!.String == moveName)
                {
                    move = iMove;
                    break;
                }
            }

            foreach (ProtoField iAnime in gameData[spriteAddress]![1]!.Repeated)
            {
                if (iAnime[1]!.String == moveName)
                {
                    anime = iAnime;
                    break;
                }
            }

            return (move, anime);
        }

        string moveName = "LariatAspire";

        (ProtoField moveMod, ProtoField animeMod) = FindMove(mod, characterAddress, spriteAdress, moveName);
        (ProtoField moveBase, ProtoField animeBase) = FindMove(v8, characterAddress, spriteAdress, moveName);

        moveMod.Message = moveBase.Message;
        animeMod.Message = animeBase.Message;

        mod[characterAddress]!.Message = v8[characterAddress]!.Message;

        //modIO.OutputToFile(outputFileName);
    }

    public static void Modding()
    {
        string v7FileName = @"D:\Games\SOR4_MOD_PROJ\v7\original";
        string v8FileName = @"D:\Games\SOR4_MOD_PROJ\v8\original";
        GameDataIO v7IO = new(File.OpenRead(v7FileName), isCompressed: true);
        GameDataIO modIO = new(File.OpenRead(v8FileName), isCompressed: true);

        List<FieldAddress> addresses =
        [
            new(MainKeys.CharacterData, @"characters/sor4_playables/chrsor4blaze"),
            new(MainKeys.CharacterData, @"characters/sor4_playables/chrsor4max_playable"),
            new(MainKeys.AnimatedSpriteData, @"animatedsprites/sor4/playables/sprsor4max_playable"),
            new(MainKeys.AnimatedSpriteData, @"animatedsprites/sor4/playables/sprsor4max_thunderattack"),
            new(MainKeys.CharacterData, @"characters/sor4_playables/chrsor4shiva"),
            new(MainKeys.CharacterData, @"characters/sor4_playables/chrsor4shivaairspecial1"),
            new(MainKeys.CharacterData, @"characters/sor4_playables/chrsor4shivaairspecial2"),
            new(MainKeys.CharacterData, @"characters/sor4_playables/chrsor4shivaspecialforward"),
            new(MainKeys.CharacterData, @"characters/sor4_playables/chrsor4shivaultra"),
            new(MainKeys.CharacterData, @"characters/sor4_enemies/shiva/chrsor4shivadouble"),
            new(MainKeys.CharacterData, @"characters/sor4_enemies/shiva/chrsor4shivadoublesurvival"),
            new(MainKeys.AnimatedSpriteData, @"animatedsprites/sor4/playables/sprsor4shiva_playable"),
            new(MainKeys.AnimatedSpriteData, @"animatedsprites/sor4/enemies/shiva/sprsor4shiva_double")
        ];
        Console.WriteLine("AnimatedSpriteIDs...");
        foreach (string id in modIO.GameData.AnimatedSpriteIDs)
        {
            if (id.Contains(@"animatedsprites/sor4/enemies/"))
            {
                addresses.Add(new(MainKeys.AnimatedSpriteData, id));
            }
        }
        Console.WriteLine("CharacterIDs...");
        foreach (string id in modIO.GameData.CharacterIDs)
        {
            if (id.Contains(@"characters/sor4_enemies/"))
            {
                addresses.Add(new(MainKeys.CharacterData, id));
            }
        }
        Console.WriteLine("Assign...");
        foreach (FieldAddress address in addresses)
        {
            modIO.GameData[address]!.Message = v7IO.GameData[address]?.Message ?? modIO.GameData[address]!.Message;
        }

        modIO.OutputToFile(gameBigfileName);
    }

    public static void FontFixToChinese()
    {
        Mvmb.ChangeMod(File.OpenRead(gameBigfileName), true);

        Message fontBoldMsg = 
        [
            (1, ProtoField.CreateString("br|de|en|es|fr|it|nl|pl|ru|uk|ja|ko|zh|zt|ztpc")),
            (2, ProtoField.CreateString("GUI/Fonts/NotoSans/NotoSansSC-Bold.otf")),
            (3, ProtoField.CreateFixed32(29.0f)),
            (4, ProtoField.CreateFixed32(0)),
        ];

        Message fontLightMsg =
        [
            (1, ProtoField.CreateString("br|de|en|es|fr|it|nl|pl|ru|uk|ja|ko|zh|zt|ztpc")),
            (2, ProtoField.CreateString("GUI/Fonts/NotoSans/NotoSansSC-Light.otf")),
            (3, ProtoField.CreateFixed32(29.0f)),
            (4, ProtoField.CreateFixed32(0)),
        ];

        List<FieldAddress> boldAddressList = 
        [
            new(@"MetaFont", @"gui/fonts/fntdefault", 7),
            new(@"MetaFont", @"gui/fonts/fntmenus", 7),
            new(@"MetaFont", @"gui/fonts/fntmenusreg", 7),
        ];

        FieldAddress lightAddress = new(@"MetaFont", @"gui/fonts/fntmenuslight", 7);

        foreach (var address in boldAddressList)
        {
            Repeated currentRep = Mvmb.Mod[address]!.UpgradeToRepeated().Repeated;
            currentRep.Clear();
            currentRep.Add(ProtoField.CreateMessage(fontBoldMsg));
        }

        Repeated lightRep = Mvmb.Mod[lightAddress]!.UpgradeToRepeated().Repeated;
        lightRep.Clear();
        lightRep.Add(ProtoField.CreateMessage(fontLightMsg));

        Mvmb.ModIO.OutputToFile(gameBigfileName);
    }

    public static void BinDisp(this byte b) => Console.Write("{0:00000000} ", int.Parse(Convert.ToString(b, 2)));

    public static void GenCharacterIDs(GameData gameData)
    {
        String[] ids = [.. gameData.CharacterIDs];
        Array.Sort(ids, StringComparer.CurrentCultureIgnoreCase);
        using StreamWriter writer = new(File.Create(@"D:\character_ids.txt"), leaveOpen: false);
        foreach (string id in ids)
        {
            writer.WriteLine(id);
        }
    }

    public static void GenSpriteIDs(GameData gameData)
    {
        String[] ids = [.. gameData.AnimatedSpriteIDs];
        Array.Sort(ids, StringComparer.CurrentCultureIgnoreCase);
        using StreamWriter writer = new(File.Create(@"D:\sprite_ids.txt"), leaveOpen: false);
        foreach (string id in ids)
        {
            writer.WriteLine(id);
        }
    }

    public static void GenPickupIDs(GameData gameData)
    {
        String[] ids = [.. gameData.PickupIDs];
        Array.Sort(ids, StringComparer.CurrentCultureIgnoreCase);
        using StreamWriter writer = new(File.Create(@"D:\pickup_ids.txt"), leaveOpen: false);
        foreach (string id in ids)
        {
            writer.WriteLine(id);
        }
    }

    public static void GenLevelIDs(GameData gameData)
    {
        String[] ids = [.. gameData.LevelIDs];
        Array.Sort(ids, StringComparer.CurrentCultureIgnoreCase);
        using StreamWriter writer = new(File.Create(@"D:\level_ids.txt"), leaveOpen: false);
        foreach (string id in ids)
        {
            writer.WriteLine(id);
        }
    }

    public static void GenDestroyableIDs(GameData gameData)
    {
        String[] ids = [.. gameData.DestroyableIDs];
        Array.Sort(ids, StringComparer.CurrentCultureIgnoreCase);
        using StreamWriter writer = new(File.Create(@"D:\destroyable_ids.txt"), leaveOpen: false);
        foreach (string id in ids)
        {
            writer.WriteLine(id);
        }
    }

    public static void GenFeedbackIDs(GameData gameData)
    {
        String[] ids = [.. gameData.FeedbackIDs];
        Array.Sort(ids, StringComparer.CurrentCultureIgnoreCase);
        using StreamWriter writer = new(File.Create(@"D:\feedback_ids.txt"), leaveOpen: false);
        foreach (string id in ids)
        {
            writer.WriteLine(id);
        }
    }
}
