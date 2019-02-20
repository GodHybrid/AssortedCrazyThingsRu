﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AssortedCrazyThings.Projectiles.Pets
{
    public class SunPetProj : ModProjectile
    {
        public override string Texture
        {
            get
            {
                return "AssortedCrazyThings/Projectiles/Pets/SunPetProj_0"; //temp
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Personal Sun");
            Main.projFrames[projectile.type] = 1;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.DD2PetGhost);
            projectile.aiStyle = -1;
            projectile.width = 40;
            projectile.height = 40;
            projectile.alpha = 0;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Lighting.AddLight(projectile.Center, Vector3.One);

            int texture = 0;
            if (Main.eclipse) //takes priority
            {
                texture = 2;
            }
            else if (Main.player[projectile.owner].head == 12)
            {
                texture = 1;
            }

            Texture2D image = AssortedCrazyThings.sunPetTextures[texture];

            Vector2 stupidOffset = new Vector2(projectile.width / 2, (projectile.height - 18f));
            Vector2 drawPos = projectile.position - Main.screenPosition + stupidOffset;

            spriteBatch.Draw(image, drawPos, image.Bounds, lightColor, 0f, image.Bounds.Size() / 2, 1f, SpriteEffects.None, 0f);
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            PetPlayer modPlayer = player.GetModPlayer<PetPlayer>(mod);
            if (player.dead)
            {
                modPlayer.SunPet = false;
            }
            if (modPlayer.SunPet)
            {
                projectile.timeLeft = 2;

                CompanionDungeonSoulPetProj.FlickerwickPetAI(projectile, lightPet: false, lightDust: false, offsetX: 20f, offsetY: -32f);
            }
        }
    }
}
