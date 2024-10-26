
using LabFusion.Network;
using HarmonyLib;
using LabFusion.Exceptions;
using LabFusion.Utilities;
using LabFusion.Player;
using LabFusion.Extensions;
using Il2CppSLZ.Marrow.Warehouse;

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

                    var playerId = PlayerIdManager.GetPlayerId(data.owner);
                    
                    if (playerId == PlayerIdManager.LocalId) return true; // replace when perm system is in

                    string[] blockedCodes = Prefs.GetBlockedBarcodes();
                    if (blockedCodes.Contains(data.barcode))
                    {
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

                        var position = data.serializedTransform.position.ToString();

                        // display, Title and position


                        var message = $"Player {display} attempted to spawn blocked {Title} at {position}!";
                        var x = new FusionNotification() 
                        {
                            type = NotificationType.WARNING, 
                            isMenuItem = true, 
                            isPopup = true,
                            message = message,
                            popupLength = 2,
                            title = "F.I.S.H Spawn Block",
                            showTitleOnPopup = true,
                            onCreateCategory = (page) => BoneMenu.CreateNotifPage(page, playerId, data.barcode, Title, display),
                        };
                        FusionNotifier.Send(x);
                        return false;
                    }
                    return true;
                }
            }
        }

    }
}
