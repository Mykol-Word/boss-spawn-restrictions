using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace bossSpawnRestrictions
{
	public class SpawnRestrictionEnforcer : ModSystem
	{
		public override void PostUpdateNPCs()
		{
			// check all active NPCs
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];

				if (npc.active && npc.boss)
				{
					string bossName = npc.FullName;

					if (VotingSystem.IsRestricted(bossName, true))
					{
						// despawn the boss without marking as defeated
						npc.active = false;
						npc.life = 0;

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, i);
						}
					}
				}
			}
		}

		public override void PostUpdateEverything()
		{
			// check and end restricted events
			CheckAndEndEvents();
		}

		private void CheckAndEndEvents()
		{
			// goblin army
			if (Main.invasionType == InvasionID.GoblinArmy && VotingSystem.IsRestricted("Goblin Army", false))
			{
				EndInvasion();
			}

			// frost legion
			if (Main.invasionType == InvasionID.SnowLegion && VotingSystem.IsRestricted("Frost Legion", false))
			{
				EndInvasion();
			}

			// pirate invasion
			if (Main.invasionType == InvasionID.PirateInvasion && VotingSystem.IsRestricted("Pirate Invasion", false))
			{
				EndInvasion();
			}

			// martian madness
			if (Main.invasionType == InvasionID.MartianMadness && VotingSystem.IsRestricted("Martian Madness", false))
			{
				EndInvasion();
			}

			// old one's army
			if (Terraria.GameContent.Events.DD2Event.Ongoing && VotingSystem.IsRestricted("Old One's Army", false))
			{
				Terraria.GameContent.Events.DD2Event.StopInvasion();
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

			// pumpkin moon
			if (Main.pumpkinMoon && VotingSystem.IsRestricted("Pumpkin Moon", false))
			{
				Main.pumpkinMoon = false;
				Main.stopMoonEvent();
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

			// frost moon
			if (Main.snowMoon && VotingSystem.IsRestricted("Frost Moon", false))
			{
				Main.snowMoon = false;
				Main.stopMoonEvent();
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

			// solar eclipse
			if (Main.eclipse && VotingSystem.IsRestricted("Solar Eclipse", false))
			{
				Main.eclipse = false;
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

			// blood moon
			if (Main.bloodMoon && VotingSystem.IsRestricted("Blood Moon", false))
			{
				Main.bloodMoon = false;
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

			// rain
			if (Main.raining && VotingSystem.IsRestricted("Rain", false))
			{
				Main.StopRain();
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

			// sandstorm
			if (Terraria.GameContent.Events.Sandstorm.Happening && VotingSystem.IsRestricted("Sandstorm", false))
			{
				Terraria.GameContent.Events.Sandstorm.Happening = false;
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

			// slime rain
			if (Main.slimeRain && VotingSystem.IsRestricted("Slime Rain", false))
			{
				Main.StopSlimeRain();
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

			// lantern night
			if (Terraria.GameContent.Events.LanternNight.LanternsUp && VotingSystem.IsRestricted("Lantern Night", false))
			{
				Terraria.GameContent.Events.LanternNight.WorldClear();
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

			// party
			if (Terraria.GameContent.Events.BirthdayParty.PartyIsUp && VotingSystem.IsRestricted("Party", false))
			{
				Terraria.GameContent.Events.BirthdayParty.WorldClear();
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}
		}

		private void EndInvasion()
		{
			Main.invasionType = InvasionID.None;
			Main.invasionSize = 0;

			if (Main.netMode == NetmodeID.Server)
			{
				NetMessage.SendData(MessageID.WorldData);
			}
		}
	}
}
