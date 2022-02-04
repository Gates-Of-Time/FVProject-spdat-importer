using System;
using System.Collections.Generic;
using System.Linq;

namespace SpellParser.Core.Updater
{
    public class RangeUpdater : ISpellPropertyUpdater
    {
        public IEnumerable<Change> UpdateFrom(PEQSpell peqSpell, EQCasterSpell eqCasterSpell)
        {
            var changes = new List<Change>();
            var range = eqCasterSpell.Range;
            if (range != "" && range != peqSpell.range)
            {
                changes.Add(new Change { Name = nameof(PEQSpell.range), OldValue = peqSpell.range, NewValue = range });
            }

            var aoeRange = eqCasterSpell.AoE_Range;
            if (aoeRange != "" && aoeRange != peqSpell.aoerange)
            {
                changes.Add(new Change { Name = nameof(PEQSpell.aoerange), OldValue = peqSpell.aoerange, NewValue = aoeRange });
            }

            if (changes.Any())
            {
                return changes;
            }

            return Array.Empty<Change>();
        }
    }
}