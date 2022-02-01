﻿using Microsoft.Extensions.Configuration;
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
            Console.WriteLine("");

            var eqCasterSpellsRepository = new EQCasterSpellRepository();
            var eqCasterSpells = eqCasterSpellsRepository.GetClassicSpells();

            CheckDuplicateSPDATSpells(eqCasterSpells);
            //CheckSpellNames(eqCasterSpells);
            //CreateAutomaticUpdateScript(eqCasterSpells);
            CheckIds(eqCasterSpells);
            CheckDoubles(eqCasterSpells);
        }

        private static void CheckDuplicateSPDATSpells(IEnumerable<EQCasterSpell> eqCasterSpells)
        {
            var duplicates = eqCasterSpells.GroupBy(s => s.Spell_Name)
                           .Select(group => new
                           {
                               Name = group.Key,
                               Count = group.Count()
                           })
                           .Where(g => g.Count > 1);
            Console.WriteLine($"Duplicate SPDAT spells <{duplicates.Count()}>");
            Console.WriteLine(string.Join("\n", duplicates.Select(g => g.Name)));
            Console.WriteLine("");
        }

        private static void CreateAutomaticUpdateScript(IEnumerable<EQCasterSpell> eqCasterSpells)
        {
            var peqSpellsRepository = new PEQSpellRepository();
            var peqSpells = peqSpellsRepository.GetAll();

            var updaters = new ISpellPropertyUpdater[] { 
                new NameUpdater()
                , new CastTimersUpdater()
                , new DurationUpdater()
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
        private static void CreateManualUpdateLog(IEnumerable<EQCasterSpell> eqCasterSpells)
        {
            var peqSpellsRepository = new PEQSpellRepository();
            var peqSpells = peqSpellsRepository.GetAll();

            var updaters = new ISpellPropertyUpdater[] {
                new NameUpdater()
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

            var errors = eqCasterSpells.Select(x => x.Spell_Name.ToLower()).Except(peqSpells.Select(x => x.name.ToLower()));
            var errorsCount = errors.Count();
            var names = string.Join("\n", errors);

            if (errorsCount < 1) return;

            Console.WriteLine($"Missing spell names in PEQ <{errorsCount}>");
            Console.WriteLine(names);
            Console.WriteLine("");
        }

        private static void CheckDoubles(IEnumerable<EQCasterSpell> eqCasterSpells)
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

            foreach (var item in updateSpells)
            {
                foreach (var updater in item.PEQSpellUpdater)
                {
                    updater.UpdateFrom(item.EQCasterSpell);
                }
            }

            var doubles = updateSpells.Where(x => x.PEQSpellUpdater.Count() > 1 && x.PEQSpellUpdater.Any(u => u.ChangeTracker.Changes.Any()));
            var doublesCount = doubles.Count();
            var names = string.Join("\n", doubles.Select(x => $"{x.EQCasterSpell.Spell_Name} [ {string.Join(", ", x.PEQSpellUpdater.Select(y => y.PEQSpell.id))} ]"));

            if (doublesCount < 1) return;

            Console.WriteLine($"Multiple PEQ spell name mappings <{doublesCount}>");
            Console.WriteLine(names);
            Console.WriteLine("");
        }

        private static void CheckIds(IEnumerable<EQCasterSpell> eqCasterSpells) {
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

            var doubles = updateSpells.Where(x => x.PEQSpellUpdater.Count() == 0);
            var doublesCount = doubles.Count();
            var names = string.Join("\n", doubles.Select(x => $"{x.EQCasterSpell.Spell_Name} [ {string.Join(", ", x.PEQSpellUpdater.Select(y => y.PEQSpell.id))} ]"));


            Console.WriteLine($"Missing number of mappings <{doublesCount}>");
            Console.WriteLine(names);
            Console.WriteLine("");
        }
    }
}