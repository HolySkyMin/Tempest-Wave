//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//namespace TempestWave.Ingame
//{
//    public class ObsoleteNote : MonoBehaviour
//    {
//        public int ID { get; set; }
//        public int TouchedFinger { get; set; }
//        public int PreviousSlideID { get; set; }
//        public int NextNoteID { get; set; }
//        public float StartLine { get; set; }
//        public float EndLine { get; set; }
//        public float SizeVal { get; set; }
//        public float SizeValPrev { get; set; }
//        public float SizeValNext { get; set; }
//        public float AppearFrame { get; set; }
//        public float ReachFrame { get; set; }
//        public float LastSlideAppearFrame { get; set; }
//        public float LastSlideReachFrame { get; set; }
//        public float MyFrameCount { get; set; }
//        public float Speed { get; set; }
//        public float StartEndDistance { get; set; }
//        public float FlickBorderLeft { get; set; }
//        public float FlickBorderRight { get; set; }
//        public float FlickThreshold { get; set; }
//        public float SlideFrameCalibrator { get; set; }
//        public float SlideFrameDistance { get; set; }
//        public bool IsAppeared { get; set; }
//        public bool IsHit { get; set; }
//        public bool IsSlideHolding { get; set; }
//        public bool IsSlideGroupDead { get; set; }
//        public Vector3 StartPos { get; set; }
//        public Vector3 EndPos { get; set; }
//        public Vector3 CurrentPos { get; set; }
//        public Vector3 BeforePos { get; set; }
//        public Color32 ColorKey { get; set; }
//        public RectTransform Body { get; set; }
//        public ImprovedTail TailToNext { get; set; }
//        public ImprovedConnector ConnectorToNext { get; set; }
//        public NoteInfo Mode { get; set; }
//        public FlickMode Flick { get; set; }
//        public GameManager Game { get; set; }
//        public List<int> PreviousNoteIDs { get; set; }
//        public List<ImprovedTail> TailsFromPrevious { get; set; }
//        public List<ImprovedConnector> ConnectorsFromPrevious { get; set; }

//        public Animator OwnAnimator;
//        private float DistanceToNext, FlickStartedPos;
//        private bool IsFlickBeingHitted, IsSlideCheckHitted, TailCreated;

//        public Note()
//        {
//            PreviousNoteIDs = new List<int>();
//            TailsFromPrevious = new List<ImprovedTail>();
//            TailToNext = null;
//            ConnectorsFromPrevious = new List<ImprovedConnector>();
//            ConnectorToNext = null;
//            TailCreated = false;
//        }

//        public override string ToString()
//        {
//            return "Note Object (ID: " + ID.ToString()  + ") - Mode: " + Mode.ToString() + ", Flick: " + Flick.ToString() + ", Appear Frame: " + AppearFrame.ToString();
//        }

//        /// <summary>
//        /// 출현 프레임을 비교값으로 사용합니다.
//        /// </summary>
//        /// <param name="target">비교할 노트입니다.</param>
//        /// <returns>결과값입니다.</returns>
//        public int CompareTo(Note target)
//        {
//            if (target == null) { return 1; }
//            else { return AppearFrame.CompareTo(target.AppearFrame); }
//        }

//        /// <summary>
//        /// 해당 노트의 속성을 설정합니다. (Game 속성에 값을 할당한 다음 호출해 주세요.)
//        /// </summary>
//        /// <param name="id">해당 노트의 고유 번호입니다.</param>
//        /// <param name="start">해당 노트가 출현하는 라인의 번호입니다.</param>
//        /// <param name="end">해당 노트가 도달하는 라인의 번호입니다.</param>
//        /// <param name="frame">해당 노트의 도달 프레임 좌표입니다.</param>
//        /// <param name="speed">해당 노트의 고유 배속입니다.</param>
//        /// <param name="threshold">해당 노트의 플릭 문턱값입니다.</param>
//        /// <param name="mode">해당 노트의 모드입니다.</param>
//        /// <param name="flick">해당 노트의 플릭 설정값입니다.</param>
//        /// <param name="color">해당 노트의 색상값입니다.</param>
//        /// <param name="previous">해당 노트와 연결되는 이전 노트들의 번호의 집합입니다.</param>
//        /// <param name="next">해당 노트와 연결되는 다음 노트들의 번호의 집합입니다.</param>
//        public void SetNote(int id, float start, float end, float frame, float speed, float threshold, NoteInfo mode, FlickMode flick, Color32 color, List<int> previous, int next)
//        {
//            ID = id;
//            StartLine = start;
//            EndLine = end;
//            ReachFrame = frame;
//            Speed = speed;
//            FlickThreshold = threshold;
//            Mode = mode;
//            Flick = flick;
//            ColorKey = color;
//            PreviousNoteIDs = previous;
//            NextNoteID = next;
//            Body = gameObject.GetComponent<RectTransform>();

//            SetNotePrecisely();
//            ResetValue();
//        }

//        private void SetNotePrecisely()
//        {
//            SizeVal = Body.sizeDelta.x;
//            SizeValPrev = SizeVal;
//            SizeValNext = SizeVal;
//            if(!Game.Mode.Equals(GameMode.Starlight) && !Flick.Equals(FlickMode.None)) { Body.sizeDelta = new Vector2(110, 110); }
//            gameObject.GetComponent<Image>().sprite = Game.GetNoteTexture(Mode, Flick);
//            if(SizeVal > 130) { gameObject.GetComponent<Image>().sprite = Instantiate(Game.IsNoteColored ? Game.WhiteTexture[9] : Game.NoteTexture[9]) as Sprite; }
//            if (Flick.Equals(FlickMode.Left))
//            {
//                Body.sizeDelta = new Vector2(Body.sizeDelta.y * 1.2f, Body.sizeDelta.y);
//                Body.pivot = new Vector2(7f / 12f, 0.5f);
//            }
//            else if (Flick.Equals(FlickMode.Right))
//            {
//                Body.sizeDelta = new Vector2(Body.sizeDelta.y * 1.2f, Body.sizeDelta.y);
//                Body.pivot = new Vector2(5f / 12f, 0.5f);
//            }
//            else if (Flick.Equals(FlickMode.Up))
//            {
//                Body.sizeDelta = new Vector2(Body.sizeDelta.x, Body.sizeDelta.x * 1.2f);
//                Body.pivot = new Vector2(0.5f, 5f / 12f);
//            }
//            else if (Flick.Equals(FlickMode.Down))
//            {
//                Body.sizeDelta = new Vector2(Body.sizeDelta.x, Body.sizeDelta.x * 1.2f);
//                Body.pivot = new Vector2(0.5f, 7f / 12f);
//            }
//            if (Game.IsNoteColored) { gameObject.GetComponent<Image>().color = ColorKey; }
//            if (gameObject.GetComponent<Image>().sprite == null) { gameObject.GetComponent<Image>().color = Color.clear; }
//            //if (Mode.Equals(NoteInfo.SlideNoteCheckpoint)) { gameObject.GetComponent<Image>().color = Color.clear; }

