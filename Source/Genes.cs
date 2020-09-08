using RimWorld;
using Verse;

namespace AnimalGenetics
{
    public static class Genes
    {
        public static float GetGene(Pawn pawn, StatDef gene)
        {
            return Find.World.GetComponent<AnimalGenetics>().GetFactor(pawn, gene).Value;
        }

    }
}
