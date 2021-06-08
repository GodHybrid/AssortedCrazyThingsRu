using AssortedCrazyThings.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AssortedCrazyThings.Projectiles.Pets
{
    public class GobletProj : ModProjectile
    {
        private int frame2Counter = 0;
        private int frame2 = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Goblet");
            Main.projFrames[Projectile.type] = 12;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BabyGrinch);
            AIType = ProjectileID.BabyGrinch;
            Projectile.width = 24; //40 for flying
            Projectile.height = 38;
            DrawOriginOffsetY = 4;
        }

        public override bool PreAI()
        {
            Player player = Projectile.GetOwner();
            player.grinch = false; // Relic from AIType

            GetFrame();

            return true;
        }

        public bool InAir => Projectile.ai[0] != 0f;

        private void GetFrame()
        {
            if (!InAir) //not flying
            {
                if (Projectile.velocity.Y == 0f)
                {
                    if (Projectile.velocity.X == 0f)
                    {
                        frame2 = 0;
                        frame2Counter = 0;
                    }
                    else if (Projectile.velocity.X < -0.8f || Projectile.velocity.X > 0.8f)
                    {
                        frame2Counter += (int)Math.Abs(Projectile.velocity.X);
                        frame2Counter++;
                        if (frame2Counter > 20) //6
                        {
                            frame2++;
                            frame2Counter = 0;
                        }
                        if (frame2 > 6) //frame 1 to 6 is running
                        {
                            frame2 = 1;
                        }
                    }
                    else
                    {
                        frame2 = 0; //frame 0 is idle
                        frame2Counter = 0;
                    }
                }
                else if (Projectile.velocity.Y != 0f)
                {
                    frame2Counter = 0;
                    frame2 = 7; //frame 7 is jumping
                }
                //projectile.velocity.Y += 0.4f;
                //if (projectile.velocity.Y > 10f)
                //{
                //    projectile.velocity.Y = 10f;
                //}
            }
            else //flying
            {
                if (Projectile.velocity.X <= 0) Projectile.direction = -1;
                else Projectile.direction = 1;
                frame2Counter++;
                if (Projectile.velocity.Length() > 3.6f) Projectile.velocity *= 0.97f;
                if (frame2Counter > 4)
                {
                    frame2++;
                    frame2Counter = 0;
                }
                if (frame2 < 8 || frame2 > 11)
                {
                    frame2 = 8;
                }
                Projectile.rotation = Projectile.velocity.X * 0.01f;
            }
        }

        //public override bool PreDraw(ref Color lightColor)
        //{
        //    lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16), Color.White);
        //    SpriteEffects effects = SpriteEffects.None;
        //    if (Projectile.direction != -1)
        //    {
        //        effects = SpriteEffects.FlipHorizontally;
        //    }
        //    Texture2D image = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
        //    Rectangle bounds = new Rectangle();
        //    bounds.X = 0;
        //    bounds.Width = image.Bounds.Width;
        //    bounds.Height = image.Bounds.Height / Main.projFrames[Projectile.type];
        //    bounds.Y = Projectile.frame * bounds.Height;
        //    Vector2 stupidOffset = new Vector2(10f, 23f + Projectile.gfxOffY);
        //    Main.spriteBatch.Draw(image, Projectile.position - Main.screenPosition + stupidOffset, bounds, lightColor, Projectile.rotation, bounds.Size() / 2, Projectile.scale, effects, 0f);

        //    return false;
        //}

        public override void AI()
        {
            Player player = Projectile.GetOwner();
            PetPlayer modPlayer = player.GetModPlayer<PetPlayer>();
            if (player.dead)
            {
                modPlayer.Goblet = false;
            }
            if (modPlayer.Goblet)
            {
                Projectile.timeLeft = 2;
            }
        }

        public override void PostAI()
        {
            Projectile.frame = frame2;
        }
    }
}
