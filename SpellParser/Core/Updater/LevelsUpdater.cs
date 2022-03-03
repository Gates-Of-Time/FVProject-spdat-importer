namespace SpellParser.Core.Updater
{
    internal class LevelsUpdater : ISpellPropertyUpdater
    {
        public IEnumerable<Change> UpdateFrom(PEQSpell peqSpell, EQCasterSpell eqCasterSpell)
        {
            var changes = new List<Change>();
            if (peqSpell.BRD != ConvertEqCasterLevel(eqCasterSpell.BRD))
            {
                changes.Add(new Change { Name = nameof(PEQSpell.classes8), OldValue = peqSpell.BRD, NewValue = ConvertEqCasterLevel(eqCasterSpell.BRD) });
            }
            else if (peqSpell.CLR != ConvertEqCasterLevel(eqCasterSpell.CLR))
            {
                changes.Add(new Change { Name = nameof(PEQSpell.classes2), OldValue = peqSpell.CLR, NewValue = ConvertEqCasterLevel(eqCasterSpell.CLR) });
            }
            else if (peqSpell.DRU != ConvertEqCasterLevel(eqCasterSpell.DRU))
            {
                changes.Add(new Change { Name = nameof(PEQSpell.classes6), OldValue = peqSpell.DRU, NewValue = ConvertEqCasterLevel(eqCasterSpell.DRU) });
            }
            else if (peqSpell.ENC != ConvertEqCasterLevel(eqCasterSpell.ENC))
            {
                changes.Add(new Change { Name = nameof(PEQSpell.classes14), OldValue = peqSpell.ENC, NewValue = ConvertEqCasterLevel(eqCasterSpell.ENC) });
            }
            else if (peqSpell.MAG != ConvertEqCasterLevel(eqCasterSpell.MAG))
            {
                changes.Add(new Change { Name = nameof(PEQSpell.classes13), OldValue = peqSpell.MAG, NewValue = ConvertEqCasterLevel(eqCasterSpell.MAG) });
            }
            else if (peqSpell.NEC != ConvertEqCasterLevel(eqCasterSpell.NEC))
            {
                changes.Add(new Change { Name = nameof(PEQSpell.classes11), OldValue = peqSpell.NEC, NewValue = ConvertEqCasterLevel(eqCasterSpell.NEC) });
            }
            else if (peqSpell.PAL != ConvertEqCasterLevel(eqCasterSpell.PAL))
            {
                changes.Add(new Change { Name = nameof(PEQSpell.classes3), OldValue = peqSpell.PAL, NewValue = ConvertEqCasterLevel(eqCasterSpell.PAL) });
            }
            else if (peqSpell.RNG != ConvertEqCasterLevel(eqCasterSpell.RNG))
            {
                changes.Add(new Change { Name = nameof(PEQSpell.classes4), OldValue = peqSpell.RNG, NewValue = ConvertEqCasterLevel(eqCasterSpell.RNG) });
            }
            else if (peqSpell.SHD != ConvertEqCasterLevel(eqCasterSpell.SHD))
            {
                changes.Add(new Change { Name = nameof(PEQSpell.classes5), OldValue = peqSpell.SHD, NewValue = ConvertEqCasterLevel(eqCasterSpell.SHD) });
            }
            else if (peqSpell.SHM != ConvertEqCasterLevel(eqCasterSpell.SHM))
            {
                changes.Add(new Change { Name = nameof(PEQSpell.classes10), OldValue = peqSpell.SHM, NewValue = ConvertEqCasterLevel(eqCasterSpell.SHM) });
            }
            else if (peqSpell.WIZ != ConvertEqCasterLevel(eqCasterSpell.WIZ))
            {
                changes.Add(new Change { Name = nameof(PEQSpell.classes12), OldValue = peqSpell.WIZ, NewValue = ConvertEqCasterLevel(eqCasterSpell.WIZ) });
            }

            if (changes.Any())
            {
                return changes;
            }

            return Array.Empty<Change>();
        }

        private string ConvertEqCasterLevel(string level)
        {
            var intLevel = Convert.ToInt32(level);
            if (intLevel > 60)
            {
                return "255";
            }
            else return level;
        }
    }
}