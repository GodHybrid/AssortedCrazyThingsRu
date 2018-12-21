using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AssortedCrazyThings.NPCs
{
	public class AssGlobalNPC : GlobalNPC
	{
        public bool shouldSoulDrop = false;

		public override bool InstancePerEntity
		{
			get
			{
				return true;
			}
		}

        public override void ResetEffects(NPC npc)
        {
            shouldSoulDrop = false;
        }

        public override void HitEffect(NPC npc, int hitDirection, double damage)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.life <= 0 && shouldSoulDrop)
                {
                    if (npc.type != mod.NPCType(AssWorld.soulName))
                    {
                        int soulType = mod.NPCType(AssWorld.soulName);

                        //NewNPC starts looking for the first !active from 0 to 200
                        int soulID = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, soulType);
                        Main.npc[soulID].timeLeft = 5000; //change later
                        if (Main.netMode == NetmodeID.Server && soulID < 200)
                        {
                            NetMessage.SendData(23, -1, -1, null, soulID);
                        }
                    }
                }
            }
        }
    }
}