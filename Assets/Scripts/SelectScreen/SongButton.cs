using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System;
using LitJson;

namespace TempestWave.SelectScreen
{
    public class SongButton : MonoBehaviour
    {
        private List<BeatmapInfo>[] // 0: EASY, 1: NORMAL, 2: HARD, 3: APEX
            Theater2 = new List<BeatmapInfo>[4], 
            Theater4 = new List<BeatmapInfo>[4], 
            Starlight5 = new List<BeatmapInfo>[4], 
            Theater6 = new List<BeatmapInfo>[4],
            Platinum1 = new List<BeatmapInfo>[4];
        private bool[] Filled = new bool[5] { false, false, false, false, false };
        private List<string> Tag = new List<string>();
        private int bgaframe = 0;
        private string wavPath = "", mp3Path = "", oggPath = "", bgaPath = "", jacketPath = "", backPath = "";
        public int Index;
        public Text buttonText;
        public Button startButton;
        public SongSelector selector;
        public TagManager tagmanager;


        public bool loaded { get; private set; }

        string _title;
        string _path;
        string _searchKey;

        public int SetSongName(string title, string path, string searchKey)
        {
            _title = title;
            _path = path;
            _searchKey = searchKey;
            if (searchKey != "" && !title.Substring(0, searchKey.Length > title.Length ? title.Length : searchKey.Length).ToUpper().Equals(searchKey.ToUpper())) { return 1; }
            buttonText.text = title;
            loaded = false;
            return 0;
        }

        public int SetSong(string title, string path, string searchKey)
        {
            loaded = true;
            if (searchKey != "" && !title.Substring(0, searchKey.Length > title.Length ? title.Length : searchKey.Length).ToUpper().Equals(searchKey.ToUpper())) { return 1; }
            buttonText.text = title;

            for(int i = 0; i < 4; i++)
            {
                Theater2[i] = new List<BeatmapInfo>();
                Theater4[i] = new List<BeatmapInfo>();
                Starlight5[i] = new List<BeatmapInfo>();
                Theater6[i] = new List<BeatmapInfo>();
                Platinum1[i] = new List<BeatmapInfo>();
            }

            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();

            bool breakFlag = false, hasError = false;
            
            for(int i = 0; i < files.Length; i++)
            {
                if(files[i].Name.Equals("info.txt"))
                {
                    breakFlag = true;
                }
                if (files[i].Extension.ToUpper().Equals(".WAV")) { wavPath = files[i].FullName; }
                else if (files[i].Extension.ToUpper().Equals(".MP3")) { mp3Path = files[i].FullName; }
                else if (files[i].Extension.ToUpper().Equals(".OGG")) { oggPath = files[i].FullName; }
                else if (files[i].Extension.ToUpper().Equals(".MP4")) { bgaPath = files[i].FullName; }
                else if (files[i].Name.ToUpper().Equals("JACKET.JPG") || files[i].Name.ToUpper().Equals("JACKET.PNG")) { jacketPath = files[i].FullName; }
                else if (files[i].Name.ToUpper().Equals("BACKGROUND.JPG") || files[i].Name.ToUpper().Equals("BACKGROUND.PNG")) { backPath = files[i].FullName; }
            }

            if (breakFlag) { ReadInfoTxt(path + "/info.txt", path, ref hasError); }
            else
            {
                for (int i = 0; i < files.Length && !hasError; i++)
                {
                    if (files[i].Extension.Equals(".tw1")) { ReadTWxMetadata(files[i].FullName, 4, ref hasError); }
                    else if (files[i].Extension.Equals(".tw2")) { ReadTWxMetadata(files[i].FullName, 1, ref hasError); }
                    else if (files[i].Extension.Equals(".tw4")) { ReadTWxMetadata(files[i].FullName, 2, ref hasError); }
                    else if (files[i].Extension.Equals(".txt")) { ReadDelesteMetadata(files[i].FullName, ref hasError); }
                    else if (files[i].Extension.ToUpper().Equals(".JSON")) { ReadSSTrainMetadata(files[i].FullName, ref hasError); }
                    else if (files[i].Extension.Equals(".tw5")) { ReadTWxMetadata(files[i].FullName, 0, ref hasError); }
                    else if (files[i].Extension.Equals(".tw6")) { ReadTWxMetadata(files[i].FullName, 3, ref hasError); }
                }
            }

            if (hasError) { return -1; }
            else { return 0; }
        }

