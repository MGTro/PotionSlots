using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;

namespace PotionSlots.Core
{
	internal class PotionStoragePlayer : ModPlayer
	{
		public Item lifeSlot;
		public Item manaSlot;

		public override void Load()
		{
			On_Player.QuickHeal_GetItemToUse += PickLifeSlot;
			On_Player.QuickMana_GetItemToUse += PickManaSlot;
		}

		private Item PickLifeSlot(On_Player.orig_QuickHeal_GetItemToUse orig, Player self)
		{
			var lifeSlot = self.GetModPlayer<PotionStoragePlayer>().lifeSlot;
			if (lifeSlot != null && !lifeSlot.IsAir)
				return lifeSlot;

			return orig(self);
		}

		private Item PickManaSlot(On_Player.orig_QuickMana_GetItemToUse orig, Player self)
		{
			var manaSlot = self.GetModPlayer<PotionStoragePlayer>().manaSlot;
			if (manaSlot != null && !manaSlot.IsAir)
				return manaSlot;

			return orig(self);
		}

		public override void SaveData(TagCompound tag)
		{
			tag.Add("life", lifeSlot);
			tag.Add("mana", manaSlot);
		}

		public override void LoadData(TagCompound tag)
		{
			lifeSlot = tag.Get<Item>("life");
			manaSlot = tag.Get<Item>("mana");
		}
	}
}
