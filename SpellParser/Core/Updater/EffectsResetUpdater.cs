namespace SpellParser.Core.Updater
{
    internal class EffectsResetUpdater : ISpellPropertyUpdater
    {
        public EffectsResetUpdater(Func<EQCasterSpell, bool> skipUpdateFilter)
        {
            SkipUpdate = skipUpdateFilter;
        }

        public Func<EQCasterSpell, bool> SkipUpdate { get; }

        public IEnumerable<Change> UpdateFrom(PEQSpell peqSpell, EQCasterSpell eqCasterSpell)
        {
            if (SkipUpdate(eqCasterSpell))
            {
                return Array.Empty<Change>();
            }
            var changes = new List<Change>();
            var effectChange = new EffectResetUpdater();
            for (int i = eqCasterSpell.SpellEffects.Length; i < peqSpell.SpellEffects.Length; i++)
            {
                changes.AddRange(effectChange.UpdateFrom(i + 1, peqSpell, peqSpell.SpellEffects[i], new SpellEffect()));
            }

            return changes;
        }
    }
}