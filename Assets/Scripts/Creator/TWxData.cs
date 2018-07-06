using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TempestWave.Creator
{
    public class TWxData
    {
        public int version { get; set; }
        public TWxMetadata metadata { get; set; }
        public TWxNote[] notes { get; set; }

        public TWxData()
        {
            version = 2;
        }
    }

    public class TWxMetadata
    {
        public int[] bpm { get; set; }
        public int[] bpmQueue { get; set; }
        public int[] beats { get; set; }
        public int[] beatsQueue { get; set; }
        public int offset { get; set; }
        public int level { get; set; }
        public string artist { get; set; }
        public string mapper { get; set; }
        public int density { get; set; }

        public TWxMetadata()
        {

        }

        public void SetValue(int[] b, int[] q, int[] beat, int[] beatQ, int off, int lv, string art, string map, int den)
        {
            bpm = b;
            bpmQueue = q;
            beats = beat;
            beatsQueue = beatQ;
            offset = off;
            level = lv;
            artist = art;
            mapper = map;
            density = den;
        }
    }

    public class TWxNote
    {
        public int YPos { get; set; }
        public int ID { get; set; }
        public int Size { get; set; }
        public byte[] Color { get; set; }
        public int Mode { get; set; }
        public int Flick { get; set; }
        public double Time { get; set; }
        public double Speed { get; set; }
        public double StartLine { get; set; }
        public double EndLine { get; set; }
        public int[] PrevIDs { get; set; }

        public TWxNote()
        {
            Color = new byte[4];
        }

        public TWxNote(int prevLength)
        {
            Color = new byte[4];
            PrevIDs = new int[prevLength];
        }

        public void SetValue(int ypos, int id, int size, byte[] color, int mode, int flick, float time, float speed, float startline, float endline, int[] prevs)
        {
            YPos = ypos;
            ID = id;
            Size = size;
            Color = color;
            Mode = mode;
            Flick = flick;
            Time = time;
            Speed = speed;
            StartLine = startline;
            EndLine = endline;
            PrevIDs = prevs;
        }
    }
}
