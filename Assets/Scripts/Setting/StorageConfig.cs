using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TempestWave.Setting
{
    public class StorageConfig : MonoBehaviour
    {
        public Toggle inter, exter;
        public SettingManager manager;

        // Use this for initialization
        void Start()
        {
            if (PlayerPrefs.HasKey("storage").Equals(true))
            {
                string a = PlayerPrefs.GetString("storage");
                if (a.Equals("internal")) { inter.isOn = true; }
                else if (a.Equals("external")) { exter.isOn = true; }
                manager.SetStorage(a);
            }
            else
            {
                inter.isOn = true;
                manager.SetStorage("internal");
            }
        }

        public void ValueChanged()
        {
            if (inter.isOn.Equals(true)) { manager.SetStorage("internal"); }
            else if (exter.isOn.Equals(true)) { manager.SetStorage("external"); }
        }
    }

}