using HarmonyLib;
using System.Reflection;
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
			static NaturalFocusPatch()
			{
				DefDatabase<PreceptDef>.GetNamed("Trees_Desired").description += "\n\n<color=#E6E64C>Enables meditation focus types:</color>\n - Natural";
			}
			static void Postfix(ref bool __result, string ___defName, Pawn p)
			{
				if (___defName == "Natural")
				{
					if (p.Ideo != null)
					{
						if (p.Ideo.HasPrecept(DefDatabase<PreceptDef>.GetNamed("Trees_Desired"))) __result = true;
						else __result = false;
					}
					else __result = false;
				}
			}
		}
	}
}
