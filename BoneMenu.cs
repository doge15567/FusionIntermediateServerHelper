using LabFusion.Player;
using UnityEngine;
using Page = BoneLib.BoneMenu.Page;
using BoneLib.BoneMenu;
using LabFusion.Network;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.Marrow.Pool;

namespace FusionIntermediateServerHelper
{
    internal partial class Main
    {
        private static class BoneMenu
        {
            private static readonly Color mainColor = new(0.658824f, 0.321569f, 1f);
            private static Page _mainCategory;
            private static Page _blockedItemsList;

            public static void Setup()
            {
                _mainCategory = BoneLib.BoneMenu.Page.Root.CreatePage("Fusion Intermediate Server Helper", mainColor);

                Page SpawnableBlockingPage = _mainCategory.CreatePage("Spawnable Blocking", Color.white);
                SpawnableBlockingPage.CreateBool("Enabled", Color.white, Prefs.SpawnableBlockingEnabled.Value, (val) => { Prefs.SpawnableBlockingEnabled.Value = val; Prefs.GlobalCategory.SaveToFile(false); });
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
                        
                        var blockeds = Prefs.GetBlockedBarcodes();
                        if (!blockeds.Contains(barcode))
                        {
                            var newblockeds = blockeds.ToList();
                            newblockeds.Add(barcode);
                            Prefs.SetBlockedBarcodes(newblockeds.ToArray());
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

                        var blockeds = Prefs.GetBlockedBarcodes();
                        if (!blockeds.Contains(barcode))
                        {
                            var newblockeds = blockeds.ToList();
                            newblockeds.Add(barcode);
                            Prefs.SetBlockedBarcodes(newblockeds.ToArray());
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
                    var blockeds = Prefs.GetBlockedBarcodes();
                    if (!blockeds.Contains(barcode))
                    {
                        var newblockeds = blockeds.ToList();
                        newblockeds.Add(barcode);
                        Prefs.SetBlockedBarcodes(newblockeds.ToArray());
                        var title = TryGetTitleFromBarcode(barcode);
                        BoneMenuNotif(BoneLib.Notifications.NotificationType.Success, $"Added {title} to blocklist!", 1.5f);
                    }
                    else
                    { BoneMenuNotif(BoneLib.Notifications.NotificationType.Success, "Error: The spawnable selected is already blocked.", 1.5f);}
                });
                // add pref for permission level allowed to spawn blocked spawnables
            }

            public static void RefreshBlockedItems() 
            {
                _blockedItemsList.RemoveAll();
                _blockedItemsList.CreateFunction("Refresh", Color.yellow, RefreshBlockedItems);

                var blockeds = Prefs.GetBlockedBarcodes();

                foreach(string barcode in  blockeds)
                {
                    var title = TryGetTitleFromBarcode(barcode);

                    var spawnablePage = _blockedItemsList.CreatePage(title, Color.white);
                    spawnablePage.CreateFunction($"Barcode: {barcode}", Color.white, null);
                    spawnablePage.CreateFunction("Unblock", Color.cyan, () =>
                    {
                        var blockeds = Prefs.GetBlockedBarcodes();
                        if (blockeds.Contains(barcode))
                        {
                            var newblockeds = blockeds.ToList();
                            if (newblockeds.Remove(barcode))
                                Prefs.SetBlockedBarcodes(newblockeds.ToArray());
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
                page.CreateFunction($"Player Guilty: {playerName}", Color.white, null);
                page.CreateFunction("Unblock Spawnable", Color.green, () =>
                {
                    Menu.DisplayDialog(
                        $"Remove {spawnableName} from blocked spawnables?",
                        $"Are you sure you want to unblock spawnable \"{spawnableName}\" from being spawned in?",
                        confirmAction:
                        () =>
                        {
                            var blockeds = Prefs.GetBlockedBarcodes();
                            if (blockeds.Contains(barcode))
                            {
                                var newblockeds = blockeds.ToList();
                                if (newblockeds.Remove(barcode))
                                Prefs.SetBlockedBarcodes(newblockeds.ToArray());
                            }
                        }
                    );
                });
                page.CreateFunction("Kick Player", Color.red, () => 
                {
                    Menu.DisplayDialog(
                        $"Kick {playerName} for spawning {spawnableName}?",
                        $"Are you sure you want to kick the player {playerName} for spawning in the blocked spawnable {spawnableName}?",
                        confirmAction:
                        () =>
                        {
                            NetworkHelper.KickUser(id);
                        }
                    );
                });
                

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
