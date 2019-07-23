using UnityEngine;
using UnityEngine.UI;

namespace TempestWave.Creator
{
    public class FileButton : MonoBehaviour
    {
        public int Index { get; set; }
        public string FilePath { get; set; }

        public Text ButtonText;
        public CreatorManager Manager;

        public void SetText(string fileName)
        {
            ButtonText.text = fileName;
        }

        public void SetColor(Color bodyColor, Color textColor)
        {
            gameObject.GetComponent<Image>().color = bodyColor;
            ButtonText.color = textColor;
        }

        public void Clicked()
        {
            Manager.ReceiveFileData(Index, FilePath);
        }
    }
}