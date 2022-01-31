using System;
using System.Collections.Generic;

namespace SpellParser.Core
{
    public class EQCasterSpell
    {
        public string Spell_Name { get; set; }
        public string Description_1 { get; set; }
        public string Description_2 { get; set; }
        public string Description_3 { get; set; }
        public string Description_4 { get; set; }
        public string Extra { get; set; }
        public string Mana_Drain { get; set; }
        public string WAR { get; set; }
        public string CLR { get; set; }
        public string PAL { get; set; }
        public string RNG { get; set; }
        public string SHD { get; set; }
        public string DRU { get; set; }
        public string MNK { get; set; }
        public string BRD { get; set; }
        public string ROG { get; set; }
        public string SHM { get; set; }
        public string NEC { get; set; }
        public string WIZ { get; set; }
        public string MAG { get; set; }
        public string ENC { get; set; }
        public string Casting_Time { get; set; }
        public string Fizzle_Time { get; set; }
        public string Recovery_Time { get; set; }
        public string Indoor_Outdoor { get; set; }
        public string Location { get; set; }
        public string Skill { get; set; }
        public string Duration { get; set; }
        public string Dur_Formula { get; set; }
        public string AoE_Duration { get; set; }
        public string Pos_Neg { get; set; }
        public string Category { get; set; }
        public string Attrib_1 { get; set; }
        public string Min_1 { get; set; }
        public string Max_1 { get; set; }
        public string Calc_1 { get; set; }
        public string Attrib_2 { get; set; }
        public string Min_2 { get; set; }
        public string Max_2 { get; set; }
        public string Calc_2 { get; set; }
        public string Attrib_3 { get; set; }
        public string Min_3 { get; set; }
        public string Max_3 { get; set; }
        public string Calc_3 { get; set; }
        public string Attrib_4 { get; set; }
        public string Min_4 { get; set; }
        public string Max_4 { get; set; }
        public string Calc_4 { get; set; }
        public string Reagent_1 { get; set; }
        public string Quantity_1 { get; set; }
        public string Reageant_2 { get; set; }
        public string Quantity_2 { get; set; }
        public string Reageant_3 { get; set; }
        public string Quantity_3 { get; set; }
        public string Reageant_4 { get; set; }
        public string Quantity_4 { get; set; }
        public string Focus_1 { get; set; }
        public string Focus_2 { get; set; }
        public string Resistance { get; set; }
        public string Target { get; set; }
        public string Resist_Adj { get; set; }
        public string Gem { get; set; }
        public string Range { get; set; }
        public string AoE_Range { get; set; }
        public string Back_Range { get; set; }
        public string Up_Range { get; set; }

        public bool IsPCSpell => HasValidSkill
            && HasValidSpellName
            && IsMaxLevel(60)
            ;

        public bool IsClassic => HasValidSkill
            && HasValidSpellName
            && IsMaxLevel(50)
            ;

        public bool IsKunark => HasValidSkill
            && HasValidSpellName
            && IsMaxLevel(60)
            ;

        public bool IsVelious => HasValidSkill
            && HasValidSpellName
            && IsMaxLevel(60)
            ;

        private bool HasValidSpellName => Spell_Name.EndsWith("Fear2") == false
            && Spell_Name != "Gift of";

        private bool HasValidSkill => Skill != "Instantaneous";

        private bool IsMaxLevel(int minLevel)
        {
            return IsMaxLevel(WAR, minLevel)
                || IsMaxLevel(CLR, minLevel)
                || IsMaxLevel(PAL, minLevel)
                || IsMaxLevel(RNG, minLevel)
                || IsMaxLevel(SHD, minLevel)
                || IsMaxLevel(DRU, minLevel)
                || IsMaxLevel(MNK, minLevel)
                || IsMaxLevel(BRD, minLevel)
                || IsMaxLevel(ROG, minLevel)
                || IsMaxLevel(SHM, minLevel)
                || IsMaxLevel(NEC, minLevel)
                || IsMaxLevel(WIZ, minLevel)
                || IsMaxLevel(MAG, minLevel)
                || IsMaxLevel(ENC, minLevel);
        }

        private bool IsMaxLevel(string classLevel, int minLevel)
        {
            return string.IsNullOrWhiteSpace(classLevel) == false && Convert.ToInt32(classLevel) <= minLevel;
        }
    }
}