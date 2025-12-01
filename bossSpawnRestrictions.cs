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

				VotingSystem.ReceiveVote(whoAmI, name, restricted, isBoss);

				// server forwards to all clients
				if (Terraria.Main.netMode == Terraria.ID.NetmodeID.Server)
				{
					ModPacket packet = GetPacket();
					packet.Write((byte)0);
					packet.Write(isBoss);
					packet.Write(name);
					packet.Write(restricted);
					packet.Send(-1, -1);
				}
			}
		}
	}
}
