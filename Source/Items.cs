using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using RimWorld.Planet;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalGenetics
{
    public class CompUseEffect_IncreaseAllGenes : CompUseEffect
    {
        static float modifyBy = 0.15f;
        public override void DoEffect(Pawn usedBy)
        {
            foreach (StatDef stat in Constants.affectedStats)
            {
                Genes.ModifyGene(usedBy, stat, modifyBy);
            }
        }
    }
}