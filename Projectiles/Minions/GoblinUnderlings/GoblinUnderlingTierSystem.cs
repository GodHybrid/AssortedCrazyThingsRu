﻿using AssortedCrazyThings.Base.Chatter.GoblinUnderlings;
using AssortedCrazyThings.Projectiles.Minions.GoblinUnderlings.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace AssortedCrazyThings.Projectiles.Minions.GoblinUnderlings
{
	//flinx: 22 dps (dummy) -> matches preboss tier
	//frog: 68 dps (dummy). skeleton: 35. skeleton archer: 24 -> matches skeletron tier
	//blade: 30 dps (dummy). skeleton archer: 30
	//optic: 80 dps (dummy), skeleton archer: 55
	//xeno: 90 dps (dummy), armored skeleton: 55
	public enum GoblinUnderlingProgressionTierStage : int
	{
		//Value important, texture index, ordered by progression
		PreBoss = 0,
		EoC = 1,
		Evil = 2,
		Skeletron = 3,
		WoF = 4,
		Mech = 5,
		Plantera = 6,
		Cultist = 7
	}

	[Content(ContentType.Weapons)]
	public class GoblinUnderlingTierSystem : AssSystem
	{
		public static Dictionary<int, GoblinUnderlingChatterType> GoblinUnderlingProjs { get; private set; }

		private static Dictionary<GoblinUnderlingClass, Dictionary<GoblinUnderlingProgressionTierStage, GoblinUnderlingTierStats>> tierStats = new();
		private static Dictionary<GoblinUnderlingProgressionTierStage, Func<bool>> tiers = new();

		public static int TierCount => tiers.Count;
		public static GoblinUnderlingProgressionTierStage CurrentTier { get; private set; }
		public static int CurrentTierIndex => (int)CurrentTier;

		public static List<GoblinUnderlingProgressionTierStage> GetTiers()
		{
			return tiers.Keys.ToList();
		}

		public static void RegisterStats(GoblinUnderlingClass @class, Dictionary<GoblinUnderlingProgressionTierStage, GoblinUnderlingTierStats> stats)
		{
			tierStats[@class] = stats;
		}

		public static GoblinUnderlingTierStats GetCurrentTierStats(GoblinUnderlingClass @class)
		{
			return tierStats[@class][CurrentTier];
		}

		private static void LoadTiers()
		{
			tiers = new Dictionary<GoblinUnderlingProgressionTierStage, Func<bool>>()
			{
				{ GoblinUnderlingProgressionTierStage.PreBoss, () => true },
				{ GoblinUnderlingProgressionTierStage.EoC, () => NPC.downedBoss1 },
				{ GoblinUnderlingProgressionTierStage.Evil, () => NPC.downedBoss2 },
				{ GoblinUnderlingProgressionTierStage.Skeletron, () => NPC.downedBoss3 },
				{ GoblinUnderlingProgressionTierStage.WoF, () => Main.hardMode },
				{ GoblinUnderlingProgressionTierStage.Mech, () => NPC.downedMechBossAny },
				{ GoblinUnderlingProgressionTierStage.Plantera, () => NPC.downedPlantBoss },
				{ GoblinUnderlingProgressionTierStage.Cultist, () => NPC.downedAncientCultist },
			};
		}

		private static void DetermineCurrentProgressionTier()
		{
			CurrentTier = GoblinUnderlingProgressionTierStage.PreBoss;
			for (int i = TierCount - 1; i >= 0; i--)
			{
				var tier = (GoblinUnderlingProgressionTierStage)i;
				//Start from last tier, prioritize
				var tierCondition = tiers[tier];
				if (tierCondition.Invoke())
				{
					CurrentTier = tier;
					break;
				}
			}
		}

		public override void OnModLoad()
		{
			GoblinUnderlingProjs = new();

			LoadTiers();
		}

		public override void Unload()
		{
			GoblinUnderlingProjs = null;

			tiers = null;
			tierStats = null;
		}

		public override void PostSetupContent()
		{
			var tierStats = new Dictionary<GoblinUnderlingProgressionTierStage, GoblinUnderlingTierStats>
			{
				//PreBoss = Baseline values in Item/AI code																									   //dmg    kb    ap  sp     m  hb  ran   ransp ranmp
				{ GoblinUnderlingProgressionTierStage.PreBoss   , new GoblinUnderlingMeleeTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponDart_0>()   , 1f   , 1f  , 0 , 0.3f , 6, 0 , 1.5f, 8f , 1f) },
				{ GoblinUnderlingProgressionTierStage.EoC       , new GoblinUnderlingMeleeTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponDart_1>()   , 1.50f, 1.2f, 0 , 0.35f, 6, 2 , 1.5f, 9f , 1.2f) },
				{ GoblinUnderlingProgressionTierStage.Evil      , new GoblinUnderlingMeleeTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponDart_2>()   , 1.70f, 1.4f, 5 , 0.4f , 6, 4 , 1.5f, 10f, 1.3f) },
				{ GoblinUnderlingProgressionTierStage.Skeletron , new GoblinUnderlingMeleeTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponDart_3>()   , 1.75f, 1.6f, 5 , 0.45f, 5, 6 , 1.5f, 11f, 1.4f) },
				{ GoblinUnderlingProgressionTierStage.WoF       , new GoblinUnderlingMeleeTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponDart_3>()   , 2.4f , 1.8f, 10, 0.45f, 5, 6 , 1.5f, 11f, 1.6f) }, //Mostly a copy of previous tier with more damage, same visuals too
				{ GoblinUnderlingProgressionTierStage.Mech      , new GoblinUnderlingMeleeTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponDart_4>()   , 2.8f , 2.0f, 10, 0.6f , 5, 6 , 1.5f, 12f, 1.7f) },
				{ GoblinUnderlingProgressionTierStage.Plantera  , new GoblinUnderlingMeleeTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponTerraBeam>(), 3.0f , 2.2f, 10, 0.7f , 4, 10, 1f  , 14f, 2f, -1, 0, showMeleeDuringRanged: true) },
				{ GoblinUnderlingProgressionTierStage.Cultist   , new GoblinUnderlingMeleeTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponDaybreak>() , 3.25f, 2.4f, 10, 0.8f , 4, 10, 1f  , 16f, 2f, showMeleeDuringRanged: false, rangedOnly: true) },
			};
			RegisterStats(GoblinUnderlingClass.Melee, tierStats);

			//TODO stats
			tierStats = new Dictionary<GoblinUnderlingProgressionTierStage, GoblinUnderlingTierStats>
			{
				//PreBoss = Baseline values in Item/AI code																									   //dmg    kb    ap  sp     m  hb  ran   ransp ranmp
				{ GoblinUnderlingProgressionTierStage.PreBoss   , new GoblinUnderlingMeleeTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponDart_0>()   , 1f   , 1f  , 0 , 0.3f , 6, 0 , 1.5f, 8f , 1f) },
				{ GoblinUnderlingProgressionTierStage.EoC       , new GoblinUnderlingMeleeTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponDart_1>()   , 1.50f, 1.2f, 0 , 0.35f, 6, 2 , 1.5f, 9f , 1.2f) },
				{ GoblinUnderlingProgressionTierStage.Evil      , new GoblinUnderlingMeleeTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponDart_2>()   , 1.70f, 1.4f, 5 , 0.4f , 6, 4 , 1.5f, 10f, 1.3f) },
				{ GoblinUnderlingProgressionTierStage.Skeletron , new GoblinUnderlingMeleeTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponDart_3>()   , 1.75f, 1.6f, 5 , 0.45f, 5, 6 , 1.5f, 11f, 1.4f) },
				{ GoblinUnderlingProgressionTierStage.WoF       , new GoblinUnderlingMeleeTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponDart_3>()   , 2.4f , 1.6f, 10, 0.45f, 5, 6 , 1.5f, 11f, 1.6f) }, //Mostly a copy of previous tier with more damage, same visuals too
				{ GoblinUnderlingProgressionTierStage.Mech      , new GoblinUnderlingMeleeTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponDart_4>()   , 2.8f , 1.8f, 10, 0.6f , 5, 6 , 1.5f, 12f, 1.7f) },
				{ GoblinUnderlingProgressionTierStage.Plantera  , new GoblinUnderlingMeleeTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponTerraBeam>(), 3.0f , 2f  , 10, 0.7f , 4, 10, 1f  , 14f, 2f, -1, 0, showMeleeDuringRanged: true) },
				{ GoblinUnderlingProgressionTierStage.Cultist   , new GoblinUnderlingMeleeTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponDaybreak>() , 3.25f, 2.2f, 10, 0.8f , 4, 10, 1f  , 16f, 2f, showMeleeDuringRanged: false, rangedOnly: true) },
			};
			RegisterStats(GoblinUnderlingClass.Magic, tierStats);

			//TODO stats
			//TODO increase range, adjust arrow speed
			tierStats = new Dictionary<GoblinUnderlingProgressionTierStage, GoblinUnderlingTierStats>
			{
				//PreBoss = Baseline values in Item/AI code																									  //dmg   kb    ap  sp     m ransp ranmp
				{ GoblinUnderlingProgressionTierStage.PreBoss   , new GoblinUnderlingRangedTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponArrow_0>(), 1.2f, 1f  , 2 , 0.25f, 8, 9f , 1.4f) },
				{ GoblinUnderlingProgressionTierStage.EoC       , new GoblinUnderlingRangedTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponArrow_1>(), 1.5f, 1.2f, 4 , 0.3f , 8, 10f , 1.6f) },
				{ GoblinUnderlingProgressionTierStage.Evil      , new GoblinUnderlingRangedTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponArrow_2>(), 1.7f, 1.4f, 6 , 0.35f, 7, 5.5f , 1.7f) },
				{ GoblinUnderlingProgressionTierStage.Skeletron , new GoblinUnderlingRangedTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponArrow_3>(), 1.9f, 1.6f, 8 , 0.4f , 7, 12f, 1.8f) },
				{ GoblinUnderlingProgressionTierStage.WoF       , new GoblinUnderlingRangedTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponArrow_3>(), 2.4f, 1.6f, 10, 0.4f , 6, 13f, 1.9f) }, //Mostly a copy of previous tier with more damage, same visuals too
				{ GoblinUnderlingProgressionTierStage.Mech      , new GoblinUnderlingRangedTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponArrow_4>(), 2.8f, 1.8f, 10, 0.5f , 5, 14f, 2.0f) },
				{ GoblinUnderlingProgressionTierStage.Plantera  , new GoblinUnderlingRangedTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponArrow_5>(), 2.8f, 2f  , 10, 0.6f , 4, 15f, 2.2f, GoblinUnderlingWeaponArrow_5.Gravity, GoblinUnderlingWeaponArrow_5.TicksWithoutGravity) },
				{ GoblinUnderlingProgressionTierStage.Cultist   , new GoblinUnderlingRangedTierStats(ModContent.ProjectileType<GoblinUnderlingWeaponBlaster>(), 3.4f, 2f  , 10, 0.7f , 3, 16f, 2.4f, -1, 0) },
			};
			RegisterStats(GoblinUnderlingClass.Ranged, tierStats);
		}

		public override void PostUpdatePlayers()
		{
			DetermineCurrentProgressionTier();
		}
	}
}
