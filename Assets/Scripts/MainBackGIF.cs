using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TempestWave
{
    public class MainBackGIF : MonoBehaviour
    {
        public Texture[] seperated = new Texture[43];
        private float cnt;

        // Use this for initialization
        void Start()
        {
            cnt = 0;
        }

        void Update()
        {
            gameObject.GetComponent<RawImage>().texture = seperated[Mathf.RoundToInt(cnt)];
            cnt += 30 * Time.deltaTime;
            if (cnt >= 42.5f) { cnt = 0f; }
        }
    }
}
