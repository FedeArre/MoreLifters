using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LifterMod
{
    public class LifterMod : Mod
    {
        public override string ID => "LifterMod";
        public override string Name => "MoreLifters";
        public override string Author => "Federico Arredondo";
        public override string Version => "1.0";

        LifterHandler lf;
        public LifterMod()
        {
            if(Application.version == "0.771")
            {
                GameObject dummy = new GameObject("dummy_popup");
                dummy.AddComponent<InvalidVersionPopup>();

                Debug.LogError("MoreLifters detected invalid version!");
            }
        }

        public override void OnLoad()
        {
            lf = GameObject.Find("Player").AddComponent<LifterHandler>();
            SavingWrapper.GetInstance().Load();
        }

        public override void Continue()
        {
            lf.Continue();
        }

        public override void OnSave()
        {
            SavingWrapper.GetInstance().Save();
        }
    }
}
