﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AssortedCrazyThings.Projectiles.Pets;
using AssortedCrazyThings.Base.SwarmDraw.FairySwarmDraw;
using AssortedCrazyThings.Base.SwarmDraw.SwarmofCthulhuDraw;
using Microsoft.Xna.Framework;
using System;

namespace AssortedCrazyThings.Base.SwarmDraw
{
    //TODO unhardcode this when both a better tml loader comes along and more swarm draw sets exist
    public class SwarmDrawPlayer : ModPlayer
    {
        public static SwarmDrawSet NewFairySwarmDrawSet()
        {
            return (FairySwarmDrawSet)ModContent.GetInstance<FairySwarmDrawSet>().Clone();
        }

        public static SwarmDrawSet NewSwarmofCthulhuDrawSet()
        {
            return (SwarmofCthulhuDrawSet)ModContent.GetInstance<SwarmofCthulhuDrawSet>().Clone();
        }

        public static void HandleDrawSet(ref SwarmDrawSet set, Func<SwarmDrawSet> gen, bool condition, Vector2 center)
        {
            if (condition)
            {
                if (set == null)
                {
                    set = gen();
                }

                if (set == null)
                {
                    return;
                }

                if (!set.Active)
                {
                    set.Activate();
                }

                set.Update(center);
            }
            else if (set != null)
            {
                set.Deactivate();
            }
        }

        public SwarmDrawSet fairySwarmDrawSet;

        public SwarmDrawSet swarmofCthulhuDrawSet;

        public override void Initialize()
        {
            fairySwarmDrawSet = null;
            swarmofCthulhuDrawSet = null;
        }

        public override void PostUpdate()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            HandleDrawSet(ref fairySwarmDrawSet,
                NewFairySwarmDrawSet,
                Player.ownedProjectileCounts[ModContent.ProjectileType<FairySwarmProj>()] > 0,
                Player.Center);

            HandleDrawSet(ref swarmofCthulhuDrawSet,
                NewSwarmofCthulhuDrawSet,
                Player.ownedProjectileCounts[ModContent.ProjectileType<SwarmofCthulhuProj>()] > 0,
                Player.Center);
        }
    }
}
