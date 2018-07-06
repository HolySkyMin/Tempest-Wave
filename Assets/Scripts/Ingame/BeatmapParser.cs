using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TempestWave.Data;
using System.IO;
using TempestWave.Core.UI;
using TempestWave.Monitorer;
using System;
using LitJson;

namespace TempestWave.Ingame
{
    public class BeatmapParser : MonoBehaviour
    {
        private GameManager Game;

        public BeatmapParser(GameManager g)
        {
            Game = g;
        }

        

        private int LCM(int x, int y)
        {
            int a, b, c;
            a = x;
            b = y;
            while (a != 0)
            {
                c = a;
                a = b % a;
                b = c;
            }
            return x * y / b;
        }

        public void ParseBeatmap()
        {
            string nmPath = DataSender.ReturnMobilePath(DataSender.ReturnNotemapMode());
            if (DataSender.ReturnNotemapMode().Equals(NotemapMode.SSTrain)) { ParseSSTrain(nmPath); }
            else if (DataSender.ReturnNotemapMode().Equals(NotemapMode.DelesteSimulator)) { ParseDeleste(nmPath); }
            else if (DataSender.ReturnNotemapMode().Equals(NotemapMode.TW2)) { ParseTWx(2); }
            else if (DataSender.ReturnNotemapMode().Equals(NotemapMode.TW4)) { ParseTWx(4); }
            else if (DataSender.ReturnNotemapMode().Equals(NotemapMode.TW5)) { ParseTWx(5); }
            else if (DataSender.ReturnNotemapMode().Equals(NotemapMode.TW6)) { ParseTWx(6); }
            else if (DataSender.ReturnNotemapMode().Equals(NotemapMode.TW1)) { ParseTWx(1); }
        }

        private void ParseSSTrain(string path)
        {
            bool initNoteUsed = false;
            float initNotePos = 0;
            StreamReader stream = null;
            try { stream = new StreamReader(path); }
            catch { Game.ThrowError(ErrorMode.NoJsonFile); }
            try
            {
                string rawdata = stream.ReadToEnd();
                JsonData data = JsonMapper.ToObject(rawdata);
                if (PlayerPrefs.HasKey("gamesync").Equals(true)) { initNotePos = PlayerPrefs.GetFloat("gamesync"); }
                else { Game.ThrowError(ErrorMode.NoSyncValue); return; }
                for (int i = 0; i < data["notes"].Count; i++)
                {
                    int index = (int)data["notes"][i]["id"];
                    double frame;
                    if (data["notes"][i]["timing"].IsDouble.Equals(true)) { frame = (double)data["notes"][i]["timing"]; }
                    else { frame = (int)data["notes"][i]["timing"]; }
                    if (initNoteUsed.Equals(false) && initNotePos <= (float)frame) { Game.Dispensor.CreateNote(0, 0, Color.white, NoteInfo.SystemNoteStarter, FlickMode.None, initNotePos, Game.GlobalNoteSpeed, 1, 1, new List<int>()); initNoteUsed = true; }
                    int speci = (int)data["notes"][i]["type"];
                    int start = (int)data["notes"][i]["startPos"];
                    int end = (int)data["notes"][i]["endPos"];
                    FlickMode mode = (FlickMode)(int)data["notes"][i]["status"];
                    int prev = (int)data["notes"][i]["prevNoteId"];
                    int next = (int)data["notes"][i]["nextNoteId"];
                    // 예외적 허용: 롱 노트 끝부분 슬라이드를 종류 1번으로 처리한 채보가 존재함. 그 때 값을 2로 수정. 그리고 롱 노트 끝부분의 경우 시작지점 통일
                    if (prev > 0 && Game.Dispensor.Notes[prev].Mode.Equals(NoteInfo.LongNoteStart))
                    {
                        if (speci.Equals(1)) { speci = 2; }
                        start = (int)Game.Dispensor.Notes[prev].StartLine;
                        if (DataSender.ReturnMirror()) { start = 6 - start; }
                    }

                    NoteInfo noteMode = NoteInfo.NormalNote;
                    if (speci.Equals(1)) { noteMode = NoteInfo.NormalNote; }
                    else if (speci.Equals(2) && prev <= 0) { noteMode = NoteInfo.LongNoteStart; }
                    else if (speci.Equals(2) && prev > 0) { noteMode = NoteInfo.LongNoteEnd; }
                    else if (speci.Equals(3) && prev <= 0) { noteMode = NoteInfo.SlideNoteStart; }
                    else if (speci.Equals(3) && prev > 0) { noteMode = NoteInfo.SlideNoteCheckpoint; }
                    //else if (speci.Equals(3) && prev > 0 && next <= 0) { noteMode = NoteInfo.SlideNoteEnd; }

                    if (DataSender.ReturnRandWave()) { start = UnityEngine.Random.Range(1, 6); }
                    if(DataSender.ReturnMirror())
                    {
                        if (mode.Equals(FlickMode.Left)) { mode = FlickMode.Right; }
                        else if (mode.Equals(FlickMode.Right)) { mode = FlickMode.Left; }
                        start = 6 - start;
                        end = 6 - end;
                    }

                    List<int> Prevs = new List<int>();
                    if (prev > 0) { Prevs.Add(prev); }
                    Game.Dispensor.CreateNote(index, 1, new Color32(255, 255, 255, 255), noteMode, mode, (float)frame * 60, Game.GlobalNoteSpeed, start, end, Prevs);
                }
            }
            catch(Exception e)
            {
                Game.ThrowError(ErrorMode.NotemapSyntaxError, e.Message + e.StackTrace);
                Debug.Log(e);
            }
            stream.Close();
        }

