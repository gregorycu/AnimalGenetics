using RimWorld;
using Verse;

namespace AnimalGenetics
{
    public class StatPart : RimWorld.StatPart
    {
        public StatPart(StatDef statDef)
        {
            _StatDef = statDef;
            priority = 1.1f;
        }

        public override void TransformValue(StatRequest req, ref float val)
        {
            var factor = GetFactor(req);
            if (factor != null)
                val = val * (float)factor;
        }

        public override string ExplanationPart(StatRequest req)
        {
            if (!Genes.EffectsThing(req.Thing))
                return null;

            Pawn pawn = req.Thing as Pawn;

            if (!Settings.Core.omniscientMode && pawn.Faction != Faction.OfPlayer)
                return null;

            var statRecord = pawn.AnimalGenetics().GeneRecords[_StatDef]; 

            if (statRecord == null)
                return null;

            string postfix = "";
            if (statRecord.Parent != GeneRecord.Source.None)
            {
                string icon = statRecord.Parent == GeneRecord.Source.Mother ? "♀" : "♂";
                postfix = " (x" + GenText.ToStringPercent(statRecord.ParentValue) + icon + ")";
            }

            return "AG.Genetics".Translate() + ": x" + GenText.ToStringPercent(statRecord.Value) + postfix;
        }

        float? GetFactor(StatRequest req)
        {
            if (!req.HasThing)
                return null;

            if (!Genes.EffectsThing(req.Thing))
                return null;

            Pawn pawn = req.Thing as Pawn;

            var statRecord = pawn.AnimalGenetics().GeneRecords[_StatDef];
            return statRecord == null ? 1.0f : statRecord.Value;
        }

        StatDef _StatDef;
    }
}
