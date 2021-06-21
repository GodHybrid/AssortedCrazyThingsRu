using AssortedCrazyThings.Buffs.Pets;
using AssortedCrazyThings.Projectiles.Pets;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AssortedCrazyThings.Items.Pets
{
    [Content(ContentType.Boss)]
    public class PetHarvesterItem : SimplePetItemBase
    {
        public override int PetType => ModContent.ProjectileType<PetHarvesterProj>();

        public override int BuffType => ModContent.BuffType<PetHarvesterBuff>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stubborn Bird Soul");
            Tooltip.SetDefault("Summons a stubborn bird to follow you");

            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 6));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;

            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SafeSetDefaults()
        {
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Master;
            Item.master = true;
            Item.value = Item.sellPrice(0, 5);
        }
    }
}
