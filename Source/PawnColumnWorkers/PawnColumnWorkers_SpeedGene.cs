using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalGenetics
{
    public class PawnColumnWorker_SpeedGene : PawnColumnWorker
    {
        static StatDef statDef = StatDefOf.MoveSpeed;
        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            float gene = Genes.GetGene(pawn, statDef);
            GUI.color = Utilities.TextColor(gene);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect, (gene * 100).ToString("F0") + "%");
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
        }

        public override int GetMinWidth(PawnTable table)
        {
<<<<<<< HEAD
            //return Constants.TextCellWidth;
            return 50;
        }

        public float GetSpeedGene(Pawn pawn)
        {
            return Find.World.GetComponent<AnimalGenetics>().GetFactor(pawn, StatDefOf.MoveSpeed).Value;
=======
            return 80;
>>>>>>> dev/TrainingTab
        }

        public override int Compare(Pawn a, Pawn b)
        {
            return Genes.GetGene(a, statDef).CompareTo(Genes.GetGene(b, statDef));
        }
    }
}