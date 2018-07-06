using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TempestWave.Core;

namespace TempestWave.Creator
{
    public class Block : MonoBehaviour
    {
        public int Index { get; set; }
        public int BPM { get; set; }
        public int Beats { get; set; }
        public bool NoteDeleted { get; set; }
        public float BlockTime { get; set; }
        public Block NextBlock { get; set; }

        public int MaxLine;
        public float LineGap;
        public GameObject
            LineTemplate,
            NormalNoteTemplate,
            LongNoteTemplate,
            SlideNoteTemplate,
            DamageNoteTemplate,
            HiddenNoteTemplate,
            FlickLeftTemplate,
            FlickRightTemplate,
            FlickUpTemplate,
            FlickDownTemplate,
            TailTemplate;
        public RectTransform 
            Body,
            RowBody,
            NoteBody;
        public Toggle Checkbox;
        public Workspace Table;
        public Text
            IndexText,
            BPMText,
            BeatsText;

        private int myFinger;
        private float inputCount;
        private bool justAdded = false;
        private CreatorNote QueuedNote = null;
        private List<GameObject> DivLines = new List<GameObject>();
        private List<CreatorNote> Notes = new List<CreatorNote>();
        private Dictionary<int, float> TouchTime = new Dictionary<int, float>();

        public Block(int idx, int bpm)
        {
            Index = idx;
            BPM = bpm;

            DivLines = new List<GameObject>();
            Notes = new List<CreatorNote>();
            TouchTime = new Dictionary<int, float>();
        }

        public void Initialize()
        {
            DivLines = new List<GameObject>();
            Notes = new List<CreatorNote>();
            TouchTime = new Dictionary<int, float>();

            for (int i = 1; i < 192; i++)
            {
                GameObject newRow = Instantiate(LineTemplate);

                float curSize = Body.sizeDelta.y;
                float rawY = curSize / 192 * i;
                float realY = rawY - (curSize / 2);
                newRow.transform.SetParent(RowBody);
                newRow.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, realY);
                newRow.GetComponent<RectTransform>().offsetMin = new Vector2(0, newRow.GetComponent<RectTransform>().offsetMin.y);
                newRow.GetComponent<RectTransform>().offsetMax = new Vector2(0, newRow.GetComponent<RectTransform>().offsetMax.y);
                newRow.transform.localScale = new Vector3(1, 1, 1);
                newRow.name = "row" + i.ToString();
                DivLines.Add(newRow);
            }
        }

        public CreatorNote[] GetNoteArray() { return Notes.ToArray(); }

        private void Start()
        {
            IndexText.text = Index.ToString();
            BPMText.text = BPM.ToString();
            BeatsText.text = Beats.ToString();
            BlockTime = (60f * Beats) / BPM;
            UpdateDivLine();
        }

