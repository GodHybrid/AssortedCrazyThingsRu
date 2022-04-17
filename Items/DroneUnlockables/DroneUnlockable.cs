using AssortedCrazyThings.Items.Weapons;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AssortedCrazyThings.Items.DroneUnlockables
{
	[Content(ContentType.Weapons)]
	public abstract class DroneUnlockable : AssItem
	{
		public sealed override void SetStaticDefaults()
		{
			Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			SafeSetStaticDefaults();
		}

		public virtual void SafeSetStaticDefaults()
		{

		}

		public override void SetDefaults()
		{
			Item.maxStack = 999;
			Item.rare = 4;
			Item.width = 26;
			Item.height = 24;
			Item.consumable = true;
			Item.maxStack = 1;
			Item.UseSound = SoundID.Item4;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.value = Item.sellPrice(silver: 50);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			AssPlayer mPlayer = Main.LocalPlayer.GetModPlayer<AssPlayer>();
			string tooltip = DroneController.GetDroneData(UnlockedType).Name + " for the Drone Controller";
			if (!mPlayer.droneControllerUnlocked.HasFlag(UnlockedType))
			{
				tooltip = "Unlocks the " + tooltip;
			}
			else
			{
				tooltip = "Already unlocked " + tooltip;
			}
			tooltips.Add(new TooltipLine(Mod, "Unlocks", tooltip));
		}

		public abstract DroneType UnlockedType { get; }

		public override bool CanUseItem(Player player)
		{
			if (Main.netMode != NetmodeID.Server && Main.myPlayer == player.whoAmI)
			{
				AssPlayer mPlayer = player.GetModPlayer<AssPlayer>();
				if (!mPlayer.droneControllerUnlocked.HasFlag(UnlockedType))
				{
					return true;
				}
			}
			return false;
		}

		public override bool? UseItem(Player player)
		{
			if (Main.netMode != NetmodeID.Server && Main.myPlayer == player.whoAmI)
			{
				player.GetModPlayer<AssPlayer>().droneControllerUnlocked |= UnlockedType;
				Main.NewText("Unlocked: " + DroneController.GetDroneData(UnlockedType).Name, CombatText.HealLife);
			}
			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe(1)
				.AddIngredient(ModContent.ItemType<DroneParts>())
				.AddCondition(new Recipe.Condition(NetworkText.FromLiteral("If it isn't unlocked yet"),
				(Recipe recipe) => !Main.LocalPlayer.GetModPlayer<AssPlayer>().droneControllerUnlocked.HasFlag(UnlockedType)))
				.Register();
		}
	}

	public class DroneUnlockableBasicLaserDrone : DroneUnlockable
	{
		public override void SafeSetStaticDefaults()
		{
			DisplayName.SetDefault("Basic Laser Drone Components");
		}

		public override DroneType UnlockedType => DroneType.BasicLaser;
	}

	public class DroneUnlockableHeavyLaserDrone : DroneUnlockable
	{
		public override void SafeSetStaticDefaults()
		{
			DisplayName.SetDefault("Heavy Laser Drone Components");
		}

		public override DroneType UnlockedType => DroneType.HeavyLaser;
	}

	public class DroneUnlockableMissileDrone : DroneUnlockable
	{
		public override void SafeSetStaticDefaults()
		{
			DisplayName.SetDefault("Missile Drone Components");
		}

		public override DroneType UnlockedType => DroneType.Missile;
	}

	public class DroneUnlockableHealingDrone : DroneUnlockable
	{
		public override void SafeSetStaticDefaults()
		{
			DisplayName.SetDefault("Healing Drone Components");
		}

		public override DroneType UnlockedType => DroneType.Healing;
	}

	public class DroneUnlockableShieldDrone : DroneUnlockable
	{
		public override void SafeSetStaticDefaults()
		{
			DisplayName.SetDefault("Shield Drone Components");
		}

		public override DroneType UnlockedType => DroneType.Shield;
	}
}
