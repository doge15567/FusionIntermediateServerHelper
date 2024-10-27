
using Il2CppMK.Glow;
using Il2CppSLZ.Marrow.Warehouse;
using LabFusion.Representation;
using MelonLoader;
using MelonLoader.TinyJSON;
using System.Text.Json;
//using Il2CppSLZ.Bonelab;


namespace FusionIntermediateServerHelper
{
    internal partial class Main
    {
        public static class Prefs
        {
            public static readonly MelonPreferences_Category GlobalCategory = MelonPreferences.CreateCategory("FusionIntermediateServerHelper");
            public static MelonPreferences_Entry<int> loggingMode;
            public static MelonPreferences_Entry<bool> SpawnableBlockingEnabled;
            public static MelonPreferences_Entry<string> JSONStringListBlockedSpawnables;
            public static MelonPreferences_Entry<PermissionLevel> SpawnBlockedSpawnablesAllowed;


            public static void RefreshMelonPrefs()
            {
                string defaultjson = JsonSerializer.Serialize(new string[2] { "stayx.weaponpack.Spawnable.NuclearBomb", "stayx.weaponpack.Spawnable.Pref" }, prettyPrint);

                loggingMode = GlobalCategory.GetEntry<int>("loggingMode") ?? GlobalCategory.CreateEntry("loggingMode", 1);
                
                SpawnableBlockingEnabled = GlobalCategory.GetEntry<bool>("SpawnableBlockingEnabled") ?? GlobalCategory.CreateEntry("SpawnableBlockingEnabled", false);
                JSONStringListBlockedSpawnables = GlobalCategory.GetEntry<string>("JSONStringListBlockedSpawnables") ?? GlobalCategory.CreateEntry("JSONStringListBlockedSpawnables", defaultjson);
                SpawnBlockedSpawnablesAllowed = GlobalCategory.GetEntry<PermissionLevel>("Spawn Blocked Spawnables Allowed") ?? GlobalCategory.CreateEntry("Spawn Blocked Spawnables Allowed", PermissionLevel.DEFAULT);

                GlobalCategory.SaveToFile(false);
                MelonLog.Msg("Initalised prefs");
            }

            static readonly JsonSerializerOptions prettyPrint = new JsonSerializerOptions { WriteIndented = true };
            public static string[] GetBlockedBarcodes()
            {
                var json = JSONStringListBlockedSpawnables.Value;
                var barcodes = JsonSerializer.Deserialize<string[]>(json);
                return barcodes;
            }
            public static void SetBlockedBarcodes(string[] barcodes)
            {
                string json = JsonSerializer.Serialize(barcodes, prettyPrint);
                JSONStringListBlockedSpawnables.Value = json;
                GlobalCategory.SaveToFile(false);
            }
        }
    }
}


