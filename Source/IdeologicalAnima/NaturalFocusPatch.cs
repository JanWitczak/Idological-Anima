using HarmonyLib;
using System.Reflection;
using UnityEngine;
using Verse;

namespace RimWorld
{
	namespace IdeologicalAnima
	{
		public class HarmonyPatches : Verse.Mod
		{
			public HarmonyPatches(ModContentPack content) : base(content)
			{
				var harmony = new Harmony("Azuraal.IdeologicalAnima");
				var assembly = Assembly.GetExecutingAssembly();
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
				if (___defName == "Natural" && p.Ideo != null)
				{
					if (p.Ideo.HasPrecept(Trees_Precept)) __result = true;
					else if (!IdeologicalAnimaMod.Settings.InateTribalNaturalFocus) __result = false;
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

		[StaticConstructorOnStartup]
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
}
