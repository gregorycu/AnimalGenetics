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

        public static String GetInheritString(Pawn pawn, StatDef gene)
        {
            StatRecord.Source parentType = Find.World.GetComponent<AnimalGenetics>().GetFactor(pawn, gene).Parent;
            if (parentType == StatRecord.Source.None)
            {
                return "";
            }
            else {
                string gender = "♂";
                if (parentType == StatRecord.Source.Mother)
                {
                    gender = "♀";
                }
                return (Find.World.GetComponent<AnimalGenetics>().GetFactor(pawn, gene).ParentValue * 100).ToString("F0") + "% " + gender;
            }
        }
        
        public static float GetInheritValue(Pawn pawn, StatDef gene)
        {
            return Find.World.GetComponent<AnimalGenetics>().GetFactor(pawn, gene).ParentValue;
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
    }
}
