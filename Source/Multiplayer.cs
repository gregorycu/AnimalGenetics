using Verse;
using Multiplayer.API;

namespace AnimalGenetics
{
    [StaticConstructorOnStartup]
    public static class StaticConstructorClass
    {
        static StaticConstructorClass()
        {
            // Multiplayer compatibility
            if (MP.enabled)
            {
                MP.RegisterAll();
            }

        }
    }
}
