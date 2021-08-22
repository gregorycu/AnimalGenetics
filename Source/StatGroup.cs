using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AnimalGenetics
{
    public class GeneRecord : IExposable
    {
        public enum Source
        {
            None, Mother, Father
        };
        public Source Parent;
        public float Value;

        public void ExposeData()
        {
            Scribe_Values.Look(ref Value, "Value");
            Scribe_Values.Look(ref Parent, "SelectedParent");
        }
    }
}
