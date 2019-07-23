using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TempestWave.Ingame
{
    public class StarlightNote : Note
    {
        public StarlightNote() { }

        protected override void SetNoteObject()
        {
            base.SetNoteObject();

            StartPos = new Vector3(-432 + 144 * StartLine, 212, 0);
            EndPos = new Vector3(-588 + 196 * EndLine, -238, 0);
            FlickBorderLeft = EndPos.x - 98;
            FlickBorderRight = EndPos.x + 98;
            BorderUp = 0;
            BorderDown = float.MinValue;
            if (SizeInPixel > 130)
            {
                StartPos = new Vector3(0, 212, 0);
                EndPos = new Vector3(0, -238, 0);
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
                    TailToNext.Color = new Color32(ColorKey.r, ColorKey.g, ColorKey.b, 180);
                else
                    TailToNext.Color = new Color32(255, 255, 255, 180);

                if (Mode.Equals(NoteInfo.LongNoteStart))
                    TailToNext.SetEndPos(Game.Dispensor.Notes[NextNoteID].StartPos, 0, 0);
                else if (Mode.Equals(NoteInfo.SlideNoteStart) || Mode.Equals(NoteInfo.SlideNoteCheckpoint))
                {
                    TailToNext.SetStartPos(StartPos, 0f, 0);
                    TailToNext.SetEndPos(Game.Dispensor.Notes[NextNoteID].StartPos, 0f, 0);
                }
            }

            if (!Flick.Equals(FlickMode.None))
            {
                GameObject MyConnector = Instantiate(Game.BaseConnector) as GameObject;
                MyConnector.name = "c" + ID.ToString();
                ConnectorToNext = MyConnector.GetComponent<ImprovedConnector>();
                ConnectorToNext.OwnerID = ID;
                if (Game.IsNoteColored) { ConnectorToNext.MainRenderer.material.color = new Color32(ColorKey.r, ColorKey.g, ColorKey.b, 180); }
                ConnectorToNext.EndPos = Game.Dispensor.Notes[NextNoteID].StartPos;
                ConnectorToNext.EndScale = 0;
                Game.Dispensor.Notes[NextNoteID].ConnectorsFromPrevious.Add(ConnectorToNext);
            }

            TailCreated = true;
        }

        public override void WakeupSignal()
        {
            base.WakeupSignal();

            if (Mode.Equals(NoteInfo.SlideNoteEnd))
            {
                FlickBorderLeft = EndPos.x - (98 * 1.2f);
                FlickBorderRight = EndPos.x + (98 * 1.2f);
            }
        }

        protected override void InitPhase()
        {
            if ((Mode.Equals(NoteInfo.SlideNoteCheckpoint) || Mode.Equals(NoteInfo.SlideNoteEnd)) && Game.Dispensor.Notes[PreviousNoteIDs[0]].IsSlideGroupDead)
                Game.Judger.JudgeAsSilentMiss();
            base.InitPhase();
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
                    float cX = NotePath.GetStarlightX(StartPos.x, EndPos.x, ProgressBy1);
                    float cY = NotePath.GetStarlightY(ProgressBy100);
                    CurrentPos = new Vector3(cX, cY, CurrentPos.z);
                    Body.anchoredPosition3D = CurrentPos;
                    Body.localScale = new Vector3(NotePath.GetStarlightScale(ProgressBy1), NotePath.GetStarlightScale(ProgressBy1), 1);
                    BeforePos = CurrentPos;
                }
                else if (Mode.Equals(NoteInfo.SlideNoteStart) && !Game.Paused && (IsHit || Game.Frame >= ReachFrame))
                {
                    Body.anchoredPosition3D += new Vector3(((Game.Dispensor.Notes[NextNoteID].EndPos.x - EndPos.x) / ((SlideFrameDistance + SlideFrameCalibrator) / 60)) * Time.deltaTime, 0);
                    if (IsHit)
                    {
                        FlickBorderLeft = Body.anchoredPosition3D.x - 98;
                        FlickBorderRight = Body.anchoredPosition3D.x + 98;
                    }
                }

                if (!Game.Paused)
                {
                    if (!Flick.Equals(FlickMode.None) && ConnectorToNext != null && ConnectorToNext.gameObject.activeSelf)
                    {
                        ConnectorToNext.StartPos = Body.anchoredPosition3D;
                        ConnectorToNext.StartScale = Body.localScale.x * 120;
                    }
                    if (!Flick.Equals(FlickMode.None) && ConnectorsFromPrevious.Count > 0)
                    {
                        for (int i = 0; i < ConnectorsFromPrevious.Count; i++)
                        {
                            if (ConnectorsFromPrevious[i] != null && !Game.Dispensor.Notes[ConnectorsFromPrevious[i].OwnerID].IsHit)
                            {
                                ConnectorsFromPrevious[i].EndPos = Body.anchoredPosition3D;
                                ConnectorsFromPrevious[i].EndScale = Body.localScale.x * 120;
                            }
                            if (ConnectorsFromPrevious[i] != null && Game.Dispensor.Notes[ConnectorsFromPrevious[i].OwnerID].IsHit) { ConnectorsFromPrevious[i].gameObject.SetActive(false); }
                        }
                    }
                    if (Mode.Equals(NoteInfo.LongNoteStart) && !IsHit)
                        TailToNext.SetStartPos(Body.anchoredPosition3D, ProgressBy1, SizeInPixel * (140f / 100));
                    if ((Mode.Equals(NoteInfo.SlideNoteStart) || Mode.Equals(NoteInfo.SlideNoteCheckpoint) && TailToNext != null))
                        TailToNext.SetStartPos(Body.anchoredPosition3D, (Mode.Equals(NoteInfo.SlideNoteStart) && Game.Frame >= ReachFrame) ? 1 : ProgressBy1, SizeInPixel * (140f / 100), (Game.Frame - LastSlideReachFrame) / (Game.Dispensor.Notes[NextNoteID].ReachFrame - LastSlideReachFrame));
                    if (TailsFromPrevious.Count > 0)
                    {
                        for (int i = 0; i < TailsFromPrevious.Count; i++)
                            if (TailsFromPrevious[i] != null)
                                TailsFromPrevious[i].SetEndPos(Body.anchoredPosition3D, CurrentFrame * Speed > 100 ? 1 : ProgressBy1, SizeInPixel * (140f / 100));
                    }
                }
            }
        }

        protected override void ProcessPhase()
        {
            base.ProcessPhase();

            if (Mode.Equals(NoteInfo.SlideNoteCheckpoint))
            {
                if (CurrentFrame  >= RunningFrames + 13 && !IsSlideCheckHitted)
                {
                    Game.Judger.JudgeAsMiss();
                    Erase();
                }
            }
            if (Mode.Equals(NoteInfo.SystemNoteXLStarter))
            {
                if (CurrentFrame * Speed >= 100)
                {
                    Game.SceneAnimate.Play("XLNote_Starlight5L"); 
                    Erase();
                }
            }
            if (Mode.Equals(NoteInfo.SystemNoteXLEnder))
            {
                if (CurrentFrame * Speed >= 100)
                {
                    Game.SceneAnimate.Play("XLNote2_Starlight5L"); 
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
                        Hit(EndPos, 0);
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
                                if (Input.GetTouch(i).fingerId.Equals(Game.Dispensor.Notes[PreviousNoteIDs[0]].TouchedFinger))
                                {
                                    if (point.x >= FlickBorderLeft && point.x < FlickBorderRight) { Hit(point, Input.GetTouch(i).fingerId); }
                                    else
                                    {
                                        Game.Judger.JudgeAsMiss();
                                        if (Mode.Equals(NoteInfo.LongNoteEnd)) { Game.Dispensor.Notes[TailsFromPrevious[0].OwnerID].Erase(); }
                                        Erase();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Input.touchCount; i++)
                        {
                            if (Mode.Equals(NoteInfo.LongNoteEnd) && Input.GetTouch(i).fingerId.Equals(Game.Dispensor.Notes[PreviousNoteIDs[0]].TouchedFinger) && Input.GetTouch(i).phase.Equals(TouchPhase.Ended))
                            {
                                Game.Judger.JudgeAsMiss();
                                Game.Dispensor.Notes[PreviousTailID].Erase();
                                Erase();
                            }
                        }
                    }
                }
                if ((Mode.Equals(NoteInfo.LongNoteStart) || Mode.Equals(NoteInfo.SlideNoteStart)) && IsHit)
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        if ((Input.GetTouch(i).phase.Equals(TouchPhase.Began) || Input.GetTouch(i).phase.Equals(TouchPhase.Stationary) || Input.GetTouch(i).phase.Equals(TouchPhase.Moved)))
                        {
                            if (!IsSlideHolding) { IsSlideHolding = true; }
                        }
                        if (Input.GetTouch(i).fingerId.Equals(TouchedFinger) && Input.GetTouch(i).phase.Equals(TouchPhase.Ended))
                        {
                            if (Mode.Equals(NoteInfo.LongNoteStart) && !Game.Dispensor.Notes[NextNoteID].IsAppeared)
                            {
                                Game.Judger.JudgeAsMiss();
                                Game.Judger.HoldNoteDead[EndLine] = true;
                                Erase();
                            }
                            else if (Mode.Equals(NoteInfo.SlideNoteStart))
                            {
                                if (Game.Dispensor.Notes[NextNoteID].Mode.Equals(NoteInfo.SlideNoteCheckpoint) || (Game.Dispensor.Notes[NextNoteID].Mode.Equals(NoteInfo.SlideNoteEnd) && !Game.Dispensor.Notes[NextNoteID].IsAppeared))
                                {
                                    IsSlideGroupDead = true;
                                    Game.Judger.JudgeAsFakeMiss();
                                    Erase();
                                }
                            }
                        }
                    }
                }
                if (Mode.Equals(NoteInfo.SlideNoteCheckpoint) && Game.Dispensor.Notes[PreviousTailID].Mode.Equals(NoteInfo.SlideNoteStart) && !IsHit && CurrentFrame >= RunningFrames - 6f)
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        if (Input.GetTouch(i).fingerId.Equals(Game.Dispensor.Notes[PreviousTailID].TouchedFinger))
                        {
                            Vector3 point = SyncPosition(Input.GetTouch(i).position);
                            if (point.x >= FlickBorderLeft && point.x < FlickBorderRight)
                            {
                                IsSlideCheckHitted = true;
                            }
                        }
                    }
                    if (CurrentFrame >= RunningFrames - 0.5f)
                    {
                        if (TailToNext != null) { TransferTail(); }
                        if (IsSlideCheckHitted && TailToNext == null) { Hit(EndPos, Game.Dispensor.Notes[PreviousTailID].TouchedFinger); }
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
                        if (Mode.Equals(NoteInfo.LongNoteStart))
                        {
                            Game.Judger.JudgeAsMiss();
                            Game.Judger.HoldNoteDead[EndLine] = true;
                            Erase();
                        }
                        else if (Mode.Equals(NoteInfo.SlideNoteStart))
                        {
                            IsSlideGroupDead = true;
                            Game.Judger.JudgeAsFakeMiss();
                            Erase();
                        }
                    }
                }
            }
        }
    }
}
