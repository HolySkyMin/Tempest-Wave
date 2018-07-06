using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TempestWave.Core;
using TempestWave.Core.UI;
using TempestWave.Data;
using TempestWave.Monitorer;
#if UNITY_ANDROID
using GooglePlayGames;
#endif

namespace TempestWave.Ingame
{
    public class GameManager : MonoBehaviour
    {
        public float Frame { get; set; }
        public float FrameFixed { get; set; }
        public float FrameSpeed { get; set; }
        public float GlobalNoteSpeed { get; set; }
        public float SecuredNoteSpeed { get; set; }
        public float GameSync { get; set; }
        public float ScrollAmp { get; set; }
        public float SongTime { get; set; }
        public float SongDelay { get; set; }
        public bool IsPlaying { get; set; }
        public bool IsStarted { get; set; }
        public bool IsEnded { get; set; }
        public bool Paused { get; set; }
        public bool IsAutoPlay { get; set; }
        public bool IsNoteColored { get; set; }
        public bool HasError { get; set; }
        public Vector3[] StartPos { get; set; }
        public Vector3[] EndPos { get; set; }
        public KeyCode[] Keys { get; set; }
        public GameMode Mode { get; set; }
        public NoteManager Dispensor { get; set; }
        public BeatmapParser Parser { get; set; }
        public List<GameObject> Trashcan { get; set; }

        public GameObject[] HitEffect = new GameObject[6];
        public GameObject HitXL, SystemNote, StarlightNote, StarlightXL, TheaterSmall, TheaterLarge, TheaterXL, PlatinumSmall, PlatinumLarge, PlatinumXL, BaseMultiLine, BaseTail, BaseConnector;
        public RectTransform NoteParent;
        public Sprite[] NoteTexture = new Sprite[10], WhiteTexture = new Sprite[10], StarlightTexture = new Sprite[10], StarlightWhite = new Sprite[10];
        public GameObject AutoPanel;
        public Text ErrorText, StartCountText, FullCombo;
        public GameObject StartPanel, EndPanel, PausePanel, StarlightPanel, Theater6LPanel, Theater4LPanel, Theater2LPanel, PlatinumPanel;
        public Button PauseButton;
        public ImprovedJudge Judger;
        public AudioSource MusicFile;
        public Animator SceneAnimate;
        public SongPlayer Player;
        public BGAManager BGA;
        public GameDataArchiver DataContainer;
        public SceneChanger changer;

        private int PreservedWidth, PreservedHeight;
        public bool IsNoMusic;
        private bool ScreenAdjusted = false;

        public override string ToString()
        {
            return "Game Manager - Mode: " + Mode.ToString();
        }


