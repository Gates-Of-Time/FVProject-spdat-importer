using Microsoft.Extensions.Logging;
using SpellParser.Core;
using SpellParser.Core.Updater;
using SpellParser.Infrastructure.Reporters;

namespace SpellParser.Commmands
{
    public class CheckMissingSpellNamesInPEQCommand : ICommand
    {
        public CheckMissingSpellNamesInPEQCommand(IEnumerable<EQCasterSpell> eqCasterSpells, IEnumerable<PEQSpell> peqSpells, MarkdownReporter spellParserReporter, ILogger logger)
        {
            EqCasterSpells = eqCasterSpells;
            PeqSpells = peqSpells;
            SpellParserReporter = spellParserReporter;
            Logger = logger;
        }

        private IEnumerable<EQCasterSpell> EqCasterSpells { get; }
        private IEnumerable<PEQSpell> PeqSpells { get; }
        private MarkdownReporter SpellParserReporter { get; }
        private ILogger Logger { get; }

        public void Execute()
        {
            Logger.LogInformation("Check Missing Spell Names In PEQ");
            var updaters = new ISpellPropertyUpdater[] {
                new NameUpdater()
            };

            var peqSpellUpdaters = PeqSpells.Select(x => SpellUpdater.From(x, updaters)).ToArray();
            var errors = EqCasterSpells.Select(x => x.Spell_Name.ToLower()).Except(peqSpellUpdaters.Select(x => x.PEQSpell.name.ToLower()));
            var errorsCount = errors.Count();
            Logger.LogInformation($"Missing spell names in PEQ <{errorsCount}>");

            if (errorsCount > 0)
            {
                SpellParserReporter.AppendBulletsSection("Missing spell names in PEQ", errors, x => x);
            }
        }
    }
}