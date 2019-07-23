using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TempestWave.Creator
{
    public class MusicFileBtn : MonoBehaviour
    {
        public int Index { get; set; }
        public Text ButtonText;
        public MusicPlayer Player;

        private string FilePath;

        public void SetData(string path, string name)
        {
            FilePath = path;
            ButtonText.text = name;
        }

        public void SendData()
        {
            Player.LoadMusic(FilePath, Index);
        }
    }
}