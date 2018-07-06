using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TempestWave.Data;

namespace TempestWave.Core.UI
{
    public class MessageBoxButton : MonoBehaviour
    {
        public static readonly MessageBoxButtonType[] OK = new MessageBoxButtonType[] { MessageBoxButtonType.OK };
        public static readonly MessageBoxButtonType[] OKCancel = new MessageBoxButtonType[] { MessageBoxButtonType.OK, MessageBoxButtonType.Cancel };
        public static readonly MessageBoxButtonType[] YesNo = new MessageBoxButtonType[] { MessageBoxButtonType.Yes, MessageBoxButtonType.No };

        public MessageBox Parent { get; set; }
        public MessageBoxButtonType Type { get; set; }

        public Text ButtonText;

        public virtual void SetText()
        {
            if (Type.Equals(MessageBoxButtonType.OK)) { ButtonText.text = LocaleManager.instance.GetLocaleText("button_ok"); }
            else if (Type.Equals(MessageBoxButtonType.Cancel)) { ButtonText.text = LocaleManager.instance.GetLocaleText("button_cancel"); }
            else if (Type.Equals(MessageBoxButtonType.Yes)) { ButtonText.text = LocaleManager.instance.GetLocaleText("button_yes"); }
            else if (Type.Equals(MessageBoxButtonType.No)) { ButtonText.text = LocaleManager.instance.GetLocaleText("button_no"); }
        }
        
        public virtual void Clicked()
        {
            Parent.SetResult(Type);
        }
    }
}
