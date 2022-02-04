using System.Collections.Generic;

namespace SpellParser.Core.Updater
{
    internal class EffectsResetUpdater : ISpellPropertyUpdater
    {
        public IEnumerable<Change> UpdateFrom(PEQSpell peqSpell, EQCasterSpell eqCasterSpell)
        {
            var changes = new List<Change>();
            var effectChange = new EffectResetUpdater();
            for (int i = eqCasterSpell.SpellEffects.Length - 1; i < peqSpell.SpellEffects.Length; i++)
            {
                changes.AddRange(effectChange.UpdateFrom(i, peqSpell, peqSpell.SpellEffects[i], new SpellEffect()));
            }

            return changes;
        }
    }
}