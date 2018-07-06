using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TempestWave.Core;
using TempestWave.Core.UI;
using TempestWave.Data;
using LitJson;

namespace TempestWave.Creator
{
    public enum CreatorMode
    {
        Starlight, Theater2, Theater4, Theater6
    }

    public class Workspace : MonoBehaviour
    {
        public delegate void BlockUpdate();
        public bool IsActive { get; set; }

        public int MaxLine { get; set; }
        public int BPM { get; set; }
        public int BeatVal { get; set; }
        public int SizeVal { get; set; }
        public float ScopeVal { get; set; }
        public int DivLine { get; set; }
        public int NotePlaceMode { get; set; }
        public bool ViewSpecific { get; set; }
        public bool MetadataMode { get; set; }
        public int LevelVal { get; set; }
        public int DensityVal { get; set; }
        public int NoteCount { get; set; }

        public string SongName { get; set; }
        public string Artist { get; set; }
        public string Mapper { get; set; }

        public int NoteType { get; set; } // 0: TAP, 1: HOLD, 2: SLIDE, 3: DAMAGE, 4: HIDDEN
        public int FlickType { get; set; } // 0: None, 1: L, 2: R, 3: U, 4: D
        public float StartPoint { get; set; }
        public float NoteSpeed { get; set; }
        public byte NoteColorR { get; set; }
        public byte NoteColorG { get; set; }
        public byte NoteColorB { get; set; }

        public CreatorNote LastLongNote { get; set; }
        public CreatorNote LastSlideNote { get; set; }
        public CreatorNote LastFlickNote { get; set; }
        public List<CreatorNote> SelectedNote { get; set; }

        public Button[]
            BeatButton = new Button[2],
            SizeButtons = new Button[3],
            NoteButtons = new Button[5],
            FlickButtons = new Button[5],
            LevelButtons = new Button[4],
            LineButtons = new Button[10];
        public Sprite
            SpecialNote,
            SpecialWhiteNote;
        public GameObject
            RightControlPanel,
            RightMetadataPanel,
            L1Block,
            L2Block,
            L4Block,
            L5Block,
            L6Block;
        public RectTransform BlockParent;
        public Text
            BPMText,
            StartPointText,
            SpeedText,
            ScopeText,
            DivLineText,
            RText,
            GText,
            BText,
            PlaceModeTitle,
            PlaceModeDesc,
            DensityText,
            NoteCountText;
        public InputField
            NameField,
            ArtistField,
            MapperField,
            OffsetField;
        public Button
            OpenMetadataBtn,
            ViewSpecificBtn,
            PlaceModeBtn;
        public Image ColorPanel, PlaceModePanel;
        public MusicPlayer Player;

        private int DivArrayIndex = 5, Offset;
        private int[] DivLineArray = new int[] { 2, 3, 4, 6, 8, 12, 16, 24, 32, 48, 64 };
        private List<Block> Blocks = new List<Block>();
        private BlockUpdate 
            UpdateBlockBPM,
            UpdateBlockDivLine,
            UpdateBlockNoteSpeed, 
            UpdateBlockSize,
            UpdateBlockNoteColor,
            UpdateSpecificView;

        private void Awake()
        {
            SelectedNote = new List<CreatorNote>();
            ScopeVal = 1.0f;
        }

