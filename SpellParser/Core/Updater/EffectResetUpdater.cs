using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SpellParser.Core.Updater
{
    internal class EffectResetUpdater : ISpellPropertyUpdater
    {
        // Effect Ids
        public const string SE_Mez = "31";
        public const string SE_SummonCorpse = "91";
        public const string SE_StackingCommand_Block = "148";

        // Category Ids (groups)
        public const string SummonCorpse = "52";

        public EffectResetUpdater(int effectNumber)
        {
            EffectNumber = effectNumber;

            var rof2Spell = new PEQSpell();
            PEQEffectId = rof2Spell.GetType().GetProperty(PEQEffectIdColumnName, BindingFlags.Public | BindingFlags.Instance);
            PEQBaseValue = rof2Spell.GetType().GetProperty(PEQBaseValueColumnName, BindingFlags.Public | BindingFlags.Instance);
            PEQMaxValue = rof2Spell.GetType().GetProperty(PEQMaxValueColumnName, BindingFlags.Public | BindingFlags.Instance);
            PEQForumla = rof2Spell.GetType().GetProperty(PEQForumlaColumnName, BindingFlags.Public | BindingFlags.Instance);
        }

        private PropertyInfo PEQEffectId { get; }
        private PropertyInfo PEQBaseValue { get; }
        private PropertyInfo PEQMaxValue { get; }
        private PropertyInfo PEQForumla { get; }
        private int EffectNumber { get; }

        private string PEQEffectIdColumnName => $"{nameof(PEQSpell.effectid1).Remove(nameof(PEQSpell.effectid1).Length - 1)}{EffectNumber}";
        private string PEQBaseValueColumnName => $"{nameof(PEQSpell.effect_base_value1).Remove(nameof(PEQSpell.effect_base_value1).Length - 1)}{EffectNumber}";
        private string PEQMaxValueColumnName => $"{nameof(PEQSpell.max1).Remove(nameof(PEQSpell.max1).Length - 1)}{EffectNumber}";
        private string PEQForumlaColumnName => $"{nameof(PEQSpell.formula1).Remove(nameof(PEQSpell.formula1).Length - 1)}{EffectNumber}";

        public IEnumerable<Change> UpdateFrom(PEQSpell peqSpell, EQCasterSpell eQCaster)
        {
            var peqEffectId = $"{PEQEffectId.GetValue(peqSpell)}";
            var peqBaseValue = $"{PEQBaseValue.GetValue(peqSpell)}";
            var peqMaxValue = $"{PEQMaxValue.GetValue(peqSpell)}";
            var peqForumla = $"{PEQForumla.GetValue(peqSpell)}";

            if (peqEffectId == SE_SummonCorpse || peqEffectId == SE_StackingCommand_Block || peqSpell.spell_category == SummonCorpse) Array.Empty<Change>();

            var changes = new List<Change>();
            if (peqEffectId != "254")
            {
                changes.Add(new Change { Name = PEQEffectIdColumnName, OldValue = peqEffectId, NewValue = "254" });
            }

            if (peqBaseValue != "0")
            {
                changes.Add(new Change { Name = PEQBaseValueColumnName, OldValue = peqBaseValue, NewValue = "0" });
            }

            if (peqMaxValue != "0")
            {
                changes.Add(new Change { Name = PEQMaxValueColumnName, OldValue = peqMaxValue, NewValue = "0" });
            }

            if (peqForumla != "100")
            {
                changes.Add(new Change { Name = PEQForumlaColumnName, OldValue = peqForumla, NewValue = "100" });
            }

            if (changes.Any())
            {
                return changes;
            }

            return Array.Empty<Change>();
        }
    }
}