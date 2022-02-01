using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SpellParser.Core.Updater
{
    internal class EffectUpdater : ISpellPropertyUpdater
    {
        // Effect Ids
        public const string SE_Charm = "22";
        public const string SE_Mez = "31";
        public const string SE_Illusion = "58";
        public const string SE_SummonCorpse = "91";
        public const string SE_SpellTrigger = "340";

        // Category Ids (groups)
        public const string SummonCorpse = "52";

        public EffectUpdater(int effectNumber)
        {
            EffectNumber = effectNumber;

            var rof2Spell = new PEQSpell();
            PEQEffectId = rof2Spell.GetType().GetProperty(PEQEffectIdColumnName, BindingFlags.Public | BindingFlags.Instance);
            PEQBaseValue = rof2Spell.GetType().GetProperty(PEQBaseValueColumnName, BindingFlags.Public | BindingFlags.Instance);
            PEQMaxValue = rof2Spell.GetType().GetProperty(PEQMaxValueColumnName, BindingFlags.Public | BindingFlags.Instance);
            PEQForumla = rof2Spell.GetType().GetProperty(PEQForumlaColumnName, BindingFlags.Public | BindingFlags.Instance);



            var eqCasterSpell = new EQCasterSpell();
            EQCasterEffectId = eqCasterSpell.GetType().GetProperty($"Attrib_{EffectNumber}", BindingFlags.Public | BindingFlags.Instance);
            EQCasterBaseValue = eqCasterSpell.GetType().GetProperty($"Min_{EffectNumber}", BindingFlags.Public | BindingFlags.Instance);
            EQCasterMaxValue = eqCasterSpell.GetType().GetProperty($"Max_{EffectNumber}", BindingFlags.Public | BindingFlags.Instance);
            EQCasterForumla = eqCasterSpell.GetType().GetProperty($"Calc_{EffectNumber}", BindingFlags.Public | BindingFlags.Instance);
        }

        private PropertyInfo PEQEffectId { get; }
        private PropertyInfo PEQBaseValue { get; }
        private PropertyInfo PEQMaxValue { get; }
        private PropertyInfo PEQForumla { get; }
        private PropertyInfo EQCasterEffectId { get; }
        private PropertyInfo EQCasterBaseValue { get; }
        private PropertyInfo EQCasterMaxValue { get; }
        private PropertyInfo EQCasterForumla { get; }
        private int EffectNumber { get; }

        private string PEQEffectIdColumnName => $"{nameof(PEQSpell.effectid1).Remove(nameof(PEQSpell.effectid1).Length - 1)}{EffectNumber}";
        private string PEQBaseValueColumnName => $"{nameof(PEQSpell.effect_base_value1).Remove(nameof(PEQSpell.effect_base_value1).Length - 1)}{EffectNumber}";
        private string PEQMaxValueColumnName => $"{nameof(PEQSpell.max1).Remove(nameof(PEQSpell.max1).Length - 1)}{EffectNumber}";
        private string PEQForumlaColumnName => $"{nameof(PEQSpell.formula1).Remove(nameof(PEQSpell.formula1).Length - 1)}{EffectNumber}";

        public IEnumerable<Change> UpdateFrom(PEQSpell peqSpell, EQCasterSpell eqCasterSpell)
        {
            var changes = new List<Change>();
            var peqEffectId = $"{PEQEffectId.GetValue(peqSpell)}";
            var peqBaseValue = $"{PEQBaseValue.GetValue(peqSpell)}";
            var peqMaxValue = $"{PEQMaxValue.GetValue(peqSpell)}";
            var peqForumla = $"{PEQForumla.GetValue(peqSpell)}";


            var eqCasterEffectId = $"{EQCasterEffectId.GetValue(eqCasterSpell)}";
            var eqCasterBaseValue = $"{EQCasterBaseValue.GetValue(eqCasterSpell)}";
            var eqCasterMaxValue = $"{EQCasterMaxValue.GetValue(eqCasterSpell)}";
            var eqCasterForumla = $"{EQCasterForumla.GetValue(eqCasterSpell)}";

            if (peqEffectId == SE_Charm|| peqEffectId == SE_SummonCorpse || peqEffectId == SE_Illusion || peqEffectId == SE_SpellTrigger || peqSpell.spell_category == SummonCorpse) return Array.Empty<Change>();

            string effectId = peqEffectId == SE_Mez ? peqEffectId : AttribConverter(eqCasterEffectId, eqCasterBaseValue);
            if (peqEffectId != effectId && !(effectId == "254" && peqEffectId == "10"))
            {
                changes.Add(new Change { Name = PEQEffectIdColumnName, OldValue = peqEffectId, NewValue = effectId });
            }

            if (eqCasterBaseValue != "" && peqBaseValue != eqCasterBaseValue)
            {
                changes.Add(new Change { Name = PEQBaseValueColumnName, OldValue = peqBaseValue, NewValue = eqCasterBaseValue });
            }

            if (eqCasterMaxValue != "" && peqMaxValue != eqCasterMaxValue)
            {
                changes.Add(new Change { Name = PEQMaxValueColumnName, OldValue = peqMaxValue, NewValue = eqCasterMaxValue });
            }

            string formula = FormulaConverter(eqCasterForumla);
            if (peqForumla != formula)
            {
                changes.Add(new Change { Name = PEQForumlaColumnName, OldValue = peqForumla, NewValue = formula });
            }

            if (changes.Any())
            {
                return changes;
            }

            return Array.Empty<Change>();
        }

        // PEQ base formula types: https://docs.eqemu.io/server/spells/base-value-formulas/
        private string FormulaConverter(string eqCasterFormula)
        {
            switch (eqCasterFormula)
            {
                case "":
                    return "100";

                case "min + level":
                    return "102";

                case "min + level *  2":
                    return "103";

                case "min + level *  3":
                    return "104";

                case "min + level *  4":
                    return "105";

                case "min + level /  2":
                    return "101";

                case "min + level /  3":
                    return "108";

                case "min + level /  4":
                    return "109";

                case "min + level /  5":
                    return "110";

                case "min + level /  8":
                    return "119";

                case "min + ( level - base ) *  6":
                    return "111";

                case "min + ( level - base ) *  8":
                    return "112";

                case "min + ( level - base ) * 10":
                    return "113";

                case "min + ( level - base ) * 12":
                    return "117";

                case "min + ( level - base ) * 15":
                    return "114";

                case "min + ( level - base ) * 20":
                    return "118";

                case "x 1":
                    return "1";

                case "x 2":
                    return "2";

                case "x 4":
                    return "4";

                case "x 5":
                    return "5";

                case "x 6":
                    return "6";

                case "x 8":
                    return "8";

                case "x 10":
                    return "10";

                case "121":
                    return "121";

                case "122":
                    return "122";
                default:
                    throw new Exception($"Unable to parse eqcaster forumla <{eqCasterFormula}>");
            }
        }

        // PEQ effect ids https://docs.eqemu.io/server/spells/spell-effect-ids/?h=effect+id
        private string AttribConverter(string attribute, string baseValue)
        {
            switch (attribute)
            {
                case "":
                    return "254";
                case "Absorb Damage":
                    return "55";
                case "Agility":
                    return "6";
                case "Armor Class (AC)":
                    return "1";
                case "Attack (ATK)":
                    return "2";
                case "Attack Speed":
                    return "11";
                case "Bind Affinity":
                    return "25";
                case "Bind Sight":
                    return "73";
                case "Blinds":
                    return "20";
                case "Cancel Magic":
                    return "27";
                case "Change Weather":
                    return "93";
                case "Charisma":
                    return "10";
                case "Charm":
                    return "22";
                case "Cloak":
                    return "90";
                case "Cold Resist":
                    return "47";
                case "Current Hitpoints":
                    return "79";
                case "Damage Shield":
                    return "59";
                case "Destroy Target":
                    return "41";
                case "Dexterity":
                    return "5";
                case "Disease Resist":
                    return "49";
                case "Disease":
                    return "35";
                case "Divine Aura":
                    return "40";
                case "Evacuate":
                    return "88";
                case "Eye of Zomm":
                    return "87";
                case "Faction":
                    return "19";
                case "Fear":
                    return "23";
                case "Feign Death":
                    return "74";
                case "Fire Resist":
                    return "46";
                case "Frenzy Radius":
                    return "30";
                case "Gate Home":
                    return "26";
                case "Group Gate":
                    return "83";
                case "Hit Points (HP)":
                    return "0";
                case "Identify Item":
                    return "61";
                case "Illusion":
                    return "58";
                case "Infravision":
                    return "65";
                case "Intelligence":
                    return "8";
                case "Invisi/Gate":
                case "Invisibility":
                    return "12";
                case "Invisible to Animals":
                    return "29";
                case "Invisible to Undead":
                    return "28";
                case "Levitate":
                    return "57";
                case "Locate Corpse":
                    return "77";
                case "Lower Aggression":
                    return "18";
                case "Lower Hate":
                    return "92";
                case "Lycanthropy":
                    return "44";
                case "Magic Resist":
                    return "50";
                case "Magnification":
                    return "87";
                case "Mana":
                    return "15";
                case "Mana Drain":
                    return "98";
                case "Movement":
                    return baseValue == "-10000" ? "99" : "3";
                case "Negate if Combat":
                    return "94";
                case "Poison Resist":
                    return "48";
                case "Poison":
                    return "36";
                case "Reaction Radius":
                    return "86";
                case "Reclaim Mana":
                    return "68";
                case "Resurrection":
                    return "81";
                case "See Invisible":
                    return "254";
                case "See Spell Number":
                    return "85";
                case "Sense Animals":
                    return "54";
                case "Sense Summoned":
                    return "53";
                case "Sense Undead":
                    return "52";
                case "Sentinel":
                    return "76";
                case "Shadow Step":
                    return "42";
                case "Shrinkage":
                    return "89";
                case "Spin Target":
                    return "64";
                case "Stamina Loss":
                    return "24";
                case "Stamina":
                    return "7";
                case "Strength":
                    return "4";
                case "Stun":
                    return "21";
                case "Summon Corpse":
                    return "91";
                case "Summon Item":
                    return "32";
                case "Summon Pet":
                    return "33";
                case "Summon Skeleton":
                    return "71";
                case "Throw Into Sky":
                    return "84";
                case "Total Hit Points":
                    return "69";
                case "Total Mana":
                    return "97";
                case "True North":
                    return "56";
                case "Ultravision":
                    return "66";
                case "Voice Graft":
                    return "75";
                case "Water Breathing":
                    return "14";
                case "Wipe Hate List":
                    return "63";
                case "Wisdom":
                    return "9";
                case "Stuns":
                    return "21";

                case "Unknown 4E":
                    return "78";
                default:
                    throw new Exception($"Unable to parse eqcaster attribute <{attribute}>");
            }
        }
    }
}