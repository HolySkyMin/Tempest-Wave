using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TempestWave.Core
{
    public class ObjectTagger : MonoBehaviour
    {
        public string Tag;

        private void Awake()
        {
            gameObject.tag = Tag;
        }
    }
}