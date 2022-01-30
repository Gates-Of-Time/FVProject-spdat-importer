using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SpellParser.Core.Updater
{
    internal class CastTimersUpdater : ISpellPropertyUpdater
    {
        public IEnumerable<Change> UpdateFrom(PEQSpell rof2Spell, EQCasterSpell eQCaster)
        { 
            // skip bard songs
            if (rof2Spell.skill == "12" || rof2Spell.skill == "41" || rof2Spell.skill == "49" || rof2Spell.skill == "54" || rof2Spell.skill == "70")
            {
                return Array.Empty<Change>();
            }

            var changes = new List<Change>();
            string castingTime = TimeConvertion(eQCaster.Casting_Time);
            if (castingTime != "0" && rof2Spell.cast_time != castingTime)
            {
                changes.Add(new Change { Name = nameof(PEQSpell.cast_time), OldValue = rof2Spell.cast_time, NewValue = castingTime });
            }

            string recastTime = TimeConvertion(eQCaster.Recovery_Time);
            if (recastTime != "0" && rof2Spell.recast_time != recastTime)
            {
                changes.Add(new Change { Name = nameof(PEQSpell.recast_time), OldValue = rof2Spell.recast_time, NewValue = recastTime });
            }

            string recoveryTime = TimeConvertion(eQCaster.Fizzle_Time);
            if (recoveryTime != "0" && rof2Spell.recovery_time != recoveryTime)
            {
                changes.Add(new Change { Name = nameof(PEQSpell.recovery_time), OldValue = rof2Spell.recovery_time, NewValue = recoveryTime });
            }

            if (changes.Any()) {
                return changes;
            }

            return Array.Empty<Change>();
        }

        private string TimeConvertion(string value)
        {
            return $"{Convert.ToInt32(Convert.ToDecimal(value, new CultureInfo("en-US")) * 1000)}";
        }
    }
}