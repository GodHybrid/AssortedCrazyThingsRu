using AssortedCrazyThings.Base;
using AssortedCrazyThings.Items;
using AssortedCrazyThings.Items.PetAccessories;
using AssortedCrazyThings.Tiles;
using AssortedCrazyThings.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace AssortedCrazyThings
{
	[Content(ConfigurationSystem.AllFlags)]
	public class AssUISystem : AssSystem
	{
		/// <summary>
		/// Zoom level, (for UIs). 0f == fully zoomed out, 1f == fully zoomed in
		/// </summary>
		public static Vector2 ZoomFactor;

		//UI stuff
		internal static UserInterface CircleUIInterface;
		internal static CircleUI CircleUI;

		internal static UserInterface HoverNPCUIInterface;
		internal static HoverNPCUI HoverNPCUI;

		internal static UserInterface EnhancedHunterUIInterface;
		internal static EnhancedHunterUI EnhancedHunterUI;

		internal static UserInterface PetVanityUIInterface;
		internal static PetVanityUI PetVanityUI;

		public override void PostSetupContent()
		{
			if (!Main.dedServ && Main.netMode != NetmodeID.Server)
			{
				CircleUI = new CircleUI();
				CircleUI.Activate();
				CircleUIInterface = new UserInterface();
				CircleUIInterface.SetState(CircleUI);

				HoverNPCUI = new HoverNPCUI();
				HoverNPCUI.Activate();
				HoverNPCUIInterface = new UserInterface();
				HoverNPCUIInterface.SetState(HoverNPCUI);

				if (ContentConfig.Instance.Bosses)
				{
					EnhancedHunterUI = new EnhancedHunterUI();
					EnhancedHunterUI.Activate();
					EnhancedHunterUIInterface = new UserInterface();
					EnhancedHunterUIInterface.SetState(EnhancedHunterUI);
				}

				if (ContentConfig.Instance.CuteSlimes)
				{
					PetVanityUI = new PetVanityUI();
					PetVanityUI.Activate();
					PetVanityUIInterface = new UserInterface();
					PetVanityUIInterface.SetState(PetVanityUI);
				}
			}
		}

		public override void Unload()
		{
			if (!Main.dedServ && Main.netMode != NetmodeID.Server)
			{
				CircleUIInterface = null;
				CircleUI = null;

				HoverNPCUIInterface = null;
				HoverNPCUI = null;

				EnhancedHunterUIInterface = null;
				EnhancedHunterUI = null;

				PetVanityUIInterface = null;
				PetVanityUI = null;

				EnhancedHunterUI.arrowTexture = null;
				PetVanityUI.redCrossTexture = null;
				CircleUI.UIConf = null;
				CircleUIHandler.TriggerListLeft.Clear();
				CircleUIHandler.TriggerListRight.Clear();
			}
		}

		/// <summary>
		/// Returns language agnostic representation of ": "
		/// </summary>
		public static string GetColon()
		{
			string divider = ": ";
			if (Language.ActiveCulture.LegacyId == (int)GameCulture.CultureName.Chinese)
			{
				divider = ":";
			}
			return divider;
		}

		/// <summary>
		/// Creates golden dust particles at the projectiles location with that type and LocalPlayer as owner. (Used for pets)
		/// </summary>
		private void PoofVisual(int projType)
		{
			int projIndex = -1;
			//find first occurence of a player owned projectile
			for (int i = 0; i < Main.maxProjectiles; i++)
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

			if (projIndex != -1)
			{
				Dust dust;
				for (int i = 0; i < 14; i++)
				{
					dust = Dust.NewDustDirect(Main.projectile[projIndex].position, Main.projectile[projIndex].width, Main.projectile[projIndex].height, 204, Main.projectile[projIndex].velocity.X, Main.projectile[projIndex].velocity.Y, 0, new Color(255, 255, 255), 0.8f);
					dust.noGravity = true;
					dust.noLight = true;
				}
			}
		}

		/// <summary>
		/// Called when CircleUI starts
		/// </summary>
		private void CircleUIStart(int triggerType, bool triggerLeft = true, bool fromDresser = false)
		{
			AssPlayer mPlayer = Main.LocalPlayer.GetModPlayer<AssPlayer>();
			PetPlayer pPlayer = Main.LocalPlayer.GetModPlayer<PetPlayer>();

			//combine both lists of the players (split for organization and player load shenanigans)
			List<CircleUIHandler> l = mPlayer.CircleUIList;
			l.AddRange(pPlayer.CircleUIList);

			bool found = false;
			for (int i = 0; i < l.Count; i++)
			{
				var handler = l[i];
				if (handler.Condition())
				{
					if (handler.TriggerItem == triggerType && handler.TriggerLeft == triggerLeft)
					{
						CircleUI.UIConf = handler.UIConf();
						CircleUI.currentSelected = handler.OnUIStart();
						found = true;
						break;
					}
				}
			}
			//extra things that happen
			if (!found)
			{
				if (triggerType == ModContent.ItemType<VanitySelector>())
				{
					var text = triggerLeft ? ModContent.GetInstance<VanityDresserTile>().NoCostumesFoundPetText :
						ModContent.GetInstance<VanityDresserTile>().NoCostumesFoundLightPetText;
					AssUtils.UIText(text.ToString(), CombatText.DamagedFriendly);
					return;
				}
			}

			//Spawn UI
			CircleUI.Start(triggerType, triggerLeft, fromDresser);
		}

		/// <summary>
		/// Called when CircleUI ends
		/// </summary>
		private void CircleUIEnd(bool triggerLeft = true)
		{
			AssPlayer mPlayer = Main.LocalPlayer.GetModPlayer<AssPlayer>();
			if (CircleUI.returned != CircleUI.NONE && CircleUI.returned != CircleUI.currentSelected)
			{
				//if something returned AND if the returned thing isn't the same as the current one

				try
				{
					SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.6f });
				}
				catch
				{
					//No idea why but this threw errors one time
				}

				List<CircleUIHandler> l = mPlayer.CircleUIList;
				for (int i = 0; i < l.Count; i++)
				{
					if (l[i].Condition())
					{
						if (l[i].TriggerItem == CircleUI.triggerItemType)
						{
							if (l[i].TriggerLeft == triggerLeft)
							{
								l[i].OnUIEnd();
								break;
							}
						}
					}
				}
				//extra things that happen
				if (CircleUI.triggerItemType == ModContent.ItemType<VanitySelector>())
				{
					PoofVisual(CircleUI.UIConf.AdditionalInfo);
					AssUtils.UIText(AssLocalization.SelectedText.Format(CircleUI.UIConf.Tooltips[CircleUI.returned]), CombatText.HealLife);
				}
			}

			CircleUI.returned = CircleUI.NONE;
			CircleUI.visible = false;
		}

		/// <summary>
		/// Called in UpdateUI
		/// </summary>
		private void UpdateCircleUI()
		{
			Player player = Main.LocalPlayer;
			AssPlayer mPlayer = player.GetModPlayer<AssPlayer>();

			int triggerType = player.HeldItem.type;
			bool openWithDresser = mPlayer.mouseoveredDresser;
			if (openWithDresser)
			{
				triggerType = ModContent.ItemType<VanitySelector>();
			}
			bool? left = null;
			if (mPlayer.LeftClickPressed && (CircleUIHandler.TriggerListLeft.Contains(triggerType) || openWithDresser))
			{
				left = true;
			}
			else if (mPlayer.RightClickPressed && (CircleUIHandler.TriggerListRight.Contains(triggerType) || openWithDresser))
			{
				left = false;
			}

			if (left != null && AllowedToOpenUI()) CircleUIStart(triggerType, (bool)left, openWithDresser);

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

				if (left != null && left == CircleUI.openedWithLeft) CircleUIEnd((bool)left);

				if (CircleUI.triggerItemType != triggerType && !CircleUI.triggeredFromDresser) //cancel the UI when you switch items
				{
					CircleUI.returned = CircleUI.NONE;
					CircleUI.visible = false;
				}
			}
		}

		/// <summary>
		/// Called in UpdateUI
		/// </summary>
		private void UpdatePetVanityUI()
		{
			Player player = Main.LocalPlayer;
			AssPlayer mPlayer = player.GetModPlayer<AssPlayer>();
			PetPlayer pPlayer = player.GetModPlayer<PetPlayer>();

			int itemType = player.HeldItem.type;
			if (mPlayer.LeftClickPressed && AllowedToOpenUI() && PetAccessory.TryGetAccessoryFromItem(itemType, out PetAccessory petAccessory))
			{
				if (petAccessory.HasAlts && pPlayer.HasValidSlimePet(out SlimePet slimePet) &&
					!slimePet.IsSlotTypeBlacklisted[(int)petAccessory.Slot])
				{
					//Spawn UI
					PetVanityUI.Start(petAccessory);
				}
			}

			if (!PetVanityUI.visible)
			{
				return;
			}

			if (mPlayer.LeftClickReleased)
			{
				if (PetVanityUI.returned > PetVanityUI.NONE)
				{
					//if something returned AND if the returned thing isn't the same as the current one

					try
					{
						SoundEngine.PlaySound(SoundID.Item1, player.position);
					}
					catch
					{
						//No idea why but this threw errors one time
					}
					//UIText(SelectedText.Format(PetVanityUI.petAccessory.AltTextureSuffixes[PetVanityUI.returned]), CombatText.HealLife);

					PetVanityUI.petAccessory.AltTextureIndex = (byte)PetVanityUI.returned;
					pPlayer.ToggleAccessory(PetVanityUI.petAccessory);
				}
				else if (PetVanityUI.hasEquipped && PetVanityUI.returned == PetVanityUI.NONE)
				{
					//hovered over the middle and had something equipped: take accessory away
					pPlayer.DelAccessory(PetVanityUI.petAccessory);
				}
				//else if (returned == PetVanityUI.IGNORE) {nothing happens}

				PetVanityUI.returned = PetVanityUI.NONE;
				PetVanityUI.visible = false;
			}

			if (PetVanityUI.petAccessory.Type != itemType) //cancel the UI when you switch items
			{
				PetVanityUI.returned = PetVanityUI.NONE;
				PetVanityUI.visible = false;
			}
		}

		private void UpdateHoverNPCUI(GameTime gameTime)
		{
			HoverNPCUI?.Update(gameTime);
		}

		private void UpdateEnhancedHunterUI(GameTime gameTime)
		{
			if (Main.LocalPlayer.GetModPlayer<AssPlayer>().enhancedHunterBuff)
			{
				EnhancedHunterUI.visible = true;
			}
			else
			{
				EnhancedHunterUI.visible = false;
			}
			EnhancedHunterUI?.Update(gameTime);
		}

		public override void UpdateUI(GameTime gameTime)
		{
			UpdateCircleUI();
			UpdateHoverNPCUI(gameTime);
			UpdateEnhancedHunterUI(gameTime);
			UpdatePetVanityUI();
		}

		/// <summary>
		/// Checks if LocalPlayer can open a UI
		/// </summary>
		private bool AllowedToOpenUI()
		{
			return Main.hasFocus &&
				!Main.gamePaused &&
				!Main.LocalPlayer.dead &&
				!Main.LocalPlayer.mouseInterface &&
				!Main.drawingPlayerChat &&
				!Main.editSign &&
				!Main.editChest &&
				!Main.blockInput &&
				!Main.mapFullscreen &&
				!Main.HoveringOverAnNPC &&
				Main.LocalPlayer.cursorItemIconID != -1 &&
				Main.LocalPlayer.talkNPC == -1 &&
				Main.LocalPlayer.itemTime == 0 && Main.LocalPlayer.itemAnimation == 0 &&
				!(Main.LocalPlayer.frozen || Main.LocalPlayer.webbed || Main.LocalPlayer.stoned);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Hotbar"));
			if (inventoryIndex != -1)
			{
				if (CircleUI.visible && CircleUIInterface != null)
				{
					//remove the item icon when using the item while held outside the inventory
					int mouseItemIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Item / NPC Head"));
					if (mouseItemIndex != -1) layers.RemoveAt(mouseItemIndex);
					layers.Insert(++inventoryIndex, new LegacyGameInterfaceLayer
						(
						"ACT: Appearance Select",
						delegate
						{
							CircleUIInterface.Draw(Main.spriteBatch, new GameTime());
							return true;
						},
						InterfaceScaleType.UI)
					);
				}

				if (PetVanityUI.visible && PetVanityUIInterface != null)
				{
					//remove the item icon when using the item while held outside the inventory
					int mouseItemIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Item / NPC Head"));
					if (mouseItemIndex != -1) layers.RemoveAt(mouseItemIndex);
					layers.Insert(++inventoryIndex, new LegacyGameInterfaceLayer
						(
						"ACT: Pet Vanity Select",
						delegate
						{
							PetVanityUIInterface.Draw(Main.spriteBatch, new GameTime());
							return true;
						},
						InterfaceScaleType.UI)
					);
				}
			}

			int mouseOverIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Over"));
			if (mouseOverIndex != -1)
			{
				if (HoverNPCUIInterface != null)
				{
					layers.Insert(++mouseOverIndex, new LegacyGameInterfaceLayer
						(
						"ACT: NPC Mouse Over",
						delegate
						{
							HoverNPCUIInterface.Draw(Main.spriteBatch, new GameTime());
							return true;
						},
						InterfaceScaleType.UI)
					);
				}

				if (EnhancedHunterUI.visible && EnhancedHunterUIInterface != null)
				{
					layers.Insert(++mouseOverIndex, new LegacyGameInterfaceLayer
						(
						"ACT: Enhanced Hunter",
						delegate
						{
							EnhancedHunterUIInterface.Draw(Main.spriteBatch, new GameTime());
							return true;
						},
						InterfaceScaleType.UI)
					);
				}
			}
		}

		public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
		{
			ZoomFactor = Transform.Zoom - (Vector2.UnitX + Vector2.UnitY);
		}
	}
}
