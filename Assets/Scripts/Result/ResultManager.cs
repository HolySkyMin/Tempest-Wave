using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TempestWave.Core;
using TempestWave.Core.UI;
using TempestWave.Data;
using TempestWave.Ingame;
#if UNITY_ANDROID
using GooglePlayGames;
#elif UNITY_IOS
using UnityEngine.SocialPlatforms;
#endif

namespace TempestWave.Result
{
    public class ResultManager : MonoBehaviour
    {
        public Text song, all, perf, tpst, great, nice, bad, miss, totcom, totsco, perc, gameMode, level;
        public GameObject fc, ap, rt, auto;
        public SceneChanger Changer;
        private GameDataArchiver data;
        private int[] j = new int[6];
        private int allNote, sc;
        private float pc;
        private bool autoplay, isFC, isAP, isRT;

        // Use this for initialization
        void Start()
        {
            data = DataSender.ReturnGameResult();
            autoplay = DataSender.ReturnAutoPlay();
            j = data.GetAllNoteDecision();
            allNote = data.GetNoteCount();
            sc = data.GetMaxScore();
            pc = data.GetMaxPercent();

            song.text = DataSender.ReturnSongName();
            miss.text = j[0].ToString();
            bad.text = j[1].ToString();
            nice.text = j[2].ToString();
            great.text = j[3].ToString();
            perf.text = (j[4] + j[5]).ToString();
            all.text = allNote.ToString();
            totcom.text = data.GetMaxCombo().ToString();
            if (j[5] > 0)
            {
                tpst.gameObject.SetActive(true);
                tpst.text = j[5].ToString();
            }
            totsco.text = sc.ToString();
            perc.text = pc.ToString("N2") + "%";

            if (data.GetMaxCombo() >= allNote)
            {
                isFC = true;
                if ((j[4] + j[5]) >= allNote)
                {
                    isAP = true;
                    if ((j[4] + j[5]).Equals(j[5]))
                    {
                        isRT = true;
                    }
                }
            }

            if(!autoplay && allNote >= 500 && isFC && Social.localUser.authenticated)
            {
#if UNITY_ANDROID
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_sweety_fruition, 100.0f, null);
                PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_marginal_utility, 1, null);
                PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_voracity, 1, null);
#elif UNITY_IOS
                Achievementer.ReportProgress("sweetyfruit");
                Achievementer.ReportProgress("marginalutility", 5.0f);
                Achievementer.ReportProgress("voracity", 2.0f);
#endif
            }
            if(!autoplay && allNote >= 500 && isAP && Social.localUser.authenticated)
            {
#if UNITY_ANDROID
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_perfect_itself, 100.0f, null);
                PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_admirable, 1, null);
#elif UNITY_IOS
                Achievementer.ReportProgress("perfectitself");
                Achievementer.ReportProgress("admirable", 5.0f);
#endif
            }
            if(!autoplay && allNote >= 500 && isRT && Social.localUser.authenticated)
            {
#if UNITY_ANDROID
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_you_are_really_a_monster, 100.0f, null);
#elif UNITY_IOS
                Achievementer.ReportProgress("youaremonster");
#endif
            }
            if(!autoplay && allNote >= 300 && (j[0] + j[1] + j[2]).Equals(1) && Social.localUser.authenticated)
            {
#if UNITY_ANDROID
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_its_ok_dont_be_depressed, 100.0f, null);
#elif UNITY_IOS
                Achievementer.ReportProgress("itsok");
#endif
                if (j[2].Equals(1))
                {
#if UNITY_ANDROID
                    PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_discovering_science, 1, null);
                    PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_science_theory, 1, null);
                    PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_should_earn_a_nobel_prize, 1, null);
#elif UNITY_IOS
                    Achievementer.ReportProgress("science", 34.0f);
                    Achievementer.ReportProgress("theory", 10.0f);
                    Achievementer.ReportProgress("nobelprize", 4.0f);
#endif
                }
            }

            string tsl, tff, tsa; // tsl만 쓰임
            bool tap, trw, tmi;
            DataSender.ResultPopOut(out tsl, out tff, out tap, out trw, out tmi, out tsa);
            float speed, sync;
            bool hit, tpstb;
            int flick;
            data.ResultPopOut(out speed, out sync, out hit, out tpstb, out flick);

            if (DataSender.ReturnGameMode().Equals(GameMode.Starlight)) { gameMode.text = "5L STARLIGHT"; }
            else if (DataSender.ReturnGameMode().Equals(GameMode.Theater2L)) { gameMode.text = "2L THEATER (L)"; }
            else if (DataSender.ReturnGameMode().Equals(GameMode.Theater2P)) { gameMode.text = "2L THEATER (P)"; }
            else if (DataSender.ReturnGameMode().Equals(GameMode.Theater4)) { gameMode.text = "4L THEATER"; }
            else if (DataSender.ReturnGameMode().Equals(GameMode.Theater)) { gameMode.text = "6L THEATER"; }
            else if (DataSender.ReturnGameMode().Equals(GameMode.Platinum)) { gameMode.text = "1L PLATINUM"; }
            level.text = tsl;

            GameMode mode = DataSender.ReturnGameMode();
            if(!autoplay && mode.Equals(GameMode.Starlight) && Social.localUser.authenticated)
            {
#if UNITY_ANDROID
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_seeing_the_starlight_at_the_stage, 100.0f, null);
#elif UNITY_IOS
                Achievementer.ReportProgress("StarlightStage");
