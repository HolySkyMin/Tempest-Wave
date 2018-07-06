using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TempestWave.Data
{
    public class GameDataArchiver : MonoBehaviour
    {
        private float maxPercent, noteSpeed, gameSync;
        private bool hitSound, tempestic;
        private int maxScore, maxCombo, noteCount, flickSevere;
        private int[] judgeCount = new int[6];

        void Start()
        {
            // 초기화합니다.
            maxPercent = 0;
            maxScore = 0;
            maxCombo = 0;
            noteCount = 0;
            judgeCount[0] = 0; // MISS
            judgeCount[1] = 0; // BAD
            judgeCount[2] = 0; // NICE
            judgeCount[3] = 0; // GREAT
            judgeCount[4] = 0; // PERFECT
            judgeCount[5] = 0; // TEMPESTIC
        }

        public void TryUpdateCombo(int newCombo)
        {
            if (maxCombo < newCombo) { maxCombo = newCombo; }
        }

        public int GetMaxCombo()
        {
            return maxCombo;
        }

        public int[] GetAllNoteDecision()
        {
            return judgeCount;
        }

        public void UpdateNote(int value)
        {
            judgeCount[value]++;
        }

        public int GetNoteCount()
        {
            return noteCount;
        }

        public void SetNoteCount(int value)
        {
            noteCount = value;
        }

        public int GetMaxScore()
        {
            return maxScore;
        }

        public void SetMaxScore(int value)
        {
            maxScore = value;
        }

        public float GetMaxPercent()
        {
            return maxPercent;
        }

        public void SetMaxPercent(float value)
        {
            maxPercent = value;
        }

        public void SetNoteSpeed(float value) { noteSpeed = value; }
        public void SetGameSync(float value) { gameSync = value; }
        public void SetHitSound(bool value) { hitSound = value; }
        public void SetTempestic(bool value) { tempestic = value; }
        public void SetFlickMode(int value) { flickSevere = value; }

        public void ResultPopOut(out float nS, out float gS, out bool hS, out bool tP, out int fS)
        {
            nS = noteSpeed;
            gS = gameSync;
            hS = hitSound;
            tP = tempestic;
            fS = flickSevere;
        }
    }
}