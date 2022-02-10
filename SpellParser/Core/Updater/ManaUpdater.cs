namespace SpellParser.Core.Updater
{
    public class ManaUpdater : ISpellPropertyUpdater
    {
        public IEnumerable<Change> UpdateFrom(PEQSpell rof2Spell, EQCasterSpell eqCasterSpell)
        {
            if (eqCasterSpell.Mana_Drain != "" && rof2Spell.mana != eqCasterSpell.Mana_Drain)
            {
                return new[] { new Change { Name = nameof(PEQSpell.mana), OldValue = rof2Spell.mana, NewValue = eqCasterSpell.Mana_Drain } };
            }

            return Array.Empty<Change>();
        }
    }
}