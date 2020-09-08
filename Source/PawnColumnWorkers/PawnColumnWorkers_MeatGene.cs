using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalGenetics
{
    public class PawnColumnWorker_MeatGene : PawnColumnWorker
    {
        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            GUI.color = Utilities.TextColor(GetMeatGene(pawn));
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect, (GetMeatGene(pawn)*100).ToString("F0"));
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
        }

        public override int GetMinWidth(PawnTable table)
        {
            //return Constants.TextCellWidth;
            return 50;
        }

        public float GetMeatGene(Pawn pawn)
        {
            return Find.World.GetComponent<AnimalGenetics>().GetFactor(pawn, StatDefOf.MeatAmount).Value;
        }

        public override int Compare(Pawn a, Pawn b)
        {
            return GetMeatGene(a).CompareTo(GetMeatGene(b));
        }
    }
}