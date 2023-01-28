using System;
using DOL.GS.ServerProperties;

namespace DOL.GS
{
	public class TheurgistPet : GameSummonedPet
	{
		public TheurgistPet(INpcTemplate npcTemplate) : base(npcTemplate)
		{
			if (npcTemplate.Name.ToLower().Contains("earth"))
				ScalingFactor = 17;
			else if (npcTemplate.Name.ToLower().Contains("air"))
			{
				ScalingFactor = 11;

				// Make air pet's instant stun a bit more random, see 'DisableSkill'.
				// Should ideally be in its own class.
				foreach (Spell spell in Spells)
				{
					if (spell.IsInstantCast)
						DisableSkill(spell, 0);
				}
			}
		}

		public override void DisableSkill(Skill skill, int duration)
		{
			// Make air pet's instant stun a bit more random.
			// Should ideally be in its own class.
			if (skill is Spell spell && spell.IsInstantCast)
				duration += Util.Random((int)(spell.RecastDelay / 2.5));

			base.DisableSkill(skill, duration);
		}

		protected override void BuildAmbientTexts()
		{
			base.BuildAmbientTexts();

			// Not each summoned pet will fire ambient sentences.
			if (ambientTexts.Count > 0)
			{
				foreach (MobXAmbientBehaviour ambientText in ambientTexts)
					ambientText.Chance /= 10;
			}
		}

		public override void AutoSetStats()
		{
			Strength = Properties.PET_AUTOSET_STR_BASE;
			if (Strength < 1)
				Strength = 1;

			Constitution = Properties.PET_AUTOSET_CON_BASE;
			if (Constitution < 1)
				Constitution = 1;

			Quickness = Properties.PET_AUTOSET_QUI_BASE;
			if (Quickness < 1)
				Quickness = 1;

			Dexterity = Properties.PET_AUTOSET_DEX_BASE;
			if (Dexterity < 1)
				Dexterity = 1;

			Intelligence = Properties.PET_AUTOSET_INT_BASE;
			if (Intelligence < 1)
				Intelligence = 1;

			Empathy = 30;
			Piety = 30;
			Charisma = 30;

			if (Level > 1)
			{
				// Now add stats for levelling
				Strength += (short)Math.Round(10.0 * (Level - 1) * Properties.PET_AUTOSET_STR_MULTIPLIER);
				Constitution += (short)Math.Round((Level - 1) * Properties.PET_AUTOSET_CON_MULTIPLIER / 2);
				Quickness += (short)Math.Round((Level - 1) * Properties.PET_AUTOSET_QUI_MULTIPLIER);
				Dexterity += (short)Math.Round((Level - 1) * Properties.PET_AUTOSET_DEX_MULTIPLIER);
				Intelligence += (short)Math.Round((Level - 1) * Properties.PET_AUTOSET_INT_MULTIPLIER);
				Empathy += (short)(Level - 1);
				Piety += (short)(Level - 1);
				Charisma += (short)(Level - 1);
			}
		}
	}
}