        private void Update()
        {
            if(Input.touchSupported && Input.touchCount > 0)
            {
                for(int i = 0; i < Input.touchCount; i++)
                {
                    if(Input.GetTouch(i).phase.Equals(TouchPhase.Began))
                    {
                        TouchTime.Add(Input.GetTouch(i).fingerId, 0);
                        if (ExtendedInputModule.GetPointerEventData(Input.GetTouch(i).fingerId).pointerEnter != null && ExtendedInputModule.GetPointerEventData(Input.GetTouch(i).fingerId).pointerEnter.tag.Equals("CreatorNote") && ExtendedInputModule.GetPointerEventData(Input.GetTouch(i).fingerId).pointerEnter.GetComponent<CreatorNote>().ParentBlock.Equals(this))
                        {
                            CreatorNote note = ExtendedInputModule.GetPointerEventData(Input.GetTouch(i).fingerId).pointerEnter.GetComponent<CreatorNote>();
                            if (Table.NotePlaceMode.Equals(3) && !note.Flick.Equals(0) && (Table.LastFlickNote.ParentBlock.Index < Index || (Table.LastFlickNote.ParentBlock.Index.Equals(Index) && Table.LastFlickNote.YPos < note.YPos)))
                            {
                                CreateTail(Table.LastFlickNote.gameObject, note.gameObject, 30);
                                Table.LastFlickNote.NextNote = note;
                                note.PreviousNotes.Add(Table.LastFlickNote);
                                Table.PlaceModeChanged(0);
                            }
                            else { SelectNote(ExtendedInputModule.GetPointerEventData(Input.GetTouch(i).fingerId).pointerEnter.GetComponent<CreatorNote>()); }
                            TouchTime[Input.GetTouch(i).fingerId] += Time.deltaTime;
                        }
                        else if (ExtendedInputModule.GetPointerEventData(Input.GetTouch(i).fingerId).pointerEnter.Equals(gameObject))
                        {
                            if (Table.SelectedNote.Count.Equals(0)) { CreateNote(Input.GetTouch(i).position); }
                            else
                            {
                                while (Table.SelectedNote.Count > 0)
                                {
                                    CreatorNote target = Table.SelectedNote[0];
                                    target.ChangeSelected();
                                    Table.SelectedNote.Remove(target);
                                }
                            }
                        }
                    }
                    else if(Input.GetTouch(i).phase.Equals(TouchPhase.Stationary))
                    {
                        if (ExtendedInputModule.GetPointerEventData(Input.GetTouch(i).fingerId).pointerEnter != null && ExtendedInputModule.GetPointerEventData(Input.GetTouch(i).fingerId).pointerEnter.tag.Equals("CreatorNote") && ExtendedInputModule.GetPointerEventData(Input.GetTouch(i).fingerId).pointerEnter.GetComponent<CreatorNote>().ParentBlock.Equals(this))
                        {
                            if (TouchTime[Input.GetTouch(i).fingerId] >= 0.4f)
                            {
                                DeleteNote(ExtendedInputModule.GetPointerEventData(Input.GetTouch(i).fingerId).pointerEnter);
                            }
                            else { TouchTime[Input.GetTouch(i).fingerId] += Time.deltaTime; }
                        }
                    }
                    else if (Input.GetTouch(i).phase.Equals(TouchPhase.Ended))
                    {
                        TouchTime.Remove(Input.GetTouch(i).fingerId);
                    }
                }
            }
            else if(Input.GetMouseButtonDown(0))
            {
                if (ExtendedInputModule.GetPointerEventData().pointerEnter != null && ExtendedInputModule.GetPointerEventData().pointerEnter.tag.Equals("CreatorNote") && ExtendedInputModule.GetPointerEventData().pointerEnter.GetComponent<CreatorNote>().ParentBlock.Equals(this))
                {
                    if (inputCount.Equals(0)) { DeleteNote(ExtendedInputModule.GetPointerEventData().pointerEnter); }
                }
                else if(ExtendedInputModule.GetPointerEventData().pointerEnter.Equals(gameObject))
                {
                    if (inputCount.Equals(0)) { CreateNote(Input.mousePosition); justAdded = true; }
                }
                inputCount += Time.deltaTime;
            }
            else if(Input.GetMouseButtonDown(1))
            {
                if (ExtendedInputModule.GetPointerEventData().pointerEnter != null && ExtendedInputModule.GetPointerEventData().pointerEnter.tag.Equals("CreatorNote") && ExtendedInputModule.GetPointerEventData().pointerEnter.GetComponent<CreatorNote>().ParentBlock.Equals(this))
                {
                    CreatorNote note = ExtendedInputModule.GetPointerEventData().pointerEnter.GetComponent<CreatorNote>();
                    if (Table.NotePlaceMode.Equals(3) && !note.Flick.Equals(0) && (Table.LastFlickNote.ParentBlock.Index < Index || (Table.LastFlickNote.ParentBlock.Index.Equals(Index) && Table.LastFlickNote.YPos < note.YPos)))
                    {
                        CreateTail(Table.LastFlickNote.gameObject, note.gameObject, 30);
                        Table.LastFlickNote.NextNote = note;
                        note.PreviousNotes.Add(Table.LastFlickNote);
                        Table.PlaceModeChanged(0);
                    }
                    else { SelectNote(ExtendedInputModule.GetPointerEventData().pointerEnter.GetComponent<CreatorNote>()); }
                }
                else if(ExtendedInputModule.GetPointerEventData().pointerEnter.Equals(gameObject))
                {
                    while(Table.SelectedNote.Count > 0)
                    {
                        CreatorNote target = Table.SelectedNote[0];
                        target.ChangeSelected();
                        Table.SelectedNote.Remove(target);
                    }
                }
            }
            else if(Input.GetMouseButtonUp(0))
            {
                if (justAdded) { justAdded = false; }
                inputCount = 0;
            }
        }

