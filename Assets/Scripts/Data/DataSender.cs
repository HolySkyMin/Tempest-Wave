using UnityEngine;
using System.Collections.Generic;
using TempestWave.Core;
using TempestWave.Ingame;

namespace TempestWave.Data
{
    public enum NotemapMode
    {
        TW5,
        SSTrain,
        DelesteSimulator, // STARLIGHT
        TW2, TW4, TW6, // THEATER
        TW1 // PLATINUM
    }

    public static class DataSender
    {
        private static string songName, songLevel, pathBasic, folderName, beatmapPath, wavPath, mp3Path, oggPath, bgaPath, backPath;
        private static bool autoPlay, noMusic, noBga, randWave, isMirror;
        private static int bgaFrame;
        private static float speedAmp;
        private static GameDataArchiver data;
        private static NotemapMode notemapMode;
        private static GameMode gameMode;

        public static void GetSongData(string fd, string sN, string sL, string basePath, string beatmap, string wav, string mp3, string ogg, string bga, string back)
        {
            folderName = fd;
            songName = sN;
            songLevel = sL;
            pathBasic = basePath;
            beatmapPath = beatmap;
            wavPath = wav;
            mp3Path = mp3;
            oggPath = ogg;
            bgaPath = bga;
            backPath = back;
        }

        public static void GetGameOptionData(bool aP, bool nm, bool nbga, bool rW, bool iM, float sA, int bf)
        {
            autoPlay = aP;
            noMusic = nm;
            noBga = nbga;
            randWave = rW;
            isMirror = iM;
            speedAmp = sA;
            bgaFrame = bf;
        }

        public static void SetNotemapMode(NotemapMode mode)
        {
            notemapMode = mode;

            if (mode.Equals(NotemapMode.TW5) || mode.Equals(NotemapMode.SSTrain) || mode.Equals(NotemapMode.DelesteSimulator)) { gameMode = GameMode.Starlight; }
            else if (mode.Equals(NotemapMode.TW2))
            {
                if (PlayerPrefs.HasKey("theater2") && PlayerPrefs.GetInt("theater2").Equals(0)) { gameMode = GameMode.Theater2P; }
                else { gameMode = GameMode.Theater2L; }
            }
            else if (mode.Equals(NotemapMode.TW4)) { gameMode = GameMode.Theater4; }
            else if (mode.Equals(NotemapMode.TW6)) { gameMode = GameMode.Theater; }
            else if (mode.Equals(NotemapMode.TW1)) { gameMode = GameMode.Platinum; }
        }

        public static void SetGameResult(GameDataArchiver get)
        {
            data = get;
        }

        public static string ReturnSongName()
        {
            return songName;
        }

        public static bool ReturnAutoPlay()
        {
            return autoPlay;
        }

        public static bool ReturnMusicNotPlay()
        {
            return noMusic;
        }

        public static bool ReturnNoBGA()
        {
            return noBga;
        }

        public static bool ReturnMirror() { return isMirror; }

        public static bool ReturnRandWave() { return randWave; }

        public static float ReturnSpeedAmp()
        {
            return speedAmp;
        }

        public static string ReturnMobilePath(NotemapMode mode)
        {
            return beatmapPath;
        }

        public static NotemapMode ReturnNotemapMode()
        {
            return notemapMode;
        }

        public static GameMode ReturnGameMode() { return gameMode; }

        public static string ReturnMp3Path()
        {
            return mp3Path;
        }

        public static string ReturnWavPath()
        {
            return wavPath;
        }

        public static string ReturnOggPath()
        {
            return oggPath;
        }

        public static string ReturnSongFolderPath() { return pathBasic + folderName; }

        public static string ReturnBasePath()
        {
            return pathBasic + folderName + "/" + songName;
        }

        public static int ReturnBGAFrame()
        {
            return bgaFrame;
        }

        public static string ReturnBGAPath() { return bgaPath; }

        public static string ReturnBackImgPath() { return backPath; }

        public static GameDataArchiver ReturnGameResult()
        {
            return data;
        }

        public static float DivideBetweenTwoPos(float left, float right, float m, float n)
        {
            return (m * right + n * left) / (m + n);
        }

        public static void ResultPopOut(out string sL, out string fF, out bool aP, out bool rW, out bool iM, out string sA)
        {
            sL = songLevel.ToUpper();
            fF = notemapMode.ToString();
            aP = autoPlay;
            rW = randWave;
            iM = isMirror;
            sA = speedAmp.ToString("N1");
        }
    }
}