        private void Update()
        {
            if(IsActive)
            {
                bool ShiftHold = false;
                if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) { ShiftHold = true; }

                if (Input.GetKeyDown(KeyCode.BackQuote)) { ChangeStartPosImmediate(0, ShiftHold); }
                else if (Input.GetKeyDown(KeyCode.Alpha1)) { ChangeStartPosImmediate(1, ShiftHold); }
                else if (Input.GetKeyDown(KeyCode.Alpha2)) { ChangeStartPosImmediate(2, ShiftHold); }
                else if (Input.GetKeyDown(KeyCode.Alpha3)) { ChangeStartPosImmediate(3, ShiftHold); }
                else if (Input.GetKeyDown(KeyCode.Alpha4)) { ChangeStartPosImmediate(4, ShiftHold); }
                else if (Input.GetKeyDown(KeyCode.Alpha5)) { ChangeStartPosImmediate(5, ShiftHold); }
                else if (Input.GetKeyDown(KeyCode.Alpha6)) { ChangeStartPosImmediate(6, ShiftHold); }
                else if (Input.GetKeyDown(KeyCode.Alpha7)) { ChangeStartPosImmediate(7, ShiftHold); }
                else if (Input.GetKeyDown(KeyCode.Alpha8)) { ChangeStartPosImmediate(8, ShiftHold); }
                else if (Input.GetKeyDown(KeyCode.Alpha9)) { ChangeStartPosImmediate(9, ShiftHold); }
                else if (Input.GetKeyDown(KeyCode.Alpha0)) { ChangeStartPosImmediate(0, true); }

                if (Input.GetKeyDown(KeyCode.Q)) { SelectNoteMode(0); }
                else if (Input.GetKeyDown(KeyCode.W)) { SelectNoteMode(1); }
                else if (Input.GetKeyDown(KeyCode.E)) { SelectNoteMode(2); }
                else if (Input.GetKeyDown(KeyCode.R)) { SelectNoteMode(3); }
                else if (Input.GetKeyDown(KeyCode.T)) { SelectNoteMode(4); }

                if (Input.GetKeyDown(KeyCode.A)) { SelectFlickMode(0); }
                else if (Input.GetKeyDown(KeyCode.S)) { SelectFlickMode(1); }
                else if (Input.GetKeyDown(KeyCode.D)) { SelectFlickMode(2); }
                else if (Input.GetKeyDown(KeyCode.F)) { SelectFlickMode(3); }
                else if (Input.GetKeyDown(KeyCode.G)) { SelectFlickMode(4); }

                if (Input.GetKeyDown(KeyCode.Escape)) { PlaceModeChanged(0); }
            }
        }

        public void CleanBeforeInit()
        {
            ExitClean();
            BPM = 110;
            BPMText.text = BPM.ToString();
            MaxLine = 0;
            BeatVal = 4;
            NoteColorR = 255;
            NoteColorG = 255;
            NoteColorB = 255;
            for (int i = 1; i < 9; i++)
            {
                if (LineButtons[i] != null)
                {
                    LineButtons[i].targetGraphic.color = GlobalTheme.ThemeColor();
                    LineButtons[i].gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
                }
            }
            for(int i = 0; i < 4; i++)
            {
                LevelButtons[i].targetGraphic.color = GlobalTheme.ThemeColor();
                LevelButtons[i].gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
            }
            DensityVal = 20;
            DensityText.text = DensityVal.ToString();
            LevelSelect(2);
            NoteCount = 0;
            NoteCountText.text = "0 notes";

            NameField.text = "";
            ArtistField.text = "";
            MapperField.text = "";
            OffsetField.text = "";
        }

        public void Initialize()
        {
            IsActive = true;
            StartPoint = 0;
            StartPointText.text = LocaleManager.instance.GetLocaleText("creator_auto");
            NoteSpeed = 1.0f;
            SpeedText.text = NoteSpeed.ToString("N1");
            ScopeVal = 1.0f;
            ScopeText.text = (ScopeVal * 100).ToString("N0") + "%";
            DivLine = 8;
            DivArrayIndex = 5;
            DivLineText.text = DivLine.ToString();
            ViewSpecific = false;
            ViewSpecificBtn.targetGraphic.color = GlobalTheme.ThemeColor();
            ViewSpecificBtn.gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
            RText.text = NoteColorR.ToString();
            GText.text = NoteColorG.ToString();
            BText.text = NoteColorB.ToString();
            ColorPanel.color = new Color32(NoteColorR, NoteColorG, NoteColorB, 255);

            LevelSelect(LevelVal);
            ChangeLine(MaxLine);
            SelectNoteMode(0);
            SelectFlickMode(0);
            ChangeBeats(BeatVal);
            ChangeSize(1);
            PlaceModeChanged(0);
            Player.Clean();

            MetadataMode = false;
            ShowMetadata();
        }

        public void AddBlock()
        {
            GameObject newObj = null;
            if (MaxLine.Equals(5)) { newObj = Instantiate(L5Block); }
            else if (MaxLine.Equals(2)) { newObj = Instantiate(L2Block); }
            else if (MaxLine.Equals(4)) { newObj = Instantiate(L4Block); }
            else if (MaxLine.Equals(6)) { newObj = Instantiate(L6Block); }
            else if (MaxLine.Equals(1)) { newObj = Instantiate(L1Block); }
            else
            {
                MessageBox.Show(LocaleManager.instance.GetLocaleText("creator_notenoughinfo"), LocaleManager.instance.GetLocaleText("creator_error_nolineval"), MessageBoxButton.OK);
                return;
            }
            newObj.GetComponent<Block>().Initialize();
            newObj.GetComponent<Block>().Index = Blocks.Count;
            newObj.GetComponent<Block>().BPM = BPM;
            newObj.GetComponent<Block>().Beats = BeatVal;
            if (Blocks.Count > 0) { Blocks[Blocks.Count - 1].NextBlock = newObj.GetComponent<Block>(); }
            Blocks.Add(newObj.GetComponent<Block>());
            newObj.transform.SetParent(BlockParent);
            newObj.GetComponent<RectTransform>().sizeDelta = new Vector2(newObj.GetComponent<RectTransform>().sizeDelta.x, 670 * ScopeVal);
            newObj.transform.localScale = new Vector3(1, 1, 1);
            newObj.transform.SetAsFirstSibling();
            newObj.name = "block" + newObj.GetComponent<Block>().Index.ToString();
            newObj.SetActive(true);

            UpdateBlockDivLine += newObj.GetComponent<Block>().UpdateDivLine;
            UpdateBlockSize += newObj.GetComponent<Block>().UpdateSize;
            UpdateSpecificView += newObj.GetComponent<Block>().SpecificView;
        }

