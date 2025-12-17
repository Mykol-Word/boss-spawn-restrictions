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
		[Tooltip("democratic: players vote on restrictions\nserver-based: use the lists below")]
		public RestrictionMode RestrictionModeValue { get; set; }

		[Label("Restricted Bosses")]
		[Tooltip("bosses to restrict when using server-based mode")]
		public HashSet<string> RestrictedBosses { get; set; } = new HashSet<string>();

		[Label("Restricted Events")]
		[Tooltip("events to restrict when using server-based mode")]
		public HashSet<string> RestrictedEvents { get; set; } = new HashSet<string>();

		public static bool IsServerMode()
		{
			return ModContent.GetInstance<ServerConfig>().RestrictionModeValue == RestrictionMode.ServerBased;
		}

		public static bool IsRestricted(string name, bool isBoss)
		{
			var config = ModContent.GetInstance<ServerConfig>();

			if (isBoss)
				return config.RestrictedBosses.Contains(name);
			else
				return config.RestrictedEvents.Contains(name);
		}
	}
}
