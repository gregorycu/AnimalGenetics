using RimWorld;
using Verse;
using System;

namespace AnimalGenetics
{
    public static class Genes
    {
        public static float GetGene(Pawn pawn, StatDef gene)
        {
            return Find.World.GetComponent<AnimalGenetics>().GetFactor(pawn, gene).Value;
        }
		


        public static StatRecord GetStatRecord(Pawn pawn, StatDef gene)
        {
            return Find.World.GetComponent<AnimalGenetics>().GetFactor(pawn, gene);
        }

        public static String GetInheritString(StatRecord statRecord)
        {
            StatRecord.Source parentType = statRecord.Parent;
            if (parentType == StatRecord.Source.None)
                return "";

            string gender = parentType == StatRecord.Source.Mother ? "♀" : "♂";
            return (statRecord.ParentValue * 100).ToString("F0") + "% " + gender;
        }

        public static String GetGenderSymbol(StatRecord.Source source)
        {
            return source == StatRecord.Source.Mother ? "♀" : "♂";
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

            return pawn.RaceProps.Animal || pawn.RaceProps.Humanlike && Controller.Settings.humanMode;
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