        private void ReadTWxMetadata(string path, int gameMode, ref bool hasError)
        {
            StreamReader reader = new StreamReader(path);
            string rawData = reader.ReadToEnd();
            JsonData data = JsonMapper.ToObject(rawData);
            reader.Close();
            if(!data.Keys.Contains("version") || !data["version"].IsInt || (int)data["version"] < 2) { hasError = true; return; }

            int level = 1;
            BeatmapInfo info = new BeatmapInfo() { WavPath = wavPath, Mp3Path = mp3Path, OggPath = oggPath };
            try
            {
                level = (int)data["metadata"]["level"];
                info.Artist = (string)data["metadata"]["artist"];
                info.Author = (string)data["metadata"]["mapper"];
                info.Density = (int)data["metadata"]["density"];
            }
            catch(Exception e)
            {
                hasError = true;
                Debug.Log(path + Environment.NewLine + e);
                return;
            }
            info.FilePath = path;

            if (gameMode.Equals(0)) { info.FormatMode = 0; Starlight5[level - 1].Add(info); Filled[0] = true; }
            else if (gameMode.Equals(1)) { info.FormatMode = 3; Theater2[level - 1].Add(info); Filled[1] = true; }
            else if (gameMode.Equals(2)) { info.FormatMode = 4; Theater4[level - 1].Add(info); Filled[2] = true; }
            else if (gameMode.Equals(3)) { info.FormatMode = 5; Theater6[level - 1].Add(info); Filled[3] = true; }
            else if (gameMode.Equals(4)) { info.FormatMode = 6; Platinum1[level - 1].Add(info); Filled[4] = true; }
        }

        private void ReadSSTrainMetadata(string path, ref bool hasError)
        {
            StreamReader reader = new StreamReader(path);
            string rawData = reader.ReadToEnd();
            JsonData data = JsonMapper.ToObject(rawData);
            reader.Close();

            int level = 1;
            string levelName;
            BeatmapInfo info = new BeatmapInfo() { WavPath = wavPath, Mp3Path = mp3Path, OggPath = oggPath };
            try
            {
                info.Artist = ((string)data["metadata"]["composer"] + " / " + (string)data["metadata"]["lyricist"]);
                info.Density = (int)data["metadata"]["difficulty"];
                info.FilePath = path;
                info.FormatMode = 1;
                levelName = (string)data["metadata"]["difficultyName"];
            }
            catch(Exception e)
            {
                hasError = true;
                Debug.Log(path + Environment.NewLine + e);
                return;
            }
            if (levelName.Equals("Debut") || levelName.Equals("Regular")) { level = 1; }
            else if (levelName.Equals("Pro")) { level = 2; }
            else if (levelName.Equals("Master")) { level = 3; }
            else if (levelName.Equals("Master+")) { level = 4; }

            Starlight5[level - 1].Add(info);
            Filled[0] = true;
        }

        private void ReadDelesteMetadata(string path, ref bool hasError)
        {
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(stream);
            char[] div = new char[] { ' ' };

            BeatmapInfo info = new BeatmapInfo() { WavPath = wavPath, Mp3Path = mp3Path, OggPath = oggPath };
            info.FilePath = path;
            info.FormatMode = 2;
            int level = 1;
            for(int i = 0; ; )
            {
                try
                {
                    string t = reader.ReadLine();
                    string[] data = t.Split(div, StringSplitOptions.RemoveEmptyEntries);

                    if (data.Length > 0 && data[0].StartsWith("#"))
                    {
                        if (int.TryParse(data[0].Substring(1, 1), out i)) { break; }

                        if (data[0].Substring(1).ToUpper().Equals("COMPOSER"))
                        {
                            info.Artist = "";
                            for (int j = 1; j < data.Length; j++)
                            {
                                if (j.Equals(1)) { info.Artist += data[j]; }
                                else { info.Artist += (" " + data[j]); }
                            }
                        }
                        else if(data[0].Substring(1).ToUpper().Equals("MAPPER") || data[0].Substring(1).ToUpper().Equals("AUTHOR"))
                        {
                            info.Author = "";
                            for (int j = 1; j < data.Length; j++)
                            {
                                if (j.Equals(1)) { info.Author += data[j]; }
                                else { info.Author += (" " + data[j]); }
                            }
                        }
                        else if(data[0].Substring(1).ToUpper().Equals("DIFFICULTY"))
                        {
                            if (data[1].Equals("1") || data[1].Equals("2") || data[1].ToUpper().Equals("DEBUT") || data[1].ToUpper().Equals("REGULAR")) { level = 1; }
                            else if (data[1].Equals("3") || data[1].ToUpper().Equals("PRO")) { level = 2; }
                            else if (data[1].Equals("4") || data[1].ToUpper().Equals("MASTER")) { level = 3; }
                            else if (data[1].Equals("5") || data[1].ToUpper().Equals("MASTER+")) { level = 4; }
                        }
                        else if(data[0].Substring(1).ToUpper().Equals("LV") || data[0].Substring(1).ToUpper().Equals("LEVEL"))
                        {
                            info.Density = int.Parse(data[1]);
                        }
                    }
                }
                catch(Exception e)
                {
                    hasError = true;
                    Debug.Log(path + Environment.NewLine + e);
                    return;
                }
            }
            reader.Close();
            stream.Close();

            Starlight5[level - 1].Add(info);
            Filled[0] = true;
        }

