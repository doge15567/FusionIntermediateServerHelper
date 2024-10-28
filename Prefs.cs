
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
            public static MelonPreferences_Entry<bool> RegexCheckEnabled;
            public static MelonPreferences_Entry<string> JSONStringListRegexBlockedBarcodes;
            public static MelonPreferences_Entry<PermissionLevel> SpawnBlockedSpawnablesAllowed;


            public static void RefreshMelonPrefs()
            {
                string blockedSpawnablesDefaultJson = JsonSerializer.Serialize(new string[2] { "stayx.weaponpack.Spawnable.NuclearBomb", "stayx.weaponpack.Spawnable.Pref" }, prettyPrint);
                string defaultStringArrayJson = JsonSerializer.Serialize(new string[2] { "", "" });

                loggingMode = GlobalCategory.GetEntry<int>("loggingMode") ?? GlobalCategory.CreateEntry("loggingMode", 1);
                
                SpawnableBlockingEnabled = GlobalCategory.GetEntry<bool>("SpawnableBlockingEnabled") ?? GlobalCategory.CreateEntry("SpawnableBlockingEnabled", false);
                JSONStringListBlockedSpawnables = GlobalCategory.GetEntry<string>("JSONStringListBlockedSpawnables") ?? GlobalCategory.CreateEntry("JSONStringListBlockedSpawnables", blockedSpawnablesDefaultJson);
                RegexCheckEnabled = GlobalCategory.GetEntry<bool>("Regex Check Enabled") ?? GlobalCategory.CreateEntry("Regex Check Enabled", false);
                JSONStringListRegexBlockedBarcodes = GlobalCategory.GetEntry<string>("JSON List of Regex to Block Barcodes") ?? GlobalCategory.CreateEntry("JSON List of Regex to Block Barcodes", defaultStringArrayJson, description: "List of Regex entries that spawned crate's barcode will be checked against. See above pref for json formatting. Strings need to be escaped properly, see https://www.csharpescaper.com/. Also, https://regex101.com/");
                SpawnBlockedSpawnablesAllowed = GlobalCategory.GetEntry<PermissionLevel>("Spawn Blocked Spawnables Allowed") ?? GlobalCategory.CreateEntry("Spawn Blocked Spawnables Allowed", PermissionLevel.DEFAULT);

                GlobalCategory.SaveToFile(false);
                MelonLog.Msg("Initalised prefs");
            }

            static readonly JsonSerializerOptions prettyPrint = new JsonSerializerOptions { WriteIndented = true };
            public static string[] GetJSONStringArrayPref(ref MelonPreferences_Entry<string> JSONStringPref)
            {
                var json = JSONStringPref.Value;
                var barcodes = JsonSerializer.Deserialize<string[]>(json);
                return barcodes;
            }
            public static void SetJSONStringArrayPref(ref MelonPreferences_Entry<string> JSONStringPref, string[] barcodes)
            {
                string json = JsonSerializer.Serialize(barcodes, prettyPrint);
                JSONStringPref.Value = json;
                GlobalCategory.SaveToFile(false);
            }
        }
    }
}


