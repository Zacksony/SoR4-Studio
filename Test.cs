using SoR4_Studio.Modules.DataModel.GameDataModel;
using SoR4_Studio.Modules.DataModel.GameDataModel.FieldDescriber;
using SoR4_Studio.Modules.ViewModel;
using System;
using System.IO;
using Mvmb = SoR4_Studio.Modules.ViewModel.ModdingViewModelBase;

namespace SoR4_Studio;

internal static class Test
{
    public static void Main()
    {
        string fileName = @"D:\SteamLibrary\steamapps\common\Streets of Rage 4\data\bigfile";
        Mvmb.ChangeMod(File.OpenRead(fileName), true);
        FieldAddress address = new(@"MetaFont", @"gui/fonts/fntmenuslight", 7, 0, 2);
        Mvmb.Mod[address]!.String = @"GUI/Fonts/NotoSans/NotoSansSC-Bold.otf";
        Mvmb.ModIO.OutputToFile(fileName);
    }

    public static void BinDisp(this byte b) => Console.Write("{0:00000000} ", int.Parse(Convert.ToString(b, 2)));

    public static void GenCharacterIDs(GameData gameData)
    {
        String[] ids = [.. gameData.CharacterIDs];
        Array.Sort(ids, StringComparer.CurrentCultureIgnoreCase);
        using StreamWriter writer = new(File.Create(@"D:\character_ids.txt"), leaveOpen: false);
        foreach (string id in ids)
        {
            string simpleID = id
                .Replace("characters/", "")
                .Replace("_enemies/", ".enemies.")
                .Replace("_playables/", ".playables.")
                .Replace("chrsor1_", "")
                .Replace("chrsor2_", "")
                .Replace("chrsor3_", "")
                .Replace("chrsor4_", "")
                .Replace("chrsor1", "")
                .Replace("chrsor2", "")
                .Replace("chrsor3", "")
                .Replace("chrsor4", "")
                .Replace("/", ":")
                ;
            writer.WriteLine($"            [\"{id}\"] = \"{simpleID}\",");
        }
    }

    public static void GenPickupIDs(GameData gameData)
    {
        String[] ids = [.. gameData.PickupIDs];
        Array.Sort(ids, StringComparer.CurrentCultureIgnoreCase);
        using StreamWriter writer = new(File.Create(@"D:\pickup_ids.txt"), leaveOpen: false);
        foreach (string id in ids)
        {
            string simpleID = id
                .Replace("objects/pickup_", "")
                .Replace("objects/survival/pickup_", "survival_")
                ;
            writer.WriteLine($"            [\"{id}\"] = \"{simpleID}\",");
        }
    }

    public static void GenLevelIDs(GameData gameData)
    {
        String[] ids = [.. gameData.LevelIDs];
        Array.Sort(ids, StringComparer.CurrentCultureIgnoreCase);
        using StreamWriter writer = new(File.Create(@"D:\level_ids.txt"), leaveOpen: false);
        foreach (string id in ids)
        {
            string simpleID = id
                .Replace("levels/", "")
                .Replace("main_campaign/lvl_", "")
                .Replace("stage1_", "Stage.01.")
                .Replace("stage2_", "Stage.02.")
                .Replace("stage3_", "Stage.03.")
                .Replace("stage4_", "Stage.04.")
                .Replace("stage5_", "Stage.05.")
                .Replace("stage6_", "Stage.06.")
                .Replace("stage7_", "Stage.07.")
                .Replace("stage8_", "Stage.08.")
                .Replace("stage9_", "Stage.09.")
                .Replace("stage10_", "Stage.10.")
                .Replace("stage11_", "Stage.11.")
                .Replace("stage12_", "Stage.12.")
                ;
            writer.WriteLine($"            [\"{id}\"] = \"{simpleID}\",");
        }
    }

    public static void GenDestroyableIDs(GameData gameData)
    {
        String[] ids = [.. gameData.DestroyableIDs];
        Array.Sort(ids, StringComparer.CurrentCultureIgnoreCase);
        using StreamWriter writer = new(File.Create(@"D:\destroyable_ids.txt"), leaveOpen: false);
        foreach (string id in ids)
        {
            string simpleID = id
                .Replace("objects/object_", "")
                .Replace("objects/survival/object_", "survival_");
            ;
            writer.WriteLine($"            [\"{id}\"] = \"{simpleID}\",");
        }
    }

    public static void GenFeedbackIDs(GameData gameData)
    {
        String[] ids = [.. gameData.FeedbackIDs];
        Array.Sort(ids, StringComparer.CurrentCultureIgnoreCase);
        using StreamWriter writer = new(File.Create(@"D:\feedback_ids.txt"), leaveOpen: false);
        foreach (string id in ids)
        {
            string simpleID = id
                .Replace("feedbacks/", "")
                ;
            writer.WriteLine($"            [\"{id}\"] = \"{simpleID}\",");
        }
    }
}
