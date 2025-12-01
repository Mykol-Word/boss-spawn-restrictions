using System.IO;
using Terraria.ModLoader;

namespace bossSpawnRestrictions
{
	public class bossSpawnRestrictions : Mod
	{
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			byte packetType = reader.ReadByte();

			if (packetType == 0) // vote packet
			{
				bool isBoss = reader.ReadBoolean();
				string name = reader.ReadString();
				bool restricted = reader.ReadBoolean();
				int playerIndex = reader.ReadInt32();

				// server receives from client: whoAmI is the sender
				// client receives from server: playerIndex is the sender
				int actualPlayer = Terraria.Main.netMode == Terraria.ID.NetmodeID.Server ? whoAmI : playerIndex;

				VotingSystem.ReceiveVote(actualPlayer, name, restricted, isBoss);

				// server forwards to all clients with player index
				if (Terraria.Main.netMode == Terraria.ID.NetmodeID.Server)
				{
					ModPacket packet = GetPacket();
					packet.Write((byte)0);
					packet.Write(isBoss);
					packet.Write(name);
					packet.Write(restricted);
					packet.Write(whoAmI); // include player index
					packet.Send(-1, -1);
				}
			}
			else if (packetType == 1) // sync request from client
			{
				if (Terraria.Main.netMode == Terraria.ID.NetmodeID.Server)
				{
					VotingSystem.SendFullSyncToPlayer(whoAmI);
				}
			}
		}
	}
}
