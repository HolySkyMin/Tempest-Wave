using UnityEngine;
using TempestWave.Core.UI;
using System.IO;

namespace TempestWave.Core
{
    public class GamePath
    {
        public static string SongPath()
        {
            if (Application.platform.Equals(RuntimePlatform.WindowsPlayer)) { return "./Songs/"; }
            else if (Application.platform.Equals(RuntimePlatform.Android))
            {
                if (PlayerPrefs.HasKey("storage") && PlayerPrefs.GetString("storage").Equals("external"))
                {
                    if (PlayerPrefs.HasKey("sdcard") && Directory.Exists("/storage/" + PlayerPrefs.GetString("sdcard") + "/TempestWave/Songs/")) { return "/storage/" + PlayerPrefs.GetString("sdcard") + "/TempestWave/Songs/"; }
                    else { return "/storage/sdcard1/TempestWave/Songs/"; }
                }
                else { return "/storage/emulated/0/TempestWave/Songs/"; }
            }
            else if (Application.platform.Equals(RuntimePlatform.IPhonePlayer)) { return Application.persistentDataPath + "/Songs/"; }
            else
            {
                MessageBox.Show("Error", "Error.", new MessageBoxButtonType[] { MessageBoxButtonType.OK });
                return null;
            }
        }

        public static string CreatorPath()
        {
            if (Application.platform.Equals(RuntimePlatform.WindowsPlayer)) { return "./Creator/"; }
            else if (Application.platform.Equals(RuntimePlatform.Android))
            {
                return "/storage/emulated/0/TempestWave/Creator/";
            }
            else if (Application.platform.Equals(RuntimePlatform.IPhonePlayer)) { return Application.persistentDataPath + "/Creator/"; }
            else
            {
                MessageBox.Show("Error", "Error.", new MessageBoxButtonType[] { MessageBoxButtonType.OK });
                return null;
            }
        }

        public static string LanguagePath()
        {
            if (Application.platform.Equals(RuntimePlatform.WindowsPlayer)) { return "./Languages/"; }
            else if (Application.platform.Equals(RuntimePlatform.Android))
            {
                if (PlayerPrefs.HasKey("storage") && PlayerPrefs.GetString("storage").Equals("external"))
                {
                    if (PlayerPrefs.HasKey("sdcard") && Directory.Exists("/storage/" + PlayerPrefs.GetString("sdcard") + "/TempestWave/Languages/")) { return "/storage/" + PlayerPrefs.GetString("sdcard") + "/TempestWave/Languages/"; }
                    else { return "/storage/sdcard1/TempestWave/Languages/"; }
                }
                else { return "/storage/emulated/0/TempestWave/Languages/"; }
            }
            else if (Application.platform.Equals(RuntimePlatform.IPhonePlayer)) { return Application.persistentDataPath + "/Languages/"; }
            else
            {
                MessageBox.Show("Error", "Error.", new MessageBoxButtonType[] { MessageBoxButtonType.OK });
                return null;
            }
        }
    }
}

