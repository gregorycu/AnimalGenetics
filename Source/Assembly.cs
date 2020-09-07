using Verse;

namespace AnimalGenetics
{
    namespace Assembly
    {
        [StaticConstructorOnStartup]
        public static class AnimalGeneticsAssemblyLoader
        {
            static AnimalGeneticsAssemblyLoader()
            {
                var h = new HarmonyLib.Harmony("AnimalGenetics");
                h.PatchAll();
            }
        }
    }
}