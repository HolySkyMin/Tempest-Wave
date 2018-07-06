using UnityEngine;
using UnityEngine.UI;

namespace TempestWave.Core.UI
{
    public class ThemeShower : MonoBehaviour
    {
        public Image ImageObject;
        public Image ImageContrastObject;
        public ThemeType Theme;

        private void Awake()
        {
            ImageObject.color = GlobalTheme.ThemeColor((int)Theme);
            ImageContrastObject.color = GlobalTheme.ThemeContrastColor((int)Theme);
        }
    }
}
