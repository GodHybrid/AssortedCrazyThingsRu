using Terraria;
using Terraria.ModLoader;

namespace Harblesnargits_Mod_01.Buffs
{
	public class pet_Eye_09 : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Docile Mechanical Eye");
			Description.SetDefault("A Mechanical Eye is following you.");
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.buffTime[buffIndex] = 18000;
            player.GetModPlayer<MyPlayer>(mod).pet_Eye_09 = true;
			bool petProjectileNotSpawned = player.ownedProjectileCounts[mod.ProjectileType("pet_Eye_09")] <= 0;
			if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, mod.ProjectileType("pet_Eye_09"), 0, 0f, player.whoAmI, 0f, 0f);
			}
		}
	}
}