        public Block GetBlock(int idx) { return Blocks[idx]; }
        public int BlockCount() { return Blocks.Count; }

        public void PlaceModeChanged(int value)
        {
            NotePlaceMode = value;

            if(NotePlaceMode.Equals(0)) // FREE NOTE MODE
            {
                PlaceModeTitle.text = LocaleManager.instance.GetLocaleText("creator_placemode0_title");
                PlaceModeDesc.text = LocaleManager.instance.GetLocaleText("creator_placemode0_desc");

                for (int i = 0; i < 5; i++)
                {
                    NoteButtons[i].interactable = true;
                    FlickButtons[i].interactable = true;
                }
                SelectNoteMode(NoteType);
                SizeButtons[2].interactable = true;
            }
            else if(NotePlaceMode.Equals(1)) // LONG NOTE MODE
            {
                PlaceModeTitle.text = LocaleManager.instance.GetLocaleText("creator_placemode1_title");
                PlaceModeDesc.text = LocaleManager.instance.GetLocaleText("creator_placemode1_desc");

                for(int i = 0; i < 5; i++)
                {
                    if (i.Equals(1)) { NoteButtons[i].interactable = true; }
                    else { NoteButtons[i].interactable = false; }
                    FlickButtons[i].interactable = true;
                }
                SizeButtons[2].interactable = false;
            }
            else if (NotePlaceMode.Equals(2)) // SLIDE NOTE MODE
            {
                PlaceModeTitle.text = LocaleManager.instance.GetLocaleText("creator_placemode2_title");
                PlaceModeDesc.text = LocaleManager.instance.GetLocaleText("creator_placemode2_desc");

                for (int i = 0; i < 5; i++)
                {
                    if (i.Equals(2)) { NoteButtons[i].interactable = true; }
                    else { NoteButtons[i].interactable = false; }
                    FlickButtons[i].interactable = true;
                }
                SizeButtons[2].interactable = false;
            }
            else if (NotePlaceMode.Equals(3)) // FLICK NOTE MODE
            {
                PlaceModeTitle.text = LocaleManager.instance.GetLocaleText("creator_placemode3_title");
                PlaceModeDesc.text = LocaleManager.instance.GetLocaleText("creator_placemode3_desc");
                

                NoteButtons[0].interactable = true;
                SelectNoteMode(0);
                for (int i = 0; i < 5; i++)
                {
                    if (i.Equals(0))
                    {
                        NoteButtons[i].interactable = true;
                        FlickButtons[i].interactable = false;
                    }
                    else
                    {
                        NoteButtons[i].interactable = false;
                        FlickButtons[i].interactable = true;
                    }
                }
                SizeButtons[2].interactable = false;
            }

            if (MetadataMode) { ShowMetadata(); }
            ChangePlaceModeColor();
        }

