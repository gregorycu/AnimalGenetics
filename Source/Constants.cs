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
            StatDefOf.MeleeWeapon_DamageMultiplier,
            AnimalGenetics.GatherYield
        };

        // Stats to insert and modify. For StatDefs that we are using for internal representation but do not effect what we want.
        // MeleeWeapon_DamageMultiplier is for melee weapons - Using as our store for DamageGene via Harmony patch.
        public static List<StatDef> affectedStatsToInsert = new List<StatDef>()
        {
            StatDefOf.MoveSpeed,
            StatDefOf.LeatherAmount,
            StatDefOf.MeatAmount,
            StatDefOf.CarryingCapacity,
            AnimalGenetics.GatherYield
        };

        public static Dictionary<StatDef, String> statNames= new Dictionary<StatDef, String>()
        {
            { StatDefOf.MoveSpeed, "Speed"},
            { StatDefOf.LeatherAmount, "Leather Amount"},
            { StatDefOf.MeatAmount, "Meat Amount" },
            { StatDefOf.CarryingCapacity, "Carrying Capacity" },
            { StatDefOf.MeleeWeapon_DamageMultiplier, "Damage" },
        };
    }
}