//            if(Game.Mode.Equals(GameMode.Starlight))
//            {
//                StartPos = new Vector3(-432 + 144 * StartLine, 212, 0);
//                EndPos = new Vector3(-588 + 196 * EndLine, -238, 0);
//            }
//            else if(Game.Mode.Equals(GameMode.Theater))
//            {
//                StartPos = new Vector3(-307.22195f + 87.7777f * StartLine, 217, 0);
//                //StartPos = new Vector3(-553 + 158 * StartLine, -240, 0);
//                EndPos = new Vector3(-553 + 158 * EndLine, -240, 0);
//            }
//            else if(Game.Mode.Equals(GameMode.Theater4))
//            {
//                StartPos = new Vector3(-363.889f + 145.5556f * StartLine, 217, 0);
//                //StartPos = new Vector3(-655 + 262 * StartLine, -240, 0);
//                EndPos = new Vector3(-655 + 262 * EndLine, -240, 0);
//            }
//            else if(Game.Mode.Equals(GameMode.Theater2L))
//            {
//                StartPos = new Vector3(-396.6666f + 264.4444f * StartLine, 217, 0);
//                //StartPos = new Vector3(-714 + 476 * StartLine, -240, 0);
//                EndPos = new Vector3(-714 + 476 * EndLine, -240, 0);
//            }
//            else if (Game.Mode.Equals(GameMode.Theater2P))
//            {
//                StartPos = new Vector3(-256.5f + 171 * StartLine, 241, 0);
//                //StartPos = new Vector3(-567 + 378 * StartLine, -311, 0);
//                EndPos = new Vector3(-567 + 378 * EndLine, -311, 0);
//            }

//            if(Game.Mode.Equals(GameMode.Starlight))
//            {
//                FlickBorderLeft = EndPos.x - 98;
//                FlickBorderRight = EndPos.x + 98;
//            }
//            else
//            {
//                FlickBorderLeft = EndPos.x - 79;
//                FlickBorderRight = EndPos.x + 79;
//            }
//            Body.SetAsFirstSibling();
//            if(SizeVal > 130)
//            {
//                if(Game.Mode.Equals(GameMode.Starlight))
//                {
//                    StartPos = new Vector3(0, 212, 0);
//                    EndPos = new Vector3(0, -238, 0);
//                }
//                else if(Game.Mode.Equals(GameMode.Theater2P))
//                {
//                    StartPos = new Vector3(0, 241, 0);
//                    EndPos = new Vector3(0, -311, 0);
//                }
//                else
//                {
//                    StartPos = new Vector3(0, 217, 0);
//                    EndPos = new Vector3(0, -240, 0);
//                }
//                FlickBorderLeft = -100;
//                FlickBorderRight = 100;
//            }
//            StartEndDistance = 100 / Speed;
//            AppearFrame = ReachFrame - StartEndDistance;
//            if (Mode.Equals(NoteInfo.SystemNoteStarter)) { Game.Frame -= StartEndDistance; }
//        }

//        public void CreateTailConnector()
//        {
//            if (TailCreated) { return; }
//            if (NextNoteID > 0)
//            {
//                if (Mode.Equals(NoteInfo.LongNoteStart) || Mode.Equals(NoteInfo.SlideNoteStart) || (Mode.Equals(NoteInfo.SlideNoteCheckpoint) && (Game.Dispensor.Notes[NextNoteID].Mode.Equals(NoteInfo.SlideNoteCheckpoint) || Game.Dispensor.Notes[NextNoteID].Mode.Equals(NoteInfo.SlideNoteEnd))))
//                {
//                    //Debug.Log("Making tail...");
//                    GameObject MyTail = Instantiate(Game.BaseTail) as GameObject;
//                    MyTail.name = "t" + ID.ToString();
//                    TailToNext = MyTail.GetComponent<ImprovedTail>();
//                    TailToNext.Game = Game;
//                    if (Game.IsNoteColored) { TailToNext.Color = new Color32(ColorKey.r, ColorKey.g, ColorKey.b, Game.Mode.Equals(GameMode.Starlight) ? (byte)180 : ColorKey.a); }
//                    else
//                    {
//                        if (Game.Mode.Equals(GameMode.Starlight)) { TailToNext.Color = new Color32(255, 255, 255, 180); }
//                        else { TailToNext.Color = new Color32(255, 255, 0, 255); }
//                    }
//                    if (Mode.Equals(NoteInfo.LongNoteStart))
//                    {
//                        TailToNext.SetEndPos(Game.Dispensor.Notes[NextNoteID].StartPos, 0, Game.Mode.Equals(GameMode.Starlight) ? 0 : SizeVal * (140f / 100) * 1f / 2);
//                        TailToNext.SetLines(StartPos.x, EndPos.x, Game.Dispensor.Notes[NextNoteID].StartPos.x, Game.Dispensor.Notes[NextNoteID].EndPos.x, true);
//                    }
//                    else if (Mode.Equals(NoteInfo.SlideNoteStart) || Mode.Equals(NoteInfo.SlideNoteCheckpoint))
//                    {
//                        TailToNext.SetStartPos(StartPos, 0f, Game.Mode.Equals(GameMode.Starlight) ? 0 : SizeVal * (140f / 100) * 1f / 2);
//                        TailToNext.SetEndPos(Game.Mode.Equals(GameMode.Starlight) ? Game.Dispensor.Notes[NextNoteID].StartPos : StartPos, 0f, Game.Mode.Equals(GameMode.Starlight) ? 0 : SizeVal * (140f / 100) * 1f / 2);
//                        TailToNext.SetLines(StartPos.x, EndPos.x, Game.Dispensor.Notes[NextNoteID].StartPos.x, Game.Dispensor.Notes[NextNoteID].EndPos.x, false);
//                        LastSlideAppearFrame = AppearFrame;
//                        LastSlideReachFrame = ReachFrame;
//                    }
//                    TailToNext.OwnerID = ID;
//                    //Debug.Log("Tail made. ID: " + TailToNext.OwnerID.ToString());
//                    Game.Dispensor.Notes[NextNoteID].TailsFromPrevious.Add(TailToNext);
//                    Game.Dispensor.Notes[NextNoteID].PreviousSlideID = ID;
//                    if (Mode.Equals(NoteInfo.SlideNoteCheckpoint))
//                    {
//                        if (!Game.Mode.Equals(GameMode.Starlight)) { Game.Dispensor.VisibleNoteCount--; }
//                    }
//                }
//                if (!Flick.Equals(FlickMode.None))
//                {
//                    GameObject MyConnector = Instantiate(Game.BaseConnector) as GameObject;
//                    MyConnector.name = "c" + ID.ToString();
//                    ConnectorToNext = MyConnector.GetComponent<ImprovedConnector>();
//                    ConnectorToNext.OwnerID = ID;
//                    if (Game.IsNoteColored) { ConnectorToNext.MainRenderer.material.color = new Color32(ColorKey.r, ColorKey.g, ColorKey.b, 180); }
//                    ConnectorToNext.EndPos = Game.Dispensor.Notes[NextNoteID].StartPos;
//                    ConnectorToNext.EndScale = 0;
//                    Game.Dispensor.Notes[NextNoteID].ConnectorsFromPrevious.Add(ConnectorToNext);
//                }
//            }
//            TailCreated = true;
//        }

