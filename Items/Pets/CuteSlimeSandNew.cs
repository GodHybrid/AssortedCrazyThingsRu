using AssortedCrazyThings.Buffs;
using AssortedCrazyThings.Projectiles.Pets;
using Terraria;
using Terraria.ID;

namespace AssortedCrazyThings.Items.Pets
{
    public class CuteSlimeSandNew : CuteSlimeItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bottled Cute Sand Slime");
            Tooltip.SetDefault("Summons a friendly Cute Sand Slime to follow you");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.LizardEgg);
            item.shoot = mod.ProjectileType<CuteSlimeSandNewProj>();
            item.buffType = mod.BuffType<CuteSlimeSandNewBuff>();
            item.rare = -11;
            item.value = Item.sellPrice(copper: 10);
        }
    }
}
