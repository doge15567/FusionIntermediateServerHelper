using BoneLib;
using BoneLib.BoneMenu;
using MelonLoader;
using FusionIntermediateServerHelper;
using Prefs = FusionIntermediateServerHelper.Main.Prefs;
using UnityEngine;

internal static class MenuHelper
{
	// By Camobiwon under "Here ya go" license

	internal static IntElement CreateIntPref(this Page page, string name, Color color, ref MelonPreferences_Entry<int> value, int increment, int minValue, int maxValue, Action<int> callback = null, string prefName = null, int prefDefaultValue = default)
	{
		prefName ??= name;

		if(!Prefs.GlobalCategory.HasEntry(prefName))
			value = Prefs.GlobalCategory.CreateEntry(prefName, prefDefaultValue);

		MelonPreferences_Entry<int> val = value;
		return page.CreateInt(name, color, val.Value, increment, minValue, maxValue, (x) =>
		{
			val.Value = x;
			Prefs.GlobalCategory.SaveToFile(false);
			callback?.InvokeActionSafe(x);
		});
	}

	internal static FloatElement CreateFloatPref(this Page page, string name, Color color, ref MelonPreferences_Entry<float> value, float increment, float minValue, float maxValue, Action<float> callback = null, string prefName = null, float prefDefaultValue = default)
	{
		prefName ??= name;

		if(!Prefs.GlobalCategory.HasEntry(prefName))
			value = Prefs.GlobalCategory.CreateEntry(prefName, prefDefaultValue);

		MelonPreferences_Entry<float> val = value;
		return page.CreateFloat(name, color, val.Value, increment, minValue, maxValue, (x) =>
		{
			val.Value = x;
			Prefs.GlobalCategory.SaveToFile(false);
			callback?.InvokeActionSafe(x);
		});
	}

	internal static BoolElement CreateBoolPref(this Page page, string name, Color color, ref MelonPreferences_Entry<bool> value, Action<bool> callback = null, string prefName = null, bool prefDefaultValue = default)
	{
		prefName ??= name;

		if(!Prefs.GlobalCategory.HasEntry(prefName))
			value = Prefs.GlobalCategory.CreateEntry(prefName, prefDefaultValue);

		MelonPreferences_Entry<bool> val = value;
		return page.CreateBool(name, color, val.Value, (x) =>
		{
			val.Value = x;
			Prefs.GlobalCategory.SaveToFile(false);
			callback?.InvokeActionSafe(x);
		});
	}

	internal static EnumElement CreateEnumPref<T>(this Page page, string name, Color color, ref MelonPreferences_Entry<T> value, Action<Enum> callback = null, string prefName = null, Enum prefDefaultValue = default) where T : Enum
	{
		prefName ??= name;

		if(!Prefs.GlobalCategory.HasEntry(prefName))
			value = Prefs.GlobalCategory.CreateEntry(prefName, (T)prefDefaultValue);

		MelonPreferences_Entry<T> val = value;
		return page.CreateEnum(name, color, val.Value, (x) =>
		{
			val.Value = (T)x;
			Prefs.GlobalCategory.SaveToFile(false);
			callback?.InvokeActionSafe(x);
		});
	}

	internal static StringElement CreateStringPref(this Page page, string name, Color color, ref MelonPreferences_Entry<string> value, Action<string> callback = null, string prefName = null, string prefDefaultValue = default)
	{
		prefName ??= name;

		if(!Prefs.GlobalCategory.HasEntry(prefName))
			value = Prefs.GlobalCategory.CreateEntry(prefName, prefDefaultValue);

		MelonPreferences_Entry<string> val = value;
		StringElement element = page.CreateString(name, color, val.Value, (x) =>
		{
			val.Value = x;
			Prefs.GlobalCategory.SaveToFile(false);
			callback?.InvokeActionSafe(x);
		});
		element.Value = value.Value; //BoneMenu temp hack fix
		return element;
	}
}