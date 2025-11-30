using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace bossSpawnRestrictions.UI
{
	public class BossEventMenuUI : UIState
	{
		private UIPanel mainPanel;
		private UIList bossEventList;
		private UIScrollbar scrollbar;

		public override void OnInitialize()
		{
			// main container panel
			mainPanel = new UIPanel();
			mainPanel.SetPadding(0);
			mainPanel.Left.Set(400f, 0f);
			mainPanel.Top.Set(200f, 0f);
			mainPanel.Width.Set(600f, 0f);
			mainPanel.Height.Set(400f, 0f);
			mainPanel.BackgroundColor = new Color(73, 94, 171) * 0.95f;
			Append(mainPanel);

			// title
			var title = new UIText("Bosses & Events", 1.2f);
			title.HAlign = 0.5f;
			title.Top.Set(15f, 0f);
			mainPanel.Append(title);

			// list for bosses and events
			bossEventList = new UIList();
			bossEventList.Width.Set(-25f, 1f);
			bossEventList.Height.Set(-60f, 1f);
			bossEventList.Top.Set(50f, 0f);
			bossEventList.ListPadding = 5f;
			bossEventList.ManualSortMethod = (list) => { };
			mainPanel.Append(bossEventList);

			// scrollbar
			scrollbar = new UIScrollbar();
			scrollbar.SetView(100f, 1000f);
			scrollbar.Height.Set(-60f, 1f);
			scrollbar.Top.Set(50f, 0f);
			scrollbar.HAlign = 1f;
			mainPanel.Append(scrollbar);

			bossEventList.SetScrollbar(scrollbar);

			// don't populate here - wait until menu is opened
		}

		private void PopulateList()
		{
			bossEventList.Clear();

			// add bosses section header
			var bossHeaderItem = new BossEventItem("=== BOSSES ===", false, true);
			bossEventList.Add(bossHeaderItem);

			var bosses = BossEventDetector.GetAllBosses();
			foreach (var boss in bosses)
			{
				var bossItem = new BossEventItem(boss.Name, boss.IsModded, false);
				bossEventList.Add(bossItem);
			}

			// add events section header
			var eventHeaderItem = new BossEventItem("=== EVENTS ===", false, true);
			bossEventList.Add(eventHeaderItem);

			var events = BossEventDetector.GetAllEvents();
			foreach (var evt in events)
			{
				var eventItem = new BossEventItem(evt.Name, evt.IsModded, false);
				bossEventList.Add(eventItem);
			}
		}

		public void RefreshList()
		{
			PopulateList();
		}
	}

	public class BossEventItem : UIPanel
	{
		private readonly string itemName;
		private readonly bool isModded;
		private readonly bool isHeader;

		public BossEventItem(string name, bool modded, bool header = false)
		{
			itemName = name;
			isModded = modded;
			isHeader = header;

			Width.Set(0, 1f);
			Height.Set(30f, 0f);
			BackgroundColor = new Color(35, 40, 83) * 0.8f;
			BorderColor = new Color(18, 18, 38);

			var text = new UIText(name, header ? 1f : 0.9f);
			text.Left.Set(10f, 0f);
			text.VAlign = 0.5f;
			text.TextColor = header ? Color.Gold : (modded ? new Color(150, 255, 150) : Color.White);
			Append(text);

			if (modded && !header)
			{
				var moddedLabel = new UIText("[MODDED]", 0.7f);
				moddedLabel.HAlign = 1f;
				moddedLabel.VAlign = 0.5f;
				moddedLabel.Left.Set(-10f, 0f);
				moddedLabel.TextColor = new Color(100, 200, 100);
				Append(moddedLabel);
			}
		}

		public override void MouseOver(UIMouseEvent evt)
		{
			base.MouseOver(evt);
			BackgroundColor = new Color(44, 57, 105) * 0.9f;
			BorderColor = new Color(89, 116, 213);
		}

		public override void MouseOut(UIMouseEvent evt)
		{
			base.MouseOut(evt);
			BackgroundColor = new Color(35, 40, 83) * 0.8f;
			BorderColor = new Color(18, 18, 38);
		}
	}
}