        private void ChangePlaceModeColor()
        {
            if(NotePlaceMode.Equals(0))
            {
                PlaceModePanel.color = GlobalTheme.ThemeColor();
                PlaceModeTitle.color = GlobalTheme.ThemeContrastColor();
                PlaceModeDesc.color = GlobalTheme.ThemeContrastColor();
                PlaceModeBtn.targetGraphic.color = GlobalTheme.ThemeColor();
                PlaceModeBtn.GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
            }
            else if(NotePlaceMode.Equals(1))
            {
                PlaceModePanel.color = new Color32(0, 0, 255, 255);
                PlaceModeTitle.color = new Color32(255, 255, 255, 255);
                PlaceModeDesc.color = new Color32(255, 255, 255, 255);
                PlaceModeBtn.targetGraphic.color = new Color32(0, 0, 255, 255);
                PlaceModeBtn.GetComponentInChildren<Text>().color = new Color32(255, 255, 255, 255);
            }
            else if(NotePlaceMode.Equals(2))
            {
                PlaceModePanel.color = new Color32(178, 0, 255, 255);
                PlaceModeTitle.color = new Color32(255, 255, 255, 255);
                PlaceModeDesc.color = new Color32(255, 255, 255, 255);
                PlaceModeBtn.targetGraphic.color = new Color32(178, 0, 255, 255);
                PlaceModeBtn.GetComponentInChildren<Text>().color = new Color32(255, 255, 255, 255);
            }
            else if (NotePlaceMode.Equals(3))
            {
                PlaceModePanel.color = new Color32(255, 128, 0, 255);
                PlaceModeTitle.color = new Color32(0, 0, 0, 255);
                PlaceModeDesc.color = new Color32(0, 0, 0, 255);
                PlaceModeBtn.targetGraphic.color = new Color32(255, 128, 0, 255);
                PlaceModeBtn.GetComponentInChildren<Text>().color = new Color32(0, 0, 0, 255);
            }
        }

        public void CancelUntil()
        {
            PlaceModeChanged(0);
        }

        public void SelectNoteMode(int mode)
        {
            if(NoteButtons[mode].interactable)
            {
                NoteType = mode;
                for (int i = 0; i < 5; i++)
                {
                    if (i.Equals(mode)) { NoteButtons[i].gameObject.GetComponent<Image>().color = GlobalTheme.ThemeContrastColor(); }
                    else { NoteButtons[i].gameObject.GetComponent<Image>().color = GlobalTheme.ThemeColor(); }
                    if (NoteType.Equals(0)) { FlickButtons[i].interactable = true; }
                    else { FlickButtons[i].interactable = i.Equals(0) ? true : false; }
                }
                if (!NoteType.Equals(0)) { SelectFlickMode(0); }
            }
        }

        public void SelectFlickMode(int mode)
        {
            if(FlickButtons[mode].interactable)
            {
                FlickType = mode;
                for (int i = 0; i < 5; i++)
                {
                    if (i.Equals(mode)) { FlickButtons[i].gameObject.GetComponent<Image>().color = GlobalTheme.ThemeContrastColor(); }
                    else { FlickButtons[i].gameObject.GetComponent<Image>().color = GlobalTheme.ThemeColor(); }
                }
            }
        }

        public void ChangeBPM(int deltaVal)
        {
            BPM += deltaVal;
            if (BPM <= 0) { BPM = 1; }
            if (BPM >= 1000) { BPM = 999; }
            BPMText.text = BPM.ToString();
        }

        public void ChangeBPMWithValue(int newBPM)
        {
            BPM = newBPM;
            BPMText.text = BPM.ToString();
        }

        public void ChangeBeats(int value)
        {
            if(value.Equals(3))
            {
                BeatVal = 3;
                BeatButton[0].targetGraphic.color = GlobalTheme.ThemeContrastColor();
                BeatButton[0].gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeColor();
                BeatButton[1].targetGraphic.color = GlobalTheme.ThemeColor();
                BeatButton[1].gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
            }
            else if(value.Equals(4))
            {
                BeatVal = 4;
                BeatButton[1].targetGraphic.color = GlobalTheme.ThemeContrastColor();
                BeatButton[1].gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeColor();
                BeatButton[0].targetGraphic.color = GlobalTheme.ThemeColor();
                BeatButton[0].gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
            }
        }

        public void ChangeStartPos(int moveVal)
        {
            if (moveVal.Equals(0)) { StartPoint -= 1; }
            else if (moveVal.Equals(1)) { StartPoint += 1; }
            if (StartPoint < 0) { StartPoint = MaxLine; }
            if (StartPoint > MaxLine) { StartPoint = 0; }
            StartPointText.text = StartPoint.ToString("N0");
            if (StartPoint.Equals(0)) { StartPointText.text = LocaleManager.instance.GetLocaleText("creator_auto"); }

            if (SelectedNote.Count > 0)
            {
                foreach (CreatorNote note in SelectedNote)
                {
                    note.StartPoint = StartPoint;
                    note.UpdateInfoToUI();
                }
            }
        }

        public void ChangeStartPosImmediate(int startpos)
        {
            StartPoint = startpos;
            StartPointText.text = StartPoint.ToString();
            if (StartPoint.Equals(0)) { StartPointText.text = LocaleManager.instance.GetLocaleText("creator_auto"); }
            if (SelectedNote.Count > 0)
            {
                foreach (CreatorNote note in SelectedNote)
                {
                    note.StartPoint = StartPoint;
                    note.UpdateInfoToUI();
                }
            }
        }

