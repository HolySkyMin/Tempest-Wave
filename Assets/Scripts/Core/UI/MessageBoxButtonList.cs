using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TempestWave.Data;

namespace TempestWave.Core.UI
{
    public class MessageBoxButtonList : MonoBehaviour
    {
        public MessageBox Parent;
        public RectTransform Container;
        public GameObject Template, LanguageBase;

        private List<GameObject> Objects = new List<GameObject>();

        public void Add(MessageBoxButtonType buttonType)
        {
            GameObject NewButton = Instantiate(Template) as GameObject;
            NewButton.SetActive(true);

            MessageBoxButton data = NewButton.GetComponent<MessageBoxButton>();
            data.Parent = Parent;
            data.Type = buttonType;
            data.SetText();

            NewButton.transform.SetParent(Container);
            NewButton.transform.localScale = new Vector3(1, 1, 1);
            Objects.Add(NewButton);
        }

        public void AddLanguage()
        {
            for(int i = 0; i < LocaleManager.instance.Locales.Count; i++)
            {
                GameObject NewButton = Instantiate(LanguageBase) as GameObject;
                NewButton.SetActive(true);

                LanguageButton data = NewButton.GetComponent<LanguageButton>();
                data.Parent = Parent;
                data.Type = MessageBoxButtonType.Language;
                data.Index = i;
                data.SetText();
                Debug.Log("Added lang button: index " + data.Index + ", name: " + data.ButtonText.text);

                NewButton.transform.SetParent(Container);
                NewButton.transform.localScale = new Vector3(1, 1, 1);
                Objects.Add(NewButton);
            }
        }

        public void Clear()
        {
            if (Objects.Count > 0)
            {
                for(int i = 0; i < Objects.Count; i++)
                {
                    Objects[i].SetActive(false);
                }
                Objects.Clear();
            }
        }
    }

}