//        public void ResetValue()
//        {
//            TouchedFinger = 50;
//            MyFrameCount = 0;
//            IsAppeared = false;
//            IsHit = false;
//            IsSlideGroupDead = false;
//            IsFlickBeingHitted = false;
//            IsSlideCheckHitted = false;
//            CurrentPos = StartPos;
//            BeforePos = StartPos;
//        }

//        /// <summary>
//        /// 해당 노트가 활성화되었다는 신호를 담당하는 함수입니다.
//        /// </summary>
//        public void WakeupSignal()
//        {
//            InitPhase();
//            IsAppeared = true;
//            if (TailToNext != null) { TailToNext.gameObject.SetActive(true); }
//            if (ConnectorToNext != null) { ConnectorToNext.gameObject.SetActive(true); }
//            if (Mode.Equals(NoteInfo.SlideNoteCheckpoint) && TailToNext == null)
//            {
//                Mode = NoteInfo.SlideNoteEnd;
//                if (Game.IsNoteColored) { gameObject.GetComponent<Image>().color = ColorKey; }
//                else { gameObject.GetComponent<Image>().color = Color.white; }
//                gameObject.GetComponent<Image>().sprite = Game.GetNoteTexture(Mode, Flick);
//                if (Game.Mode.Equals(GameMode.Starlight))
//                {
//                    FlickBorderLeft = EndPos.x - (98 * 1.2f);
//                    FlickBorderRight = EndPos.x + (98 * 1.2f);
//                }
//                else
//                {
//                    FlickBorderLeft = EndPos.x - (79 * 1.1f);
//                    FlickBorderRight = EndPos.x + (79 * 1.1f);
//                }
//            }
//            if (Mode.Equals(NoteInfo.SlideNoteStart) || Mode.Equals(NoteInfo.SlideNoteCheckpoint))
//            {
//                SlideFrameDistance = Game.Dispensor.Notes[NextNoteID].AppearFrame - AppearFrame;
//            }
//        }

//        private void Update()
//        {
//            InputSubPhase();
//            if (!Game.Paused)
//            {
//                //Debug.Log("Note " + ID.ToString() + " looping... (Frame: " + Game.Frame.ToString() + ", MyFrame: " + (MyFrameCount * Speed).ToString() + ")");
//                InitPhase();
//                MovePhase();
//                ProcessPhase();
//                InputPhase();
//            }
            
//        }

//        private void InitPhase()
//        {
//            if(Mode.Equals(NoteInfo.LongNoteEnd) && Game.Judger.HoldNoteDead[EndLine])
//            {
//                Game.Judger.HoldNoteDead[EndLine] = false;
//                Erase();
//            }
//            if((Mode.Equals(NoteInfo.SlideNoteCheckpoint) || Mode.Equals(NoteInfo.SlideNoteEnd)) && Game.Dispensor.Notes[PreviousNoteIDs[0]].IsSlideGroupDead)
//            {
//                IsSlideGroupDead = true;
//                if (Game.Mode.Equals(GameMode.Starlight)) { Game.Judger.JudgeAsSilentMiss(); }
//                Erase();
//            }
//        }

//        private void MovePhase()
//        {
//            if(gameObject.activeSelf)
//            {
//                MyFrameCount = Game.Frame - AppearFrame;
//                float ProgressBy100 = MyFrameCount * Speed;
//                float ProgressBy1 = ProgressBy100 / 100;
//                if(!IsHit && (!Mode.Equals(NoteInfo.SlideNoteStart) || (Mode.Equals(NoteInfo.SlideNoteStart) && Game.Frame < ReachFrame)))
//                {
//                    if (Game.Mode.Equals(GameMode.Starlight))
//                    {
//                        float cX = NotePath.GetStarlightX(StartPos.x, EndPos.x, ProgressBy1);
//                        float cY = NotePath.GetStarlightY(ProgressBy100);
//                        CurrentPos = new Vector3(cX, cY, CurrentPos.z);
//                        Body.anchoredPosition3D = CurrentPos;
//                        Body.localScale = new Vector3(NotePath.GetStarlightScale(ProgressBy1), NotePath.GetStarlightScale(ProgressBy1), 1);
//                    }
//                    else if(Game.Mode.Equals(GameMode.Theater2P))
//                    {
//                        float cX = NotePath.GetTheaterX(StartPos.x, EndPos.x, ProgressBy1);
//                        float cY = NotePath.GetTheaterPortY(ProgressBy100);
//                        CurrentPos = new Vector3(cX, cY, CurrentPos.z);
//                        Body.anchoredPosition3D = CurrentPos;
//                        Body.localScale = new Vector3(NotePath.GetTheaterScale(ProgressBy1), NotePath.GetTheaterScale(ProgressBy1), 1);
//                    }
//                    else 
//                    {
//                        float cX = NotePath.GetTheaterX(StartPos.x, EndPos.x, ProgressBy1);
//                        float cY = NotePath.GetTheaterY(ProgressBy100);
//                        CurrentPos = new Vector3(cX, cY, CurrentPos.z);
//                        Body.anchoredPosition3D = CurrentPos;
//                        Body.localScale = new Vector3(NotePath.GetTheaterScale(ProgressBy1), NotePath.GetTheaterScale(ProgressBy1), 1);
//                    }
//                    BeforePos = CurrentPos;
//                }
//                else if(Mode.Equals(NoteInfo.SlideNoteStart) && !Game.Paused && (IsHit || Game.Frame >= ReachFrame))
//                {
//                    Body.anchoredPosition3D += new Vector3(((Game.Dispensor.Notes[NextNoteID].EndPos.x - EndPos.x) / ((SlideFrameDistance + SlideFrameCalibrator) / 60)) * Time.deltaTime, 0);
//                    if(IsHit)
//                    {
//                        FlickBorderLeft = Body.anchoredPosition3D.x - 79;
//                        FlickBorderRight = Body.anchoredPosition3D.x + 79;
//                    }
//                }

