using SpellParser.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SpellParser.Infrastructure.Data
{
    public class EQCasterSpellRepository
    {
        public IEnumerable<EQCasterSpell> GetAll(IImportOptions options, Expansion expansion)
        {
            var values = File.ReadAllLines(options.EQCasterExportFilePath)
                                       .Skip(1)
                                       .Select(v => Parse(v))
                                       .Where(HasValidSpellName)
                                       .Where(HasValidSkill)
                                       .Where(HasValidAttrib1)
                                       .Where(ExpansionFlag(expansion))
                                       .ToArray();

            return values;
        }

        private static Func<EQCasterSpell, bool> HasValidSpellName = x => x.Spell_Name.EndsWith("Fear2") == false && x.Spell_Name != "Gift of";
        private static Func<EQCasterSpell, bool> HasValidSkill = x => x.Skill != "Instantaneous";
        private static Func<EQCasterSpell, bool> HasValidAttrib1 = x => x.Attrib_1.Contains("Teleport") == false && x.Attrib_1.Contains("Evacuate") == false;

        private static Func<EQCasterSpell, bool> ExpansionFlag(Expansion expansion)
        {
            switch (expansion)
            {
                case Expansion.Original:
                    return x => x.IsMaxLevel(50);

                case Expansion.Kunark:
                case Expansion.Velious:
                    return x => x.IsMaxLevel(60);

                default:
                    throw new Exception($"Un-handled expansions <{expansion}>");
            }
        }

        private static EQCasterSpell Parse(string line)
        {
            var columns = line.Substring(1).Split("\',\'");
            var obj = new EQCasterSpell();

            for (int i = 0; i < columns.Length - 1; i++)
            {
                var columnName = SpellColumns[i];
                var columnValue = columns[i].Replace('`', '\'');
                if (columnName == nameof(EQCasterSpell.Spell_Name))
                {
                    columnValue = FixSpellName(columnValue);
                }

                var prop = obj.GetType().GetProperty(columnName, BindingFlags.Public | BindingFlags.Instance);
                if (null != prop && prop.CanWrite)
                {
                    var val = Convert.ChangeType(columnValue, prop.PropertyType);
                    prop.SetValue(obj, val, null);
                }
            }

            return obj;
        }

        private static string FixSpellName(string spellName)
        {
            if (SpellNames.TryGetValue(spellName, out var correctSpellName))
            {
                return correctSpellName;
            }

            return spellName;
        }

        private static Dictionary<string, string> SpellNames = new Dictionary<string, string>() {
            { "Subjugation", "Boltran's Agacerie" },
            { "Jaxan's Jig o' Vigor", "Jaxan's Jig o` Vigor" },
            { "Illusion: Halfing", "Illusion: Halfling" },
            { "Cessation of the Blood", "Cessation of Cor" },
            { "Manicial Strength", "Maniacal Strength" },
            { "Wind of Tishania", "Wind of Tishanian" },
            { "O'Keils Radiation", "O`Keils Radiation" },
            { "United Resolve", "Heroic Bond" },
            { "Track Cropse", "Track Corpse" },
            { "Cantana of Replenishment", "Cantata of Replenishment" },
            { "Selo's Assonait Strane", "Selo's Assonant Strain" },
        };

        private static string[] SpellColumns
        {
            get
            {
                return new[] {
                    "Spell_Name",
                    "Description_1",
                    "Description_2",
                    "Description_3",
                    "Description_4",
                    "Extra",
                    "Mana_Drain",
                    "WAR",
                    "CLR",
                    "PAL",
                    "RNG",
                    "SHD",
                    "DRU",
                    "MNK",
                    "BRD",
                    "ROG",
                    "SHM",
                    "NEC",
                    "WIZ",
                    "MAG",
                    "ENC",
                    "Casting_Time",
                    "Fizzle_Time",
                    "Recovery_Time",
                    "Indoor_Outdoor",
                    "Location",
                    "Skill",
                    "Duration",
                    "Dur_Formula",
                    "AoE_Duration",
                    "Pos_Neg",
                    "Category",
                    "Attrib_1",
                    "Min_1",
                    "Max_1",
                    "Calc_1",
                    "Attrib_2",
                    "Min_2",
                    "Max_2",
                    "Calc_2",
                    "Attrib_3",
                    "Min_3",
                    "Max_3",
                    "Calc_3",
                    "Attrib_4",
                    "Min_4",
                    "Max_4",
                    "Calc_4",
                    "Reagent_1",
                    "Quantity_1",
                    "Reageant_2",
                    "Quantity_2",
                    "Reageant_3",
                    "Quantity_3",
                    "Reageant_4",
                    "Quantity_4",
                    "Focus_1",
                    "Focus_2",
                    "Resistance",
                    "Target",
                    "Resist_Adj",
                    "Gem",
                    "Range",
                    "AoE_Range",
                    "Back_Range",
                    "Up_Range"
                };
            }
        }
    }
}