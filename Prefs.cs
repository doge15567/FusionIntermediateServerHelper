
using Il2CppMK.Glow;
using Il2CppSLZ.Marrow.Warehouse;
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
            public static MelonPreferences_Entry<string> JSONStringListBlockedSpawnables;


            public static void RefreshMelonPrefs()
            {
                string defaultjson = JsonSerializer.Serialize(new string[2] { "stayx.weaponpack.Spawnable.NuclearBomb", "stayx.weaponpack.Spawnable.Pref" }, prettyPrint);
                loggingMode = GlobalCategory.GetEntry<int>("loggingMode") ?? GlobalCategory.CreateEntry("loggingMode", 1);
                JSONStringListBlockedSpawnables = GlobalCategory.GetEntry<string>("JSONStringListBlockedSpawnables") ?? GlobalCategory.CreateEntry("JSONStringListBlockedSpawnables", defaultjson);

                GlobalCategory.SaveToFile(false);
                MelonLog.Msg("Initalised prefs");
            }

            static JsonSerializerOptions prettyPrint = new JsonSerializerOptions { WriteIndented = true };
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


