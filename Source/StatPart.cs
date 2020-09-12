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
            Pawn pawn = req.Thing as Pawn;

            if (pawn == null || !pawn.RaceProps.Animal)
                return null;

            var statRecord = Find.World.GetComponent<AnimalGenetics>().GetFactor(pawn, _StatDef); 

            if (statRecord == null)
                return null;

            string postfix = "";
            if (statRecord.Parent != StatRecord.Source.None)
            {
                string icon = statRecord.Parent == StatRecord.Source.Mother ? "♀" : "♂";
                postfix = " (x" + GenText.ToStringPercent(statRecord.ParentValue) + icon + ")";
            }

            return "Genetics: x" + GenText.ToStringPercent(statRecord.Value) + postfix;
        }

        float? GetFactor(StatRequest req)
        {
            if (!req.HasThing)
                return null;

            Pawn pawn = req.Thing as Pawn;

            if (pawn == null)
                return null;

            var statRecord = Find.World.GetComponent<AnimalGenetics>().GetFactor(pawn, _StatDef);
            return statRecord == null ? 1.0f : statRecord.Value;
        }

        StatDef _StatDef;
    }
}
