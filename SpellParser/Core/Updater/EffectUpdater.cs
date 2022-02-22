using SpellParser.Core.PEQ;

namespace SpellParser.Core.Updater
{
    internal class EffectUpdater
    {
        private static string[] SkipPEQCategories = new[] {
            Category.SummonCorpse
            , Category.MemBlur
            , Category.Gate
            , Category.Translocate
        };

        private static string[] SkipPEQEffectIds = new[] {
            EffectType.SE_Charm
            , EffectType.SE_SummonCorpse
            , EffectType.SE_Illusion
            , EffectType.SE_SpellTrigger
            , EffectType.SE_Hate
            , EffectType.SE_Mez
            , EffectType.SE_Invisibility2
            , EffectType.SE_InvisVsUndead2
            , EffectType.SE_PoisonCounter
        };

        private static string[] SkipSPDATEffectIds = new[] {
            EffectType.SE_Stamina
        };

        public IEnumerable<Change> UpdateFrom(int effectNumber, PEQSpell peqSpell, SpellEffect peqSpellEffect, SpellEffect eqCasterSpellEffect)
        {
            var changes = new List<Change>();
            var peqEffectId = peqSpellEffect.EffectId;
            var peqBaseValue = peqSpellEffect.BaseValue;
            var peqMaxValue = peqSpellEffect.MaxValue;
            var peqForumla = peqSpellEffect.Formula;

            var eqCasterEffectId = eqCasterSpellEffect.EffectId;
            var eqCasterBaseValue = eqCasterSpellEffect.BaseValue;
            var eqCasterMaxValue = eqCasterSpellEffect.MaxValue != "" ? $"{Math.Abs(Convert.ToInt32(eqCasterSpellEffect.MaxValue))}" : "";
            var eqCasterForumla = eqCasterSpellEffect.Formula;


            string effectId = peqEffectId == EffectType.SE_Mez ? peqEffectId : AttribConverter(eqCasterEffectId, eqCasterBaseValue);
            if (SkipSPDATEffectIds.Contains(effectId) || SkipPEQEffectIds.Contains(peqEffectId) || SkipPEQCategories.Contains(peqSpell.spell_category)) return Array.Empty<Change>();

            if (peqEffectId != effectId && !(effectId == EffectType.SE_Blank && peqEffectId == EffectType.SE_CHA) && !(peqEffectId == EffectType.SE_DiseaseCounter && peqSpell.spell_category == Category.SlowSingle))
            {
                changes.Add(new Change { Name = PEQEffectIdColumnName(effectNumber), OldValue = peqEffectId, NewValue = effectId });
            }

            if ( eqCasterEffectId != "" && eqCasterBaseValue != "" && peqBaseValue != eqCasterBaseValue)
            {
                changes.Add(new Change { Name = PEQBaseValueColumnName(effectNumber), OldValue = peqBaseValue, NewValue = eqCasterBaseValue });
            }

            if (effectId != EffectType.SE_Stun && effectId != EffectType.SE_Fear && effectId != EffectType.SE_ChangeFrenzyRad && effectId != EffectType.SE_Harmony && !(peqEffectId == EffectType.SE_AttackSpeed && peqSpell.spell_category == Category.SlowSingle) && eqCasterEffectId != "" && eqCasterMaxValue != "" && peqMaxValue != eqCasterMaxValue && !(HasSameBaseAndMaxValueForHPEffect(effectId, peqBaseValue, eqCasterMaxValue)))
            {
                changes.Add(new Change { Name = PEQMaxValueColumnName(effectNumber), OldValue = peqMaxValue, NewValue = eqCasterMaxValue });
            }

            string formula = FormulaConverter(eqCasterForumla);
            if (eqCasterEffectId != "" && peqForumla != formula)
            {
                changes.Add(new Change { Name = PEQForumlaColumnName(effectNumber), OldValue = peqForumla, NewValue = formula });
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

        private bool HasSameBaseAndMaxValueForHPEffect(string effectId, string peqBaseValue, string eqCasterMaxValue)
        {
            return (effectId == "0" && Math.Abs(Convert.ToInt32(peqBaseValue)) == Math.Abs(Convert.ToInt32(eqCasterMaxValue)));
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

                case "120":
                    return "120";

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

                case "Sacrifice":
                    return "95";

                case "See Invisible":
                    return "13";

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

                case "Summon Player":
                    return "82";

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