        public void ChangeStartPosImmediate(int startpos, bool shift)
        {
            if (!shift) { ChangeStartPosImmediate(startpos); }
            else
            {
                if(startpos.Equals(0))
                {
                    StartPoint = startpos;
                    StartPointText.text = StartPoint.ToString();
                    foreach(CreatorNote note in SelectedNote)
                    {
                        note.StartPoint = startpos;
                        note.StartLineText.text = note.StartPoint.ToString("N1");
                    }
                }
                else { ChangeStartPosImmediate(-1 * startpos); }
            }
        }

        public void ChangeSpeed(int moveVal)
        {
            if (moveVal.Equals(0) && NoteSpeed > 0.1f) { NoteSpeed -= 0.1f; }
            else if (moveVal.Equals(1) && NoteSpeed < 9.9f) { NoteSpeed += 0.1f; }
            SpeedText.text = NoteSpeed.ToString("N1");

            if(SelectedNote.Count > 0)
            {
                foreach(CreatorNote note in SelectedNote)
                {
                    note.Speed = NoteSpeed;
                    note.UpdateInfoToUI();
                }
            }
        }

        public void ChangeSize(int value)
        {
            SizeVal = value;
            for(int i = 0; i < 3; i++)
            {
                if(i.Equals(value))
                {
                    SizeButtons[i].targetGraphic.color = GlobalTheme.ThemeContrastColor();
                    SizeButtons[i].gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeColor();
                }
                else
                {
                    SizeButtons[i].targetGraphic.color = GlobalTheme.ThemeColor();
                    SizeButtons[i].gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
                }
            }

            if (SelectedNote.Count > 0 && SizeVal != 2)
            {
                foreach (CreatorNote note in SelectedNote)
                {
                    note.Size = SizeVal;
                    note.UpdateInfoToUI();
                }
            }
        }

        public void ChangeScope(int moveVal)
        {
            if (moveVal.Equals(0)) { ScopeVal -= 0.1f; }
            else if (moveVal.Equals(1)) { ScopeVal += 0.1f; }
            if (ScopeVal < 0.1f) { ScopeVal = 0.1f; }
            if(ScopeVal > 5.0f) { ScopeVal = 5.0f; }
            ScopeText.text = (ScopeVal * 100).ToString("N0") + "%";

            UpdateBlockSize();
        }

        public void ChangeDivLine(int moveVal)
        {
            if (moveVal.Equals(0) && DivArrayIndex > 0) { DivArrayIndex--; }
            else if (moveVal.Equals(1) && DivArrayIndex < 10) { DivArrayIndex++; }
            DivLine = DivLineArray[DivArrayIndex];
            DivLineText.text = DivLine.ToString();

            UpdateBlockDivLine();
        }

        public void ChangeColorR(int deltaVal)
        {
            if(deltaVal < 0)
            {
                if (NoteColorR + deltaVal >= 0) { NoteColorR += (byte)deltaVal; }
                else { NoteColorR = 0; }
            }
            else
            {
                if (NoteColorR + deltaVal <= 255) { NoteColorR += (byte)deltaVal; }
                else { NoteColorR = 255; }
            }
            RText.text = NoteColorR.ToString();
            ColorPanel.color = new Color32(NoteColorR, NoteColorG, NoteColorB, 255);

            if (SelectedNote.Count > 0)
            {
                foreach (CreatorNote note in SelectedNote)
                {
                    note.NoteColor = new Color32(NoteColorR, NoteColorG, NoteColorB, 255);
                    note.ApplyColor();
                }
            }
        }

        public void ChangeColorG(int deltaVal)
        {
            if (deltaVal < 0)
            {
                if (NoteColorG + deltaVal >= 0) { NoteColorG += (byte)deltaVal; }
                else { NoteColorG = 0; }
            }
            else
            {
                if (NoteColorG + deltaVal <= 255) { NoteColorG += (byte)deltaVal; }
                else { NoteColorG = 255; }
            }
            GText.text = NoteColorG.ToString();
            ColorPanel.color = new Color32(NoteColorR, NoteColorG, NoteColorB, 255);

            if (SelectedNote.Count > 0)
            {
                foreach (CreatorNote note in SelectedNote)
                {
                    note.NoteColor = new Color32(NoteColorR, NoteColorG, NoteColorB, 255);
                    note.ApplyColor();
                }
            }
        }

