using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TempestWave.Core;
using TempestWave.Core.UI;
using TempestWave.Data;
#if UNITY_ANDROID
using GooglePlayGames;
#elif UNITY_IOS
using UnityEngine.SocialPlatforms;
#endif

namespace TempestWave.Creator
{
    public class CreatorManager : MonoBehaviour
    {
        private readonly string[] TWxExtensions = { ".tw1", ".tw2", ".tw4", ".tw5", ".tw6" };

        public int CurrentListIndex { get; set; }
        public string CurrentFilePath { get; set; }

        public GameObject 
            FileObject,
            FilePanel;
        public RectTransform ListField;
        public Button LoadBtn;
        public GameObject
            LeftMainPanel,
            LeftCreatingPanel,
            RightPanel,
            RedLine;
        public Workspace Table;
        public Animator Anim;

        private List<FileButton> FileButtons = new List<FileButton>();

        private void Start()
        {
            LoadList();
            FilePanel.SetActive(true);

#if UNITY_ANDROID
            if (Social.localUser.authenticated) { PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_starting_line_as_a_creator, 100.0f, null); }
#elif UNITY_IOS
            Achievementer.ReportProgress("startingline");
#endif
        }

        public void LoadList()
        {
            CurrentListIndex = 0;
            while(FileButtons.Count > 0)
            {
                FileButton target = FileButtons[0];
                Destroy(target.gameObject);
                FileButtons.Remove(target);
            }

            if (!Directory.Exists(GamePath.CreatorPath())) { Directory.CreateDirectory(GamePath.CreatorPath()); }

            DirectoryInfo CreatorDir = new DirectoryInfo(GamePath.CreatorPath());
            FileInfo[] CreatorFiles = CreatorDir.GetFiles();

            for(int i = 0, idx = 0; i < CreatorFiles.Length; i++)
            {
                string ext = CreatorFiles[i].Extension;

                bool checkPassed = false;
                for(int j = 0; j < TWxExtensions.Length; j++)
                {
                    if(ext.Equals(TWxExtensions[j]))
                    {
                        checkPassed = true;
                        break;
                    }
                }
                if(checkPassed && File.Exists(CreatorFiles[i].FullName))
                {
                    GameObject newObj = Instantiate(FileObject);
                    newObj.SetActive(true);
                    FileButton fileBtn = newObj.GetComponent<FileButton>();
                    fileBtn.Index = idx++;
                    fileBtn.FilePath = CreatorFiles[i].FullName;
                    fileBtn.SetText(CreatorFiles[i].Name);
                    newObj.GetComponent<RectTransform>().SetParent(FileObject.transform.parent);
                    newObj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                    FileButtons.Add(fileBtn);
                }
            }
        }

        public void ReceiveFileData(int index, string fullPath)
        {
            CurrentFilePath = fullPath;
            FileButtons[CurrentListIndex].SetColor(GlobalTheme.ThemeColor(), GlobalTheme.ThemeContrastColor());
            FileButtons[index].SetColor(GlobalTheme.ThemeContrastColor(), GlobalTheme.ThemeColor());
            CurrentListIndex = index;

            if (!LoadBtn.interactable) { LoadBtn.interactable = true; }
        }

        public void EnterWithFile()
        {
            string fileName = FileButtons[CurrentListIndex].gameObject.GetComponentInChildren<Text>().text;
            if (Table.ReadFromTWx(CurrentFilePath, fileName.Substring(0, fileName.Length - 4)))
            {
                Anim.Play("Creator_ShowWorkspace");
                LeftCreatingPanel.SetActive(true);
                LeftMainPanel.SetActive(false);
                RightPanel.SetActive(true);
                RedLine.SetActive(true);
                Table.Initialize();
            }
            else
            {
                MessageBox.Show(LocaleManager.instance.GetLocaleText("error_occured"), LocaleManager.instance.GetLocaleText("creator_cannotload"), MessageBoxButton.OK);
            }
        }

        public void EnterClearly()
        {
            Anim.Play("Creator_ShowWorkspace");
            LeftCreatingPanel.SetActive(true);
            LeftMainPanel.SetActive(false);
            RightPanel.SetActive(true);
            RedLine.SetActive(true);
            Table.CleanBeforeInit();
            Table.Initialize();
        }

        public void BackToLobby()
        {
            Anim.Play("Creator_HideWorkspace");
            LeftCreatingPanel.SetActive(false);
            LeftMainPanel.SetActive(true);
            RightPanel.SetActive(false);
            RedLine.SetActive(false);

            Table.IsActive = false;
            LoadBtn.interactable = false;
            LoadList();
        }

        public void ShowHelp()
        {
            MessageBox.Show(LocaleManager.instance.GetLocaleText("creator_help"),
                LocaleManager.instance.GetLocaleText("creator_help_desc1") +
                LocaleManager.instance.GetLocaleText("creator_help_desc2") +
                LocaleManager.instance.GetLocaleText("creator_help_desc3") +
                LocaleManager.instance.GetLocaleText("creator_help_desc4") +
                LocaleManager.instance.GetLocaleText("creator_help_desc5"), MessageBoxButton.OK);
        }
    }
}
