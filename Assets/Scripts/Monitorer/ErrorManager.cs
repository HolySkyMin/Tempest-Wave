using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TempestWave.Core.UI;
using TempestWave.Data;

namespace TempestWave.Monitorer
{
    public enum ErrorMode
    {
        Strange,
        NoSongsFolder, NoSong, SongInfoError, // 0번대 오류 : 선택 화면에서의 오류
        NoMusicFile = 11, NoNotemapFile, NoJsonFile, NoDeresimuFile, NotemapSyntaxError, WrongNotemap, NoKeyValue, NoSpeedValue, NoSyncValue,// 1번, 2번대 오류 : 게임 시작 준비 시의 오류
        NoKeySettingValue = 31, NoSpeedSettingValue, // 3번대 오류 : 설정 화면에서의 오류
    }

    public class ErrorManager : MonoBehaviour
    {
        public static void showErrorText(GameObject errText, ErrorMode errCode)
        {
            string errInfo;

            errText.SetActive(true);
            errInfo = GetErrorText(errCode);
            errInfo += System.Environment.NewLine;
            errInfo += LocaleManager.instance.GetLocaleText("error_errornum") + ((byte)errCode).ToString();
            errText.GetComponent<Text>().text = errInfo;
        }

        public static void showErrorText(GameObject errText, ErrorMode errCode, string AdditionalInfo)
        {
            string errInfo;

            errText.SetActive(true);
            errInfo = GetErrorText(errCode);
            errInfo += System.Environment.NewLine;
            errInfo += LocaleManager.instance.GetLocaleText("error_additional") + AdditionalInfo;
            errText.GetComponent<Text>().text = errInfo;
        }

        public static string GetErrorText(ErrorMode errCode)
        {
            string errInfo;
            switch (errCode)
            {
                case ErrorMode.NoSongsFolder: // 선곡 화면에서, 악곡 모음 폴더(Songs) 자체가 존재하지 않는 경우
                    errInfo = LocaleManager.instance.GetLocaleText("error_nofolder");
                    break;
                case ErrorMode.NoSong: // 선곡 화면에서, 악곡 모음 폴더 자체는 존재하나 개별 악곡의 폴더가 하나도 없는 경우
                    errInfo = LocaleManager.instance.GetLocaleText("error_nosongdata");
                    break;
                case ErrorMode.SongInfoError: // 선곡 화면에서, 악곡 모음 폴더 및 개별 악곡의 폴더가 존재하나 악곡 정보의 데이터가 없거나 문법이 이상한 경우
                    errInfo = LocaleManager.instance.GetLocaleText("error_infoerror");
                    break;
                case ErrorMode.NoMusicFile: // 게임을 시작하려 하나 음원이 없는 경우
                    errInfo = LocaleManager.instance.GetLocaleText("error_nomusic");
                    break;
                case ErrorMode.NoNotemapFile: // 게임을 시작하려 하나 노트맵이 없는 경우
                    errInfo = LocaleManager.instance.GetLocaleText("error_nonotemap");
                    break;
                case ErrorMode.NoJsonFile: // 게임을 시작하려 하나 노트맵이 없는 경우
                    errInfo = LocaleManager.instance.GetLocaleText("error_nojson");
                    break;
                case ErrorMode.NoDeresimuFile: // 게임을 시작하려 하나 노트맵이 없는 경우
                    errInfo = LocaleManager.instance.GetLocaleText("error_noderesimu");
                    break;
                case ErrorMode.NotemapSyntaxError: // 게임을 시작하려 하나 노트맵 데이터에 문법적 오류가 발견된 경우
                    errInfo = LocaleManager.instance.GetLocaleText("error_syntaxerror");
                    break;
                case ErrorMode.WrongNotemap: // 게임을 시작하려 하나 노트맵 데이터가 잘못되어 노트맵을 읽을 수 없는 경우
                    errInfo = LocaleManager.instance.GetLocaleText("error_wrongfile");
                    break;
                case ErrorMode.NoKeyValue: // 게임을 시작하려 하나 저장되어 있는 키 값이 일부(또는 전체) 없는 경우
                    errInfo = LocaleManager.instance.GetLocaleText("error_nokeyval");
                    break;
                case ErrorMode.NoSpeedValue: // 게임을 시작하려 하나 게임 속도가 설정되어 있지 않은 경우
                    errInfo = LocaleManager.instance.GetLocaleText("error_nogamespeed");
                    break;
                case ErrorMode.NoSyncValue: // 게임을 시작하려 하나 게임 싱크가 설정되어 있지 않은 경우
                    errInfo = LocaleManager.instance.GetLocaleText("error_nogamesync");
                    break;
                case ErrorMode.NoKeySettingValue: // 설정 화면에서, 키 세팅을 가져오려 하나 일부(또는 전체) 키 세팅이 존재하지 않는 경우
                    errInfo = LocaleManager.instance.GetLocaleText("error_nokeysetting");
                    break;
                case ErrorMode.NoSpeedSettingValue:
                    errInfo = LocaleManager.instance.GetLocaleText("error_nosettingval");
                    break;
                default: // 기타 알 수 없는 원인(데이터 손상 등)으로 인해 오류가 생긴 경우. 대부분 0(ErrorMode.Strange)을 던집니다.
                    errInfo = LocaleManager.instance.GetLocaleText("error_unknown");
                    break;
            }
            return errInfo;
        }
    }
}
