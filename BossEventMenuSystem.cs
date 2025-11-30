using bossSpawnRestrictions.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace bossSpawnRestrictions
{
	public class BossEventMenuSystem : ModSystem
	{
		public static ModKeybind OpenMenuKeybind { get; private set; }

		private UserInterface bossEventMenuInterface;
		internal BossEventMenuUI bossEventMenuUI;

		public override void Load()
		{
			OpenMenuKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Boss/Event Menu", Keys.B);

			if (!Main.dedServ)
			{
				bossEventMenuInterface = new UserInterface();
				bossEventMenuUI = new BossEventMenuUI();
				bossEventMenuUI.Activate();
			}
		}

		public override void UpdateUI(GameTime gameTime)
		{
			if (bossEventMenuInterface?.CurrentState != null)
			{
				bossEventMenuInterface.Update(gameTime);
			}

			// check for keybind press
			if (OpenMenuKeybind.JustPressed)
			{
				ToggleMenu();
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseTextIndex != -1)
			{
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
					"bossSpawnRestrictions: Boss Event Menu",
					delegate
					{
						if (bossEventMenuInterface?.CurrentState != null)
						{
							bossEventMenuInterface.Draw(Main.spriteBatch, new GameTime());
						}
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}

		private void ToggleMenu()
		{
			if (bossEventMenuInterface.CurrentState != null)
			{
				bossEventMenuInterface.SetState(null);
			}
			else
			{
				bossEventMenuUI.RefreshList();
				bossEventMenuInterface.SetState(bossEventMenuUI);
			}
		}

		public override void Unload()
		{
			OpenMenuKeybind = null;
			bossEventMenuInterface = null;
			bossEventMenuUI = null;
		}
	}
}
