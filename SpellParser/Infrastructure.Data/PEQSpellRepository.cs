﻿using SpellParser.Core;
using System.Reflection;

namespace SpellParser.Infrastructure.Data
{
    public class PEQSpellRepository
    {
        public IEnumerable<PEQSpell> GetAll(IImportOptions options, Expansion expansion)
        {
            var values = File.ReadAllLines(options.SpellsUSFilePath)
                                        .Select(v => Parse(v))
                                        .Where(s => string.IsNullOrWhiteSpace(s.name) == false)
                                        .Where(IsInExpansion(expansion))
                                        .ToArray();
            return values;
        }

        private static Func<PEQSpell, bool> IsInExpansion(Expansion expansion)
        {
            switch (expansion)
            {
                case Expansion.Original:
                    return x => x.Id < 1830;

                case Expansion.Kunark:
                    return x => x.Id < 2100;

                case Expansion.Velious:
                    return x => x.Id < 2100;

                default:
                    throw new Exception($"Un-handled expansions <{expansion}>");
            }
        }

        private PEQSpell Parse(string line)
        {
            var columns = line.Split('^');
            var obj = new PEQSpell();

            for (int i = 0; i < columns.Length; i++)
            {
                var columnName = SpellColumns[i];
                var columnValue = columns[i];
                var prop = obj.GetType().GetProperty(columnName, BindingFlags.Public | BindingFlags.Instance);
                if (null != prop && prop.CanWrite)
                {
                    var val = Convert.ChangeType(columnValue, prop.PropertyType);
                    prop.SetValue(obj, val, null);
                }
            }

            return obj;
        }

        private static string[] SpellColumns
        {
            get
            {
                return new[] {
                    "id",
                    "name",
                    "player_1",
                    "teleport_zone",
                    "you_cast",
                    "other_casts",
                    "cast_on_you",
                    "cast_on_other",
                    "spell_fades",
                    "range",
                    "aoerange",
                    "pushback",
                    "pushup",
                    "cast_time",
                    "recovery_time",
                    "recast_time",
                    "buffdurationformula",
                    "buffduration",
                    "AEDuration",
                    "mana",
                    "effect_base_value1",
                    "effect_base_value2",
                    "effect_base_value3",
                    "effect_base_value4",
                    "effect_base_value5",
                    "effect_base_value6",
                    "effect_base_value7",
                    "effect_base_value8",
                    "effect_base_value9",
                    "effect_base_value10",
                    "effect_base_value11",
                    "effect_base_value12",
                    "effect_limit_value1",
                    "effect_limit_value2",
                    "effect_limit_value3",
                    "effect_limit_value4",
                    "effect_limit_value5",
                    "effect_limit_value6",
                    "effect_limit_value7",
                    "effect_limit_value8",
                    "effect_limit_value9",
                    "effect_limit_value10",
                    "effect_limit_value11",
                    "effect_limit_value12",
                    "max1",
                    "max2",
                    "max3",
                    "max4",
                    "max5",
                    "max6",
                    "max7",
                    "max8",
                    "max9",
                    "max10",
                    "max11",
                    "max12",
                    "icon",
                    "memicon",
                    "components1",
                    "components2",
                    "components3",
                    "components4",
                    "component_counts1",
                    "component_counts2",
                    "component_counts3",
                    "component_counts4",
                    "NoexpendReagent1",
                    "NoexpendReagent2",
                    "NoexpendReagent3",
                    "NoexpendReagent4",
                    "formula1",
                    "formula2",
                    "formula3",
                    "formula4",
                    "formula5",
                    "formula6",
                    "formula7",
                    "formula8",
                    "formula9",
                    "formula10",
                    "formula11",
                    "formula12",
                    "LightType",
                    "goodEffect",
                    "Activated",
                    "resisttype",
                    "effectid1",
                    "effectid2",
                    "effectid3",
                    "effectid4",
                    "effectid5",
                    "effectid6",
                    "effectid7",
                    "effectid8",
                    "effectid9",
                    "effectid10",
                    "effectid11",
                    "effectid12",
                    "targettype",
                    "basediff",
                    "skill",
                    "zonetype",
                    "EnvironmentType",
                    "TimeOfDay",
                    "classes1",
                    "classes2",
                    "classes3",
                    "classes4",
                    "classes5",
                    "classes6",
                    "classes7",
                    "classes8",
                    "classes9",
                    "classes10",
                    "classes11",
                    "classes12",
                    "classes13",
                    "classes14",
                    "classes15",
                    "classes16",
                    "CastingAnim",
                    "TargetAnim",
                    "TravelType",
                    "SpellAffectIndex",
                    "disallow_sit",
                    "deities0",
                    "deities1",
                    "deities2",
                    "deities3",
                    "deities4",
                    "deities5",
                    "deities6",
                    "deities7",
                    "deities8",
                    "deities9",
                    "deities10",
                    "deities11",
                    "deities12",
                    "deities13",
                    "deities14",
                    "deities15",
                    "deities16",
                    "field142",
                    "field143",
                    "new_icon",
                    "spellanim",
                    "uninterruptable",
                    "ResistDiff",
                    "dot_stacking_exempt",
                    "deleteable",
                    "RecourseLink",
                    "no_partial_resist",
                    "field152",
                    "field153",
                    "short_buff_box",
                    "descnum",
                    "typedescnum",
                    "effectdescnum",
                    "effectdescnum2",
                    "npc_no_los",
                    "field160",
                    "reflectable",
                    "bonushate",
                    "field163",
                    "field164",
                    "ldon_trap",
                    "EndurCost",
                    "EndurTimerIndex",
                    "IsDiscipline",
                    "field169",
                    "field170",
                    "field171",
                    "field172",
                    "HateAdded",
                    "EndurUpkeep",
                    "numhitstype",
                    "numhits",
                    "pvpresistbase",
                    "pvpresistcalc",
                    "pvpresistcap",
                    "spell_category",
                    "field181",
                    "field182",
                    "pcnpc_only_flag",
                    "cast_not_standing",
                    "can_mgb",
                    "nodispell",
                    "npc_category",
                    "npc_usefulness",
                    "MinResist",
                    "MaxResist",
                    "viral_targets",
                    "viral_timer",
                    "nimbuseffect",
                    "ConeStartAngle",
                    "ConeStopAngle",
                    "sneaking",
                    "not_extendable",
                    "field198",
                    "field199",
                    "suspendable",
                    "viral_range",
                    "songcap",
                    "field203",
                    "field204",
                    "no_block",
                    "field206",
                    "spellgroup",
                    "rank",
                    "field209",
                    "field210",
                    "CastRestriction",
                    "allowrest",
                    "InCombat",
                    "OutofCombat",
                    "field215",
                    "field216",
                    "field217",
                    "aemaxtargets",
                    "maxtargets",
                    "field220",
                    "field221",
                    "field222",
                    "field223",
                    "persistdeath",
                    "field225",
                    "field226",
                    "min_dist",
                    "min_dist_mod",
                    "max_dist",
                    "max_dist_mod",
                    "min_range",
                    "field232",
                    "field233",
                    "field234",
                    "field235",
                    "field236"
                };
            }
        }
    }
}