﻿using AssortedCrazyThings.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;

namespace AssortedCrazyThings.Projectiles.Minions.Drones
{
    /// <summary>
    /// Fires a salvo of homing rockets with a long delay
    /// </summary>
    public class MissileDrone : DroneBase
    {
        public override string Texture
        {
            get
            {
                return "AssortedCrazyThings/Projectiles/Minions/Drones/HealingDrone";
            }
        }

        private static readonly string nameGlow = "Projectiles/Minions/Drones/" + "HealingDrone_Glowmask";
        private static readonly string nameLower = "Projectiles/Minions/Drones/" + "HealingDrone_Lower";
        private static readonly string nameLowerGlow = "Projectiles/Minions/Drones/" + "HealingDrone_Lower_Glowmask";

        public const int AttackCooldown = 180; //120 but incremented by 1.5f
        public const int AttackDelay = 60;
        public const int AttackDuration = 60;

        //in cooldown: lamps turn on in order for "charge up"

        private const byte STATE_COOLDOWN = 0;
        private const byte STATE_IDLE = 1;
        private const byte STATE_FIRING = 2;

        private byte AI_STATE = 0;
        private int RocketNumber = 0;

        private Vector2 ShootOrigin
        {
            get
            {
                Vector2 position = projectile.Center;
                position.X = projectile.position.X + RocketNumber * 12f;
                position.Y += sinY;
                return position;
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Missile Drone");
            Main.projFrames[projectile.type] = 6;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.DD2PetGhost);
            projectile.aiStyle = -1;
            projectile.width = 38;
            projectile.height = 30;
            projectile.alpha = 0;
            projectile.minion = true;
            projectile.minionSlots = 2f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write((byte)AI_STATE);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            AI_STATE = reader.ReadByte();
        }

