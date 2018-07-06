using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TempestWave.Ingame
{
    public class HitText : MonoBehaviour
    {
        private float sustainTime;
        // Update is called once per frame
        void Update()
        {
            if (!gameObject.activeSelf) { return; }
            if (gameObject.transform.localScale.x < 1.0f) { gameObject.transform.localScale += new Vector3(5f * Time.deltaTime, 5f * Time.deltaTime, 0); }
            if (gameObject.transform.localScale.x >= 1.0f) { gameObject.transform.localScale = new Vector3(1f, 1f, 1f); }
            if (gameObject.transform.localScale.x >= 1.0f)
            {
                sustainTime += 1 * Time.deltaTime;
                if (sustainTime >= 0.3f) { gameObject.SetActive(false); }
            }
        }

        public void Wake(int value)
        {
            sustainTime = 0;
            if (gameObject.activeSelf.Equals(false)) { gameObject.SetActive(true); }
            if (value.Equals(0)) { gameObject.GetComponent<Text>().text = "miss"; gameObject.GetComponent<Text>().color = Color.gray; }
            else if (value.Equals(1)) { gameObject.GetComponent<Text>().text = "bad"; gameObject.GetComponent<Text>().color = Color.blue; }
            else if (value.Equals(2)) { gameObject.GetComponent<Text>().text = "nice"; gameObject.GetComponent<Text>().color = Color.yellow; }
            else if (value.Equals(3)) { gameObject.GetComponent<Text>().text = "great"; gameObject.GetComponent<Text>().color = Color.green; }
            else if (value.Equals(4)) { gameObject.GetComponent<Text>().text = "perfect"; gameObject.GetComponent<Text>().color = Color.cyan; }
            else if (value.Equals(5)) { gameObject.GetComponent<Text>().text = "tempestic"; gameObject.GetComponent<Text>().color = Color.magenta; }
            gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        }
    }
}