//                if(!Game.Paused)
//                {
//                    if(!Flick.Equals(FlickMode.None) && ConnectorToNext != null && ConnectorToNext.gameObject.activeSelf)
//                    {
//                        ConnectorToNext.StartPos = Body.anchoredPosition3D;
//                        ConnectorToNext.StartScale = Body.localScale.x * 120;
//                    }
//                    if(!Flick.Equals(FlickMode.None) && ConnectorsFromPrevious.Count > 0)
//                    {
//                        for(int i = 0; i < ConnectorsFromPrevious.Count; i++)
//                        {
//                            if(ConnectorsFromPrevious[i] != null && !Game.Dispensor.Notes[ConnectorsFromPrevious[i].OwnerID].IsHit)
//                            {
//                                ConnectorsFromPrevious[i].EndPos = Body.anchoredPosition3D;
//                                ConnectorsFromPrevious[i].EndScale = Body.localScale.x * 120;
//                            }
//                            if (ConnectorsFromPrevious[i] != null && Game.Dispensor.Notes[ConnectorsFromPrevious[i].OwnerID].IsHit) { ConnectorsFromPrevious[i].gameObject.SetActive(false); }
//                        }
//                    }
//                    if(Mode.Equals(NoteInfo.LongNoteStart) && !IsHit)
//                    {
//                        if (Game.Mode.Equals(GameMode.Starlight)) { TailToNext.SetStartPos(Body.anchoredPosition3D, ProgressBy1, SizeValNext * (140f / 100)); }
//                        else { TailToNext.SetStartPos(Body.anchoredPosition3D, ProgressBy1, SizeValNext * (140f / 100)); }
//                    }
//                    if((Mode.Equals(NoteInfo.SlideNoteStart) || Mode.Equals(NoteInfo.SlideNoteCheckpoint) && TailToNext != null))
//                    {
//                        if (Game.Mode.Equals(GameMode.Starlight)) { TailToNext.SetStartPos(Body.anchoredPosition3D, (Mode.Equals(NoteInfo.SlideNoteStart) && Game.Frame >= ReachFrame) ? 1 : ProgressBy1, SizeValNext * (140f / 100), (Game.Frame - LastSlideReachFrame) / (Game.Dispensor.Notes[NextNoteID].ReachFrame - LastSlideReachFrame)); }
//                        else
//                        {
//                            TailToNext.SetStartPos(Body.anchoredPosition3D, (Mode.Equals(NoteInfo.SlideNoteStart) && Game.Frame >= ReachFrame) ? 1 : ProgressBy1, SizeValNext * (140f / 100), (Game.Frame - LastSlideReachFrame) / (Game.Dispensor.Notes[NextNoteID].ReachFrame - LastSlideReachFrame));
//                            if (!Game.Dispensor.Notes[NextNoteID].IsAppeared)
//                            {
//                                TailToNext.SetDeltaEndPos((Game.Dispensor.Notes[NextNoteID].StartPos - StartPos) / (SlideFrameDistance + SlideFrameCalibrator), 0, SizeValNext * (140f / 100), (Game.Frame - LastSlideAppearFrame) / (Game.Dispensor.Notes[NextNoteID].AppearFrame - LastSlideAppearFrame));
//                            }
//                        }
//                    }
//                    if(TailsFromPrevious.Count > 0)
//                    {
//                        for(int i = 0; i < TailsFromPrevious.Count; i++)
//                        {
//                            if(TailsFromPrevious[i] != null)
//                            {
//                                if (Game.Mode.Equals(GameMode.Starlight)) { TailsFromPrevious[i].SetEndPos(Body.anchoredPosition3D, MyFrameCount * Speed > 100 ? 1 : ProgressBy1, SizeValPrev * (140f / 100)); }
//                                else { TailsFromPrevious[i].SetEndPos(Body.anchoredPosition3D, MyFrameCount * Speed > 100 ? 1 : ProgressBy1, SizeValPrev * (140f / 100)); }
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        private void ProcessPhase()
//        {
//            if((int)Mode < 10)
//            {
//                if(Mode.Equals(NoteInfo.DamageNote))
//                {
//                    if(MyFrameCount * Speed >= 100)
//                    {
//                        Game.Judger.HitJudge(Game.Mode, Mode, Flick, MyFrameCount, StartEndDistance, false);
//                        IsHit = true;
//                        Game.ShortEffectPlay(Mathf.RoundToInt(EndLine), false);
//                        Erase();
//                    }
//                }
//                else if (Mode.Equals(NoteInfo.SlideNoteCheckpoint))
//                {
//                    // Starlight와 Theater의 동작이 완전히 다름
//                    if(Game.Mode.Equals(GameMode.Starlight))
//                    {
//                        if(MyFrameCount >= StartEndDistance + 13 && !IsSlideCheckHitted)
//                        {
//                            Game.Judger.JudgeAsMiss();
//                            Erase();
//                        }
//                    }
//                    else
//                    {
//                        if (MyFrameCount >= StartEndDistance - 0.5f && TailToNext.gameObject.activeSelf && Game.Dispensor.Notes[PreviousSlideID].IsHit)
//                        {
//                            TransferTail();
//                            Erase();
//                        }
//                    }
//                }
//                else if((Mode.Equals(NoteInfo.LongNoteStart) || Mode.Equals(NoteInfo.SlideNoteStart)) && IsSlideHolding)
//                {
//                    if (!Game.Mode.Equals(GameMode.Starlight)) { Game.Judger.CalculateHoldScore(); }
//                    // 슬라이드가 끝까지 진행되었을 때 시작점은 자동으로 지워진다.
//                    if (Mode.Equals(NoteInfo.SlideNoteStart) && Game.Dispensor.Notes[NextNoteID].Mode.Equals(NoteInfo.SlideNoteEnd) && Game.Frame >= Game.Dispensor.Notes[NextNoteID].ReachFrame) { Erase(); }
//                }
//                else
//                {
//                    if (Mode.Equals(NoteInfo.HiddenNote) && MyFrameCount >= StartEndDistance + 4f) { Erase(); }
//                    if (MyFrameCount > StartEndDistance + 13 && !IsHit)
//                    {
//                        if(Mode.Equals(NoteInfo.LongNoteStart))
//                        {
//                            Game.Judger.HoldNoteDead[EndLine] = true;
//                            Game.Judger.JudgeAsMiss();
//                        }
//                        if(Mode.Equals(NoteInfo.LongNoteEnd))
//                        {
//                            for(int i = 0; i < TailsFromPrevious.Count; i++)
//                            {
//                                Game.Dispensor.Notes[TailsFromPrevious[i].OwnerID].Erase();
//                            }
//                        }
//                        if(Mode.Equals(NoteInfo.SlideNoteStart))
//                        {
//                            IsSlideGroupDead = true;
//                            Game.Judger.NoteQueue[EndLine].Remove(ID);
//                        }
//                        Game.Judger.JudgeAsMiss();
//                        Erase();
//                    }
//                }
//            }
//            else
//            {
//                if(Mode.Equals(NoteInfo.SystemNoteStarter))
//                {
//                    if(MyFrameCount * Speed >= 100)
//                    {
//                        Game.IsPlaying = true;
//                        Game.SongDelay = StartEndDistance;
//                        Game.MusicFile.Play();
//                        Erase();
//                    }
//                }
//                if(Mode.Equals(NoteInfo.SystemNoteEnder))
//                {
//                    if(MyFrameCount * Speed >= 100)
//                    {
//                        Game.IsPlaying = false;
//                        Game.IsEnded = true;
//                        Erase();
//                    }
//                }
//                if(Mode.Equals(NoteInfo.SystemNoteSpeeder))
//                {
//                    if(MyFrameCount * Speed >= 100)
//                    {
//                        Game.GlobalNoteSpeed = Game.SecuredNoteSpeed * Speed;
//                        Erase();
//                    }
//                }
//                if(Mode.Equals(NoteInfo.SystemNoteScroller))
//                {
//                    if(MyFrameCount * Speed >= 100)
//                    {
//                        Game.FrameSpeed = Speed;
//                        Erase();
//                    }
//                }
//                if(Mode.Equals(NoteInfo.SystemNoteXLStarter))
//                {
//                    if(MyFrameCount * Speed >= 100)
//                    {
//                        if (Game.Mode.Equals(GameMode.Theater)) { Game.SceneAnimate.Play("XLNote_Theater6L"); }
//                        else if (Game.Mode.Equals(GameMode.Theater4)) { Game.SceneAnimate.Play("XLNote_Theater4L"); }
//                        else if (Game.Mode.Equals(GameMode.Theater2L)) { Game.SceneAnimate.Play("XLNote_Theater2LP"); }
//                        else if (Game.Mode.Equals(GameMode.Theater2P)) { Game.SceneAnimate.Play("XLNote_Theater2P"); }
//                        else if (Game.Mode.Equals(GameMode.Starlight)) { Game.SceneAnimate.Play("XLNote_Starlight5L"); }
//                        Erase();
//                    }
//                }
//                if(Mode.Equals(NoteInfo.SystemNoteXLEnder))
//                {
//                    if(MyFrameCount * Speed >= 100)
//                    {
//                        if (Game.Mode.Equals(GameMode.Theater)) { Game.SceneAnimate.Play("XLNote2_Theater6L"); }
//                        else if (Game.Mode.Equals(GameMode.Theater4)) { Game.SceneAnimate.Play("XLNote2_Theater4L"); }
//                        else if (Game.Mode.Equals(GameMode.Theater2L)) { Game.SceneAnimate.Play("XLNote2_Theater2LP"); }
//                        else if (Game.Mode.Equals(GameMode.Theater2P)) { Game.SceneAnimate.Play("XLNote2_Theater2P"); }
//                        else if (Game.Mode.Equals(GameMode.Starlight)) { Game.SceneAnimate.Play("XLNote2_Starlight5L"); }
//                        Erase();
//                    }
//                }
//                if(Mode.Equals(NoteInfo.SystemNoteSlideDummy))
//                {
//                    if (Game.Dispensor.Notes[ID].IsHit) { Erase(); }
//                    if (MyFrameCount >= StartEndDistance + 13f) { Erase(); }
//                }
//            }
//        }

