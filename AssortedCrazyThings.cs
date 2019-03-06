using AssortedCrazyThings.Items;
using AssortedCrazyThings.Items.PetAccessories;
using AssortedCrazyThings.Items.Weapons;
using AssortedCrazyThings.Projectiles.Minions;
using AssortedCrazyThings.Projectiles.Pets;
using AssortedCrazyThings.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace AssortedCrazyThings
{
    class AssortedCrazyThings : Mod
    {
        //Slime pet legacy
        public static int[] slimePetLegacy = new int[9];
        public static int[] slimePetNoHair = new int[6];

        //Sun pet textures
        public static Texture2D[] sunPetTextures;

        //Soul item animated textures
        public static Texture2D[] animatedSoulTextures;

        //Soul NPC spawn blacklist
        public static int[] soulBuffBlacklist;

        // UI stuff
        internal static UserInterface CircleUIInterface;
        internal static CircleUI CircleUI;

        //Mod Helpers compat
        public static string GithubUserName { get { return "Werebearguy"; } }
        public static string GithubProjectName { get { return "AssortedCrazyThings"; } }

        private void LoadPets()
        {
            int index = 0;
            slimePetLegacy[index++] = ProjectileType<CuteSlimeBlackPet>();
            slimePetLegacy[index++] = ProjectileType<CuteSlimeBluePet>();
            slimePetLegacy[index++] = ProjectileType<CuteSlimeGreenPet>();
            slimePetLegacy[index++] = ProjectileType<CuteSlimePinkPet>();
            slimePetLegacy[index++] = ProjectileType<CuteSlimePurplePet>();
            slimePetLegacy[index++] = ProjectileType<CuteSlimeRainbowPet>();
            slimePetLegacy[index++] = ProjectileType<CuteSlimeRedPet>();
            slimePetLegacy[index++] = ProjectileType<CuteSlimeXmasPet>();
            slimePetLegacy[index++] = ProjectileType<CuteSlimeYellowPet>();

            index = 0;

            slimePetNoHair[index++] = ProjectileType<CuteSlimeBlackNewPet>();
            slimePetNoHair[index++] = ProjectileType<CuteSlimeBlueNewPet>();
            slimePetNoHair[index++] = ProjectileType<CuteSlimePurpleNewPet>();
            slimePetNoHair[index++] = ProjectileType<CuteSlimePinkNewPet>();
            slimePetNoHair[index++] = ProjectileType<CuteSlimeRedNewPet>();
            slimePetNoHair[index++] = ProjectileType<CuteSlimeYellowNewPet>();

            if (!Main.dedServ && Main.netMode != 2)
            {
                PetAccessory.Load();
            }
        }

        private void LoadSoulBuffBlacklist()
        {
            soulBuffBlacklist = new int[40];
            int index = 0;
            soulBuffBlacklist[index++] = NPCID.GiantWormBody;
            soulBuffBlacklist[index++] = NPCID.GiantWormTail;
            soulBuffBlacklist[index++] = NPCID.DiggerBody;
            soulBuffBlacklist[index++] = NPCID.DiggerTail;
            soulBuffBlacklist[index++] = NPCID.DevourerBody;
            soulBuffBlacklist[index++] = NPCID.DevourerTail;
            soulBuffBlacklist[index++] = NPCID.EaterofWorldsBody;
            soulBuffBlacklist[index++] = NPCID.EaterofWorldsTail;
            soulBuffBlacklist[index++] = NPCID.SeekerBody;
            soulBuffBlacklist[index++] = NPCID.SeekerTail;
            soulBuffBlacklist[index++] = NPCID.TombCrawlerBody;
            soulBuffBlacklist[index++] = NPCID.TombCrawlerTail;
            soulBuffBlacklist[index++] = NPCID.LeechBody;
            soulBuffBlacklist[index++] = NPCID.LeechTail;
            soulBuffBlacklist[index++] = NPCID.BoneSerpentBody;
            soulBuffBlacklist[index++] = NPCID.BoneSerpentTail;
            soulBuffBlacklist[index++] = NPCID.DuneSplicerBody;
            soulBuffBlacklist[index++] = NPCID.DuneSplicerTail;
            soulBuffBlacklist[index++] = NPCID.SpikeBall;
            soulBuffBlacklist[index++] = NPCID.BlazingWheel;

            soulBuffBlacklist[index++] = NPCID.BlueSlime;
            soulBuffBlacklist[index++] = NPCID.SlimeSpiked;
            //immune to all debuffs anyway
            //soulBuffBlacklist[index++] = NPCID.TheDestroyerBody;
            //soulBuffBlacklist[index++] = NPCID.TheDestroyerTail;
            //soulBuffBlacklist[index++] = NPCID.CultistDragonBody1;
            //soulBuffBlacklist[index++] = NPCID.CultistDragonBody2;
            //soulBuffBlacklist[index++] = NPCID.CultistDragonBody3;
            //soulBuffBlacklist[index++] = NPCID.CultistDragonBody4;
            //soulBuffBlacklist[index++] = NPCID.CultistDragonTail;

            Array.Resize(ref soulBuffBlacklist, index + 1);
        }

        private void AddToSoulBuffBlacklist()
        {
            //assuming this is called after InitSoulBuffBlacklist
            int index = soulBuffBlacklist.Length - 1; //last index

            Array.Resize(ref soulBuffBlacklist, index + 40); //buffer


            Mod pinkymod = ModLoader.GetMod("pinkymod");
            if (pinkymod != null)
            {
                soulBuffBlacklist[index++] = pinkymod.NPCType("BoneLeechBody");
                soulBuffBlacklist[index++] = pinkymod.NPCType("BoneLeechTail");
            }

            Array.Resize(ref soulBuffBlacklist, index + 1);
        }

        private void LoadUI()
        {
            if (!Main.dedServ && Main.netMode != 2)
            {
                CircleUI = new CircleUI();
                CircleUI.Activate();
                CircleUIInterface = new UserInterface();
                CircleUIInterface.SetState(CircleUI);

                CircleUIConf.AddItemAsTrigger(ItemType<EverhallowedLantern>(), false); //right click of Everhallowed Lantern
                CircleUIConf.AddItemAsTrigger(ItemType<VanitySelector>()); //left click of Costume Suitcase
                CircleUIConf.AddItemAsTrigger(ItemType<VanitySelector>(), false); //right click of Costume Suitcase
            }
        }

        private void UnloadUI()
        {
            if (!Main.dedServ && Main.netMode != 2)
            {
                CircleUIInterface = null;
                CircleUI = null;
            }
        }

        private void LoadMisc()
        {
            if (!Main.dedServ && Main.netMode != 2)
            {
                animatedSoulTextures = new Texture2D[2];

                animatedSoulTextures[0] = GetTexture("Items/CaughtDungeonSoulAnimated");
                animatedSoulTextures[1] = GetTexture("Items/CaughtDungeonSoulFreedAnimated");

                sunPetTextures = new Texture2D[3];

                for (int i = 0; i < sunPetTextures.Length; i++)
                {
                    sunPetTextures[i] = GetTexture("Projectiles/Pets/PetSunProj_" + i);
                    PremultiplyTexture(sunPetTextures[i]);
                }
            }
        }

        private void UnloadMisc()
        {
            if (!Main.dedServ && Main.netMode != 2)
            {
                animatedSoulTextures = null;

                sunPetTextures = null;
            }
        }

        public override void Load()
        {
            AssUtils.Instance = this;

            ModConf.Load();

            LoadPets();

            LoadSoulBuffBlacklist();

            LoadMisc();
        }

        public override void Unload()
        {
            PetAccessory.Unload();

            UnloadUI();

            UnloadMisc();

            AssUtils.Instance = null;
        }

        public override void PostSetupContent()
        {
            LoadUI();

            //for things that have to be called after Load() because of Main.projFrames[projectile.type] calls (and similar)

            AddToSoulBuffBlacklist();

            //https://forums.terraria.org/index.php?threads/boss-checklist-in-game-progression-checklist.50668/
            Mod bossChecklist = ModLoader.GetMod("BossChecklist");
            if (bossChecklist != null)
            {
                //5.1f means just after skeletron
                bossChecklist.Call("AddMiniBossWithInfo", NPCs.DungeonBird.Harvester.name, 5.1f, (Func<bool>)(() => AssWorld.downedHarvester), "Use a [i:" + ItemType<Items.IdolOfDecay>() + "] in the dungeon after Skeletron has been defeated");
            }
        }

        private void PoofVisual(int projType)
        {
            int projIndex = -1;
            //find first occurence of a player owned projectile
            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].active)
                {
                    if (Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == projType)
                    {
                        projIndex = i;
                        break;
                    }
                }
            }

            if(projIndex != -1)
            {
                Dust dust;
                for (int i = 0; i < 14; i++)
                {
                    dust = Main.dust[Dust.NewDust(Main.projectile[projIndex].position, Main.projectile[projIndex].width, Main.projectile[projIndex].height, 204, Main.projectile[projIndex].velocity.X, Main.projectile[projIndex].velocity.Y, 0, new Color(255, 255, 255), 0.8f)];
                    dust.noGravity = true;
                    dust.noLight = true;
                }
            }
        }

        private void UIText(string str)
        {
            CombatText.NewText(Main.LocalPlayer.getRect(),
                CombatText.HealLife, "Selected: " + str);
        }

        private void CircleUIStart(int triggerType, bool triggerLeft = true)
        {
            AssPlayer mPlayer = Main.LocalPlayer.GetModPlayer<AssPlayer>();
            PetPlayer pPlayer = Main.LocalPlayer.GetModPlayer<PetPlayer>();

            if (triggerLeft) //left click
            {
                if (triggerType == ItemType<VanitySelector>())
                {
                    if (pPlayer.DocileDemonEye)
                    {
                        //set custom config with starting value
                        CircleUI.currentSelected = pPlayer.petEyeType;

                        CircleUI.UIConf = CircleUIConf.DocileDemonEyeConf();
                    }
                    else if (pPlayer.LifelikeMechanicalFrog)
                    {
                        CircleUI.currentSelected = pPlayer.mechFrogCrown ? 1 : 0;

                        CircleUI.UIConf = CircleUIConf.LifeLikeMechFrogConf();
                    }
                    else if (pPlayer.CursedSkull)
                    {
                        CircleUI.currentSelected = pPlayer.cursedSkullType;

                        CircleUI.UIConf = CircleUIConf.CursedSkullConf();
                    }
                    else if (pPlayer.YoungWyvern)
                    {
                        CircleUI.currentSelected = pPlayer.youngWyvernType;

                        CircleUI.UIConf = CircleUIConf.YoungWyvernConf();
                    }
                    else if (pPlayer.PetFishron)
                    {
                        CircleUI.currentSelected = pPlayer.petFishronType;

                        CircleUI.UIConf = CircleUIConf.PetFishronConf();
                    }
                    else if (pPlayer.YoungHarpy)
                    {
                        CircleUI.currentSelected = pPlayer.youngHarpyType;

                        CircleUI.UIConf = CircleUIConf.YoungHarpyConf();
                    }
                    else if (pPlayer.Abeemination)
                    {
                        CircleUI.currentSelected = pPlayer.abeeminationType;

                        CircleUI.UIConf = CircleUIConf.AbeeminiationConf();
                    }
                    else if (pPlayer.LilWraps)
                    {
                        CircleUI.currentSelected = pPlayer.lilWrapsType;

                        CircleUI.UIConf = CircleUIConf.LilWrapsConf();
                    }
                    else if (pPlayer.VampireBat)
                    {
                        CircleUI.currentSelected = pPlayer.vampireBatType;

                        CircleUI.UIConf = CircleUIConf.VampireBatConf();
                    }
                    else if (pPlayer.Pigronata)
                    {
                        CircleUI.currentSelected = pPlayer.pigronataType;

                        CircleUI.UIConf = CircleUIConf.PigronataConf();
                    }
                    //FOR LEFT CLICK ONLY (REGULAR PET)
                    //ALTERNATE
                    //else if (pPlayer.ClassName)
                    //{
                    //    CircleUI.currentSelected = pPlayer.classNameType;

                    //    CircleUI.UIConf = CircleUIConf.ClassNameConf();
                    //}
                    else
                    {
                        return;
                    }
                }
            }
            else //right click
            {
                if (triggerType == ItemType<VanitySelector>())
                {
                    if (pPlayer.PetMoon)
                    {
                        CircleUI.currentSelected = pPlayer.petMoonType;

                        CircleUI.UIConf = CircleUIConf.PetMoonConf();
                    }
                    //FOR RIGHT CLICK ONLY (LIGHT PET)
                    //ALTERNATE
                    //else if (pPlayer.ClassName)
                    //{
                    //    CircleUI.currentSelected = pPlayer.classNameType;

                    //    CircleUI.UIConf = CircleUIConf.ClassNameConf();
                    //}
                    else
                    {
                        return;
                    }
                }
                else if (triggerType == ItemType<EverhallowedLantern>())
                {
                    CircleUI.currentSelected = mPlayer.selectedSoulMinionType;

                    //this one needs to be created anew because of the unlocked list
                    CircleUI.UIConf = CircleUIConf.EverhallowedLanternConf();
                }
                else
                {
                    return;
                }
            }

            // Spawn UI
            CircleUI.visible = true;
            CircleUI.spawnPosition = Main.MouseScreen;
            CircleUI.leftCorner = Main.MouseScreen - new Vector2(CircleUI.mainRadius, CircleUI.mainRadius);
            CircleUI.heldItemType = triggerType;
        }

        private void CircleUIEnd(bool triggerLeft = true)
        {
            AssPlayer mPlayer = Main.LocalPlayer.GetModPlayer<AssPlayer>();
            PetPlayer pPlayer = Main.LocalPlayer.GetModPlayer<PetPlayer>();
            if (CircleUI.returned != -1 && CircleUI.returned != CircleUI.currentSelected)
            {
                //if something returned AND if the returned thing isn't the same as the current one

                Main.PlaySound(SoundID.Item4.WithVolume(0.8f), Main.LocalPlayer.position);

                if (triggerLeft) //left click
                {
                    if (CircleUI.heldItemType == ItemType<VanitySelector>())
                    {
                        PoofVisual(CircleUI.UIConf.AdditionalInfo);
                        UIText(CircleUI.UIConf.Tooltips[CircleUI.returned]);
                        if (pPlayer.DocileDemonEye)
                        {
                            pPlayer.petEyeType = (byte)CircleUI.returned;
                        }
                        else if (pPlayer.LifelikeMechanicalFrog)
                        {
                            pPlayer.mechFrogCrown = (CircleUI.returned > 0) ? true : false;
                        }
                        else if (pPlayer.CursedSkull)
                        {
                            pPlayer.cursedSkullType = (byte)CircleUI.returned;
                        }
                        else if (pPlayer.YoungWyvern)
                        {
                            pPlayer.youngWyvernType = (byte)CircleUI.returned;
                        }
                        else if (pPlayer.PetFishron)
                        {
                            pPlayer.petFishronType = (byte)CircleUI.returned;
                        }
                        else if (pPlayer.YoungHarpy)
                        {
                            pPlayer.youngHarpyType = (byte)CircleUI.returned;
                        }
                        else if (pPlayer.Abeemination)
                        {
                            pPlayer.abeeminationType = (byte)CircleUI.returned;
                        }
                        else if (pPlayer.LilWraps)
                        {
                            pPlayer.lilWrapsType = (byte)CircleUI.returned;
                        }
                        else if (pPlayer.VampireBat)
                        {
                            pPlayer.vampireBatType = (byte)CircleUI.returned;
                        }
                        else if (pPlayer.Pigronata)
                        {
                            pPlayer.pigronataType = (byte)CircleUI.returned;
                        }
                        //ALTERNATE
                        //else if (pPlayer.ClassName)
                        //{
                        //    pPlayer.classNameType = (byte)CircleUI.returned;
                        //}
                    }
                }
                else //right click
                {
                    if (CircleUI.heldItemType == ItemType<VanitySelector>())
                    {
                        PoofVisual(CircleUI.UIConf.AdditionalInfo);
                        UIText(CircleUI.UIConf.Tooltips[CircleUI.returned]);
                        if (pPlayer.PetMoon)
                        {
                            pPlayer.petMoonType = (byte)CircleUI.returned;
                        }
                        //ALTERNATE
                        //else if (pPlayer.ClassName)
                        //{
                        //    pPlayer.classNameType = (byte)CircleUI.returned;
                        //}
                    }
                    else if (CircleUI.heldItemType == ItemType<EverhallowedLantern>())
                    {
                        mPlayer.selectedSoulMinionType = CircleUI.returned;

                        UpdateEverhallowedLanternStats(CircleUI.returned);
                    }
                }
            }

            CircleUI.returned = -1;
            CircleUI.visible = false;
        }

        private void UpdateCircleUI(GameTime gameTime)
        {
            AssPlayer mPlayer = Main.LocalPlayer.GetModPlayer<AssPlayer>();

            bool? left = null;
            if (mPlayer.LeftClickPressed && CircleUIConf.TriggerListLeft.Contains(Main.LocalPlayer.HeldItem.type))
            {
                left = true;
            }
            else if (mPlayer.RightClickPressed && CircleUIConf.TriggerListRight.Contains(Main.LocalPlayer.HeldItem.type))
            {
                left = false;
            }

            if (left != null) CircleUIStart(Main.LocalPlayer.HeldItem.type, (bool)left);

            if (CircleUI.visible)
            {
                left = null;
                if (mPlayer.LeftClickReleased)
                {
                    left = true;
                }
                else if (mPlayer.RightClickReleased)
                {
                    left = false;
                }

                if (left != null) CircleUIEnd((bool)left);

                if (CircleUI.heldItemType != Main.LocalPlayer.HeldItem.type) //cancel the UI when you switch items
                {
                    CircleUI.returned = -1;
                    CircleUI.visible = false;
                }
            }
        }

        private void UpdateEverhallowedLanternStats(int selectedSoulType)
        {
            bool first = true;
            for(int i = 0; i < Main.LocalPlayer.inventory.Length; i++)
            {
                if(Main.LocalPlayer.inventory[i].type == ItemType<EverhallowedLantern>())
                {
                    var stats = CompanionDungeonSoulMinionBase.GetAssociatedStats(selectedSoulType);
                    //bad practice, don't do this
                    Main.LocalPlayer.inventory[i].damage = stats.Damage;
                    Main.LocalPlayer.inventory[i].shoot = stats.Type;
                    Main.LocalPlayer.inventory[i].knockBack = stats.Knockback;

                    var soulType = (CompanionDungeonSoulMinionBase.SoulType)stats.SoulType;
                    if (first && soulType == CompanionDungeonSoulMinionBase.SoulType.Dungeon)
                    {
                        CombatText.NewText(Main.LocalPlayer.getRect(),
                            CombatText.HealLife, "Selected: " + soulType.ToString() + " Soul");
                    }
                    else if(first)
                    {
                        CombatText.NewText(Main.LocalPlayer.getRect(),
                            CombatText.HealLife, "Selected: Soul of " + soulType.ToString());
                    }
                    first = false;
                }
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            UpdateCircleUI(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int InventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Hotbar"));
            if (InventoryIndex != -1)
            {
                layers.Insert(++InventoryIndex, new LegacyGameInterfaceLayer
                    (
                    "ACT: Appearance Selection",
                    delegate
                    {
                        if (CircleUI.visible) CircleUIInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        //public void SyncAltTextureNPC(NPC npc)
        //{
        //    //server side only

        //    if (Main.netMode == NetmodeID.Server)
        //    {
        //        ModPacket packet = GetPacket();
        //        packet.Write((byte)AssMessageType.SyncAltTextureNPC);
        //        packet.Write((byte)npc.whoAmI);
        //        packet.Write((byte)npc.altTexture);
        //        packet.Send();
        //    }
        //}

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            AssMessageType msgType = (AssMessageType)reader.ReadByte();
            short knapSackSlimeIndex;
            int arrayLength;
            byte knapSackSlimeTexture;
            byte playerNumber;
            AssPlayer mPlayer;
            PetPlayer petPlayer;
            byte changes;
            //byte npcnumber;
            //byte npcAltTexture;

            switch (msgType)
            {
                case AssMessageType.SyncKnapSackSlimeTexture:
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        knapSackSlimeIndex = reader.ReadInt16();
                        knapSackSlimeTexture = reader.ReadByte();
                        if (Main.projectile[knapSackSlimeIndex].type == ProjectileType<SlimePackMinion>())
                        {
                            Main.projectile[knapSackSlimeIndex].localAI[1] = knapSackSlimeTexture;
                        }
                    }
                    break;

                case AssMessageType.SyncPlayer:
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        playerNumber = reader.ReadByte();
                        mPlayer = Main.player[playerNumber].GetModPlayer<AssPlayer>();

                        arrayLength = reader.ReadInt16();
                        //Main.NewText(arrayLength);
                        short[] indexes = new short[arrayLength];
                        byte[] textures = new byte[arrayLength];

                        for (int i = 0; i < arrayLength; i++)
                        {
                            indexes[i] = reader.ReadInt16();
                            textures[i] = reader.ReadByte();
                        }
                        for (int i = 0; i < arrayLength; i++)
                        {
                            //Main.NewText("recv SyncKnapSackSlimeTextureOnEnterWorld with " + indexes[i] + " " + textures[i]);
                            if (Main.projectile[indexes[i]].type == ProjectileType<SlimePackMinion>())
                            {
                                Main.projectile[indexes[i]].localAI[1] = textures[i];
                            }
                        }
                    }
                    break;
                case AssMessageType.SyncPlayerVanity:
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        playerNumber = reader.ReadByte();
                        petPlayer = Main.player[playerNumber].GetModPlayer<PetPlayer>();
                        //no "changes" packet
                        petPlayer.RecvSyncPlayerVanitySub(reader);
                    }
                    break;
                case AssMessageType.SendClientChangesVanity:
                    //client and server
                    playerNumber = reader.ReadByte();
                    petPlayer = Main.player[playerNumber].GetModPlayer<PetPlayer>();
                    changes = reader.ReadByte();

                    petPlayer.RecvClientChangesPacketSub(reader, changes);

                    //server transmits to others
                    if (Main.netMode == NetmodeID.Server)
                    {
                        petPlayer.SendClientChangesPacketSub(changes, -1, playerNumber);
                    }
                    break;
                case AssMessageType.ConvertInertSoulsInventory:
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        //convert souls in local inventory
                        mPlayer = Main.LocalPlayer.GetModPlayer<AssPlayer>();
                        mPlayer.ConvertInertSoulsInventory();
                    }
                    break;
                //case AssMessageType.SyncAltTextureNPC:
                //    if (Main.netMode == NetmodeID.MultiplayerClient)
                //    {
                //        npcnumber = reader.ReadByte();
                //        npcAltTexture = reader.ReadByte();
                //        Main.NewText("recv tex " + npcAltTexture + " from " + npcnumber);
                //        Main.NewText("type " + Main.npc[npcnumber].type);
                //        Main.NewText("extracount " + NPCID.Sets.ExtraTextureCount[Main.npc[npcnumber].type]);
                //        if (NPCID.Sets.ExtraTextureCount[Main.npc[npcnumber].type] > 0)
                //        {
                //            Main.NewText("set tex to" + npcAltTexture);
                //            Main.npc[npcnumber].altTexture = npcAltTexture;
                //        }
                //    }
                //    break;
                default:
                    ErrorLogger.Log("AssortedCrazyThings: Unknown Message type: " + msgType);
                    break;
            }
        }

        //Credit to jopojelly
        //makes alpha on .png textures actually properly rendered
        public static void PremultiplyTexture(Texture2D texture)
        {
            Color[] buffer = new Color[texture.Width * texture.Height];
            texture.GetData(buffer);
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Color.FromNonPremultiplied(buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);
            }
            texture.SetData(buffer);
        }
    }

    public enum AssMessageType : byte
    {
        SendClientChangesVanity,
        SyncKnapSackSlimeTexture,
        SyncPlayer,
        SyncPlayerVanity,
        //SyncAltTextureNPC,
        ConvertInertSoulsInventory
    }

    public enum PetPlayerChanges : byte
    {
        //easier to copypaste when its not capitalized
        none,
        all,
        slots,
        mechFrogCrown,
        petEyeType,
        cursedSkullType,
        youngWyvernType,
        petFishronType,
        petMoonType,
        youngHarpyType,
        abeeminationType,
        lilWrapsType,
        vampireBatType,
        pigronataType,
        //ALTERNATE
        //classNameType,
    }
}
