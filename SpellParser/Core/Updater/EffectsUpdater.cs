using System.Collections.Generic;

namespace SpellParser.Core.Updater
{
    internal class EffectsUpdater : ISpellPropertyUpdater
    {
        public IEnumerable<Change> UpdateFrom(PEQSpell rof2Spell, EQCasterSpell eqCasterSpell)
        {
            var changes = new List<Change>();

            var effectChange = new EffectUpdater();
            for (int i = 0; i < eqCasterSpell.SpellEffects.Length; i++)
            {
                changes.AddRange(effectChange.UpdateFrom(i, rof2Spell, rof2Spell.SpellEffects[i], eqCasterSpell.SpellEffects[i]));
            }

            return changes;
        }
    }
}