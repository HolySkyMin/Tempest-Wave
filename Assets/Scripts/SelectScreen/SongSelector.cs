using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TempestWave.Core;
using TempestWave.Core.UI;
using TempestWave.Data;
using TempestWave.Monitorer;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_ANDROID
using GooglePlayGames;
#endif

namespace TempestWave.SelectScreen
{
    public class SongSelector : MonoBehaviour
    {
        private readonly string[] LevelText = { "easy", "normal", "hard", "apex" };

        private string selectedSong, selectedFolder, tpath, BGAPath, BackPath;
        private int CurrentIndex = 0, LevelIndex = 0, CurBGAVal, lastButtonIdx = -1;
        private float curScrollVal = 1;
        private bool
            autoPlay = false,
            randWave = false,
            noMusic = false,
            mirror = false,
            defBGA = false,
            SongSelectedPrevious = false,
            LevelSelectedPrevious = false,
            OptionShown = false;
        private List<string> ErrorDirList = new List<string>();

        private List<BeatmapInfo>[]
            Starlight5 = new List<BeatmapInfo>[4],
            Theater2 = new List<BeatmapInfo>[4],
            Theater4 = new List<BeatmapInfo>[4],
            Theater6 = new List<BeatmapInfo>[4],
            Platinum1 = new List<BeatmapInfo>[4];
        private List<BeatmapInfo>[] CurrentQueue = new List<BeatmapInfo>[4];
        private BeatmapInfo[] CurrentBeatmaps;

        private List<GameObject> buttons = new List<GameObject>();
        public GameObject listButton, listCore, TitlePanel, PeoplePanel, ModePanel, DensityPanel, OptionPanel, StartPanel, BeatmapIndexPanel, ErrorInfo;
        public GridLayoutGroup grid;
        public Text titleText, artistText, authorText, fileIndexText, densityText, errorText, searchTexts, scrollAmpText, densityMainText, densitySubText;
        public RawImage Jacket;
        public Button[]
            ModeBtn = new Button[4], // 0: 5L STARLIGHT, 1: 2L THEATER, 2: 4L THEATER, 3: 6L THEATER
            LevelBtn = new Button[4];
        public Button 
            AutoPlayBtn,
            RandWaveBtn,
            NoMusicBtn,
            MirrorBtn,
            DefBGABtn,
            startButton;
        public Slider scrollAmpSlider;
        public SceneChanger changer;
        public Animator animationObj, OptionAnim;
        public LevelText levText;

        // Use this for initialization
        void Start()
        {
            SearchLoad(searchTexts);

            for(int i = 0; i < 4; i++)
            {
                Starlight5[i] = new List<BeatmapInfo>();
                Theater2[i] = new List<BeatmapInfo>();
                Theater4[i] = new List<BeatmapInfo>();
                Theater6[i] = new List<BeatmapInfo>();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                animationObj.Play("SelectSong_FadeOutSimple");
                changer.ChangeToScene("TitleScreen", 1f / 3f);
            }
        }

        public void Reload()
        {
            SearchLoad(searchTexts);
        }

        public void SearchLoad(Text searchText)
        {
            string value = searchText.text;
            lastButtonIdx = -1;

            if (buttons.Count > 0)
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    buttons[i].SetActive(false);
                }
                buttons.Clear();
            }
            DirectoryInfo source = new DirectoryInfo(GamePath.SongPath());
            DirectoryInfo[] dir = source.GetDirectories();
            if (dir.Length.Equals(0))
            {
                ErrorManager.showErrorText(errorText.gameObject, ErrorMode.NoSong);
                return;
            }

            List<string> list = new List<string>();
            for (int i = 0; i < dir.Length; i++)
            {
                list.Add(dir[i].Name);
            }
            list.Sort();

            int chk; // 악곡 데이터가 정상적으로 입력되었는지를 알려주는 값.
            foreach (string dat in list)
            {
                GameObject go = Instantiate(listButton) as GameObject;
                go.SetActive(true);
                SongButton realBtn = go.GetComponent<SongButton>();
                chk = realBtn.SetSongName(dat, source.FullName + dat, value);
                //chk = realBtn.SetSong(dat, source.FullName + dat, value);                // 이거를 버튼 눌렀을 때로도 뺀다.
                realBtn.Index = buttons.Count;

                if (chk.Equals(1)) { go.SetActive(false); continue; }
                    else if (chk.Equals(-1))
                    {
                        if (!ErrorInfo.activeSelf) { ErrorInfo.SetActive(true); }
                        ErrorDirList.Add(dat);
                    }
                //여기까지
                go.transform.SetParent(listButton.transform.parent);
                go.transform.localScale = new Vector3(1, 1, 1); // 해상도에 따른 조정
                buttons.Add(go);
            }

