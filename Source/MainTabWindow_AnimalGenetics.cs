using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using RimWorld;
using Verse;

namespace AnimalGenetics
{
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

        protected override IEnumerable<Pawn> Pawns
        {
            get
            {
                /*return from p in Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer)
                       where p.RaceProps.Animal
                       select p;*/

                return from p in Find.CurrentMap.mapPawns.AllPawns
                       where p.Spawned && p.AnimalOrWildMan() && !p.Position.Fogged(p.Map) && !p.IsPrisonerInPrisonCell()
                       select p;
            }
        }
    }
}