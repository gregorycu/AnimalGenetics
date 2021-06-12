using RimWorld;
using Verse;
using System;

namespace AnimalGenetics
{
    public static class Genes
    {
        public static float GetGene(Pawn pawn, StatDef gene)
        {
            return pawn.GetGene(gene);
        }
		
        public static String GetGenderSymbol(GeneRecord.Source source)
        {
            return source == GeneRecord.Source.Mother ? "♀" : "♂";
        }

        public static String GetTooltip(StatDef gene)
        {
            return Constants.GetDescription(gene);
        }

        public static bool EffectsThing(Thing thing)
        {
            if (thing == null)
                return false;

            Pawn pawn = thing as Pawn;

            if (pawn == null)
                return false;

            return pawn.RaceProps.Animal || pawn.RaceProps.Humanlike && Settings.Core.humanMode;
        }
        
        public static bool Gatherable(Pawn pawn)
        {
            foreach (Type type in Assembly.AnimalGeneticsAssemblyLoader.gatherableTypes)
            {
                if (pawn.def.HasComp(type)) { return true; }
            }
            return false;
        }
    }
}
