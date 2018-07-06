using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TempestWave.Ingame
{
    public class IngameBackGIF : MonoBehaviour
    {
        public Sprite[] background = new Sprite[43];
        private float cnt;

        // Use this for initialization
        void Start()
        {
            cnt = 0f;

            float a = (float)Screen.width / Screen.height;
            float curY = gameObject.transform.localScale.y;
            gameObject.transform.localScale = new Vector3(curY * (a / (16f / 9f)), curY, 1);
        }

        // Update is called once per frame
        void Update()
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = background[Mathf.RoundToInt(cnt)];
            cnt += 30 * Time.deltaTime;
            if (cnt >= 42.5f) { cnt = 0f; }
        }
    }
}
