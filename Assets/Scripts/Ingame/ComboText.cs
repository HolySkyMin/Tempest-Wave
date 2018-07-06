using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TempestWave.Ingame
{
    public class ComboText : MonoBehaviour
    {
        public GameObject t;

        void Update()
        {
            if (!gameObject.activeSelf) { return; }
            if (gameObject.transform.localScale.x > 1.0f) { gameObject.transform.localScale -= new Vector3(2f * Time.deltaTime, 2f * Time.deltaTime, 0); }
            if (gameObject.transform.localScale.x <= 1.0f) { gameObject.transform.localScale = new Vector3(1f, 1f, 1f); }
        }

        public void Wake(int value)
        {
            if (value < 2) { return; }
            if (gameObject.activeSelf.Equals(false)) { gameObject.SetActive(true); t.SetActive(true); }
            gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            gameObject.GetComponent<Text>().text = value.ToString();
        }

        public void Nuzzle()
        {
            gameObject.SetActive(false);
            t.SetActive(false);
        }
    }
}
