using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TempestWave.Core.UI
{
    public class MessageBox : MonoBehaviour
    {
        public static MessageBox Instance { get; set; }

        /// <summary>
        /// 메시지 박스가 버튼을 입력받은 순간에 true로 바뀝니다. <see cref="CompletelyEnded"/> 보다 먼저 값이 true로 활성화됩니다.
        /// </summary>
        public bool ResultExists { get; set; }

        /// <summary>
        /// 메시지 박스의 모든 애니메이션이 끝나는 순간에 true로 바뀝니다. <see cref="ResultExists"/> 보다 늦게 값이 true로 활성화됩니다.
        /// </summary>
        public bool CompletelyEnded { get; set; }

        /// <summary>
        /// 메시지 박스가 이후에 비활성화되는지를 나타내는 변수입니다.
        /// </summary>
        public bool ShouldBeErased { get; set; }

        public MessageBoxButtonType Result { get; set; }
        public int LanguageIndex { get; set; }

        public GameObject Dimmer;
        public Image ColorBox;
        public Text TitleText;
        public Text BodyText;
        public Scrollbar BodyScrollBar;
        public MessageBoxButtonList ButtonList;
        public Animator MessageAnimator;

        public void Clear()
        {
            ResultExists = false;
            CompletelyEnded = false;
            ShouldBeErased = false;
            Result = MessageBoxButtonType.None;
            LanguageIndex = -1;
            ButtonList.Clear();
        }

        public void SetResult(MessageBoxButtonType result)
        {
            Result = result;
            ResultExists = true;
        }

        public static void SetInstance(MessageBox obj) { Instance = obj; }

        /// <summary>
        /// 메시지 박스를 호출합니다. 이 메시지 박스는 이후 반드시 비활성화됩니다.
        /// </summary>
        /// <param name="title">메시지 박스의 제목입니다.</param>
        /// <param name="body">메시지의 내용입니다.</param>
        /// <param name="buttons">메시지 박스에 포함될 버튼의 종류들입니다.</param>
        public static void Show(string title, string body, MessageBoxButtonType[] buttons)
        {
            Instance.Clear();

            Instance.Dimmer.SetActive(true);
            Instance.ColorBox.color = GlobalTheme.ThemeColor();
            Instance.TitleText.text = title;
            Instance.BodyText.text = body;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].Equals(MessageBoxButtonType.Language)) { Instance.ButtonList.AddLanguage(); }
                else { Instance.ButtonList.Add(buttons[i]); }
            }
            Instance.ShouldBeErased = true;
            Instance.gameObject.SetActive(true);
            Instance.MessageAnimator.Play("MessageBoxShow");
            Instance.StartCoroutine(WaitToResponse());
        }

        /// <summary>
        /// 이후의 존재 여부를 설정할 수 있는 메시지 박스를 호출합니다.
        /// </summary>
        /// <param name="title">메시지 박스의 제목입니다.</param>
        /// <param name="body">메시지의 내용입니다.</param>
        /// <param name="buttons">메시지 박스에 포함될 버튼의 종류들입니다.</param>
        /// <param name="shouldErase">false로 설정하면 연속적인 메시지 박스를 구현할 수 있습니다.</param>
        public static void Show(string title, string body, MessageBoxButtonType[] buttons, bool shouldErase)
        {
            Instance.Clear();

            Instance.Dimmer.SetActive(true);
            Instance.ColorBox.color = GlobalTheme.ThemeColor();
            Instance.TitleText.text = title;
            Instance.BodyText.text = body;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].Equals(MessageBoxButtonType.Language)) { Instance.ButtonList.AddLanguage(); }
                else { Instance.ButtonList.Add(buttons[i]); }
            }
            Instance.ShouldBeErased = shouldErase;
            Instance.gameObject.SetActive(true);
            Instance.MessageAnimator.Play("MessageBoxShow");
            Instance.StartCoroutine(WaitToResponse());
        }

        public static IEnumerator WaitToResponse()
        {
            yield return null;
            Instance.BodyScrollBar.value = 1;
            yield return new WaitUntil(() => Instance.ResultExists == true);
            Instance.MessageAnimator.Play("MessageBoxHide");
            yield return new WaitForSeconds(0.67f);
            Instance.CompletelyEnded = true;
            if(Instance.ShouldBeErased)
            {
                Instance.Dimmer.SetActive(false);
                Instance.gameObject.SetActive(false);
            }
        }
    }
}