        public void ChangeColorB(int deltaVal)
        {
            if (deltaVal < 0)
            {
                if (NoteColorB + deltaVal >= 0) { NoteColorB += (byte)deltaVal; }
                else { NoteColorB = 0; }
            }
            else
            {
                if (NoteColorB + deltaVal <= 255) { NoteColorB += (byte)deltaVal; }
                else { NoteColorB = 255; }
            }
            BText.text = NoteColorB.ToString();
            ColorPanel.color = new Color32(NoteColorR, NoteColorG, NoteColorB, 255);

            if (SelectedNote.Count > 0)
            {
                foreach (CreatorNote note in SelectedNote)
                {
                    note.NoteColor = new Color32(NoteColorR, NoteColorG, NoteColorB, 255);
                    note.ApplyColor();
                }
            }
        }

        public void SelectBlock(int index)
        {
            Block block = Blocks[index];
            if(block.Checkbox.isOn)
            {
                UpdateBlockBPM += block.UpdateBPM;
                UpdateBlockNoteSpeed += block.UpdateNoteSpeed;
                UpdateBlockNoteColor += block.UpdateNoteColor;
            }
            else
            {
                UpdateBlockBPM -= block.UpdateBPM;
                UpdateBlockNoteSpeed -= block.UpdateNoteSpeed;
                UpdateBlockNoteColor -= block.UpdateNoteColor;
            }
        }

        public void SpecificView()
        {
            ViewSpecific = ViewSpecific ? false : true;
            UpdateSpecificView();

            if (ViewSpecific)
            {
                ViewSpecificBtn.targetGraphic.color = GlobalTheme.ThemeContrastColor();
                ViewSpecificBtn.gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeColor();
            }
            else
            {
                ViewSpecificBtn.targetGraphic.color = GlobalTheme.ThemeColor();
                ViewSpecificBtn.gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
            }
        }

        public void BlockChangeBPM() { UpdateBlockBPM(); }
        public void BlockChangeSpeed() { UpdateBlockNoteSpeed(); }
        public void BlockChangeColor() { UpdateBlockNoteColor(); }

        public void ChangeMicroStart(float deltaVal)
        {
            foreach(CreatorNote note in SelectedNote)
            {
                note.StartPoint += deltaVal;
                if (note.StartPoint < -9.9f) { note.StartPoint = -9.9f; }
                if (note.StartPoint > 19.9f) { note.StartPoint = 19.9f; }
                note.StartLineText.text = note.StartPoint.ToString("N1");
            }
        }

        public void ChangeMicroEnd(float deltaVal)
        {
            foreach(CreatorNote note in SelectedNote)
            {
                note.EndPoint += deltaVal;
                if (note.EndPoint < 0) { note.EndPoint = 0; }
                if (note.EndPoint > MaxLine + 1) { note.EndPoint = MaxLine + 1; }
                note.AdjustPos();
            }
        }

        public void ExitClean()
        {
            while (Blocks.Count > 0)
            {
                Block target = Blocks[0];
                UpdateBlockDivLine -= target.UpdateDivLine;
                UpdateBlockSize -= target.UpdateSize;
                UpdateSpecificView -= target.SpecificView;
                if (target.Checkbox.isOn)
                {
                    UpdateBlockBPM -= target.UpdateBPM;
                    UpdateBlockNoteSpeed -= target.UpdateNoteSpeed;
                    UpdateBlockNoteColor -= target.UpdateNoteColor;
                }
                Blocks.Remove(target);
                Destroy(target.gameObject);
            }
        }

        public void UpdateNoteCount(int delta)
        {
            NoteCount += delta;
            NoteCountText.text = NoteCount.ToString() + " note(s)";
        }

        // ========== FROM HERE THIS IS METADATA CONTROL

        public void ShowMetadata()
        {
            MetadataMode = MetadataMode ? false : true;

            if (MetadataMode)
            {
                RightMetadataPanel.transform.SetAsLastSibling();
                OpenMetadataBtn.targetGraphic.color = GlobalTheme.ThemeContrastColor();
                OpenMetadataBtn.gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeColor();
            }
            else
            {
                RightControlPanel.transform.SetAsLastSibling();
                OpenMetadataBtn.targetGraphic.color = GlobalTheme.ThemeColor();
                OpenMetadataBtn.gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
            }
        }

        public void ChangeDensity(int moveVal)
        {
            if (moveVal.Equals(0))
            {
                DensityVal--;
                if (DensityVal < 1) { DensityVal = 1; }
            }
            else
            {
                DensityVal++;
                if(DensityVal > 99) { DensityVal = 99; }
            }
            DensityText.text = DensityVal.ToString();
        }

