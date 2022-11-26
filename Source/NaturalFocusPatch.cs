using HarmonyLib;
using System.Reflection;
using UnityEngine;
using RimWorld;
using Verse;

namespace IdeologicalAnima
{
	[StaticConstructorOnStartup]
	static class HarmonyPatches
	{
		static HarmonyPatches()
		{
			Harmony harmony = new Harmony("Azuraal.IdeologicalAnima");
			Assembly assembly = Assembly.GetExecutingAssembly();
			harmony.PatchAll(assembly);
		}

	}
	[HarmonyPatch(typeof(MeditationFocusDef), "CanPawnUse")]
	class NaturalFocusPatch
	{
		private static PreceptDef Trees_Precept;
		static NaturalFocusPatch()
		{
			DefDatabase<PreceptDef>.GetNamed("Trees_Desired").description += "\n\n<color=#E6E64C>Enables meditation focus types:</color>\n - Natural";
			Trees_Precept = DefDatabase<PreceptDef>.GetNamed("Trees_Desired");
		}
		static void Postfix(ref bool __result, string ___defName, Pawn p)
		{
			if (p.Ideo != null)
			{
				if (___defName == "Natural")
				{
					if (p.Ideo.HasPrecept(Trees_Precept)) __result = true;
					else if (!IdeologicalAnimaMod.Settings.InateTribalNaturalFocus) __result = false;
				}
				else if (___defName == "Artistic")
				{
					if (!p.Ideo.HasPrecept(Trees_Precept) && !IdeologicalAnimaMod.Settings.InateTribalNaturalFocus) __result = true;
				}
			}
		}
	}

	public class IdeologicalAnimaSettings : ModSettings
	{
		public bool InateTribalNaturalFocus = false;
		public override void ExposeData()
		{
			Scribe_Values.Look(ref InateTribalNaturalFocus, "InateTribalFocus", defaultValue: false); ;
			base.ExposeData();
		}
	}

	public class IdeologicalAnimaMod : Verse.Mod
	{
		public static IdeologicalAnimaSettings Settings;
		public IdeologicalAnimaMod(ModContentPack content) : base(content)
		{
			Settings = GetSettings<IdeologicalAnimaSettings>();
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			Listing_Standard listingStandard = new Listing_Standard();
			listingStandard.Begin(inRect);
			listingStandard.CheckboxLabeled("Inate natural focus type for characters with tribal upbringing.", ref Settings.InateTribalNaturalFocus);
			listingStandard.End();
			base.DoSettingsWindowContents(inRect);
		}
		public override string SettingsCategory()
		{
			return "Ideological Anima";
		}
	}

}