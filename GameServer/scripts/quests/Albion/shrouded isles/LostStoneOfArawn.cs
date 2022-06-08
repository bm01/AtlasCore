/*
 * Atlas Si Quest - Atlas 1.65v Classic Freeshard
 */
/*
*Author         : Kelt
*Editor			: Kelt
*Source         : Custom
*Date           : 08 June 2022
*Quest Name     : Lost Stone of Arawn
*Quest Classes  : all
*Quest Version  : v1.0
*
*Changes:
* 
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DOL.AI.Brain;
using DOL.Database;
using DOL.Events;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.GS.PlayerTitles;
using log4net;

namespace DOL.GS.Quests.Albion
{
	public class LostStoneofArawn : BaseQuest
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected const string questTitle = "Lost Stone of Arawn";
		protected const int minimumLevel = 48;
		protected const int maximumLevel = 50;

		private static GameNPC Honaytrt = null; // Start NPC Honayt'rt
		private static GameNPC Nchever = null; // N'chever
		private static GameNPC Ohonat = null; // O'honat
		private static GameNPC Nyaegha = null; // O'honat

		private static GameLocation demonLocation = new GameLocation("Nyaegha", 51, 348381, 479838, 3320);

		private static IArea demonArea = null;
		
		private static ItemTemplate ancient_copper_necklace = null;
		private static ItemTemplate scroll_wearyall_loststone = null;
		private static ItemTemplate lost_stone_of_arawn = null;

		// Constructors
		public LostStoneofArawn() : base()
		{
		}

		public LostStoneofArawn(GamePlayer questingPlayer) : base(questingPlayer)
		{
		}

		public LostStoneofArawn(GamePlayer questingPlayer, int step) : base(questingPlayer, step)
		{
		}

		public LostStoneofArawn(GamePlayer questingPlayer, DBQuest dbQuest) : base(questingPlayer, dbQuest)
		{
		}

		public override int Level
		{
			get
			{
				// Quest Level
				return minimumLevel;
			}
		}

		[ScriptLoadedEvent]
		public static void ScriptLoaded(DOLEvent e, object sender, EventArgs args)
		{
			if (!ServerProperties.Properties.LOAD_QUESTS)
				return;
			

			#region defineNPCs

			GameNPC[] npcs = WorldMgr.GetNPCsByName("Honayt\'rt", eRealm.Albion);

			if (npcs.Length > 0)
				foreach (GameNPC npc in npcs)
					if (npc.CurrentRegionID == 51 && npc.X == 435217 && npc.Y == 495273)
					{
						Honaytrt = npc;
						break;
					}

			if (Honaytrt == null)
			{
				if (log.IsWarnEnabled)
					log.Warn("Could not find Honaytrt, creating it ...");
				Honaytrt = new GameNPC();
				Honaytrt.Model = 759;
				Honaytrt.Name = "Honayt\'rt";
				Honaytrt.GuildName = "";
				Honaytrt.Realm = eRealm.Albion;
				Honaytrt.CurrentRegionID = 51;
				Honaytrt.LoadEquipmentTemplateFromDatabase("097fe8c1-7d7e-4b82-a7ca-04a6e192afc1");
				Honaytrt.Size = 51;
				Honaytrt.Level = 50;
				Honaytrt.X = 435217;
				Honaytrt.Y = 495273;
				Honaytrt.Z = 3134;
				Honaytrt.Heading = 3270;
				Honaytrt.AddToWorld();
				if (SAVE_INTO_DATABASE)
				{
					Honaytrt.SaveIntoDatabase();
				}
			}
			
			npcs = WorldMgr.GetNPCsByName("N\'chever", eRealm.Albion);

			if (npcs.Length > 0)
				foreach (GameNPC npc in npcs)
					if (npc.CurrentRegionID == 51 && npc.X == 30763 && npc.Y == 29908)
					{
						Nchever = npc;
						break;
					}

			if (Nchever == null)
			{
				if (log.IsWarnEnabled)
					log.Warn("Could not find Nchever , creating it ...");
				Nchever = new GameNPC();
				Nchever.Model = 752;
				Nchever.Name = "N\'chever";
				Nchever.GuildName = "";
				Nchever.Realm = eRealm.Albion;
				Nchever.CurrentRegionID = 51;
				Nchever.LoadEquipmentTemplateFromDatabase("a2639e94-f032-4041-ad67-15dfeaf004d2");
				Nchever.Size = 51;
				Nchever.Level = 52;
				Nchever.X = 435972;
				Nchever.Y = 492370;
				Nchever.Z = 3087;
				Nchever.Heading = 594;
				Nchever.AddToWorld();
				if (SAVE_INTO_DATABASE)
				{
					Nchever.SaveIntoDatabase();
				}
			}
			// end npc

			npcs = WorldMgr.GetNPCsByName("O\'honat", eRealm.Albion);

			if (npcs.Length > 0)
				foreach (GameNPC npc in npcs)
					if (npc.CurrentRegionID == 51 && npc.X == 404696 && npc.Y == 503469)
					{
						Ohonat = npc;
						break;
					}

			if (Ohonat == null)
			{
				if (log.IsWarnEnabled)
					log.Warn("Could not find Ohonat , creating it ...");
				Ohonat = new GameNPC();
				Ohonat.LoadEquipmentTemplateFromDatabase("a58ef747-80e0-4cda-9052-15711ea0f4f7");
				Ohonat.Model = 761;
				Ohonat.Name = "O\'honat";
				Ohonat.GuildName = "";
				Ohonat.Realm = eRealm.Albion;
				Ohonat.CurrentRegionID = 51;
				Ohonat.Size = 52;
				Ohonat.Level = 50;
				Ohonat.X = 404696;
				Ohonat.Y = 503469;
				Ohonat.Z = 5192;
				Ohonat.Heading = 1037;
				Ohonat.VisibleActiveWeaponSlots = 51;
				Ohonat.Flags ^= GameNPC.eFlags.PEACE;
				Ohonat.MaxSpeedBase = 200;
				Ohonat.AddToWorld();
				if (SAVE_INTO_DATABASE)
				{
					Ohonat.SaveIntoDatabase();
				}
			}
			// end npc

			/*npcs = WorldMgr.GetNPCsByName("Nyaegha", eRealm.None);

			if (npcs.Length > 0)
				foreach (GameNPC npc in npcs)
					if (npc.CurrentRegionID == 51 && npc.X == 348381 && npc.Y == 479838)
					{
						Nyaegha = npc;
						break;
					}

			if (Nyaegha == null)
			{
				if (log.IsWarnEnabled)
					log.Warn("Could not find Nyaegha , creating it ...");
				Nyaegha = new GameNPC();
				Nyaegha.LoadEquipmentTemplateFromDatabase("Nyaegha");
				Nyaegha.Model = 605;
				Nyaegha.Name = "Nyaegha";
				Nyaegha.GuildName = "";
				Nyaegha.Realm = eRealm.None;
				Nyaegha.Race = 2001;
				Nyaegha.BodyType = (ushort)NpcTemplateMgr.eBodyType.Demon;
				Nyaegha.CurrentRegionID = 51;
				Nyaegha.Size = 150;
				Nyaegha.Level = 60;
				Nyaegha.X = 348381;
				Nyaegha.Y = 479838;
				Nyaegha.Z = 3320;
				Nyaegha.Heading = 3424;
				Nyaegha.VisibleActiveWeaponSlots = 34;
				Nyaegha.MaxSpeedBase = 200;
				Nyaegha.AddToWorld();
				if (SAVE_INTO_DATABASE)
				{
					Nyaegha.SaveIntoDatabase();
				}
			}*/
			
			#endregion

			#region defineItems

				ancient_copper_necklace = GameServer.Database.FindObjectByKey<ItemTemplate>("ancient_copper_necklace");
			if (ancient_copper_necklace == null)
			{
				if (log.IsWarnEnabled)
					log.Warn("Could not find Ancient Copper Necklace, creating it ...");
				ancient_copper_necklace = new ItemTemplate();
				ancient_copper_necklace.Id_nb = "ancient_copper_necklace";
				ancient_copper_necklace.Name = "Ancient Copper Necklace";
				ancient_copper_necklace.Level = 51;
				ancient_copper_necklace.Durability = 50000;
				ancient_copper_necklace.MaxDurability = 50000;
				ancient_copper_necklace.Condition = 50000;
				ancient_copper_necklace.MaxCondition = 50000;
				ancient_copper_necklace.Item_Type = 29;
				ancient_copper_necklace.Object_Type = (int)eObjectType.Magical;
				ancient_copper_necklace.Model = 101;
				ancient_copper_necklace.Bonus = 35;
				ancient_copper_necklace.IsDropable = true;
				ancient_copper_necklace.IsTradable = true;
				ancient_copper_necklace.IsIndestructible = false;
				ancient_copper_necklace.IsPickable = true;
				ancient_copper_necklace.Bonus1 = 10;
				ancient_copper_necklace.Bonus2 = 10;
				ancient_copper_necklace.Bonus3 = 10;
				ancient_copper_necklace.Bonus4 = 10;
				ancient_copper_necklace.Bonus1Type = 11;
				ancient_copper_necklace.Bonus2Type = 19;
				ancient_copper_necklace.Bonus3Type = 18;
				ancient_copper_necklace.Bonus4Type = 13;
				ancient_copper_necklace.Price = 0;
				ancient_copper_necklace.Realm = (int)eRealm.Albion;
				ancient_copper_necklace.DPS_AF = 0;
				ancient_copper_necklace.SPD_ABS = 0;
				ancient_copper_necklace.Hand = 0;
				ancient_copper_necklace.Type_Damage = 0;
				ancient_copper_necklace.Quality = 100;
				ancient_copper_necklace.Weight = 10;
				ancient_copper_necklace.LevelRequirement = 50;
				ancient_copper_necklace.BonusLevel = 30;
				ancient_copper_necklace.Description = "";
				if (SAVE_INTO_DATABASE)
				{
					GameServer.Database.AddObject(ancient_copper_necklace);
				}

			}
			
			scroll_wearyall_loststone = GameServer.Database.FindObjectByKey<ItemTemplate>("scroll_wearyall_loststone");
			if (scroll_wearyall_loststone == null)
			{
				if (log.IsWarnEnabled)
					log.Warn("Could not find Victory Speech for Albion, creating it ...");
				scroll_wearyall_loststone = new ItemTemplate();
				scroll_wearyall_loststone.Id_nb = "scroll_wearyall_loststone";
				scroll_wearyall_loststone.Name = "Victory Speech for Albion";
				scroll_wearyall_loststone.Level = 5;
				scroll_wearyall_loststone.Item_Type = 0;
				scroll_wearyall_loststone.Model = 498;
				scroll_wearyall_loststone.IsDropable = true;
				scroll_wearyall_loststone.IsTradable = false;
				scroll_wearyall_loststone.IsIndestructible = true;
				scroll_wearyall_loststone.IsPickable = true;
				scroll_wearyall_loststone.DPS_AF = 0;
				scroll_wearyall_loststone.SPD_ABS = 0;
				scroll_wearyall_loststone.Object_Type = 0;
				scroll_wearyall_loststone.Hand = 0;
				scroll_wearyall_loststone.Type_Damage = 0;
				scroll_wearyall_loststone.Quality = 100;
				scroll_wearyall_loststone.Weight = 1;
				scroll_wearyall_loststone.Description = "Bring this Speech to Honayt\'rt in Wearyall Village. She initiated this mission and deserves to be recognized.";
				if (SAVE_INTO_DATABASE)
				{
					GameServer.Database.AddObject(scroll_wearyall_loststone);
				}

			}
			
			lost_stone_of_arawn = GameServer.Database.FindObjectByKey<ItemTemplate>("lost_stone_of_arawn");
			if (lost_stone_of_arawn == null)
			{
				if (log.IsWarnEnabled)
					log.Warn("Could not find Lost Stone of Arawn, creating it ...");
				lost_stone_of_arawn = new ItemTemplate();
				lost_stone_of_arawn.Id_nb = "lost_stone_of_arawn";
				lost_stone_of_arawn.Name = "Lost Stone of Arawn";
				lost_stone_of_arawn.Level = 55;
				lost_stone_of_arawn.Item_Type = 0;
				lost_stone_of_arawn.Model = 110;
				lost_stone_of_arawn.IsDropable = true;
				lost_stone_of_arawn.IsTradable = false;
				lost_stone_of_arawn.IsIndestructible = true;
				lost_stone_of_arawn.IsPickable = true;
				lost_stone_of_arawn.DPS_AF = 0;
				lost_stone_of_arawn.SPD_ABS = 0;
				lost_stone_of_arawn.Object_Type = 0;
				lost_stone_of_arawn.Hand = 0;
				lost_stone_of_arawn.Type_Damage = 0;
				lost_stone_of_arawn.Quality = 100;
				lost_stone_of_arawn.Weight = 1;
				lost_stone_of_arawn.Description = "A stone of infinite power.";
				if (SAVE_INTO_DATABASE)
				{
					GameServer.Database.AddObject(lost_stone_of_arawn);
				}

			}
			//Item Descriptions End

			#endregion

			const int radius = 1500;
			Region region = WorldMgr.GetRegion(demonLocation.RegionID);
			demonArea = region.AddArea(new Area.Circle("Nyaegha Area", demonLocation.X, demonLocation.Y, demonLocation.Z, radius));
			demonArea.RegisterPlayerEnter(new DOLEventHandler(PlayerEnterDemonArea));
			
			GameEventMgr.AddHandler(GamePlayerEvent.AcceptQuest, new DOLEventHandler(SubscribeQuest));
			GameEventMgr.AddHandler(GamePlayerEvent.DeclineQuest, new DOLEventHandler(SubscribeQuest));

			GameEventMgr.AddHandler(Honaytrt, GameObjectEvent.Interact, new DOLEventHandler(TalkToHonaytrt));
			GameEventMgr.AddHandler(Honaytrt, GameLivingEvent.WhisperReceive, new DOLEventHandler(TalkToHonaytrt));
			
			GameEventMgr.AddHandler(Nchever, GameObjectEvent.Interact, new DOLEventHandler(TalkToNchever));
			GameEventMgr.AddHandler(Nchever, GameLivingEvent.WhisperReceive, new DOLEventHandler(TalkToNchever));
			
			GameEventMgr.AddHandler(Ohonat, GameObjectEvent.Interact, new DOLEventHandler(TalkToOhonat));
			GameEventMgr.AddHandler(Ohonat, GameLivingEvent.WhisperReceive, new DOLEventHandler(TalkToOhonat));
			
			//demonArea = WorldMgr.GetRegion(demonLocation.RegionID).AddArea(new Area.Circle("Nyaegha Area", demonLocation.X, demonLocation.Y, 0, 1500));
			//demonArea.RegisterPlayerEnter(new DOLEventHandler(PlayerEnterDemonArea));

			/* Now we bring to Honaytrt the possibility to give this quest to players */
			Honaytrt.AddQuestToGive(typeof (LostStoneofArawn));

			if (log.IsInfoEnabled)
				log.Info("Quest \"" + questTitle + "\" initialized");
		}

		[ScriptUnloadedEvent]
		public static void ScriptUnloaded(DOLEvent e, object sender, EventArgs args)
		{
			//if not loaded, don't worry
			if (Honaytrt == null)
				return;
			// remove handlers
			GameEventMgr.RemoveHandler(GamePlayerEvent.AcceptQuest, new DOLEventHandler(SubscribeQuest));
			GameEventMgr.RemoveHandler(GamePlayerEvent.DeclineQuest, new DOLEventHandler(SubscribeQuest));

			demonArea.UnRegisterPlayerEnter(new DOLEventHandler(PlayerEnterDemonArea));
			WorldMgr.GetRegion(demonLocation.RegionID).RemoveArea(demonArea);

			GameEventMgr.RemoveHandler(Honaytrt, GameObjectEvent.Interact, new DOLEventHandler(TalkToHonaytrt));
			GameEventMgr.RemoveHandler(Honaytrt, GameLivingEvent.WhisperReceive, new DOLEventHandler(TalkToHonaytrt));
			
			GameEventMgr.RemoveHandler(Nchever, GameObjectEvent.Interact, new DOLEventHandler(TalkToNchever));
			GameEventMgr.RemoveHandler(Nchever, GameLivingEvent.WhisperReceive, new DOLEventHandler(TalkToNchever));
			
			GameEventMgr.RemoveHandler(Ohonat, GameObjectEvent.Interact, new DOLEventHandler(TalkToOhonat));
			GameEventMgr.RemoveHandler(Ohonat, GameLivingEvent.WhisperReceive, new DOLEventHandler(TalkToOhonat));

			/* Now we remove to Honaytrt the possibility to give this quest to players */
			Honaytrt.RemoveQuestToGive(typeof (LostStoneofArawn));
		}

		protected virtual void CreateNyaegha()
		{
			Nyaegha = new GameNPC();
			Nyaegha.LoadEquipmentTemplateFromDatabase("Nyaegha");
			Nyaegha.Model = 605;
			Nyaegha.Name = "Nyaegha";
			Nyaegha.GuildName = "";
			Nyaegha.Realm = eRealm.None;
			Nyaegha.Race = 2001;
			Nyaegha.BodyType = (ushort)NpcTemplateMgr.eBodyType.Demon;
			Nyaegha.CurrentRegionID = 51;
			Nyaegha.Size = 150;
			Nyaegha.Level = 60;
			Nyaegha.X = 348381;
			Nyaegha.Y = 479838;
			Nyaegha.Z = 3320;
			Nyaegha.Heading = 3424;
			Nyaegha.VisibleActiveWeaponSlots = 34;
			Nyaegha.MaxSpeedBase = 250;
			Nyaegha.AddToWorld();
			

			StandardMobBrain brain = new StandardMobBrain();
			brain.AggroLevel = 200;
			brain.AggroRange = 500;
			Nyaegha.SetOwnBrain(brain);

			Nyaegha.AddToWorld();
			if (SAVE_INTO_DATABASE)
			{
				Nyaegha.SaveIntoDatabase();
			}

			foreach (GamePlayer player in Nyaegha.GetPlayersInRadius(1600))
			{
				if (player == null) return;
				
				Nyaegha.StartAttack(player);
			}
			
		}

		protected static void PlayerEnterDemonArea(DOLEvent e, object sender, EventArgs args)
		{
			AreaEventArgs aargs = args as AreaEventArgs;
			GamePlayer player = aargs?.GameObject as GamePlayer;
			
			if (player == null)
				return;
			
			LostStoneofArawn quest = player.IsDoingQuest(typeof (LostStoneofArawn)) as LostStoneofArawn;

			if (quest != null && Nyaegha == null && quest.Step == 4)
			{
				// player near demon           
				SendSystemMessage(player, "This is Marw Gwlad. The ground beneath your feet is cracked and burned, and the air holds a faint scent of brimstone.");
				player.Out.SendMessage("Nyaegha is angry and attacks you!", eChatType.CT_ScreenCenter, eChatLoc.CL_SystemWindow);
				quest.CreateNyaegha();
				
			}
		}

		protected static void TalkToHonaytrt(DOLEvent e, object sender, EventArgs args)
		{
			//We get the player from the event arguments and check if he qualifies		
			GamePlayer player = ((SourceEventArgs) args).Source as GamePlayer;
			if (player == null)
				return;

			if(Honaytrt.CanGiveQuest(typeof (LostStoneofArawn), player)  <= 0)
				return;

			//We also check if the player is already doing the quest
			LostStoneofArawn quest = player.IsDoingQuest(typeof (LostStoneofArawn)) as LostStoneofArawn;

			if (e == GameObjectEvent.Interact)
			{
				if (quest != null)
				{
					switch (quest.Step)
					{
						case 1:
							Honaytrt.SayTo(player, "Thank you for your help, N\'chever, O\'honat and me are trying to find this stone for very long.\n" +
							                       "Speak with N\'chever in Wearyall Village, he will tell you more about the [Stone of Arawn].");
							break;
						case 2:
							Honaytrt.SayTo(player, "Hey "+player.Name+", did you visit N\'chever yet? You can find him in Wearyall Village.");
							break;
						case 3:
							Honaytrt.SayTo(player, "Greetings, I heard you need to go to O\'honat, she can help you find what we are searching for.");
							break;
						case 4:
							Honaytrt.SayTo(player, "Wow, O\'honat really found something?\nI knew she could be counted on!");
							break;
						case 5:
							Honaytrt.SayTo(player, "Oh dear, did you really find the stone?\nPlease bring it to O\'honat first, she has to look at it!");
							break;
						case 6:
							Honaytrt.SayTo(player, "I cant really explain how happy I am, thank you for your help "+player.CharacterClass.Name+"!\n" +
							                       "Here is your [reward].");
							break;
					}
				}
				else
				{
					Honaytrt.SayTo(player, "Hello "+player.Name+", we live in dark times and I tried to locate the lost stone of the arawn for several years,\n" +
					                       "could you [help me] retrieve the stone?");
				}
			}
				// The player whispered to the NPC
			else if (e == GameLivingEvent.WhisperReceive)
			{
				WhisperReceiveEventArgs wArgs = (WhisperReceiveEventArgs) args;
				if (quest == null)
				{
					switch (wArgs.Text)
					{
						case "help me":
							player.Out.SendQuestSubscribeCommand(Honaytrt, QuestMgr.GetIDForQuestType(typeof(LostStoneofArawn)), "Will you help Honayt\'rt [Lost Stone of Arawn]?");
							break;
					}
				}
				else
				{
					switch (wArgs.Text)
					{
						case "Stone of Arawn":
							if (quest.Step == 1)
							{
								quest.Step = 2;
								Honaytrt.SayTo(player, "You can find N\'chever north in Wearyall Village and speak with him.");
							}
							break;
						case "reward":
							if (quest.Step == 6)
							{
								quest.FinishQuest();
							}
							break;
						case "abort":
							player.Out.SendCustomDialog("Do you really want to abort this quest, \nall items gained during quest will be lost?", new CustomDialogResponse(CheckPlayerAbortQuest));
							break;
					}
				}
			}
			else if (e == GameLivingEvent.ReceiveItem)
			{
				ReceiveItemEventArgs rArgs = (ReceiveItemEventArgs) args;
				if (quest != null)
				{
					/*if (rArgs.Item.Id_nb == .Id_nb)
					{
						Honaytrt.SayTo(player, "Thank you "+ player.Name +".\n");
						//quest.Step = 3;
					}*/
				}
			}
		}
		
		protected static void TalkToNchever(DOLEvent e, object sender, EventArgs args)
		{
			//We get the player from the event arguments and check if he qualifies		
			GamePlayer player = ((SourceEventArgs) args).Source as GamePlayer;
			if (player == null)
				return;

			//We also check if the player is already doing the quest
			LostStoneofArawn quest = player.IsDoingQuest(typeof (LostStoneofArawn)) as LostStoneofArawn;

			if (e == GameObjectEvent.Interact)
			{
				if (quest != null)
				{
					switch (quest.Step)
					{
						case 1:
							Nchever.SayTo(player, "Hey "+player.Name+", welcome in Wearyall Village, if you need some rest go to our stable.\n" +
							                      "There you can find my friend Honayt\'rt, i know you will like each other!");
							break;
						case 2:
							Nchever.SayTo(player, "Greetings, I see you spoke with Honayt\'rt about our mission? We are searching for a [stone], do you want to help us?");
							break;
						case 3:
							Nchever.SayTo(player, "Hey "+player.CharacterClass.Name+", did you visit O\'honat yet? You can find her in Caer Diogel on the ramparts.");
							break;
						case 4:
							Nchever.SayTo(player, "Incredible, O\'honat really found something?\nThats great!");
							break;
						case 5:
							Nchever.SayTo(player, "Please bring this stone to O\'honat, she knows what we need to do next!");
							break;
						case 6:
							Nchever.SayTo(player, "Thanks for showing me the stone, but bring it Honayt\'rt in the stable, she initiated this mission.");
							break;
					}
				}
				else
				{
					Nchever.SayTo(player, "Greetings, isn\'t it a perfect day?");
				}
			}
				// The player whispered to the NPC
			else if (e == GameLivingEvent.WhisperReceive)
			{
				WhisperReceiveEventArgs wArgs = (WhisperReceiveEventArgs) args;
				if (quest == null)
				{
					switch (wArgs.Text)
					{
					}
				}
				else
				{
					switch (wArgs.Text)
					{
						case "stone":
							if (quest.Step == 2)
							{
								Nchever.SayTo(player, "Visit O\'honat in Caer Diogel! Telling her about the [Lost Stone of Arawn], she is on the ramparts and will tell you more about it.");
							}
							break;
						case "Lost Stone of Arawn":
							if (quest.Step == 2)
							{
								quest.Step = 3;
								Nchever.SayTo(player, "Visit O\'honat in Caer Diogel!");
							}
							break;
					}
				}
			}
		}

		protected static void TalkToOhonat(DOLEvent e, object sender, EventArgs args)
		{
			//We get the player from the event arguments and check if he qualifies		
			GamePlayer player = ((SourceEventArgs) args).Source as GamePlayer;
			if (player == null)
				return;

			//We also check if the player is already doing the quest
			LostStoneofArawn quest = player.IsDoingQuest(typeof (LostStoneofArawn)) as LostStoneofArawn;

			if (e == GameObjectEvent.Interact)
			{
				if (quest != null)
				{
					switch (quest.Step)
					{
						case 1:
							Ohonat.SayTo(player, "Hello Adventurer, I am "+Ohonat.Name+"! Did you visit Wearyall Village?\n" +
							                     "I have some friends there, Honayt\'rt and N\'chever, feel free to speak with them.");
							break;
						case 2:
							Ohonat.SayTo(player, "Hey, did you visit Honayt\'rt or N\'chever yet? They are really very nice people.");
							break;
						case 3:
							Ohonat.SayTo(player, "Did N\'chever send you?\nYeah we doing a mission to find the lost stone of arawn. " +
							                     "I heard of a demon who kills animals and other creatures in [Gwyddneau] to get stronger, " +
							                     "we have to do something immediately, otherwise it is too late for Albion!");
							break;
						case 4:
							Ohonat.SayTo(player, "Leave Caer Diogel and head west out of town, when you reach the coast turn north. " +
							                     "The Demon that you will need to kill can be found in the Plains of Gwyddneau!\n" +
							                     "Kill this demon and bring me the stone.");
							break;
						case 5:
							Ohonat.SayTo(player, "Hey "+player.Name+", you are our savior in need. I thought it will be [impossible].");
							break;
						case 6:
							Ohonat.SayTo(player, "I know Honayt\'rt will be very happy. Show her the speech!");
							break;
					}
				}
				else
				{
					Ohonat.SayTo(player, "Greetings Adventurer, feel free to buy something in our merchant house, if you need anything.");
				}
			}
				// The player whispered to the NPC
			else if (e == GameLivingEvent.WhisperReceive)
			{
				WhisperReceiveEventArgs wArgs = (WhisperReceiveEventArgs) args;
				if (quest == null)
				{
					switch (wArgs.Text)
					{
					}
				}
				else
				{
					switch (wArgs.Text)
					{ 
						case "Gwyddneau":
							if (quest.Step == 3)
							{
								quest.Step = 4;
								Ohonat.SayTo(player, "Leave Caer Diogel and head west out of town, when you reach the coast turn north. " +
								                     "The Demon that you will need to kill can be found in the Plains of Gwyddneau!\n" +
								                     "Kill this demon and bring me the stone.");
							}
							break;
						case "impossible":
							if (quest.Step == 5)
							{
								Ohonat.SayTo(player, "I will give you a scroll, bring this to Honayt\'rt, she needs to see it!\n[Farewell] my savior of Albion!");
								Ohonat.Emote(eEmote.Cheer);
							}
							break;
						case "Farewell":
							if (quest.Step == 5 && player.Inventory.IsSlotsFree(1, eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack))
							{
								GiveItem(player, scroll_wearyall_loststone);
								RemoveItem(player, lost_stone_of_arawn);
								player.Out.SendSpellEffectAnimation(Ohonat, player, 4310, 0, false, 1);
								new ECSGameTimer(player, new ECSGameTimer.ECSTimerCallback(timer => TeleportToWearyall(timer, player)), 3000);
								quest.Step = 6;
								Ohonat.SayTo(player, "I know Honayt\'rt will be very happy. Show her the speech!");
							}
							break;
					}
				}
			}
			else if (e == GameLivingEvent.ReceiveItem)
			{
				ReceiveItemEventArgs rArgs = (ReceiveItemEventArgs) args;
				if (quest != null)
				{
					if (rArgs.Item.Id_nb == lost_stone_of_arawn.Id_nb)
					{
						Ohonat.SayTo(player, "Thank you "+ player.Name +". I will give you a scroll, bring this to Honayt\'rt, she needs to see it!\n[Farewell] my savior of Albion!\n");
						Ohonat.Emote(eEmote.Cheer);
					}
				}
			}
		}

		public static int TeleportToWearyall(ECSGameTimer timer, GamePlayer player)
		{
			//teleport to wearyall village
			player.MoveTo(51, 435868, 493994, 3088, 3587);
			return 0;
		}

		public override bool CheckQuestQualification(GamePlayer player)
		{
			// if the player is already doing the quest his level is no longer of relevance
			if (player.IsDoingQuest(typeof (LostStoneofArawn)) != null)
				return true;

			if (player.Level < minimumLevel || player.Level > maximumLevel)
				return false;

			return true;
		}

		private static void CheckPlayerAbortQuest(GamePlayer player, byte response)
		{
			LostStoneofArawn quest = player.IsDoingQuest(typeof (LostStoneofArawn)) as LostStoneofArawn;

			if (quest == null)
				return;

			if (response == 0x00)
			{
				SendSystemMessage(player, "Good, now go out there and finish your work!");
			}
			else
			{
				SendSystemMessage(player, "Aborting Quest " + questTitle + ". You can start over again if you want.");
				quest.AbortQuest();
			}
		}

		protected static void SubscribeQuest(DOLEvent e, object sender, EventArgs args)
		{
			QuestEventArgs qargs = args as QuestEventArgs;
			if (qargs == null)
				return;

			if (qargs.QuestID != QuestMgr.GetIDForQuestType(typeof(LostStoneofArawn)))
				return;

			if (e == GamePlayerEvent.AcceptQuest)
				CheckPlayerAcceptQuest(qargs.Player, 0x01);
			else if (e == GamePlayerEvent.DeclineQuest)
				CheckPlayerAcceptQuest(qargs.Player, 0x00);
		}

		private static void CheckPlayerAcceptQuest(GamePlayer player, byte response)
		{
			if(Honaytrt.CanGiveQuest(typeof (LostStoneofArawn), player)  <= 0)
				return;

			if (player.IsDoingQuest(typeof (LostStoneofArawn)) != null)
				return;

			if (response == 0x00)
			{
				player.Out.SendMessage("Come back if you are ready to help us in our mission.", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
			}
			else
			{
				//Check if we can add the quest!
				if (!Honaytrt.GiveQuest(typeof (LostStoneofArawn), player, 1))
					return;

				Honaytrt.SayTo(player, "Thank you, lets talk more about the stone!");
				Honaytrt.SayTo(player, "N\'chever, O\'honat and me are trying to find this stone for a very long time.\n" +
				                       "Speak with N\'chever in Wearyall Village, he will tell you more about the [Stone of Arawn].");

			}
		}

		//Set quest name
		public override string Name
		{
			get { return questTitle; }
		}

		// Define Steps
		public override string Description
		{
			get
			{
				switch (Step)
				{
					case 1:
						return "Speak to Honayt\'rt in Wearyall Village.";
					case 2:
						return "Speak to N\'chever in Wearyall Village.";
					case 3:
						return "Speak to O\'honat in Caer Diogel.";
					case 4:
						return "Leave Caer Diogel and head west out of town, when you reach the coast turn north. " +
						       "The Demon that you will need to kill can be found in the Plains of Gwyddneau.";
					case 5:
						return "Go back to Caer Diogel and give O'honat the stone.";
					case 6:
						return "Read the speech to see who it is addressed to and turn it in for your reward.";
				}
				return base.Description;
			}
		}

		public override void Notify(DOLEvent e, object sender, EventArgs args)
		{
			GamePlayer player = sender as GamePlayer;
			
			if (sender != m_questPlayer)
				return;
			
			if (player==null || player.IsDoingQuest(typeof (LostStoneofArawn)) == null)
				return;

			if (e == GameLivingEvent.EnemyKilled && Step == 4 && player.TargetObject.Name == Nyaegha.Name)
			{
				if (!m_questPlayer.Inventory.IsSlotsFree(1, eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack))
				{
					player.Out.SendMessage("You dont have enough room for "+lost_stone_of_arawn.Name+" and drops on the ground.", eChatType.CT_Important, eChatLoc.CL_SystemWindow);
				}
				GiveItem(player, lost_stone_of_arawn);
				Step = 5;
			}
		}

		public override void AbortQuest()
		{
			base.AbortQuest(); //Defined in Quest, changes the state, stores in DB etc ...
			RemoveItem(m_questPlayer, lost_stone_of_arawn);
			RemoveItem(m_questPlayer, scroll_wearyall_loststone);
		}

		public override void FinishQuest()
		{
			if (m_questPlayer.Inventory.IsSlotsFree(1, eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack))
			{
				if (m_questPlayer.Level >= 49)
				{
					m_questPlayer.GainExperience(eXPSource.Quest, (m_questPlayer.ExperienceForNextLevel - m_questPlayer.ExperienceForCurrentLevel) / 3, false);
				}
				else
				{
					m_questPlayer.GainExperience(eXPSource.Quest, (m_questPlayer.ExperienceForNextLevel - m_questPlayer.ExperienceForCurrentLevel) / 2, false);
				}
				RemoveItem(m_questPlayer, scroll_wearyall_loststone);
				GiveItem(m_questPlayer, ancient_copper_necklace);
				m_questPlayer.AddMoney(Money.GetMoney(0,0,121,41,Util.Random(50)), "You receive {0} as a reward.");

				base.FinishQuest(); //Defined in Quest, changes the state, stores in DB etc ...
			}
			else
			{
				m_questPlayer.Out.SendMessage("You do not have enough free space in your inventory!", eChatType.CT_Important, eChatLoc.CL_SystemWindow);
			}
		}
	}
}
