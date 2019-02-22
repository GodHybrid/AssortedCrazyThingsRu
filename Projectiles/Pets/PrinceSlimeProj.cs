using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AssortedCrazyThings.Projectiles.Pets
{
    //check this file for more info vvvvvvvv
    public class PrinceSlimeProj : BabySlimeBase
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prince Slime");
            Main.projFrames[projectile.type] = 6;
            Main.projPet[projectile.type] = true;
            drawOffsetX = 0;
            drawOriginOffsetY = 4;
        }

        public override void MoreSetDefaults()
        {
            //used to set dimensions and damage (if there is, defaults to 0)
            projectile.width = 52;
            projectile.height = 38;

            Damage = 0;
        }

        public override bool PreAI()
        {
            PetPlayer modPlayer = Main.player[projectile.owner].GetModPlayer<PetPlayer>(mod);
            if (Main.player[projectile.owner].dead)
            {
                modPlayer.PrinceSlimePet = false;
            }
            if (modPlayer.PrinceSlimePet)
            {
                projectile.timeLeft = 2;
            }
            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D image = mod.GetTexture("Projectiles/Pets/PrinceSlimeProj_Glowmask");
            Rectangle bounds = new Rectangle
            {
                X = 0,
                Y = projectile.frame,
                Width = image.Bounds.Width,
                Height = image.Bounds.Height / 6
            };
            bounds.Y *= bounds.Height; //cause proj.frame only contains the frame number

            Vector2 stupidOffset = new Vector2(0f, projectile.gfxOffY); //gfxoffY is for when the npc is on a slope or half brick
            SpriteEffects effect = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawOrigin = new Vector2(projectile.width * 0.5f, projectile.height * 0.5f + drawOriginOffsetY);
            Vector2 drawPos = projectile.position - Main.screenPosition + drawOrigin + stupidOffset;

            drawColor.A = 255;

            spriteBatch.Draw(image, drawPos, bounds, drawColor, projectile.rotation, bounds.Size() / 2, projectile.scale, effect, 0f);
        }
    }
}