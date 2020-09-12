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
                if (_TableDef == null)
                {
                    _TableDef = PawnTableDefs.Genetics;

                    int index = _TableDef.columns.FindIndex((RimWorld.PawnColumnDef col) => { return col.defName == "AnimalGenetics_Genes"; });
                    _TableDef.columns.RemoveAt(index);

                    for (int i = 0; i < Constants.affectedStats.Count; ++i)
                        _TableDef.columns.Insert(index+i, new PawnColumnDef(Constants.affectedStats[i]));
                }
                return _TableDef;
            }
        }

        RimWorld.PawnTableDef _TableDef;

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
                           where p.Spawned && p.RaceProps.Animal && !p.Position.Fogged(p.Map)  && (p.Faction == null || p.Faction == Faction.OfPlayer)
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
                           where p.Spawned && p.Faction == null && p.RaceProps.Animal && !p.Position.Fogged(p.Map)
                           select p;
                }
                return new List<Pawn>();
            }
        }

        public class PawnColumnDef : RimWorld.PawnColumnDef
        {
            public PawnColumnDef(StatDef stat)
            {
                workerClass = typeof(PawnColumnWorker);
                Stat = stat;
                sortable = true;
                label = Constants.GetLabel(stat);
            }
            public StatDef Stat;
        };

        public class PawnColumnWorker : RimWorld.PawnColumnWorker
        {
            StatDef _StatDef
            {
                get
                {
                    return ((PawnColumnDef)def).Stat;
                }
            }
            public override void DoCell(Rect rect, Pawn pawn, PawnTable table )
            {
                if (_StatDef != AnimalGenetics.GatherYield || (_StatDef == AnimalGenetics.GatherYield && (pawn.def.HasComp(typeof(CompShearable)) || pawn.def.HasComp(typeof(CompMilkable))))) {
                    float gene = Genes.GetGene(pawn, _StatDef);
                    GUI.color = Utilities.TextColor(gene);
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Widgets.Label(rect, (gene * 100).ToString("F0") + "%");
                    Text.Anchor = TextAnchor.UpperLeft;
                    GUI.color = Color.white;
                }
            }

            public override int GetMinWidth(PawnTable table)
            {
                return 80;
            }

            public override int Compare(Pawn a, Pawn b)
            {
                return Genes.GetGene(a, _StatDef).CompareTo(Genes.GetGene(b, _StatDef));
            }
        }
    }
}