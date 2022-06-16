﻿using DOL.AI.Brain;
using DOL.Database;
using DOL.GS;

namespace DOL.GS
{
	public class Loken : GameEpicBoss
	{
		public Loken() : base() { }
		public override int GetResist(eDamageType damageType)
		{
			switch (damageType)
			{
				case eDamageType.Slash: return 40;// dmg reduction for melee dmg
				case eDamageType.Crush: return 40;// dmg reduction for melee dmg
				case eDamageType.Thrust: return 40;// dmg reduction for melee dmg
				default: return 70;// dmg reduction for rest resists
			}
		}
		public override double AttackDamage(InventoryItem weapon)
		{
			return base.AttackDamage(weapon) * Strength / 100;
		}
		public override int AttackRange
		{
			get { return 350; }
			set { }
		}
		public override bool HasAbility(string keyName)
		{
			if (IsAlive && keyName == GS.Abilities.CCImmunity)
				return true;

			return base.HasAbility(keyName);
		}
		public override double GetArmorAF(eArmorSlot slot)
		{
			return 350;
		}
		public override double GetArmorAbsorb(eArmorSlot slot)
		{
			// 85% ABS is cap.
			return 0.20;
		}
		public override int MaxHealth
		{
			get { return 30000; }
		}
		public override bool AddToWorld()
		{
			INpcTemplate npcTemplate = NpcTemplateMgr.GetTemplate(60163372);
			LoadTemplate(npcTemplate);
			Strength = npcTemplate.Strength;
			Dexterity = npcTemplate.Dexterity;
			Constitution = npcTemplate.Constitution;
			Quickness = npcTemplate.Quickness;
			Piety = npcTemplate.Piety;
			Intelligence = npcTemplate.Intelligence;
			Empathy = npcTemplate.Empathy;
			SpawnWolfs();

			LokenBrain sbrain = new LokenBrain();
			SetOwnBrain(sbrain);
			LoadedFromScript = false;//load from database
			SaveIntoDatabase();
			base.AddToWorld();
			return true;
		}
        public override void Die(GameObject killer)
        {
			foreach(GameNPC npc in GetNPCsInRadius(8000))
            {
				if (npc != null && npc.IsAlive && npc.Brain is LokenWolfBrain)
					npc.Die(this);
            }
            base.Die(killer);
        }
        private void SpawnWolfs()
		{
			Point3D spawn = new Point3D(636780, 762427, 4597);
			for (int i = 0; i < 2; i++)
			{
				LokenWolf npc = new LokenWolf();
				npc.X = spawn.X + Util.Random(-100, 100);
				npc.Y = spawn.Y + Util.Random(-100, 100);
				npc.Z = spawn.Z;
				npc.Heading = Heading;
				npc.CurrentRegion = CurrentRegion;
				npc.AddToWorld();
			}
		}
	}
}
namespace DOL.AI.Brain
{
	public class LokenBrain : StandardMobBrain
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public LokenBrain() : base()
		{
			AggroLevel = 100;
			AggroRange = 500;
			ThinkInterval = 1500;
		}
		private bool SpawnWolf = false;
		public override void Think()
		{
			if(!HasAggressionTable())
            {
				FSM.SetCurrentState(eFSMStateType.RETURN_TO_SPAWN);
				Body.Health = Body.MaxHealth;
				if (LokenWolf.WolfsCount < 2 && !SpawnWolf)
                {
					SpawnWolfs();
					SpawnWolf = true;
                }
            }
			if(HasAggro)
            {
				SpawnWolf = false;
				GameLiving target = Body.TargetObject as GameLiving;
				foreach (GameNPC npc in Body.GetNPCsInRadius(1000))
				{
					if (npc != null && npc.IsAlive && npc.Brain is LokenWolfBrain brian)
					{
						if (!brian.HasAggro && brian != null && target != null && target.IsAlive)
							brian.AddToAggroList(target, 10);
					}
				}
			}
			base.Think();
		}
		private void SpawnWolfs()
		{
			Point3D spawn = new Point3D(636780, 762427, 4597);
			for (int i = 0; i < 2; i++)
			{
				if (LokenWolf.WolfsCount < 2)
				{
					LokenWolf npc = new LokenWolf();
					npc.X = spawn.X + Util.Random(-100, 100);
					npc.Y = spawn.Y + Util.Random(-100, 100);
					npc.Z = spawn.Z;
					npc.Heading = Body.Heading;
					npc.CurrentRegion = Body.CurrentRegion;
					npc.AddToWorld();
				}
			}
		}
	}
}

#region Loken wolfs
namespace DOL.GS
{
	public class LokenWolf : GameNPC
	{
		public LokenWolf() : base() { }
		#region Stats
		public override short Constitution { get => base.Constitution; set => base.Constitution = 200; }
		public override short Dexterity { get => base.Dexterity; set => base.Dexterity = 200; }
		public override short Quickness { get => base.Quickness; set => base.Quickness = 80; }
		public override short Strength { get => base.Strength; set => base.Strength = 150; }
		#endregion
		public override bool AddToWorld()
		{
			Name = "Loken's companion";
			Level = 50;
			Model = 459;
			Size = 45;
			LokenWolfBrain sbrain = new LokenWolfBrain();
			SetOwnBrain(sbrain);
			++WolfsCount;
			MaxSpeedBase = 225;
			LoadedFromScript = true;
			RespawnInterval = -1;
			base.AddToWorld();
			return true;
		}
		public static int WolfsCount = 0;
        public override void Die(GameObject killer)
        {
			--WolfsCount;
            base.Die(killer);
        }
    }
}
namespace DOL.AI.Brain
{
	public class LokenWolfBrain : StandardMobBrain
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public LokenWolfBrain() : base()
		{
			AggroLevel = 100;
			AggroRange = 500;
			ThinkInterval = 1500;
		}
		public override void Think()
		{
			base.Think();
		}
	}
}
#endregion