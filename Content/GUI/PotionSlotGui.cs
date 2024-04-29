using PotionSlots.Core.Loaders.UILoading;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ID;
using Terraria.UI;

namespace PotionSlots.Content.GUI
{
	public class PotionSlotGui : SmartUIState
	{
		LifeSlot life = new();
		ManaSlot mana = new();

		public override bool Visible => Main.playerInventory;

		public override int InsertionIndex(List<GameInterfaceLayer> layers)
		{
			return layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
		}

		public override void OnInitialize()
		{
			AddElement(life, 572, 107, 28, 28);
			AddElement(mana, 572, 139, 28, 28);
		}

		public override void SafeUpdate(GameTime gameTime)
		{
			// Debug
			if (Main.LocalPlayer.controlHook)
			{
				RemoveAllChildren();
				OnInitialize();
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			var font = Terraria.GameContent.FontAssets.MouseText.Value;
			spriteBatch.DrawString(font, "Potions", new Vector2(570, 85), Main.MouseTextColorReal, 0f, Vector2.Zero, 0.7f, 0, 0);

			base.Draw(spriteBatch);
		}
	}

	public abstract class PotionSlot : SmartUIElement
	{
		public abstract ref Item item { get; }

		public abstract Func<Item, bool> isValid { get; }

		public abstract string Texture { get; }

		public abstract string TextureFilled { get; }

		public bool NoItem => item is null || item.IsAir;


		public PotionSlot()
		{
			// TOOD: Adjust for texture
			Width.Set(42, 0);
			Height.Set(42, 0);
		}

		public override void SafeClick(UIMouseEvent evt)
		{
			if (!Main.mouseItem.IsAir && isValid(Main.mouseItem) && NoItem)
			{
				item = Main.mouseItem.Clone();

				Main.LocalPlayer.HeldItem.TurnToAir();
				Main.mouseItem.TurnToAir();
				SoundEngine.PlaySound(SoundID.Grab);
			}
			else if (Main.mouseItem.IsAir && !NoItem)
			{
				Main.mouseItem = item.Clone();
				item.TurnToAir();
				SoundEngine.PlaySound(SoundID.Grab);
			}
			else if (!Main.mouseItem.IsAir && isValid(Main.mouseItem) && !NoItem)
			{
				if (Main.mouseItem.type != item.type)
				{
					var temp = item;
					item = Main.mouseItem.Clone();
					Main.mouseItem = temp.Clone();
				}
				else
				{
					var joined = item.stack + Main.mouseItem.stack;
					if (joined > item.maxStack)
					{
						item.stack = item.maxStack;
						Main.mouseItem.stack = joined - item.maxStack;
					}
					else
					{
						item.stack = joined;
						Main.mouseItem.TurnToAir();
					}
				}
				SoundEngine.PlaySound(SoundID.Grab);
			}
		}

		public override void SafeRightClick(UIMouseEvent evt)
		{
			if (Main.mouseItem.IsAir && !NoItem)
			{
				var temp = item.Clone();
				temp.stack = 1;
				Main.mouseItem = temp;

				item.stack--;

				if (item.stack <= 0)
					item.TurnToAir();

				SoundEngine.PlaySound(SoundID.MenuTick);
			}
			else if (!Main.mouseItem.IsAir && !NoItem && Main.mouseItem.type == item.type && Main.mouseItem.stack < Main.mouseItem.maxStack)
			{
				Main.mouseItem.stack++;
				item.stack--;

				if (item.stack <= 0)
					item.TurnToAir();

				SoundEngine.PlaySound(SoundID.MenuTick);
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			var tex = ModContent.Request<Texture2D>(NoItem ? Texture : TextureFilled).Value;
			spriteBatch.Draw(tex, GetDimensions().ToRectangle(), Color.White);

			if (!NoItem)
			{
				Main.inventoryScale = 36 / 52f * 28 / 36f;
				ItemSlot.Draw(spriteBatch, ref item, 21, GetDimensions().Position());

				if (IsMouseHovering)
				{
					Main.LocalPlayer.mouseInterface = true;
					Main.HoverItem = item.Clone();
					Main.hoverItemName = "a";
				}
			}

			if (IsMouseHovering)
				Main.LocalPlayer.mouseInterface = true;
		}
	}

	public class LifeSlot : PotionSlot
	{
		public override ref Item item => ref Main.LocalPlayer.GetModPlayer<PotionStoragePlayer>().lifeSlot;

		public override Func<Item, bool> isValid => (item) => item.healLife > 0;

		public override string Texture => "PotionSlots/Assets/healing_sprite";
		public override string TextureFilled => "PotionSlots/Assets/healingbg";

	}

	public class ManaSlot : PotionSlot
	{
		public override ref Item item => ref Main.LocalPlayer.GetModPlayer<PotionStoragePlayer>().manaSlot;

		public override Func<Item, bool> isValid => (item) => item.healMana > 0;

		public override string Texture => "PotionSlots/Assets/mana_sprite";
		public override string TextureFilled => "PotionSlots/Assets/manabg";
	}
}