using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TempestWave.Creator
{
    public class TailPosUpdator : MonoBehaviour
    {
        public GameObject HeadNote { get; set; }
        public GameObject TailNote { get; set; }

        public LineRenderer LineDrawer;

        private void LateUpdate()
        {
            try
            {
                LineDrawer.SetPosition(0, HeadNote.transform.position);
                LineDrawer.SetPosition(1, TailNote.transform.position);
            }
            catch { Destroy(gameObject); }
        }
    }
}
