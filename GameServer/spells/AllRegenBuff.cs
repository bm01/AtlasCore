using System.Collections.Generic;
using DOL.AI.Brain;
using DOL.GS.PacketHandler;

namespace DOL.GS.Spells
{
	/// <summary>
	/// All Stats buff
	/// </summary>
	[SpellHandler(eSpellType.AllRegenBuff)]
	public class AllRegenBuff : PropertyChangingSpell
	{
		public static List<int> RegenList = new List<int> {8084,8080,8076};
		private int pomID = 8084;
		private int endID = 8080;
		private int healID = 8076;

		public override bool StartSpell(GameLiving target)
        {
			SpellLine potionEffectLine = SkillBase.GetSpellLine(GlobalSpellsLines.Potions_Effects);

			Spell pomSpell = SkillBase.FindSpell(pomID, potionEffectLine);
			SpellHandler pomSpellHandler = ScriptMgr.CreateSpellHandler(target, pomSpell, potionEffectLine) as SpellHandler;

			Spell endSpell = SkillBase.FindSpell(endID, potionEffectLine);
			SpellHandler endSpellHandler = ScriptMgr.CreateSpellHandler(target, endSpell, potionEffectLine) as SpellHandler;

			Spell healSpell = SkillBase.FindSpell(healID, potionEffectLine);
			SpellHandler healthConSpellHandler = ScriptMgr.CreateSpellHandler(target, healSpell, potionEffectLine) as SpellHandler;

			return pomSpellHandler.StartSpell(target) |
				endSpellHandler.StartSpell(target) |
				healthConSpellHandler.StartSpell(target);
		}

        public override eProperty Property1 => eProperty.PowerRegenerationAmount;
        public override eProperty Property2 => eProperty.EnduranceRegenerationAmount;
        public override eProperty Property3 => eProperty.HealthRegenerationAmount;

        // constructor
        public AllRegenBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line)
		{
		}
	}

	/// <summary>
	/// All Stats buff
	/// </summary>
	[SpellHandler(eSpellType.BeadRegen)]
	public class BeadRegen : PropertyChangingSpell 
	{
		public static List<int> BeadRegenList = new List<int> {31057,31056,31055};
		private int pomID = 31057;
		private int endID = 31056;
		private int healID = 31055;

		public override bool StartSpell(GameLiving target)
		{
            if (Caster.CurrentZone.IsRvR)
            {
				if (Caster is GamePlayer p)
					p.Out.SendMessage("You cannot use this item in an RvR zone.", eChatType.CT_SpellResisted, eChatLoc.CL_SystemWindow);
				return false;
            }
			if (Caster.Level > 49)
            {
				if (Caster is GamePlayer p)
					p.Out.SendMessage("You are too powerful for this item's effects.", eChatType.CT_SpellResisted, eChatLoc.CL_SystemWindow);
				return false;
			}

			if(Caster.ControlledBrain != null && Caster.ControlledBrain is AI.Brain.NecromancerPetBrain necroPet && necroPet.Body.InCombatInLast(5000))
            {
				if (Caster is GamePlayer p)
					p.Out.SendMessage("Your pet must be out of combat for 5 seconds to use this.", eChatType.CT_SpellResisted, eChatLoc.CL_SystemWindow);
				return false;
			}
				
			target = Caster;
			SpellLine potionEffectLine = SkillBase.GetSpellLine(GlobalSpellsLines.Potions_Effects);

			Spell pomSpell = SkillBase.FindSpell(pomID, potionEffectLine);
			SpellHandler pomSpellHandler = ScriptMgr.CreateSpellHandler(target, pomSpell, potionEffectLine) as SpellHandler;

			Spell endSpell = SkillBase.FindSpell(endID, potionEffectLine);
			SpellHandler endSpellHandler = ScriptMgr.CreateSpellHandler(target, endSpell, potionEffectLine) as SpellHandler;

			Spell healSpell = SkillBase.FindSpell(healID, potionEffectLine);
			SpellHandler healthConSpellHandler = ScriptMgr.CreateSpellHandler(target, healSpell, potionEffectLine) as SpellHandler;

			bool success;
			success = pomSpellHandler.StartSpell(target);
			success = endSpellHandler.StartSpell(target) || success;
			success = healthConSpellHandler.StartSpell(target) || success;

			if (Caster.ControlledBrain is NecromancerPetBrain brain)
			{
				SpellHandler petHealHandler = ScriptMgr.CreateSpellHandler(brain.Body, healSpell, potionEffectLine) as SpellHandler;
				success = petHealHandler.StartSpell(brain.Body) || success;
			}

			return success;
		}

		public override eProperty Property1 => eProperty.PowerRegenerationAmount;
		public override eProperty Property2 => eProperty.EnduranceRegenerationAmount;
		public override eProperty Property3 => eProperty.HealthRegenerationAmount;

		// constructor
		public BeadRegen(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line)
		{
		}
	}
}
