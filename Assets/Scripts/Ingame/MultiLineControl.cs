using UnityEngine;
using System.Collections;
using TempestWave.Ingame.Mobile;

namespace TempestWave.Ingame
{
    public class MultiLineControl : MonoBehaviour
    {
        public GameObject left, right;
        public LineRenderer render;

        public void Set(GameObject l, GameObject r)
        {
            left = l;
            right = r;
        }

        void Update()
        {
            if (left.activeSelf.Equals(true) && right.activeSelf.Equals(true))
            {
                render.SetPosition(0, left.GetComponent<RectTransform>().anchoredPosition3D + new Vector3(0, 0, 1));
                render.SetPosition(1, right.GetComponent<RectTransform>().anchoredPosition3D + new Vector3(0, 0, 1));
                render.startWidth = left.transform.localScale.x * 10;
                render.endWidth = right.transform.localScale.x * 10;

                if (left.GetComponent<Note>().IsHit.Equals(true) || right.GetComponent<Note>().IsHit.Equals(true))
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
