using AssortedCrazyThings.Buffs;
using AssortedCrazyThings.Projectiles.Pets;
using Terraria;
using Terraria.ID;

namespace AssortedCrazyThings.Items.Pets
{
    public class CuteSlimeBlueNew : CuteSlimeItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bottled Cute Blue Slime");
            Tooltip.SetDefault("Summons a friendly Cute Blue Slime to follow you");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.LizardEgg);
            item.shoot = mod.ProjectileType<CuteSlimeBlueNewProj>();
            item.buffType = mod.BuffType<CuteSlimeBlueNewBuff>();
            item.rare = -11;
            item.value = Item.sellPrice(copper: 10);
        }
    }
}
