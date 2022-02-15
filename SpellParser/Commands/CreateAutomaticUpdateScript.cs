using Microsoft.Extensions.Logging;
using SpellParser.Core;
using SpellParser.Core.Updater;
using SpellParser.Infrastructure.Reporters;

namespace SpellParser.Commmands
{
    public class CreateAutomaticUpdateScript : ICommand
    {
        public CreateAutomaticUpdateScript(IEnumerable<EQCasterSpell> eqCasterSpells, IEnumerable<PEQSpell> peqSpells, MarkdownReporter spellParserReporter, IExportOptions exportOptions, IImportOptions importOptions, ILogger logger)
        {
            EqCasterSpells = eqCasterSpells;
            PeqSpells = peqSpells;
            SpellParserReporter = spellParserReporter;
            ExportOptions = exportOptions;
            ImportOptions = importOptions;
            Logger = logger;
        }

        private IEnumerable<EQCasterSpell> EqCasterSpells { get; }
        private IEnumerable<PEQSpell> PeqSpells { get; }
        private MarkdownReporter SpellParserReporter { get; }
        private IExportOptions ExportOptions { get; }
        private IImportOptions ImportOptions { get; }
        private ILogger Logger { get; }

        private bool SkipUpdateFilter(EQCasterSpell eqCasterSpell) {
            return ImportOptions.ExcludateAutomaticUpdatesSpellNames.Union(ImportOptions.ExcludeSpellNames).Contains(eqCasterSpell.Spell_Name);
        }

        public void Execute()
        {
            Logger.LogInformation("Create Automatic Update Script");
            var updaters = new ISpellPropertyUpdater[] {
                new NameUpdater()
                , new CastTimersUpdater()
                , new DurationUpdater()
                , new ManaUpdater()
                , new ResistUpdater()
                , new RangeUpdater()
                , new EffectsUpdater(SkipUpdateFilter)
                , new EffectsResetUpdater(SkipUpdateFilter)
            };

            var peqSpellUpdaters = PeqSpells.Select(x => SpellUpdater.From(x, updaters)).ToArray();
            var updateSpells = EqCasterSpells
                .GroupBy(s => s.Spell_Name)
                .Where(g => g.Count() == 1)
                .Select(g => g.First())
                .Select(x => new
                    {
                        EQCasterSpell = x,
                        PEQSpellUpdater = peqSpellUpdaters.Where(y => x.Spell_Name.ToLower() == y.PEQSpell.name.ToLower()).OrderBy(u => u.PEQSpell.id).ToArray()
                    }).ToArray();

            foreach (var item in updateSpells.Where(x => x.PEQSpellUpdater.Any()))
            {
                item.PEQSpellUpdater.First().UpdateFrom(item.EQCasterSpell);
            }

            var changes = updateSpells.SelectMany(x => x.PEQSpellUpdater).Where(x => x.ChangeTracker.Changes.Any());
            var updatesCount = changes.Count();
            Logger.LogInformation($"Updated spells <{updatesCount}>");

            if (updatesCount > 0)
            {
                SQLReporter.WriteToDisk(ExportOptions, changes.Select(u => u.ChangeTracker));
                SpellParserReporter.AppendSection("Automatic SQL update scripts created", changes);
            }
        }
    }
}