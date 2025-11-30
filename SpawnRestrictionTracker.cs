using System.Collections.Generic;

namespace bossSpawnRestrictions
{
	public static class SpawnRestrictionTracker
	{
		private static Dictionary<string, bool> restrictedBosses = new Dictionary<string, bool>();
		private static Dictionary<string, bool> restrictedEvents = new Dictionary<string, bool>();

		public static bool IsBossRestricted(string bossName)
		{
			return restrictedBosses.ContainsKey(bossName) && restrictedBosses[bossName];
		}

		public static bool IsEventRestricted(string eventName)
		{
			return restrictedEvents.ContainsKey(eventName) && restrictedEvents[eventName];
		}

		public static void SetBossRestriction(string bossName, bool restricted)
		{
			restrictedBosses[bossName] = restricted;
		}

		public static void SetEventRestriction(string eventName, bool restricted)
		{
			restrictedEvents[eventName] = restricted;
		}

		public static void ClearAllRestrictions()
		{
			restrictedBosses.Clear();
			restrictedEvents.Clear();
		}

		public static Dictionary<string, bool> GetAllBossRestrictions()
		{
			return new Dictionary<string, bool>(restrictedBosses);
		}

		public static Dictionary<string, bool> GetAllEventRestrictions()
		{
			return new Dictionary<string, bool>(restrictedEvents);
		}
	}
}
