using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Reflection;

namespace StatsAsTokens_BFAV_Compat
{
	public class ModEntry : Mod
    {
		private static Harmony harmony;
		public static IMonitor Logger;

		/// <summary>The mod entry point.</summary>
		/// <param name="helper" />
		public override void Entry(IModHelper helper)
		{
			Logger = Monitor; 
			
			helper.Events.GameLoop.GameLaunched += (_, _) => PerformHarmonyPatches();
		}

		public static void LogTrace(string message)
		{
			Logger.Log(message, LogLevel.Trace);
		}

		public static void LogError(string message)
		{
			Logger.Log(message, LogLevel.Error);
		}

		private void PerformHarmonyPatches()
		{
			harmony = new(ModManifest.UniqueID);

			if (Helper.ModRegistry.IsLoaded("Paritee.BetterFarmAnimalVariety"))
			{
				FarmAnimal_FindTruffle_Patch();
			}
		}

		private static void FarmAnimal_FindTruffle_Patch()
		{
			MethodInfo findTruffle = AccessTools.Method(AccessTools.TypeByName("BetterFarmAnimalVariety.Framework.Patches.FarmAnimal.FindTruffle"),"AttemptToSpawnProduce");

			try
			{
				harmony.Patch(
					original: findTruffle,
					postfix: new HarmonyMethod(typeof(ModEntry), nameof(FarmAnimal_AttemptToSpawnProduce_Postfix))
				);

				LogTrace($"Patched {findTruffle.Name} successfully");
			}
			catch (Exception ex)
			{
				LogError($"Exception encountered while patching method {findTruffle.Name} with postfix {nameof(FarmAnimal_AttemptToSpawnProduce_Postfix)}: {ex}");
			}
		}

		private static void FarmAnimal_AttemptToSpawnProduce_Postfix(bool __result)
		{
			if (__result)
			{
				Game1.player.stats.TrufflesFound++;
			}
		}

	}
}
