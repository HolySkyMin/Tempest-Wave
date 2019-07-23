using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TempestWave.Data;

namespace TempestWave.Ingame
{
    public class ImprovedJudge : MonoBehaviour
    {
        public List<double> EndPoint { get; set; }
        public Dictionary<double, List<int>> NoteQueue { get; set; }
        public Dictionary<double, bool> TapHitted { get; set; }
        public Dictionary<double, bool> LeftHitted { get; set; }
        public Dictionary<double, bool> RightHitted { get; set; }
        public Dictionary<double, bool> UpHitted { get; set; }
        public Dictionary<double, bool> DownHitted { get; set; }
        public Dictionary<double, bool> HoldNoteDead { get; set; }
        public Dictionary<double, float> TapRecoverCount { get; set; }
        public Dictionary<double, float> LeftRecoverCount { get; set; }
        public Dictionary<double, float> RightRecoverCount { get; set; }
        public Dictionary<double, float> UpRecoverCount { get; set; }
        public Dictionary<double, float> DownRecoverCount { get; set; }

        public GameManager Game;
        public HitText DecisionText;
        public ComboText ComboValueText;
        public GameDataArchiver Data;
        public Text SongNameText, ScoreText, PercentText;
        public AudioSource TapSound, TapBadSound, FlickSound, StarlightTap, StarlightBad, StarlightFlick;

        private int CurCombo = 0;
        private float CurScore = 0, CurPercent = 0, SEVol;
        private float PerfectBorder, GreatBorder, NiceBorder, BadBorder, PerfectBorderL, GreatBorderL, NiceBorderL, BadBorderL;
        private bool AllowTempestic, HitSound;

        private void Awake()
        {
            EndPoint = new List<double>();
            NoteQueue = new Dictionary<double, List<int>>();
            TapHitted = new Dictionary<double, bool>();
            LeftHitted = new Dictionary<double, bool>();
            RightHitted = new Dictionary<double, bool>();
            UpHitted = new Dictionary<double, bool>();
            DownHitted = new Dictionary<double, bool>();
            HoldNoteDead = new Dictionary<double, bool>();
            TapRecoverCount = new Dictionary<double, float>();
            LeftRecoverCount = new Dictionary<double, float>();
            RightRecoverCount = new Dictionary<double, float>();
            UpRecoverCount = new Dictionary<double, float>();
            DownRecoverCount = new Dictionary<double, float>();
            SongNameText.text = DataSender.ReturnSongName();

            GameMode mode = DataSender.ReturnGameMode();
            if (PlayerPrefs.HasKey("hitsound") && PlayerPrefs.GetString("hitsound").Equals("false")) { HitSound = false; }
            else
            {
                HitSound = true;
                if (PlayerPrefs.HasKey("sevol"))
                {
                    SEVol = PlayerPrefs.GetFloat("sevol");
                    TapSound.volume = SEVol;
                    TapBadSound.volume = SEVol;
                    FlickSound.volume = SEVol;
                    if(mode.Equals(GameMode.Starlight))
                    {
                        StarlightTap.volume = SEVol;
                        StarlightBad.volume = SEVol;
                        StarlightFlick.volume = SEVol;
                    }
                }
                else
                {
                    TapSound.volume = 1f;
                    TapBadSound.volume = 1f;
                    FlickSound.volume = 1f;
                    if(mode.Equals(GameMode.Starlight))
                    {
                        StarlightTap.volume = 1f;
                        StarlightBad.volume = 1f;
                        StarlightFlick.volume = 1f;
                    }
                }
            }
            if (PlayerPrefs.HasKey("tempestic") && PlayerPrefs.GetString("tempestic").Equals("false")) { AllowTempestic = false; }
            else { AllowTempestic = true; }
            Data.SetHitSound(HitSound);
            Data.SetTempestic(AllowTempestic);

            if (mode.Equals(GameMode.Starlight))
            {
                PerfectBorder = 4; GreatBorder = 5.5f; NiceBorder = 7; BadBorder = 13;
                PerfectBorderL = 8; GreatBorderL = 9.5f; NiceBorderL = 11; BadBorderL = 13;
            }
            else if (mode.Equals(GameMode.Theater) || mode.Equals(GameMode.Theater4) || mode.Equals(GameMode.Theater2L) || mode.Equals(GameMode.Theater2P))
            {
                PerfectBorder = 2.5f; GreatBorder = 5; NiceBorder = 6; BadBorder = 10;
                PerfectBorderL = 4.5f; GreatBorderL = 7; NiceBorderL = 8; BadBorderL = 10;
            }
            else if(mode.Equals(GameMode.Platinum))
            {
                PerfectBorder = 4.5f; GreatBorder = 8; NiceBorder = 10.5f; BadBorder = 13;
                PerfectBorderL = 4.5f; GreatBorderL = 8; NiceBorderL = 10.5f; BadBorderL = 13;
            }
        }

