using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace bossSpawnRestrictions
{
	public class PlayerVoteData : ModPlayer
	{
		private Dictionary<string, bool> savedBossVotes = new Dictionary<string, bool>();
		private Dictionary<string, bool> savedEventVotes = new Dictionary<string, bool>();

		public override void SaveData(TagCompound tag)
		{
			// save only restricted items as lists
			tag["restrictedBosses"] = savedBossVotes.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
			tag["restrictedEvents"] = savedEventVotes.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
		}

		public override void LoadData(TagCompound tag)
		{
			savedBossVotes.Clear();
			savedEventVotes.Clear();

			var restrictedBosses = tag.GetList<string>("restrictedBosses");
			var restrictedEvents = tag.GetList<string>("restrictedEvents");

			foreach (var boss in restrictedBosses)
				savedBossVotes[boss] = true;

			foreach (var evt in restrictedEvents)
				savedEventVotes[evt] = true;
		}

		public override void OnEnterWorld()
		{
			// restore saved votes when entering world
			foreach (var vote in savedBossVotes)
			{
				SpawnRestrictionTracker.SetBossRestriction(vote.Key, vote.Value);
				VotingSystem.SetLocalVote(vote.Key, vote.Value, true);
			}

			foreach (var vote in savedEventVotes)
			{
				SpawnRestrictionTracker.SetEventRestriction(vote.Key, vote.Value);
				VotingSystem.SetLocalVote(vote.Key, vote.Value, false);
			}
		}

		public static void SaveVote(string name, bool restricted, bool isBoss)
		{
			var player = Terraria.Main.LocalPlayer.GetModPlayer<PlayerVoteData>();
			if (player != null)
			{
				if (isBoss)
					player.savedBossVotes[name] = restricted;
				else
					player.savedEventVotes[name] = restricted;
			}
		}
	}
}
