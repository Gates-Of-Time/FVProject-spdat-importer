namespace SpellParser.Core.Updater
{
    internal class DurationUpdater : ISpellPropertyUpdater
    {
        public IEnumerable<Change> UpdateFrom(PEQSpell rof2Spell, EQCasterSpell eqCasterSpell)
        {
            // skip bard songs
            if (rof2Spell.skill == "12" || rof2Spell.skill == "41" || rof2Spell.skill == "49" || rof2Spell.skill == "54" || rof2Spell.skill == "70")
            {
                return Array.Empty<Change>();
            }

            var changes = new List<Change>();

            string durationFormula = eqCasterSpell.Dur_Formula;
            if (durationFormula != "" && rof2Spell.buffdurationformula != durationFormula)
            {
                changes.Add(new Change { Name = nameof(PEQSpell.buffdurationformula), OldValue = rof2Spell.buffdurationformula, NewValue = durationFormula });
            }

            string duration = eqCasterSpell.Duration;
            if ((duration != "0" || changes.Any()) && duration != "" && rof2Spell.buffduration != duration)
            {
                changes.Add(new Change { Name = nameof(PEQSpell.buffduration), OldValue = rof2Spell.buffduration, NewValue = duration });
            }

            if (changes.Any())
            {
                return changes;
            }

            return Array.Empty<Change>();
        }
    }
}