using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TempestWave.Ingame;

namespace TempestWave.Creator
{
    public class CreatorNote : MonoBehaviour, IComparable<CreatorNote>
    {
        public int FinalIndex { get; set; }
        public int YPos { get; set; }
        public int Size { get; set; }
        public int Type { get; set; }
        public int Flick { get; set; }
        public float StartPoint { get; set; }
        public float EndPoint { get; set; }
        public float Speed { get; set; }
        public Color32 NoteColor { get; set; }
        public GameObject NextTail { get; set; }
        public CreatorNote NextNote { get; set; }
        public List<GameObject> PreviousTails { get; set; }
        public List<CreatorNote> PreviousNotes { get; set; }

        public RectTransform Body;
        public Image 
            SelectedTexture,
            NoteTexture,
            WhiteTexture;
        public Text
            SpeedText,
            StartLineText;
        public Block ParentBlock;

        public bool Selected { get; set; }
        
        public CreatorNote()
        {
            PreviousTails = new List<GameObject>();
            PreviousNotes = new List<CreatorNote>();
            Selected = false;
        }

        public int CompareTo(CreatorNote target)
        {
            if (target == null) { return 1; }
            return YPos.CompareTo(target.YPos);
        }

        public void UpdateInfoToUI()
        {
            if (Size.Equals(0))
            {
                NoteTexture.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 1);
                WhiteTexture.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 1);
            }
            else if(Size.Equals(1))
            {
                NoteTexture.gameObject.transform.localScale = new Vector3(1, 1, 1);
                WhiteTexture.gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
            else if(Size.Equals(2))
            {
                Body.localScale = new Vector3(1.3f, 1.3f, 1);
                Type = 0;
                Flick = 0;
                NoteTexture.sprite = ParentBlock.Table.SpecialNote;
                WhiteTexture.sprite = ParentBlock.Table.SpecialWhiteNote;
            }
            SpeedText.text = Speed.ToString("N1");

            if (StartPoint.Equals(0)) { StartPoint = EndPoint; }
            StartLineText.text = StartPoint.ToString("N1");
        }

        public void ChangeSelected()
        {
            Selected = Selected ? false : true;
            if (Selected) { SelectedTexture.gameObject.SetActive(true); }
            else { SelectedTexture.gameObject.SetActive(false); }
        }

        public void Clean()
        {
            if (NextTail != null) { Destroy(NextTail); }

            if(Type.Equals(1))
            {
                if (NextNote != null && NextNote.Type.Equals(Type)) { NextNote.ParentBlock.DeleteNote(NextNote.gameObject); }
                if(PreviousNotes.Count > 0)
                {
                    for(int i = 0; i < PreviousNotes.Count; i++)
                    {
                        if (PreviousNotes[i].Type.Equals(Type))
                        {
                            CreatorNote refer = PreviousNotes[i];
                            PreviousNotes.RemoveAt(i--);
                            refer.ParentBlock.DeleteNote(refer.gameObject);
                        }
                    }
                }
            }
            else if(Type.Equals(2))
            {
                if(NextNote != null && NextNote.Type.Equals(Type))
                {
                    NextNote.PreviousNotes.Remove(this);
                    if (PreviousNotes.Count > 0)
                    {
                        for (int i = 0; i < PreviousNotes.Count; i++)
                        {
                            if (PreviousNotes[i].Type.Equals(Type))
                            {
                                PreviousNotes[i].NextNote = NextNote;
                                NextNote.PreviousNotes.Add(PreviousNotes[i]);
                                PreviousNotes[i].NextTail.GetComponent<LineRenderer>().SetPosition(1, NextNote.Body.position);
                                PreviousNotes[i].NextTail.GetComponent<TailPosUpdator>().TailNote = NextNote.gameObject;
                                NextNote.PreviousTails.Add(PreviousNotes[i].NextTail);
                            }
                        }
                    }
                }
                else
                {
                    if (PreviousNotes.Count > 0)
                    {
                        for (int i = 0; i < PreviousNotes.Count; i++)
                        {
                            if (PreviousNotes[i].Type.Equals(Type))
                            {
                                PreviousNotes[i].NextNote = null;
                                if (ParentBlock.Table.NotePlaceMode.Equals(2) && ParentBlock.Table.LastSlideNote.Equals(this)) { ParentBlock.Table.LastSlideNote = PreviousNotes[i]; }
                            }
                        }
                    }
                    if (ParentBlock.Table.NotePlaceMode.Equals(2) && ParentBlock.Table.LastSlideNote.Equals(this))
                    {
                        ParentBlock.Table.LastSlideNote = null;
                        ParentBlock.Table.PlaceModeChanged(0);
                    }
                }
            }

            if(!Flick.Equals(0))
            {
                if(NextNote != null)
                {
                    NextNote.PreviousNotes.Remove(this);
                    if(PreviousNotes.Count > 0)
                    {
                        for(int i = 0; i < PreviousNotes.Count; i++)
                        {
                            if(!PreviousNotes[i].Flick.Equals(0))
                            {
                                PreviousNotes[i].NextNote = NextNote;
                                NextNote.PreviousNotes.Add(PreviousNotes[i]);
                                PreviousNotes[i].NextTail.GetComponent<LineRenderer>().SetPosition(1, NextNote.Body.position);
                                PreviousNotes[i].NextTail.GetComponent<TailPosUpdator>().TailNote = NextNote.gameObject;
                                NextNote.PreviousTails.Add(PreviousNotes[i].NextTail);
                            }
                        }
                    }
                }
                else
                {
                    if (PreviousNotes.Count > 0)
                    {
                        for (int i = 0; i < PreviousNotes.Count; i++)
                        {
                            if (!PreviousNotes[i].Flick.Equals(0))
                            {
                                PreviousNotes[i].NextNote = null;
                                if (ParentBlock.Table.NotePlaceMode.Equals(3) && ParentBlock.Table.LastFlickNote.Equals(this)) { ParentBlock.Table.LastFlickNote = PreviousNotes[i]; }
                            }
                        }
                    }
                    if (ParentBlock.Table.NotePlaceMode.Equals(3) && ParentBlock.Table.LastFlickNote.Equals(this))
                    {
                        ParentBlock.Table.LastFlickNote = null;
                        ParentBlock.Table.PlaceModeChanged(0);
                    }
                }
            }
        }

        public void ApplyColor()
        {
            WhiteTexture.color = NoteColor;
        }

        public void SwitchViewMode()
        {
            if(ParentBlock.Table.ViewSpecific)
            {
                NoteTexture.gameObject.SetActive(false);
                WhiteTexture.gameObject.SetActive(true);
                SpeedText.gameObject.SetActive(true);
                StartLineText.gameObject.SetActive(true);
            }
            else
            {
                NoteTexture.gameObject.SetActive(true);
                WhiteTexture.gameObject.SetActive(false);
                SpeedText.gameObject.SetActive(false);
                StartLineText.gameObject.SetActive(false);
            }
        }

        public void AdjustPos()
        {
            float curBodySize = ParentBlock.Body.sizeDelta.y;
            float rawY = curBodySize / 192 * YPos;
            float realY = rawY;

            Body.localPosition = new Vector3((EndPoint - ((ParentBlock.MaxLine + 1) / 2f)) * ParentBlock.LineGap, realY, Body.localPosition.z);
        }
    }

}