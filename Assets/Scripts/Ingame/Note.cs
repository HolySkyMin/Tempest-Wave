using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TempestWave.Ingame
{
    public class Note : MonoBehaviour
    {
        #region Fundamental note data properties
        public int ID { get; set; }
        public int NextNoteID { get; set; }
        public float AppearFrame { get; set; }
        public float ReachFrame { get; set; }
        public float RunningFrames { get; set; }
        public float Speed { get; set; }
        public float StartLine { get; set; }
        public float EndLine { get; set; }
        public NoteInfo Mode { get; set; }
        public FlickMode Flick { get; set; }
        public Color32 ColorKey { get; set; }
        public List<int> PreviousNoteIDs { get; set; }
        #endregion

        #region Core note properties as an object
        public float CurrentFrame { get; set; }
        public float SizeInPixel { get; set; }
        public float BorderUp { get; set; }
        public float BorderDown { get; set; }
        public bool IsAppeared { get; set; }
        public bool IsHit { get; set; }
        public Vector3 StartPos { get; set; }
        public Vector3 EndPos { get; set; }
        public Vector3 CurrentPos { get; set; }
        public Vector3 BeforePos { get; set; }
        public ImprovedTail TailToNext { get; set; }
        public ImprovedConnector ConnectorToNext { get; set; }
        public List<ImprovedTail> TailsFromPrevious { get; set; }
        public List<ImprovedConnector> ConnectorsFromPrevious { get; set; }
        #endregion

        #region Extension for Hold/Flick/Slide
        public int TouchedFinger { get; set; }
        public int PreviousTailID { get; set; }
        public float FlickBorderLeft { get; set; }
        public float FlickBorderRight { get; set; }
        public float FlickThreshold { get; set; }
        public float LastSlideAppearFrame { get; set; }
        public float LastSlideReachFrame { get; set; }
        public float SlideFrameCalibrator { get; set; }
        public float SlideFrameDistance { get; set; }
        public bool IsSlideHolding { get; set; }
        public bool IsSlideGroupDead { get; set; }
        #endregion

        public GameManager Game;
        public RectTransform Body;
        public Animator OwnAnimator;
        protected float FlickStartedPos;
        protected bool IsFlickBeingHitted, IsSlideCheckHitted, TailCreated;

        public Note()
        {
            PreviousNoteIDs = new List<int>();
            TailsFromPrevious = new List<ImprovedTail>();
            TailToNext = null;
            ConnectorsFromPrevious = new List<ImprovedConnector>();
            ConnectorToNext = null;
            TailCreated = false;
        }

        public int CompareTo(Note target)
        {
            if (target == null) { return 1; }
            else { return AppearFrame.CompareTo(target.AppearFrame); }
        }

        public void SetNote(int id, float start, float end, float frame, float speed, float threshold, NoteInfo mode, FlickMode flick, Color32 color, List<int> previous, int next)
        {
            ID = id;
            StartLine = start;
            EndLine = end;
            ReachFrame = frame;
            Speed = speed;
            FlickThreshold = threshold;
            Mode = mode;
            Flick = flick;
            ColorKey = color;
            PreviousNoteIDs = previous;
            NextNoteID = next;

            SetNoteObject();
            ResetValue();
        }

        protected virtual void SetNoteObject()
        {
            SizeInPixel = Body.sizeDelta.x;
            gameObject.GetComponent<Image>().sprite = Game.GetNoteTexture(Mode, Flick);
            if (SizeInPixel > 130) { gameObject.GetComponent<Image>().sprite = Instantiate(Game.IsNoteColored ? Game.WhiteTexture[9] : Game.NoteTexture[9]) as Sprite; }
            if (Flick.Equals(FlickMode.Left))
            {
                Body.sizeDelta = new Vector2(Body.sizeDelta.y * 1.2f, Body.sizeDelta.y);
                Body.pivot = new Vector2(7f / 12f, 0.5f);
            }
            else if (Flick.Equals(FlickMode.Right))
            {
                Body.sizeDelta = new Vector2(Body.sizeDelta.y * 1.2f, Body.sizeDelta.y);
                Body.pivot = new Vector2(5f / 12f, 0.5f);
            }
            else if (Flick.Equals(FlickMode.Up))
            {
                Body.sizeDelta = new Vector2(Body.sizeDelta.x, Body.sizeDelta.x * 1.2f);
                Body.pivot = new Vector2(0.5f, 5f / 12f);
            }
            else if (Flick.Equals(FlickMode.Down))
            {
                Body.sizeDelta = new Vector2(Body.sizeDelta.x, Body.sizeDelta.x * 1.2f);
                Body.pivot = new Vector2(0.5f, 7f / 12f);
            }
            if (Game.IsNoteColored) { gameObject.GetComponent<Image>().color = ColorKey; }
            if (gameObject.GetComponent<Image>().sprite == null) { gameObject.GetComponent<Image>().color = Color.clear; }
            Body.SetAsFirstSibling();
            RunningFrames = 100 / Speed;
            AppearFrame = ReachFrame - RunningFrames;
            if (Mode.Equals(NoteInfo.SystemNoteStarter)) { Game.Frame -= RunningFrames; }
        }

        public virtual void CreateTailConnector()
        {
            if (TailCreated) { return; }
            if (NextNoteID > 0)
            {
                if (Mode.Equals(NoteInfo.LongNoteStart) || Mode.Equals(NoteInfo.SlideNoteStart) || (Mode.Equals(NoteInfo.SlideNoteCheckpoint) && (Game.Dispensor.Notes[NextNoteID].Mode.Equals(NoteInfo.SlideNoteCheckpoint) || Game.Dispensor.Notes[NextNoteID].Mode.Equals(NoteInfo.SlideNoteEnd))))
                {
                    GameObject MyTail = Instantiate(Game.BaseTail) as GameObject;
                    MyTail.name = "t" + ID.ToString();
                    TailToNext = MyTail.GetComponent<ImprovedTail>();
                    TailToNext.Game = Game;
                    if (Mode.Equals(NoteInfo.LongNoteStart))
                        TailToNext.SetLines(StartPos.x, EndPos.x, Game.Dispensor.Notes[NextNoteID].StartPos.x, Game.Dispensor.Notes[NextNoteID].EndPos.x, true);
                    else if (Mode.Equals(NoteInfo.SlideNoteStart) || Mode.Equals(NoteInfo.SlideNoteCheckpoint))
                    {
                        TailToNext.SetLines(StartPos.x, EndPos.x, Game.Dispensor.Notes[NextNoteID].StartPos.x, Game.Dispensor.Notes[NextNoteID].EndPos.x, false);
                        LastSlideAppearFrame = AppearFrame;
                        LastSlideReachFrame = ReachFrame;
                    }
                    TailToNext.OwnerID = ID;
                    Game.Dispensor.Notes[NextNoteID].TailsFromPrevious.Add(TailToNext);
                    Game.Dispensor.Notes[NextNoteID].PreviousTailID = ID;
                }
            }
        }

        protected void ResetValue()
        {
            TouchedFinger = 50;
            CurrentFrame = 0;
            IsAppeared = false;
            IsHit = false;
            IsFlickBeingHitted = false;
            IsSlideCheckHitted = false;
            IsSlideGroupDead = false;
            CurrentPos = StartPos;
            BeforePos = StartPos;
        }

        public virtual void WakeupSignal()
        {
            Body.anchoredPosition3D = StartPos;

            InitPhase();
            IsAppeared = true;
            if (TailToNext != null) { TailToNext.gameObject.SetActive(true); }
            if (ConnectorToNext != null) { ConnectorToNext.gameObject.SetActive(true); }
            if (Mode.Equals(NoteInfo.SlideNoteCheckpoint) && TailToNext == null)
            {
                Mode = NoteInfo.SlideNoteEnd;
                if (Game.IsNoteColored) { gameObject.GetComponent<Image>().color = ColorKey; }
                else { gameObject.GetComponent<Image>().color = Color.white; }
                gameObject.GetComponent<Image>().sprite = Game.GetNoteTexture(Mode, Flick);
            }
            if (Mode.Equals(NoteInfo.SlideNoteStart) || Mode.Equals(NoteInfo.SlideNoteCheckpoint))
            {
                SlideFrameDistance = Game.Dispensor.Notes[NextNoteID].AppearFrame - AppearFrame;
            }
        }

        protected void Update()
        {
            InputSubPhase();
            if (!Game.Paused)
            {
                InitPhase();
                MovePhase();
                ProcessPhase();
                InputPhase();
            }
        }

        protected virtual void InitPhase()
        {
            if (Mode.Equals(NoteInfo.LongNoteEnd) && Game.Judger.HoldNoteDead[EndLine])
            {
                Game.Judger.HoldNoteDead[EndLine] = false;
                Erase();
            }
            if ((Mode.Equals(NoteInfo.SlideNoteCheckpoint) || Mode.Equals(NoteInfo.SlideNoteEnd)) && Game.Dispensor.Notes[PreviousNoteIDs[0]].IsSlideGroupDead)
            {
                IsSlideGroupDead = true;
                Erase();
            }
        }

        protected virtual void MovePhase()
        {
            if (gameObject.activeSelf)
                CurrentFrame = Game.Frame - AppearFrame;
        }

        protected virtual void ProcessPhase()
        {
            if ((int)Mode < 10)
            {
                if (Mode.Equals(NoteInfo.DamageNote))
                {
                    if (CurrentFrame * Speed >= 100)
                    {
                        Game.Judger.HitJudge(Game.Mode, Mode, Flick, CurrentFrame, RunningFrames, false);
                        IsHit = true;
                        Game.ShortEffectPlay(Mathf.RoundToInt(EndLine), false);
                        Erase();
                    }
                }
                else if ((Mode.Equals(NoteInfo.LongNoteStart) || Mode.Equals(NoteInfo.SlideNoteStart)) && IsSlideHolding)
                {
                    if (Mode.Equals(NoteInfo.SlideNoteStart) && Game.Dispensor.Notes[NextNoteID].Mode.Equals(NoteInfo.SlideNoteEnd) && Game.Frame >= Game.Dispensor.Notes[NextNoteID].ReachFrame) { Erase(); }
                }
                else
                {
                    if (Mode.Equals(NoteInfo.HiddenNote) && CurrentFrame >= RunningFrames + 4f) { Erase(); }
                    if (CurrentFrame > RunningFrames + 13 && !IsHit)
                    {
                        if (Mode.Equals(NoteInfo.LongNoteStart))
                        {
                            Game.Judger.HoldNoteDead[EndLine] = true;
                            Game.Judger.JudgeAsMiss();
                        }
                        if (Mode.Equals(NoteInfo.LongNoteEnd))
                        {
                            for (int i = 0; i < TailsFromPrevious.Count; i++)
                            {
                                Game.Dispensor.Notes[TailsFromPrevious[i].OwnerID].Erase();
                            }
                        }
                        if (Mode.Equals(NoteInfo.SlideNoteStart))
                        {
                            IsSlideGroupDead = true;
                            Game.Judger.NoteQueue[EndLine].Remove(ID);
                        }
                        Game.Judger.JudgeAsMiss();
                        Erase();
                    }
                }
            }
            else
            {
                if (Mode.Equals(NoteInfo.SystemNoteStarter))
                {
                    if (CurrentFrame * Speed >= 100)
                    {
                        Game.IsPlaying = true;
                        Game.SongDelay = RunningFrames;
                        Game.MusicFile.Play();
                        Erase();
                    }
                }
                if (Mode.Equals(NoteInfo.SystemNoteEnder))
                {
                    if (CurrentFrame * Speed >= 100)
                    {
                        Game.IsPlaying = false;
                        Game.IsEnded = true;
                        Erase();
                    }
                }
                if (Mode.Equals(NoteInfo.SystemNoteSpeeder))
                {
                    if (CurrentFrame * Speed >= 100)
                    {
                        Game.GlobalNoteSpeed = Game.SecuredNoteSpeed * Speed;
                        Erase();
                    }
                }
                if (Mode.Equals(NoteInfo.SystemNoteScroller))
                {
                    if (CurrentFrame * Speed >= 100)
                    {
                        Game.FrameSpeed = Speed;
                        Erase();
                    }
                }
                if (Mode.Equals(NoteInfo.SystemNoteSlideDummy))
                {
                    if (Game.Dispensor.Notes[ID].IsHit) { Erase(); }
                    if (CurrentFrame >= RunningFrames + 13f) { Erase(); }
                }
            }
        }

        protected virtual void InputPhase()
        {
            if (Game.IsAutoPlay)
            {
                if (Game.IsStarted.Equals(true) && (int)Mode < 6 && CurrentFrame >= RunningFrames - 0.5f && !IsHit)
                {
                    if (Mode.Equals(NoteInfo.SlideNoteCheckpoint)) { TransferTail(); }
                    else { Hit(EndPos, 0); }
                    if (Mode.Equals(NoteInfo.SlideNoteEnd)) { Game.Dispensor.Notes[TailsFromPrevious[0].OwnerID].Erase(); }
                }
            }
            else
            {
                if ((int)Mode < 10 && !Mode.Equals(NoteInfo.SlideNoteCheckpoint) && Input.touchCount > 0 && ((Game.Judger.NoteQueue[EndLine].Count < 1 || Game.Judger.NoteQueue[EndLine][0].Equals(ID)) || (Mode.Equals(NoteInfo.LongNoteEnd) && Game.Dispensor.Notes[TailsFromPrevious[0].OwnerID].IsHit)))
                {
                    if (Flick.Equals(FlickMode.None))
                    {
                        for (int i = 0; i < Input.touchCount; i++)
                        {
                            Vector3 point = SyncPosition(Input.GetTouch(i).position);
                            if (!(Mode.Equals(NoteInfo.LongNoteEnd) || Mode.Equals(NoteInfo.SlideNoteEnd)) && Input.GetTouch(i).phase.Equals(TouchPhase.Began) && !Game.Judger.TapHitted[EndLine]) { Hit(point, Input.GetTouch(i).fingerId); }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Input.touchCount; i++)
                        {
                            if (Input.GetTouch(i).phase.Equals(TouchPhase.Moved))
                            {
                                Vector3 point = SyncPosition(Input.GetTouch(i).position);
                                Vector3 deltaPoint = point - SyncPosition(Input.GetTouch(i).position - Input.GetTouch(i).deltaPosition);
                                if (Flick.Equals(FlickMode.Left))
                                {
                                    if (deltaPoint.x < 0) { FlickCheck(point, deltaPoint.x, Input.GetTouch(i).fingerId); }
                                }
                                else if (Flick.Equals(FlickMode.Right))
                                {
                                    if (deltaPoint.x > 0) { FlickCheck(point, deltaPoint.x, Input.GetTouch(i).fingerId); }
                                }
                                else if (Flick.Equals(FlickMode.Up))
                                {
                                    if (deltaPoint.y > 0) { FlickCheck(point, deltaPoint.y, Input.GetTouch(i).fingerId); }
                                }
                                else if (Flick.Equals(FlickMode.Down))
                                {
                                    if (deltaPoint.y < 0) { FlickCheck(point, deltaPoint.y, Input.GetTouch(i).fingerId); }
                                }
                            }
                            if (Input.GetTouch(i).fingerId.Equals(TouchedFinger) && Input.GetTouch(i).phase.Equals(TouchPhase.Ended))
                            {
                                IsFlickBeingHitted = false;
                                TouchedFinger = 100;
                            }
                        }
                    }
                }

                // 키보드 입력
                if ((int)Mode < 6 && !(Mode.Equals(NoteInfo.LongNoteEnd) || Mode.Equals(NoteInfo.SlideNoteStart) || Mode.Equals(NoteInfo.SlideNoteCheckpoint) || Mode.Equals(NoteInfo.SlideNoteEnd)) && (Game.Judger.NoteQueue[EndLine].Count < 1 || Game.Judger.NoteQueue[EndLine][0].Equals(ID)))
                {
                    if (Input.GetKeyDown(Game.Keys[Mathf.RoundToInt(EndLine - 1)]) && !Game.Judger.TapHitted[EndLine]) { Hit(EndPos, 0); }
                }
                if (Mode.Equals(NoteInfo.LongNoteEnd) && Game.Dispensor.Notes[PreviousTailID].IsHit)
                {
                    if (Input.GetKeyUp(Game.Keys[Mathf.RoundToInt(EndLine - 1)])) { Hit(EndPos, 0); }
                }
            }
        }

        protected virtual void InputSubPhase() { }

        protected Vector3 SyncPosition(Vector3 target)
        {
            Vector3 pos = new Vector3();
            if (Game.Mode.Equals(GameMode.Theater2P))
            {
                if (Screen.height / (float)Screen.width >= 16f / 9) { pos.Set(((target.x - (Camera.main.pixelWidth / 2f)) / Camera.main.pixelWidth) * 720f, ((target.y - (Camera.main.pixelHeight / 2f)) / Camera.main.pixelHeight) * (720f * ((float)Camera.main.pixelHeight / Camera.main.pixelWidth)), target.z); }
                else { pos.Set(((target.x - (Camera.main.pixelWidth / 2f)) / Camera.main.pixelWidth) * (1280f * ((float)Camera.main.pixelWidth / Camera.main.pixelHeight)), ((target.y - (Camera.main.pixelHeight / 2f)) / Camera.main.pixelHeight) * 1280f, target.z); }
            }
            else
            {
                if (Screen.width / (float)Screen.height >= 16f / 9) { pos.Set(((target.x - (Camera.main.pixelWidth / 2f)) / Camera.main.pixelWidth) * (720f * ((float)Camera.main.pixelWidth / Camera.main.pixelHeight)), ((target.y - (Camera.main.pixelHeight / 2f)) / Camera.main.pixelHeight) * 720f, target.z); }
                else { pos.Set(((target.x - (Camera.main.pixelWidth / 2f)) / Camera.main.pixelWidth) * 1280f, ((target.y - (Camera.main.pixelHeight / 2f)) / Camera.main.pixelHeight) * (1280f * ((float)Camera.main.pixelHeight / Camera.main.pixelWidth)), target.z); }
            }
            return pos;
        }

        protected virtual void TransferTail()
        {
            Note prev = Game.Dispensor.Notes[TailsFromPrevious[0].OwnerID];
            Game.Dispensor.Notes[NextNoteID].PreviousNoteIDs.Remove(ID);
            Game.Dispensor.Notes[NextNoteID].PreviousNoteIDs.Add(TailsFromPrevious[0].OwnerID);
            Game.Dispensor.Notes[NextNoteID].PreviousTailID = PreviousTailID;
            TailToNext.OwnerID = TailsFromPrevious[0].OwnerID;
            // send variables to slide note start.
            prev.NextNoteID = NextNoteID;
            prev.StartPos = StartPos;
            prev.EndPos = EndPos;
            prev.SlideFrameCalibrator = 0;
            prev.SlideFrameDistance = SlideFrameDistance;
            prev.LastSlideAppearFrame = AppearFrame;
            prev.LastSlideReachFrame = ReachFrame;
            prev.TailToNext.gameObject.SetActive(false);
            prev.Body.anchoredPosition3D = EndPos;
            prev.TailToNext = TailToNext;
            TailToNext = null;
        }

        protected void Hit(Vector3 pos, int finger)
        {
            if ((Mode.Equals(NoteInfo.LongNoteEnd) || Mode.Equals(NoteInfo.SlideNoteEnd)))
            {
                if (!Game.Dispensor.Notes[PreviousTailID].IsHit) { return; }
            }
            if ((Mode.Equals(NoteInfo.SlideNoteCheckpoint) || Mode.Equals(NoteInfo.SlideNoteEnd)))
            {
                int Count = 0;
                for (int i = 0; i < PreviousNoteIDs.Count; i++)
                {
                    if (Game.Dispensor.Notes[PreviousNoteIDs[i]].Mode.Equals(NoteInfo.SlideNoteStart)) { Count++; }
                }
                if (Count.Equals(0)) { return; }
            }
            if (pos.y < BorderUp && pos.y >= BorderDown && pos.x >= FlickBorderLeft && pos.x < FlickBorderRight && !IsHit)
            {
                if (Game.Judger.HitJudge(Game.Mode, Mode, Flick, CurrentFrame, RunningFrames, true))
                {
                    TouchedFinger = finger;
                    if (Flick.Equals(FlickMode.None)) { Game.Judger.TapHitted[EndLine] = true; }
                    else { Game.Judger.SetFlickHitted(Flick, EndLine); }
                    IsHit = true;
                    if (!Mode.Equals(NoteInfo.SlideNoteStart))
                    {
                        gameObject.GetComponent<Image>().color = Color.clear;
                    }
                    Game.ShortEffectPlay(Mathf.RoundToInt(EndLine), SizeInPixel > 150 ? true : false);

                    if (Mode.Equals(NoteInfo.LongNoteEnd)) { Game.Dispensor.Notes[TailsFromPrevious[0].OwnerID].Erase(); }
                    if (Mode.Equals(NoteInfo.LongNoteStart))
                    {
                        Body.anchoredPosition3D = EndPos;
                        TailToNext.SetStartPos(Body.anchoredPosition3D, 1, SizeInPixel * (140f / 100) * 1);
                        IsSlideHolding = true;
                        OwnAnimator.Play("HoldEffect");
                    }
                    else if (Mode.Equals(NoteInfo.SlideNoteStart))
                    {
                        Game.Judger.NoteQueue[EndLine].Remove(ID);
                        IsSlideHolding = true;
                        OwnAnimator.Play("HoldEffect");
                        if (Game.Frame < ReachFrame)
                        {
                            Body.anchoredPosition3D = EndPos;
                            SlideFrameCalibrator = RunningFrames - CurrentFrame;
                            LastSlideReachFrame = Game.Frame;
                        }
                    }
                    else { Erase(); }
                }
            }
        }

        protected void FlickCheck(Vector3 pos, float delta, int finger)
        {
            if (!IsFlickBeingHitted && CurrentFrame >= RunningFrames - 13f && Game.Judger.FindFlickHitted(Flick, EndLine).Equals(false))
            {
                if (Flick.Equals(FlickMode.Left))
                {
                    if (pos.x <= FlickBorderRight && pos.x - delta > FlickBorderLeft) { IsFlickBeingHitted = true; FlickStartedPos = pos.x - delta; TouchedFinger = finger; }
                }
                if (Flick.Equals(FlickMode.Right))
                {
                    if (pos.x >= FlickBorderLeft && pos.x - delta < FlickBorderRight) { IsFlickBeingHitted = true; FlickStartedPos = pos.x - delta; TouchedFinger = finger; }
                }
                if (Flick.Equals(FlickMode.Up))
                {
                    if (pos.x >= FlickBorderLeft && pos.x < FlickBorderRight && pos.y < 0) { IsFlickBeingHitted = true; FlickStartedPos = pos.y - delta; TouchedFinger = finger; }
                }
                if (Flick.Equals(FlickMode.Down))
                {
                    if (pos.x >= FlickBorderLeft && pos.x < FlickBorderRight && pos.y < 0) { IsFlickBeingHitted = true; FlickStartedPos = pos.y - delta; TouchedFinger = finger; }
                }
            }
            if (IsFlickBeingHitted && CurrentFrame >= RunningFrames - 13f && finger.Equals(TouchedFinger) && Game.Judger.FindFlickHitted(Flick, EndLine).Equals(false))
            {
                if (Flick.Equals(FlickMode.Left))
                {
                    if (pos.x - FlickStartedPos < FlickThreshold * -1) { Hit(EndPos, TouchedFinger); }
                }
                if (Flick.Equals(FlickMode.Right))
                {
                    if (pos.x - FlickStartedPos > FlickThreshold) { Hit(EndPos, TouchedFinger); ; }
                }
                if (Flick.Equals(FlickMode.Up))
                {
                    if (pos.y - FlickStartedPos > FlickThreshold) { Hit(EndPos, TouchedFinger); ; }
                }
                if (Flick.Equals(FlickMode.Down))
                {
                    if (pos.y - FlickStartedPos < FlickThreshold * -1) { Hit(EndPos, TouchedFinger); ; }
                }
            }
        }

        public void Erase()
        {
            IsHit = true;
            OwnAnimator.StopPlayback();
            if ((int)Mode < 10 && !Mode.Equals(NoteInfo.SlideNoteStart)) { Game.Judger.NoteQueue[EndLine].Remove(ID); }
            if (Mode.Equals(NoteInfo.SlideNoteEnd))
            {
                if (Game.Dispensor.Notes[TailsFromPrevious[0].OwnerID] != null) { Game.Dispensor.Notes[TailsFromPrevious[0].OwnerID].Erase(); }
            }
            if (TailToNext != null)
            {
                Game.Trashcan.Add(TailToNext.gameObject);
                TailToNext.gameObject.SetActive(false);
            }
            if (ConnectorToNext != null)
            {
                Game.Trashcan.Add(ConnectorToNext.gameObject);
                ConnectorToNext.gameObject.SetActive(false);
            }
            if (ConnectorsFromPrevious.Count > 0)
            {
                for (int i = 0; i < ConnectorsFromPrevious.Count; i++)
                {
                    if (ConnectorsFromPrevious[i] != null)
                    {
                        Game.Trashcan.Add(ConnectorsFromPrevious[i].gameObject);
                        ConnectorsFromPrevious[i].gameObject.SetActive(false);
                    }
                }
            }
            Game.Trashcan.Add(gameObject);
            gameObject.SetActive(false);
        }
    }
}
