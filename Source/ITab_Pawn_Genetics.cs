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
				return base.SelPawn.RaceProps.Animal;
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
			//Text.Font = GameFont.Tiny;
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(new Rect(rect.width * 0.6f, curY, rect.width * 0.2f, 30f), "Value");
			Widgets.Label(new Rect(rect.width * 0.8f, curY, rect.width * 0.2f, 30f), "Parent");
			curY += 20;
			foreach (var stat in affectedStats)
			{
				curY += DrawRow(rect, curY, Constants.statNames[stat], Genes.GetGene(pawn, stat), Genes.GetInheritString(pawn, stat), Genes.GetInheritValue(pawn, stat));
			}
			
			//GUI.color = Utilities.TextColor(Find.World.GetComponent<AnimalGenetics>().GetFactor(base.SelPawn, StatDefOf.CarryingCapacity));
			//Text.Anchor = TextAnchor.MiddleCenter;
			//Widgets.Label(rect, (Find.World.GetComponent<AnimalGenetics>().GetFactor(pawn, StatDefOf.CarryingCapacity) * 100).ToString("F0"));
			//Text.Anchor = TextAnchor.UpperLeft;
			
			//listingStandard.Label((Find.World.GetComponent<AnimalGenetics>().GetFactor(base.SelPawn, StatDefOf.CarryingCapacity) * 100).ToString("F0"));
			//GUI.color = Color.white;
			//listingStandard.End();
		}

        protected override void UpdateSize()
        {
            base.UpdateSize();
			this.size = new Vector2(300f, 220f);

		}

		private static float DrawRow(Rect rect, float curY, String name, float value, String parent, float parentValue)
		{
			Text.Font = GameFont.Small;
			Rect rect2 = new Rect(0f, curY, rect.width, 20f);
			if (Mouse.IsOver(rect2))
			{
				GUI.color = new Color(0.5f, 0.5f, 0.5f, 1f);
				GUI.DrawTexture(rect2, TexUI.HighlightTex);
			}
			Text.Anchor = TextAnchor.UpperLeft;
			Widgets.Label(new Rect(20f, curY, (rect.width * 0.5f) - 20f, 30f), name);
			Text.Anchor = TextAnchor.MiddleCenter;
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
