using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace bossSpawnRestrictions
{
	public class VotingSystem : ModSystem
	{
		// tracks votes per player: playerIndex -> (bossName -> voted)
		private static Dictionary<int, Dictionary<string, bool>> playerBossVotes = new Dictionary<int, Dictionary<string, bool>>();
		private static Dictionary<int, Dictionary<string, bool>> playerEventVotes = new Dictionary<int, Dictionary<string, bool>>();

		public override void OnWorldLoad()
		{
			playerBossVotes.Clear();
			playerEventVotes.Clear();
		}

		public override void OnWorldUnload()
		{
			playerBossVotes.Clear();
			playerEventVotes.Clear();
		}

		public static void SetLocalVote(string name, bool restricted, bool isBoss)
		{
			int myPlayer = Main.myPlayer;

			if (isBoss)
			{
				if (!playerBossVotes.ContainsKey(myPlayer))
					playerBossVotes[myPlayer] = new Dictionary<string, bool>();

				playerBossVotes[myPlayer][name] = restricted;
			}
			else
			{
				if (!playerEventVotes.ContainsKey(myPlayer))
					playerEventVotes[myPlayer] = new Dictionary<string, bool>();

				playerEventVotes[myPlayer][name] = restricted;
			}

			// send vote to server/clients
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				ModPacket packet = ModContent.GetInstance<bossSpawnRestrictions>().GetPacket();
				packet.Write((byte)0);
				packet.Write(isBoss);
				packet.Write(name);
				packet.Write(restricted);
				packet.Write(myPlayer);
				packet.Send();
			}
			else if (Main.netMode == NetmodeID.Server)
			{
				// forward to all clients when called from server
				ModPacket packet = ModContent.GetInstance<bossSpawnRestrictions>().GetPacket();
				packet.Write((byte)0);
				packet.Write(isBoss);
				packet.Write(name);
				packet.Write(restricted);
				packet.Write(myPlayer);
				packet.Send(-1, -1);
			}
		}

		public static void ReceiveVote(int fromPlayer, string name, bool restricted, bool isBoss)
		{
			if (isBoss)
			{
				if (!playerBossVotes.ContainsKey(fromPlayer))
					playerBossVotes[fromPlayer] = new Dictionary<string, bool>();

				playerBossVotes[fromPlayer][name] = restricted;
			}
			else
			{
				if (!playerEventVotes.ContainsKey(fromPlayer))
					playerEventVotes[fromPlayer] = new Dictionary<string, bool>();

				playerEventVotes[fromPlayer][name] = restricted;
			}
		}

		public static (int votesFor, int totalPlayers) GetVotes(string name, bool isBoss)
		{
			int votesFor = 0;
			int totalPlayers = 0;

			var votes = isBoss ? playerBossVotes : playerEventVotes;

			// count active players
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				if (Main.player[i].active)
				{
					totalPlayers++;

					if (votes.ContainsKey(i) && votes[i].ContainsKey(name) && votes[i][name])
					{
						votesFor++;
					}
				}
			}

			return (votesFor, totalPlayers);
		}

		public static bool IsRestricted(string name, bool isBoss)
		{
			var (votesFor, totalPlayers) = GetVotes(name, isBoss);

			if (totalPlayers == 0)
				return false;

			// majority (rounded up from 1/2) - half or more restricts (ties restrict)
			return votesFor >= (totalPlayers + 1) / 2;
		}

		public static void SendFullSyncToPlayer(int toPlayer)
		{
			// send all boss votes
			foreach (var playerVotes in playerBossVotes)
			{
				int playerIndex = playerVotes.Key;
				foreach (var vote in playerVotes.Value)
				{
					ModPacket packet = ModContent.GetInstance<bossSpawnRestrictions>().GetPacket();
					packet.Write((byte)0);
					packet.Write(true); // isBoss
					packet.Write(vote.Key); // name
					packet.Write(vote.Value); // restricted
					packet.Write(playerIndex);
					packet.Send(toPlayer);
				}
			}

			// send all event votes
			foreach (var playerVotes in playerEventVotes)
			{
				int playerIndex = playerVotes.Key;
				foreach (var vote in playerVotes.Value)
				{
					ModPacket packet = ModContent.GetInstance<bossSpawnRestrictions>().GetPacket();
					packet.Write((byte)0);
					packet.Write(false); // isBoss
					packet.Write(vote.Key); // name
					packet.Write(vote.Value); // restricted
					packet.Write(playerIndex);
					packet.Send(toPlayer);
				}
			}
		}

		public static void RequestSync()
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				ModPacket packet = ModContent.GetInstance<bossSpawnRestrictions>().GetPacket();
				packet.Write((byte)1); // sync request
				packet.Send();
			}
		}
	}

	public class VotingNetworking : ModSystem
	{
		public override void Load()
		{
			if (Main.netMode != NetmodeID.Server)
			{
				// client/singleplayer doesn't need to do anything special
			}
		}
	}
}
