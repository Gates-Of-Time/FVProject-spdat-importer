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

            var eqCasterSpellsRepository = new EQCasterSpellRepository();
            var eqCasterSpells = eqCasterSpellsRepository.GetClassicSpells();

            CheckSpellNames(eqCasterSpells);
            CheckDoubles(eqCasterSpells);
            UpdateSpells(eqCasterSpells);
        }

        private static void UpdateSpells(IEnumerable<EQCasterSpell> eqCasterSpells)
        {
            var peqSpellsRepository = new PEQSpellRepository();
            var peqSpells = peqSpellsRepository.GetAll();

            var updaters = new ISpellPropertyUpdater[] { 
                new NameUpdater()
                , new CastTimersUpdater()
                , new DurationUpdater()
                , new EffectUpdater(1)
                , new EffectUpdater(2)
                , new EffectUpdater(3)
                , new EffectUpdater(4)
                , new EffectResetUpdater(5)
                , new EffectResetUpdater(6)
                , new EffectResetUpdater(7)
                , new EffectResetUpdater(8)
            };

            var peqSpellUpdaters = peqSpells.Select(x => SpellUpdater.From(x, updaters)).ToArray();
            var updateSpells = eqCasterSpells.Select(x => new
                        {
                            EQCasterSpell = x,
                            PEQSpellUpdater = peqSpellUpdaters.Where(y => x.Spell_Name.ToLower() == y.PEQSpell.name.ToLower()).ToArray()

                        }).ToArray();
            
            foreach (var item in updateSpells.Where(x => x.PEQSpellUpdater.Count() == 1))
            {
                item.PEQSpellUpdater.First().UpdateFrom(item.EQCasterSpell);
            }

            var changes = updateSpells.SelectMany(x => x.PEQSpellUpdater).Where(x => x.ChangeTracker.Changes.Any());
            var updatesCount = changes.Count();
            OutputWriter.WriteToDisk(changes);
        }

        private static void CheckSpellNames(IEnumerable<EQCasterSpell> eqCasterSpells)
        {
            var peqSpellsRepository = new PEQSpellRepository();
            var peqSpells = peqSpellsRepository.GetAll();

            var updaters = new ISpellPropertyUpdater[] {
                new NameUpdater()
            };

            var peqSpellUpdaters = peqSpells.Select(x => SpellUpdater.From(x, updaters)).ToArray();
            var updateSpells = eqCasterSpells.Select(x => new
                        {
                            EQCasterSpell = x,
                            PEQSpellUpdater = peqSpellUpdaters.Where(y => x.Spell_Name.ToLower() == y.PEQSpell.name.ToLower()).ToArray()

                        }).ToArray();

            var errors = eqCasterSpells.Where(x => x.IsClassic).Select(x => x.Spell_Name.ToLower()).Except(peqSpells.Select(x => x.name.ToLower()));
            var errorsCount = errors.Count();
            var names = string.Join("\n", errors);
            Console.WriteLine(names);
        }

        private static void CheckDoubles(IEnumerable<EQCasterSpell> eqCasterSpells)
        {
            var peqSpellsRepository = new PEQSpellRepository();
            var peqSpells = peqSpellsRepository.GetAll();

            var updaters = new ISpellPropertyUpdater[] {
                new NameUpdater()
            };

            var peqSpellUpdaters = peqSpells.Select(x => SpellUpdater.From(x, updaters)).ToArray();
            var updateSpells = eqCasterSpells.Select(x => new
                        {
                            EQCasterSpell = x,
                            PEQSpellUpdater = peqSpellUpdaters.Where(y => x.Spell_Name.ToLower() == y.PEQSpell.name.ToLower()).ToArray()

                        }).ToArray();

            var doubles = updateSpells.Where(x => x.PEQSpellUpdater.Count() > 1);
            var doublesCount = doubles.Count();
            var names = string.Join("\n", doubles.SelectMany(x => x.PEQSpellUpdater.Select(y => $"{y.PEQSpell.id} - {y.PEQSpell.name}")));
            Console.WriteLine(names);
        }
    }
}