//        private void InputPhase()
//        {
//            if(Game.IsAutoPlay)
//            {
//                if(Game.IsStarted.Equals(true) && (int)Mode < 6 && MyFrameCount >= StartEndDistance - 0.5f && !IsHit)
//                {
//                    if (Mode.Equals(NoteInfo.SlideNoteCheckpoint))
//                    {
//                        TransferTail();
//                        if (Game.Mode.Equals(GameMode.Starlight)) { Hit(EndPos, 0); }
//                        else { Erase(); }
//                    }
//                    else { Hit(EndPos, 0); }
//                    if (Mode.Equals(NoteInfo.SlideNoteEnd)) { Game.Dispensor.Notes[TailsFromPrevious[0].OwnerID].Erase(); }
//                }
//            }
//            else
//            {
//                if ((int)Mode < 10 && !Mode.Equals(NoteInfo.SlideNoteCheckpoint) && Input.touchCount > 0 && ((Game.Judger.NoteQueue[EndLine].Count < 1 || Game.Judger.NoteQueue[EndLine][0].Equals(ID)) || (Mode.Equals(NoteInfo.LongNoteEnd) && Game.Dispensor.Notes[TailsFromPrevious[0].OwnerID].IsHit)))
//                {
//                    if (Flick.Equals(FlickMode.None))
//                    {
//                        for (int i = 0; i < Input.touchCount; i++)
//                        {
//                            Vector3 point = SyncPosition(Input.GetTouch(i).position);
//                            if (!(Mode.Equals(NoteInfo.LongNoteEnd) || Mode.Equals(NoteInfo.SlideNoteEnd)) && Input.GetTouch(i).phase.Equals(TouchPhase.Began) && !Game.Judger.TapHitted[EndLine]) { Hit(point, Input.GetTouch(i).fingerId); }
//                            else if ((Mode.Equals(NoteInfo.LongNoteEnd) || Mode.Equals(NoteInfo.SlideNoteEnd)) && Input.GetTouch(i).phase.Equals(TouchPhase.Ended))
//                            {
//                                if (Game.Mode.Equals(GameMode.Starlight))
//                                {
//                                    if (Input.GetTouch(i).fingerId.Equals(Game.Dispensor.Notes[PreviousNoteIDs[0]].TouchedFinger))
//                                    {
//                                        if (point.x >= FlickBorderLeft && point.x < FlickBorderRight) { Hit(point, Input.GetTouch(i).fingerId); }
//                                        else
//                                        {
//                                            Game.Judger.JudgeAsMiss();
//                                            if (Mode.Equals(NoteInfo.LongNoteEnd)) { Game.Dispensor.Notes[TailsFromPrevious[0].OwnerID].Erase(); }
//                                            Erase();
//                                        }
//                                    }
//                                }
//                                else { Hit(point, Input.GetTouch(i).fingerId); }
//                            }
//                        }
//                    }
//                    else
//                    {
//                        for (int i = 0; i < Input.touchCount; i++)
//                        {
//                            if (Input.GetTouch(i).phase.Equals(TouchPhase.Moved))
//                            {
//                                Vector3 point = SyncPosition(Input.GetTouch(i).position);
//                                Vector3 deltaPoint = point - SyncPosition(Input.GetTouch(i).position - Input.GetTouch(i).deltaPosition);
//                                if (Flick.Equals(FlickMode.Left))
//                                {
//                                    if (deltaPoint.x < 0) { FlickCheck(point, deltaPoint.x, Input.GetTouch(i).fingerId); }
//                                }
//                                else if (Flick.Equals(FlickMode.Right))
//                                {
//                                    if (deltaPoint.x > 0) { FlickCheck(point, deltaPoint.x, Input.GetTouch(i).fingerId); }
//                                }
//                                else if (Flick.Equals(FlickMode.Up))
//                                {
//                                    if (deltaPoint.y > 0) { FlickCheck(point, deltaPoint.y, Input.GetTouch(i).fingerId); }
//                                }
//                                else if (Flick.Equals(FlickMode.Down))
//                                {
//                                    if (deltaPoint.y < 0) { FlickCheck(point, deltaPoint.y, Input.GetTouch(i).fingerId); }
//                                }
//                            }
//                            if (Input.GetTouch(i).fingerId.Equals(TouchedFinger) && Input.GetTouch(i).phase.Equals(TouchPhase.Ended))
//                            {
//                                IsFlickBeingHitted = false;
//                                TouchedFinger = 100;
//                            }
//                            if(Game.Mode.Equals(GameMode.Starlight) && Mode.Equals(NoteInfo.LongNoteEnd) && Input.GetTouch(i).fingerId.Equals(Game.Dispensor.Notes[PreviousNoteIDs[0]].TouchedFinger) && Input.GetTouch(i).phase.Equals(TouchPhase.Ended))
//                            {
//                                Game.Judger.JudgeAsMiss();
//                                Game.Dispensor.Notes[PreviousSlideID].Erase();
//                                Erase();
//                            }
//                        }
//                    }
//                }
//                if ((Mode.Equals(NoteInfo.LongNoteStart) || Mode.Equals(NoteInfo.SlideNoteStart)) && IsHit)
//                {
//                    for (int i = 0; i < Input.touchCount; i++)
//                    {
//                        if ((Input.GetTouch(i).phase.Equals(TouchPhase.Began) || Input.GetTouch(i).phase.Equals(TouchPhase.Stationary) || Input.GetTouch(i).phase.Equals(TouchPhase.Moved)))
//                        {
//                            Vector3 point = SyncPosition(Input.GetTouch(i).position);
//                            if(Game.Mode.Equals(GameMode.Starlight))
//                            {
//                                if (!IsSlideHolding) { IsSlideHolding = true; }
//                            }
//                            else
//                            {
//                                if (point.x >= FlickBorderLeft && point.x < FlickBorderRight && !IsSlideHolding)
//                                {
//                                    IsSlideHolding = true;
//                                    OwnAnimator.enabled = true;
//                                    gameObject.transform.GetChild(0).gameObject.SetActive(true);
//                                    OwnAnimator.Play("HoldEffect");
//                                    TouchedFinger = Input.GetTouch(i).fingerId;
//                                }
//                                else if (Input.GetTouch(i).fingerId.Equals(TouchedFinger) && !(point.x >= FlickBorderLeft && point.x < FlickBorderRight) && IsSlideHolding)
//                                {
//                                    IsSlideHolding = false;
//                                    OwnAnimator.enabled = false;
//                                    gameObject.transform.GetChild(0).gameObject.SetActive(false);
//                                    //OwnAnimator.StopPlayback();
//                                    TouchedFinger = 100;
//                                }
//                            }
//                        }
//                        if (Input.GetTouch(i).fingerId.Equals(TouchedFinger) && Input.GetTouch(i).phase.Equals(TouchPhase.Ended))
//                        {
//                            if (Game.Mode.Equals(GameMode.Starlight))
//                            {
//                                if(Mode.Equals(NoteInfo.LongNoteStart) && !Game.Dispensor.Notes[NextNoteID].IsAppeared)
//                                {
//                                    Game.Judger.JudgeAsMiss();
//                                    Game.Judger.HoldNoteDead[EndLine] = true;
//                                    Erase();
//                                }
//                                else if(Mode.Equals(NoteInfo.SlideNoteStart))
//                                {
//                                    if(Game.Dispensor.Notes[NextNoteID].Mode.Equals(NoteInfo.SlideNoteCheckpoint) || (Game.Dispensor.Notes[NextNoteID].Mode.Equals(NoteInfo.SlideNoteEnd) && !Game.Dispensor.Notes[NextNoteID].IsAppeared))
//                                    {
//                                        IsSlideGroupDead = true;
//                                        Game.Judger.JudgeAsFakeMiss();
//                                        Erase();
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                IsSlideHolding = false;
//                                OwnAnimator.enabled = false;
//                                gameObject.transform.GetChild(0).gameObject.SetActive(false);
//                                //OwnAnimator.StopPlayback();
//                                TouchedFinger = 100;
//                            }
//                        }
//                    }
//                }
//                if(Game.Mode.Equals(GameMode.Starlight) && Mode.Equals(NoteInfo.SlideNoteCheckpoint) && Game.Dispensor.Notes[PreviousSlideID].Mode.Equals(NoteInfo.SlideNoteStart) && !IsHit && MyFrameCount >= StartEndDistance - 6f)
//                {
//                    for (int i = 0; i < Input.touchCount; i++)
//                    {
//                        if (Input.GetTouch(i).fingerId.Equals(Game.Dispensor.Notes[PreviousSlideID].TouchedFinger))
//                        {
//                            Vector3 point = SyncPosition(Input.GetTouch(i).position);
//                            if (point.x >= FlickBorderLeft && point.x < FlickBorderRight)
//                            {
//                                IsSlideCheckHitted = true;
//                            }
//                        }
//                    }
//                    if (MyFrameCount >= StartEndDistance - 0.5f)
//                    {
//                        if (TailToNext != null) { TransferTail(); }
//                        if (IsSlideCheckHitted && TailToNext == null) { Hit(EndPos, Game.Dispensor.Notes[PreviousSlideID].TouchedFinger); }
//                    }
//                }

