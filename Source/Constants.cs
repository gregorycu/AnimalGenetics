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

        public static Dictionary<StatDef, String> statNames= new Dictionary<StatDef, String>()
        {
            { StatDefOf.MoveSpeed, "Speed"},
            { AnimalGenetics.Health, "Health" },
            { AnimalGenetics.Damage, "Damage" },
            { StatDefOf.CarryingCapacity, "Carrying Capacity" },
            { StatDefOf.MeatAmount, "Meat Amount" },
            { StatDefOf.LeatherAmount, "Leather Amount"},
            { AnimalGenetics.GatherYield, "Milk / Wool" }
        };

        public static Dictionary<StatDef, String> statTooltips = new Dictionary<StatDef, String>()
        {
            { StatDefOf.MoveSpeed, "Movement speed"},
            { AnimalGenetics.Health, "Body part health" },
            { AnimalGenetics.Damage, "Melee attack damage" },
            { StatDefOf.CarryingCapacity, "Caravan and carry capacity" },
            { StatDefOf.MeatAmount, "Meat from butchering" },
            { StatDefOf.LeatherAmount, "Leather from butchering"},
            { AnimalGenetics.GatherYield, "Milk and Wool yields" }
        };
    }
}
