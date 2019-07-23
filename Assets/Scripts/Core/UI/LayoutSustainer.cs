using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TempestWave.Core.UI
{
    public class LayoutSustainer : MonoBehaviour
    {
        public bool Left, Right;

        private void Start()
        {
            float width, height, safeWidth, safeHeight;
            if (Screen.width / (float)Screen.height < 16f / 9f)
            {
                width = 1280 * (Screen.height / Screen.width);
                height = 1280;
            }
            else
            {
                width = 720;
                height = 720 * (Screen.width / Screen.height);
            }
            safeWidth = width * (Screen.safeArea.width / Screen.width);
            safeHeight = height * (Screen.safeArea.height / Screen.height);

            if (width > safeWidth)
            {
                if (Left)
                    gameObject.GetComponent<RectTransform>().offsetMin += new Vector2((width - safeWidth) / 2f, 0);
                if (Right)
                    gameObject.GetComponent<RectTransform>().offsetMax -= new Vector2((width - safeWidth) / 2f, 0);
            }

            //if (Left)
            //    gameObject.GetComponent<RectTransform>().offsetMin += new Vector2(50, 0);
            //if (Right)
            //    gameObject.GetComponent<RectTransform>().offsetMax += new Vector2(-50, 0);
        }
    }

}