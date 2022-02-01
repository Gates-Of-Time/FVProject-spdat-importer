using Microsoft.Extensions.Configuration;
using SpellParser.Core;
using SpellParser.Core.Updater;
using SpellParser.Infrastructure.Data;
using SpellParser.Infrastructure.Reporters;
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

            // https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration
            IConfiguration config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();


            var settings = config.GetRequiredSection("Settings").Get<Settings>();

            using var reporter = new SpellParserReporter(settings.Export);

            var eqCasterSpellsRepository = new EQCasterSpellRepository();
            var eqCasterSpells = eqCasterSpellsRepository.GetAll(settings.Import, settings.Expansion);

            var peqSpellsRepository = new PEQSpellRepository();
            var peqSpells = peqSpellsRepository.GetAll(settings.Import);

            CheckDuplicateSPDATSpells(eqCasterSpells, reporter);
            CheckMissingSpellNamesInPEQ(eqCasterSpells, peqSpells, reporter);
            CreateAutomaticUpdateScript(eqCasterSpells, peqSpells, settings.Export, reporter);
            CheckDoublicateSpellNamesWithChangesInPEQ(eqCasterSpells, peqSpells, reporter);
            CreateManualUpdateLog(eqCasterSpells, peqSpells, reporter);
        }

        private static void CheckDuplicateSPDATSpells(IEnumerable<EQCasterSpell> eqCasterSpells, SpellParserReporter spellParserReporter)
        {
            Console.WriteLine("\nCheckDuplicateSPDATSpells"); 
            var duplicates = eqCasterSpells.GroupBy(s => s.Spell_Name)
                           .Select(group => new
                           {
                               Name = group.Key,
                               Count = group.Count()
                           })
                           .Where(g => g.Count > 1);

            int duplicateCount = duplicates.Count();
            string duplicateNames = $"+  {string.Join("\n+ ", duplicates.Select(g => g.Name))}";
            Console.WriteLine($"Duplicate SPDAT spells <{duplicateCount}>");
            Console.WriteLine(duplicateNames);

            if (duplicateCount > 0) {
                spellParserReporter.Write($@"## Duplicate SPDAT check
Number of duplicates: {duplicateCount}
Duplicates:
{duplicateNames}

");
            }
        }

        private static void CreateAutomaticUpdateScript(IEnumerable<EQCasterSpell> eqCasterSpells, IEnumerable<PEQSpell> peqSpells, IExportOptions options, SpellParserReporter spellParserReporter)
        {
            Console.WriteLine("\nCreateAutomaticUpdateScript");
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
            SQLReporter.WriteToDisk(options, changes.Select(u => u.ChangeTracker));
            Console.WriteLine($"Updated spells <{updatesCount}>");

            if (updatesCount > 0)
            {
                spellParserReporter.Write($@"## Created SQL update scripts
Number of updates: {updatesCount}

");
            }
        }

        private static void CreateManualUpdateLog(IEnumerable<EQCasterSpell> eqCasterSpells, IEnumerable<PEQSpell> peqSpells, SpellParserReporter spellParserReporter)
        {
            Console.WriteLine("\nCreateManualUpdateLog");
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

            var changes = updateSpells.SelectMany(x => x.PEQSpellUpdater).Where(x => x.ChangeTracker.Changes.Any()).Select(c => c.ChangeTracker);
            var updatesCount = changes.Count();
            spellParserReporter.Write(changes);
        }

        private static void CheckMissingSpellNamesInPEQ(IEnumerable<EQCasterSpell> eqCasterSpells, IEnumerable<PEQSpell> peqSpells, SpellParserReporter spellParserReporter)
        {
            Console.WriteLine("\nCheckMissingSpellNamesInPEQ");
            var updaters = new ISpellPropertyUpdater[] {
                new NameUpdater()
            };

            var peqSpellUpdaters = peqSpells.Select(x => SpellUpdater.From(x, updaters)).ToArray();
            var errors = eqCasterSpells.Select(x => x.Spell_Name.ToLower()).Except(peqSpellUpdaters.Select(x => x.PEQSpell.name.ToLower()));
            var errorsCount = errors.Count();
            var names = $"+ {string.Join("\n+ ", errors)}";

            if (errorsCount < 1) return;

            Console.WriteLine($"Missing spell names in PEQ <{errorsCount}>");
            Console.WriteLine(names);

            if (errorsCount > 0)
            {
                spellParserReporter.Write($@"## Missing spell names in PEQ
Number of missing spells: {errorsCount}
Missing spells:
{names}

");
            }
        }

        private static void CheckDoublicateSpellNamesWithChangesInPEQ(IEnumerable<EQCasterSpell> eqCasterSpells, IEnumerable<PEQSpell> peqSpells, SpellParserReporter spellParserReporter)
        {
            Console.WriteLine("\nCheckDoublicateSpellNamesInPEQ");
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
            var names = $"+ {string.Join("\n+ ", doubles.Select(x => $"{x.EQCasterSpell.Spell_Name} [ {string.Join(", ", x.PEQSpellUpdater.Select(y => y.PEQSpell.id))} ]"))}";

            if (doublesCount < 1) return;

            Console.WriteLine($"Multiple PEQ spell name mappings <{doublesCount}>");
            Console.WriteLine(names);

            if (doublesCount > 0)
            {
                spellParserReporter.Write($@"## EQCaster spells have multiple hits in PEQ name mapping
Number of spells with doubles: {doublesCount}
Doubles:
{names}

");
            }
        }
    }
}