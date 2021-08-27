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

            if (!(req.Thing is Pawn pawn))
                return "";

            if (!Settings.Core.omniscientMode && pawn.Faction != Faction.OfPlayer)
                return null;

            var statRecord = pawn.GetGeneRecord(_StatDef); 

            if (statRecord == null)
                return null;

            string postfix = "";
            if (statRecord.Parent != GeneRecord.Source.None)
            {
                string icon = statRecord.Parent == GeneRecord.Source.Mother ? "♀" : "♂";

                var parentGeneticInformation = statRecord.Parent == GeneRecord.Source.Mother
                    ? pawn.AnimalGenetics()?.Mother
                    : pawn.AnimalGenetics()?.Father;

                // Shouldn't really occur...
                if (parentGeneticInformation != null)
                {
                    var parentValue = parentGeneticInformation.GeneRecords[_StatDef].Value;
                    postfix = " (x" + parentValue.ToStringPercent() + icon + ")";
                }
            }

            return "AG.Genetics".Translate() + ": x" + statRecord.Value.ToStringPercent() + postfix;
        }

        float? GetFactor(StatRequest req)
        {
            if (!req.HasThing)
                return null;

            if (!Genes.EffectsThing(req.Thing))
                return null;

            Pawn pawn = req.Thing as Pawn;

            if (pawn == null)
                Log.Error(req.Thing.ToStringSafe() +  " is not a Pawn");

            var statRecord = pawn.GetGeneRecord(_StatDef);
            return statRecord == null ? 1.0f : statRecord.Value;
        }

        StatDef _StatDef;
    }
}