        private void ReadInfoTxt(string path, string originPath, ref bool hasError)
        {
            try
            {
                FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
                StreamReader scanner = new StreamReader(file, Encoding.Default);
                char[] div = { ' ' };

                string Title = "", Artist = "-";

                while (scanner.Peek() != -1)
                {
                    string temp = scanner.ReadLine();
                    if (temp.StartsWith("#"))
                    {
                        string[] dat = temp.Split(div);
                        if (dat[0].Equals("#title")) { Title = temp.Substring(7); }
                        else if (dat[0].Equals("#artist"))
                        {
                            Artist = temp.Substring(8);
                        }
                        else if (dat[0].Equals("#easy") || dat[0].Equals("#normal") || dat[0].Equals("#hard") || dat[0].Equals("#apex"))
                        {
                            int level = 1;
                            if (dat[0].Equals("#easy")) { level = 1; }
                            else if (dat[0].Equals("#normal")) { level = 2; }
                            else if (dat[0].Equals("#hard")) { level = 3; }
                            else if (dat[0].Equals("#apex")) { level = 4; }

                            for (int i = 1; i < dat.Length; i++)
                            {
                                string[] basedata = dat[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                for (int j = 0; j < basedata.Length; j++)
                                {
                                    int densityVal = 0;
                                    string[] formatData = basedata[j].Split(new char[] { '=' });

                                    if (formatData.Length.Equals(2))
                                    {
                                        string[] infoData = formatData[1].Split(new char[] { ':' });
                                        densityVal = int.Parse(infoData[0]);
                                        string authorDat = "-";
                                        if (infoData.Length > 1) { authorDat = infoData[1].Replace('_', ' '); }
                                        if (formatData[0].Equals("tw5")) { Starlight5[level - 1].Add(new BeatmapInfo(0, densityVal, Artist, authorDat, wavPath, mp3Path, oggPath, originPath + "/" + Title + "_" + dat[0].Substring(1) + ".tw5")); Filled[0] = true; }
                                        else if (formatData[0].Equals("sstrain")) { Starlight5[level - 1].Add(new BeatmapInfo(1, densityVal, Artist, authorDat, wavPath, mp3Path, oggPath, originPath + "/" + Title + "_" + dat[0].Substring(1) + ".json")); Filled[0] = true; }
                                        else if (formatData[0].Equals("deleste")) { Starlight5[level - 1].Add(new BeatmapInfo(2, densityVal, Artist, authorDat, wavPath, mp3Path, oggPath, originPath + "/" + Title + "_" + dat[0].Substring(1) + ".txt")); Filled[0] = true; }
                                        else if (formatData[0].Equals("tw2")) { Theater2[level - 1].Add(new BeatmapInfo(3, densityVal, Artist, authorDat, wavPath, mp3Path, oggPath, originPath + "/" + Title + "_" + dat[0].Substring(1) + ".tw2")); Filled[1] = true; }
                                        else if (formatData[0].Equals("tw4")) { Theater4[level - 1].Add(new BeatmapInfo(4, densityVal, Artist, authorDat, wavPath, mp3Path, oggPath, originPath + "/" + Title + "_" + dat[0].Substring(1) + ".tw4")); Filled[2] = true; }
                                        else if (formatData[0].Equals("tw6")) { Theater6[level - 1].Add(new BeatmapInfo(5, densityVal, Artist, authorDat, wavPath, mp3Path, oggPath, originPath + "/" + Title + "_" + dat[0].Substring(1) + ".tw6")); Filled[3] = true; }
                                    }
                                    else
                                    {
                                        hasError = true;
                                        return;
                                    }
                                }
                            }
                        }
                        else if (dat[0].Equals("#bgastart"))
                        {
                            bgaframe = int.Parse(dat[1]);
                        }
                    }
                }
                scanner.Close();
                file.Close();
            }
            catch(Exception e)
            {
                hasError = true;
                Debug.Log(originPath + Environment.NewLine + e);
                return;
            }
        }

        public void Click()
        {
            if (!loaded)
                SetSong(_title, _path, _searchKey);
            selector.Selected(Index, buttonText.text, Starlight5, Theater2, Theater4, Theater6, Platinum1, Filled, bgaframe, bgaPath, backPath);
            gameObject.GetComponent<Graphic>().color = Core.UI.GlobalTheme.ThemeContrastColor();
            buttonText.color = Core.UI.GlobalTheme.ThemeColor();
            selector.LoadJacket(jacketPath);
        }
    }
}