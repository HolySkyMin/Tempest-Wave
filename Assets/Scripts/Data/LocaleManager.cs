using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TempestWave.Core.Language;
using TempestWave.Core.UI;

namespace TempestWave.Data
{
    public class LocaleManager : MonoBehaviour
    {
        public static LocaleManager instance;

        public delegate void Prepare();

        public int CurrentIndex { get; set; }
        public bool LocaleExists { get; set; }
        public Prepare PrepareInit { get; set; }
        public TextAsset[] textData = new TextAsset[3];
        public List<LocaleData> Locales = new List<LocaleData>();
        public bool localeChanged = false;
        private Dictionary<string, string> localeText = new Dictionary<string, string>();
        private int curCode;
        private string missingText = "No text.";

        private void Awake()
        {
            if (instance == null) { instance = this; }
            else if (instance != this) { Destroy(gameObject); }

            DontDestroyOnLoad(gameObject);
        }
	
	    public void LoadLocale()
        {
            LanguageLoader.LoadInternalLanguages(instance);
            LanguageLoader.LoadExternalLanguages(instance);

            if (PlayerPrefs.HasKey("locale") && PlayerPrefs.GetInt("locale") < Locales.Count) { SetLocale(PlayerPrefs.GetInt("locale")); }
            else
            {
                SelectLocale(true);
                PlayerPrefs.SetInt("locale", CurrentIndex);
                PlayerPrefs.Save();
            }
        }

        public void SetLocale(int index)
        {
            for (int i = 0; i < Locales[index].items.Length; i++)
            {
                //Debug.Log("Index: " + index + ", key: " + Locales[index].items[i].key + ", value: " + Locales[index].items[i].value);
                if (!localeText.ContainsKey(Locales[index].items[i].key)) { localeText.Add(Locales[index].items[i].key, Locales[index].items[i].value); }
                else { localeText[Locales[index].items[i].key] = Locales[index].items[i].value; }
            }
            localeChanged = true;
            LocaleExists = true;
            CurrentIndex = index;
            PrepareInit();
            StartCoroutine(WaitToApply());
        }

        IEnumerator WaitToApply()
        {
            yield return new WaitForSeconds(0.05f);
            instance.localeChanged = false;
        }

        public static void SelectLocale(bool firstTime)
        {
            if (firstTime) { MessageBox.Show("Languages", "Please select your language.", new MessageBoxButtonType[] { MessageBoxButtonType.Language }); }
            else { MessageBox.Show("Languages", "Please select your language.", new MessageBoxButtonType[] { MessageBoxButtonType.Language, MessageBoxButtonType.Cancel }); }
            instance.StartCoroutine(MessageAct_SelectLocale());
        }

        public static IEnumerator MessageAct_SelectLocale()
        {
            yield return new WaitUntil(() => MessageBox.Instance.ResultExists == true);
            if(MessageBox.Instance.Result.Equals(MessageBoxButtonType.Language))
            {
                instance.SetLocale(MessageBox.Instance.LanguageIndex);
            }
        }

        public string GetLocaleText(string key)
        {
            string dat = missingText;
            if (localeText.ContainsKey(key)) { dat = localeText[key]; }
            return dat;
        }
    }
}