using MelonLoader;
using UnityEngine;
using Il2CppSLZ.Marrow.Warehouse;

namespace FusionIntermediateServerHelper
{
    internal partial class Main : MelonMod
    {
        internal const string Name = "Fusion Intermediate Server Helper";
        internal const string Description = "\"I,       am steve.\"";
        internal const string Author = "doge15567";
        internal const string Company = "";
        internal const string Version = "0.0.1";
        internal const string DownloadLink = "Link";
        internal static MelonLogger.Instance MelonLog;
        internal static Main Instance;

        public override void OnEarlyInitializeMelon()
        {
            Instance = this;
            MelonLog = LoggerInstance;
        }
        public override void OnInitializeMelon()
        {
            //GenericUtils.ShowPopupBox("Test!");
            MelonLog.Msg("Initalising Mod");
            Prefs.RefreshMelonPrefs();
            BoneMenu.Setup();
            MelonLog.Msg("Initialised Mod");
        }
        public static void Msg(string message, int loggingMode = 0)
        {
            if (loggingMode <= Prefs.loggingMode.Value)
            {
                MelonLog.Msg(message);
            }
        }
        public static string TryGetTitleFromBarcode(string barcode) // ik a "TryGet" function is probably supposed to return a bool and have an out var but idkkkkkk
        {
            var Title = barcode;
            Crate e;

            if (AssetWarehouse.Instance.TryGetCrate(new Barcode() { ID = barcode }, out e))
            {
                Title = e.Title;
            }
            else
            {
                char[] charArray = barcode.ToCharArray();
                Array.Reverse(charArray);
                var reverseBarcode = new string(charArray);
                string stringBeforeChar = reverseBarcode.Substring(0, reverseBarcode.IndexOf("."));
                char[] charArray2 = stringBeforeChar.ToCharArray();
                Array.Reverse(charArray2);
                var unReversedBarcodeTitleThing = new string(charArray2);


                Title = unReversedBarcodeTitleThing;
            }
            
            return Title;
        }

        public static GUILayoutOption[] emptyOptions = Array.Empty<GUILayoutOption>();

        public override void OnGUI()
        {
        }
    }
}
