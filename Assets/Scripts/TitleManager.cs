using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TempestWave.Core;
using TempestWave.Core.UI;
using TempestWave.Data;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#elif UNITY_IOS
using UnityEngine.SocialPlatforms;
#endif

namespace TempestWave
{
    public class TitleManager : MonoBehaviour
    {
        public Text verText, rmText, randText, exitAlertText;
        public TextAsset[] readMe = new TextAsset[2];
        public Animator sceneAnim;
        public GameObject langPanel, buttonPanel;
        public SceneChanger changer;
        // Use this for initialization
        void Start()
        {
            Application.targetFrameRate = 60;

            int fullsc;
            int wid, hei;
            RuntimePlatform pf = Application.platform;
            verText.text += Application.version;
            if (pf.Equals(RuntimePlatform.WindowsPlayer))
            {
                if (PlayerPrefs.HasKey("fullscreen")) { fullsc = PlayerPrefs.GetInt("fullscreen"); }
                else { PlayerPrefs.SetInt("fullscreen", 1); fullsc = 1; }
                if (PlayerPrefs.HasKey("screenwidth")) { wid = PlayerPrefs.GetInt("screenwidth"); }
                else { PlayerPrefs.SetInt("screenwidth", 1280); wid = 1280; }
                if (PlayerPrefs.HasKey("screenheight")) { hei = PlayerPrefs.GetInt("screenheight"); }
                else { PlayerPrefs.SetInt("screenheight", 720); hei = 720; }
                if (PlayerPrefs.HasKey("screenselect").Equals(false)) { PlayerPrefs.SetInt("screenselect", 2); }
                Screen.SetResolution(wid, hei, fullsc.Equals(0) ? true : false);
            }

            if (LocaleManager.instance.LocaleExists) { AfterLocaleLoad(); }
            else
            {
                LocaleManager.instance.PrepareInit += AfterLocaleLoad;
                LocaleManager.instance.LoadLocale();
            }

            // using social feature
#if UNITY_ANDROID
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
            PlayGamesPlatform.InitializeInstance(config);
            //PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
#elif UNITY_IOS
            UnityEngine.SocialPlatforms.GameCenter.GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
#endif

            if (!Social.localUser.authenticated) { SocialSignIn(); }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
        }

        private void AfterLocaleLoad()
        {
            buttonPanel.SetActive(true);
            int seed = Random.Range(1, 18);
            randText.text = LocaleManager.instance.GetLocaleText("titlescreen_tiptext" + seed.ToString());
            randText.gameObject.SetActive(true);
            LocaleManager.instance.PrepareInit -= AfterLocaleLoad;
        }

        private void SocialSignIn()
        {
            Social.localUser.Authenticate((bool success) => { });
        }

        int endCount = 0;

        IEnumerator TryExit()
        {
            exitAlertText.gameObject.SetActive(true);
            yield return new WaitForSeconds(3);
            endCount--;
            exitAlertText.gameObject.SetActive(false);
        }

        public void CallForExit()
        {
            if (endCount >= 1) { Application.Quit(); }
            else if(endCount.Equals(0))
            {
                endCount++;
                StartCoroutine(TryExit());
            }
        }

        public void ShowInformation()
        {
            string source = LocaleManager.instance.GetLocaleText("information_body");
            string final = source.Replace("?", Application.version).Replace("#", GlobalTheme.Platform());
            MessageBox.Show(LocaleManager.instance.GetLocaleText("information_head"), final, MessageBoxButton.OK);
        }

        public void OpenInstruction()
        {
            if (LocaleManager.instance.CurrentIndex.Equals(0)) { Application.OpenURL("https://drive.google.com/open?id=0B4lmwqLbQ-YKQ0xkWXh2VG50UjA"); }
            else { Application.OpenURL("https://drive.google.com/open?id=0B4lmwqLbQ-YKb2U2YWFkTUUyVWc"); }
#if UNITY_ANDROID
            if (Social.localUser.authenticated) { PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_reading_the_field_manual, 100.0f, null); }
#elif UNITY_IOS
            Achievementer.ReportProgress("ReadTheFieldManual");
#endif
        }
    }
}

