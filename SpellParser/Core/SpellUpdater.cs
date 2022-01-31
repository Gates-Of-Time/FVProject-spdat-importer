using SpellParser.Core;
using SpellParser.Core.Updater;
using System;
using System.Collections.Generic;

namespace SpellParser.Core
{
    public class SpellUpdater
    {
        private SpellUpdater(PEQSpell peqSpell, IEnumerable<ISpellPropertyUpdater> updaters)
        {
            PEQSpell = peqSpell ?? throw new ArgumentNullException(nameof(peqSpell));
            Updaters = updaters ?? throw new ArgumentNullException(nameof(updaters));
            ChangeTracker = ChangeTracker.From(peqSpell);
        }

        public static SpellUpdater From(PEQSpell rof2Spell, IEnumerable<ISpellPropertyUpdater> updaters)
        {
            return new SpellUpdater(rof2Spell, updaters);
        }

        private IEnumerable<ISpellPropertyUpdater> Updaters { get; }

        public PEQSpell PEQSpell { get; }
        public ChangeTracker ChangeTracker { get; }

        public void UpdateFrom(EQCasterSpell eQCaster)
        {
            foreach (var updater in Updaters)
            {
                ChangeTracker.AddChanges(updater.UpdateFrom(PEQSpell, eQCaster));
            }
        }
    }
}