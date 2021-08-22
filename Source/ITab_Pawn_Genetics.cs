using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalGenetics
{
    class ITab_Pawn_Genetics : ITab
	{
		public ITab_Pawn_Genetics()
		{
			this.labelKey = "AG.TabGenetics";
			this.tutorTag = "AG.Genetics";
		}

		public override bool IsVisible
		{
			get
			{
				if (!Settings.Core.omniscientMode && base.SelPawn.Faction != Faction.OfPlayer)
                {
					return false;
                }
				return Genes.EffectsThing(base.SelPawn);
			}
		}

		protected override void FillTab()
		{
			Rect rect = new Rect(0f, 0f, this.size.x, this.size.y).ContractedBy(20f);
			Pawn pawn = base.SelPawn;

			string str = (pawn.gender == Gender.None ? "PawnSummary" : "PawnSummaryWithGender").Translate(pawn.Named("PAWN"));

			Text.Font = GameFont.Small;
			Widgets.Label(new Rect(15f, 15f, rect.width * 0.9f, 30f), "AG.GeneticsOf".Translate() + ":  " + pawn.Label);
			Text.Font = GameFont.Tiny;
			Widgets.Label(new Rect(15f, 35f, rect.width * 0.9f, 30f), str);
            Text.Font = GameFont.Small;

            float headerY = 55f;
            float curY = headerY;

            Text.Anchor = TextAnchor.MiddleCenter;

            Rect rectValue = new Rect(rect.x + rect.width * 0.4f, curY, rect.width * 0.2f, 20f);
            Widgets.Label(rectValue, "AG.Value".Translate());
            TooltipHandler.TipRegion(rectValue, "AG.ValueTooltop".Translate());

            curY += 20;

            var stats = Constants.affectedStats.Where((StatDef stat) => stat != AnimalGenetics.GatherYield || Genes.Gatherable(pawn));
            foreach (var stat in stats)
            {
                Rect rect2 = new Rect(rect.x, curY, rect.width, 20f);
                TooltipHandler.TipRegion(rect2, Genes.GetTooltip(stat));
                if (Mouse.IsOver(rect2))
                {
                    GUI.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                    GUI.DrawTexture(rect2, TexUI.HighlightTex);
                    GUI.color = Color.white;
                }

                Text.Anchor = TextAnchor.MiddleLeft;

                Widgets.Label(new Rect(20f, curY, (rect.x + rect.width * 0.4f) - 20f, 20f), Constants.GetLabel(stat));

                Utility.GUI.DrawGeneValueLabel(new Rect(rect.x + rect.width * 0.4f, curY, rect.width * 0.2f, 20f), pawn.GetGene(stat));

                curY += 20;
            }

            if (Settings.UI.showBothParentsInPawnTab)
                curY += DrawBothParentData(rect, headerY, pawn);
            else
                curY += DrawSingleParentData(rect, headerY, pawn);

            Text.Anchor = TextAnchor.UpperLeft;
        }

        protected override void UpdateSize()
        {
            base.UpdateSize();
			this.size = new Vector2(300f, 225f);
		}

        private static float DrawBothParentData(Rect rect, float curY, Pawn pawn)
        {
            Text.Anchor = TextAnchor.MiddleCenter;

            var rectMother = new Rect(rect.x + rect.width * 0.6f, curY, rect.width * 0.2f, 20f);
            Widgets.Label(rectMother, "AnimalGenetics.Mother".Translate());

            var rectFather = new Rect(rect.x + rect.width * 0.8f, curY, rect.width * 0.2f, 20f);
            Widgets.Label(rectFather, "AnimalGenetics.Father".Translate());

            curY += 20;

            var statsGroup = pawn.AnimalGenetics()?.GeneRecords;
            if (statsGroup == null)
                return 0;

            var motherGeneRecords = pawn.AnimalGenetics().Mother?.GeneRecords;
            var fatherGeneRecords = pawn.AnimalGenetics().Father?.GeneRecords;

            var stats = Constants.affectedStats.Where((StatDef stat) => stat != AnimalGenetics.GatherYield || Genes.Gatherable(pawn));
            foreach (var stat in stats)
            {
                var statRecord = statsGroup[stat];
                var motherStatRecord = motherGeneRecords?[stat];
                var fatherStatRecord = fatherGeneRecords?[stat];

                if (motherStatRecord != null)
                    Utility.GUI.DrawGeneValueLabel(new Rect(rect.x + rect.width * 0.6f, curY, rect.width * 0.2f, 20f), motherStatRecord.Value, statRecord.Parent != GeneRecord.Source.Mother);

                if (fatherStatRecord != null)
                    Utility.GUI.DrawGeneValueLabel(new Rect(rect.x + rect.width * 0.8f, curY, rect.width * 0.2f, 20f), fatherStatRecord.Value, statRecord.Parent != GeneRecord.Source.Father);

                curY += 20f;
            }

            return stats.Count() * 20f;
        }

        private static float DrawSingleParentData(Rect rect, float curY, Pawn pawn)
        {
            Text.Anchor = TextAnchor.MiddleCenter;

            Rect rectParent = new Rect(rect.x + rect.width * 0.6f, curY, rect.width * 0.2f, 20f);
            Widgets.Label(rectParent, "AG.Parent".Translate());
            TooltipHandler.TipRegion(rectParent, "AG.ParentTooltop".Translate());

            curY += 20f;

            var motherGeneRecords = pawn.AnimalGenetics().Mother?.GeneRecords;
            var fatherGeneRecords = pawn.AnimalGenetics().Father?.GeneRecords;

            var stats = Constants.affectedStats.Where((StatDef stat) => stat != AnimalGenetics.GatherYield || Genes.Gatherable(pawn));
            foreach (var stat in stats)
            {
                var statRecord = pawn.GetGeneRecord(stat);
                if (statRecord.Parent != GeneRecord.Source.None)
                {
                    string extra = Genes.GetGenderSymbol(statRecord.Parent);

                    var record = statRecord.Parent == GeneRecord.Source.Mother ? motherGeneRecords?[stat] : fatherGeneRecords?[stat];

                    if (record != null)
                        Utility.GUI.DrawGeneValueLabel(new Rect(rect.x + rect.width * 0.6f, curY, rect.width * 0.2f, 20f), (float)record.Value, false, extra);
                }

                curY += 20f;
            }

            return stats.Count() * 20f;
        }
	}
}
