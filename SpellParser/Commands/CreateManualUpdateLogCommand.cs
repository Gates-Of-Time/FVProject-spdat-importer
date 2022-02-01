using Microsoft.Extensions.Logging;
using SpellParser.Core;
using SpellParser.Core.Updater;
using SpellParser.Infrastructure.Reporters;
using System.Collections.Generic;
using System.Linq;

namespace SpellParser.Commmands
{
    public class CreateManualUpdateLogCommand : ICommand
    {
        public CreateManualUpdateLogCommand(IEnumerable<EQCasterSpell> eqCasterSpells, IEnumerable<PEQSpell> peqSpells, SpellParserReporter spellParserReporter, ILogger logger)
        {
            EqCasterSpells = eqCasterSpells;
            PeqSpells = peqSpells;
            SpellParserReporter = spellParserReporter;
            Logger = logger;
        }

        private IEnumerable<EQCasterSpell> EqCasterSpells { get; }
        private IEnumerable<PEQSpell> PeqSpells { get; }
        private SpellParserReporter SpellParserReporter { get; }
        private ILogger Logger { get; }

        public void Execute()
        {
            Logger.LogInformation("Create Manual Update Log");
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

            var changes = updateSpells.SelectMany(x => x.PEQSpellUpdater).Where(x => x.ChangeTracker.Changes.Any()).Select(c => c.ChangeTracker);
            var updatesCount = changes.Count();
            Logger.LogInformation($"Manuel updates needed <{updatesCount}>");

            if (updatesCount > 0)
            {
                SpellParserReporter.WriteSectionWithoutBullets("Manual Spell Updates", changes, x => ChangeTrackerReporter.ToMarkdown(x));
            }
        }
    }
}