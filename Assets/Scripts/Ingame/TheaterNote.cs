using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TempestWave.Ingame
{
    public class TheaterNote : Note
    {
        #region Exclusive properties for THEATER mode
        public float SizeInPixelPrev { get; set; }
        public float SizeInPixelNext { get; set; }
        #endregion

        public TheaterNote() { }

        protected override void SetNoteObject()
        {
            if (!Flick.Equals(FlickMode.None)) { Body.sizeDelta = new Vector2(110, 110); }
            base.SetNoteObject();

            SizeInPixelPrev = SizeInPixel;
            SizeInPixelNext = SizeInPixel;
            if (Game.Mode.Equals(GameMode.Theater))
            {
                StartPos = new Vector3(-307.22195f + 87.7777f * StartLine, 217, 0);
                EndPos = new Vector3(-553 + 158 * EndLine, -240, 0);
            }
            else if (Game.Mode.Equals(GameMode.Theater4))
            {
                StartPos = new Vector3(-363.889f + 145.5556f * StartLine, 217, 0);
                EndPos = new Vector3(-655 + 262 * EndLine, -240, 0);
            }
            else if (Game.Mode.Equals(GameMode.Theater2L))
            {
                StartPos = new Vector3(-396.6666f + 264.4444f * StartLine, 217, 0);
                EndPos = new Vector3(-714 + 476 * EndLine, -240, 0);
            }
            else if (Game.Mode.Equals(GameMode.Theater2P))
            {
                StartPos = new Vector3(-256.5f + 171 * StartLine, 241, 0);
                EndPos = new Vector3(-567 + 378 * EndLine, -311, 0);
            }
            FlickBorderLeft = EndPos.x - 79;
            FlickBorderRight = EndPos.x + 79;
            BorderUp = 0;
            BorderDown = float.MinValue;
            if (SizeInPixel > 130)
            {
                if (Game.Mode.Equals(GameMode.Theater2P))
                {
                    StartPos = new Vector3(0, 241, 0);
                    EndPos = new Vector3(0, -311, 0);
                }
                else
                {
                    StartPos = new Vector3(0, 217, 0);
                    EndPos = new Vector3(0, -240, 0);
                }
                FlickBorderLeft = -100;
                FlickBorderRight = 100;
            }
        }

        public override void CreateTailConnector()
        {
            base.CreateTailConnector();

            if(TailToNext != null)
            {
                if (Game.IsNoteColored)
                    TailToNext.Color = new Color32(ColorKey.r, ColorKey.g, ColorKey.b, ColorKey.a);
                else
                    TailToNext.Color = new Color32(255, 255, 0, 255);

                if (Mode.Equals(NoteInfo.LongNoteStart))
                    TailToNext.SetEndPos(Game.Dispensor.Notes[NextNoteID].StartPos, 0, SizeInPixel * (140f / 100) * 1f / 2);
                else if (Mode.Equals(NoteInfo.SlideNoteStart) || Mode.Equals(NoteInfo.SlideNoteCheckpoint))
                {
                    TailToNext.SetStartPos(StartPos, 0f, SizeInPixel * (140f / 100) * 1f / 2);
                    TailToNext.SetEndPos(StartPos, 0f, SizeInPixel * (140f / 100) * 1f / 2);
                }
            }

            if (Mode.Equals(NoteInfo.SlideNoteCheckpoint))
                Game.Dispensor.VisibleNoteCount--;

            TailCreated = true;
        }

        public override void WakeupSignal()
        {
            base.WakeupSignal();

            if (Mode.Equals(NoteInfo.SlideNoteCheckpoint) && TailToNext == null)
            {
                FlickBorderLeft = EndPos.x - (79 * 1.1f);
                FlickBorderRight = EndPos.x + (79 * 1.1f);
            }
        }

        protected override void MovePhase()
        {
            base.MovePhase();

            if(gameObject.activeSelf)
            {
                float ProgressBy100 = CurrentFrame * Speed;
                float ProgressBy1 = ProgressBy100 / 100;
                if (!IsHit && (!Mode.Equals(NoteInfo.SlideNoteStart) || (Mode.Equals(NoteInfo.SlideNoteStart) && Game.Frame < ReachFrame)))
                {
                    if (Game.Mode.Equals(GameMode.Theater2P))
                    {
                        float cX = NotePath.GetTheaterX(StartPos.x, EndPos.x, ProgressBy1);
                        float cY = NotePath.GetTheaterPortY(ProgressBy100);
                        CurrentPos = new Vector3(cX, cY, CurrentPos.z);
                        Body.anchoredPosition3D = CurrentPos;
                        Body.localScale = new Vector3(NotePath.GetTheaterScale(ProgressBy1), NotePath.GetTheaterScale(ProgressBy1), 1);
                    }
                    else
                    {
                        float cX = NotePath.GetTheaterX(StartPos.x, EndPos.x, ProgressBy1);
                        float cY = NotePath.GetTheaterY(ProgressBy100);
                        CurrentPos = new Vector3(cX, cY, CurrentPos.z);
                        Body.anchoredPosition3D = CurrentPos;
                        Body.localScale = new Vector3(NotePath.GetTheaterScale(ProgressBy1), NotePath.GetTheaterScale(ProgressBy1), 1);
                    }
                    BeforePos = CurrentPos;
                }
                else if (Mode.Equals(NoteInfo.SlideNoteStart) && !Game.Paused && (IsHit || Game.Frame >= ReachFrame))
                {
                    Body.anchoredPosition3D += new Vector3(((Game.Dispensor.Notes[NextNoteID].EndPos.x - EndPos.x) / ((SlideFrameDistance + SlideFrameCalibrator) / 60)) * Time.deltaTime, 0);
                    if (IsHit)
                    {
                        FlickBorderLeft = Body.anchoredPosition3D.x - 79;
                        FlickBorderRight = Body.anchoredPosition3D.x + 79;
                    }
                }

                if (!Game.Paused)
                {
                    if (Mode.Equals(NoteInfo.LongNoteStart) && !IsHit)
                        TailToNext.SetStartPos(Body.anchoredPosition3D, ProgressBy1, SizeInPixelNext * (140f / 100));
                    if ((Mode.Equals(NoteInfo.SlideNoteStart) || Mode.Equals(NoteInfo.SlideNoteCheckpoint) && TailToNext != null))
                    {
                        TailToNext.SetStartPos(Body.anchoredPosition3D, (Mode.Equals(NoteInfo.SlideNoteStart) && Game.Frame >= ReachFrame) ? 1 : ProgressBy1, SizeInPixelNext * (140f / 100), (Game.Frame - LastSlideReachFrame) / (Game.Dispensor.Notes[NextNoteID].ReachFrame - LastSlideReachFrame));
                        if (!Game.Dispensor.Notes[NextNoteID].IsAppeared)
                            TailToNext.SetDeltaEndPos((Game.Dispensor.Notes[NextNoteID].StartPos - StartPos) / (SlideFrameDistance + SlideFrameCalibrator), 0, SizeInPixelNext * (140f / 100), (Game.Frame - LastSlideAppearFrame) / (Game.Dispensor.Notes[NextNoteID].AppearFrame - LastSlideAppearFrame));
                    }
                    if (TailsFromPrevious.Count > 0)
                    {
                        for (int i = 0; i < TailsFromPrevious.Count; i++)
                            if (TailsFromPrevious[i] != null)
                                TailsFromPrevious[i].SetEndPos(Body.anchoredPosition3D, CurrentFrame * Speed > 100 ? 1 : ProgressBy1, SizeInPixelPrev * (140f / 100));
                    }
                }
            }
        }

        protected override void ProcessPhase()
        {
            base.ProcessPhase();

            if (Mode.Equals(NoteInfo.SlideNoteCheckpoint))
            {
                if (CurrentFrame >= RunningFrames - 0.5f && TailToNext.gameObject.activeSelf && Game.Dispensor.Notes[PreviousTailID].IsHit)
                {
                    TransferTail();
                    Erase();
                }
            }
            else if((Mode.Equals(NoteInfo.LongNoteStart) || Mode.Equals(NoteInfo.SlideNoteStart)) && IsSlideHolding)
                Game.Judger.CalculateHoldScore();
            if (Mode.Equals(NoteInfo.SystemNoteXLStarter))
            {
                if (CurrentFrame * Speed >= 100)
                {
                    if (Game.Mode.Equals(GameMode.Theater)) { Game.SceneAnimate.Play("XLNote_Theater6L"); }
                    else if (Game.Mode.Equals(GameMode.Theater4)) { Game.SceneAnimate.Play("XLNote_Theater4L"); }
                    else if (Game.Mode.Equals(GameMode.Theater2L)) { Game.SceneAnimate.Play("XLNote_Theater2LP"); }
                    else if (Game.Mode.Equals(GameMode.Theater2P)) { Game.SceneAnimate.Play("XLNote_Theater2P"); }
                    Erase();
                }
            }
            if (Mode.Equals(NoteInfo.SystemNoteXLEnder))
            {
                if (CurrentFrame * Speed >= 100)
                {
                    if (Game.Mode.Equals(GameMode.Theater)) { Game.SceneAnimate.Play("XLNote2_Theater6L"); }
                    else if (Game.Mode.Equals(GameMode.Theater4)) { Game.SceneAnimate.Play("XLNote2_Theater4L"); }
                    else if (Game.Mode.Equals(GameMode.Theater2L)) { Game.SceneAnimate.Play("XLNote2_Theater2LP"); }
                    else if (Game.Mode.Equals(GameMode.Theater2P)) { Game.SceneAnimate.Play("XLNote2_Theater2P"); }
                    Erase();
                }
            }
        }

        protected override void InputPhase()
        {
            base.InputPhase();

            if (Game.IsAutoPlay)
            {
                if (Game.IsStarted.Equals(true) && (int)Mode < 6 && CurrentFrame >= RunningFrames - 0.5f && !IsHit)
                {
                    if (Mode.Equals(NoteInfo.SlideNoteCheckpoint))
                        Erase();
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
                            if ((Mode.Equals(NoteInfo.LongNoteEnd) || Mode.Equals(NoteInfo.SlideNoteEnd)) && Input.GetTouch(i).phase.Equals(TouchPhase.Ended))
                            {
                                Hit(point, Input.GetTouch(i).fingerId);
                            }
                        }
                    }
                }
                if ((Mode.Equals(NoteInfo.LongNoteStart) || Mode.Equals(NoteInfo.SlideNoteStart)) && IsHit)
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        Vector3 point = SyncPosition(Input.GetTouch(i).position);
                        if ((Input.GetTouch(i).phase.Equals(TouchPhase.Began) || Input.GetTouch(i).phase.Equals(TouchPhase.Stationary) || Input.GetTouch(i).phase.Equals(TouchPhase.Moved)))
                        {
                            if (point.x >= FlickBorderLeft && point.x < FlickBorderRight && !IsSlideHolding)
                            {
                                IsSlideHolding = true;
                                OwnAnimator.enabled = true;
                                gameObject.transform.GetChild(0).gameObject.SetActive(true);
                                OwnAnimator.Play("HoldEffect");
                                TouchedFinger = Input.GetTouch(i).fingerId;
                            }
                            else if (Input.GetTouch(i).fingerId.Equals(TouchedFinger) && !(point.x >= FlickBorderLeft && point.x < FlickBorderRight) && IsSlideHolding)
                            {
                                IsSlideHolding = false;
                                OwnAnimator.enabled = false;
                                gameObject.transform.GetChild(0).gameObject.SetActive(false);
                                TouchedFinger = 100;
                            }
                        }
                        if (Input.GetTouch(i).fingerId.Equals(TouchedFinger) && Input.GetTouch(i).phase.Equals(TouchPhase.Ended))
                        {
                            IsSlideHolding = false;
                            OwnAnimator.enabled = false;
                            gameObject.transform.GetChild(0).gameObject.SetActive(false);
                            TouchedFinger = 100;
                        }
                    }
                }
            }
        }

        protected override void InputSubPhase()
        {
            base.InputSubPhase();

            if (!Input.anyKey && !Game.IsAutoPlay && (Mode.Equals(NoteInfo.LongNoteStart) || Mode.Equals(NoteInfo.SlideNoteStart)) && IsHit)
            {
                bool fingerExists = false;
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (Input.GetTouch(i).fingerId.Equals(TouchedFinger)) { fingerExists = true; }
                }

                if (!fingerExists)
                {
                    if (!Game.Paused)
                    {
                        IsSlideHolding = false;
                        OwnAnimator.enabled = false;
                        gameObject.transform.GetChild(0).gameObject.SetActive(false);
                        //OwnAnimator.StopPlayback();
                        TouchedFinger = 100;
                    }
                }
            }
        }

        protected override void TransferTail()
        {
            base.TransferTail();

            TheaterNote prev = Game.Dispensor.Notes[TailsFromPrevious[0].OwnerID] as TheaterNote;
            prev.SizeInPixelNext = SizeInPixelNext;
        }
    }
}