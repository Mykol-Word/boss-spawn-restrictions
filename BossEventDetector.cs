using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace bossSpawnRestrictions
{
	public class BossEventDetector
	{
		public class BossInfo
		{
			public string Name { get; set; }
			public int Type { get; set; }
			public bool IsModded { get; set; }
		}

		public class EventInfo
		{
			public string Name { get; set; }
			public bool IsModded { get; set; }
		}

		public static List<BossInfo> GetAllBosses()
		{
			var bosses = new List<BossInfo>();
			var seenNames = new HashSet<string>();

			// iterate through all loaded NPCs using ContentSamples
			foreach (var npcPair in ContentSamples.NpcsByNetId)
			{
				NPC npc = npcPair.Value;

				if (npc.boss && !string.IsNullOrEmpty(npc.FullName))
				{
					// avoid duplicates
					if (!seenNames.Contains(npc.FullName))
					{
						seenNames.Add(npc.FullName);
						bosses.Add(new BossInfo
						{
							Name = npc.FullName,
							Type = npc.netID,
							IsModded = npc.netID >= NPCID.Count
						});
					}
				}
			}

			return bosses.OrderBy(b => b.Name).ToList();
		}

		public static List<EventInfo> GetAllEvents()
		{
			var events = new List<EventInfo>();

			// vanilla events
			events.Add(new EventInfo { Name = "Goblin Army", IsModded = false });
			events.Add(new EventInfo { Name = "Frost Legion", IsModded = false });
			events.Add(new EventInfo { Name = "Pirate Invasion", IsModded = false });
			events.Add(new EventInfo { Name = "Martian Madness", IsModded = false });
			events.Add(new EventInfo { Name = "Old One's Army", IsModded = false });
			events.Add(new EventInfo { Name = "Pumpkin Moon", IsModded = false });
			events.Add(new EventInfo { Name = "Frost Moon", IsModded = false });
			events.Add(new EventInfo { Name = "Solar Eclipse", IsModded = false });
			events.Add(new EventInfo { Name = "Blood Moon", IsModded = false });
			events.Add(new EventInfo { Name = "Rain", IsModded = false });
			events.Add(new EventInfo { Name = "Sandstorm", IsModded = false });
			events.Add(new EventInfo { Name = "Slime Rain", IsModded = false });
			events.Add(new EventInfo { Name = "Lantern Night", IsModded = false });
			events.Add(new EventInfo { Name = "Party", IsModded = false });

			// add modded events
			foreach (var mod in ModLoader.Mods)
			{
				// etc.
			}

			return events.OrderBy(e => e.Name).ToList();
		}
	}
}
