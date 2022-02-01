using SpellParser.Core.Updater;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpellParser.Core
{
    public class SpellUpdater
    {
        private SpellUpdater(PEQSpell peqSpell, IEnumerable<ISpellPropertyUpdater> updaters)
        {
            PEQSpell = peqSpell ?? throw new ArgumentNullException(nameof(peqSpell));
            Updaters = updaters ?? throw new ArgumentNullException(nameof(updaters));
            ChangeTracker = ChangeTracker.From(peqSpell);

            ChangeTracker.AddChanges(updaters.FirstOrDefault(u => u is NameUpdater)?.UpdateFrom(PEQSpell, new EQCasterSpell()) ?? Array.Empty<Change>());
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
                if (updater is NameUpdater) continue;

                ChangeTracker.AddChanges(updater.UpdateFrom(PEQSpell, eQCaster));
            }
        }
    }
}