        public void MakeNewKey(double value)
        {
            EndPoint.Add(value);
            NoteQueue.Add(value, new List<int>());
            TapHitted.Add(value, false);
            LeftHitted.Add(value, false);
            RightHitted.Add(value, false);
            UpHitted.Add(value, false);
            DownHitted.Add(value, false);
            TapRecoverCount.Add(value, 0);
            LeftRecoverCount.Add(value, 0);
            RightRecoverCount.Add(value, 0);
            UpRecoverCount.Add(value, 0);
            DownRecoverCount.Add(value, 0);
            HoldNoteDead.Add(value, false);
        }

        private void Update()
        {
            for(int i = 0; i < EndPoint.Count; i++)
            {
                if(TapHitted[EndPoint[i]])
                {
                    if(TapRecoverCount[EndPoint[i]] >= 1f)
                    {
                        TapHitted[EndPoint[i]] = false;
                        TapRecoverCount[EndPoint[i]] = 0f;
                    }
                    else { TapRecoverCount[EndPoint[i]] += 60 * Time.deltaTime; }
                }
                if(LeftHitted[EndPoint[i]])
                {
                    if(LeftRecoverCount[EndPoint[i]] >= 4f)
                    {
                        LeftHitted[EndPoint[i]] = false;
                        LeftRecoverCount[EndPoint[i]] = 0f;
                    }
                    else { LeftRecoverCount[EndPoint[i]] += 60 * Time.deltaTime; }
                }
                if (RightHitted[EndPoint[i]])
                {
                    if (RightRecoverCount[EndPoint[i]] >= 4f)
                    {
                        RightHitted[EndPoint[i]] = false;
                        RightRecoverCount[EndPoint[i]] = 0f;
                    }
                    else { RightRecoverCount[EndPoint[i]] += 60 * Time.deltaTime; }
                }
                if (UpHitted[EndPoint[i]])
                {
                    if (UpRecoverCount[EndPoint[i]] >= 4f)
                    {
                        UpHitted[EndPoint[i]] = false;
                        UpRecoverCount[EndPoint[i]] = 0f;
                    }
                    else { UpRecoverCount[EndPoint[i]] += 60 * Time.deltaTime; }
                }
                if (DownHitted[EndPoint[i]])
                {
                    if (DownRecoverCount[EndPoint[i]] >= 4f)
                    {
                        DownHitted[EndPoint[i]] = false;
                        DownRecoverCount[EndPoint[i]] = 0f;
                    }
                    else { DownRecoverCount[EndPoint[i]] += 60 * Time.deltaTime; }
                }
            }
        }

