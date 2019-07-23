using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TempestWave.Data;

namespace TempestWave.Ingame
{
    public class SELoader : MonoBehaviour
    {
        public AudioSource Audio;
        public string FileName;

        private void Awake()
        {
            StartCoroutine(FindLoadSE());
        }

        IEnumerator FindLoadSE()
        {
            WWW www = null;
            string myPath = DataSender.ReturnSongFolderPath() + "/se/" + FileName;
            string fullPath;
            if (File.Exists(myPath + ".wav"))
            {
                FileInfo info = new FileInfo(myPath + ".wav");
                fullPath = info.FullName;
            }
            else if (File.Exists(myPath + ".ogg"))
            {
                FileInfo info = new FileInfo(myPath + ".ogg");
                fullPath = info.FullName;
            }
            else { yield break; }
            if (Application.platform.Equals(RuntimePlatform.WindowsPlayer)) { www = new WWW(fullPath); }
            else { www = new WWW("file://" + fullPath); }
            while (!www.isDone) { yield return www; }

            Audio.clip = null;
            Audio.clip = www.GetAudioClip(false, false);
        }
    }
}
