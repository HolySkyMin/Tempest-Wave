using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TempestWave.SelectScreen
{
    public class TagButton : MonoBehaviour
    {
        public Image tagBackground;
        public Text tagText;

        public void AddInfo(string textdat)
        {
            tagText.text = textdat;
        }

        public void ChangeBackColor(Color color)
        {
            tagBackground.color = color;
        } 
    }
}
