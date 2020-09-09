using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalGenetics
{
    public class MainTabWindow_AnimalGenetics : MainTabWindow_PawnTable
    {
        private static bool animals;
        private static bool wildlife;
        private static bool lastAnimals;
        private static bool lastWildlife;

        [DefOf]
        public static class PawnTableDefs
        {
            public static PawnTableDef Genetics;
        }

        public MainTabWindow_AnimalGenetics()
        {
            wildlife = false;
            animals = true;
            lastWildlife = wildlife;
            lastAnimals = animals;

            //wildlife.onValueChanged.AddListener(SetDirty());
            forcePause = false;
        }

        public override void DoWindowContents(Rect rect)
        {
            base.DoWindowContents(rect);
            Widgets.CheckboxLabeled(new Rect(5f, 10f, 80f, 22f), "Animals", ref animals, false, null, null, false);
            Widgets.CheckboxLabeled(new Rect(95f, 10f, 75f, 22f), "Wildlife", ref wildlife, false, null, null, false);
            if (wildlife != lastWildlife)
            {
                lastWildlife = wildlife;
                SetDirty();
            }
            if (animals != lastAnimals)
            {
                lastAnimals = animals;
                SetDirty();
            }
        }

        protected override PawnTableDef PawnTableDef
        {
            get
            {
                return PawnTableDefs.Genetics;
            }
        }

        public override void PostOpen()
        {
            base.PostOpen();
            Find.World.renderer.wantedMode = WorldRenderMode.None;
        }

        protected override IEnumerable<Pawn> Pawns
        {
            get
            {
                if (animals && wildlife)
                {
                    return from p in Find.CurrentMap.mapPawns.AllPawns
                           where p.Spawned && p.RaceProps.Animal && !p.Position.Fogged(p.Map) && !p.IsPrisonerInPrisonCell()
                           select p;
                }
                if (animals)
                {
                    return from p in Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer)
                       where p.RaceProps.Animal
                       select p;
                }
                if (wildlife)
                {
                    return from p in Find.CurrentMap.mapPawns.AllPawns
                           where p.Spawned && p.Faction == null && p.RaceProps.Animal && !p.Position.Fogged(p.Map) && !p.IsPrisonerInPrisonCell()
                           select p;
                }
                return new List<Pawn>();
            }
        }
    }
}