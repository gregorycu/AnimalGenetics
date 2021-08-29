using System.Linq;
using RimWorld;
using Verse;

namespace AnimalGenetics
{
    internal class MarketValueCalculator : RimWorld.StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            var pawn = req.Thing as Pawn;
            if (! Genes.EffectsThing(pawn))
                return;

            var genes = pawn.GetGenes();

            var factor = genes.Select(g => pawn.GetGene(g)).Aggregate(1.0f, (lhs, rhs) => lhs * rhs);

            val *= factor;
        }

        public override string ExplanationPart(StatRequest req)
        {
            float factor = 1;
            TransformValue(req, ref factor);
            return "AG.Genetics".Translate() + ": x" + (factor * 100).ToString("F0") + "%\n";
        }
    }
}


