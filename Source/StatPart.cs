using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var factor = GetFactor(req);

            if (factor == null)
                return null;

            return "Genetics: x" + GenText.ToStringPercent((float)factor);
        }

        float? GetFactor(StatRequest req)
        {
            if (!req.HasThing)
                return null;

            Pawn pawn = req.Thing as Pawn;

            if (pawn == null)
                return null;

            return Find.World.GetComponent<AnimalGenetics>().GetFactor(pawn, _StatDef);
        }

        StatDef _StatDef;
    }
}