        private void ParseDeleste(string path)
        {
            FileStream file = null;
            StreamReader scanner = null;
            try
            {
                file = new FileStream(path, FileMode.Open, FileAccess.Read);
                scanner = new StreamReader(file, System.Text.Encoding.UTF8);
            }
            catch
            {
                Game.ThrowError(ErrorMode.NoDeresimuFile);
                return;
            }

            int d, MaxBlockNum = -1, ID = 1;
            char[] div = { ' ', ',', ':' };
            DelesteGlobalData StaticData = new DelesteGlobalData();
            Dictionary<int, DelesteBlockData> Blocks = new Dictionary<int, DelesteBlockData>();
            double Time = 0, SongTime = 0, Speed = 1.0;
            byte[] Color = new byte[] { 255, 255, 255, 255 };

            if (PlayerPrefs.HasKey("gamesync")) { SongTime = PlayerPrefs.GetFloat("gamesync") / 60; }

            while(scanner.Peek() != -1)
            {
                try
                {
                    string dataLine = scanner.ReadLine();
                    string[] data = dataLine.Split(div, StringSplitOptions.RemoveEmptyEntries);

                    if (data.Length > 1 && data[0].StartsWith("#") && (data[0].Substring(1).ToUpper().Equals("BPM") || data[0].Substring(1).ToUpper().Equals("TEMPO")))
                    {
                        StaticData.CurrentBPM = double.Parse(data[1]);
                    }
                    else if (data.Length > 1 && data[0].StartsWith("#") && data[0].Substring(1).ToUpper().Equals("OFFSET"))
                    {
                        Time += (int.Parse(data[1]) / 1000d);
                    }
                    else if (data.Length > 1 && data[0].StartsWith("#") && (data[0].Substring(1).ToUpper().Equals("SONGOFFSET") || data[0].Substring(1).ToUpper().Equals("MUSICOFFSET") || data[0].Substring(1).ToUpper().Equals("BGMOFFSET")))
                    {
                        Time -= (int.Parse(data[1]) / 1000d);
                    }
                    else if (data.Length > 1 && data[0].StartsWith("#") && data[0].Substring(1).ToUpper().Equals("ATTRIBUTE"))
                    {
                        if(data[1].ToUpper().Equals("CUTE") || data[1].ToUpper().Equals("CU") || data[1].Equals("1")) { Color = new byte[] { 255, 100, 200, 255}; }
                        else if(data[1].ToUpper().Equals("COOL") || data[1].ToUpper().Equals("CO") || data[1].Equals("2")) { Color = new byte[] { 85, 135, 255, 255 }; }
                        else if(data[1].ToUpper().Equals("PASSION") || data[1].ToUpper().Equals("PA") || data[1].Equals("3")) { Color = new byte[] { 255, 220, 50, 255 }; }
                        else if (data[1].ToUpper().Equals("ALL") || data[1].Equals("4")) { Color = new byte[] { 230, 255, 255, 255 }; }
                    }
                    else if (data.Length > 2 && data[0].StartsWith("#") && (data[0].Substring(1).ToUpper().Equals("MEASURE") || data[0].Substring(1).ToUpper().Equals("MEAS") || data[0].Substring(1).ToUpper().Equals("MEA")))
                    {
                        if (data[2].Contains("/"))
                        {
                            string[] numbers = data[2].Split(new char[] { '/' });
                            StaticData.Measure.Add(double.Parse(numbers[0]) / double.Parse(numbers[1]));
                        }
                        else { StaticData.Measure.Add(double.Parse(data[2])); }
                        StaticData.MeasurePos.Add(double.Parse(data[1]));
                    }
                    else if (data.Length > 2 && (data[0].StartsWith("#") && (data[0].Substring(1).ToUpper().Equals("CHANGEBPM") || data[0].Substring(1).ToUpper().Equals("CHANGETEMPO"))))
                    {
                        StaticData.ChangeBPM.Add(double.Parse(data[2]));
                        StaticData.ChangeBPMPos.Add(double.Parse(data[1]));
                    }
                    else if (data.Length > 1 && data[0].StartsWith("#") && data[0].Substring(1).ToUpper().Equals("CHANGEATTRIBUTE"))
                    {
                        if (data[1].ToUpper().Equals("CUTE") || data[1].ToUpper().Equals("CU") || data[1].Equals("1")) { Color = new byte[] { 255, 100, 200, 255 }; }
                        else if (data[1].ToUpper().Equals("COOL") || data[1].ToUpper().Equals("CO") || data[1].Equals("2")) { Color = new byte[] { 85, 135, 255, 255 }; }
                        else if (data[1].ToUpper().Equals("PASSION") || data[1].ToUpper().Equals("PA") || data[1].Equals("3")) { Color = new byte[] { 255, 220, 50, 255 }; }
                        else if (data[1].ToUpper().Equals("ALL") || data[1].Equals("4")) { Color = new byte[] { 230, 255, 255, 255 }; }
                    }
                    else if (data.Length > 1 && data[0].StartsWith("#") && (data[0].Substring(1).ToUpper().Equals("HISPEED") || data[0].Substring(1).ToUpper().Equals("HS")))
                    {
                        Speed = double.Parse(data[1]);
                        if (Speed <= 0) { throw new Exception(LocaleManager.instance.GetLocaleText("errordetail_speed")); }
                    }
                    /*
                    else if(data.Length > 2 && data[0].StartsWith("#") && data[0].Substring(1).ToUpper().Equals("HS2"))
                    {
                        if(double.Parse(data[2]) <= 0){ throw new Exception(LocaleManager.instance.GetLocaleText("errordetail_speed")); }
                        StaticData.HS2.Add(double.Parse(data[2]));
                        StaticData.HS2Pos.Add(double.Parse(data[1]));
                    }
                    */
                    else if(data.Length > 2 && data[0].StartsWith("#") && data[0].Substring(1).ToUpper().Equals("DELAY"))
                    {
                        StaticData.Delay.Add(double.Parse(data[2]));
                        StaticData.DelayPos.Add(double.Parse(data[1]));
                    }
                    else if (data.Length > 3 && data[0].StartsWith("#") && data[0].Substring(1).ToUpper().Equals("SCROLL"))
                    {
                        StaticData.Scroll.Add(new double[2] { double.Parse(data[2]) / 1000d, double.Parse(data[3]) / 1000d});
                        StaticData.ScrollPos.Add(double.Parse(data[1]));
                    }
                    else if (data.Length > 0 && data[0].StartsWith("#") && int.TryParse(data[0].Substring(1, 1), out d))
                    {
                        int CurBlock = int.Parse(data[1]);
                        if (!Blocks.ContainsKey(CurBlock)) { Blocks.Add(CurBlock, new DelesteBlockData(CurBlock)); }
                        Blocks[CurBlock].DataLines.Add(dataLine);
                        Blocks[CurBlock].Channel.Add(int.Parse(data[0].Substring(1)));
                        Blocks[CurBlock].Color.Add(Color);
                        Blocks[CurBlock].Speed.Add(Speed * Game.GlobalNoteSpeed);

                        if (MaxBlockNum < CurBlock) { MaxBlockNum = CurBlock; }
                    }
                }
                catch(Exception e)
                {
                    Game.ThrowError(ErrorMode.NotemapSyntaxError, e.Message + e.StackTrace);
                    Debug.Log(e);
                }
            }
            scanner.Close();
            file.Close();

            Game.Dispensor.CreateNote(0, 0, new Color32(255, 255, 255, 255), NoteInfo.SystemNoteStarter, FlickMode.None, (float)SongTime * 60, (float)Speed * Game.GlobalNoteSpeed, 1, 1, new List<int>());
            for(int i = 0; i <= MaxBlockNum; i++)
            {
                if(Blocks.ContainsKey(i))
                {
                    Blocks[i].ParseBlock(Game, ref ID, ref Time, ref StaticData);
                }
                else { Time += ((240 / StaticData.CurrentBPM) * StaticData.BeatMultiplier); }
            }
        }

