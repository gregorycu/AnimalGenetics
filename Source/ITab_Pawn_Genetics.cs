using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalGenetics
{
    class ITab_Pawn_Genetics : ITab
	{
		//public string labelKey = "TabGenetics";
		//public string tutorTag = "Genetics";

		public ITab_Pawn_Genetics()
		{
			this.labelKey = "TabGenetics";
			this.tutorTag = "Genetics";
		}

		public override bool IsVisible
		{
			get
			{
                return Genes.EffectsThing(base.SelPawn);
			}
		}

		protected override void FillTab()
		{
			Rect rect = new Rect(0f, 0f, this.size.x, this.size.y).ContractedBy(17f);
			rect.yMin += 10f;
			Pawn pawn = base.SelPawn;
			string str;

			if (pawn.gender != Gender.None)
			{
				str = "PawnSummaryWithGender".Translate(pawn.Named("PAWN"));
			}
			else
			{
				str = "PawnSummary".Translate(pawn.Named("PAWN"));
			}
			Text.Font = GameFont.Small;
			Widgets.Label(new Rect(15f, 15f, rect.width * 0.9f, 30f), "Genetics of:  " + pawn.Label);
			Text.Font = GameFont.Tiny;
			Widgets.Label(new Rect(15f, 35f, rect.width * 0.9f, 30f), str);

			float curY = 55f;
			var affectedStats = Constants.affectedStats;
			Text.Anchor = TextAnchor.MiddleCenter;
			Rect rectValue = new Rect(rect.width * 0.6f, curY, rect.width * 0.2f, 20f);
			Widgets.Label(rectValue, "Value");
			TooltipHandler.TipRegion(rectValue, "Gene value. Applied to animal's base stats.");
			Rect rectParent = new Rect(rect.width * 0.8f, curY, rect.width * 0.2f, 20f);
			Widgets.Label(rectParent, "Parent");
			TooltipHandler.TipRegion(rectParent, "The parental value this aniaml inherited from. Will be empty for wild or newly generated animals with no parents." );
			curY += 21;
			foreach (var stat in affectedStats)
			{
				if (stat != AnimalGenetics.GatherYield || (stat == AnimalGenetics.GatherYield && (pawn.def.HasComp(typeof(CompShearable)) || pawn.def.HasComp(typeof(CompMilkable)))))
				{
					curY += DrawRow(rect, curY, Constants.statNames[stat], Genes.GetGene(pawn, stat), Genes.GetInheritString(pawn, stat), Genes.GetInheritValue(pawn, stat), Genes.GetTooltip(stat));
				}
			}
		}

        protected override void UpdateSize()
        {
            base.UpdateSize();
			this.size = new Vector2(300f, 225f);

		}

		private static float DrawRow(Rect rect, float curY, String name, float value, String parent, float parentValue, String tooltip)
		{
			Text.Font = GameFont.Small;
			Rect rect2 = new Rect(0f, curY, rect.width, 20f);
			TooltipHandler.TipRegion(rect2, tooltip);
			if (Mouse.IsOver(rect2))
			{
				GUI.color = new Color(0.5f, 0.5f, 0.5f, 1f);
				GUI.DrawTexture(rect2, TexUI.HighlightTex);
			}
			Text.Anchor = TextAnchor.UpperLeft;
			Widgets.Label(new Rect(20f, curY, (rect.width * 0.5f) - 20f, 30f), name);
			Text.Anchor = TextAnchor.UpperCenter;
			GUI.color = Utilities.TextColor(value);
			Widgets.Label(new Rect(rect.width * 0.6f, curY, rect.width * 0.2f, 30f), (value * 100).ToString("F0") + "%");
			GUI.color = Utilities.TextColor(parentValue);
			Widgets.Label(new Rect(rect.width * 0.8f, curY, rect.width * 0.2f, 30f), parent);
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;

			return 20f;
		}
	}
}
