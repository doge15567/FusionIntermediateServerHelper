

using LabFusion.Extensions;
using LabFusion.Network;
using LabFusion.Player;
using LabFusion.Representation;
using LabFusion.Utilities;
using HarmonyLib;
using BoneLib;
using LabFusion.Patching;
using Il2CppSLZ.Bonelab;

namespace FusionIntermediateServerHelper
{
    internal partial class Main
    {
        public static class ServerStats
        {
            // implement server stats
            /* Ideas:
             * Unique Players seen
             * Spawnables Spawned
             * Spawnables Despawned // Not going to do this one, sorry!
             * Levels Loaded
             * Players Killed
             */
            public static int SpawnablesSpawned = 0; // done
            public static int LevelsLoaded = 0; // done
            public static int PlayerDeaths = 0; // done
            public static int PlayersSeen = 0; // done
            public static List<ulong> PlayersSeenList = new List<ulong>();

            public static void Setup()
            {
                Reset();
                MultiplayerHooking.OnStartServer += () =>
                {
                    Reset();
                    MultiplayerHooking.OnPlayerJoin += OnPlayerJoin;
                    MultiplayerHooking.OnMainSceneInitialized += OnMainSceneInitialized;
                };
                MultiplayerHooking.OnDisconnect += () => 
                { 
                    Reset();
                    MultiplayerHooking.OnPlayerJoin -= OnPlayerJoin;
                    MultiplayerHooking.OnMainSceneInitialized -= OnMainSceneInitialized;
                };
            }
            public static void Reset()
            {
                SpawnablesSpawned = 0;
                LevelsLoaded = 0;
                PlayerDeaths = 0;
                PlayersSeen = 0;
            }
            public static void UpdateMetadata()
            {
                if (NetworkInfo.IsServer)
                {
                    var lobby = NetworkInfo.CurrentLobby;
                    lobby.SetMetadata("spawnablesspawned", SpawnablesSpawned.ToString());
                    lobby.SetMetadata("levelsloaded", LevelsLoaded.ToString());
                    lobby.SetMetadata("playerdeaths", PlayerDeaths.ToString());
                    lobby.SetMetadata("playersseen", PlayersSeen.ToString());
                }
            }
            public static void OnPlayerJoin(PlayerId playerID)
            {
                if (!PlayersSeenList.ToArray().Contains(playerID.LongId))
                {
                    PlayersSeenList.Add(playerID.LongId);
                    PlayersSeen += 1;
                }
                UpdateMetadata();
            }
            public static void OnMainSceneInitialized()
            {
                LevelsLoaded += 1;
                UpdateMetadata();
            }




        }
        public partial class ServerStatsPatches
        {

            [HarmonyPatch(typeof(PlayerRepActionMessage))]
            public static class PlayerRepActionMessagePatch
            {
                [HarmonyPrefix]
                [HarmonyPatch(nameof(PlayerRepActionMessage.HandleMessage))]
                public static void Prefix(byte[] bytes, bool isServerHandled = false)
                {
                    if (!isServerHandled)
                    {
                        return; // Use server message
                    }
#if DEBUG
                    Msg("Patch of handling message",2);
#endif
                    using var reader = FusionReader.Create(bytes);
                    var data = reader.ReadFusionSerializable<PlayerRepActionData>();

                    if (data.type == LabFusion.Senders.PlayerActionType.DEATH)
                    {
                        ServerStats.PlayerDeaths += 1;
                        ServerStats.UpdateMetadata();
                    }

                }
            }
            [HarmonyPatch(typeof(SpawnGunPatches))]
            public static class FusionSpawngunPatchPatch
            {
                [HarmonyPrefix]
                [HarmonyPatch("OnFireSpawn")]
                public static void Postfix(SpawnGun spawnGun)
                {
                    if (!NetworkInfo.IsServer) return;
                    // Get to the part where the achievement for spawning something is incremented using fusion code //https://github.com/Lakatrazz/BONELAB-Fusion/blob/9e2be403cc689598548fcf3e862d02e3781336d9/LabFusion/src/Patching/Patches/Props/SpawnGunPatches.cs#L21
                    // Check for prevention
                    if (FusionDevTools.PreventSpawnGun(PlayerIdManager.LocalId))
                    {
                        return;
                    }

                    var crate = spawnGun._selectedCrate;

                    if (crate == null)
                    {
                        return;
                    }

                    ServerStats.SpawnablesSpawned += 1;
                    ServerStats.UpdateMetadata();

                }
            }



        }

    }
}


