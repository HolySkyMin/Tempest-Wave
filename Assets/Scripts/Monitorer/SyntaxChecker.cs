using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TempestWave.Monitorer
{
    public class SyntaxChecker : MonoBehaviour
    {
        private bool[] longNoteStarted = new bool[7];
        private int[] WhereItStarted = new int[7];

        void Start()
        {
            // 초기화합니다.
            for(int i = 0; i < 7; i++)
            {
                longNoteStarted[i] = false;
                WhereItStarted[i] = 0;
            }
        }

        public bool Check(float frame, int type, int start, int end)
        {
            if (type.Equals(1))
            {
                if (longNoteStarted[end].Equals(true)) { return false; }
                else { return true; }
            }
            else if (type.Equals(2))
            {
                if (longNoteStarted[end].Equals(true)) { return false; }
                else
                {
                    longNoteStarted[end] = true;
                    WhereItStarted[end] = start;
                    return true;
                }
            }
            else if (type.Equals(3))
            {
                if (longNoteStarted[end].Equals(true))
                {
                    if (WhereItStarted[end].Equals(start))
                    {
                        longNoteStarted[end] = false;
                        return true;
                    }
                    else { return false; }
                }
                else { return false; }
            }
            else if (type.Equals(9))
            {
                return true;
            }
            else { return false; }
        }
    }
}