            if(buttons.Count >= 30 && !ErrorInfo.activeSelf)
            {
#if UNITY_ANDROID
                if (Social.localUser.authenticated) { PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_pride_of_the_rich, 100.0f, null); }
#elif UNITY_IOS
                Achievementer.ReportProgress("prideoftherich");
#endif
            }
        }

        public void Selected(int index, string title, List<BeatmapInfo>[] St5, List<BeatmapInfo>[] Th2, List<BeatmapInfo>[] Th4, List<BeatmapInfo>[] Th6, List<BeatmapInfo>[] Pt1, bool[] filled, int bgaFrame, string bgaPath, string backPath)
        {
            Starlight5 = St5;
            Theater2 = Th2;
            Theater4 = Th4;
            Theater6 = Th6;
            Platinum1 = Pt1;
            CurBGAVal = bgaFrame;
            BGAPath = bgaPath;
            BackPath = backPath;

            titleText.text = title;
            artistText.text = null;
            authorText.text = null;
            for(int i = 0; i < 5; i++)
            {
                ModeBtn[i].interactable = false;
                ModeBtn[i].gameObject.GetComponent<Image>().color = Color.white;
                ModeBtn[i].gameObject.GetComponentInChildren<Text>().color = Color.black;
                if (filled[i]) { ModeBtn[i].interactable = true; }
            }
            for(int i = 0; i < 4; i++)
            {
                LevelBtn[i].interactable = false;
                LevelBtn[i].gameObject.GetComponent<Image>().color = GlobalTheme.ThemeColor();
                LevelBtn[i].gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
            }
            animationObj.Play("SelectSong_SongSelected", 0, 0);
            if (!TitlePanel.activeSelf) { TitlePanel.SetActive(true); }
            if (!PeoplePanel.activeSelf) { PeoplePanel.SetActive(true); }
            if (!ModePanel.activeSelf) { ModePanel.SetActive(true); }

            if (!SongSelectedPrevious)
            {
                OptionPanel.SetActive(true);
                SongSelectedPrevious = true;
            }
            LevelSelectedPrevious = false;
            if (BeatmapIndexPanel.activeSelf) { BeatmapIndexPanel.SetActive(false); }

            if(lastButtonIdx != -1)
            {
                buttons[lastButtonIdx].GetComponent<Graphic>().color = GlobalTheme.ThemeColor();
                buttons[lastButtonIdx].GetComponent<SongButton>().buttonText.color = GlobalTheme.ThemeContrastColor();
            }
            lastButtonIdx = index;
        }

        public void ModeClicked(int index)
        {
            if (index.Equals(1)) { CurrentQueue = Starlight5; }
            else if (index.Equals(2)) { CurrentQueue = Theater2; }
            else if (index.Equals(3)) { CurrentQueue = Theater4; }
            else if (index.Equals(4)) { CurrentQueue = Theater6; }
            else if (index.Equals(5)) { CurrentQueue = Platinum1; }

            for(int i = 0; i < 5; i++)
            {
                if(i.Equals(index - 1))
                {
                    ModeBtn[i].gameObject.GetComponent<Image>().color = GlobalTheme.ThemeColor();
                    ModeBtn[i].gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
                }
                else
                {
                    ModeBtn[i].gameObject.GetComponent<Image>().color = Color.white;
                    ModeBtn[i].gameObject.GetComponentInChildren<Text>().color = Color.black;
                }
            }

            for(int i = 0; i < 4; i++)
            {
                LevelBtn[i].interactable = false;
                LevelBtn[i].gameObject.GetComponent<Image>().color = GlobalTheme.ThemeColor();
                LevelBtn[i].gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
                if (CurrentQueue[i].Count > 0) { LevelBtn[i].interactable = true; }
            }

            if (LevelSelectedPrevious)
            {
                animationObj.Play("SelectSong_ModeReselected");
                LevelSelectedPrevious = false;
            }
            if (BeatmapIndexPanel.activeSelf) { BeatmapIndexPanel.SetActive(false); }
        }

        public void LevelSelected(int index)
        {
            CurrentBeatmaps = CurrentQueue[index].ToArray();
            LevelIndex = index;
            CurrentIndex = 0;
            if(CurrentBeatmaps.Length > 1) { BeatmapIndexPanel.SetActive(true); BeatmapIndexChanged(0); }
            else { BeatmapIndexPanel.SetActive(false); BeatmapIndexChanged(0); }
            LevelSelectedPrevious = true;

            for(int i = 0; i < 4; i++)
            {
                if(i.Equals(index))
                {
                    Color c = Color.white, t = Color.white;
                    if (index.Equals(0)) { c = Color.cyan; t = Color.black; }
                    else if (index.Equals(1)) { c = Color.yellow; t = Color.black; }
                    else if (index.Equals(2)) { c = Color.red; t = Color.white; }
                    else if (index.Equals(3)) { c = Color.magenta; t = Color.white; }
                    LevelBtn[i].gameObject.GetComponent<Image>().color = c;
                    LevelBtn[i].gameObject.GetComponentInChildren<Text>().color = t;
                    DensityPanel.GetComponent<Image>().color = c;
                    densityMainText.color = t;
                    densitySubText.color = t;
                }
                else
                {
                    LevelBtn[i].gameObject.GetComponent<Image>().color = GlobalTheme.ThemeColor();
                    LevelBtn[i].gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
                }
            }
            LoadBeatmapInfo();
        }

        public void BeatmapIndexChanged(int value)
        {
            if (CurrentIndex + value < 0) { CurrentIndex = CurrentBeatmaps.Length - 1; }
            else if (CurrentIndex + value > CurrentBeatmaps.Length - 1) { CurrentIndex = 0; }
            else { CurrentIndex += value; }
            fileIndexText.text = (CurrentIndex + 1).ToString();
            LoadBeatmapInfo();
        }

        private void LoadBeatmapInfo()
        {
            artistText.text = CurrentBeatmaps[CurrentIndex].Artist;
            authorText.text = CurrentBeatmaps[CurrentIndex].Author;
            densityText.text = CurrentBeatmaps[CurrentIndex].Density.ToString();
            //Debug.Log(CurrentBeatmaps[CurrentIndex].FilePath);

            animationObj.Play("SelectSong_BeatmapSelected", 0, 0);
            if (!DensityPanel.activeSelf) { DensityPanel.SetActive(true); }
            if (!OptionPanel.activeSelf) { OptionPanel.SetActive(true); }
            if (!StartPanel.activeSelf) { StartPanel.SetActive(true); }
        }

        public void LoadJacket(string path)
        {
            if(path == "") { Jacket.gameObject.SetActive(false); }
            else
            {
                StartCoroutine(LoadJacketTexture(path));
            }
        }

        IEnumerator LoadJacketTexture(string path)
        {
            string modifiedPath;
            if (Application.platform.Equals(RuntimePlatform.WindowsPlayer)) { modifiedPath = path.Replace("\\", "/"); }
            else { modifiedPath = "file://" + path.Replace("\\", "/"); }

            using (WWW www = new WWW(modifiedPath))
            {
                yield return www;
                Jacket.texture = www.texture;
                Jacket.gameObject.SetActive(true);
            }
        }

        public void OptionPanelClicked()
        {
            if (OptionShown) { OptionAnim.Play("OptionHideDown"); OptionShown = false; }
            else { OptionAnim.Play("OptionShowUp"); OptionShown = true; }
        }

        public void GameOptionSelected(int index)
        {
            //Debug.Log(index);
            if (index.Equals(0)) // AUTO PLAY
            {
                autoPlay = autoPlay ? false : true;
                if (autoPlay)
                {
                    AutoPlayBtn.gameObject.GetComponent<Image>().color = GlobalTheme.ThemeColor();
                    AutoPlayBtn.gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
                }
                else
                {
                    AutoPlayBtn.gameObject.GetComponent<Image>().color = Color.white;
                    AutoPlayBtn.gameObject.GetComponentInChildren<Text>().color = Color.black;
                }
            }
            else if(index.Equals(1)) // RANDOM WAVE
            {
                randWave = randWave ? false : true;
                if (randWave)
                {
                    RandWaveBtn.gameObject.GetComponent<Image>().color = GlobalTheme.ThemeColor();
                    RandWaveBtn.gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
                }
                else
                {
                    RandWaveBtn.gameObject.GetComponent<Image>().color = Color.white;
                    RandWaveBtn.gameObject.GetComponentInChildren<Text>().color = Color.black;
                }
            }
            else if (index.Equals(2)) // NO MUSIC
            {
                noMusic = noMusic ? false : true;
                if (noMusic)
                {
                    NoMusicBtn.gameObject.GetComponent<Image>().color = GlobalTheme.ThemeColor();
                    NoMusicBtn.gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
                }
                else
                {
                    NoMusicBtn.gameObject.GetComponent<Image>().color = Color.white;
                    NoMusicBtn.gameObject.GetComponentInChildren<Text>().color = Color.black;
                }
            }
            else if(index.Equals(3)) // MIRROR
            {
                mirror = mirror ? false : true;
                if(mirror)
                {
                    MirrorBtn.gameObject.GetComponent<Image>().color = GlobalTheme.ThemeColor();
                    MirrorBtn.gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
                }
                else
                {
                    MirrorBtn.gameObject.GetComponent<Image>().color = Color.white;
                    MirrorBtn.gameObject.GetComponentInChildren<Text>().color = Color.black;
                }
            }
            else if(index.Equals(4)) // DEFAULT BGA
            {
                defBGA = defBGA ? false : true;
                if (defBGA)
                {
                    DefBGABtn.gameObject.GetComponent<Image>().color = GlobalTheme.ThemeColor();
                    DefBGABtn.gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
                }
                else
                {
                    DefBGABtn.gameObject.GetComponent<Image>().color = Color.white;
                    DefBGABtn.gameObject.GetComponentInChildren<Text>().color = Color.black;
                }
            }
        }

        public void ScrollAmpChanged()
        {
            curScrollVal = scrollAmpSlider.value / 10;
            scrollAmpText.text = "x" + curScrollVal.ToString("N1");
        }

        public void ShowErrorList()
        {
            string errList = "";
            for(int i = 0; i < ErrorDirList.Count; i++)
            {
                errList += ErrorDirList[i];
                errList += Environment.NewLine;
            }
            StartCoroutine(MsgBox_ShowErrorList(errList));
        }

        IEnumerator MsgBox_ShowErrorList(string err)
        {
            MessageBox.Show(LocaleManager.instance.GetLocaleText("selectsong_errorlisthead"), LocaleManager.instance.GetLocaleText("selectsong_errorlist1") + err + LocaleManager.instance.GetLocaleText("selectsong_errorlist2"), MessageBoxButton.YesNo);
            yield return new WaitUntil(() => MessageBox.Instance.ResultExists == true);
            if (MessageBox.Instance.Result.Equals(MessageBoxButtonType.Yes))
            {
                ErrorInfo.SetActive(false);
                ErrorDirList.Clear();
                SearchLoad(searchTexts);
            }
        }

        public void SendToGame()
        {
            string sPath = GamePath.SongPath();
            DataSender.GetSongData(titleText.text, titleText.text, LevelText[LevelIndex], sPath, CurrentBeatmaps[CurrentIndex].FilePath, CurrentBeatmaps[CurrentIndex].WavPath, CurrentBeatmaps[CurrentIndex].Mp3Path, CurrentBeatmaps[CurrentIndex].OggPath, BGAPath, BackPath);
            DataSender.GetGameOptionData(autoPlay, noMusic, defBGA, randWave, mirror, curScrollVal, CurBGAVal);
            DataSender.SetNotemapMode((NotemapMode)CurrentBeatmaps[CurrentIndex].FormatMode); // FormatMode - 0: TW5, 1: SSTrain, 2: Deleste, 3: TW2, 4: TW4, 5: TW6, 6: TW1
            if (CurrentBeatmaps[CurrentIndex].FormatMode.Equals(3))
            {
                if(PlayerPrefs.HasKey("theater2") && PlayerPrefs.GetInt("theater2").Equals(0)) { changer.ChangeToScene("2LTheaterP", 0.5f); }
                else { changer.ChangeToScene("6LineGame", 0.5f); }
            }
            else { changer.ChangeToScene("6LineGame", 0.5f); }
        }

        public void EraseUnanimatedVisuals()
        {
            Jacket.gameObject.SetActive(false);
            // ADD UNANIMATED THINGS
        }
    }
    
}