        private GameObject FindRootOfNote(GameObject child)
        {
            if (child.GetComponent<RectTransform>().parent.gameObject.tag.Equals("CreatorNote")) { return FindRootOfNote(child.GetComponent<RectTransform>().parent.gameObject); }
            else { return child; }
        }

        public void CreateNote(Vector2 pos)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Body, pos, Camera.main, out localPoint);
            Debug.Log(localPoint);

            int xpos = Mathf.RoundToInt((localPoint.x / LineGap) + ((MaxLine + 1) / 2f));
            if (xpos < 1 || xpos > MaxLine) { return; }
            int ypos = CalculateNoteYPos(localPoint.y);
            if (ypos < 192)
            {
                CreatorNote sameNote = null;
                bool sameExists = false;
                foreach(CreatorNote note in Notes)
                {
                    if (xpos.Equals((int)note.EndPoint) && ypos.Equals(note.YPos)) { sameExists = true; sameNote = note; }
                }
                if (sameExists) { DeleteNote(sameNote.gameObject); return; }

                GameObject newObj = null;
                if (Table.FlickType.Equals(0))
                {
                    if (Table.NoteType.Equals(0)) { newObj = Instantiate(NormalNoteTemplate); }
                    else if (Table.NoteType.Equals(1)) { newObj = Instantiate(LongNoteTemplate); }
                    else if (Table.NoteType.Equals(2)) { newObj = Instantiate(SlideNoteTemplate); }
                    else if (Table.NoteType.Equals(3)) { newObj = Instantiate(DamageNoteTemplate); }
                    else if (Table.NoteType.Equals(4)) { newObj = Instantiate(HiddenNoteTemplate); }
                }
                else if (Table.FlickType.Equals(1)) { newObj = Instantiate(FlickLeftTemplate); }
                else if (Table.FlickType.Equals(2)) { newObj = Instantiate(FlickRightTemplate); }
                else if (Table.FlickType.Equals(3)) { newObj = Instantiate(FlickUpTemplate); }
                else if (Table.FlickType.Equals(4)) { newObj = Instantiate(FlickDownTemplate); }

                CreatorNote newNote = newObj.GetComponent<CreatorNote>();
                newNote.StartPoint = Table.StartPoint;
                newNote.EndPoint = xpos;
                if (Table.StartPoint.Equals(0)) { newNote.StartPoint = newNote.EndPoint; }
                newNote.YPos = ypos;
                newNote.Size = Table.SizeVal;
                newNote.Type = Table.NoteType;
                newNote.Flick = Table.FlickType;
                newNote.Speed = Table.NoteSpeed;
                newNote.NoteColor = new Color32(Table.NoteColorR, Table.NoteColorG, Table.NoteColorB, 255);
                newNote.ApplyColor();
                newNote.UpdateInfoToUI();
                newNote.SwitchViewMode();
                Notes.Add(newNote);

                newObj.transform.SetParent(NoteBody);
                newObj.transform.localPosition = new Vector3((newNote.EndPoint - ((MaxLine + 1) / 2f)) * LineGap, Body.sizeDelta.y * (newNote.YPos / 192f), newNote.YPos / 192f);
                newObj.transform.localScale = new Vector3(1, 1, 1);

                newObj.SetActive(true);
                Table.UpdateNoteCount(1);

                if(newNote.Flick != 0)
                {
                    if(Table.NotePlaceMode.Equals(3))
                    {
                        newNote.PreviousNotes.Add(Table.LastFlickNote);
                        Table.LastFlickNote.NextNote = newNote;
                        CreateTail(Table.LastFlickNote.gameObject, newObj, 30);
                        Table.LastFlickNote = newNote;
                    }
                }

                if (newNote.Type.Equals(1))
                {
                    if(Table.NotePlaceMode.Equals(1))
                    {
                        newNote.PreviousNotes.Add(Table.LastLongNote);
                        Table.LastLongNote.NextNote = newNote;
                        CreateTail(Table.LastLongNote.gameObject, newObj, 50);
                        if (newNote.Flick.Equals(0))
                        {
                            Table.NotePlaceMode = 0;
                            Table.PlaceModeChanged(0);
                        }
                    }
                    else
                    {
                        Table.NotePlaceMode = 1;
                        Table.LastLongNote = newNote;
                        Table.PlaceModeChanged(1);
                    }
                }
                else if(newNote.Type.Equals(2))
                {
                    if (Table.NotePlaceMode.Equals(2))
                    {
                        newNote.PreviousNotes.Add(Table.LastSlideNote);
                        Table.LastSlideNote.NextNote = newNote;
                        CreateTail(Table.LastSlideNote.gameObject, newObj, 50);
                        Table.LastSlideNote = newNote;
                    }
                    else
                    {
                        Table.NotePlaceMode = 2;
                        Table.LastSlideNote = newNote;
                        Table.PlaceModeChanged(2);
                    }
                }

                if (newNote.Flick != 0)
                {
                    if (Table.NotePlaceMode != 3)
                    {
                        Table.NotePlaceMode = 3;
                        Table.LastFlickNote = newNote;
                        Table.PlaceModeChanged(3);
                    }
                }

                SortNotes();
            }
            else
            {
                if (NextBlock != null) { NextBlock.CreateNote(pos); }
            }
        }

        public void CreateNote(TWxNote note)
        {
            GameObject newObj = null;
            if (note.Flick.Equals(0))
            {
                if (note.Mode.Equals(0)) { newObj = Instantiate(NormalNoteTemplate); }
                else if (note.Mode.Equals(1)) { newObj = Instantiate(LongNoteTemplate); }
                else if (note.Mode.Equals(2)) { newObj = Instantiate(SlideNoteTemplate); }
                else if (note.Mode.Equals(3)) { newObj = Instantiate(DamageNoteTemplate); }
                else if (note.Mode.Equals(4)) { newObj = Instantiate(HiddenNoteTemplate); }
            }
            else if (note.Flick.Equals(1)) { newObj = Instantiate(FlickLeftTemplate); }
            else if (note.Flick.Equals(2)) { newObj = Instantiate(FlickRightTemplate); }
            else if (note.Flick.Equals(3)) { newObj = Instantiate(FlickUpTemplate); }
            else if (note.Flick.Equals(4)) { newObj = Instantiate(FlickDownTemplate); }

            CreatorNote newNote = newObj.GetComponent<CreatorNote>();
            newNote.StartPoint = (float)note.StartLine;
            newNote.EndPoint = (float)note.EndLine;
            newNote.YPos = note.YPos % 192;
            newNote.Size = note.Size;
            newNote.Type = note.Mode;
            newNote.Flick = note.Flick;
            newNote.Speed = (float)note.Speed;
            newNote.NoteColor = new Color32(note.Color[0], note.Color[1], note.Color[2], 255);
            newNote.ApplyColor();
            newNote.UpdateInfoToUI();
            newNote.SwitchViewMode();
            Notes.Add(newNote);
            Table.SelectedNote.Add(newNote);

            newObj.transform.SetParent(NoteBody);
            newObj.transform.localPosition = new Vector3((newNote.EndPoint - ((MaxLine + 1) / 2f)) * LineGap, Body.sizeDelta.y * (newNote.YPos / 192f), newNote.YPos / 192f);
            newObj.transform.localScale = new Vector3(1, 1, 1);

            newObj.SetActive(true);
            Table.UpdateNoteCount(1);

            if(note.PrevIDs.Length > 0)
            {
                for(int i = 0; i < note.PrevIDs.Length; i++)
                {
                    if(note.PrevIDs[i] > 0)
                    {
                        if(Table.SelectedNote[note.PrevIDs[i] - 1].Type.Equals(1) && newNote.Type.Equals(1))
                        {
                            newNote.PreviousNotes.Add(Table.SelectedNote[note.PrevIDs[i] - 1]);
                            Table.SelectedNote[note.PrevIDs[i] - 1].NextNote = newNote;
                            CreateTail(Table.SelectedNote[note.PrevIDs[i] - 1].gameObject, newObj, 50);
                        }
                        else if(Table.SelectedNote[note.PrevIDs[i] - 1].Type.Equals(2) && newNote.Type.Equals(2))
                        {
                            newNote.PreviousNotes.Add(Table.SelectedNote[note.PrevIDs[i] - 1]);
                            Table.SelectedNote[note.PrevIDs[i] - 1].NextNote = newNote;
                            CreateTail(Table.SelectedNote[note.PrevIDs[i] - 1].gameObject, newObj, 50);
                        }
                        else if(!Table.SelectedNote[note.PrevIDs[i] - 1].Flick.Equals(0) && !newNote.Flick.Equals(0))
                        {
                            newNote.PreviousNotes.Add(Table.SelectedNote[note.PrevIDs[i] - 1]);
                            Table.SelectedNote[note.PrevIDs[i] - 1].NextNote = newNote;
                            CreateTail(Table.SelectedNote[note.PrevIDs[i] - 1].gameObject, newObj, 30);
                        }
                    }
                }
            }

            Table.NoteColorR = newNote.NoteColor.r;
            Table.NoteColorG = newNote.NoteColor.g;
            Table.NoteColorB = newNote.NoteColor.b;
            Table.RText.text = Table.NoteColorR.ToString();
            Table.GText.text = Table.NoteColorG.ToString();
            Table.BText.text = Table.NoteColorB.ToString();
        }
        
        private int CalculateNoteYPos(float localPos)
        {
            float curSize = Body.sizeDelta.y;
            float scaledPos = localPos;

            float unitVal = curSize / Table.DivLine;
            int resultInUnit = Mathf.RoundToInt(scaledPos / unitVal);
            return resultInUnit * (192 / Table.DivLine);
        }

        private void CreateTail(GameObject startObj, GameObject endObj, float size)
        {
            GameObject newObj = Instantiate(TailTemplate);
            LineRenderer newTail = newObj.GetComponent<LineRenderer>();
            newTail.SetPosition(0, startObj.transform.position);
            newTail.SetPosition(1, endObj.transform.position);
            newTail.startWidth = size;
            newTail.endWidth = size;
            if(startObj.GetComponent<CreatorNote>().Size.Equals(0) || endObj.GetComponent<CreatorNote>().Size.Equals(0))
            {
                newTail.startWidth = size * 0.8f;
                newTail.endWidth = size * 0.8f;
            }
            startObj.GetComponent<CreatorNote>().NextTail = newObj;
            endObj.GetComponent<CreatorNote>().PreviousTails.Add(newObj);

            newObj.GetComponent<TailPosUpdator>().HeadNote = startObj;
            newObj.GetComponent<TailPosUpdator>().TailNote = endObj;
            newObj.SetActive(true);
        }

        private void SortNotes()
        {
            Notes.Sort();

            for (int i = Notes.Count - 1; i >= 0; i--) { Notes[i].gameObject.transform.SetAsLastSibling(); }
        }

        private void SelectNote(CreatorNote target)
        {
            if (target.Selected) { Table.SelectedNote.Remove(target); }
            else { Table.SelectedNote.Add(target); }
            target.ChangeSelected();
        }

        public void DeleteNote(GameObject target)
        {
            target.GetComponent<CreatorNote>().Clean();
            Notes.Remove(target.GetComponent<CreatorNote>());
            if (target.GetComponent<CreatorNote>().Selected) { Table.SelectedNote.Remove(target.GetComponent<CreatorNote>()); }
            Destroy(target);
            Table.UpdateNoteCount(-1);
            if (Table.NotePlaceMode.Equals(1)) { Table.PlaceModeChanged(0); }
        }

        public void CheckBlock()
        {
            Table.SelectBlock(Index);
        }

        public void UpdateBPM()
        {
            BPM = Table.BPM;
            Beats = Table.BeatVal;
            BPMText.text = BPM.ToString();
            BeatsText.text = Beats.ToString();
            BlockTime = (60f * Beats) / BPM;
        }

        public void UpdateSize()
        {
            Body.sizeDelta = new Vector3(Body.sizeDelta.x, 670 * Table.ScopeVal);

            for(int i = 0; i < DivLines.Count; i++)
            {
                float curSize = Body.sizeDelta.y;
                float rawY = curSize / 192 * (i + 1);
                float realY = rawY - (curSize / 2);

                DivLines[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, realY);
                DivLines[i].GetComponent<RectTransform>().offsetMin = new Vector2(0, DivLines[i].GetComponent<RectTransform>().offsetMin.y);
                DivLines[i].GetComponent<RectTransform>().offsetMax = new Vector2(0, DivLines[i].GetComponent<RectTransform>().offsetMax.y);
            }

            foreach (CreatorNote note in Notes) { note.AdjustPos(); }
        }

        public void UpdateDivLine()
        {
            //while(DivLines.Count > 0)
            //{
            //    GameObject target = DivLines[0];
            //    DivLines.Remove(target);
            //    Destroy(target);
            //}

            for (int i = 1; i < 192; i++)
            {
                //GameObject newRow = Instantiate(LineTemplate);

                //float curSize = Body.sizeDelta.y;
                //float rawY = curSize / Table.DivLine * i;
                //float realY = rawY - (curSize / 2);
                //newRow.transform.SetParent(RowBody);
                ////newRow.transform.localPosition = new Vector3(newRow.transform.localPosition.x, realY, newRow.transform.localPosition.z);
                //newRow.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, realY);
                //newRow.GetComponent<RectTransform>().offsetMin = new Vector2(0, newRow.GetComponent<RectTransform>().offsetMin.y);
                //newRow.GetComponent<RectTransform>().offsetMax = new Vector2(0, newRow.GetComponent<RectTransform>().offsetMax.y);
                //newRow.transform.localScale = new Vector3(1, 1, 1);
                //newRow.name = "row" + i.ToString();
                //DivLines.Add(newRow);
                //newRow.SetActive(true);

                if ((i - 1) % (192 / Table.DivLine) == 0) { DivLines[i - 1].SetActive(true); }
                else { DivLines[i - 1].SetActive(false); }
            }
        }

        public void UpdateNoteSpeed()
        {
            foreach (CreatorNote note in Notes)
            {
                note.Speed = Table.NoteSpeed;
                note.UpdateInfoToUI();
            }
        }

        public void UpdateNoteColor()
        {
            foreach (CreatorNote note in Notes)
            {
                note.NoteColor = new Color32(Table.NoteColorR, Table.NoteColorG, Table.NoteColorB, 255);
                note.ApplyColor();
            }
        }

        public void SpecificView()
        {
            foreach (CreatorNote note in Notes) { note.SwitchViewMode(); }
        }
    }
}
