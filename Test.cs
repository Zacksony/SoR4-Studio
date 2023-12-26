using SoR4_Studio.Modules.DataModel.GameDataModel;
using System;
using System.IO;

namespace SoR4_Studio;

internal static class Test
{
    public static void Main()
    {
        //GameDataIO io = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("SoR4_Studio.Private.r18163.bin")!, false);

        //foreach (string decorID in io.GameData.DecorIDs)
        //{
        //    DecorData.DecorDataClass decorData = io.GameData.DecorData[decorID];
        //    Console.WriteLine(decorID);
        //    foreach (var area in decorData.Areas)
        //    {
        //        foreach ((int x, int y) in area)
        //        {
        //            Console.WriteLine($"{x}, {y}");
        //        }

        //        Console.WriteLine("-----------------------");
        //    }
        //}

        //int i = 0;
        //int total = io.GameData.TotalChunkCount;
        //foreach (var pair1 in io.GameData.Chunks)
        //{
        //    foreach (var pair2 in pair1.Value)
        //    {
        //        _ = pair2.Value.Root;
        //        Console.WriteLine($"({++i}/{total}) {pair1.Key}:{pair2.Key}");
        //    }
        //}

        // Save
        //string path = @"D:\SteamLibrary\steamapps\common\Streets of Rage 4\data\bigfile";
        //io.Save(path, true);
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