        private void Awake()
        {
            Mode = DataSender.ReturnGameMode();
            ScrollAmp = DataSender.ReturnSpeedAmp();
            IsAutoPlay = DataSender.ReturnAutoPlay();
            IsNoMusic = DataSender.ReturnMusicNotPlay();
            IsEnded = false;
            IsStarted = false;
            IsPlaying = false;
            HasError = false;
            Dispensor = new NoteManager(this);
            Parser = new BeatmapParser(this);
            Trashcan = new List<GameObject>();
            StartPos = new Vector3[8];
            EndPos = new Vector3[6];
            Keys = new KeyCode[6];

            if (Mode.Equals(GameMode.Theater2P))
            {
                if (Application.platform.Equals(RuntimePlatform.Android) || Application.platform.Equals(RuntimePlatform.IPhonePlayer))
                {
                    Screen.orientation = ScreenOrientation.Portrait;
                }
                else if (Application.platform.Equals(RuntimePlatform.WindowsPlayer))
                {
                    PreservedWidth = Screen.width;
                    PreservedHeight = Screen.height;
                    Screen.SetResolution((int)(Screen.height * (9f / 16f)), Screen.height, Screen.fullScreen);
                }
            }

            if (PlayerPrefs.HasKey("gamespeed")) { GlobalNoteSpeed = PlayerPrefs.GetFloat("gamespeed"); }
            else { GlobalNoteSpeed = 2f; }
            SecuredNoteSpeed = GlobalNoteSpeed;
            if (PlayerPrefs.HasKey("gamesync")) { GameSync = PlayerPrefs.GetFloat("gamesync"); }
            else { GameSync = 0; }
            if (PlayerPrefs.HasKey("flickmode").Equals(true)) { DataContainer.SetFlickMode(PlayerPrefs.GetInt("flickmode")); }
            else { DataContainer.SetFlickMode(1); }
            if (PlayerPrefs.HasKey("canvasnote") && PlayerPrefs.GetInt("canvasnote").Equals(0)) { IsNoteColored = true; }
            else { IsNoteColored = false; }
            DataContainer.SetNoteSpeed(SecuredNoteSpeed * 10);
            DataContainer.SetGameSync(GameSync);
            Frame = GameSync * (1 / ScrollAmp);
            FrameFixed = Frame;
            FrameSpeed = 1;

            if (PlayerPrefs.HasKey("720p") && PlayerPrefs.GetInt("720p").Equals(0))
            {
                if (Screen.width >= 1280 && Screen.height >= 720)
                {
                    PreservedWidth = Screen.width;
                    PreservedHeight = Screen.height;
                    if (Mode.Equals(GameMode.Theater2P)) { Screen.SetResolution(720, Mathf.RoundToInt(720 * (PreservedHeight / (float)PreservedWidth)), Screen.fullScreen); }
                    else { Screen.SetResolution(Mathf.RoundToInt(720 * (PreservedWidth / (float)PreservedHeight)), 720, Screen.fullScreen); }
                    ScreenAdjusted = true;
                }
            }

            for(int i = 0; i < 6; i++)
            {
                if (PlayerPrefs.HasKey("key" + (i + 1).ToString())) { Keys[i] = (KeyCode)PlayerPrefs.GetInt("key" + (i + 1).ToString()); }
                else { Keys[i] = KeyCode.None; }
            }

            if (Mode.Equals(GameMode.Starlight))
            {
                StarlightPanel.SetActive(true);
                for (int i = 0; i < 6; i++)
                {
                    HitEffect[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-392 + 196 * i, -238);
                }
            }
            else if (Mode.Equals(GameMode.Theater)) { Theater6LPanel.SetActive(true); }
            else if (Mode.Equals(GameMode.Theater4))
            {
                Theater4LPanel.SetActive(true);
                for (int i = 0; i < 6; i++)
                {
                    HitEffect[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-393 + 262 * i, -240);
                }
            }
            else if (Mode.Equals(GameMode.Theater2L))
            {
                Theater2LPanel.SetActive(true);
                for (int i = 0; i < 6; i++)
                {
                    HitEffect[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-238 + 476 * i, -240);
                }
            }
            else if(Mode.Equals(GameMode.Platinum))
            {
                PlatinumPanel.SetActive(true);
                for (int i = 0; i < 6; i++)
                {
                    HitEffect[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -245);
                }
            }
        }

        private void Start()
        {
            Parser.ParseBeatmap();
            Dispensor.CreateNote(Dispensor.NoteCount, 0, Color.white, NoteInfo.SystemNoteEnder, FlickMode.None, Dispensor.Notes[Dispensor.NoteCount - 1].ReachFrame * ScrollAmp, Dispensor.Notes[Dispensor.NoteCount - 1].Speed, 1, 1, new List<int>());
            Player.LoadMusic(ref IsNoMusic);
            if (IsAutoPlay) { AutoPanel.SetActive(true); }
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            BGA.Initialize();
        }

        private float StartCount = 0, EndCount = 0;
        private bool FCFlag = false;

        private void Update()
        {
            if(IsStarted && !Paused)
            {
                Dispensor.UpdateNote(Frame);
                Frame += 60 * Time.deltaTime * FrameSpeed;
                FrameFixed += 60 * Time.deltaTime;
                if (IsPlaying) { SongTime += 1 * Time.deltaTime; }
            }
            else if(!IsStarted && !HasError && (!BGA.isCustom || (BGA.isCustom && BGA.readyToPlay)))
            {
                StartPanel.SetActive(true);
                StartCount += 1 * Time.deltaTime;
                StartCountText.text = Mathf.Ceil(3 - StartCount).ToString();
#if UNITY_ANDROID
                if (Social.localUser.authenticated) { PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_your_first_step, 100.0f, null); }
#elif UNITY_IOS
                Achievementer.ReportProgress("YourFirstStep");
#endif
                if(StartCount >= 3f)
                {
                    PauseButton.interactable = true;
                    DataContainer.SetNoteCount(Dispensor.VisibleNoteCount);
                    IsStarted = true;
                    BGA.ResumeBGA();
                    BGA.SetStartFrame();
                    StartPanel.SetActive(false);
                }
            }
            if(IsEnded)
            {
                PauseButton.interactable = false;
                EndCount += 1 * Time.deltaTime;
                if(EndCount >= 3 && !EndPanel.activeSelf) { EndPanel.SetActive(true); }
                if(EndCount >= 6)
                {
                    DataSender.SetGameResult(DataContainer);
                    SceneAnimate.Play("6LineCvFadeOut");
                    Screen.sleepTimeout = SleepTimeout.SystemSetting;
                    if (ScreenAdjusted) { Screen.SetResolution(PreservedWidth, PreservedHeight, Screen.fullScreen); }
                    if (Mode.Equals(GameMode.Theater2P))
                    {
                        if (Application.platform.Equals(RuntimePlatform.Android) || Application.platform.Equals(RuntimePlatform.IPhonePlayer))
                        {
                            Screen.orientation = ScreenOrientation.Landscape;
                            Screen.orientation = ScreenOrientation.AutoRotation;
                        }
                        else if (Application.platform.Equals(RuntimePlatform.WindowsPlayer)) { Screen.SetResolution(PreservedWidth, PreservedHeight, Screen.fullScreen); }
                    }
                    changer.ChangeToScene("GameResult", 0.5f);
                }
            }
            if(IsEnded && DataContainer.GetMaxCombo() >= Dispensor.VisibleNoteCount && !FCFlag)
            {
                StartCoroutine(FullComboShow());
                FCFlag = true;
            }
        }

