using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TempestWave.Data;
using TempestWave.Monitorer;
using System;
using TempestWave.Core.UI;
using TempestWave.Core.Language;

namespace TempestWave.Setting
{
    public class SettingManager : MonoBehaviour
    {
        private KeyCode[] gameKeys = new KeyCode[6];
        private int selectedLine, langCode, tinyTipVal, flickValue, canvasVal, T2ScreenVal, fixResVal, themeVal;
        private float gameSpeed, gameSync, musicVol, seVolume;
        private bool KeyInputWaiting;
        private string storageSet, hitSound, tempestic;
        public GameObject[] Tabs = new GameObject[3], Panels = new GameObject[3], btn = new GameObject[6];
        public GameObject errorText, storageField, resolutionField, WindowsPanel, AndroidPanel, iOSPanel;
        public Toggle fullScreen, hitSoundTog, tinyTipTog, tpstTog, flickLoose, flickNormal, flickStrict, flickDynamic, canvasNote, T2Portrait, T2Landscape, fixResTog;
        public Toggle[] themeToggle;
        public InputField SDval;
        public Text socialTextAnd, socialTextiOS, themeName;
        public Dropdown screenSelector;
        public GameSpeedConfig speedConfig;
        public ScreenScaleControl scalecontrol;

        // Use this for initialization
        void Start()
        {
            if (Application.platform.Equals(RuntimePlatform.WindowsPlayer)) { WindowsPanel.SetActive(true); }
            else if (Application.platform.Equals(RuntimePlatform.Android)) { AndroidPanel.SetActive(true); }
            else if (Application.platform.Equals(RuntimePlatform.IPhonePlayer)) { iOSPanel.SetActive(true); }

            KeyInputWaiting = false;
            selectedLine = 0;
            for (int i = 1; i <= 6; i++)
            {
                if (PlayerPrefs.HasKey("key" + i.ToString()).Equals(true))
                {
                    gameKeys[i - 1] = (KeyCode)PlayerPrefs.GetInt("key" + i.ToString());
                    btn[i - 1].GetComponentInChildren<Text>().text = gameKeys[i - 1].ToString();
                }
            }
#if UNITY_STANDALONE || UNITY_EDITOR
            screenSelector.value = PlayerPrefs.GetInt("screenselect");
            if (PlayerPrefs.GetInt("fullscreen").Equals(0)) { fullScreen.isOn = true; }
            else { fullScreen.isOn = false; }
            fullScreen.interactable = true;
#endif
            if (PlayerPrefs.HasKey("hitsound").Equals(false)) { PlayerPrefs.SetString("hitsound", "true"); hitSound = "true"; }
            else { hitSound = PlayerPrefs.GetString("hitsound"); }
            if (hitSound.Equals("true")) { hitSoundTog.isOn = true; }

            if (PlayerPrefs.HasKey("tempestic").Equals(false)) { PlayerPrefs.SetString("tempestic", "true"); tempestic = "true"; }
            else { tempestic = PlayerPrefs.GetString("tempestic"); }
            if (tempestic.Equals("true")) { tpstTog.isOn = true; }

            if (PlayerPrefs.HasKey("flickmode").Equals(false)) { PlayerPrefs.SetInt("flickmode", 1); flickValue = 1; }
            else { flickValue = PlayerPrefs.GetInt("flickmode"); }
            if (flickValue.Equals(0)) { flickLoose.isOn = true; }
            else if (flickValue.Equals(1)) { flickNormal.isOn = true; }
            else if (flickValue.Equals(2)) { flickStrict.isOn = true; }
            else if (flickValue.Equals(3)) { flickDynamic.isOn = true; }

            if (PlayerPrefs.HasKey("canvasnote") && PlayerPrefs.GetInt("canvasnote").Equals(0)) { canvasNote.isOn = true; canvasVal = 0; }
            else { canvasNote.isOn = false; canvasVal = 1; }

            if (PlayerPrefs.HasKey("sdcard")) { SDval.text = PlayerPrefs.GetString("sdcard"); }

            if (PlayerPrefs.HasKey("theater2") && PlayerPrefs.GetInt("theater2").Equals(0)) { T2Portrait.isOn = true; T2ScreenVal = 0; }
            else { T2Landscape.isOn = true; T2ScreenVal = 1; }

            langCode = PlayerPrefs.GetInt("locale");
            if(PlayerPrefs.HasKey("tinytip").Equals(true) && PlayerPrefs.GetInt("tinytip").Equals(1)) { tinyTipTog.isOn = false; tinyTipVal = 1; }
            else { tinyTipTog.isOn = true; tinyTipVal = 0; }
            if (PlayerPrefs.HasKey("720p").Equals(true) && PlayerPrefs.GetInt("720p").Equals(0)) { fixResTog.isOn = true; fixResVal = 0; }
            else { fixResTog.isOn = false; fixResVal = 1; }

            themeVal = 0;
            if (PlayerPrefs.HasKey("theme")) { themeVal = PlayerPrefs.GetInt("theme"); }
            themeToggle[themeVal].isOn = true;
            SetTheme();


#if UNITY_ANDROID
            if (!Social.localUser.authenticated)
            {
                socialTextAnd.text = LocaleManager.instance.GetLocaleText("setting_notloggedin");
                socialTextAnd.color = Color.white;
            }
            else
            {
                socialTextAnd.text = LocaleManager.instance.GetLocaleText("setting_playgameslogged");
                socialTextAnd.color = GlobalTheme.ThemeColor(RuntimePlatform.Android);
            }
#elif UNITY_IOS
            if (!Social.localUser.authenticated)
            {
                socialTextiOS.text = LocaleManager.instance.GetLocaleText("setting_notloggedin");
                socialTextiOS.color = Color.white;
            }
            else
            {
                socialTextiOS.text = LocaleManager.instance.GetLocaleText("setting_gamecenterlogged");
                socialTextiOS.color = GlobalTheme.ThemeColor(RuntimePlatform.IPhonePlayer);
            }
#endif
        }

