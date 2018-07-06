using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TempestWave.Ingame.Mobile
{
    public class LightHitEffect : MonoBehaviour
    {
        private bool isActive = false;
        private float t = 0;
        public SpriteRenderer sprite;

        // Update is called once per frame
        void Update()
        {
            if(isActive.Equals(true))
            {
                if(t < 5)
                {
                    gameObject.transform.localScale += new Vector3(180 * Time.deltaTime, 180 * Time.deltaTime, 0);
                }
                if(t >= 6)
                {
                    sprite.color -= new Color(0, 0, 0, ((float)40 / 9) * Time.deltaTime);
                }
                if(t >= 15)
                {
                    isActive = false;
                    gameObject.SetActive(false);
                }
                t += 60 * Time.deltaTime;
            }
        }

        public void Wake()
        {
            isActive = true;
            gameObject.transform.localScale = new Vector3(20, 20, 1);
            sprite.color = new Color(1, 1, 1, (float)2 / 3);
            gameObject.SetActive(true);
            t = 0;
        }
    }
}