        IEnumerator FullComboShow()
        {
            FullCombo.gameObject.SetActive(true);
            SceneAnimate.Play("Theater6_FCText");
            yield return new WaitForSeconds(1);
            FullCombo.gameObject.SetActive(false);
        }

        private void OnApplicationPause(bool pause)
        {
            if (IsStarted && !IsEnded && pause && !Paused) { GamePause(); }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (IsStarted && !IsEnded && !focus && !Paused) { GamePause(); }
        }

        public void End(string SceneName)
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            if (ScreenAdjusted) { Screen.SetResolution(PreservedWidth, PreservedHeight, Screen.fullScreen); }
            if (Mode.Equals(GameMode.Theater2P))
            {
                if (Application.platform.Equals(RuntimePlatform.Android) || Application.platform.Equals(RuntimePlatform.IPhonePlayer))
                {
                    Screen.orientation = ScreenOrientation.Landscape;
                    Screen.orientation = ScreenOrientation.AutoRotation;
                }
                else if (Application.platform.Equals(RuntimePlatform.WindowsPlayer)) { Screen.SetResolution(PreservedWidth, PreservedHeight, Screen.fullScreen); }
            }
            changer.ChangeToScene(SceneName, 0.5f);
        }

        public void GamePause()
        {
            MusicFile.Pause();
            BGA.PauseBGA();
            Paused = true;
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            StartCoroutine(RootPause());
        }

        IEnumerator RootPause()
        {
            MessageBox.Show(LocaleManager.instance.GetLocaleText("ingame_pausetitle"), LocaleManager.instance.GetLocaleText("ingame_pausetext"), MessageBoxButton.YesNo, false);
            yield return StartCoroutine(FirstAsk());
        }

        IEnumerator FirstAsk()
        {
            yield return new WaitUntil(() => MessageBox.Instance.ResultExists == true);
            if(MessageBox.Instance.Result.Equals(MessageBoxButtonType.Yes))
            {
                yield return new WaitUntil(() => MessageBox.Instance.CompletelyEnded == true);
                MessageBox.Show(LocaleManager.instance.GetLocaleText("ingame_pausetitle"), LocaleManager.instance.GetLocaleText("ingame_pauseaskagain"), MessageBoxButton.YesNo, false);
                yield return StartCoroutine(SecondAsk());
            }
            else
            {
                MessageBox.Instance.ShouldBeErased = true;
                yield return new WaitUntil(() => MessageBox.Instance.CompletelyEnded == true);
                if (!IsNoMusic && IsPlaying)
                {
                    MusicFile.UnPause();
                    //Frame = (GameSync * (1 / ScrollAmp)) + (MusicFile.time * 60 * (1 / ScrollAmp));
                    MusicFile.time = SongTime * ScrollAmp;
                }
                BGA.ResumeBGA();
                Paused = false;
                PauseButton.interactable = true;
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }
        }

        IEnumerator SecondAsk()
        {
            yield return new WaitUntil(() => MessageBox.Instance.ResultExists == true);
            if(MessageBox.Instance.Result.Equals(MessageBoxButtonType.Yes))
            {
                MessageBox.Instance.ShouldBeErased = true;
                yield return new WaitUntil(() => MessageBox.Instance.CompletelyEnded == true);
                SceneAnimate.Play("6LineCvFadeOut");
                End("SelectSong");
            }
            else
            {
                yield return new WaitUntil(() => MessageBox.Instance.CompletelyEnded == true);
                yield return StartCoroutine(RootPause());
            }
        }

        public Sprite GetNoteTexture(NoteInfo mode, FlickMode flick)
        {
            Sprite text = null;
            if(Mode.Equals(GameMode.Starlight) || Mode.Equals(GameMode.Platinum))
            {
                if (!flick.Equals(FlickMode.None)) { text = Instantiate(IsNoteColored ? StarlightWhite[(int)flick] : StarlightTexture[(int)flick]) as Sprite; }
                else
                {
                    if (mode.Equals(NoteInfo.NormalNote)) { text = Instantiate(IsNoteColored ? StarlightWhite[0] : StarlightTexture[0]) as Sprite; }
                    else if (mode.Equals(NoteInfo.LongNoteStart) || mode.Equals(NoteInfo.LongNoteEnd)) { text = Instantiate(IsNoteColored ? StarlightWhite[5] : StarlightTexture[5]) as Sprite; }
                    else if (mode.Equals(NoteInfo.SlideNoteStart) || mode.Equals(NoteInfo.SlideNoteEnd) || mode.Equals(NoteInfo.SystemNoteSlideDummy)) { text = Instantiate(IsNoteColored ? StarlightWhite[6] : StarlightTexture[6]) as Sprite; }
                    else if (mode.Equals(NoteInfo.SlideNoteCheckpoint)) { text = Instantiate(IsNoteColored ? StarlightWhite[7] : StarlightTexture[7]) as Sprite; }
                    else if (mode.Equals(NoteInfo.DamageNote)) { text = Instantiate(IsNoteColored ? StarlightWhite[8] : StarlightTexture[8]) as Sprite; }
                }
            }
            else
            {
                if (!flick.Equals(FlickMode.None)) { text = Instantiate(IsNoteColored ? WhiteTexture[(int)flick] : NoteTexture[(int)flick]) as Sprite; }
                else
                {
                    if (mode.Equals(NoteInfo.NormalNote)) { text = Instantiate(IsNoteColored ? WhiteTexture[0] : NoteTexture[0]) as Sprite; }
                    else if (mode.Equals(NoteInfo.LongNoteStart) || mode.Equals(NoteInfo.LongNoteEnd)) { text = Instantiate(IsNoteColored ? WhiteTexture[5] : NoteTexture[5]) as Sprite; }
                    else if (mode.Equals(NoteInfo.SlideNoteStart) || mode.Equals(NoteInfo.SlideNoteEnd) || mode.Equals(NoteInfo.SystemNoteSlideDummy)) { text = Instantiate(IsNoteColored ? WhiteTexture[6] : NoteTexture[6]) as Sprite; }
                    else if (mode.Equals(NoteInfo.SlideNoteCheckpoint)) { text = Instantiate(IsNoteColored ? WhiteTexture[7] : NoteTexture[7]) as Sprite; }
                    else if (mode.Equals(NoteInfo.DamageNote)) { text = Instantiate(IsNoteColored ? WhiteTexture[8] : NoteTexture[8]) as Sprite; }
                }
            }
            return text;
        }

        public void ShortEffectPlay(int line, bool isXL)
        {
            if (isXL)
            {
                HitXL.SetActive(true);
                HitXL.GetComponent<Animator>().Play("noteEffect", 0, 0);
            }
            else
            {
                HitEffect[line - 1].SetActive(true);
                HitEffect[line - 1].GetComponent<Animator>().Play("noteEffect", 0, 0);
            }
        }

        public float GetNoteX(float startX, float endX, float curProgress)
        {
            float progress = (2 * curProgress) / (curProgress + 1);
            return startX + (endX - startX) * progress;
        }

        public float GetNoteScale(float curProgress)
        {
            if(curProgress <= 0.236909f)
            {
                float bet01 = curProgress / 0.236909f;
                return 0.33f * bet01;
            }
            else
            {
                float bet01 = (curProgress - 0.236909f) / (1f - 0.236909f);
                return 0.33f + 0.67f * bet01;
            }
        }

        public void ThrowError(ErrorMode errMode)
        {
            HasError = true;
            MessageBox.Show(LocaleManager.instance.GetLocaleText("error_occured"), ErrorManager.GetErrorText(errMode), MessageBoxButton.OK);
            StartCoroutine(MessageAct_ThrowError());
#if UNITY_ANDROID
            if (Social.localUser.authenticated) { PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_never_give_up, 1, null); }
#elif UNITY_IOS
            Achievementer.ReportProgress("nevergiveup", 20.0f);
#endif
        }

        public void ThrowError(ErrorMode errMode, string detail)
        {
            string bodyText = ErrorManager.GetErrorText(errMode) + Environment.NewLine + Environment.NewLine + LocaleManager.instance.GetLocaleText("error_additional") + detail;
            HasError = true;
            MessageBox.Show(LocaleManager.instance.GetLocaleText("error_occured"), bodyText, MessageBoxButton.OK);
            StartCoroutine(MessageAct_ThrowError());
#if UNITY_ANDROID
            if (Social.localUser.authenticated) { PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_never_give_up, 1, null); }
#elif UNITY_IOS
            Achievementer.ReportProgress("nevergiveup", 20.0f);
#endif
        }

        private IEnumerator MessageAct_ThrowError()
        {
            yield return new WaitUntil(() => MessageBox.Instance.CompletelyEnded == true);
            SceneAnimate.Play("6LineCvFadeOut");
            End("SelectSong");
        }
    }
}