using System.Reflection;
using FusionIntermediateServerHelper;
using MelonLoader;

[assembly: AssemblyDescription(FusionIntermediateServerHelper.Main.Description)]
[assembly: AssemblyVersion(FusionIntermediateServerHelper.Main.Version)]
[assembly: AssemblyFileVersion(FusionIntermediateServerHelper.Main.Version)]
[assembly: AssemblyCopyright("Developed by " + FusionIntermediateServerHelper.Main.Author)]
[assembly: AssemblyTrademark(FusionIntermediateServerHelper.Main.Company)]
[assembly: MelonInfo(typeof(FusionIntermediateServerHelper.Main), FusionIntermediateServerHelper.Main.Name, FusionIntermediateServerHelper.Main.Version, FusionIntermediateServerHelper.Main.Author, FusionIntermediateServerHelper.Main.DownloadLink)]
[assembly: MelonColor(50, 168, 82, 255)]

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Stress Level Zero", "BONELAB")]