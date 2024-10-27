# F.I.S.H ( Fusion Intermediate Server Helper )

Fusion Intermediate Server Helper is a mod that extends the capability of the server host's management abilities within BONELAB Fusion.

## Features

Fusion Intermediate Server Helper currently has 1 feature:

- [Spawnable Blocking](#spawnable-blocking)
### Spawnable Blocking

Spawnable blocking allows for blocking of certain spawnables from being blocked
*Crazy, right?*

Usage Breakdown
```md
Bone Menu
└ Fusion Intermediate Server Helper (Page)
  └ Spawnable Blocking (Page)
    ├ Enabled (bool): Toggles feature functionality 
    ├ Spawn Blocked Spawnables Allowed Permission Level (PermissionLevel Enum): Changes what PermissionLevel is needed to spawn blocked spawnables.
    ├ Blocked Spawnables (Page): List of Blocked Spawnables
    | ├ Refresh (void): Refreshes the list of spawnables
    | └ Spawnable Name (Page): A page describing a spawnable that is blocked from being spawned. One of these is created for each blocked spawnable. 
    |   ├ Barcode (void): Shows the user the barcode of a blocked spawnable, also allowing for user extrapolation of pallet and author information.
    |   └ Unblock (void): Removes the spawnable from the blocked spawnables list, allowing to be spawned again.
    ├ Add Spawnable to Blocklist from Spawn Gun (void): Adds the spawnable selected on the Spawn Gun held in the ***left*** hand to the spawn block list.
    ├ Add Spawnable in hand to Blocklist (void): Ditto, but with what is held instead.
    └ Add Spawnable to Blocklist from string (string): Adds the spawnable directly to the blocklist using its barcode.
```

See [Actions tab](https://github.com/doge15567/FusionIntermediateServerHelper/actions) for beta builds!