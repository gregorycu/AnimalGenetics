using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalGenetics
{
    public class PawnColumnWorker_GatherYieldGene : PawnColumnWorker
    {
        static StatDef statDef = AnimalGenetics.GatherYield;
        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            if (pawn.def.HasComp(typeof(CompShearable)) || pawn.def.HasComp(typeof(CompMilkable)))
            {
                float gene = Genes.GetGene(pawn, statDef);
                GUI.color = Utilities.TextColor(gene);
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect, (gene * 100).ToString("F0") + "%");
                Text.Anchor = TextAnchor.UpperLeft;
                GUI.color = Color.white;
            }
        }

        public override int GetMinWidth(PawnTable table)
        {
            return 70;
        }

        public override int Compare(Pawn a, Pawn b)
        {
            return Genes.GetGene(a, statDef).CompareTo(Genes.GetGene(b, statDef));
        }
    }
}