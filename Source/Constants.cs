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
            StatDefOf.CarryingCapacity
        };

        public static Dictionary<StatDef, String> statNames= new Dictionary<StatDef, String>()
        {
            { StatDefOf.MoveSpeed, "Speed"},
            { StatDefOf.LeatherAmount, "Leather Amount"},
            { StatDefOf.MeatAmount, "Meat Amount" },
            { StatDefOf.CarryingCapacity, "Carrying Capacity" }
        };
    }
}
