using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TempestWave.Core;
using TempestWave.Core.UI;

namespace TempestWave.Creator
{
    public class MusicPlayer : MonoBehaviour
    {
        public int CurrentBlockIndex { get; set; }
        public float CurrentTime { get; set; }
        public float CurrentBlockTime { get; set; }
        public bool IsPlaying { get; set; }

        public GameObject 
            BtnTemplate,
            Dim1,
            Dim2,
            Dim3;
        public RectTransform 
            BtnParent,
            BlockParent;
        public Text TimeText;
        public AudioSource
            MusicSource,
            TapSound,
            FlickSound;
        public Workspace Table;

        private List<GameObject> Buttons;
        private int beforeY = -1;

        private void Start()
        {
            Buttons = new List<GameObject>();
        }

        private void Update()
        {
            int frameIndex = Mathf.FloorToInt(-1 * BlockParent.anchoredPosition.y / (670 * Table.ScopeVal));
            if(frameIndex < 0) { BlockParent.anchoredPosition = new Vector3(BlockParent.anchoredPosition.x, 0); frameIndex = 0; }
            if(frameIndex >= Table.BlockCount()) { BlockParent.anchoredPosition = new Vector3(BlockParent.anchoredPosition.x, (-670 * Table.ScopeVal) * Table.BlockCount() - 0.1f); frameIndex = Table.BlockCount() - 1; }
            if (frameIndex != CurrentBlockIndex && frameIndex < Table.BlockCount())
            {
                CurrentBlockIndex = frameIndex;
                if(Table.BlockCount() > 0) { CurrentBlockTime = Table.GetBlock(CurrentBlockIndex).BlockTime; }
            }
            if (Table.BlockCount() > 0 && CurrentBlockIndex >= 0 && CurrentBlockIndex < Table.BlockCount()) { CalculateCurrentTime(); }
            if (CurrentTime >= 0 && !IsPlaying) { MusicSource.time = CurrentTime; }
            TimeText.text = Mathf.FloorToInt(CurrentTime / 60).ToString("D2") + ":" + Mathf.FloorToInt(CurrentTime % 60).ToString("D2");

            int y = Mathf.FloorToInt(((Mathf.Abs(BlockParent.anchoredPosition.y) % (670 * Table.ScopeVal)) / (670 * Table.ScopeVal)) * 192);
            if (IsPlaying)
            {
                if(CurrentTime >= 0 && !MusicSource.isPlaying) { MusicSource.Play(); }

                
                foreach (CreatorNote note in Table.GetBlock(CurrentBlockIndex).GetNoteArray())
                {
                    if ((beforeY <= y && note.YPos > beforeY && note.YPos <= y) || (beforeY > y && note.YPos > (beforeY - 192) && note.YPos <= y))
                    {
                        if (!note.Flick.Equals(0)) { FlickSound.Play(); }
                        else { TapSound.Play(); }
                    }
                }
                
                BlockParent.anchoredPosition = new Vector3(BlockParent.anchoredPosition.x, BlockParent.anchoredPosition.y - (((670 * Table.ScopeVal) / CurrentBlockTime) * Time.deltaTime));

                if (Mathf.FloorToInt(Mathf.Abs(BlockParent.anchoredPosition.y) / (670 * Table.ScopeVal)) >= Table.BlockCount()) { StopMusic(); }
            }
            beforeY = y;
        }

        public void Clean()
        {
            MusicSource.clip = null;
            IsPlaying = false;
            CurrentBlockIndex = -1;
            CurrentTime = 0;
            while(Buttons.Count > 0)
            {
                GameObject target = Buttons[0];
                Destroy(target);
                Buttons.Remove(target);
            }

            DirectoryInfo CreatorDir = new DirectoryInfo(GamePath.CreatorPath());
            FileInfo[] CreatorFiles = CreatorDir.GetFiles();

            for(int i = 0; i < CreatorFiles.Length; i++)
            {
                string ext = CreatorFiles[i].Extension;
                if(ext.ToUpper().Equals(".WAV") || ext.ToUpper().Equals(".OGG") || ((Application.platform.Equals(RuntimePlatform.Android) || Application.platform.Equals(RuntimePlatform.IPhonePlayer)) && ext.ToUpper().Equals(".MP3")))
                {
                    GameObject newBtn = Instantiate(BtnTemplate);
                    newBtn.SetActive(true);
                    MusicFileBtn fileBtn = newBtn.GetComponent<MusicFileBtn>();
                    fileBtn.Index = Buttons.Count;
                    fileBtn.SetData(CreatorFiles[i].FullName, CreatorFiles[i].Name);
                    newBtn.GetComponent<RectTransform>().SetParent(BtnParent);
                    newBtn.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                    Buttons.Add(newBtn);
                }
            }
        }

        public void CalculateCurrentTime()
        {
            int tempOffset;
            CurrentTime = 0;
            if (int.TryParse(Table.OffsetField.text, out tempOffset)) { CurrentTime = tempOffset / 1000f; }
            
            for(int i = 0; i < CurrentBlockIndex; i++) { CurrentTime += Table.GetBlock(i).BlockTime; }
            CurrentTime += (Table.GetBlock(CurrentBlockIndex).BlockTime * ((Mathf.Abs(BlockParent.anchoredPosition.y) % (670 * Table.ScopeVal)) / (670 * Table.ScopeVal)));
        }

        public void LoadMusic(string path, int index)
        {
            if (Application.platform.Equals(RuntimePlatform.Android) || Application.platform.Equals(RuntimePlatform.IPhonePlayer)) { StartCoroutine(LoadMusicProgress("file://" + path)); }
            else { StartCoroutine(LoadMusicProgress(path)); }
            for(int i = 0; i < Buttons.Count; i++)
            {
                if(i.Equals(index))
                {
                    Buttons[i].GetComponent<Image>().color = GlobalTheme.ThemeContrastColor();
                    Buttons[i].GetComponentInChildren<Text>().color = GlobalTheme.ThemeColor();
                }
                else
                {
                    Buttons[i].GetComponent<Image>().color = GlobalTheme.ThemeColor();
                    Buttons[i].GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
                }
            }
        }

        IEnumerator LoadMusicProgress(string path)
        {
            WWW www = new WWW(path);
            yield return www;
            MusicSource.clip = www.GetAudioClip();
        }

        public void PlayMusic()
        {
            if (Table.BlockCount() > 0 && CurrentBlockTime <= 0) { CurrentBlockTime = Table.GetBlock(CurrentBlockIndex).BlockTime; }
            IsPlaying = true;
            Dim1.SetActive(true);
            Dim2.SetActive(true);
            Dim3.SetActive(true);
        }

        public void StopMusic()
        {
            IsPlaying = false;
            Dim1.SetActive(false);
            Dim2.SetActive(false);
            Dim3.SetActive(false);
            MusicSource.Stop();

            //beforeY = -1;
        }
    }
}