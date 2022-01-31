using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SpellParser.Core.Updater
{
    internal class EffectResetUpdater : ISpellPropertyUpdater
    {
        public EffectResetUpdater(int effectNumber)
        {
            EffectNumber = effectNumber;

            var rof2Spell = new PEQSpell();
            PEQEffectId = rof2Spell.GetType().GetProperty($"effectid{EffectNumber}", BindingFlags.Public | BindingFlags.Instance);
            PEQBaseValue = rof2Spell.GetType().GetProperty($"effect_base_value{EffectNumber}", BindingFlags.Public | BindingFlags.Instance);
            PEQMaxValue = rof2Spell.GetType().GetProperty($"max{EffectNumber}", BindingFlags.Public | BindingFlags.Instance);
            PEQForumla = rof2Spell.GetType().GetProperty($"formula{EffectNumber}", BindingFlags.Public | BindingFlags.Instance);
        }

        private PropertyInfo PEQEffectId { get; }
        private PropertyInfo PEQBaseValue { get; }
        private PropertyInfo PEQMaxValue { get; }
        private PropertyInfo PEQForumla { get; }
        private int EffectNumber { get; }

        public IEnumerable<Change> UpdateFrom(PEQSpell rof2Spell, EQCasterSpell eQCaster)
        {
            if (rof2Spell.effectid1 == "91" || rof2Spell.spell_category == "52") Array.Empty<Change>();

            var peqEffectId = $"{PEQEffectId.GetValue(rof2Spell)}";
            var peqBaseValue = $"{PEQBaseValue.GetValue(rof2Spell)}";
            var peqMaxValue = $"{PEQMaxValue.GetValue(rof2Spell)}";
            var peqForumla = $"{PEQForumla.GetValue(rof2Spell)}";

            var changes = new List<Change>();
            if (peqEffectId != "254")
            {
                changes.Add(new Change { Name = $"{nameof(PEQSpell.effectid1).Remove(nameof(PEQSpell.effectid1).Length - 1)}{EffectNumber}", OldValue = peqEffectId, NewValue = "254" });
            }

            if (peqBaseValue != "0")
            {
                changes.Add(new Change { Name = $"{nameof(PEQSpell.effect_base_value2).Remove(nameof(PEQSpell.effect_base_value2).Length - 1)}{EffectNumber}", OldValue = peqBaseValue, NewValue = "0" });
            }

            if (peqMaxValue != "0")
            {
                changes.Add(new Change { Name = $"{nameof(PEQSpell.max2).Remove(nameof(PEQSpell.max2).Length - 1)}{EffectNumber}", OldValue = peqMaxValue, NewValue = "0" });
            }

            if (peqForumla != "100")
            {
                changes.Add(new Change { Name = $"{nameof(PEQSpell.formula2).Remove(nameof(PEQSpell.formula2).Length - 1)}{EffectNumber}", OldValue = peqForumla, NewValue = "100" });
            }

            if (changes.Any())
            {
                return changes;
            }

            return Array.Empty<Change>();
        }
    }
}