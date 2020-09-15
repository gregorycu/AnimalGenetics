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

        private static float gap = 12f;
        private static float checkboxWidth = 26f;
        private float animalsWidth = Text.CalcSize("AG.Animals".Translate()).x + checkboxWidth;
        private float humanlikesWidth = Text.CalcSize("AG.Humanlikes".Translate()).x + checkboxWidth;
        private float colonyWidth = Text.CalcSize("AG.Colony".Translate()).x + checkboxWidth;
        private float wildWidth = Text.CalcSize("AG.Wild".Translate()).x + checkboxWidth;
        private float otherFactionsWidth = Text.CalcSize("AG.OtherFactions".Translate()).x + checkboxWidth;
        private float searchWidth = Text.CalcSize("AG.Search".Translate()).x + 5f;

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
            float curX = 5f;
            float curX2 = rect.width - 300f;
            if (!Controller.Settings.humanMode)
            {
                humans = false;
            }
            base.DoWindowContents(rect);

            //working from left side
            Text.Anchor = TextAnchor.LowerLeft;
            if (Controller.Settings.humanMode)
            {
                
                Widgets.CheckboxLabeled(new Rect(curX, 10f, animalsWidth, checkboxHeight), "AG.Animals".Translate(), ref animals, false, null, null, true);
                curX += animalsWidth + gap;
                Widgets.CheckboxLabeled(new Rect(curX, 10f, humanlikesWidth, checkboxHeight), "AG.Humanlikes".Translate(), ref humans, false, null, null, true);
                curX += humanlikesWidth + gap + 20f; //extra 20 for category gap
            }
            Widgets.CheckboxLabeled(new Rect(curX, 10f, colonyWidth, checkboxHeight), "AG.Colony".Translate(), ref factionOwn, false, null, null, true);
            curX += colonyWidth + gap;
            Widgets.CheckboxLabeled(new Rect(curX, 10f, wildWidth, checkboxHeight), "AG.Wild".Translate(), ref factionWild, false, null, null, true);
            curX += wildWidth + gap;
            Widgets.CheckboxLabeled(new Rect(curX, 10f, otherFactionsWidth, checkboxHeight), "AG.OtherFactions".Translate(), ref factionOther, false, null, null, true);
            curX += otherFactionsWidth + gap;
            Text.Anchor = TextAnchor.UpperLeft;

            // Working from right side
            curX2 -= 50f;
            Text.Anchor = TextAnchor.MiddleCenter;
            if (Widgets.ButtonText(new Rect(curX2, 10f, 42f, 24f), Constants.sortMode[Controller.Settings.sortMode], true, true, true))
            {
                Controller.Settings.sortMode += 1;
                if (Controller.Settings.sortMode >= Constants.sortMode.Count) { Controller.Settings.sortMode = 0; }
                SetDirty();
            }
            curX2 -= 55f;
            Text.Font = GameFont.Tiny;
            Widgets.Label(new Rect(curX2, 5f, 50f, 32f), "AG.PrimarySort".Translate());
            Text.Font = GameFont.Small;
            curX2 -= searchWidth + 15f;
            if (Widgets.ButtonText(new Rect(curX2, 10f, 55f, 24f), "AG.Search".Translate(), true, true, true)) { SetDirty(); }
            Text.Anchor = TextAnchor.UpperLeft;
            curX2 -= 125f;
            filterText = Widgets.TextField(new Rect(curX2, 10f, 120f, 24f), filterText, 20, new Regex(".*"));

            

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