//                // 키보드 입력
//                if((int)Mode < 6 && !(Mode.Equals(NoteInfo.LongNoteEnd) || Mode.Equals(NoteInfo.SlideNoteStart) || Mode.Equals(NoteInfo.SlideNoteCheckpoint) || Mode.Equals(NoteInfo.SlideNoteEnd)) && (Game.Judger.NoteQueue[EndLine].Count < 1 || Game.Judger.NoteQueue[EndLine][0].Equals(ID)))
//                {
//                    if (Input.GetKeyDown(Game.Keys[Mathf.RoundToInt(EndLine - 1)]) && !Game.Judger.TapHitted[EndLine]) { Hit(EndPos, 0); }
//                }
//                if(Mode.Equals(NoteInfo.LongNoteEnd) && Game.Dispensor.Notes[PreviousSlideID].IsHit)
//                {
//                    if(Input.GetKeyUp(Game.Keys[Mathf.RoundToInt(EndLine - 1)])) { Hit(EndPos, 0); }
//                }
//            }
//        }

//        private void InputSubPhase()
//        {
//            if(!Input.anyKey && !Game.IsAutoPlay && (Mode.Equals(NoteInfo.LongNoteStart) || Mode.Equals(NoteInfo.SlideNoteStart)) && IsHit)
//            {
//                bool fingerExists = false;
//                for(int i = 0; i < Input.touchCount; i++)
//                {
//                    if (Input.GetTouch(i).fingerId.Equals(TouchedFinger)) { fingerExists = true; }
//                }

