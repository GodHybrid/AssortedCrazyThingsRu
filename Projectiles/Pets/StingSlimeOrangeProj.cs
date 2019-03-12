using Terraria;

namespace AssortedCrazyThings.Projectiles.Pets
{
    //check this file for more info vvvvvvvv
    public class StingSlimeOrangeProj : BabySlimeBase
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sting Slime");
            Main.projFrames[projectile.type] = 6;
            Main.projPet[projectile.type] = true;
            drawOffsetX = 0;
            drawOriginOffsetY = 4;
        }

        /*					
            if (projPet[projectile[i].type] && !projectile[i].minion && projectile[i].owner != 255 && projectile[i].damage == 0 && !ProjectileID.Sets.LightPet[projectile[i].type])
            {
	            num3 = player[projectile[i].owner].cPet;
            }
         */

        public override void MoreSetDefaults()
        {
            //used to set dimensions (if necessary) //also use to set projectile.minion
            projectile.width = 52;
            projectile.height = 38;

            projectile.minion = false;
        }

        public override bool PreAI()
        {
            PetPlayer modPlayer = Main.player[projectile.owner].GetModPlayer<PetPlayer>(mod);
            if (Main.player[projectile.owner].dead)
            {
                modPlayer.StingSlimeOrange = false;
            }
            if (modPlayer.StingSlimeOrange)
            {
                projectile.timeLeft = 2;
            }
            return true;
        }
    }
}
