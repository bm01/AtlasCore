using System;
using System.Reflection;
using System.Collections;
using DOL.Events;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.Database;
using DOL.GS.Effects;

namespace DOL.GS
{
    public class GuildBanner
    {
		private static readonly Logging.Logger log = Logging.LoggerManager.Create(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		ECSGameTimer m_timer;
        GamePlayer m_player;
		GuildBannerItem m_item;
        WorldInventoryItem gameItem;

        public GuildBanner(GamePlayer player)
		{
			m_player = player;
		}

        public GamePlayer Player
        {
            get { return m_player; }
        }

		public GuildBannerItem BannerItem
		{
			get { return m_item; }
		}

        public void Start()
        {
            if (m_player.Group != null)
            {
                if (m_player != null)
                {
					bool groupHasBanner = false;

                    foreach (GamePlayer groupPlayer in m_player.Group.GetPlayersInTheGroup())
                    {
                        if (groupPlayer.GuildBanner != null)
						{
							groupHasBanner = true;
							break;
						}
					}

                    if (groupHasBanner == false)
                    {
						if (m_item == null)
						{
							GuildBannerItem item = new GuildBannerItem(GuildBannerTemplate);

							item.OwnerGuild = m_player.Guild;
							item.SummonPlayer = m_player;
							m_item = item;
						}

						m_player.GuildBanner = this;
						m_player.Stealth(false);
						AddHandlers();

                        if (m_timer != null)
                        {
                            m_timer.Stop();
                            m_timer = null;
                        }

                        m_timer = new ECSGameTimer(m_player, new ECSGameTimer.ECSTimerCallback(TimerTick));
                        m_timer.Start(1);

                    }
                    else
                    {
                        m_player.Out.SendMessage("Someone in your group already has a guild banner active!", eChatType.CT_Loot, eChatLoc.CL_SystemWindow);
                    }
                }
                else
                {
                    if (m_timer != null)
                    {
                        m_timer.Stop();
                        m_timer = null;
                    }
                }
            }
            else if (m_player.Client.Account.PrivLevel == (int)ePrivLevel.Player)
            {
                m_player.Out.SendMessage("You have left the group and your guild banner disappears!", eChatType.CT_Loot, eChatLoc.CL_SystemWindow);
                m_player.GuildBanner = null;
                if (m_timer != null)
                {
                    m_timer.Stop();
                    m_timer = null;
                }
            }
        }

        public void Stop()
        {
			RemoveHandlers();
            if (m_timer != null)
            {
                m_timer.Stop();
                m_timer = null;
            }
        }

        private int TimerTick(ECSGameTimer timer)
        {
            foreach (GamePlayer player in m_player.GetPlayersInRadius(1500))
            {
                if (player.Group != null && m_player.Group != null && m_player.Group.IsInTheGroup(player))
                {
                    if (GameServer.ServerRules.IsAllowedToAttack(m_player, player, true) == false)
                    {
                        GuildBannerEffect effect = GuildBannerEffect.CreateEffectOfClass(m_player, player);

                        if (effect != null)
                        {
                            GuildBannerEffect oldEffect = player.EffectList.GetOfType(effect.GetType()) as GuildBannerEffect;
							if (oldEffect == null)
							{
								effect.Start(player);
							}
							else
							{
								oldEffect.Stop();
								effect.Start(player);
							}
                        }
                    }
                }
            }

            return 9000; // Pulsing every 9 seconds with a duration of 9 seconds - Tolakram
        }

        protected virtual void AddHandlers()
        {
			GameEventMgr.AddHandler(m_player, GamePlayerEvent.LeaveGroup, new DOLEventHandler(PlayerLoseBanner));
			GameEventMgr.AddHandler(m_player, GamePlayerEvent.Quit, new DOLEventHandler(PlayerLoseBanner));
            GameEventMgr.AddHandler(m_player, GamePlayerEvent.StealthStateChanged, new DOLEventHandler(PlayerLoseBanner));
            GameEventMgr.AddHandler(m_player, GamePlayerEvent.Linkdeath, new DOLEventHandler(PlayerLoseBanner));
			GameEventMgr.AddHandler(m_player, GamePlayerEvent.RegionChanging, new DOLEventHandler(PlayerLoseBanner));
			GameEventMgr.AddHandler(m_player, GamePlayerEvent.Dying, new DOLEventHandler(PlayerDied));
        }

		protected virtual void RemoveHandlers()
		{
			GameEventMgr.RemoveHandler(m_player, GamePlayerEvent.LeaveGroup, new DOLEventHandler(PlayerLoseBanner));
			GameEventMgr.RemoveHandler(m_player, GamePlayerEvent.Quit, new DOLEventHandler(PlayerLoseBanner));
			GameEventMgr.RemoveHandler(m_player, GamePlayerEvent.StealthStateChanged, new DOLEventHandler(PlayerLoseBanner));
			GameEventMgr.RemoveHandler(m_player, GamePlayerEvent.Linkdeath, new DOLEventHandler(PlayerLoseBanner));
			GameEventMgr.RemoveHandler(m_player, GamePlayerEvent.RegionChanging, new DOLEventHandler(PlayerLoseBanner));
			GameEventMgr.RemoveHandler(m_player, GamePlayerEvent.Dying, new DOLEventHandler(PlayerDied));
		}

		protected void PlayerLoseBanner(DOLEvent e, object sender, EventArgs args)
        {
			Stop();
			m_player.GuildBanner = null;
			m_player.Guild.SendMessageToGuildMembers(string.Format("{0} has put away the guild banner!", m_player.Name), eChatType.CT_Guild, eChatLoc.CL_SystemWindow);
			m_player = null;
        }

        protected void PlayerDied(DOLEvent e, object sender, EventArgs args)
        {
            DyingEventArgs arg = args as DyingEventArgs;
            if (arg == null) return;
            GameObject killer = arg.Killer as GameObject;
			GamePlayer playerKiller = null;

			if (killer is GamePlayer)
			{
				playerKiller = killer as GamePlayer;
			}
			else if (killer is GameNPC && (killer as GameNPC).Brain != null && (killer as GameNPC).Brain is AI.Brain.IControlledBrain)
			{
				playerKiller = ((killer as GameNPC).Brain as AI.Brain.IControlledBrain).Owner as GamePlayer;
			}

			Stop();
			m_player.Guild.SendMessageToGuildMembers(m_player.Name + " has dropped the guild banner!", eChatType.CT_Guild, eChatLoc.CL_SystemWindow);

			gameItem = new WorldInventoryItem(m_item);
			Point2D point = m_player.GetPointFromHeading(m_player.Heading, 30);
            gameItem.X = point.X;
            gameItem.Y = point.Y;
            gameItem.Z = m_player.Z;
            gameItem.Heading = m_player.Heading;
            gameItem.CurrentRegionID = m_player.CurrentRegionID;
			gameItem.AddOwner(m_player);

			if (playerKiller != null)
			{
				// Guild banner can be picked up by anyone in the enemy group
				if (playerKiller.Group != null)
				{
					foreach (GamePlayer player in playerKiller.Group.GetPlayersInTheGroup())
					{
						gameItem.AddOwner(player);
					}
				}
				else
				{
					gameItem.AddOwner(playerKiller);
				}
			}

			// Guild banner can be picked up by anyone in the dead players group
			if (m_player.Group != null)
			{
				foreach (GamePlayer player in m_player.Group.GetPlayersInTheGroup())
				{
					gameItem.AddOwner(player);
				}
			}

            gameItem.StartPickupTimer(10);
			m_item.OnLose(m_player);
            gameItem.AddToWorld();
        }

        protected DbItemTemplate m_guildBannerTemplate;
        public DbItemTemplate GuildBannerTemplate
        {
            get
            {
                if (m_guildBannerTemplate == null)
                {
					string guildIDNB = "GuildBanner_" + m_player.Guild.GuildID;

					m_guildBannerTemplate = new DbItemTemplate();
					m_guildBannerTemplate.CanDropAsLoot = false;
					m_guildBannerTemplate.Id_nb = guildIDNB;
					m_guildBannerTemplate.IsDropable = false;
					m_guildBannerTemplate.IsPickable = true;
					m_guildBannerTemplate.IsTradable = false;
					m_guildBannerTemplate.IsIndestructible = true;
					m_guildBannerTemplate.Item_Type = 41;
					m_guildBannerTemplate.Level = 1;
					m_guildBannerTemplate.MaxCharges = 1;
					m_guildBannerTemplate.MaxCount = 1;
					m_guildBannerTemplate.Emblem = m_player.Guild.Emblem;
					switch (m_player.Realm)
					{
						case eRealm.Albion:
							m_guildBannerTemplate.Model = 3223;
							break;
						case eRealm.Midgard:
							m_guildBannerTemplate.Model = 3224;
							break;
						case eRealm.Hibernia:
							m_guildBannerTemplate.Model = 3225;
							break;
					}
					m_guildBannerTemplate.Name = m_player.Guild.Name + "'s Banner";
					m_guildBannerTemplate.Object_Type = (int)eObjectType.HouseWallObject;
					m_guildBannerTemplate.Realm = 0;
					m_guildBannerTemplate.Quality = 100;
					m_guildBannerTemplate.ClassType = "DOL.GS.GuildBannerItem";
					m_guildBannerTemplate.PackageID = "GuildBanner";
                }

                return m_guildBannerTemplate;
            }
        }

    }
}



