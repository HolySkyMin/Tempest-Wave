using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TempestWave.Ingame
{
    public class NoteManager : MonoBehaviour
    {
        public int NoteCount { get; set; }
        public int VisibleNoteCount { get; set; }
        public Dictionary<int, Note> Notes { get; set; }
        public Dictionary<int, Note> SubNotes { get; set; }

        private int NoteIndex, XLNoteID = -1;
        private float CurScrollVal = 1.0f, XLReachFrame;
        private bool XLAppeared = false;
        private GameManager Game;
        private List<Note> RawNotes = new List<Note>();
        private List<Note> SortedNotes = new List<Note>();

        public NoteManager(GameManager g)
        {
            Notes = new Dictionary<int, Note>();
            Game = g;
            NoteCount = 0;
            NoteIndex = 1;
            VisibleNoteCount = 0;
        }

        public void UpdateNote(float frame)
        {
            GameObject left = null, right = null;

            foreach(KeyValuePair<int, Note> note in Notes)
            {
                if (frame >= note.Value.AppearFrame && !note.Value.IsAppeared)
                {
                    //Debug.Log("Note (ID: " + NoteIndex.ToString() + ") is going to be dispensed...");
                    note.Value.gameObject.SetActive(true);
                    //Debug.Log("Note (ID: " + NoteIndex.ToString() + ") is activated...");
                    //if ((int)note.Value.Mode < 10) { Game.Judger.SetGrid(note.Value.EndLine, note.Value.ID); }
                    note.Value.WakeupSignal();
                    //Debug.Log("Note (ID: " + NoteIndex.ToString() + ") is fully dispensed.");
                    if((int)note.Value.Mode < 6 || note.Value.Mode.Equals(NoteInfo.SystemNoteSlideDummy))
                    {
                        if(!((Game.Mode.Equals(GameMode.Theater) || Game.Mode.Equals(GameMode.Theater4) || Game.Mode.Equals(GameMode.Theater2P) || Game.Mode.Equals(GameMode.Theater2L)) && note.Value.Mode.Equals(NoteInfo.SlideNoteCheckpoint)))
                        {
                            if (left == null) { left = note.Value.gameObject; }
                            else { right = note.Value.gameObject; }
                        }
                    }
                    NoteIndex++;
                }
            }
            if(frame >= Notes[0].AppearFrame && !Notes[0].IsAppeared)
            {
                Notes[0].gameObject.SetActive(true);
                Notes[0].WakeupSignal();
            }

            if(right != null && left.GetComponent<Note>().ReachFrame.Equals(right.GetComponent<Note>().ReachFrame))
            {
                GameObject NewLine = Instantiate(Game.BaseMultiLine) as GameObject;
                NewLine.GetComponent<MultiLineControl>().Set(left, right);
                NewLine.SetActive(true);
            }
        }

        public void CreateNote(int ID, int Size, Color32 Color, NoteInfo Mode, FlickMode Flick, float Time, float Speed, float Start, float End, List<int> Prev)
        {
            if (Mode.Equals(NoteInfo.SlideNoteStart)) { CreateNote(ID, Size, Color, NoteInfo.SystemNoteSlideDummy, Flick, Time, Speed, Start, End, new List<int>()); }

            GameObject newNote = null;
            Note noteComp = null;
            if (Game.Mode.Equals(GameMode.Starlight))
            {
                if (Size <= 1) { newNote = Instantiate(Game.StarlightNote) as GameObject; }
                else { newNote = Instantiate(Game.StarlightXL) as GameObject; }
                noteComp = newNote.GetComponent<StarlightNote>();
            }
            else if(Game.Mode.Equals(GameMode.Theater) || Game.Mode.Equals(GameMode.Theater4) || Game.Mode.Equals(GameMode.Theater2L) || Game.Mode.Equals(GameMode.Theater2P))
            {
                if (Size.Equals(0)) { newNote = Instantiate(Game.TheaterSmall) as GameObject; }
                else if (Size.Equals(1)) { newNote = Instantiate(Game.TheaterLarge) as GameObject; }
                else if (Size > 1) { newNote = Instantiate(Game.TheaterXL) as GameObject; }
                noteComp = newNote.GetComponent<TheaterNote>();
            }
            else if(Game.Mode.Equals(GameMode.Platinum))
            {
                if (Size.Equals(0)) { newNote = Instantiate(Game.PlatinumSmall) as GameObject; }
                else if (Size.Equals(1)) { newNote = Instantiate(Game.PlatinumLarge) as GameObject; }
                else if (Size > 1) { newNote = Instantiate(Game.PlatinumXL) as GameObject; }
                noteComp = newNote.GetComponent<PlatinumNote>();
            }
            newNote.GetComponent<RectTransform>().SetParent(Game.NoteParent, false);
            float resist = 0;
            if(!Flick.Equals(FlickMode.None))
            {
                int flickVal;
                if (PlayerPrefs.HasKey("flickmode")) { flickVal = PlayerPrefs.GetInt("flickmode"); }
                else { flickVal = 1; }
                if (flickVal.Equals(0)) { resist = 20; }
                else if (flickVal.Equals(1)) { resist = 40; }
                else if (flickVal.Equals(2)) { resist = 60; }
                else if (flickVal.Equals(3))
                {
                    // TODO 하기
                    //if (!Game.Mode.Equals(GameMode.Starlight)) { resist = 35; }
                    if (Game.Mode.Equals(GameMode.Starlight))
                    {
                        bool applied = false;
                        for (int i = 0; i < Prev.Count; i++)
                        {
                            if (!Notes[Prev[i]].Flick.Equals(FlickMode.None))
                            {
                                float decVal;
                                if (Time * (1 / Game.ScrollAmp) - Notes[Prev[i]].ReachFrame < 4) { decVal = 20; }
                                else if (Time * (1 / Game.ScrollAmp) - Notes[Prev[i]].ReachFrame >= 20) { decVal = 60; }
                                else { decVal = 20 + (2.5f * (Time * (1 / Game.ScrollAmp) - Notes[Prev[i]].ReachFrame - 4)); }
                                Notes[Prev[i]].FlickThreshold = decVal;
                                resist = decVal;
                                applied = true;
                            }
                        }
                        if (!applied) { resist = 40; }
                    }
                    else { resist = 50; }
                }
            }
            noteComp.SetNote(ID, Start, End, Time * (1 / Game.ScrollAmp), Speed, resist, Mode, Flick, Color, Prev, 0);
            if ((int)noteComp.Mode < 10)
            {
                if (!Game.Judger.NoteQueue.ContainsKey(noteComp.EndLine)) { Game.Judger.MakeNewKey(noteComp.EndLine); }
                Game.Judger.NoteQueue[noteComp.EndLine].Add(noteComp.ID);
            }
            if (!Mode.Equals(NoteInfo.SystemNoteSlideDummy))
            {
                Notes.Add(ID, noteComp);
                NoteCount++;
            }
            else { Notes.Add(XLNoteID--, noteComp); }
            for (int i = 0; i < Prev.Count; i++)
            {
                Notes[Prev[i]].NextNoteID = ID;
                Notes[Prev[i]].CreateTailConnector();
                if(Game.Mode.Equals(GameMode.Theater) || Game.Mode.Equals(GameMode.Theater4) || Game.Mode.Equals(GameMode.Theater2L) || Game.Mode.Equals(GameMode.Theater2P))
                {
                    if ((Notes[Prev[i]].Mode.Equals(NoteInfo.LongNoteStart) ||
                        Notes[Prev[i]].Mode.Equals(NoteInfo.SlideNoteStart) ||
                        Notes[Prev[i]].Mode.Equals(NoteInfo.SlideNoteCheckpoint)) && Size.Equals(0)) { (Notes[Prev[i]] as TheaterNote).SizeInPixelNext = 70; }
                    else if((Notes[Prev[i]].Mode.Equals(NoteInfo.LongNoteStart) ||
                        Notes[Prev[i]].Mode.Equals(NoteInfo.SlideNoteStart) ||
                        Notes[Prev[i]].Mode.Equals(NoteInfo.SlideNoteCheckpoint)) && Notes[Prev[i]].SizeInPixel.Equals(70)) { (Notes[ID] as TheaterNote).SizeInPixelPrev = 70; }
                }
                else if(Game.Mode.Equals(GameMode.Platinum))
                {
                    if ((Notes[Prev[i]].Mode.Equals(NoteInfo.LongNoteStart) ||
                        Notes[Prev[i]].Mode.Equals(NoteInfo.SlideNoteStart) ||
                        Notes[Prev[i]].Mode.Equals(NoteInfo.SlideNoteCheckpoint)) && Size.Equals(0)) { (Notes[Prev[i]] as PlatinumNote).SizeInPixelNext = 70; }
                    else if ((Notes[Prev[i]].Mode.Equals(NoteInfo.LongNoteStart) ||
                        Notes[Prev[i]].Mode.Equals(NoteInfo.SlideNoteStart) ||
                        Notes[Prev[i]].Mode.Equals(NoteInfo.SlideNoteCheckpoint)) && Notes[Prev[i]].SizeInPixel.Equals(70)) { (Notes[ID] as PlatinumNote).SizeInPixelPrev = 70; }
                }
            }
            if ((int)noteComp.Mode < 7) { VisibleNoteCount++; }

            if(Mode.Equals(NoteInfo.SystemNoteScroller)) // 스크롤러일 경우 배수 값을 저장합니다.
            {
                CurScrollVal = Speed;
            }

            if(Size >= 2) // 스페셜 노트가 나타났을 경우
            {
                XLReachFrame = Time * (1 / Game.ScrollAmp);
                if(!XLAppeared) // 그중 처음일 경우
                {
                    GameObject xlStart = GetNoteInstance();
                    xlStart.GetComponent<RectTransform>().SetParent(Game.NoteParent, false);
                    xlStart.GetComponent<Note>().Game = Game;
                    xlStart.GetComponent<Note>().SetNote(XLNoteID, 0, 0, (Time * (1 / Game.ScrollAmp)) - (45 * CurScrollVal), Game.GlobalNoteSpeed, 0, NoteInfo.SystemNoteXLStarter, FlickMode.None, new Color32(0, 0, 0, 0), new List<int>(), 0);
                    Notes.Add(XLNoteID--, xlStart.GetComponent<Note>());
                    XLAppeared = true;
                }
            }
            else if(Size < 2 && XLAppeared) // 스페셜 노트 이후 첫 다른 노트가 나타날 경우
            {
                GameObject xlEnd = GetNoteInstance();
                xlEnd.GetComponent<RectTransform>().SetParent(Game.NoteParent, false);
                xlEnd.GetComponent<Note>().Game = Game;
                xlEnd.GetComponent<Note>().SetNote(XLNoteID, 0, 0, (Time * (1 / Game.ScrollAmp)) - (90 * CurScrollVal) >= XLReachFrame + 15 ? (Time * (1 / Game.ScrollAmp)) - (90 * CurScrollVal) : XLReachFrame + 15, Game.GlobalNoteSpeed, 0, NoteInfo.SystemNoteXLEnder, FlickMode.None, new Color32(0, 0, 0, 0), new List<int>(), 0);
                Notes.Add(XLNoteID--, xlEnd.GetComponent<Note>());
                XLAppeared = false;
            }
        }

        private GameObject GetNoteInstance()
        {
            if (Game.Mode.Equals(GameMode.Starlight)) { return Instantiate(Game.StarlightNote) as GameObject; }
            else if (Game.Mode.Equals(GameMode.Theater) || Game.Mode.Equals(GameMode.Theater4) || Game.Mode.Equals(GameMode.Theater2L) || Game.Mode.Equals(GameMode.Theater2P)) { return Instantiate(Game.TheaterSmall) as GameObject; }
            else if (Game.Mode.Equals(GameMode.Platinum)) { return Instantiate(Game.PlatinumSmall) as GameObject; }
            else { return Instantiate(Game.SystemNote) as GameObject; }
        }
    }
}
