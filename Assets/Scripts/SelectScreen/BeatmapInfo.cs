using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TempestWave.SelectScreen
{
    public class BeatmapInfo
    {
        public int FormatMode { get; set; }
        public int Density { get; set; }
        public string Artist { get; set; }
        public string Author { get; set; }
        public string FilePath { get; set; }
        public string WavPath { get; set; }
        public string Mp3Path { get; set; }
        public string OggPath { get; set; }

        public BeatmapInfo() { }

        public BeatmapInfo(int formatMode, int density, string artist, string author, string wavPath, string mp3Path, string oggPath, string filePath)
        {
            FormatMode = formatMode;
            Density = density;
            Artist = artist;
            Author = author;
            WavPath = wavPath;
            Mp3Path = mp3Path;
            OggPath = oggPath;
            FilePath = filePath;
        }

        public void SetValues(int formatMode, int density, string author)
        {
            FormatMode = formatMode;
            Density = density;
            Author = author;
        }
    }
}