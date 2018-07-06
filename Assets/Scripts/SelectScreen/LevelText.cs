using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TempestWave.SelectScreen
{
    public class LevelText : MonoBehaviour
    {
        private bool isAnimated;
        private int curIndex;
        private float repKey;
        public Text[] levelTexts = new Text[3];

        private void Update()
        {
            if(isAnimated.Equals(true))
            {
                if(curIndex.Equals(0))
                {
                    levelTexts[0].color = new Color(0f, 0.9f + (0.1f * Mathf.Cos((repKey / 20) * Mathf.PI)), 0.9f + (0.1f * Mathf.Cos((repKey / 20) * Mathf.PI)));
                }
                else if(curIndex.Equals(1))
                {
                    levelTexts[1].color = new Color(0.9f + (0.1f * Mathf.Cos((repKey / 20) * Mathf.PI)), 0.9f + (0.1f * Mathf.Cos((repKey / 20) * Mathf.PI)), 0f);
                }
                else if(curIndex.Equals(2))
                {
                    levelTexts[2].color = new Color(0.9f + (0.1f * Mathf.Cos((repKey / 20) * Mathf.PI)), 0f, 0f);
                }
                repKey += 60 * Time.deltaTime;
                if (repKey >= 400) { repKey -= 400f; }
            }
        }

        public void SetColor(int index, bool animate)
        {
            curIndex = index;
            isAnimated = animate;
            //Debug.Log("Level text info: curIndex=" + curIndex + ", animate=" + isAnimated);
            repKey = 0;

            for(int i = 0; i < 3; i++)
            {
                if (i.Equals(index))
                {
                    if (index.Equals(0)) { levelTexts[i].color = Color.cyan; }
                    else if (index.Equals(1)) { levelTexts[i].color = Color.yellow; }
                    else if (index.Equals(2)) { levelTexts[i].color = Color.red; }
                }
                else { levelTexts[i].color = Color.white; }
            }
        }
    }
}