        private void Update()
        {
            if (KeyInputWaiting.Equals(true))
            {
                foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(k))
                    {
                        SetKeyCode(k, selectedLine);
                    }
                }
            }
        }

        public void TabSelected(int index)
        {
            for(int i = 0; i < 3; i++)
            {
                if (i.Equals(index))
                {
                    Panels[i].GetComponent<CanvasGroup>().alpha = 1;
                    Panels[i].GetComponent<RectTransform>().SetAsLastSibling();
                    Tabs[i].GetComponent<Image>().color = GlobalTheme.ThemeContrastColor(themeVal);
                    Tabs[i].GetComponentInChildren<Text>().color = GlobalTheme.ThemeColor(themeVal);
                }
                else
                {
                    Panels[i].GetComponent<CanvasGroup>().alpha = 0;
                    Tabs[i].GetComponent<Image>().color = GlobalTheme.ThemeColor(themeVal);
                    Tabs[i].GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor(themeVal);
                }
            }
        }

        public void KeySetButtonClick(int number)
        {
            for(int i = 0; i < 6; i++)
            {
                if(i.Equals(number - 1))
                {
                    btn[i].GetComponent<Image>().color = GlobalTheme.ThemeColor(themeVal);
                    btn[i].GetComponentInChildren<Text>().text = LocaleManager.instance.GetLocaleText("setting_keybtn");
                    btn[i].GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor(themeVal);
                    selectedLine = number;
                }
                else
                {
                    btn[i].GetComponent<Image>().color = new Color32(0, 0, 0, 255);
                    btn[i].GetComponentInChildren<Text>().text = gameKeys[i].ToString();
                    btn[i].GetComponentInChildren<Text>().color = GlobalTheme.ThemeColor();
                }
            }
            KeyInputWaiting = true;
        }

        public void SetKeyCode(KeyCode key, int number)
        {
            gameKeys[number - 1] = key;
            btn[number - 1].GetComponent<Image>().color = new Color32(0, 0, 0, 255);
            btn[number - 1].GetComponentInChildren<Text>().text = gameKeys[number - 1].ToString();
            btn[number - 1].GetComponentInChildren<Text>().color = GlobalTheme.ThemeColor(themeVal);
            KeyInputWaiting = false;
        }

        public void SetGameSpeed(float value)
        {
            gameSpeed = value;
        }

        public void SetGameSync(float value)
        {
            gameSync = value;
        }

        public void SetStorage(string value)
        {
            if (Application.platform.Equals(RuntimePlatform.Android)) { storageSet = value; }
        }

        public void SetHitSound()
        {
            if (hitSoundTog.isOn) { hitSound = "true"; }
            else { hitSound = "false"; }
        }

        public void Set2LTheater()
        {
            if (T2Portrait.isOn) { T2ScreenVal = 0; }
            else if (T2Landscape.isOn) { T2ScreenVal = 1; }
        }

        public void SetTinyTip()
        {
            if (tinyTipTog.isOn) { tinyTipVal = 0; }
            else { tinyTipVal = 1; }
        }

        public void SetFixRes()
        {
            if (fixResTog.isOn) { fixResVal = 0; }
            else { fixResVal = 1; }
        }

        public void SetMusicVol(float value)
        {
            musicVol = value;
        }

        public void SetSEVol(float value)
        {
            seVolume = value;
        }

        public void SetTempestic()
        {
            if (tpstTog.isOn) { tempestic = "true"; }
            else { tempestic = "false"; }
        }

        public void SetFlickGroup()
        {
            if (flickLoose.isOn) { flickValue = 0; }
            else if (flickNormal.isOn) { flickValue = 1; }
            else if (flickStrict.isOn) { flickValue = 2; }
            else if (flickDynamic.isOn) { flickValue = 3; }
        }

        public void SetCanvasNote()
        {
            if (canvasNote.isOn) { canvasVal = 0; }
            else { canvasVal = 1; }
        }

        public void SetTheme()
        {
            for(int i = 0; i < themeToggle.Length; i++)
                if(themeToggle[i].isOn)
                {
                    themeVal = i;
                    themeName.text = GlobalTheme.ThemeName((ThemeType)i);
                    break;
                }
            ThemeApplier.UpdateTheme((ThemeType)themeVal);
        }

        public void SetResolution()
        {
            if(Application.platform.Equals(RuntimePlatform.WindowsPlayer))
            {
                if (screenSelector.value.Equals(0))
                {
                    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullScreen.isOn);
                    PlayerPrefs.SetInt("screenwidth", Screen.currentResolution.width);
                    PlayerPrefs.SetInt("screenheight", Screen.currentResolution.height);
                    scalecontrol.SetScale(Screen.currentResolution.width, Screen.currentResolution.height);
                }
                else if (screenSelector.value.Equals(1))
                {
                    Screen.SetResolution(1024, 768, fullScreen.isOn);
                    PlayerPrefs.SetInt("screenwidth", 1024);
                    PlayerPrefs.SetInt("screenheight", 768);
                    scalecontrol.SetScale(1024, 768);
                }
                else if (screenSelector.value.Equals(2))
                {
                    Screen.SetResolution(1280, 720, fullScreen.isOn);
                    PlayerPrefs.SetInt("screenwidth", 1280);
                    PlayerPrefs.SetInt("screenheight", 720);
                    scalecontrol.SetScale(1280, 720);
                }
                else if (screenSelector.value.Equals(3))
                {
                    Screen.SetResolution(1366, 768, fullScreen.isOn);
                    PlayerPrefs.SetInt("screenwidth", 1366);
                    PlayerPrefs.SetInt("screenheight", 768);
                    scalecontrol.SetScale(1366, 768);
                }
                else if (screenSelector.value.Equals(4))
                {
                    Screen.SetResolution(1600, 900, fullScreen.isOn);
                    PlayerPrefs.SetInt("screenwidth", 1600);
                    PlayerPrefs.SetInt("screenheight", 900);
                    scalecontrol.SetScale(1600, 900);
                }
                else if (screenSelector.value.Equals(5))
                {
                    Screen.SetResolution(1680, 1050, fullScreen.isOn);
                    PlayerPrefs.SetInt("screenwidth", 1680);
                    PlayerPrefs.SetInt("screenheight", 1050);
                    scalecontrol.SetScale(1680, 1050);
                }
                else if (screenSelector.value.Equals(6))
                {
                    Screen.SetResolution(1920, 1080, fullScreen.isOn);
                    PlayerPrefs.SetInt("screenwidth", 1920);
                    PlayerPrefs.SetInt("screenheight", 1080);
                    scalecontrol.SetScale(1920, 1080);
                }
                else if(screenSelector.value.Equals(7))
                {
                    Screen.SetResolution(1440, 1080, fullScreen.isOn);
                    PlayerPrefs.SetInt("screenwidth", 1440);
                    PlayerPrefs.SetInt("screenheight", 1080);
                    scalecontrol.SetScale(1440, 1080);
                }
                if (fullScreen.isOn) { PlayerPrefs.SetInt("fullscreen", 0); }
                else { PlayerPrefs.SetInt("fullscreen", 1); }
                PlayerPrefs.SetInt("screenselect", screenSelector.value);
            }
        }

        public void SetLanguage(int value)
        {
            LanguageLoader.LoadInternalLanguages(LocaleManager.instance);
            LanguageLoader.LoadExternalLanguages(LocaleManager.instance);
            LocaleManager.instance.PrepareInit += AfterLangChange;
            LocaleManager.SelectLocale(false);
        }

        private void AfterLangChange()
        {
            langCode = LocaleManager.instance.CurrentIndex;
            LocaleManager.instance.PrepareInit -= AfterLangChange;

            if (Social.localUser.authenticated)
            {
                if (Application.platform.Equals(RuntimePlatform.Android)) { socialTextAnd.text = LocaleManager.instance.GetLocaleText("setting_playgameslogged"); }
                else if (Application.platform.Equals(RuntimePlatform.IPhonePlayer)) { socialTextiOS.text = LocaleManager.instance.GetLocaleText("setting_gamecenterlogged"); }
            }
            else
            {
                if(Application.platform.Equals(RuntimePlatform.Android)) { socialTextAnd.text = LocaleManager.instance.GetLocaleText("setting_notloggedin"); }
                else if (Application.platform.Equals(RuntimePlatform.IPhonePlayer)) { socialTextiOS.text = LocaleManager.instance.GetLocaleText("setting_notloggedin"); }
            }
        }

        public void ApplyChanges(Text saveStat)
        {
            for (int i = 1; i <= 6; i++)
            {
                PlayerPrefs.SetInt("key" + i.ToString(), (int)gameKeys[i - 1]);
            }
            PlayerPrefs.SetFloat("gamespeed", gameSpeed);
            PlayerPrefs.SetFloat("gamesync", gameSync);
            PlayerPrefs.SetFloat("musicvol", musicVol);
            PlayerPrefs.SetFloat("sevol", seVolume);
            PlayerPrefs.SetString("storage", storageSet);
            PlayerPrefs.SetString("hitsound", hitSound);
            PlayerPrefs.SetString("tempestic", tempestic);
            PlayerPrefs.SetInt("flickmode", flickValue);
            PlayerPrefs.SetInt("canvasnote", canvasVal);
            PlayerPrefs.SetInt("locale", langCode);
            PlayerPrefs.SetInt("theater2", T2ScreenVal);
            PlayerPrefs.SetInt("tinytip", tinyTipVal);
            PlayerPrefs.SetInt("theme", themeVal);
            PlayerPrefs.SetInt("720p", fixResVal);
            if (Application.platform.Equals(RuntimePlatform.Android)) { PlayerPrefs.SetString("sdcard", SDval.text); }
            PlayerPrefs.Save();
            speedConfig.CheckError();
        }

        public void ShowAchievements()
        {
            if(!Social.localUser.authenticated)
            {
                Social.localUser.Authenticate((bool success) =>
                {
                    if (success)
                    {
                        if (Application.platform.Equals(RuntimePlatform.Android))
                        {
                            socialTextAnd.text = LocaleManager.instance.GetLocaleText("setting_playgameslogged");
                            socialTextAnd.color = GlobalTheme.ThemeColor(RuntimePlatform.Android);
                        }
                        else if (Application.platform.Equals(RuntimePlatform.IPhonePlayer))
                        {
                            socialTextiOS.text = LocaleManager.instance.GetLocaleText("setting_gamecenterlogged");
                            socialTextiOS.color = GlobalTheme.ThemeColor(RuntimePlatform.IPhonePlayer);
                        }
                        Social.ShowAchievementsUI();
                        return;
                    }
                    else { return; }
                });
            }
            Social.ShowAchievementsUI();
        }
    }
}
