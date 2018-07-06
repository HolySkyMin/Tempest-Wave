using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TempestWave.Data;
using LitJson;

namespace TempestWave.Core.Language
{
    public class LanguageLoader
    {
        public static void LoadInternalLanguages(LocaleManager manager)
        {
            manager.Locales.Clear();

            for(int i = 0; i < manager.textData.Length; i++)
            {
                LocaleData data = JsonMapper.ToObject<LocaleData>(manager.textData[i].text);
                manager.Locales.Add(data);
            }

            if (Directory.Exists(GamePath.LanguagePath()))
            {
                DirectoryInfo dir = new DirectoryInfo(GamePath.LanguagePath());
                FileInfo[] files = dir.GetFiles();

                for (int i = 0; i < files.Length; i++)
                {
                    Debug.Log("External locale file: " + files[i].FullName);
                    StreamReader reader = new StreamReader(files[i].FullName);
                    LocaleData data = JsonMapper.ToObject<LocaleData>(reader);
                    manager.Locales.Add(data);
                    reader.Close();
                }
            }

            Debug.Log("Totla locale count: " + manager.Locales.Count);
        }

        public static void LoadExternalLanguages(LocaleManager manager)
        {

        }
    }
}