//                if(!fingerExists)
//                {
//                    if (Game.Mode.Equals(GameMode.Starlight))
//                    {
//                        if(!Game.Paused)
//                        {
//                            if (Mode.Equals(NoteInfo.LongNoteStart))
//                            {
//                                Game.Judger.JudgeAsMiss();
//                                Game.Judger.HoldNoteDead[EndLine] = true;
//                                Erase();
//                            }
//                            else if (Mode.Equals(NoteInfo.SlideNoteStart))
//                            {
//                                IsSlideGroupDead = true;
//                                Game.Judger.JudgeAsFakeMiss();
//                                Erase();
//                            }
//                        }
//                    }
//                    else
//                    {
//                        if(!Game.Paused)
//                        {
//                            IsSlideHolding = false;
//                            OwnAnimator.enabled = false;
//                            gameObject.transform.GetChild(0).gameObject.SetActive(false);
//                            //OwnAnimator.StopPlayback();
//                            TouchedFinger = 100;
//                        }
//                    }
//                }
//            }
//        }

//        private Vector3 SyncPosition(Vector3 target)
//        {
//            Vector3 pos = new Vector3();
//            if (Game.Mode.Equals(GameMode.Theater2P))
//            {
//                if (Screen.height / (float)Screen.width >= 16f / 9) { pos.Set(((target.x - (Camera.main.pixelWidth / 2f)) / Camera.main.pixelWidth) * 720f, ((target.y - (Camera.main.pixelHeight / 2f)) / Camera.main.pixelHeight) * (720f * ((float)Camera.main.pixelHeight / Camera.main.pixelWidth)), target.z); }
//                else { pos.Set(((target.x - (Camera.main.pixelWidth / 2f)) / Camera.main.pixelWidth) * (1280f * ((float)Camera.main.pixelWidth / Camera.main.pixelHeight)), ((target.y - (Camera.main.pixelHeight / 2f)) / Camera.main.pixelHeight) * 1280f, target.z); }
//            }
//            else
//            {
//                if (Screen.width / (float)Screen.height >= 16f / 9) { pos.Set(((target.x - (Camera.main.pixelWidth / 2f)) / Camera.main.pixelWidth) * (720f * ((float)Camera.main.pixelWidth / Camera.main.pixelHeight)), ((target.y - (Camera.main.pixelHeight / 2f)) / Camera.main.pixelHeight) * 720f, target.z); }
//                else { pos.Set(((target.x - (Camera.main.pixelWidth / 2f)) / Camera.main.pixelWidth) * 1280f, ((target.y - (Camera.main.pixelHeight / 2f)) / Camera.main.pixelHeight) * (1280f * ((float)Camera.main.pixelHeight / Camera.main.pixelWidth)), target.z); }
//            }
//            return pos;
//        }

//        private void TransferTail()
//        {
//            Note prev = Game.Dispensor.Notes[TailsFromPrevious[0].OwnerID];
//            Game.Dispensor.Notes[NextNoteID].PreviousNoteIDs.Remove(ID);
//            Game.Dispensor.Notes[NextNoteID].PreviousNoteIDs.Add(TailsFromPrevious[0].OwnerID);
//            Game.Dispensor.Notes[NextNoteID].PreviousSlideID = PreviousSlideID;
//            TailToNext.OwnerID = TailsFromPrevious[0].OwnerID;
//            // send variables to slide note start.
//            prev.NextNoteID = NextNoteID;
//            prev.StartPos = StartPos;
//            prev.EndPos = EndPos;
//            //prev.SizeValPrev = SizeValPrev;
//            prev.SizeValNext = SizeValNext;
//            prev.SlideFrameCalibrator = 0;
//            prev.SlideFrameDistance = SlideFrameDistance;
//            prev.LastSlideAppearFrame = AppearFrame;
//            prev.LastSlideReachFrame = ReachFrame;
//            prev.TailToNext.gameObject.SetActive(false);
//            prev.Body.anchoredPosition3D = EndPos;
//            prev.TailToNext = TailToNext;
//            TailToNext = null;
//        }

