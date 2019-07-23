using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TempestWave.Data
{
    public class LocaleText : MonoBehaviour
    {
        private Text myText;
        public string myKey;

        void Start()
        {
            myText = gameObject.GetComponent<Text>();
            myText.text = LocaleManager.instance.GetLocaleText(myKey);
        }

        private void Update()
        {
            if (LocaleManager.instance.localeChanged.Equals(true)) { myText.text = LocaleManager.instance.GetLocaleText(myKey); }
        }
    }
}