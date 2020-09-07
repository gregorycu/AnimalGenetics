using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalGenetics
{
    public class PawnColumnWorker_SpeedGene : PawnColumnWorker
    {
        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            GUI.color = Utilities.TextColor(GetSpeedGene(pawn));
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect, (GetSpeedGene(pawn)*100).ToString());
            Text.Anchor = TextAnchor.UpperLeft;
            //base.DoCell(rect, pawn, table);
            GUI.color = Color.white;
            /*
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect, ExpectedAmount(pawn).ToString());
            Text.Anchor = TextAnchor.UpperLeft;

            TooltipHandler.TipRegion(rect,
                "AnimalTab.GatherableTip".Translate(pawn.kindDef.race.race.meatLabel, ExpectedAmount(pawn)));
            */
        }

        public override int GetMinWidth(PawnTable table)
        {
            //return Constants.TextCellWidth;
            return 50;
        }

        public float GetSpeedGene(Pawn pawn)
        {
            float mod = 1f;
            //var AnimalGenetics = Find.World.GetComponent<AnimalGenetics>();
            //float mod = AnimalGenetics.GetData((Pawn)pawn).data.Get(StatDefOf.MoveSpeed);
            //return (int)(mod * 100);
            return mod;
            //return (int)(StatDefOf.MeatAmount.defaultBaseValue * pawn.BodySize);
        }

        public override int Compare(Pawn a, Pawn b)
        {
            return GetSpeedGene(a).CompareTo(GetSpeedGene(b));
        }
    }
}