        /// <summary>
        /// 플릭 노트의 판정 쿨타임을 재설정하는 함수입니다.
        /// </summary>
        /// <param name="flick">해당 노트의 플릭 모드입니다.</param>
        /// <param name="pos">해당 노트의 도달 지점 값입니다.</param>
        public void SetFlickHitted(FlickMode flick, double pos)
        {
            if(flick.Equals(FlickMode.Left))
            {
                LeftHitted[pos] = true;
                RightHitted[pos] = false;
                RightRecoverCount[pos] = 0;
                UpHitted[pos] = false;
                UpRecoverCount[pos] = 0;
                DownHitted[pos] = false;
                DownRecoverCount[pos] = 0;
            }
            else if(flick.Equals(FlickMode.Right))
            {
                RightHitted[pos] = true;
                LeftHitted[pos] = false;
                LeftRecoverCount[pos] = 0;
                UpHitted[pos] = false;
                UpRecoverCount[pos] = 0;
                DownHitted[pos] = false;
                DownRecoverCount[pos] = 0;
            }
            else if(flick.Equals(FlickMode.Up))
            {
                UpHitted[pos] = true;
                LeftHitted[pos] = false;
                LeftRecoverCount[pos] = 0;
                RightHitted[pos] = false;
                RightRecoverCount[pos] = 0;
                DownHitted[pos] = false;
                DownRecoverCount[pos] = 0;
            }
            else if(flick.Equals(FlickMode.Down))
            {
                DownHitted[pos] = true;
                LeftHitted[pos] = false;
                LeftRecoverCount[pos] = 0;
                RightHitted[pos] = false;
                RightRecoverCount[pos] = 0;
                UpHitted[pos] = false;
                UpRecoverCount[pos] = 0;
            }
        }

        public bool FindFlickHitted(FlickMode flick, double pos)
        {
            if (flick.Equals(FlickMode.Left)) { return LeftHitted[pos]; }
            else if (flick.Equals(FlickMode.Right)) { return RightHitted[pos]; }
            else if (flick.Equals(FlickMode.Up)) { return UpHitted[pos]; }
            else if (flick.Equals(FlickMode.Down)) { return DownHitted[pos]; }
            else { return false; }
        }

