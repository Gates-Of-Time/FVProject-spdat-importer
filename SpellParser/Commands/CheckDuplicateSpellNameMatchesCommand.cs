using Microsoft.Extensions.Logging;
using SpellParser.Core;
using SpellParser.Core.Updater;
using SpellParser.Infrastructure.Reporters;

namespace SpellParser.Commmands
{
    public class CheckDuplicateSpellNameMatchesCommand : ICommand
    {
        public CheckDuplicateSpellNameMatchesCommand(IEnumerable<EQCasterSpell> eqCasterSpells, IEnumerable<PEQSpell> peqSpells, MarkdownReporter spellParserReporter, IImportOptions importOptions, ILogger logger)
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
            return ImportOptions.ExcludeSpellNames.Contains(eqCasterSpell.Spell_Name);
        }

        public void Execute()
        {
            Logger.LogInformation("Check Doublicate SpellName Matcher With PEQ");
            var updaters = new ISpellPropertyUpdater[] {
                new NameUpdater()
                , new CastTimersUpdater()
                , new DurationUpdater()
                , new ManaUpdater()
                , new ResistUpdater()
                , new EffectsUpdater(SkipUpdateFilter)
                , new EffectsResetUpdater(SkipUpdateFilter)
            };

            var peqSpellUpdaters = PeqSpells.Select(x => SpellUpdater.From(x, updaters)).ToArray();
            var updateSpells = EqCasterSpells.Select(x => new
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

            Logger.LogInformation($"Multiple PEQ spell name mappings <{doublesCount}>");

            if (doublesCount > 0)
            {
                SpellParserReporter.AppendBulletsSection("EQCaster spells have multiple hits in PEQ name mapping", doubles, x => $"{x.EQCasterSpell.Spell_Name} [ {string.Join(", ", x.PEQSpellUpdater.Select(y => y.PEQSpell.id))} ]");
            }
        }
    }
}