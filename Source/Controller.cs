using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AnimalGenetics
{
    public class Controller : Mod
    {
        public static AnimalGeneticsSettings Settings;
        public Controller(ModContentPack content) : base(content)
        {
            Controller.Settings = GetSettings<AnimalGeneticsSettings>();
        }

        public override void DoSettingsWindowContents(Rect rect)
        {;
            Settings.DoSettingsWindowContents(rect);
        }

        public override string SettingsCategory()
        {
            return "Animal Genetics";
        }
    }
}