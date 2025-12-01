using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace bossSpawnRestrictions.UI
{
	public class BossEventMenuUI : UIState
	{
		private DraggableUIPanel mainPanel;
		private UIList bossEventList;
		private UIScrollbar scrollbar;

		public override void OnInitialize()
		{
			// main container panel
			mainPanel = new DraggableUIPanel();
			mainPanel.SetPadding(0);
			mainPanel.Left.Set(400f, 0f);
			mainPanel.Top.Set(200f, 0f);
			mainPanel.Width.Set(360f, 0f);
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
			bossEventList.Width.Set(-35f, 1f);
			bossEventList.Height.Set(-60f, 1f);
			bossEventList.Top.Set(50f, 0f);
			bossEventList.Left.Set(10f, 0f);
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
			var bossHeaderItem = new BossEventItem("BOSSES", false, true);
			bossEventList.Add(bossHeaderItem);

			var bosses = BossEventDetector.GetAllBosses();
			foreach (var boss in bosses)
			{
				var bossItem = new BossEventItem(boss.Name, boss.IsModded, false, true);
				bossEventList.Add(bossItem);
			}

			// add events section header
			var eventHeaderItem = new BossEventItem("EVENTS", false, true);
			bossEventList.Add(eventHeaderItem);

			var events = BossEventDetector.GetAllEvents();
			foreach (var evt in events)
			{
				var eventItem = new BossEventItem(evt.Name, evt.IsModded, false, false);
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
		private readonly bool isBoss;
		private UIText nameText;
		private UIText moddedLabel;
		private UIText voteText;
		private bool isRestricted = false;

		public BossEventItem(string name, bool modded, bool header = false, bool boss = true)
		{
			itemName = name;
			isModded = modded;
			isHeader = header;
			isBoss = boss;

			// load existing local vote state
			if (!header)
			{
				isRestricted = isBoss ? SpawnRestrictionTracker.IsBossRestricted(name) : SpawnRestrictionTracker.IsEventRestricted(name);
			}

			Width.Set(0, 1f);
			Height.Set(header ? 35f : 30f, 0f);

			if (header)
			{
				BackgroundColor = new Color(50, 65, 120) * 0.9f;
				BorderColor = new Color(89, 116, 213);

				var text = new UIText(name, 1.1f);
				text.HAlign = 0.5f;
				text.VAlign = 0.5f;
				text.TextColor = Color.Gold;
				Append(text);
			}
			else
			{
				SetPadding(5f);
				UpdateColors();

				nameText = new UIText(name, 0.9f);
				nameText.Left.Set(10f, 0f);
				nameText.VAlign = 0.5f;
				Append(nameText);

				// vote fraction display
				voteText = new UIText("0/0", 0.8f);
				voteText.HAlign = 1f;
				voteText.VAlign = 0.5f;
				voteText.Left.Set(modded ? -85f : -10f, 0f);
				voteText.TextColor = Color.LightGray;
				Append(voteText);

				if (modded)
				{
					moddedLabel = new UIText("[MODDED]", 0.7f);
					moddedLabel.HAlign = 1f;
					moddedLabel.VAlign = 0.5f;
					moddedLabel.Left.Set(-10f, 0f);
					Append(moddedLabel);
				}

				UpdateVoteDisplay();

				UpdateTextColors();
			}
		}

		private void UpdateColors()
		{
			if (isRestricted)
			{
				BackgroundColor = new Color(80, 30, 30) * 0.8f;
				BorderColor = new Color(120, 40, 40);
			}
			else
			{
				BackgroundColor = new Color(30, 80, 30) * 0.8f;
				BorderColor = new Color(40, 120, 40);
			}
		}

		private void UpdateTextColors()
		{
			// check actual restriction status from voting
			bool actuallyRestricted = VotingSystem.IsRestricted(itemName, isBoss);

			if (actuallyRestricted)
			{
				nameText.TextColor = isModded ? new Color(200, 130, 130) : new Color(255, 180, 180);
				if (moddedLabel != null)
					moddedLabel.TextColor = new Color(150, 100, 100);
			}
			else
			{
				nameText.TextColor = isModded ? new Color(130, 200, 130) : new Color(180, 255, 180);
				if (moddedLabel != null)
					moddedLabel.TextColor = new Color(100, 150, 100);
			}
		}

		private void UpdateVoteDisplay()
		{
			if (voteText != null)
			{
				var (votesFor, totalPlayers) = VotingSystem.GetVotes(itemName, isBoss);
				voteText.SetText($"{votesFor}/{totalPlayers}");
			}
		}

		public override void MouseOver(UIMouseEvent evt)
		{
			base.MouseOver(evt);
			if (!isHeader)
			{
				if (isRestricted)
				{
					BackgroundColor = new Color(100, 40, 40) * 0.9f;
					BorderColor = new Color(150, 60, 60);
				}
				else
				{
					BackgroundColor = new Color(40, 100, 40) * 0.9f;
					BorderColor = new Color(60, 150, 60);
				}
			}
		}

		public override void MouseOut(UIMouseEvent evt)
		{
			base.MouseOut(evt);
			if (!isHeader)
			{
				UpdateColors();
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			if (!isHeader)
			{
				UpdateVoteDisplay();
				UpdateTextColors();
			}
		}

		public override void LeftClick(UIMouseEvent evt)
		{
			base.LeftClick(evt);
			if (!isHeader)
			{
				SoundEngine.PlaySound(SoundID.MenuTick);

				isRestricted = !isRestricted;

				// update tracker
				if (isBoss)
				{
					SpawnRestrictionTracker.SetBossRestriction(itemName, isRestricted);
				}
				else
				{
					SpawnRestrictionTracker.SetEventRestriction(itemName, isRestricted);
				}

				// send vote to voting system
				VotingSystem.SetLocalVote(itemName, isRestricted, isBoss);

				UpdateColors();
				UpdateTextColors();
				UpdateVoteDisplay();
			}
		}
	}

	public class DraggableUIPanel : UIPanel
	{
		private Vector2 offset;
		private bool dragging;

		public override void LeftMouseDown(UIMouseEvent evt)
		{
			base.LeftMouseDown(evt);
			DragStart(evt);
		}

		public override void LeftMouseUp(UIMouseEvent evt)
		{
			base.LeftMouseUp(evt);
			DragEnd(evt);
		}

		private void DragStart(UIMouseEvent evt)
		{
			offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
			dragging = true;
		}

		private void DragEnd(UIMouseEvent evt)
		{
			dragging = false;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (ContainsPoint(Main.MouseScreen))
			{
				Main.LocalPlayer.mouseInterface = true;
			}

			if (dragging)
			{
				Left.Set(Main.mouseX - offset.X, 0f);
				Top.Set(Main.mouseY - offset.Y, 0f);
				Recalculate();
			}

			// keep panel on screen
			var parentSpace = Parent.GetDimensions().ToRectangle();
			if (!GetDimensions().ToRectangle().Intersects(parentSpace))
			{
				Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
				Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
				Recalculate();
			}
		}
	}
}
