
using LabFusion.Network;
using HarmonyLib;
using LabFusion.Exceptions;
using LabFusion.Utilities;
using LabFusion.Player;
using LabFusion.Extensions;
using Il2CppSLZ.Marrow.Warehouse;
using LabFusion.Representation;

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

                    using var reader = FusionReader.Create(bytes);
                    var data = reader.ReadFusionSerializable<SpawnRequestData>();

                    string[] blockedCodes = Prefs.GetBlockedBarcodes();
                    if (blockedCodes.Contains(data.barcode))
                    {
                        
                        var playerId = PlayerIdManager.GetPlayerId(data.owner);

                        if (playerId == PlayerIdManager.LocalId) return true; // Kept this, as I still log when someone spawns a blocked spawnable, just no notif and dif text

                        FusionPermissions.FetchPermissionLevel(playerId.LongId, out var playerPermissionLevel, out _);

                        var wasAllowed = FusionPermissions.HasSufficientPermissions(playerPermissionLevel, Prefs.SpawnBlockedSpawnablesAllowed.Value);

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
        }

    }
}
