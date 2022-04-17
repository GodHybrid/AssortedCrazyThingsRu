using AssortedCrazyThings.Buffs.Pets;
using AssortedCrazyThings.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AssortedCrazyThings.Items.Pets
{
	[Content(ContentType.HostileNPCs)]
	[LegacyName("ChunkyandMeatball")]
	public class ChunkyandMeatballItem : SimplePetItemBase
	{
		public override int PetType => ModContent.ProjectileType<ChunkyProj>();

		public override int BuffType => ModContent.BuffType<ChunkyandMeatballBuff>();

		public override void SafeSetStaticDefaults()
		{
			DisplayName.SetDefault("Chunky and Meatball");
			Tooltip.SetDefault("Summons a pair of inseperable brothers to follow you");
		}

		public override void SafeSetDefaults()
		{
			Item.value = Item.sellPrice(silver: 4);
		}

		public override void AddRecipes()
		{
			CreateRecipe(1).AddIngredient<ChunkysEyeItem>().AddIngredient<MeatballsEyeItem>().AddTile(TileID.DemonAltar).Register();
		}
	}
}