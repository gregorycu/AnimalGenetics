namespace AnimalGenetics
{
    using System;
    using System.Collections.Generic;
    using RimWorld;
    using Verse;

    public class PawnTable_Genetics : PawnTable_Animals
    {
        public PawnTable_Genetics(PawnTableDef def, Func<IEnumerable<Pawn>> pawnsGetter, int uiWidth, int uiHeight) : base(def, pawnsGetter, uiWidth, uiHeight)
        {
            //PawnTableDef = def;
            //SetMinMaxSize(def.minWidth, uiWidth, 0, (int)(uiHeight * 400));
        }

        //public PawnTableDef PawnTableDef { get; protected set; }
    }
}