        /// <summary>
        /// 게임의 판정을 총괄하는 함수입니다. 판정 절차가 이루어졌으면 true, 그렇지 않으면 false를 반환합니다.
        /// </summary>
        /// <param name="gMode">게임의 모드입니다.</param>
        /// <param name="nMode">판정될 노트의 모드입니다.</param>
        /// <param name="fMode">판정될 노트의 플릭 여부입니다.</param>
        /// <param name="MyFrame">판정될 노트의 현재 프레임 값입니다.</param>
        /// <param name="RefFrame">판정될 노트의 기준 프레임 값입니다.</param>
        /// <param name="HasInput">사용자의 입력에 의한 판정인지의 여부입니다.</param>
        /// <returns>판정이 진행되었는지의 여부입니다.</returns>
        public bool HitJudge(GameMode gMode, NoteInfo nMode, FlickMode fMode, float MyFrame, float RefFrame, bool HasInput)
        {
            if (nMode.Equals(NoteInfo.DamageNote))
            {
                if (HasInput)
                {
                    if (MyFrame < RefFrame - BadBorder) { return false; }
                    else { DecisionProcess(0, fMode); }
                }
                else { DecisionProcess(5, fMode); }
            }
            else if (fMode.Equals(0) && (nMode.Equals(NoteInfo.NormalNote) || nMode.Equals(NoteInfo.HiddenNote)))
            {
                if (MyFrame < RefFrame - BadBorder) { return false; }
                else if (MyFrame >= RefFrame - BadBorder && MyFrame < RefFrame - NiceBorder) { DecisionProcess(1, fMode); }
                else if (MyFrame >= RefFrame - NiceBorder && MyFrame < RefFrame - GreatBorder) { DecisionProcess(2, fMode); }
                else if (MyFrame >= RefFrame - GreatBorder && MyFrame < RefFrame - PerfectBorder) { DecisionProcess(3, fMode); }
                else if (MyFrame >= RefFrame - PerfectBorder && MyFrame < RefFrame - 0.5f) { DecisionProcess(4, fMode); }
                else if (MyFrame >= RefFrame - 0.5f && MyFrame <= RefFrame + 0.5f) { DecisionProcess(5, fMode); }
                else if (MyFrame > RefFrame + 0.5f && MyFrame <= RefFrame + PerfectBorder) { DecisionProcess(4, fMode); }
                else if (MyFrame > RefFrame + PerfectBorder && MyFrame <= RefFrame + GreatBorder)
                {
                    if (nMode.Equals(NoteInfo.HiddenNote)) { return false; }
                    else { DecisionProcess(3, fMode); }
                }
                else if (MyFrame > RefFrame + GreatBorder && MyFrame <= RefFrame + NiceBorder)
                {
                    if (nMode.Equals(NoteInfo.HiddenNote)) { return false; }
                    else { DecisionProcess(2, fMode); }
                }
                else if (MyFrame > RefFrame + NiceBorder && MyFrame <= RefFrame + BadBorder)
                {
                    if (nMode.Equals(NoteInfo.HiddenNote)) { return false; }
                    else { DecisionProcess(1, fMode); }
                }
                else if (MyFrame > RefFrame + BadBorder)
                {
                    if (nMode.Equals(NoteInfo.HiddenNote)) { return false; }
                    else { DecisionProcess(0, fMode); }
                }
            }
            else
            {
                if (MyFrame < RefFrame - BadBorderL)
                {
                    if (nMode.Equals(NoteInfo.LongNoteEnd) || nMode.Equals(NoteInfo.SlideNoteEnd)) { DecisionProcess(0, fMode); }
                    else { return false; }
                }
                else if (MyFrame >= RefFrame - BadBorderL && MyFrame < RefFrame - NiceBorderL) { DecisionProcess(1, fMode); }
                else if (MyFrame >= RefFrame - NiceBorderL && MyFrame < RefFrame - GreatBorderL) { DecisionProcess(2, fMode); }
                else if (MyFrame >= RefFrame - GreatBorderL && MyFrame < RefFrame - PerfectBorderL) { DecisionProcess(3, fMode); }
                else if (MyFrame >= RefFrame - PerfectBorderL && MyFrame < RefFrame - 0.5f) { DecisionProcess(4, fMode); }
                else if (MyFrame >= RefFrame - 0.5f && MyFrame <= RefFrame + 0.5f) { DecisionProcess(5, fMode); }
                else if (MyFrame > RefFrame + 0.5f && MyFrame <= RefFrame + PerfectBorderL) { DecisionProcess(4, fMode); }
                else if (MyFrame > RefFrame + PerfectBorderL && MyFrame <= RefFrame + GreatBorderL) { DecisionProcess(3, fMode); }
                else if (MyFrame > RefFrame + GreatBorderL && MyFrame <= RefFrame + NiceBorderL) { DecisionProcess(2, fMode); }
                else if (MyFrame > RefFrame + NiceBorderL && MyFrame <= RefFrame + BadBorderL) { DecisionProcess(1, fMode); }
                else if (MyFrame > RefFrame + BadBorderL) { DecisionProcess(0, fMode); }
            }
            return true;
        }

        /// <summary>
        /// 별도의 판정 절차 없이 바로 MISS로 처리하는 함수입니다.
        /// </summary>
        public void JudgeAsMiss()
        {
            DecisionProcess(0, FlickMode.None);
        }

        /// <summary>
        /// 콤보 등에 영향을 주지 않고, 표시도 없지만, 내부적으로 조용히 MISS 처리하는 함수입니다.
        /// </summary>
        public void JudgeAsSilentMiss()
        {
            Data.UpdateNote(0);
        }

        /// <summary>
        /// 별도의 판정 절차 없이 MISS 처리하지만, 실제 MISS 값은 가산되지 않습니다.
        /// </summary>
        public void JudgeAsFakeMiss()
        {
            CurCombo = 0;
            ComboValueText.Nuzzle();
            if (HitSound)
            {
                if (Game.Mode.Equals(GameMode.Starlight)) { StarlightBad.Play(); }
                else { TapBadSound.Play(); }
            }
            DecisionText.Wake(0);
        }

