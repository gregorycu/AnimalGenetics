using UnityEngine;
using Verse;


namespace AnimalGenetics
{
    public static class Settings
    {
        public static UISettings UI;
        public static IntegrationSettings Integration;
        public static CoreSettings Core
        {
            get { return Find.World.GetComponent<AnimalGenetics>().Settings; }
        }
        public static CoreSettings InitialCore;
    }

    public class CoreMod : Mod
    {
        public static CoreSettings InitialSettings;

        public CoreMod(ModContentPack content) : base(content)
        {
            Settings.InitialCore = GetSettings<CoreSettings>();
        }

        public override void DoSettingsWindowContents(Rect rect)
        {
            SettingsUI.DoSettings(ConfigureInitialSettings ? Settings.InitialCore : Settings.Core, rect);
        }

        public bool ConfigureInitialSettings
        {
            get { return Verse.Current.ProgramState == ProgramState.Entry; }
        }

        public override string SettingsCategory()
        {
            if (ConfigureInitialSettings)
                return "Animal Genetics - Initial Game Settings";
            else
                return "Animal Genetics - Game Settings";
        }
    }

    public class UIMod : Mod
    {
         public UIMod(ModContentPack content) : base(content)
            {
                Settings.UI = GetSettings<UISettings>();
            }

        public override void DoSettingsWindowContents(Rect rect)
        {
            SettingsUI.DoSettings(Settings.UI, rect);
        }

        public override string SettingsCategory()
        {
            return "Animal Genetics - UI Settings";
        }
    }

    public class IntegrationMod : Mod
    {
        public IntegrationMod(ModContentPack content) : base(content)
        {
            Settings.Integration = GetSettings<IntegrationSettings>();
        }

        public override void DoSettingsWindowContents(Rect rect)
        {
            SettingsUI.DoSettings(Settings.Integration, rect);
        }

        public override string SettingsCategory()
        {
            return "Animal Genetics - Integration Settings";
        }
    }
}