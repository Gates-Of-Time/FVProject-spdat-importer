using SpellParser.Core.PEQ;

namespace SpellParser.Core.Updater
{
    internal class EffectResetUpdater
    {
        private static string[] SkipPEQEffectIds = new[] {
            EffectType.SE_Charm
            , EffectType.SE_SummonCorpse
            , EffectType.SE_Illusion
            , EffectType.SE_SpellTrigger
            , EffectType.SE_StackingCommand_Block
        };

        public IEnumerable<Change> UpdateFrom(int effectNumber, PEQSpell peqSpell, SpellEffect peqSpellEffect, SpellEffect eqCasterSpellEffect)
        {
            var peqEffectId = peqSpellEffect.EffectId;
            var peqBaseValue = peqSpellEffect.BaseValue;
            var peqMaxValue = peqSpellEffect.MaxValue;
            var peqForumla = peqSpellEffect.Formula;

            if (SkipPEQEffectIds.Contains(peqEffectId) || peqSpell.spell_category == Category.SummonCorpse) return Array.Empty<Change>();

            var changes = new List<Change>();
            if (peqEffectId != "254")
            {
                changes.Add(new Change { Name = PEQEffectIdColumnName(effectNumber), OldValue = peqEffectId, NewValue = "254" });
            }

            if (peqBaseValue != "0")
            {
                changes.Add(new Change { Name = PEQBaseValueColumnName(effectNumber), OldValue = peqBaseValue, NewValue = "0" });
            }

            if (peqMaxValue != "0")
            {
                changes.Add(new Change { Name = PEQMaxValueColumnName(effectNumber), OldValue = peqMaxValue, NewValue = "0" });
            }

            if (peqForumla != "100")
            {
                changes.Add(new Change { Name = PEQForumlaColumnName(effectNumber), OldValue = peqForumla, NewValue = "100" });
            }

            if (changes.Any())
            {
                return changes;
            }

            return Array.Empty<Change>();
        }

        private string PEQEffectIdColumnName(int EffectNumber) => $"{nameof(PEQSpell.effectid1).Remove(nameof(PEQSpell.effectid1).Length - 1)}{EffectNumber}";
        private string PEQBaseValueColumnName(int EffectNumber) => $"{nameof(PEQSpell.effect_base_value1).Remove(nameof(PEQSpell.effect_base_value1).Length - 1)}{EffectNumber}";
        private string PEQMaxValueColumnName(int EffectNumber) => $"{nameof(PEQSpell.max1).Remove(nameof(PEQSpell.max1).Length - 1)}{EffectNumber}";
        private string PEQForumlaColumnName(int EffectNumber) => $"{nameof(PEQSpell.formula1).Remove(nameof(PEQSpell.formula1).Length - 1)}{EffectNumber}";
    }
}