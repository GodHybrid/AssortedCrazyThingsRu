using AssortedCrazyThings.Base;
using AssortedCrazyThings.Buffs;
using AssortedCrazyThings.Projectiles.Minions;
using AssortedCrazyThings.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AssortedCrazyThings.Items.Weapons
{
	[LocalizeEnum(Category = $"Items.{nameof(SlimeHandlerKnapsack)}")]
	public enum SlimeType
	{
		Default,
		Assorted,
		Spiked,
	}

	public sealed class SlimeHandlerKnapsack : MinionItemBase
	{
		public static LocalizedText Enum2string(SlimeType e)
		{
			return AssLocalization.GetEnumText(e);
		}

		//Half-assed implementation
		public static CircleUIConf GetUIConf()
		{
			List<Asset<Texture2D>> assets = new List<Asset<Texture2D>>() {
						AssUtils.Instance.Assets.Request<Texture2D>("Projectiles/Minions/SlimePackMinions/SlimeMinionPreview"),
						AssUtils.Instance.Assets.Request<Texture2D>("Projectiles/Minions/SlimePackMinions/SlimeMinionAssortedPreview"),
						AssUtils.Instance.Assets.Request<Texture2D>("Projectiles/Minions/SlimePackMinions/SlimeMinionSpikedPreview") };
			List<string> tooltips = new List<string>
					{
						Enum2string(SlimeType.Default).ToString()
						+ $"\n{AssLocalization.BaseDamageText.Format(SlimePackMinion.DefDamage)}"
						+ $"\n{AssLocalization.BaseKnockbackText.Format(SlimePackMinion.DefKnockback)}",
						Enum2string(SlimeType.Assorted).ToString()
						+ $"\n{AssLocalization.BaseDamageText.Format(SlimePackMinion.DefDamage)}"
						+ $"\n{AssLocalization.BaseKnockbackText.Format(SlimePackMinion.DefKnockback)}",
						Enum2string(SlimeType.Spiked).ToString()
						+ $"\n{AssLocalization.BaseDamageText.Format(Math.Round(SlimePackMinion.DefDamage * (SlimePackMinion.SpikedIncrease + 1)))}"
						+ $"\n{AssLocalization.BaseKnockbackText.Format(Math.Round(SlimePackMinion.DefKnockback * (SlimePackMinion.SpikedIncrease + 1), 1))}"
						+ $"\n{SpikedBonusText}"
					};
			List<string> toUnlock = new List<string>() { Enum2string(SlimeType.Default).ToString(), Enum2string(SlimeType.Default).ToString(), SpikedUnlockText.ToString() };

			List<bool> unlocked = new List<bool>()
					{
						true,                // 0
                        true,                // 1
                        NPC.downedPlantBoss, // 2
                    };

			return new CircleUIConf(0, -1, assets, unlocked, tooltips, toUnlock, drawOffset: new Vector2(0f, -2f));
		}

		public static LocalizedText SpikedBonusText { get; private set; }
		public static LocalizedText SpikedUnlockText { get; private set; }

		public override void EvenSaferSetStaticDefaults()
		{
			SpikedBonusText = this.GetLocalization("SpikedBonus");
			SpikedUnlockText = this.GetLocalization("SpikedUnlock");

			//Needs to be called so the lang is initialized
			if (!Main.dedServ)
			{
				GetUIConf();
			}
		}

		public override void SetDefaults()
		{
			//change damage in SlimePackMinion.cs
			Item.damage = SlimePackMinion.DefDamage;
			Item.DamageType = DamageClass.Summon;
			Item.mana = 10;
			Item.width = 24;
			Item.height = 30;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.HoldUp; //4 for life crystal
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.value = Item.sellPrice(0, 0, 75, 0);
			Item.rare = 2;
			Item.UseSound = SoundID.Item44;
			Item.shoot = ModContent.ProjectileType<SlimePackMinion>();
			Item.shootSpeed = 10f;
			Item.knockBack = SlimePackMinion.DefKnockback;
			Item.buffType = ModContent.BuffType<SlimePackMinionBuff>();
		}

		public override void ModifyWeaponKnockback(Player player, ref StatModifier knockback)
		{
			AssPlayer mPlayer = player.GetModPlayer<AssPlayer>();
			if (mPlayer.selectedSlimePackMinionType == SlimeType.Spiked)
			{
				knockback *= 1f + SlimePackMinion.SpikedIncrease;
			}
		}

		public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
		{
			AssPlayer mPlayer = player.GetModPlayer<AssPlayer>();
			if (mPlayer.selectedSlimePackMinionType == SlimeType.Spiked)
			{
				damage += SlimePackMinion.SpikedIncrease;
			}
		}

		public override bool SafeShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			AssPlayer mPlayer = player.GetModPlayer<AssPlayer>();
			SlimeType selected = mPlayer.selectedSlimePackMinionType;
			if (selected == SlimeType.Assorted)
			{
				type = ModContent.ProjectileType<SlimePackAssortedMinion>();
			}
			else if (selected == SlimeType.Spiked)
			{
				type = ModContent.ProjectileType<SlimePackSpikedMinion>();
			}
			else
			{
				//default
			}
			Vector2 spawnPos = new Vector2(player.Center.X - player.direction * 12f, player.position.Y - 8f);
			if (Collision.SolidCollision(spawnPos + new Vector2(-player.direction * 18f, 0f), 12, 1))
			{
				spawnPos.X = player.Center.X + player.direction * 8f;
				spawnPos.Y = player.Center.Y;
			}
			int index = Projectile.NewProjectile(source, spawnPos.X, spawnPos.Y, -player.velocity.X, player.velocity.Y - 6f, type, damage, knockback, Main.myPlayer, 0f, 0f);

			int ogDamage = Item.damage;
			if (selected == SlimeType.Spiked)
			{
				ogDamage = (int)(ogDamage * (1f + SlimePackMinion.SpikedIncrease));
			}

			Main.projectile[index].originalDamage = ogDamage;
			return false;
		}

		public override void AddRecipes()
		{
			CreateRecipe(1).AddIngredient(ItemID.SlimeCrown, 1).AddIngredient(ItemID.Gel, 200).AddIngredient(ItemID.SoulofLight, 5).AddIngredient(ItemID.SoulofNight, 5).AddTile(TileID.MythrilAnvil).Register();
		}
	}
}
