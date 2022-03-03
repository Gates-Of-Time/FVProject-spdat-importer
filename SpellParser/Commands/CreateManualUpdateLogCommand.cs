using Microsoft.Extensions.Logging;
using SpellParser.Core;
using SpellParser.Core.Updater;
using SpellParser.Infrastructure.Reporters;

namespace SpellParser.Commmands
{
    public class CreateManualUpdateLogCommand : ICommand
    {
        public CreateManualUpdateLogCommand(IEnumerable<EQCasterSpell> eqCasterSpells, IEnumerable<PEQSpell> peqSpells, MarkdownReporter spellParserReporter, IImportOptions importOptions, ILogger logger)
        {
            EqCasterSpells = eqCasterSpells;
            PeqSpells = peqSpells;
            SpellParserReporter = spellParserReporter;
            ImportOptions = importOptions;
            Logger = logger;
        }

        private IEnumerable<EQCasterSpell> EqCasterSpells { get; }
        private IEnumerable<PEQSpell> PeqSpells { get; }
        private MarkdownReporter SpellParserReporter { get; }
        private IImportOptions ImportOptions { get; }
        private ILogger Logger { get; }

        private bool SkipUpdateFilter(EQCasterSpell eqCasterSpell)
        {
            return ImportOptions.ExcludateAutomaticUpdatesSpellNames.Contains(eqCasterSpell.Spell_Name) == false && ImportOptions.ExcludeSpellNames.Contains(eqCasterSpell.Spell_Name);
        }

        public void Execute()
        {
            Logger.LogInformation("Create Manual Update Log");
            var updaters = new ISpellPropertyUpdater[] {
                new NameUpdater()
                , new EffectsUpdater(SkipUpdateFilter)
                , new EffectsResetUpdater(SkipUpdateFilter)
            };

            var peqSpellUpdaters = PeqSpells.Where(s => !ImportOptions.ExcludeSpellIds.Contains(s.Id)).Select(x => SpellUpdater.From(x, updaters)).ToArray();
            var updateSpells = EqCasterSpells
                .GroupBy(s => s.Spell_Name)
                .Where(g => g.Count() == 1)
                .Select(g => g.First())
                .Select(x => new
                {
                    EQCasterSpell = x,
                    PEQSpellUpdater = peqSpellUpdaters.Where(y => x.Spell_Name.ToLower() == y.PEQSpell.UpdatedName.ToLower()).OrderBy(u => u.PEQSpell.id).ToArray()
                }).ToArray();

            foreach (var item in updateSpells.Where(x => x.PEQSpellUpdater.Any()))
            {
                item.PEQSpellUpdater.First().UpdateFrom(item.EQCasterSpell);
            }

            var changes = updateSpells.SelectMany(x => x.PEQSpellUpdater).Where(x => x.ChangeTracker.Changes.Any()).Select(c => c.ChangeTracker);
            var updatesCount = changes.Count();
            Logger.LogInformation($"Manuel updates needed <{updatesCount}>");

            if (updatesCount > 0)
            {
                SpellParserReporter.AppendSection("Manual Spell Updates", changes, x => ChangeTrackerReporter.ToMarkdown(x));
            }
        }
    }
}