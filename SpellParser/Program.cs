using SpellParser.Core;
using SpellParser.Core.Updater;
using SpellParser.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpellParser
{
    internal partial class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello Everquest!");
            //CheckSpellNames();
            UpdateSpells();
        }

        private static void UpdateSpells()
        {
            var rof2SpellsRepository = new PEQSpellRepository();
            var rof2Spells = rof2SpellsRepository.GetAll();

            var eqCasterSpellsRepository = new EQCasterSpellRepository();
            var eqcaster = eqCasterSpellsRepository.GetAll();

            var updaters = new ISpellPropertyUpdater[] { 
                new NameUpdater()
                , new CastTimersUpdater()
                , new DurationUpdater()
                , new EffectUpdater(1)
                , new EffectUpdater(2)
                , new EffectUpdater(3)
                , new EffectUpdater(4)
                , new EffectResettUpdater(5)
                , new EffectResettUpdater(6)
                , new EffectResettUpdater(7)
                , new EffectResettUpdater(8)
            };

            var rof2SpellUpdaters = rof2Spells.Select(x => SpellUpdater.From(x, updaters)).ToArray();
            var classicSpells = eqcaster.Where(x => x.IsClassic)
                        .Select(x => new
                        {
                            EQCasterSpell = x,
                            RoF2SpellUpdater = rof2SpellUpdaters.Where(y => x.Spell_Name.ToLower() == y.RoF2Spell.name.ToLower()).ToArray()

                        }).ToArray();
            
            foreach (var item in classicSpells.Where(x => x.RoF2SpellUpdater.Count() == 1))
            {
                item.RoF2SpellUpdater.First().UpdateFrom(item.EQCasterSpell);
            }

            var updates = classicSpells.SelectMany(x => x.RoF2SpellUpdater).Where(x => x.ChangeTracker.Changes.Any());
            var updatesCount = updates.Count();
            OutputWriter.WriteToDisk(updates);
        }

        private static void CheckSpellNames()
        {
            var rof2SpellsRepository = new PEQSpellRepository();
            var rof2Spells = rof2SpellsRepository.GetAll();

            var eqCasterSpellsRepository = new EQCasterSpellRepository();
            var eqcaster = eqCasterSpellsRepository.GetAll();

            var updaters = new ISpellPropertyUpdater[] {
                new NameUpdater()
            };

            var rof2SpellUpdaters = rof2Spells.Select(x => SpellUpdater.From(x, updaters)).ToArray();
            var classicSpells = eqcaster.Where(x => x.IsClassic)
                        .Select(x => new
                        {
                            EQCasterSpell = x,
                            RoF2SpellUpdater = rof2SpellUpdaters.Where(y => x.Spell_Name.ToLower() == y.RoF2Spell.name.ToLower()).ToArray()

                        }).ToArray();

            var errors = eqcaster.Where(x => x.IsClassic).Select(x => x.Spell_Name.ToLower()).Except(rof2Spells.Select(x => x.name.ToLower()));
            var errorsCount = errors.Count();
            var names = string.Join("\n", errors);
        }

        private static void CheckDoubles()
        {
            var rof2SpellsRepository = new PEQSpellRepository();
            var rof2Spells = rof2SpellsRepository.GetAll();

            var eqCasterSpellsRepository = new EQCasterSpellRepository();
            var eqcaster = eqCasterSpellsRepository.GetAll();

            var updaters = new ISpellPropertyUpdater[] {
                new NameUpdater()
            };

            var rof2SpellUpdaters = rof2Spells.Select(x => SpellUpdater.From(x, updaters)).ToArray();
            var classicSpells = eqcaster.Where(x => x.IsClassic)
                        .Select(x => new
                        {
                            EQCasterSpell = x,
                            RoF2SpellUpdater = rof2SpellUpdaters.Where(y => x.Spell_Name.ToLower() == y.RoF2Spell.name.ToLower()).ToArray()

                        }).ToArray();

            var doubles = classicSpells.Where(x => x.RoF2SpellUpdater.Count() > 1);
            var doublesCount = doubles.Count();
            var names = string.Join("\n", doubles.SelectMany(x => x.RoF2SpellUpdater.Select(y => $"{y.RoF2Spell.id} - {y.RoF2Spell.name}")));
        }
    }
}