        protected override void CustomFrame(int frameCounterMaxFar = 4, int frameCounterMaxClose = 8)
        {
            //frame 0, 1: above two thirds health
            //frame 2, 3: above half health, below two thirds health
            //frame 4, 5: below half health, healing
            Player player = Main.player[projectile.owner];

            int frameOffset = 0; //frame 0, 1

            if (AI_STATE == STATE_FIRING) //frame 4, 5
            {
                frameOffset = 4;
            }
            else if (AI_STATE == STATE_COOLDOWN) //frame 2, 3
            {
                frameOffset = 2;
            }
            else //AI_STATE == STATE_IDLE
            {
                //frameoffset 0
            }

            if (projectile.frame < frameOffset) projectile.frame = frameOffset;

            if (projectile.velocity.Length() > 6f)
            {
                if (++projectile.frameCounter >= frameCounterMaxFar)
                {
                    projectile.frameCounter = 0;
                    if (++projectile.frame >= 2 + frameOffset)
                    {
                        projectile.frame = frameOffset;
                    }
                }
            }
            else
            {
                if (++projectile.frameCounter >= frameCounterMaxClose)
                {
                    projectile.frameCounter = 0;
                    if (++projectile.frame >= 2 + frameOffset)
                    {
                        projectile.frame = frameOffset;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D image = Main.projectileTexture[projectile.type];
            Rectangle bounds = new Rectangle();
            bounds.X = 0;
            bounds.Width = image.Bounds.Width;
            bounds.Height = (image.Bounds.Height / Main.projFrames[projectile.type]);
            bounds.Y = projectile.frame * bounds.Height;

            SpriteEffects effects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 stupidOffset = new Vector2(projectile.width / 2, (projectile.height - 8f) + sinY);
            Vector2 drawPos = projectile.position - Main.screenPosition + stupidOffset;
            Vector2 drawOrigin = bounds.Size() / 2;

            spriteBatch.Draw(image, drawPos, bounds, lightColor, projectile.rotation, drawOrigin, 1f, effects, 0f);

            image = mod.GetTexture(nameGlow);
            spriteBatch.Draw(image, drawPos, bounds, Color.White, projectile.rotation, drawOrigin, 1f, effects, 0f);

            Vector2 rotationOffset = new Vector2(0f, -2f);
            drawPos += rotationOffset;
            drawOrigin += rotationOffset;

            //AssUtils.ShowDustAtPos(135, projectile.position + stupidOffset);

            //AssUtils.ShowDustAtPos(136, projectile.position + stupidOffset - drawOrigin);

            //rotation origin is (projectile.position + stupidOffset) - drawOrigin; //not including Main.screenPosition
            image = mod.GetTexture(nameLower);
            bounds.Y = 0;
            spriteBatch.Draw(image, drawPos, bounds, lightColor, projectile.rotation, drawOrigin, 1f, effects, 0f);

            image = mod.GetTexture(nameLowerGlow);
            bounds.Y = 0;
            spriteBatch.Draw(image, drawPos, bounds, Color.White, projectile.rotation, drawOrigin, 1f, effects, 0f);

            return false;
        }

        protected override void ModifyDroneControllerHeld(ref float dmgModifier, ref float kbModifier)
        {
            dmgModifier = 1.1f;
            kbModifier = 1.1f;

            if (AI_STATE == STATE_COOLDOWN)
            {
                Counter += Main.rand.Next(2);
            }
        }

        protected override void CustomAI()
        {
            Player player = Main.player[projectile.owner];

            #region Handle State
            if (AI_STATE == STATE_COOLDOWN)
            {
                if (Counter > AttackCooldown)
                {
                    Counter = 0;
                    //Main.NewText("Change from cooldown to idle");
                    AI_STATE = STATE_IDLE;
                    if (RealOwner) projectile.netUpdate = true;
                }
                //else stay in cooldown and wait for counter to reach 
            }
            else if (AI_STATE == STATE_IDLE)
            {
                if (Counter > AttackDelay)
                {
                    Counter = 0;
                    int targetIndex = AssAI.FindTarget(projectile, projectile.Center, 900);
                    if (targetIndex != -1)
                    {
                        Vector2 aboveCheck = new Vector2(0, -16 * 8);
                        if (Collision.CanHitLine(ShootOrigin, 1, 1, ShootOrigin + aboveCheck, 1, 1) &&
                            Collision.CanHitLine(ShootOrigin + aboveCheck, 1, 1, ShootOrigin + aboveCheck + new Vector2(-16 * 5, 0), 1, 1) &&
                            Collision.CanHitLine(ShootOrigin + aboveCheck, 1, 1, ShootOrigin + aboveCheck + new Vector2(16 * 5, 0), 1, 1))
                        {
                            //Main.NewText("Change from idle to firing");
                            AI_STATE = STATE_FIRING;
                            if (RealOwner) projectile.netUpdate = true;
                        }
                    }
                    //else stay in idle until target found
                }
            }
            else if (AI_STATE == STATE_FIRING)
            {
                if (Counter > AttackDuration)
                {
                    Counter = 0;
                    RocketNumber = 0;
                    AI_STATE = STATE_COOLDOWN;
                    //no sync since this counts down automatically after STATE_FIRING
                }
            }
            #endregion

            if (AI_STATE == STATE_FIRING)
            {
                int targetIndex = AssAI.FindTarget(projectile, projectile.Center, 900);

                if (targetIndex != -1)
                {
                    projectile.spriteDirection = projectile.direction = -(Main.npc[targetIndex].Center.X - player.Center.X > 0f).ToDirectionInt();
                    if (RealOwner)
                    {
                        int firerate = AttackDelay / 4;
                        RocketNumber = Counter / firerate;
                        if (Counter % firerate == 0 && RocketNumber < 3)
                        {
                            if (!Collision.SolidCollision(ShootOrigin, 1, 1))
                            {
                                Projectile.NewProjectile(ShootOrigin, new Vector2(Main.rand.NextFloat(-1, 1) + RocketNumber - 1, -5), mod.ProjectileType<MissileDroneRocket>(), CustomDmg, CustomKB, Main.myPlayer, 0f, 0f);
                                projectile.velocity.Y += 2f;
                                projectile.netUpdate = true;
                            }
                        }
                    }
                }
            }

            Counter += Main.rand.Next(1, AI_STATE != STATE_IDLE ? 2 : 3);
        }
    }
}