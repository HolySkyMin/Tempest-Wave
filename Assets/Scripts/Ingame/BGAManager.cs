using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TempestWave.Data;

namespace TempestWave.Ingame
{
    public class BGAManager : MonoBehaviour
    {
        public bool isCustom = false;
        public bool readyToPlay = false;
        public VideoPlayer player;
        public RawImage backImage;
        public GameObject bgaText, errorText, defaultScreen;

        // Use this for initialization
        public void Initialize()
        {
            bool isValid = true;
            if(DataSender.ReturnNoBGA().Equals(false) && isValid.Equals(true) && errorText.activeSelf.Equals(false))
            {
                //string basepath = DataSender.ReturnBasePath();
                //basepath = basepath.Replace('\\', '/');
                if (DataSender.ReturnBGAPath().Length > 0 && File.Exists(DataSender.ReturnBGAPath()))
                {
                    isCustom = true;
                    bgaText.SetActive(true);
                    player.source = VideoSource.Url;
                    if (Application.platform.Equals(RuntimePlatform.WindowsPlayer)) { player.url = DataSender.ReturnBGAPath().Replace("\\", "/"); }
                    else { player.url = "file://" + DataSender.ReturnBGAPath().Replace("\\", "/"); }
                    //player.url = basepath + ".mp4";
                    Debug.Log(player.url);
                    player.playbackSpeed = DataSender.ReturnSpeedAmp();
                    defaultScreen.SetActive(false);
                    player.prepareCompleted += Preparing;
                    player.Prepare();
                }
                else
                {
                    isCustom = false;
                    if (DataSender.ReturnBackImgPath().Length > 0 && File.Exists(DataSender.ReturnBackImgPath())) { StartCoroutine(LoadBackImage(DataSender.ReturnBackImgPath())); }
                }
            }
        }

        void Preparing(VideoPlayer p)
        {
            readyToPlay = true;
            bgaText.SetActive(false);
        }

        IEnumerator LoadBackImage(string path)
        {
            string modifiedPath;
            if (Application.platform.Equals(RuntimePlatform.WindowsPlayer)) { modifiedPath = path.Replace("\\", "/"); }
            else { modifiedPath = "file://" + path.Replace("\\", "/"); }

            using (WWW www = new WWW(modifiedPath))
            {
                yield return www;
                backImage.texture = www.texture;
                backImage.gameObject.SetActive(true);
            }
        }

        public void PauseBGA()
        {
            if (isCustom.Equals(true)) { player.Pause(); }
        }

        public void ResumeBGA()
        {
            if (isCustom.Equals(true)) { player.Play(); }
        }

        public void SetStartFrame()
        {
            if (isCustom) { player.frame = Mathf.RoundToInt(DataSender.ReturnBGAFrame() * (1 / DataSender.ReturnSpeedAmp()) * 60f / 1000f); }
        }

        private int GetSDKLevel()
        {
            var clazz = AndroidJNI.FindClass("android.os.Build$VERSION");
            var fieldID = AndroidJNI.GetStaticFieldID(clazz, "SDK_INT", "I");
            var sdkLevel = AndroidJNI.GetStaticIntField(clazz, fieldID);
            return sdkLevel;
        }
    }
}