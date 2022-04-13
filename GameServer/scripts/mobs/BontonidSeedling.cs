﻿using System;
using DOL.AI;
using DOL.AI.Brain;
using DOL.Database;
using DOL.Events;
using DOL.GS.Behaviour.Triggers;
using DOL.GS.PacketHandler;
using DOL.GS.Scripts.DOL.AI.Brain;
using DOL.GS.Scripts.DOL.GS;

namespace DOL.GS.Scripts
{
    public class BontonidSeedling : GameNPC
    {
        
        public BontonidSeedling() : base() { }
        public BontonidSeedling(ABrain defaultBrain) : base(defaultBrain) { }
        public BontonidSeedling(INpcTemplate template) : base(template) { }

        public override bool AddToWorld()
        {
            INpcTemplate npcTemplate = NpcTemplateMgr.GetTemplate(60158658);
            LoadTemplate(npcTemplate);

            Size = 10;
            Strength = npcTemplate.Strength;
            Constitution = npcTemplate.Constitution;
            Dexterity = npcTemplate.Dexterity;
            Quickness = npcTemplate.Quickness;
            Empathy = npcTemplate.Empathy;
            Piety = npcTemplate.Piety;
            Intelligence = npcTemplate.Intelligence;

            // plant
            BodyType = 10;
            Race = 2007;

            //3min
            RespawnInterval = 180000;
            Faction = FactionMgr.GetFactionByID(69);
            Faction.AddFriendFaction(FactionMgr.GetFactionByID(69));

            BotonidBrain sBrain = new BotonidBrain();
            SetOwnBrain(sBrain);

            base.AddToWorld();
            return true;
        }

        public override void Die(GameObject killer)
        {
            base.Die(null);
            
            foreach (GamePlayer player in GetPlayersInRadius(400))
            {
                player.Out.SendMessage("The lure dissapears and the Scourgin jumps out at " + player.Name, eChatType.CT_Say,
                    eChatLoc.CL_ChatWindow);
                SpawnScourgin(player); 
            }
            
        }
        
        private void SpawnScourgin(GamePlayer target)
        {
            var distanceDelta = Util.Random(0, 150);

            ScourginAdd add = new ScourginAdd();
            add.X = target.X + distanceDelta;
            add.Y = target.Y + distanceDelta;
            add.Z = target.Z;
            add.Strength = Strength;
            add.Constitution = Constitution;
            add.Dexterity = Dexterity;
            add.Empathy = Empathy;
            add.Quickness = Quickness;
            add.CurrentRegionID = target.CurrentRegionID;
            add.Level = Level;
            add.AddToWorld();
            add.StartAttack(target);
        }
    }
    
namespace DOL.AI.Brain
    {
        public class BotonidBrain : StandardMobBrain
        {
            public BotonidBrain() : base()
            {
                AggroLevel = 100;
                AggroRange = 500;
            }
            
            public override int ThinkInterval
            {
                get { return 1000; }
            }

            public override void Think()
            {
                base.Think();
                if (Body.TargetObject != null && HasAggro & Body.IsAlive)
                {
                    if (Body.IsWithinRadius(Body.TargetObject, 150))
                    {
                        Body.Die(null);
                    }
                }
                
            }

            public override void Notify(DOLEvent e, object sender, EventArgs args)
            {
                base.Notify(e, sender, args);
            }
        }
    } 

    namespace DOL.GS
    {
        public class ScourginAdd : GameNPC
        {
            public ScourginAdd()
                : base()
            {
            }

            public override bool AddToWorld()
            {
                int scourginModel = Util.Random(889, 890);
                
                Name = "scourgin";
                Model = (ushort)scourginModel;
                
                if (scourginModel == 889)
                    Gender = eGender.Male;
                else if (scourginModel == 890)
                    Gender = eGender.Female;
                
                Faction = FactionMgr.GetFactionByID(69);
                Faction.AddFriendFaction(FactionMgr.GetFactionByID(69));
                Size = 40;
                RoamingRange = 350;
                RespawnInterval = -1;
                IsWorthReward = true;
                base.AddToWorld();
                return true;
            }
        }
    }

    namespace DOL.AI.Brain
    {
        public class ScourginBrain : StandardMobBrain
        {
            public ScourginBrain() : base()
            {
                AggroLevel = 100;
                AggroRange = 500;
            }

            public override void Think()
            {
                if (Body.InCombatInLast(60 * 1000) == false && Body.InCombatInLast(65 * 1000))
                {
                    Body.Die(null);
                }
                base.Think();
            }
        }
    }
}
