
using MelonLoader;
using Il2CppSLZ.Props;
using Il2CppSLZ.Rig;
using UnityEngine;
using System;
using Il2CppInterop.Runtime.Injection;
using BoneLib.BoneMenu;
using Page = BoneLib.BoneMenu.Page;

using LabFusion;
using LabFusion.Network;
using LabFusion.Player;

using HarmonyLib;
using LabFusion.BoneMenu;
using LabFusion.Extensions;
using LabFusion.Representation;
using System.Data.Common;
using Il2CppSLZ.Marrow;
using LabFusion.Entities;
using Steamworks.Ugc;
using System.Xml.Linq;
using LabFusion.Grabbables;
using LabFusion.Utilities;
using MelonLoader.ICSharpCode.SharpZipLib.GZip;
using LabFusion.Marrow.Integration;
using UnityEngine.Playables;
using Il2CppSLZ.Marrow.Input;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.PuppetMasta;
using System.Reflection;
using Il2CppSLZ.Marrow.LateReferences;
using System.Text;
using static Il2CppSLZ.Marrow.PuppetMasta.Muscle;
using Il2CppSLZ.Marrow.Warehouse;
using static Il2CppSystem.Globalization.CultureInfo;
//using Il2CppSLZ.Bonelab;


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
        public static string TryGetTitleFromBarcode(string barcode)
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
            /*
            if (Prefs.loggingMode.Value < 1) return;
            UnityEngine.GUILayout.Label($"------------ Fusion Sanctity ------------", emptyOptions);
            UnityEngine.GUILayout.Label($"Object despawn list size: {GenericUtils.objectQueue.Count}", emptyOptions);
            UnityEngine.GUILayout.Label($"Laser Mode: {LaserUtils.mode.ToString()}", emptyOptions);
            UnityEngine.GUILayout.Label($"Did hit?: {LaserUtils.didHit.ToString()}", emptyOptions);
            UnityEngine.GUILayout.Label($"Collider Hit: {LaserUtils.hit.collider?.name}", emptyOptions);
            UnityEngine.GUILayout.Label($"MarrowBody Hit: {LaserUtils.bodyHit?.name}", emptyOptions);
            UnityEngine.GUILayout.Label($"MarrowEntity Hit: {LaserUtils.entityHit?.name}", emptyOptions);
            UnityEngine.GUILayout.Label($"RigManager Hit: {LaserUtils.rigManagerHit?.name}", emptyOptions);
            UnityEngine.GUILayout.Label($"Player Hit: {LaserUtils.playerHit?.Username}", emptyOptions);
            UnityEngine.GUILayout.Label($"-----------------------------------------", emptyOptions);
            //UnityEngine.GUILayout.Label($"thing: {}", emptyOptions);
            */

        }
    }
}

// Controller stuff taken from spiderlab
public static class ControllerInfo
{
    // Token: 0x0600000E RID: 14 RVA: 0x00002420 File Offset: 0x00000620
    public static bool GetTouchpadTouch(Handedness hand)
    {
        XRController xrcontroller;
        if (!ControllerInfo.TryGetXRController(hand, out xrcontroller))
        {
            return false;
        }
        bool flag;
        if (xrcontroller.Type == XRControllerType.OculusTouch && xrcontroller._xrDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.thumbrest, out flag))
        {
            return flag;
        }
        return xrcontroller.TouchpadTouch;
    }
    public static float GetGrip(Handedness hand)
    {
        XRController xrcontroller;
        if (!ControllerInfo.TryGetXRController(hand, out xrcontroller))
        {
            return -1f;
        }
        float flag;
        if (xrcontroller._xrDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.grip, out flag))
        {
            return flag;
        }
        return xrcontroller.Grip;
    }
    public static bool GetGripButton(Handedness hand)
    {
        XRController xrcontroller;
        if (!ControllerInfo.TryGetXRController(hand, out xrcontroller))
        {
            return false;
        }
        bool flag;
        if (xrcontroller._xrDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out flag))
        {
            return flag;
        }
        return xrcontroller.GripButton;
    }
    public static bool GetTriggerButton(Handedness hand)
    {
        XRController xrcontroller;
        if (!ControllerInfo.TryGetXRController(hand, out xrcontroller))
        {
            return false;
        }
        bool flag;
        if (xrcontroller._xrDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out flag))
        {
            return flag;
        }
        return xrcontroller.TriggerButton;
    }


    // Token: 0x0600000F RID: 15 RVA: 0x00002460 File Offset: 0x00000660
    public static bool GetFaceButtonsTouch(Handedness hand)
    {
        XRController xrcontroller;
        return ControllerInfo.TryGetXRController(hand, out xrcontroller) && xrcontroller.ATouch && xrcontroller.BTouch;
    }

    // Token: 0x06000010 RID: 16 RVA: 0x0000248C File Offset: 0x0000068C
    public static float GetTrigger(Handedness hand)
    {
        XRController xrcontroller;
        if (!ControllerInfo.TryGetXRController(hand, out xrcontroller))
        {
            return 0f;
        }
        return xrcontroller.Trigger;
    }

    // Token: 0x06000011 RID: 17 RVA: 0x000024B0 File Offset: 0x000006B0
    private static bool TryGetXRController(Handedness hand, out XRController controller)
    {
        if (hand == Handedness.LEFT)
        {
            controller = MarrowGame.xr.LeftController;
            return true;
        }
        if (hand != Handedness.RIGHT)
        {
            MelonLogger.Error("Invalid hand type: " + hand.ToString());
            controller = null;
            return false;
        }
        controller = MarrowGame.xr.RightController;
        return true;
    }

    

}
