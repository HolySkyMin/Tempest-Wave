using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TempestWave
{
    public class VerticalScaleControl : MonoBehaviour
    {
        public CanvasScaler scaler;
        public List<RectTransform> NotchSizeScale;

        private void Start()
        {
            SetScale(Screen.height, Screen.width);
            FitToNotch(Screen.height, Screen.width, Screen.safeArea.height, Screen.safeArea.width);
        }

        public void SetScale(float width, float height)
        {
            if (width / height <= 9f / 16f)
            {
                scaler.matchWidthOrHeight = 0f;
                Camera.main.orthographicSize = (720 * (height / width)) / 2f;
            }
            else
            {
                scaler.matchWidthOrHeight = 1f;
            }
        }

        private void FitToNotch(float originWidth, float originHeight, float safeOriginWidth, float safeOriginHeight)
        {
            float width, height, safeWidth, safeHeight;
            if (scaler.matchWidthOrHeight.Equals(0))
            {
                width = 720;
                height = 720 * (originHeight / originWidth);
            }
            else
            {
                width = 1280 * (originWidth / originHeight);
                height = 1280;
            }
            safeWidth = width * (safeOriginWidth / originWidth);
            safeHeight = height * (safeOriginHeight / originHeight);

            foreach(RectTransform curRect in NotchSizeScale)
            {
                curRect.sizeDelta = new Vector2(curRect.sizeDelta.x, curRect.sizeDelta.y + (height - safeHeight) / 2);
            }
        }
    }
}
