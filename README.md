# F.I.S.H ( Fusion Intermediate Server Helper )

Fusion Intermediate Server Helper is a mod that extends the capability of the server host's management abilities within BONELAB Fusion.

## Features

Fusion Intermediate Server Helper currently has 3 features:

- [Spawnable Blocking](#spawnable-blocking)
- [Despawn All (No Particles)](#despawn-all)
- [Disable DevTools Despawning](#disable-devtools-cleanup)

### Spawnable Blocking

Spawnable blocking allows for blocking of certain spawnables from being blocked
*Crazy, right?*

When a spawnable is requested to spawn, the mod (when enabled) checks the spawnable's barcode to see if it is blocked.
It does this in two ways:

- Block list: The barcode is checked against a user-defined array of barcodes.
- Regex match: The barcode is checked against a user-defined array of regex operations. If one matches, the spawnable is blocked.

Menu Breakdown
```md
Bone Menu
└ Fusion Intermediate Server Helper (Page)
  └ Spawnable Blocking (Page)
    ├ Enabled (bool): Toggles feature functionality 
    ├ Spawn Blocked Spawnables Allowed Permission Level (PermissionLevel Enum): Changes what PermissionLevel is needed to spawn blocked spawnables.
    ├ Regex Check Enabled (bool): Enables barcode checking against Regex list for blocking.
    ├ Blocked Spawnables (Page): List of Blocked Spawnables
    | ├ Refresh (Function): Refreshes the list of spawnables
    | └ Spawnable Name (Page): A page describing a spawnable that is blocked from being spawned. One of these is created for each blocked spawnable. 
    |   ├ Barcode (Function): Shows the user the barcode of a blocked spawnable, also allowing for user extrapolation of pallet and author information.
    |   └ Unblock (Function): Removes the spawnable from the blocked spawnables list, allowing to be spawned again.
    ├ Add Spawnable to Blocklist from Spawn Gun (Function): Adds the spawnable selected on the Spawn Gun held in the ***left*** hand to the spawn block list.
    ├ Add Spawnable in hand to Blocklist (Function): Ditto, but with what is held instead.
    └ Add Spawnable to Blocklist from string (string): Adds the spawnable directly to the blocklist using its barcode.
```

### Despawn All

Despawn All is a button that allows for quick access to Fusion's "Despawn All" feature, slightly modified to not show despawn particles (workaround for voicechat / sound break bug)

Menu Breakdown:
```md
Bone Menu
└ Fusion Intermediate Server Helper (Page)
  └ Despawn All (Function): Shows popup to confirm Despawn All action.
    └ Despawn All Confirmation (Function): Popup that despawns all spawnables in the server when confirmed. Shows error when not in a server or not the host.
```

### Disable DevTools Cleanup

Disable DevTools Cleanup is a feature which allows for the user to toggle having dev tools despawned after not being touched for a while

Menu Breakdown:
```md
Bone Menu
└ Fusion Intermediate Server Helper (Page)
  └ Disable DevTools Cleanup (bool): Enables/Disables feature.
```
Note: DevTools created when the feature was inactive will still despawn when it is active, DevTools created when the feature was active will still not despawn when it is deactivated.

### Credits

[BONELAB Fusion](https://github.com/Lakatrazz/BONELAB-Fusion): Main mod that this extends, used as library & used code snippets
[BoneLib](https://github.com/yowchap/BoneLib): Used as library
[ModioModNetworker](https://github.com/notnotnotswipez/ModioModNetworker): Used code snippets


### Notes
See [Actions tab](https://github.com/doge15567/FusionIntermediateServerHelper/actions) for builds.

Post to the [Issues tab](https://github.com/doge15567/FusionIntermediateServerHelper/issues) to bring up issues and recommendations.