using LabFusion.Player;
using UnityEngine;
using Page = BoneLib.BoneMenu.Page;
using BoneLib.BoneMenu;
using LabFusion.Network;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.Marrow.Pool;
using LabFusion.Representation;
using LabFusion.Entities;
using LabFusion.Utilities;

namespace FusionIntermediateServerHelper
{
    internal partial class Main
    {
        private static class BoneMenu
        {
            private static readonly Color mainColor = new(0.658824f, 0.321569f, 1f);
            private static readonly Color altColor = new(0.8784313725f, 0.7607843137f, 1);
            private static Page _mainCategory;
            private static Page _blockedItemsList;

            public static void Setup()
            {
                //MultiplayerHooking.OnLobbyCategoryCreated += OnLobbyCategoryMade; // FIX LATER

                _mainCategory = BoneLib.BoneMenu.Page.Root.CreatePage("Fusion Intermediate Server Helper", mainColor);

                Page SpawnableBlockingPage = _mainCategory.CreatePage("Spawnable Blocking", Color.white);
                SpawnableBlockingPage.CreateBoolPref("Enabled", Color.white, ref Prefs.SpawnableBlockingEnabled, prefName: "SpawnableBlockingEnabled");
                SpawnableBlockingPage.CreateEnumPref("Spawn Blocked Spawnables Allowed", Color.white, ref Prefs.SpawnBlockedSpawnablesAllowed);
                SpawnableBlockingPage.CreateBoolPref("Regex Check Enabled", Color.white, ref Prefs.RegexCheckEnabled);
                _blockedItemsList = SpawnableBlockingPage.CreatePage("Blocked Spawnables", Color.white);
                _blockedItemsList.CreateFunction("Refresh", Color.yellow, RefreshBlockedItems);
                SpawnableBlockingPage.CreateFunction("Add Spawnable to Blocklist from Spawn Gun (Left Hand)", Color.white, () =>
                {
                    string errortext = "";
                    try
                    {
                        if (BoneLib.Player.GetObjectInHand(BoneLib.Player.LeftHand) == null)
                        { errortext = "Error: Nothing in left hand."; throw new Exception(); }
                        SpawnGun spawngun = BoneLib.Player.GetComponentInHand<SpawnGun>(BoneLib.Player.LeftHand);
                        if (spawngun == null)
                        { errortext = "Error: Held object is not a Spawngun."; throw new Exception(); }

                        SpawnableCrate selectedCrate = spawngun._selectedCrate;
                        if (selectedCrate == null)
                        { errortext = "Error: Spawngun does not have a crate selected."; throw new Exception(); }

                        string barcode = selectedCrate.Barcode._id;
                        MelonLog.Msg($"spawnable from gun has id {barcode}");

                        var blockeds = Prefs.GetJSONStringArrayPref(ref Prefs.JSONStringListBlockedSpawnables);
                        if (!blockeds.Contains(barcode))
                        {
                            var newblockeds = blockeds.ToList();
                            newblockeds.Add(barcode);
                            Prefs.SetJSONStringArrayPref(ref Prefs.JSONStringListBlockedSpawnables, newblockeds.ToArray());
                            var title = TryGetTitleFromBarcode(barcode);
                            BoneMenuNotif(BoneLib.Notifications.NotificationType.Success, $"Added {title} to blocklist!", 1.5f);
                        }
                        else
                        { errortext = "Error: The spawnable selected is already blocked."; throw new Exception(); }

                    }
                    catch (Exception)
                    {
                        BoneMenuNotif(BoneLib.Notifications.NotificationType.Error, errortext, .5f);
                    }
                });
                SpawnableBlockingPage.CreateFunction("Add Spawnable in hand to Blocklist", Color.white, () =>
                {
                    string errortext = "";
                    try
                    {
                        if (BoneLib.Player.GetObjectInHand(BoneLib.Player.LeftHand) == null)
                        { errortext = "Error: Nothing in left hand."; throw new Exception(); }
                        if (BoneLib.Player.GetComponentInHand<Poolee>(BoneLib.Player.LeftHand).SpawnableCrate == null)
                        { errortext = "Error: Object is not a spawnable, or is a prefab."; throw new Exception(); }

                        string barcode = BoneLib.Player.GetComponentInHand<Poolee>(BoneLib.Player.LeftHand).SpawnableCrate.Barcode.ID;

                        var blockeds = Prefs.GetJSONStringArrayPref(ref Prefs.JSONStringListBlockedSpawnables);
                        if (!blockeds.Contains(barcode))
                        {
                            var newblockeds = blockeds.ToList();
                            newblockeds.Add(barcode);
                            Prefs.SetJSONStringArrayPref(ref Prefs.JSONStringListBlockedSpawnables, newblockeds.ToArray());
                            var title = TryGetTitleFromBarcode(barcode);
                            BoneMenuNotif(BoneLib.Notifications.NotificationType.Success, $"Added {title} to blocklist!", 1.5f);
                        }
                        else
                        { errortext = "Error: The spawnable selected is already blocked."; throw new Exception(); }
                    }
                    catch (Exception)
                    {
                        BoneMenuNotif(BoneLib.Notifications.NotificationType.Success, errortext);
                    }
                });
                SpawnableBlockingPage.CreateString("Add Spawnable to Blocklist from string", Color.white, "", (barcode) =>
                {
                    var blockeds = Prefs.GetJSONStringArrayPref(ref Prefs.JSONStringListBlockedSpawnables);
                    if (!blockeds.Contains(barcode))
                    {
                        var newblockeds = blockeds.ToList();
                        newblockeds.Add(barcode);
                        Prefs.SetJSONStringArrayPref(ref Prefs.JSONStringListBlockedSpawnables, newblockeds.ToArray());
                        var title = TryGetTitleFromBarcode(barcode);
                        BoneMenuNotif(BoneLib.Notifications.NotificationType.Success, $"Added {title} to blocklist!", 1.5f);
                    }
                    else
                    { BoneMenuNotif(BoneLib.Notifications.NotificationType.Success, "Error: The spawnable selected is already blocked.", 1.5f); }
                });

                _mainCategory.CreateFunction("Despawn All", Color.white, () => Menu.DisplayDialog("Despawn All Confirmation", "Despawn all spawnables within the server?", confirmAction: () =>
                    {
                        if (NetworkInfo.HasServer)
                        {
                            if (NetworkInfo.IsServer)
                            {
                                // Loop through all NetworkProps and despawn them
                                var entities = NetworkEntityManager.IdManager.RegisteredEntities.EntityIdLookup.Keys.ToArray();
                                foreach (var networkEntity in entities)
                                {


                                    var prop = networkEntity.GetExtender<NetworkProp>();

                                    if (prop == null)
                                    {
                                        Msg("Did not have NetworkProp, leaving this part of loop!", 2);
                                        continue;
                                    }

                                    var poolee = networkEntity.GetExtender<PooleeExtender>();

                                    if (poolee == null)
                                    {
                                        Msg("Did not have PooleeExtender, leaving this part of loop!", 2);
                                        continue;
                                    }

                                    var player = networkEntity.GetExtender<NetworkPlayer>();

                                    if (player != null)
                                    {
                                        Msg("Did have NetworkPlayer, leaving this part of loop!", 2);
                                        continue;
                                    }

                                    PooleeUtilities.RequestDespawn(networkEntity.Id, false); // use RequestDespawn to despawn without particles, SendDespawn did not work.
                                }
                            }
                            else BoneMenuNotif(BoneLib.Notifications.NotificationType.Warning, "Is not host");
                        }
                        else BoneMenuNotif(BoneLib.Notifications.NotificationType.Error, "Is not in server");
                    })); // From https://github.com/Lakatrazz/BONELAB-Fusion/blob/9e2be403cc689598548fcf3e862d02e3781336d9/LabFusion/src/Utilities/PooleeUtilities.cs#L13

                _mainCategory.CreateBoolPref("Disable DevTools Cleanup", Color.white, ref Prefs.DisableDevToolsCleanup);
            }

