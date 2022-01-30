using SpellParser.Core;
using SpellParser.Core.Updater;
using System;
using System.Collections.Generic;

namespace SpellParser.Core
{
    public class SpellUpdater
    {
        private SpellUpdater(PEQSpell rof2Spell, IEnumerable<ISpellPropertyUpdater> updaters)
        {
            RoF2Spell = rof2Spell ?? throw new ArgumentNullException(nameof(rof2Spell));
            Updaters = updaters ?? throw new ArgumentNullException(nameof(updaters));
            ChangeTracker = ChangeTracker.From(rof2Spell);
        }

        public static SpellUpdater From(PEQSpell rof2Spell, IEnumerable<ISpellPropertyUpdater> updaters)
        {
            return new SpellUpdater(rof2Spell, updaters);
        }

        private IEnumerable<ISpellPropertyUpdater> Updaters { get; }

        public PEQSpell RoF2Spell { get; }
        public ChangeTracker ChangeTracker { get; }

        public void UpdateFrom(EQCasterSpell eQCaster)
        {
            foreach (var updater in Updaters)
            {
                ChangeTracker.AddChanges(updater.UpdateFrom(RoF2Spell, eQCaster));
            }
        }
    }
}