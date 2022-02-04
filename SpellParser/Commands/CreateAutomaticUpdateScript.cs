using Microsoft.Extensions.Logging;
using SpellParser.Core;
using SpellParser.Core.Updater;
using SpellParser.Infrastructure.Reporters;
using System.Collections.Generic;
using System.Linq;

namespace SpellParser.Commmands
{
    public class CreateAutomaticUpdateScript : ICommand
    {
        public CreateAutomaticUpdateScript(IEnumerable<EQCasterSpell> eqCasterSpells, IEnumerable<PEQSpell> peqSpells, SpellParserReporter spellParserReporter, IExportOptions options, ILogger logger)
        {
            EqCasterSpells = eqCasterSpells;
            PeqSpells = peqSpells;
            SpellParserReporter = spellParserReporter;
            Options = options;
            Logger = logger;
        }

        private IEnumerable<EQCasterSpell> EqCasterSpells { get; }
        private IEnumerable<PEQSpell> PeqSpells { get; }
        private SpellParserReporter SpellParserReporter { get; }
        private IExportOptions Options { get; }
        private ILogger Logger { get; }

        public void Execute()
        {
            Logger.LogInformation("Create Automatic Update Script");
            var updaters = new ISpellPropertyUpdater[] {
                new NameUpdater()
                , new CastTimersUpdater()
                , new DurationUpdater()
                , new ManaUpdater()
                , new ResistUpdater()
            };

            var peqSpellUpdaters = PeqSpells.Select(x => SpellUpdater.From(x, updaters)).ToArray();
            var updateSpells = EqCasterSpells.Select(x => new
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
            Logger.LogInformation($"Updated spells <{updatesCount}>");

            if (updatesCount > 0)
            {
                SQLReporter.WriteToDisk(Options, changes.Select(u => u.ChangeTracker));
                SpellParserReporter.WriteSection("Automatic SQL update scripts created", changes);
            }
        }
    }
}