#endif
            }
            else if(!autoplay && (mode.Equals(GameMode.Theater) || mode.Equals(GameMode.Theater4) || mode.Equals(GameMode.Theater2L) || mode.Equals(GameMode.Theater2P)) && Social.localUser.authenticated)
            {
#if UNITY_ANDROID
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_nice_days_to_go_to_the_theater, 100.0f, null);
#elif UNITY_IOS
                Achievementer.ReportProgress("TheaterDays");
#endif
            }
            else if(!autoplay && mode.Equals(GameMode.Platinum) && Social.localUser.authenticated)
            {
#if UNITY_ANDROID
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_platinum_on_stage, 100.0f, null);
#elif UNITY_IOS
                Achievementer.ReportProgress("PlatinumOnStage");
#endif
            }
            if(!autoplay && allNote >= 300 && pc > 100 && Social.localUser.authenticated)
            {
#if UNITY_ANDROID
                if (tpstb) { PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_percentile_ignored, 100.0f, null); }
                else { PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_should_not_be_happened, 100.0f, null); }
#elif UNITY_IOS
                if (tpstb) { Achievementer.ReportProgress("percentile"); }
                else { Achievementer.ReportProgress("notbehappened"); }
#endif
            }

            if (Social.localUser.authenticated)
            {
#if UNITY_ANDROID
                PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_thank_you, 1, null);
#elif UNITY_IOS
                Achievementer.ReportProgress("thankyou", 1.0f);
#endif
            }
            StartCoroutine(ShowAchieveText());
        }

        IEnumerator ShowAchieveText()
        {
            yield return new WaitForSeconds(2);
            if (isFC) { fc.SetActive(true); }
            if (isAP) { ap.SetActive(true); }
            if (isRT) { rt.SetActive(true); }
            if (autoplay) { auto.SetActive(true); }
        }

        public void ShowGameInfo()
        {
            string BodyText = "";

            string tsl, tff, tsa; // tsl, tff는 쓰이지 않음
            bool tap, trw, tmi;
            DataSender.ResultPopOut(out tsl, out tff, out tap, out trw, out tmi, out tsa);
            float speed, sync;
            bool hit, tpst;
            int flick;
            data.ResultPopOut(out speed, out sync, out hit, out tpst, out flick);

            BodyText += (LocaleManager.instance.GetLocaleText("result_platform") + ": ");
            BodyText += GlobalTheme.Platform();
            BodyText += Environment.NewLine;
            BodyText += (LocaleManager.instance.GetLocaleText("selectsong_autoplay") + ": ");
            if (tap.Equals(true)) { BodyText += LocaleManager.instance.GetLocaleText("result_on"); }
            else { BodyText += LocaleManager.instance.GetLocaleText("result_off"); }
            BodyText += Environment.NewLine;
            BodyText += (LocaleManager.instance.GetLocaleText("selectsong_mirror") + ": ");
            if (tmi.Equals(true)) { BodyText += LocaleManager.instance.GetLocaleText("result_on"); }
            else { BodyText += LocaleManager.instance.GetLocaleText("result_off"); }
            BodyText += Environment.NewLine;
            BodyText += (LocaleManager.instance.GetLocaleText("selectsong_scrollamp") + ": x" + tsa);
            BodyText += Environment.NewLine;
            BodyText += (LocaleManager.instance.GetLocaleText("result_notespeed") + ": " + speed.ToString());
            BodyText += Environment.NewLine;
            BodyText += (LocaleManager.instance.GetLocaleText("result_gamesync") + ": " + sync.ToString());
            BodyText += Environment.NewLine;
            BodyText += (LocaleManager.instance.GetLocaleText("selectsong_randwave") + ": ");
            if (trw.Equals(true)) { BodyText += LocaleManager.instance.GetLocaleText("result_on"); }
            else { BodyText += LocaleManager.instance.GetLocaleText("result_off"); }
            BodyText += Environment.NewLine;
            BodyText += (LocaleManager.instance.GetLocaleText("setting_tempesticset") + ": ");
            if (tpst) { BodyText += LocaleManager.instance.GetLocaleText("result_activated"); }
            else { BodyText += LocaleManager.instance.GetLocaleText("result_notactivated"); }
            BodyText += Environment.NewLine;
            BodyText += (LocaleManager.instance.GetLocaleText("setting_flicksensitiveset") + ": ");
            if (flick.Equals(0)) { BodyText += LocaleManager.instance.GetLocaleText("result_flickloose"); }
            else if (flick.Equals(1)) { BodyText += LocaleManager.instance.GetLocaleText("result_flicknormal"); }
            else if (flick.Equals(2)) { BodyText += LocaleManager.instance.GetLocaleText("result_flickstrict"); }
            else if (flick.Equals(3)) { BodyText += LocaleManager.instance.GetLocaleText("result_flickdynamic"); }

            MessageBox.Show(LocaleManager.instance.GetLocaleText("result_gameinfo"), BodyText, MessageBoxButton.OK);
        }

        public void RestartGame()
        {
            if (DataSender.ReturnGameMode().Equals(GameMode.Theater2L)) { Changer.NoAsyncChange("2LTheaterP"); }
            else { Changer.NoAsyncChange("6LineGame"); }
        }
    }
}