        public void LevelSelect(int value)
        {
            LevelVal = value;
            for(int i = 0; i < 4; i++)
            {
                if ((i + 1).Equals(value))
                {
                    LevelButtons[i].targetGraphic.color = GlobalTheme.ThemeContrastColor();
                    LevelButtons[i].gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeColor();
                }
                else
                {
                    LevelButtons[i].targetGraphic.color = GlobalTheme.ThemeColor();
                    LevelButtons[i].gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
                }
            }
        }

        public void LineSelect(int value)
        {
            if (Blocks.Count > 0 && value != MaxLine) { StartCoroutine(MsgBox_ChangeLine(value)); }
            else { ChangeLine(value); }
        }

        private void ChangeLine(int value)
        {
            if(value > 0)
            {
                MaxLine = value;
                for (int i = 1; i < 9; i++)
                {
                    if (i.Equals(value))
                    {
                        LineButtons[i].targetGraphic.color = GlobalTheme.ThemeContrastColor();
                        LineButtons[i].gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeColor();
                    }
                    else if (LineButtons[i] != null)
                    {
                        LineButtons[i].targetGraphic.color = GlobalTheme.ThemeColor();
                        LineButtons[i].gameObject.GetComponentInChildren<Text>().color = GlobalTheme.ThemeContrastColor();
                    }
                }
            }
        }

        IEnumerator MsgBox_ChangeLine(int value)
        {
            MessageBox.Show(LocaleManager.instance.GetLocaleText("creator_willdeleted"), LocaleManager.instance.GetLocaleText("creator_willdeleteddesc"), MessageBoxButton.YesNo);
            yield return new WaitUntil(() => MessageBox.Instance.ResultExists == true);
            if (MessageBox.Instance.Result.Equals(MessageBoxButtonType.Yes))
            {
                while(Blocks.Count > 0)
                {
                    Block target = Blocks[0];
                    UpdateBlockDivLine -= target.UpdateDivLine;
                    UpdateBlockSize -= target.UpdateSize;
                    UpdateSpecificView -= target.SpecificView;
                    if(target.Checkbox.isOn)
                    {
                        UpdateBlockBPM -= target.UpdateBPM;
                        UpdateBlockNoteSpeed -= target.UpdateNoteSpeed;
                        UpdateBlockNoteColor -= target.UpdateNoteColor;
                    }
                    Blocks.Remove(target);
                    Destroy(target.gameObject);
                }
                ChangeLine(value);
            }
        }

        // ========== FROM HERE THIS IS TWx IN/OUTPUT