            public static void RefreshBlockedItems()
            {
                _blockedItemsList.RemoveAll();
                _blockedItemsList.CreateFunction("Refresh", Color.yellow, RefreshBlockedItems);

                var blockeds = Prefs.GetJSONStringArrayPref(ref Prefs.JSONStringListBlockedSpawnables);

                foreach (string barcode in blockeds)
                {
                    var title = TryGetTitleFromBarcode(barcode);

                    var spawnablePage = _blockedItemsList.CreatePage(title, Color.white);
                    spawnablePage.CreateFunction($"Barcode: {barcode}", Color.white, null);
                    spawnablePage.CreateFunction("Unblock", Color.cyan, () =>
                    {
                        var blockeds = Prefs.GetJSONStringArrayPref(ref Prefs.JSONStringListBlockedSpawnables);
                        if (blockeds.Contains(barcode))
                        {
                            var newblockeds = blockeds.ToList();
                            if (newblockeds.Remove(barcode))
                                Prefs.SetJSONStringArrayPref(ref Prefs.JSONStringListBlockedSpawnables, newblockeds.ToArray());
                        }

                        Menu.OpenPage(_blockedItemsList);
                        RefreshBlockedItems();
                    });
                }
            }

            public static void CreateNotifPage(Page page, PlayerId id, string barcode, string spawnableName, string playerName)
            {
                // create function element with spawnable blocked, option to kick player, option to unblock spawnable.
                // Fusion automaticaly creates dismiss and message

                page.CreateFunction($"Spawnable Blocked: {spawnableName}", Color.white, null);
                page.CreateFunction($"Player: {playerName}", Color.white, null);
                page.CreateFunction("Unblock Spawnable", Color.green, () =>
                {
                    Menu.DisplayDialog(
                        $"Remove {spawnableName} from blocked spawnables?",
                        $"Are you sure you want to unblock spawnable \"{spawnableName}\" from being spawned in?",
                        confirmAction:
                        () =>
                        {
                            var blockeds = Prefs.GetJSONStringArrayPref(ref Prefs.JSONStringListBlockedSpawnables);
                            if (blockeds.Contains(barcode))
                            {
                                var newblockeds = blockeds.ToList();
                                if (newblockeds.Remove(barcode))
                                    Prefs.SetJSONStringArrayPref(ref Prefs.JSONStringListBlockedSpawnables, newblockeds.ToArray());
                            }
                        }
                    );
                });
                page.CreateFunction("Kick Player", Color.red, () =>
                {
                    Menu.DisplayDialog(
                        $"Kick {playerName} for spawning {spawnableName}?",
                        $"Are you sure you want to kick the player {playerName} for spawning in blocked spawnable {spawnableName}?",
                        confirmAction:
                        () =>
                        {
                            NetworkHelper.KickUser(id);
                        }
                    );
                });


            }

            private static void OnLobbyCategoryMade(Page category, INetworkLobby lobby) // From https://github.com/notnotnotswipez/ModioModNetworker/blob/ab0fd8d56305fb870ac81e703226f2dd050df95b/ModioModNetworker/MainClass.cs#L206
            {
                if (lobby.TryGetMetadata("fishassisted", out _))
                {
                    category.CreateFunction("This server is F.I.S.H assisted!", mainColor, null);
                }
                if (lobby.TryGetMetadata("fishversion", out var metaverplus))
                {
                    category.CreateFunction("Version: "+metaverplus, altColor, null);
                }

            }
            public static void BoneMenuNotif(BoneLib.Notifications.NotificationType type, string content, float popuplength = 2)
            {
                var notif = new BoneLib.Notifications.Notification
                {
                    Title = "FusionIntermediateServerHelper",
                    Message = content,
                    Type = type,
                    PopupLength = popuplength,
                    ShowTitleOnPopup = true
                };
                BoneLib.Notifications.Notifier.Send(notif);

                Main.Msg("Sent notification \"" + content + "\"", 1);

            }
        }

    }



}
