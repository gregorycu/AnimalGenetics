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
            { StatDefOf.MoveSpeed, "Speed"},
            { AnimalGenetics.Health, "Health" },
            { AnimalGenetics.Damage, "Damage" },
            { StatDefOf.CarryingCapacity, "Capacity" },
            { StatDefOf.MeatAmount, "Meat" },
            { StatDefOf.LeatherAmount, "Leather"},
            { AnimalGenetics.GatherYield, "Milk / Wool" }
        };

        public static string GetDescription(StatDef stat)
        {
            if (!_descriptionOverrides.ContainsKey(stat))
                return stat.description;
            return _descriptionOverrides[stat];
        }

        static Dictionary<StatDef, String> _descriptionOverrides = new Dictionary<StatDef, String>()
        {
            { StatDefOf.MoveSpeed, "Movement speed"},
            { StatDefOf.CarryingCapacity, "Caravan and carry capacity" },
        };
    }
}
