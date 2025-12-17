using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace bossSpawnRestrictions
{
	public enum RestrictionMode
	{
		Democratic,
		ServerBased
	}

	public class ServerConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		[DefaultValue(RestrictionMode.Democratic)]
		[Label("Restriction Mode")]
		[Tooltip("democratic: players vote on restrictions\nserver-based: use the settings below")]
		public RestrictionMode RestrictionModeValue { get; set; }

		[Label("Boss Restrictions")]
		[Tooltip("true = restricted, false = allowed")]
		public Dictionary<string, bool> BossRestrictions { get; set; } = new Dictionary<string, bool>
		{
			// pre-hardmode
			{ "King Slime", false },
			{ "Eye of Cthulhu", false },
			{ "Eater of Worlds", false },
			{ "Brain of Cthulhu", false },
			{ "Queen Bee", false },
			{ "Skeletron", false },
			{ "Deerclops", false },
			{ "Wall of Flesh", false },
			// hardmode
			{ "Queen Slime", false },
			{ "The Twins", false },
			{ "The Destroyer", false },
			{ "Skeletron Prime", false },
			{ "Plantera", false },
			{ "Golem", false },
			{ "Duke Fishron", false },
			{ "Empress of Light", false },
			{ "Lunatic Cultist", false },
			{ "Moon Lord", false }
		};

		[Label("Event Restrictions")]
		[Tooltip("true = restricted, false = allowed")]
		public Dictionary<string, bool> EventRestrictions { get; set; } = new Dictionary<string, bool>
		{
			{ "Goblin Army", false },
			{ "Frost Legion", false },
			{ "Pirate Invasion", false },
			{ "Martian Madness", false },
			{ "Old One's Army", false },
			{ "Pumpkin Moon", false },
			{ "Frost Moon", false },
			{ "Solar Eclipse", false },
			{ "Blood Moon", false },
			{ "Rain", false },
			{ "Sandstorm", false },
			{ "Slime Rain", false },
			{ "Lantern Night", false },
			{ "Party", false }
		};

		public static bool IsServerMode()
		{
			return ModContent.GetInstance<ServerConfig>().RestrictionModeValue == RestrictionMode.ServerBased;
		}

		public static bool IsRestricted(string name, bool isBoss)
		{
			var config = ModContent.GetInstance<ServerConfig>();

			if (isBoss)
				return config.BossRestrictions.TryGetValue(name, out bool restricted) && restricted;
			else
				return config.EventRestrictions.TryGetValue(name, out bool restricted) && restricted;
		}
	}
}
