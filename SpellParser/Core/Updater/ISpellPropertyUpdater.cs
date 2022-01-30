using System.Collections.Generic;

namespace SpellParser.Core.Updater
{
    public interface ISpellPropertyUpdater
    {
        IEnumerable<Change> UpdateFrom(PEQSpell rof2Spell, EQCasterSpell eQCaster);
    }
}