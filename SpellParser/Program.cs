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
            CheckSpellNames();
            CheckDoubles();
            UpdateSpells();
        }

        private static void UpdateSpells()
        {
            var peqSpellsRepository = new PEQSpellRepository();
            var peqSpells = peqSpellsRepository.GetAll();

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
                , new EffectResetUpdater(5)
                , new EffectResetUpdater(6)
                , new EffectResetUpdater(7)
                , new EffectResetUpdater(8)
            };

            var peqSpellUpdaters = peqSpells.Select(x => SpellUpdater.From(x, updaters)).ToArray();
            var classicSpells = eqcaster.Where(x => x.IsVelious)
                        .Select(x => new
                        {
                            EQCasterSpell = x,
                            PEQSpellUpdater = peqSpellUpdaters.Where(y => x.Spell_Name.ToLower() == y.PEQSpell.name.ToLower()).ToArray()

                        }).ToArray();
            
            foreach (var item in classicSpells.Where(x => x.PEQSpellUpdater.Count() == 1))
            {
                item.PEQSpellUpdater.First().UpdateFrom(item.EQCasterSpell);
            }

            var updates = classicSpells.SelectMany(x => x.PEQSpellUpdater).Where(x => x.ChangeTracker.Changes.Any());
            var updatesCount = updates.Count();
            OutputWriter.WriteToDisk(updates);
        }

        private static void CheckSpellNames()
        {
            var peqSpellsRepository = new PEQSpellRepository();
            var peqSpells = peqSpellsRepository.GetAll();

            var eqCasterSpellsRepository = new EQCasterSpellRepository();
            var eqcaster = eqCasterSpellsRepository.GetAll();

            var updaters = new ISpellPropertyUpdater[] {
                new NameUpdater()
            };

            var peqSpellUpdaters = peqSpells.Select(x => SpellUpdater.From(x, updaters)).ToArray();
            var classicSpells = eqcaster.Where(x => x.IsVelious)
                        .Select(x => new
                        {
                            EQCasterSpell = x,
                            PEQSpellUpdater = peqSpellUpdaters.Where(y => x.Spell_Name.ToLower() == y.PEQSpell.name.ToLower()).ToArray()

                        }).ToArray();

            var errors = eqcaster.Where(x => x.IsClassic).Select(x => x.Spell_Name.ToLower()).Except(peqSpells.Select(x => x.name.ToLower()));
            var errorsCount = errors.Count();
            var names = string.Join("\n", errors);
            Console.WriteLine(names);
        }

        private static void CheckDoubles()
        {
            var peqSpellsRepository = new PEQSpellRepository();
            var peqSpells = peqSpellsRepository.GetAll();

            var eqCasterSpellsRepository = new EQCasterSpellRepository();
            var eqcaster = eqCasterSpellsRepository.GetAll();

            var updaters = new ISpellPropertyUpdater[] {
                new NameUpdater()
            };

            var peqSpellUpdaters = peqSpells.Select(x => SpellUpdater.From(x, updaters)).ToArray();
            var classicSpells = eqcaster.Where(x => x.IsVelious)
                        .Select(x => new
                        {
                            EQCasterSpell = x,
                            PEQSpellUpdater = peqSpellUpdaters.Where(y => x.Spell_Name.ToLower() == y.PEQSpell.name.ToLower()).ToArray()

                        }).ToArray();

            var doubles = classicSpells.Where(x => x.PEQSpellUpdater.Count() > 1);
            var doublesCount = doubles.Count();
            var names = string.Join("\n", doubles.SelectMany(x => x.PEQSpellUpdater.Select(y => $"{y.PEQSpell.id} - {y.PEQSpell.name}")));
            Console.WriteLine(names);
        }
    }
}