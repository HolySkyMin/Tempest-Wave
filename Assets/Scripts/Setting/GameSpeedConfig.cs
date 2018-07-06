using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TempestWave.Monitorer;

namespace TempestWave.Setting
{
    public class GameSpeedConfig : MonoBehaviour
    {
        public GameObject errorText;
        public Slider controller;
        public Text displayer;
        public SettingManager manager;

        void Start()
        {
            if (PlayerPrefs.HasKey("gamespeed").Equals(true))
            {
                controller.value = PlayerPrefs.GetFloat("gamespeed") * 10;
                displayer.text = ((int)controller.value).ToString();
            }
            else
            {
                controller.value = 20;
                ValueChanged();
            }
        }

        public void ValueChanged()
        {
            displayer.text = ((int)controller.value).ToString();
            manager.SetGameSpeed(controller.value / 10);
        }

        public void CheckError()
        {
            if(PlayerPrefs.HasKey("gamespeed").Equals(true))
            {
                if (errorText.activeSelf.Equals(true)) { errorText.SetActive(false); }
            }
        }
    }
}
