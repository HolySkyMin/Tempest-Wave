using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using TempestWave.Data;
using TempestWave.Monitorer;

namespace TempestWave.Ingame
{
    public class SongPlayer : MonoBehaviour
    {
        public GameManager Game;
        public GameObject container;
        public Text errorText;

        IEnumerator loadingSong()
        {
            WWW www = null;
            if (DataSender.ReturnWavPath().Length > 0 && File.Exists(DataSender.ReturnWavPath()))
            {
                //Debug.Log(DataSender.ReturnWavPath());
                if (Application.platform.Equals(RuntimePlatform.WindowsPlayer)) { www = new WWW(DataSender.ReturnWavPath()); }
                else { www = new WWW("file://" + DataSender.ReturnWavPath()); }
            }
            else if (DataSender.ReturnOggPath().Length > 0 && File.Exists(DataSender.ReturnOggPath()))
            {
                Debug.Log(DataSender.ReturnOggPath());
                if (Application.platform.Equals(RuntimePlatform.WindowsPlayer)) { www = new WWW(DataSender.ReturnOggPath()); }
                else { www = new WWW("file://" + DataSender.ReturnOggPath()); }
            }
            else if ((Application.platform.Equals(RuntimePlatform.Android) || Application.platform.Equals(RuntimePlatform.IPhonePlayer)) && DataSender.ReturnMp3Path().Length > 0 && File.Exists(DataSender.ReturnMp3Path())) { Debug.Log(DataSender.ReturnMp3Path()); www = new WWW("file://" + DataSender.ReturnMp3Path()); }
            else { Game.ThrowError(ErrorMode.NoMusicFile); }
            yield return www;

            container.GetComponent<AudioSource>().clip = www.GetAudioClip(false, false);
            if (PlayerPrefs.HasKey("musicvol").Equals(true)) { container.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("musicvol"); }
            else { container.GetComponent<AudioSource>().volume = 1.0f; }
            container.GetComponent<AudioSource>().pitch = Time.timeScale * (DataSender.ReturnSpeedAmp());
        }

        public void LoadMusic(ref bool nomusic)
        {
            if (nomusic.Equals(false))
            {
                if (!File.Exists(DataSender.ReturnWavPath()) && !File.Exists(DataSender.ReturnOggPath()))
                {
                    if ((Application.platform.Equals(RuntimePlatform.Android) || Application.platform.Equals(RuntimePlatform.IPhonePlayer)) && File.Exists(DataSender.ReturnMp3Path())) { nomusic = false; }
                    else { nomusic = true; }
                }
            }
            if(nomusic.Equals(false))
            {
                StartCoroutine(loadingSong());
            }
        }
    }
}
