namespace SpellParser.Core.Updater
{
    internal class EffectsUpdater : ISpellPropertyUpdater
    {
        public EffectsUpdater(Func<EQCasterSpell, bool> skipUpdateFilter)
        {
            SkipUpdate = skipUpdateFilter;
        }

        public Func<EQCasterSpell, bool> SkipUpdate { get; }

        public IEnumerable<Change> UpdateFrom(PEQSpell peqSpell, EQCasterSpell eqCasterSpell)
        {
            if (SkipUpdate(eqCasterSpell)) { 
                return Array.Empty<Change>();
            }

            var changes = new List<Change>();
            var effectChange = new EffectUpdater();
            for (int i = 0; i < eqCasterSpell.SpellEffects.Length; i++)
            {
                changes.AddRange(effectChange.UpdateFrom(i + 1, peqSpell, peqSpell.SpellEffects[i], eqCasterSpell.SpellEffects[i]));
            }

            return changes;
        }
    }
}