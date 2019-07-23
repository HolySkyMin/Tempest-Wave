using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TempestWave.Setting
{
    public class GameSyncConfig : MonoBehaviour
    {
        public Text numText;
        public Slider slider;
        public SettingManager manager;

        void Start()
        {
            if (PlayerPrefs.HasKey("gamesync").Equals(true)) { slider.value = PlayerPrefs.GetFloat("gamesync"); }
            else { slider.value = 0; ValueChanged(); }
            numText.text = slider.value.ToString();
        }

        public void ValueChanged()
        {
            numText.text = slider.value.ToString();
            manager.SetGameSync(slider.value);
        }
    }
}