        private void DecisionProcess(int DecValue, FlickMode Flick)
        {
            int TweakedDec = DecValue;
            if (!AllowTempestic && DecValue.Equals(5)) { TweakedDec--; }

            if(TweakedDec < 3)
            {
                CurCombo = 0;
                ComboValueText.Nuzzle();
                if (HitSound)
                {
                    if (Game.Mode.Equals(GameMode.Starlight)) { StarlightBad.Play(); }
                    else { TapBadSound.Play(); }
                }
            }
            else
            {
                CurCombo += 1;
                ComboValueText.Wake(CurCombo);
                Data.TryUpdateCombo(CurCombo);
                if(HitSound)
                {
                    if (Flick.Equals(FlickMode.None))
                    {
                        if (Game.Mode.Equals(GameMode.Starlight)) { StarlightTap.Play(); }
                        else { TapSound.Play(); }
                    }
                    else
                    {
                        if (Game.Mode.Equals(GameMode.Starlight)) { StarlightFlick.Play(); }
                        else { FlickSound.Play(); }
                    }
                }
            }

            switch(TweakedDec)
            {
                case 0:
                    break;
                case 1:
                    CalculateScore(0.1f, ComboMultilpier());
                    break;
                case 2:
                    CalculateScore(0.3f, ComboMultilpier());
                    break;
                case 3:
                    CalculateScore(0.7f, ComboMultilpier());
                    break;
                case 4:
                    CalculateScore(1.0f, ComboMultilpier());
                    break;
                case 5:
                    CalculateScore(1.1f, ComboMultilpier());
                    break;
            }
            Data.UpdateNote(TweakedDec);
            DecisionText.Wake(TweakedDec);
        }

        private void CalculateScore(float Multiplier, float Combo)
        {
            if(Game.Mode.Equals(GameMode.Starlight) || Game.Mode.Equals(GameMode.Platinum))
            {
                float GotScore = 800f * Multiplier * Combo;
                CurScore += GotScore;
                CurPercent += (100f / Data.GetNoteCount()) * Multiplier;
                Data.SetMaxScore(Mathf.RoundToInt(CurScore));
                Data.SetMaxPercent(CurPercent);
                ScoreText.text = Mathf.RoundToInt(CurScore).ToString();
                PercentText.text = CurPercent.ToString("N2") + "%";
            }
            else
            {
                float GotScore = 800f * Multiplier * Combo;
                CurScore += GotScore;
                CurPercent += (100f / Data.GetNoteCount()) * Multiplier;
                Data.SetMaxScore(Mathf.RoundToInt(CurScore));
                Data.SetMaxPercent(CurPercent);
                ScoreText.text = Mathf.RoundToInt(CurScore).ToString();
                PercentText.text = CurPercent.ToString("N2") + "%";
            }
        }

        /// <summary>
        /// 홀드 계열 노트가 홀드 중일 때 점수를 계산하는 함수입니다.
        /// </summary>
        public void CalculateHoldScore()
        {
            float GotScore = 80f / 6f;
            CurScore += GotScore;
            Data.SetMaxScore(Mathf.RoundToInt(CurScore));
            ScoreText.text = Mathf.RoundToInt(CurScore).ToString();
        }

        private float ComboMultilpier()
        {
            float vi = (float)CurCombo / Data.GetNoteCount();
            if (vi < 0.05f) { return 1.0f; }
            else if (vi >= 0.05f && vi < 0.1f) { return 1.1f; }
            else if (vi >= 0.1f && vi < 0.25f) { return 1.2f; }
            else if (vi >= 0.25f && vi < 0.5f) { return 1.3f; }
            else if (vi >= 0.5f && vi < 0.7f) { return 1.4f; }
            else if (vi >= 0.7f && vi < 0.8f) { return 1.5f; }
            else if (vi >= 0.8f && vi < 0.9f) { return 1.7f; }
            else { return 2.0f; }
        }
    }
}
