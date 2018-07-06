using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TempestWave.Core.UI
{
    public class InstanceMaker : MonoBehaviour
    {
        public MessageBox BoxInstance;

        private void Awake()
        {
            MessageBox.SetInstance(BoxInstance);
        }
    }
}
