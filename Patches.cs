
using LabFusion.Network;
using HarmonyLib;
using LabFusion.Exceptions;
using LabFusion.Utilities;
using LabFusion.Player;
using LabFusion.Extensions;
using Il2CppSLZ.Marrow.Warehouse;
using LabFusion.Representation;
using System.Text.RegularExpressions;
using LabFusion.Entities;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;

namespace FusionIntermediateServerHelper
{
    internal partial class Main
    {
        private partial class Patches
        {
            [HarmonyPatch(typeof(SpawnRequestMessage))]
            public static class SpawnRequestMessagePatch
            {
                [HarmonyPrefix]
                [HarmonyPatch(nameof(SpawnRequestMessage.HandleMessage))]
                public static bool Prefix(byte[] bytes, bool isServerHandled = false)
                {
                    if (Prefs.SpawnableBlockingEnabled.Value == false) return true;
                    if (!isServerHandled)
                    {
                        return true; // allow original method to handle error
                    }
#if DEBUG
                    Msg("Patch of handling message");
#endif

                    using var reader = FusionReader.Create(bytes);
                    var data = reader.ReadFusionSerializable<SpawnRequestData>();

                    bool matched = false;

                    #region Blocklist Check
                    string[] blockedCodes = Prefs.GetJSONStringArrayPref( ref Prefs.JSONStringListBlockedSpawnables);
#if DEBUG
                    Msg($"Checking {data.barcode} against blocklist");
#endif
                    if (!matched) matched = matched||blockedCodes.Contains(data.barcode);
#if DEBUG
                    Msg($"Matched: {matched.ToString()}");
#endif
                    #endregion
                    
                    #region Rexex Check
                    // Implement Regex checking
                    if (!matched && Prefs.RegexCheckEnabled.Value)
                    {
#if DEBUG
                        Msg("Checking against regex list");
#endif
                        string[] sbRegexEntries = Prefs.GetJSONStringArrayPref(ref Prefs.JSONStringListRegexBlockedBarcodes);
                        foreach (string strRegex in sbRegexEntries)
                        {
#if DEBUG
                            Msg("Regex: "+strRegex);
#endif
                            Regex regex = new(strRegex);

                            var iMatch = regex.IsMatch(data.barcode);

                            matched = matched || iMatch;
                            if (matched) break;
                        }
                    }
                    #endregion


                    if (matched)
                    {
#if DEBUG
                        Msg("If Matched");
#endif
                        var playerId = PlayerIdManager.GetPlayerId(data.owner);

                        #region Permissions
                        FusionPermissions.FetchPermissionLevel(playerId.LongId, out var playerPermissionLevel, out _);

                        bool wasAllowed;
                        if (Prefs.SpawnBlockedSpawnablesAllowed.Value == PermissionLevel.DEFAULT)
                        {
#if DEBUG
                            Msg("Case Permission level default");
#endif
                            wasAllowed = false; // use DEFAULT as none
                        }
                        else
                        {
                            wasAllowed = FusionPermissions.HasSufficientPermissions(playerPermissionLevel, Prefs.SpawnBlockedSpawnablesAllowed.Value);
                        }
#if DEBUG
                        Msg("wasAllowed = "+wasAllowed.ToString());
#endif
                        #endregion

                        //Fusion code for getting cool name thingy
                        #region Player Name Display
                        string username = playerId.Metadata.GetMetadata(MetadataHelper.UsernameKey);
                        string nickname = playerId.Metadata.GetMetadata(MetadataHelper.NicknameKey);

                        username = username.LimitLength(PlayerIdManager.MaxNameLength);
                        nickname = nickname.LimitLength(PlayerIdManager.MaxNameLength);

                        string display;

                        if (string.IsNullOrWhiteSpace(nickname))
                            display = username;
                        else
                            display = $"{nickname} ({username})";
                        #endregion

                        var Title = TryGetTitleFromBarcode(data.barcode);

                        //var position = data.serializedTransform.position.ToString();

                        // display, Title and position

                        var allowedText = wasAllowed ? "was allowed." : "wasn't allowed!";

                        var message = $"Player {display} attempted to spawn blocked {Title} and {allowedText}";
                        FusionNotifier.Send(new FusionNotification()
                        {
                            type = NotificationType.WARNING,
                            isMenuItem = true,
                            isPopup = !wasAllowed,
                            message = message,
                            popupLength = 1,
                            title = "F.I.S.H Spawn Block",
                            showTitleOnPopup = true,
                            onCreateCategory = (page) => BoneMenu.CreateNotifPage(page, playerId, data.barcode, Title, display),
                        });
                        return wasAllowed;
                    }
                    return true;
                }
            }

            [HarmonyPatch(typeof(LabFusion.Entities.SpawnGunExtender))]
            public static class SpawnGunOnRegisterPatch
            {
                [HarmonyPrefix]
                [HarmonyPatch("OnRegister")]

                public static bool Prefix(NetworkEntity networkEntity, SpawnGun component)
                {
                    if (Prefs.DisableDevToolsCleanup.Value)
                    {
#if DEBUG
                        Msg("Prevented creation of TimedDeletion for SpawnGun!");
#endif
                        SpawnGunExtender.Cache.Add(component, networkEntity);
                        return false;
                    }
                    return true;

                }

            }
            [HarmonyPatch(typeof(LabFusion.Entities.FlyingGunExtender))]
            public static class FlyingGunOnRegisterPatch
            {
                [HarmonyPrefix]
                [HarmonyPatch("OnRegister")]

                public static bool Prefix(NetworkEntity networkEntity, FlyingGun component)
                {
                    if (Prefs.DisableDevToolsCleanup.Value)
                    {
#if DEBUG
                        Msg("Prevented creation of TimedDeletion for Nimbus Gun!");
#endif
                        FlyingGunExtender.Cache.Add(component, networkEntity);
                        return false;
                    }
                    return true;

                }

            }

            //Implement SpawnGun Despawn remover feature https://github.com/search?q=repo%3ALakatrazz%2FBONELAB-Fusion+_despawnHandler&type=code

        }

    }
}
