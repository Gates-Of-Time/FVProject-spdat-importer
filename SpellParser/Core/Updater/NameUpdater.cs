using System;
using System.Collections.Generic;

namespace SpellParser.Core.Updater
{
    public class NameUpdater : ISpellPropertyUpdater
    {
        private static Dictionary<string, string> SpellNameChanges = new Dictionary<string, string>() {
            { "Instill", "Enstill" },
            { "Wondrous Rapidity", "Wonderous Rapidity" },
            { "Wind of Tashanian", "Wind of Tishanian" },
            { "Wind of Tashani", "Wind of Tishani" },
            { "Vocarate: Fire", "Vocerate: Fire" },
            { "Vocarate: Air", "Vocerate: Air" },
            { "Vocarate: Water", "Vocerate: Water" },
            { "Thunderbolt", "Thunderbold" },
            { "Voltaic Draught", "Voltaic Draugh" },
            { "Servant of Bones", "Servent of Bones" },
            { "Vexing Replenishment", "Vexing Mordinia" },
            { "Leech", "Leach" },
            { "Illusion: Dry Bone", "Illusion: Drybone" },
            { "O`Keil's Radiation", "O`Keils Radiation" },
            { "Rapacious Subvention", "Rapacious Subversion" },
            { "Tashina", "Tashan" },
            { "Malaisement", "Malisement" },
            { "Malaise", "Malise" },
        };

        public IEnumerable<Change> UpdateFrom(PEQSpell rof2Spell, EQCasterSpell eQCaster)
        {
            if (SpellNameChanges.TryGetValue(rof2Spell.name, out var correctSpellName))
            {
                var change = new Change() { Name = nameof(PEQSpell.name), OldValue = rof2Spell.name, NewValue = correctSpellName };
                rof2Spell.name = correctSpellName;
                return new[] { change };
            }

            return Array.Empty<Change>();
        }
    }
}