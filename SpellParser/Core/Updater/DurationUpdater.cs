using System;
using System.Collections.Generic;
using System.Linq;

namespace SpellParser.Core.Updater
{
    internal class DurationUpdater : ISpellPropertyUpdater
    {
        public IEnumerable<Change> UpdateFrom(PEQSpell rof2Spell, EQCasterSpell eQCaster)
        {

            var changes = new List<Change>();
            string duration = eQCaster.Duration;
            if (duration != "" && rof2Spell.buffduration != duration)
            {
                changes.Add(new Change { Name = nameof(PEQSpell.buffduration), OldValue = rof2Spell.buffduration, NewValue = duration });
            }

            string durationFormula = eQCaster.Dur_Formula;
            if (durationFormula != "" && rof2Spell.buffdurationformula != durationFormula)
            {
                changes.Add(new Change { Name = nameof(PEQSpell.buffdurationformula), OldValue = rof2Spell.buffdurationformula, NewValue = durationFormula });
            }

            if (changes.Any())
            {
                return changes;
            }

            return Array.Empty<Change>();
        }
    }
}