//        private void Hit(Vector3 pos, int finger)
//        {
//            if((Mode.Equals(NoteInfo.LongNoteEnd) || Mode.Equals(NoteInfo.SlideNoteEnd)))
//            {
//                if (!Game.Dispensor.Notes[PreviousSlideID].IsHit) { return; }
//            }
//            if ((Mode.Equals(NoteInfo.SlideNoteCheckpoint) || Mode.Equals(NoteInfo.SlideNoteEnd)))
//            {
//                int Count = 0;
//                for (int i = 0; i < PreviousNoteIDs.Count; i++)
//                {
//                    if (Game.Dispensor.Notes[PreviousNoteIDs[i]].Mode.Equals(NoteInfo.SlideNoteStart)) { Count++; }
//                }
//                if (Count.Equals(0)) { return; }
//            }
//            if (pos.y < 0 && pos.x >= FlickBorderLeft && pos.x < FlickBorderRight && !IsHit)
//            {
//                if (Game.Judger.HitJudge(Game.Mode, Mode, Flick, MyFrameCount, StartEndDistance, true))
//                {
//                    TouchedFinger = finger;
//                    if (Flick.Equals(FlickMode.None)) { Game.Judger.TapHitted[EndLine] = true; }
//                    else { Game.Judger.SetFlickHitted(Flick, EndLine); }
//                    IsHit = true;
//                    if (!Mode.Equals(NoteInfo.SlideNoteStart))
//                    {
//                        gameObject.GetComponent<Image>().color = Color.clear;
//                    }
//                    Game.ShortEffectPlay(Mathf.RoundToInt(EndLine), SizeVal > 150 ? true : false);

//                    if (Mode.Equals(NoteInfo.LongNoteEnd)) { Game.Dispensor.Notes[TailsFromPrevious[0].OwnerID].Erase(); }
//                    if (Mode.Equals(NoteInfo.LongNoteStart))
//                    {
//                        Body.anchoredPosition3D = EndPos;
//                        TailToNext.SetStartPos(Body.anchoredPosition3D, 1, SizeVal * (140f / 100) * 1);
//                        IsSlideHolding = true;
//                        OwnAnimator.Play("HoldEffect");
//                    }
//                    else if (Mode.Equals(NoteInfo.SlideNoteStart))
//                    {
//                        Game.Judger.NoteQueue[EndLine].Remove(ID);
//                        IsSlideHolding = true;
//                        OwnAnimator.Play("HoldEffect");
//                        if (Game.Frame < ReachFrame)
//                        {
//                            Body.anchoredPosition3D = EndPos;
//                            SlideFrameCalibrator = StartEndDistance - MyFrameCount;
//                            LastSlideReachFrame = Game.Frame;
//                        }
//                    }
//                    else { Erase(); }
//                }
//            }
//        }

//        private void FlickCheck(Vector3 pos, float delta, int finger)
//        {
//            if(!IsFlickBeingHitted && MyFrameCount >= StartEndDistance - 13f && Game.Judger.FindFlickHitted(Flick, EndLine).Equals(false))
//            {
//                if(Flick.Equals(FlickMode.Left))
//                {
//                    if(pos.x <= FlickBorderRight && pos.x - delta > FlickBorderLeft) { IsFlickBeingHitted = true; FlickStartedPos = pos.x - delta; TouchedFinger = finger; }
//                }
//                if(Flick.Equals(FlickMode.Right))
//                {
//                    if(pos.x >= FlickBorderLeft && pos.x - delta < FlickBorderRight) { IsFlickBeingHitted = true; FlickStartedPos = pos.x - delta; TouchedFinger = finger; }
//                }
//                if(Flick.Equals(FlickMode.Up))
//                {
//                    if(pos.x >= FlickBorderLeft && pos.x < FlickBorderRight && pos.y < 0) { IsFlickBeingHitted = true; FlickStartedPos = pos.y - delta; TouchedFinger = finger; }
//                }
//                if (Flick.Equals(FlickMode.Down))
//                {
//                    if (pos.x >= FlickBorderLeft && pos.x < FlickBorderRight && pos.y < 0) { IsFlickBeingHitted = true; FlickStartedPos = pos.y - delta; TouchedFinger = finger; }
//                }
//            }
//            if (IsFlickBeingHitted && MyFrameCount >= StartEndDistance - 13f && finger.Equals(TouchedFinger) && Game.Judger.FindFlickHitted(Flick, EndLine).Equals(false))
//            {
//                if (Flick.Equals(FlickMode.Left))
//                {
//                    if (pos.x - FlickStartedPos < FlickThreshold * -1) { Hit(EndPos, TouchedFinger); }
//                }
//                if (Flick.Equals(FlickMode.Right))
//                {
//                    if (pos.x - FlickStartedPos > FlickThreshold) { Hit(EndPos, TouchedFinger); ; }
//                }
//                if (Flick.Equals(FlickMode.Up))
//                {
//                    if (pos.y - FlickStartedPos > FlickThreshold) { Hit(EndPos, TouchedFinger); ; }
//                }
//                if(Flick.Equals(FlickMode.Down))
//                {
//                    if (pos.y - FlickStartedPos < FlickThreshold * -1) { Hit(EndPos, TouchedFinger); ; }
//                }
//            }
//        }

//        /// <summary>
//        /// 노트를 게임에서 지웁니다. 이 노트가 주인인 꼬리와 연결선도 같이 지워집니다.
//        /// </summary>
//        public void Erase()
//        {
//            IsHit = true;
//            OwnAnimator.StopPlayback();
//            if ((int)Mode < 10 && !Mode.Equals(NoteInfo.SlideNoteStart)) { Game.Judger.NoteQueue[EndLine].Remove(ID); }
//            if (Mode.Equals(NoteInfo.SlideNoteEnd))
//            {
//                if (Game.Dispensor.Notes[TailsFromPrevious[0].OwnerID] != null) { Game.Dispensor.Notes[TailsFromPrevious[0].OwnerID].Erase(); }
//            }
//            if (TailToNext != null)
//            {
//                Game.Trashcan.Add(TailToNext.gameObject);
//                TailToNext.gameObject.SetActive(false);
//            }
//            if(ConnectorToNext != null)
//            {
//                Game.Trashcan.Add(ConnectorToNext.gameObject);
//                ConnectorToNext.gameObject.SetActive(false);
//            }
//            if(ConnectorsFromPrevious.Count > 0)
//            {
//                for(int i = 0; i < ConnectorsFromPrevious.Count; i++)
//                {
//                    if(ConnectorsFromPrevious[i] != null)
//                    {
//                        Game.Trashcan.Add(ConnectorsFromPrevious[i].gameObject);
//                        ConnectorsFromPrevious[i].gameObject.SetActive(false);
//                    }
//                }
//            }
//            Game.Trashcan.Add(gameObject);
//            gameObject.SetActive(false);
//        }
//    }
//}