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
	[HarmonyPatch(typeof(MeditationFocusTypeAvailabilityCache), "PawnCanUseInt")]
	class NaturalFocusPatch
	{
		static NaturalFocusPatch()
		{
			TreesPreceptDefOf.Trees_Desired.description += "\n\n<color=#E6E64C>Enables meditation focus types:</color>\n - Natural";
		}
		static void Postfix(ref bool __result, Pawn p, MeditationFocusDef type)
		{
			if (p.Ideo != null && !__result && type == MeditationFocusDefOf.Natural)
			{
				if(p.Ideo.HasPrecept(TreesPreceptDefOf.Trees_Desired)) __result = true;
			}
		}
	}
	[HarmonyPatch(typeof(Pawn_IdeoTracker), "SetIdeo")]
	class ChangedIdeoPatch
	{
		static void Postfix(Pawn_IdeoTracker __instance)
		{
			MeditationFocusTypeAvailabilityCache.ClearFor((Pawn)typeof(Pawn_IdeoTracker).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance));
		}
	}
	[HarmonyPatch(typeof(Ideo),"RecachePrecepts")]
	class RecachePreceptsPatch
	{
		static void Postfix(Ideo __instance)
		{
			foreach(Pawn pawn in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive)
			{
				if(pawn.Ideo == __instance) MeditationFocusTypeAvailabilityCache.ClearFor(pawn);
			}
		}
	}
	[DefOf]
	static class TreesPreceptDefOf
	{
		public static PreceptDef Trees_Desired;
		static TreesPreceptDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(TreesPreceptDefOf));
		}
	}
}