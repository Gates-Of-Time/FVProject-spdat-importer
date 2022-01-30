using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SpellParser.Core.Updater
{
    internal class EffectUpdater : ISpellPropertyUpdater
    {
        public EffectUpdater(int effectNumber)
        {
            EffectNumber = effectNumber;

            var rof2Spell = new PEQSpell();
            PEQEffectId = rof2Spell.GetType().GetProperty($"effectid{EffectNumber}", BindingFlags.Public | BindingFlags.Instance);
            PEQBaseValue = rof2Spell.GetType().GetProperty($"effect_base_value{EffectNumber}", BindingFlags.Public | BindingFlags.Instance);
            PEQMaxValue = rof2Spell.GetType().GetProperty($"max{EffectNumber}", BindingFlags.Public | BindingFlags.Instance);
            PEQForumla = rof2Spell.GetType().GetProperty($"formula{EffectNumber}", BindingFlags.Public | BindingFlags.Instance);



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

        public IEnumerable<Change> UpdateFrom(PEQSpell rof2Spell, EQCasterSpell eQCaster)
        {
            if (rof2Spell.effectid1 == "91" || rof2Spell.spell_category == "52") Array.Empty<Change>();

            var changes = new List<Change>();
            var peqEffectId = $"{PEQEffectId.GetValue(rof2Spell)}";
            var peqBaseValue = $"{PEQBaseValue.GetValue(rof2Spell)}";
            var peqMaxValue = $"{PEQMaxValue.GetValue(rof2Spell)}";
            var peqForumla = $"{PEQForumla.GetValue(rof2Spell)}";


            var eqCasterEffectId = $"{EQCasterEffectId.GetValue(eQCaster)}";
            var eqCasterBaseValue = $"{EQCasterBaseValue.GetValue(eQCaster)}";
            var eqCasterMaxValue = $"{EQCasterMaxValue.GetValue(eQCaster)}";
            var eqCasterForumla = $"{EQCasterForumla.GetValue(eQCaster)}";


            string effectId = AttribConverter(eqCasterEffectId);
            if (peqEffectId != effectId)
            {
                changes.Add(new Change { Name = $"{nameof(PEQSpell.effectid1).Remove(nameof(PEQSpell.effectid1).Length - 1)}{EffectNumber}", OldValue = peqEffectId, NewValue = effectId });
            }

            if (eqCasterBaseValue != "" && peqBaseValue != eqCasterBaseValue)
            {
                changes.Add(new Change { Name = $"{nameof(PEQSpell.effect_base_value2).Remove(nameof(PEQSpell.effect_base_value2).Length - 1)}{EffectNumber}", OldValue = peqBaseValue, NewValue = eqCasterBaseValue });
            }

            if (eqCasterMaxValue != "" && peqMaxValue != eqCasterMaxValue)
            {
                changes.Add(new Change { Name = $"{nameof(PEQSpell.max2).Remove(nameof(PEQSpell.max2).Length - 1)}{EffectNumber}", OldValue = peqMaxValue, NewValue = eqCasterMaxValue });
            }

            string formula = FormulaConverter(eqCasterForumla);
            if (peqForumla != formula)
            {
                changes.Add(new Change { Name = $"{nameof(PEQSpell.formula2).Remove(nameof(PEQSpell.formula2).Length - 1)}{EffectNumber}", OldValue = peqForumla, NewValue = formula });
            }

            if (changes.Any())
            {
                return changes;
            }

            return Array.Empty<Change>();
        }

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
                default:
                    throw new Exception($"Unable to parse eqcaster forumla <{eqCasterFormula}>");
            }
        }

        private string AttribConverter(string attribute)
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
                    return "16";
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
                case "Lycanthropy":
                    return "44";
                case "Magic Resist":
                    return "50";
                case "Magnification":
                    return "87";
                case "Mana":
                    return "15";
                case "Movement":
                    return "3";
                case "Negate if Combat":
                    return "";
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