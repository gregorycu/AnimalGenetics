using RimWorld;
using UnityEngine;
using Verse;
using System;
using System.Collections.Generic;

namespace AnimalGenetics
{
    public static class Constants
    {
        public static List<StatDef> affectedStats = new List<StatDef>()
        {
            StatDefOf.MoveSpeed,
            StatDefOf.LeatherAmount,
            StatDefOf.MeatAmount,
            StatDefOf.CarryingCapacity,
            AnimalGenetics.Damage,
            AnimalGenetics.GatherYield
        };

        public static List<StatDef> affectedStatsToInsert = new List<StatDef>()
        {
            StatDefOf.MoveSpeed,
            StatDefOf.LeatherAmount,
            StatDefOf.MeatAmount,
            StatDefOf.CarryingCapacity
        };

        public static Dictionary<StatDef, String> statNames= new Dictionary<StatDef, String>()
        {
            { StatDefOf.MoveSpeed, "Speed"},
            { AnimalGenetics.Damage, "Damage" },
            { StatDefOf.CarryingCapacity, "Carrying Capacity" },
            { StatDefOf.MeatAmount, "Meat Amount" },
            { StatDefOf.LeatherAmount, "Leather Amount"},
            { AnimalGenetics.GatherYield, "Milk / Wool" }
        };
    }
}
