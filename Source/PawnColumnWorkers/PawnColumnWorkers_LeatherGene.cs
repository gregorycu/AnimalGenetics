using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalGenetics
{
    public class PawnColumnWorker_LeatherGene : PawnColumnWorker
    {
        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            GUI.color = Utilities.TextColor(GetLeatherGene(pawn));
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect, (GetLeatherGene(pawn)*100).ToString("F0"));
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
        }

        public override int GetMinWidth(PawnTable table)
        {
            //return Constants.TextCellWidth;
            return 50;
        }

        public float GetLeatherGene(Pawn pawn)
        {
            return Find.World.GetComponent<AnimalGenetics>().GetFactor(pawn, StatDefOf.LeatherAmount);
        }

        public override int Compare(Pawn a, Pawn b)
        {
            return GetLeatherGene(a).CompareTo(GetLeatherGene(b));
        }
    }
}