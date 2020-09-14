using RimWorld;
using UnityEngine;
using Verse;
using System;
using System.Collections.Generic;

namespace AnimalGenetics
{
    public static class Constants
    {
        // order here dictates order displayed in game.
        public static List<StatDef> affectedStats = new List<StatDef>()
        {
            AnimalGenetics.Health,
            AnimalGenetics.Damage,
            StatDefOf.MoveSpeed,
            StatDefOf.CarryingCapacity,
            StatDefOf.MeatAmount,
            StatDefOf.LeatherAmount,
            AnimalGenetics.GatherYield
        };

        public static List<StatDef> affectedStatsToInsert = new List<StatDef>()
        {
            StatDefOf.MoveSpeed,
            StatDefOf.LeatherAmount,
            StatDefOf.MeatAmount,
            StatDefOf.CarryingCapacity
        };

        public static string GetLabel(StatDef stat)
        {
            if (!_labelOverrides.ContainsKey(stat))
                return stat.label;
            return _labelOverrides[stat];
        }

        static Dictionary<StatDef, String> _labelOverrides = new Dictionary<StatDef, String>()
        {
            { StatDefOf.MoveSpeed, "Speed".Translate()},
            { AnimalGenetics.Health, "Health".Translate() },
            { AnimalGenetics.Damage, "Damage".Translate() },
            { StatDefOf.CarryingCapacity, "Capacity".Translate() },
            { StatDefOf.MeatAmount, "Meat".Translate() },
            { StatDefOf.LeatherAmount, "Leather".Translate()},
            { AnimalGenetics.GatherYield, "GatherYield".Translate() }
        };

        public static string GetDescription(StatDef stat)
        {
            if (!_descriptionOverrides.ContainsKey(stat))
                return stat.description;
            return _descriptionOverrides[stat];
        }

        static Dictionary<StatDef, String> _descriptionOverrides = new Dictionary<StatDef, String>()
        {
            { StatDefOf.MoveSpeed, "SpeedDesc".Translate()},
            { StatDefOf.CarryingCapacity, "CapacityDesc".Translate() },
        };

        /*public static Dictionary<StatDef, String> statNames= new Dictionary<StatDef, String>()
        {
            { StatDefOf.MoveSpeed, "Speed".Translate()},
            { AnimalGenetics.Health, "Health".Translate() },
            { AnimalGenetics.Damage, "Damage".Translate() },
            { StatDefOf.CarryingCapacity, "Capacity".Translate() },
            { StatDefOf.MeatAmount, "Meat".Translate() },
            { StatDefOf.LeatherAmount, "Leather".Translate()},
            { AnimalGenetics.GatherYield, "GatherYield".Translate() }
        };*/

        /*public static Dictionary<StatDef, String> statTooltips = new Dictionary<StatDef, String>()
        {
            { StatDefOf.MoveSpeed, "Movement speed"},
            { AnimalGenetics.Health, "Body part health" },
            { AnimalGenetics.Damage, "Melee attack damage" },
            { StatDefOf.CarryingCapacity, "Caravan and carry capacity" },
            { StatDefOf.MeatAmount, "Meat from butchering" },
            { StatDefOf.LeatherAmount, "Leather from butchering"},
            { AnimalGenetics.GatherYield, "Milk and Wool yields" }
        };
        */

        public static Dictionary<int, String> sortMode = new Dictionary<int, String>()
        {
            {0, "None".Translate() },
            {1, "Asc".Translate() },
            {2, "Desc".Translate() }
        };
    }
}
