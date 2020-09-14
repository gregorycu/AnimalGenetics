using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        private static bool humans;
        private static bool lastAnimals;
        private static bool lastHumans;
        private static bool factionOwn;
        private static bool factionWild;
        private static bool factionOther;
        private static bool lastFactionOwn;
        private static bool lastFactionWild;
        private static bool lastFactionOther;
        private static float checkboxHeight = 24f;
        private static String filterText;

        [DefOf]
        public static class PawnTableDefs
        {
            public static PawnTableDef Genetics;
        }

        public MainTabWindow_AnimalGenetics()
        {
            animals = true;
            humans = false;
            lastAnimals = animals;
            lastHumans = humans;
            factionOwn = false;
            factionWild = false;
            factionOther = false;
            lastFactionOwn = factionOwn;
            lastFactionWild = factionWild;
            lastFactionOther = factionOther;
            forcePause = false;
        }

        public override void DoWindowContents(Rect rect)
        {
            float humanOffset = 5;
            if (!Controller.Settings.humanMode)
            {
                humans = false;
            }
            base.DoWindowContents(rect);
            Text.Anchor = TextAnchor.LowerLeft;
            if (Controller.Settings.humanMode)
            {
                Widgets.CheckboxLabeled(new Rect(5f, 10f, 80f, checkboxHeight), "Animals".Translate(), ref animals, false, null, null, true);
                Widgets.CheckboxLabeled(new Rect(95f, 10f, 102f, checkboxHeight), "Humanlikes".Translate(), ref humans, false, null, null, true);
                humanOffset += 250f;
            }
            Widgets.CheckboxLabeled(new Rect(humanOffset, 10f, 73f, checkboxHeight), "Colony".Translate(), ref factionOwn, false, null, null, true);
            Widgets.CheckboxLabeled(new Rect(humanOffset + 83f, 10f, 55f, checkboxHeight), "Wild".Translate(), ref factionWild, false, null, null, true);
            Widgets.CheckboxLabeled(new Rect(humanOffset + 148f, 10f, 123f, checkboxHeight), "OtherFactions".Translate(), ref factionOther, false, null, null, true);
            Text.Anchor = TextAnchor.UpperLeft;

            filterText = Widgets.TextField(new Rect(humanOffset + 310f, 10f, 120f, 24f), filterText, 20, new Regex(".*"));
            Text.Anchor = TextAnchor.MiddleCenter;
            if (Widgets.ButtonText(new Rect(humanOffset + 433f, 10f, 55f, 24f), "Search".Translate(), true, true, true)) { SetDirty(); }
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(new Rect(humanOffset + 510f, 5f, 50f, 32f), "PrimarySort".Translate());
            Text.Font = GameFont.Small;
            if (Widgets.ButtonText(new Rect(humanOffset + 565f, 10f, 42f, 24f), Constants.sortMode[Controller.Settings.sortMode], true, true, true)) {
                Controller.Settings.sortMode += 1;
                if (Controller.Settings.sortMode >= Constants.sortMode.Count) { Controller.Settings.sortMode = 0; }
                SetDirty();
            }
            Text.Anchor = TextAnchor.UpperLeft;
            if (animals != lastAnimals)
            {
                lastAnimals = animals;
                SetDirty();
            }
            if (humans != lastHumans)
            {
                lastHumans = humans;
                SetDirty();
            }
            if (factionOwn != lastFactionOwn)
            {
                lastFactionOwn = factionOwn;
                SetDirty();
            }
            if (factionWild != lastFactionWild)
            {
                lastFactionWild = factionWild;
                SetDirty();
            }
            if (factionOther != lastFactionOther)
            {
                lastFactionOther = factionOther;
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
                List<Pawn> toReturn = new List<Pawn>();
                toReturn.AddRange(from p in Find.CurrentMap.mapPawns.AllPawns
                                  where p.Spawned && !p.Position.Fogged(p.Map) && VisibleSpecies(p) && VisibleFactions(p) && TextFilter(p)
                                  select p);
                return toReturn;
            }
        }

        private bool VisibleFactions(Pawn p)
        {
            if (factionOwn && p.Faction == Faction.OfPlayer)
            {
                return true;
            }
            if (factionWild && p.Faction == null)
            {
                return true;
            }
            if (factionOther && p.Faction != Faction.OfPlayer && p.Faction != null)
            {
                return true;
            }
            return false;
        }

        private bool VisibleSpecies(Pawn p)
        {
            if (animals && p.RaceProps.Animal)
            {
                return true;
            }
            if (humans && p.RaceProps.Humanlike)
            {
                return true;
            }
            return false;
        }

        private bool TextFilter(Pawn p)
        {
            if (filterText == "")
                return true;

            return (p.Name != null && p.Name.ToStringFull.Contains(filterText)) || p.KindLabel.Contains(filterText) || p.def.label.Contains(filterText);
        }
    }
}