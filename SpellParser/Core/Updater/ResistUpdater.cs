using SpellParser.Core.PEQ;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpellParser.Core.Updater
{
    internal class ResistUpdater : ISpellPropertyUpdater
    {
        const string SathirsMesmerization = "834";
        public IEnumerable<Change> UpdateFrom(PEQSpell peqSpell, EQCasterSpell eqCasterSpell)
        {
            if (peqSpell.goodEffect != "0" || peqSpell.spell_category == Category.MemBlur || peqSpell.id == SathirsMesmerization) return Array.Empty<Change>();

            var changes = new List<Change>();
            string resistance = ResistTypeConverter(eqCasterSpell.Resistance);
            if (resistance != "" && resistance != peqSpell.resisttype)
            {
                changes.Add(new Change { Name = nameof(PEQSpell.resisttype), OldValue = peqSpell.resisttype, NewValue = resistance });
            }

            if (changes.Any())
            {
                return changes;
            }

            return Array.Empty<Change>();
        }

        private string ResistTypeConverter(string eqCasterSpellResistType)
        {
            switch (eqCasterSpellResistType)
            {
                case "":
                    return "";

                case "Cold":
                    return "3";

                case "Disease":
                    return "5";

                case "Fire":
                    return "2";

                case "Magic":
                    return "1";

                case "None":
                    return "0";

                case "Poison":
                    return "4";

                default:
                    throw new Exception($"Unable to parse eqcaster resist type <{eqCasterSpellResistType}>");
            }
        }
    }
}