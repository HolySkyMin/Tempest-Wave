using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TempestWave.Setting
{
    public class SEConfig : MonoBehaviour
    {
        public Text numText_music, numText_se;
        public Slider slider_music, slider_se;
        public SettingManager manager;

        void Start()
        {
            if (PlayerPrefs.HasKey("musicvol").Equals(true)) { slider_music.value = Mathf.Floor(PlayerPrefs.GetFloat("musicvol") * 20); }
            else { slider_music.value = 20; }
            if (PlayerPrefs.HasKey("sevol").Equals(true)) { slider_se.value = Mathf.Floor(PlayerPrefs.GetFloat("sevol") * 20); }
            else { slider_se.value = 20; }
            MusicValueChanged();
            SEValueChanged();
        }

        public void SEValueChanged()
        {
            numText_se.text = slider_se.value.ToString();
            manager.SetSEVol(slider_se.value / 20);
        }

        public void MusicValueChanged()
        {
            numText_music.text = slider_music.value.ToString();
            manager.SetMusicVol(slider_music.value / 20);
        }
    }
}