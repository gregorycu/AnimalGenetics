namespace AnimalGenetics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using RimWorld;
    using UnityEngine;
    using Verse;
    
    public class MainTabWindow_AnimalGenetics : MainTabWindow_Animals
    {
        [DefOf]
        public static class PawnTableDefs
        {
            public static PawnTableDef Genetics;
        }

        public MainTabWindow_AnimalGenetics()
        {
            forcePause = false;
        }

        protected override PawnTableDef PawnTableDef
        {
            get
            {
                return PawnTableDefs.Genetics;
            }
        }

        //public override void DoWindowContents(Rect canvas)
        //{
        // set size and draw background
        //    base.DoWindowContents(canvas);
        //}

        /// <summary>
        ///     Builds pawn list + slot positions
        ///     called from base.PreOpen(), and various methods that want to reset the graph.
        /// </summary>
        /*       protected void BuildPawnList()
               {
                   // rebuild pawn list
                   pawns = Find.CurrentMap.mapPawns.FreeColonists.ToList();
                   firstDegreePawns = pawns.SelectMany(p => p.relations.RelatedPawns).Distinct().Except(pawns).ToList();
                   RelationsHelper.ResetOpinionCache();

                   // recalculate positions
                   CreateAreas();
                   CreateGraph();

                   // create list of social thoughts to pawns
                   RelationsHelper.CreateThoughtList(pawns.Concat(firstDegreePawns).ToList());
               }*/
    }
}