        public bool ReadFromTWx(string path, string name)
        {
            ScopeVal = 1;

            try
            {
                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(stream);
                string jsonText = reader.ReadToEnd();

                JsonData jsonData = JsonMapper.ToObject(jsonText);
                TWxData data = JsonMapper.ToObject<TWxData>(jsonText);
                NameField.text = name;
                ArtistField.text = data.metadata.artist;
                MapperField.text = data.metadata.mapper;
                LevelSelect(data.metadata.level);
                DensityVal = data.metadata.density;
                DensityText.text = DensityVal.ToString();
                OffsetField.text = data.metadata.offset.ToString();

                LineSelect(int.Parse(path.Substring(path.Length - 1)));

                int curBlockIndex = -1, curBPMQueue = -1, curBeatsQueue = -1;
                for(int i = 0; i < data.notes.Length; i++)
                {
                    while(data.notes[i].YPos / 192 > curBlockIndex)
                    {
                        curBlockIndex++;
                        if ((curBPMQueue + 1 < data.metadata.bpmQueue.Length && data.metadata.bpmQueue[curBPMQueue + 1] <= curBlockIndex))
                        {
                            curBPMQueue++;
                            ChangeBPMWithValue(data.metadata.bpm[curBPMQueue]);
                        }
                        if (data.metadata.beats == null) { BeatVal = 4; }
                        else if((curBeatsQueue + 1 < data.metadata.beatsQueue.Length && data.metadata.beatsQueue[curBeatsQueue + 1] <= curBlockIndex))
                        {
                            curBeatsQueue++;
                            BeatVal = data.metadata.beats[curBeatsQueue];
                        }
                        AddBlock();
                    }
                    Blocks[curBlockIndex].CreateNote(data.notes[i]);
                }
                SelectedNote.Clear();
                reader.Close();
                stream.Close();
                return true;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message + Environment.NewLine + e.StackTrace);
                return false;
            }
        }

        public void WriteToTWx()
        {
            if(MaxLine.Equals(0))
            {
                MessageBox.Show(LocaleManager.instance.GetLocaleText("creator_notenoughinfo"), LocaleManager.instance.GetLocaleText("creator_error_nolineval"), MessageBoxButton.OK);
                return;
            }
            if(LevelVal < 1)
            {
                MessageBox.Show(LocaleManager.instance.GetLocaleText("creator_notenoughinfo"), LocaleManager.instance.GetLocaleText("creator_error_nolevelval"), MessageBoxButton.OK);
                return;
            }
            if(NameField.text.Equals(""))
            {
                MessageBox.Show(LocaleManager.instance.GetLocaleText("creator_notenoughinfo"), LocaleManager.instance.GetLocaleText("creator_error_nosongname"), MessageBoxButton.OK);
                return;
            }
            if(!OffsetField.text.Equals("") && !int.TryParse(OffsetField.text, out Offset))
            {
                MessageBox.Show(LocaleManager.instance.GetLocaleText("creator_notenoughinfo"), LocaleManager.instance.GetLocaleText("creator_error_wrongoffset"), MessageBoxButton.OK);
                return;
            }

            TWxMetadata metadata = new TWxMetadata();
            metadata.level = LevelVal;
            metadata.artist = ArtistField.text;
            metadata.mapper = MapperField.text;
            metadata.density = DensityVal;
            metadata.offset = Offset;

            int noteID = 1;
            double absTime = Offset / 1000f;
            List<TWxNote> NoteBasket = new List<TWxNote>();
            List<int>
                BPMs = new List<int>(),
                BPMQueues = new List<int>(),
                Beats = new List<int>(),
                BeatsQueues = new List<int>();

            for(int i = 0; i < Blocks.Count; i++)
            {
                double blockTime = (60f * Blocks[i].Beats) / Blocks[i].BPM;
                if(i.Equals(0) || (!i.Equals(0) && Blocks[i].BPM != BPMs[BPMs.Count - 1]))
                {
                    BPMs.Add(Blocks[i].BPM);
                    BPMQueues.Add(Blocks[i].Index);
                }
                if(i.Equals(0) || (!i.Equals(0) && Blocks[i].Beats != Beats[Beats.Count - 1]))
                {
                    Beats.Add(Blocks[i].Beats);
                    BeatsQueues.Add(Blocks[i].Index);
                }
                for(int j = 0; j < Blocks[i].GetNoteArray().Length; j++)
                {
                    CreatorNote baseNote = Blocks[i].GetNoteArray()[j];
                    TWxNote note = new TWxNote();

                    baseNote.FinalIndex = noteID;
                    List<int> prevDatas = new List<int>();
                    if(baseNote.PreviousNotes.Count > 0)
                    {
                        for(int k = 0; k < baseNote.PreviousNotes.Count; k++)
                        {
                            prevDatas.Add(baseNote.PreviousNotes[k].FinalIndex);
                        }
                    }
                    int[] prevArr;
                    if (prevDatas.Count.Equals(0)) { prevArr = new int[] { 0 }; }
                    else { prevArr = prevDatas.ToArray(); }
                    note.SetValue(baseNote.YPos + 192 * baseNote.ParentBlock.Index, noteID, baseNote.Size, new byte[4] { baseNote.NoteColor.r, baseNote.NoteColor.g, baseNote.NoteColor.b, 255 }, baseNote.Type, baseNote.Flick, (float)absTime + ((float)blockTime * (baseNote.YPos / 192f)), baseNote.Speed, baseNote.StartPoint, baseNote.EndPoint, prevArr);
                    NoteBasket.Add(note);
                    noteID++;
                }
                absTime += blockTime;
            }
            metadata.bpm = BPMs.ToArray();
            metadata.bpmQueue = BPMQueues.ToArray();
            metadata.beats = Beats.ToArray();
            metadata.beatsQueue = BeatsQueues.ToArray();

            TWxData data = new TWxData();
            data.metadata = metadata;
            data.notes = NoteBasket.ToArray();

            string jsonText = JsonMapper.ToJson(data);

            FileStream stream = new FileStream(GamePath.CreatorPath() + NameField.text + ".tw" + MaxLine.ToString(), FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(jsonText);
            writer.Close();
            stream.Close();
            MessageBox.Show(LocaleManager.instance.GetLocaleText("creator_saved"), LocaleManager.instance.GetLocaleText("creator_saveddesc") + NameField.text + ".tw" + MaxLine.ToString(), MessageBoxButton.OK);
        }
    }
}
