using AssortedCrazyThings.Base;
using AssortedCrazyThings.Base.ModSupport.AoMM;
using AssortedCrazyThings.Buffs.Pets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AssortedCrazyThings.Projectiles.Pets
{
	public class PetGoldfishProj : SimplePetProjBase
	{
		public override string Texture
		{
			get
			{
				return "AssortedCrazyThings/Projectiles/Pets/PetGoldfishProj_0"; //temp
			}
		}

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 10;
			Main.projPet[Projectile.type] = true;

			ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(1, 6 - 1, 6)
				.WhenNotSelected(0, 0)
				.WithOffset(0f, 0f)
				.WithSpriteDirection(-1);

			AmuletOfManyMinionsApi.RegisterFlyingPet(this, ModContent.GetInstance<PetGoldfishBuff_AoMM>(), null);
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.BabyGrinch);
			Projectile.height = 24;
			Projectile.width = 24;
			AIType = ProjectileID.BabyGrinch;
		}

		private bool Swimming { get; set; }

		/// <summary>
		/// Player owner direction
		/// </summary>
		private int Direction { get; set; }

		private int Timer { get; set; }

		private int SavedIndex
		{
			get
			{
				return (int)Projectile.ai[1];
			}
			set
			{
				Projectile.ai[1] = value;
			}
		}

		private const int Width = 8;
		private const int Height = 4;

		private Point16 GetTilePosFromIndex(int index, Player player)
		{
			int indexX = index % Width;
			int indexY = index / Width;
			indexX *= Direction;

			Point16 startingPos = GetStartingPos(player);
			int tileX = startingPos.X + indexX;
			int tileY = startingPos.Y + indexY;
			return new Point16(tileX, tileY);
		}

		private Point16 GetStartingPos(Player player)
		{
			Point16 playerOrigin = new Point16((int)(player.Center.X / 16), (int)(player.Bottom.Y / 16));
			return new Point16(playerOrigin.X + (Direction * -(Width / 2)), playerOrigin.Y - Height);
		}

		private void UpdateSavedIndex(Player player)
		{
			if (Timer > 30)
			{
				Timer = 0;
				for (int index = 0; index < Width * Height; index++)
				{
					Direction = player.direction;
					Point16 tilePos = GetTilePosFromIndex(index, player);
					if (Framing.GetTileSafely(tilePos.X, tilePos.Y).LiquidAmount == 255 && Framing.GetTileSafely(tilePos.X, tilePos.Y - 1).LiquidAmount == 255 && Framing.GetTileSafely(tilePos.X - player.direction, tilePos.Y).LiquidAmount == 255)
					{
						SavedIndex = index;
						break;
					}
					SavedIndex = -1;
				}

				//DEBUG
				//for (int index = 0; index < Width * Height; index++)
				//{
				//    Point16 tilePos = GetTilePosFromIndex(index, player);
				//    AssUtils.DrawDustAtPos(new Vector2(tilePos.X, tilePos.Y) * 16);
				//}
			}
		}

		private Vector2 GetDesiredCenter(Player player)
		{
			/* checked area is given by Width and Height, starting at startingPos as top left/right corner (depending on player direction)
             *
             * +-4 : -4 from player.bottom
             * 
             * start checking at +-8
             */
			Vector2 desiredCenter = new Vector2(0f, player.width / 2);
			Point16 playerOrigin = new Point16((int)(player.Center.X / 16), (int)(player.Bottom.Y / 16));

			UpdateSavedIndex(player);

			if (SavedIndex != -1)
			{
				Point16 point = GetTilePosFromIndex(SavedIndex, player);
				//DEBUG
				//AssUtils.DrawDustAtPos(point.ToWorldCoordinates(0, 0), 1);

				//player.direction * 8 makes it so it has some space to swim, instead of constantly being stuck against a wall if there is one
				desiredCenter = point.ToWorldCoordinates(player.direction * 8, 8) - playerOrigin.ToWorldCoordinates(0, 0);
			}

			return desiredCenter;
		}

		private void SwimmingZephyrfishAI()
		{
			Player player = Projectile.GetOwner();
			if (!player.active)
			{
				Projectile.active = false;
				return;
			}
			Timer++;

			if (Projectile.wet)
			{
				Projectile.tileCollide = true;
			}
			else
			{
				Projectile.tileCollide = false;
			}

			float num17 = 0.3f;
			int num18 = 100;
			Vector2 between = player.Center - Projectile.Center;

			between += GetDesiredCenter(player) + new Vector2(Main.rand.Next(-10, 21), Main.rand.Next(-10, 21));

			float distance = between.Length();
			if (distance < num18 && player.velocity.Y == 0f && Projectile.position.Y + Projectile.height <= player.position.Y + player.height && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
			{
				if (Projectile.velocity.Y < -6f)
				{
					Projectile.velocity.Y = -6f;
				}
			}
			if (distance < 50f)
			{
				if (Math.Abs(Projectile.velocity.X) > 2f || Math.Abs(Projectile.velocity.Y) > 2f)
				{
					Projectile.velocity *= 0.99f;
				}
				num17 = 0.01f;
			}
			else
			{
				if (distance < 100f)
				{
					num17 = 0.1f;
				}
				if (distance > 300f)
				{
					num17 = 0.4f;
				}
				between.Normalize();
				between *= 6f;
			}
			if (Projectile.velocity.X < between.X)
			{
				Projectile.velocity.X = Projectile.velocity.X + num17;
				if (num17 > 0.05f && Projectile.velocity.X < 0f)
				{
					Projectile.velocity.X = Projectile.velocity.X + num17;
				}
			}
			if (Projectile.velocity.X > between.X)
			{
				Projectile.velocity.X = Projectile.velocity.X - num17;
				if (num17 > 0.05f && Projectile.velocity.X > 0f)
				{
					Projectile.velocity.X = Projectile.velocity.X - num17;
				}
			}
			if (Projectile.velocity.Y < between.Y)
			{
				Projectile.velocity.Y = Projectile.velocity.Y + num17;
				if (num17 > 0.05f && Projectile.velocity.Y < 0f)
				{
					Projectile.velocity.Y = Projectile.velocity.Y + num17 * 2f;
				}
			}
			if (Projectile.velocity.Y > between.Y)
			{
				Projectile.velocity.Y = Projectile.velocity.Y - num17;
				if (num17 > 0.05f && Projectile.velocity.Y > 0f)
				{
					Projectile.velocity.Y = Projectile.velocity.Y - num17 * 2f;
				}
			}

			//fix, direction gets set automatically by tmodloader based on velocity.X for some reason
			if (Projectile.velocity.X > 0.25f)
			{
				Projectile.ai[0] = -1;
			}
			else if (Projectile.velocity.X < -0.25f)
			{
				Projectile.ai[0] = 1;
			}
			Projectile.direction = (int)-Projectile.ai[0];
			Projectile.spriteDirection = Projectile.direction;

			Projectile.rotation = Projectile.velocity.X * 0.05f;
		}

		private void GetFrame()
		{
			if (Swimming)
			{
				frame2Counter++;
				if (frame2Counter > 5)
				{
					frame2++;
					frame2Counter = 0;
				}
				if (frame2 < 6 || frame2 > 9)
				{
					frame2 = 6;
				}
				return;
			}

			if (Projectile.ai[0] == 0) //not flying
			{
				if (Projectile.velocity.Y == 0f)
				{
					float xAbs = Math.Abs(Projectile.velocity.X);
					if (Projectile.velocity.X == 0f)
					{
						frame2 = 0;
						frame2Counter = 0;
					}
					else if (xAbs > 0.5f)
					{
						frame2Counter += (int)(2 * xAbs);
						frame2Counter++;
						if (frame2Counter > 20) //6
						{
							frame2++;
							frame2Counter = 0;
						}
						if (frame2 > 5) //frame 1 to 5 is running
						{
							frame2 = 1;
						}
					}
					else
					{
						frame2 = 0; //frame 0 is idle
						frame2Counter = 10;
					}
				}
				else if (Projectile.velocity.Y != 0f)
				{
					frame2Counter = 0;
					frame2 = 1; //frame 1 is jumping
				}
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
				if (frame2 < 6 || frame2 > 9)
				{
					frame2 = 6;
				}
				Projectile.rotation = Projectile.velocity.X * 0.02f;
			}
		}

		private int frame2Counter = 0;
		private int frame2 = 0;

		bool didZephyrfishAILastTick = false;

		public override bool PreAI()
		{
			Player player = Projectile.GetOwner();
			PetPlayer modPlayer = player.GetModPlayer<PetPlayer>();

			player.grinch = false;

			if (player.dead)
			{
				modPlayer.PetGoldfish = false;
			}
			if (modPlayer.PetGoldfish)
			{
				Projectile.timeLeft = 2;
			}

			bool desiredSwimming = player.wet;

			if (AmuletOfManyMinionsApi.IsActive(this) && AmuletOfManyMinionsApi.IsAttacking(this))
			{
				desiredSwimming = true;
			}

			Swimming = desiredSwimming;

			GetFrame();

			if (Swimming)
			{
				didZephyrfishAILastTick = true;
				SwimmingZephyrfishAI();
				return false;
			}
			if (didZephyrfishAILastTick)
			{
				didZephyrfishAILastTick = false;
				Timer = 0;
				Projectile.ai[0] = 0; //reset from ZephyrfishAI();
				Projectile.ai[1] = 0; //reset for vanilla AI?;
			}

			return true;
		}

		public override void PostAI()
		{
			Projectile.frameCounter = 0;
			Projectile.frame = frame2;
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			//If in water, deal more damage
			float mult = 1f;
			if (Projectile.GetOwner().wet)
			{
				mult += 0.5f;
			}
			if (Projectile.wet)
			{
				mult += 0.5f;
			}
			modifiers.SourceDamage *= mult;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			PetPlayer mPlayer = Projectile.GetOwner().GetModPlayer<PetPlayer>();
			Texture2D image = Mod.Assets.Request<Texture2D>("Projectiles/Pets/PetGoldfishProj_" + mPlayer.petGoldfishType).Value;
			Rectangle bounds = image.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);
			Vector2 stupidOffset = new Vector2(Projectile.width * 0.5f, Projectile.height * 0.5f - 2 + Projectile.gfxOffY);

			if (mPlayer.petGoldfishType == 4)
			{
				//Ghost variant
				lightColor = Color.Lerp(lightColor, Color.White, 0.4f);
			}

			Main.EntitySpriteDraw(image, Projectile.position - Main.screenPosition + stupidOffset, bounds, lightColor, Projectile.rotation, bounds.Size() / 2, Projectile.scale, effects, 0);

			return false;
		}
	}
}
