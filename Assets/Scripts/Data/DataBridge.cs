using System.Collections;
using System.Collections.Generic;
using TempestWave.Ingame;

namespace TempestWave.Data
{
    public class DataBridge
    {
        public static DataBridge Instance { get; set; }

        public float ScrollAmp { get; set; }
        public bool IsAuto { get; set; }
        public bool IsRandomWave { get; set; }
        public bool IsNoMusic { get; set; }
        public bool IsNoBGA { get; set; }
        public string BeatmapPath { get; set; }
        public string WavMusicPath { get; set; }
        public string OggMusicPath { get; set; }
        public string Mp3MusicPath { get; set; }
        public GameMode CurrentGameMode { get; set; }
        public NotemapMode CurrentBeatmapMode { get; set; }
        
    }
}