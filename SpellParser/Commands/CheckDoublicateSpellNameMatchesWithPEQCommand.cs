using Microsoft.Extensions.Logging;
using SpellParser.Core;
using SpellParser.Core.Updater;
using SpellParser.Infrastructure.Reporters;
using System.Collections.Generic;
using System.Linq;

namespace SpellParser.Commmands
{
    public class CheckDoublicateSpellNameMatchesWithPEQCommand : ICommand
    {
        public CheckDoublicateSpellNameMatchesWithPEQCommand(IEnumerable<EQCasterSpell> eqCasterSpells, IEnumerable<PEQSpell> peqSpells, SpellParserReporter spellParserReporter, ILogger logger)
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
            Logger.LogInformation("Check Doublicate SpellName Matcher With PEQ");
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
                SpellParserReporter.WriteSectionWithBullets("EQCaster spells have multiple hits in PEQ name mapping", doubles, x => $"{x.EQCasterSpell.Spell_Name} [ {string.Join(", ", x.PEQSpellUpdater.Select(y => y.PEQSpell.id))} ]");
            }
        }
    }
}