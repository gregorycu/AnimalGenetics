using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace AnimalGenetics
{
    public class AnimalGenetics : WorldComponent
    {
        public AnimalGenetics(World world) : base(world)
        {
            var affectedStats = new List<StatDef> { StatDefOf.MoveSpeed };
            foreach (var stat in affectedStats)
                stat.parts.Insert(0, new StatPart(stat));
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref _Data, "data", LookMode.Reference, LookMode.Deep, ref _Things, ref _StatGroups);
        }

        Dictionary<Thing, StatGroup> _Data = new Dictionary<Thing, StatGroup>();
        List<Thing> _Things = new List<Thing>();
        List<StatGroup> _StatGroups = new List<StatGroup>();

        private StatGroup GetData(Pawn pawn)
        {
            if (!_Data.ContainsKey(pawn))
                _Data[pawn] = new StatGroup(pawn);
            return _Data[pawn];
        }

        public float GetFactor(Pawn pawn, StatDef stat)
        {
            return GetData(pawn).GetFactor(stat);
        }
    }
}