        private void ParseTWx(int maxLine)
        {
            float StarterNotePos = 0;
            StreamReader stream = null;
            try { stream = new StreamReader(DataSender.ReturnMobilePath(DataSender.ReturnNotemapMode())); }
            catch { Game.ThrowError(ErrorMode.NoJsonFile); return; }

            try
            {
                string RawData = stream.ReadToEnd();
                JsonData data = JsonMapper.ToObject(RawData);
                if (PlayerPrefs.HasKey("gamesync").Equals(true)) { StarterNotePos = PlayerPrefs.GetFloat("gamesync"); }
                Game.Dispensor.CreateNote(0, 0, Color.white, NoteInfo.SystemNoteStarter, FlickMode.None, StarterNotePos, Game.GlobalNoteSpeed, 1, 1, new List<int>());

                int verCode = 0;
                try { verCode = (int)data["version"]; }
                catch { verCode = 0; }
                Debug.Log("TWx version: " + verCode + ", Max line: " + maxLine);

                for (int i = 0; i < data["notes"].Count; i++)
                {
                    int id = (int)data["notes"][i]["ID"];
                    int size = (int)data["notes"][i]["Size"];
                    Color32 color = new Color32((byte)(int)data["notes"][i]["Color"][0], (byte)(int)data["notes"][i]["Color"][1], (byte)(int)data["notes"][i]["Color"][2], (byte)(int)data["notes"][i]["Color"][3]);
                    int ModNum = (int)data["notes"][i]["Mode"];
                    FlickMode flick = (FlickMode)(int)data["notes"][i]["Flick"];
                    double time = (double)data["notes"][i]["Time"];
                    double speed = (double)data["notes"][i]["Speed"];
                    double start = (double)data["notes"][i]["StartLine"];
                    double end = (double)data["notes"][i]["EndLine"];
                    List<int> prevs = new List<int>();
                    for (int j = 0; j < data["notes"][i]["PrevIDs"].Count; j++)
                    {
                        if (!((int)data["notes"][i]["PrevIDs"][j]).Equals(0)) { prevs.Add((int)data["notes"][i]["PrevIDs"][j]); }
                    }
                    //int next = (int)data["notes"][i]["NextID"];

                    if(verCode < 1)
                    {
                        double k = 0;
                        if (maxLine.Equals(2)) { k = 1.5 - ((1.5 - start) * (9d / 5)); }
                        else if (maxLine.Equals(4)) { k = 2.5 - ((2.5 - start) * (9d / 5)); }
                        else if (maxLine.Equals(6)) { k = 3.5 - ((3.5 - start) * (9d / 5)); }
                        start = k;
                    }

                    NoteInfo mode = NoteInfo.NormalNote;
                    if (ModNum.Equals(0)) { mode = NoteInfo.NormalNote; }
                    else if (ModNum.Equals(1) && prevs.Count.Equals(0)) { mode = NoteInfo.LongNoteStart; }
                    else if (ModNum.Equals(1) && prevs.Count > 0) { mode = NoteInfo.LongNoteEnd; }
                    else if (ModNum.Equals(2) && prevs.Count.Equals(0)) { mode = NoteInfo.SlideNoteStart; }
                    else if (ModNum.Equals(2) && prevs.Count > 0) { mode = NoteInfo.SlideNoteCheckpoint; }
                    else if (ModNum.Equals(3)) { mode = NoteInfo.DamageNote; }
                    else if (ModNum.Equals(4)) { mode = NoteInfo.HiddenNote; }
                    else if (ModNum.Equals(12)) { mode = NoteInfo.SystemNoteScroller; }
                    //else if (ModNum.Equals(11)) { mode = NoteInfo.SystemNoteSpeeder; }

                    if (DataSender.ReturnRandWave()) { start = UnityEngine.Random.Range(0f, maxLine + 1); }
                    if(DataSender.ReturnMirror())
                    {
                        if (flick.Equals(FlickMode.Left)) { flick = FlickMode.Right; }
                        else if (flick.Equals(FlickMode.Right)) { flick = FlickMode.Left; }
                        start = (maxLine + 1) - start;
                        end = (maxLine + 1) - end;
                    }
                    Game.Dispensor.CreateNote(id, size, color, mode, flick, (float)time * 60, (float)speed * Game.GlobalNoteSpeed, (float)start, (float)end, prevs);
                }
            }
            catch(Exception e)
            {
                Game.ThrowError(ErrorMode.NotemapSyntaxError, e.Message + e.StackTrace);
                Debug.Log(e);
                return;
            }

            stream.Close();
        }
    }
}
