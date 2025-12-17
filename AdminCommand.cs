using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Terraria;
using Terraria.ModLoader;

namespace bossSpawnRestrictions
{
	public class RestrictionCommand : ModCommand
	{
		public override string Command => "restrict";

		public override CommandType Type => CommandType.Chat;

		public override string Usage => "/restrict <boss|event> <name> <true|false>";

		public override string Description => "set boss/event restrictions (admin only)";

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			// admin check - only server console or singleplayer
			if (Main.netMode == Terraria.ID.NetmodeID.MultiplayerClient)
			{
				caller.Reply("admin only", Microsoft.Xna.Framework.Color.Red);
				return;
			}

			// on dedicated server, only console can use this
			if (Main.netMode == Terraria.ID.NetmodeID.Server && caller.Player.whoAmI != -1)
			{
				caller.Reply("server console only", Microsoft.Xna.Framework.Color.Red);
				return;
			}

			// must be in server mode
			if (!ServerConfig.IsServerMode())
			{
				caller.Reply("server-based mode is not enabled", Microsoft.Xna.Framework.Color.Orange);
				return;
			}

			if (args.Length < 3)
			{
				caller.Reply($"usage: {Usage}", Microsoft.Xna.Framework.Color.Yellow);
				return;
			}

			string type = args[0].ToLower();
			string name = string.Join(" ", args.Skip(1).Take(args.Length - 2));
			string value = args[args.Length - 1].ToLower();

			bool isBoss = type == "boss";
			bool isEvent = type == "event";

			if (!isBoss && !isEvent)
			{
				caller.Reply("type must be 'boss' or 'event'", Microsoft.Xna.Framework.Color.Red);
				return;
			}

			if (value != "true" && value != "false")
			{
				caller.Reply("value must be 'true' or 'false'", Microsoft.Xna.Framework.Color.Red);
				return;
			}

			bool restricted = value == "true";
			var config = ModContent.GetInstance<ServerConfig>();

			// set restriction
			if (isBoss)
			{
				if (!config.BossRestrictions.ContainsKey(name))
				{
					caller.Reply($"boss '{name}' not found in config", Microsoft.Xna.Framework.Color.Red);
					caller.Reply("available: " + string.Join(", ", config.BossRestrictions.Keys), Microsoft.Xna.Framework.Color.Gray);
					return;
				}

				config.BossRestrictions[name] = restricted;
			}
			else
			{
				if (!config.EventRestrictions.ContainsKey(name))
				{
					caller.Reply($"event '{name}' not found in config", Microsoft.Xna.Framework.Color.Red);
					caller.Reply("available: " + string.Join(", ", config.EventRestrictions.Keys), Microsoft.Xna.Framework.Color.Gray);
					return;
				}

				config.EventRestrictions[name] = restricted;
			}

			// save config manually
			string configPath = Path.Combine(Terraria.ModLoader.ModLoader.ModPath, "..", "ModConfigs", "bossSpawnRestrictions_ServerConfig.json");
			string json = JsonConvert.SerializeObject(config, Formatting.Indented);
			File.WriteAllText(configPath, json);

			string statusText = restricted ? "restricted" : "allowed";
			caller.Reply($"{(isBoss ? "boss" : "event")} '{name}' set to {statusText}", Microsoft.Xna.Framework.Color.Green);
		}
	}

	public class ListRestrictionsCommand : ModCommand
	{
		public override string Command => "listrestrictions";

		public override CommandType Type => CommandType.Chat;

		public override string Usage => "/listrestrictions [boss|event]";

		public override string Description => "list current restrictions";

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (!ServerConfig.IsServerMode())
			{
				caller.Reply("server-based mode is not enabled", Microsoft.Xna.Framework.Color.Orange);
				return;
			}

			var config = ModContent.GetInstance<ServerConfig>();
			string filter = args.Length > 0 ? args[0].ToLower() : "";

			if (filter == "" || filter == "boss")
			{
				caller.Reply("=== bosses ===", Microsoft.Xna.Framework.Color.Gold);
				foreach (var entry in config.BossRestrictions.OrderBy(x => x.Key))
				{
					var color = entry.Value ? Microsoft.Xna.Framework.Color.Red : Microsoft.Xna.Framework.Color.Green;
					caller.Reply($"  {entry.Key}: {(entry.Value ? "restricted" : "allowed")}", color);
				}
			}

			if (filter == "" || filter == "event")
			{
				caller.Reply("=== events ===", Microsoft.Xna.Framework.Color.Gold);
				foreach (var entry in config.EventRestrictions.OrderBy(x => x.Key))
				{
					var color = entry.Value ? Microsoft.Xna.Framework.Color.Red : Microsoft.Xna.Framework.Color.Green;
					caller.Reply($"  {entry.Key}: {(entry.Value ? "restricted" : "allowed")}", color);
				}
			}
		}
	}
}
