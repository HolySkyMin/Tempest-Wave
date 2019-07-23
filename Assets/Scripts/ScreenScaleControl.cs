using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TempestWave
{
    public class ScreenScaleControl : MonoBehaviour
    {
        public CanvasScaler scaler;
        public List<RectTransform> NotchSizeScale;

        void Start()
        {
            SetScale(Screen.width, Screen.height);
            FitToNotch(Screen.width, Screen.height, Screen.safeArea.width, Screen.safeArea.height);
        }

        public void SetScale(int width, int height)
        {
            if (width / (float)height >= 16f / 9f)
            {
                scaler.matchWidthOrHeight = 1f;
            }
            else
            {
                scaler.matchWidthOrHeight = 0f;
                Camera.main.orthographicSize = (1280 * (height / (float)width)) / 2f;
            }
        }

        private void FitToNotch(float originWidth, float originHeight, float safeOriginWidth, float safeOriginHeight)
        {
            float width, height, safeWidth, safeHeight;
            if (scaler.matchWidthOrHeight.Equals(0))
            {
                width = 1280;
                height = 1280 * (originHeight / originWidth);
            }
            else
            {
                width = 720 * (originWidth / originHeight);
                height = 720;
            }
            safeWidth = width * (safeOriginWidth / originWidth);
            safeHeight = height * (safeOriginHeight / originHeight);

            foreach (RectTransform curRect in NotchSizeScale)
            {
                curRect.sizeDelta += new Vector2((width - safeWidth) / 2, (height - safeHeight) / 2);
            }

            //foreach(RectTransform curRect in NotchSizeScale)
            //{
            //    curRect.sizeDelta += new Vector2(50, 0);
            //}
        }
    }
}