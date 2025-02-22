using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AssortedCrazyThings.Projectiles.Pets.CuteSlimes
{
	public class CuteSlimeDungeonProj : CuteSlimeBaseProj
	{
		public override ref bool PetBool(Player player) => ref player.GetModPlayer<PetPlayer>().CuteSlimeDungeon;

		public override void SafeSetDefaults()
		{
			Projectile.scale = 1.2f;
			Projectile.alpha = 75;
			DrawOriginOffsetY = -14;
		}

		public override bool SafePreDrawBaseSprite(Color lightColor, bool useNoHair)
		{
			var asset = SheetAdditionAssets[Projectile.type];
			if (asset == null)
			{
				return true;
			}
			int intended = Main.CurrentDrawnEntityShader;
			Main.instance.PrepareDrawnEntityDrawing(Projectile, 0, Projectile.isAPreviewDummy ? Main.UIScaleMatrix : Main.Transform);

			SpriteEffects effects = SpriteEffects.None;
			if (Projectile.spriteDirection == -1)
			{
				effects = SpriteEffects.FlipHorizontally;
			}
			Texture2D image = asset.Value;
			Rectangle frameLocal = image.Frame(SheetCountX, SheetCountY, frameX, frameY);
			Vector2 stupidOffset = new Vector2(Projwidth * 0.5f, 10f + Projectile.gfxOffY);
			Main.spriteBatch.Draw(image, Projectile.position - Main.screenPosition + stupidOffset, frameLocal, lightColor, Projectile.rotation, frameLocal.Size() / 2, Projectile.scale, effects, 0);

			Main.instance.PrepareDrawnEntityDrawing(Projectile, intended, Projectile.isAPreviewDummy ? Main.UIScaleMatrix : Main.Transform);
			return true;
		}
	}
}
