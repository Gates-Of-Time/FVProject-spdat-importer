using Microsoft.Extensions.Logging;
using SpellParser.Core;
using SpellParser.Infrastructure.Reporters;
using System.Collections.Generic;
using System.Linq;

namespace SpellParser.Commmands
{
    public class CheckDuplicateSPDATSpellsCommand : ICommand
    {
        public CheckDuplicateSPDATSpellsCommand(IEnumerable<EQCasterSpell> eqCasterSpells, MarkdownReporter spellParserReporter, ILogger logger)
        {
            EqCasterSpells = eqCasterSpells;
            SpellParserReporter = spellParserReporter;
            Logger = logger;
        }

        private IEnumerable<EQCasterSpell> EqCasterSpells { get; }
        private MarkdownReporter SpellParserReporter { get; }
        private ILogger Logger { get; }

        public void Execute()
        {
            Logger.LogInformation("Check Duplicate SPDAT Spells");
            var duplicates = EqCasterSpells.GroupBy(s => s.Spell_Name)
                           .Select(group => new
                           {
                               Name = group.Key,
                               Count = group.Count()
                           })
                           .Where(g => g.Count > 1);

            int duplicateCount = duplicates.Count();
            Logger.LogInformation($"Duplicate SPDAT spells <{duplicateCount}>");

            if (duplicateCount > 0)
            {
                SpellParserReporter.AppendBulletsSection("Duplicate SPDAT check", duplicates, x => x.Name);
            }
        }
    }
}