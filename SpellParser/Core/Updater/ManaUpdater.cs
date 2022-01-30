using System;
using System.Collections.Generic;

namespace SpellParser.Core.Updater
{
    public class ManaUpdater : ISpellPropertyUpdater
    {
        public IEnumerable<Change> UpdateFrom(PEQSpell rof2Spell, EQCasterSpell eQCaster)
        {
            if (eQCaster.Mana_Drain != "" && rof2Spell.mana != eQCaster.Mana_Drain)
            {
                return new[] { new Change { Name = nameof(PEQSpell.mana), OldValue = rof2Spell.mana, NewValue = eQCaster.Mana_Drain } };
            }

            return Array.Empty<Change>();
        }
    }
}