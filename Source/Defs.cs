using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AnimalGenetics
{
    public class PawnTableColumnsDef : Def
    {
        public List<PawnColumnDef> columns;
    }

    [DefOf]
    public static class PawnTableColumnsDefOf
    {
        static PawnTableColumnsDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(PawnTableColumnsDefOf));
        }

        public static PawnTableColumnsDef Genetics;
    }
}
