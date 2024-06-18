using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ID;
using Terraria.Audio;

namespace PotionSlots.Core
{
    internal class PotionStoragePlayer : ModPlayer
    {
        public Item lifeSlot;
        public Item manaSlot;
        public Item wormholeSlot;

        public override void Initialize()
        {
            lifeSlot = new Item();
            manaSlot = new Item();
            wormholeSlot = new Item();
        }

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
            tag.Add("wormhole", wormholeSlot);
        }

        public override void LoadData(TagCompound tag)
        {
            lifeSlot = tag.Get<Item>("life");
            manaSlot = tag.Get<Item>("mana");
            wormholeSlot = tag.Get<Item>("wormhole");
        }

        public override bool OnPickup(Item item)
        {
            bool pickedUp = false;

            if (item.healLife > 0)
            {
                if (lifeSlot.IsAir || (lifeSlot.type == item.type && lifeSlot.stack < lifeSlot.maxStack))
                {
                    if (lifeSlot.IsAir)
                    {
                        lifeSlot = item.Clone();
                        lifeSlot.stack = 0;
                    }

                    int transferableAmount = Math.Min(item.stack, lifeSlot.maxStack - lifeSlot.stack);
                    lifeSlot.stack += transferableAmount;
                    item.stack -= transferableAmount;
                    pickedUp = true;

                    if (item.stack <= 0)
                    {
                        item.TurnToAir();
                        SoundEngine.PlaySound(SoundID.Grab);
                        return false;
                    }
                }
            }
            else if (item.healMana > 0)
            {
                if (manaSlot.IsAir || (manaSlot.type == item.type && manaSlot.stack < manaSlot.maxStack))
                {
                    if (manaSlot.IsAir)
                    {
                        manaSlot = item.Clone();
                        manaSlot.stack = 0;
                    }

                    int transferableAmount = Math.Min(item.stack, manaSlot.maxStack - manaSlot.stack);
                    manaSlot.stack += transferableAmount;
                    item.stack -= transferableAmount;
                    pickedUp = true;

                    if (item.stack <= 0)
                    {
                        item.TurnToAir();
                        SoundEngine.PlaySound(SoundID.Grab);
                        return false;
                    }
                }
            }
            else if (item.type == ItemID.WormholePotion)
            {
                if (wormholeSlot.IsAir || (wormholeSlot.type == item.type && wormholeSlot.stack < wormholeSlot.maxStack))
                {
                    if (wormholeSlot.IsAir)
                    {
                        wormholeSlot = item.Clone();
                        wormholeSlot.stack = 0;
                    }

                    int transferableAmount = Math.Min(item.stack, wormholeSlot.maxStack - wormholeSlot.stack);
                    wormholeSlot.stack += transferableAmount;
                    item.stack -= transferableAmount;
                    pickedUp = true;

                    if (item.stack <= 0)
                    {
                        item.TurnToAir();
                        SoundEngine.PlaySound(SoundID.Grab);
                        return false;
                    }
                }
            }

            if (pickedUp)
            {
                SoundEngine.PlaySound(SoundID.Grab);
            }

            return true;
        }
    }
}
