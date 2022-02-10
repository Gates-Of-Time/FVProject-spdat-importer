namespace SpellParser.Core.Updater
{
    public interface ISpellPropertyUpdater
    {
        IEnumerable<Change> UpdateFrom(PEQSpell peqSpell, EQCasterSpell eqCasterSpell);
    }
}