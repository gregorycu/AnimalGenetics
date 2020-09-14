namespace AnimalGenetics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using RimWorld;
    using Verse;

    public class PawnTable_Genetics : PawnTable
    {
        public PawnTable_Genetics(PawnTableDef def, Func<IEnumerable<Pawn>> pawnsGetter, int uiWidth, int uiHeight) : base(def, pawnsGetter, uiWidth, uiHeight)
        {
        }
        protected override IEnumerable<Pawn> PrimarySortFunction(IEnumerable<Pawn> input)
        {
            switch (Controller.Settings.sortMode)
            {
                case 1:
                    return from p in input
                           orderby p.def.label ascending
                           select p;
                case 2:
                    return from p in input
                           orderby p.def.label descending
                           select p;
            }
            return input;
        }

    }
}