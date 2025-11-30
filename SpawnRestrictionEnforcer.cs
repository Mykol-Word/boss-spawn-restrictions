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

					if (SpawnRestrictionTracker.IsBossRestricted(bossName))
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
			if (Main.invasionType == InvasionID.GoblinArmy && SpawnRestrictionTracker.IsEventRestricted("Goblin Army"))
			{
				EndInvasion();
			}

			// frost legion
			if (Main.invasionType == InvasionID.SnowLegion && SpawnRestrictionTracker.IsEventRestricted("Frost Legion"))
			{
				EndInvasion();
			}

			// pirate invasion
			if (Main.invasionType == InvasionID.PirateInvasion && SpawnRestrictionTracker.IsEventRestricted("Pirate Invasion"))
			{
				EndInvasion();
			}

			// martian madness
			if (Main.invasionType == InvasionID.MartianMadness && SpawnRestrictionTracker.IsEventRestricted("Martian Madness"))
			{
				EndInvasion();
			}

			// old one's army
			if (Terraria.GameContent.Events.DD2Event.Ongoing && SpawnRestrictionTracker.IsEventRestricted("Old One's Army"))
			{
				Terraria.GameContent.Events.DD2Event.StopInvasion();
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

			// pumpkin moon
			if (Main.pumpkinMoon && SpawnRestrictionTracker.IsEventRestricted("Pumpkin Moon"))
			{
				Main.pumpkinMoon = false;
				Main.stopMoonEvent();
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

			// frost moon
			if (Main.snowMoon && SpawnRestrictionTracker.IsEventRestricted("Frost Moon"))
			{
				Main.snowMoon = false;
				Main.stopMoonEvent();
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

			// solar eclipse
			if (Main.eclipse && SpawnRestrictionTracker.IsEventRestricted("Solar Eclipse"))
			{
				Main.eclipse = false;
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

			// blood moon
			if (Main.bloodMoon && SpawnRestrictionTracker.IsEventRestricted("Blood Moon"))
			{
				Main.bloodMoon = false;
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

			// rain
			if (Main.raining && SpawnRestrictionTracker.IsEventRestricted("Rain"))
			{
				Main.StopRain();
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

			// sandstorm
			if (Terraria.GameContent.Events.Sandstorm.Happening && SpawnRestrictionTracker.IsEventRestricted("Sandstorm"))
			{
				Terraria.GameContent.Events.Sandstorm.Happening = false;
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

			// slime rain
			if (Main.slimeRain && SpawnRestrictionTracker.IsEventRestricted("Slime Rain"))
			{
				Main.StopSlimeRain();
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

			// lantern night
			if (Terraria.GameContent.Events.LanternNight.LanternsUp && SpawnRestrictionTracker.IsEventRestricted("Lantern Night"))
			{
				Terraria.GameContent.Events.LanternNight.WorldClear();
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

			// party
			if (Terraria.GameContent.Events.BirthdayParty.PartyIsUp && SpawnRestrictionTracker.IsEventRestricted("Party"))
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
