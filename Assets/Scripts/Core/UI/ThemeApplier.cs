using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TempestWave.Core.UI
{
    public class ThemeApplier : MonoBehaviour
    {
        public static List<ThemeApplier> Instances = new List<ThemeApplier>();

        public Image ImageObject;
        public Image ImageContrastObject;
        public Button Target;
        public Text TextWithTheme;
        public Text TextContrastTheme;

        private void Awake()
        {
            ApplyTheme();
        }

        public void ApplyTheme()
        {
            if (ImageObject != null)
            {
                ImageObject.color = GlobalTheme.ThemeColor();
            }
            if (ImageContrastObject != null) { ImageContrastObject.color = GlobalTheme.ThemeContrastColor(); }
            if (Target != null)
            {
                ColorBlock themedColors = Target.colors;
                themedColors.highlightedColor = GlobalTheme.ThemeColor();
                Target.colors = themedColors;
            }
            if (TextWithTheme != null) { TextWithTheme.color = GlobalTheme.ThemeColor(); }
            if (TextContrastTheme != null) { TextContrastTheme.color = GlobalTheme.ThemeContrastColor(); }

            Instances.Add(this);
        }

        public void ApplyTheme(ThemeType theme)
        {
            if (ImageObject != null)
            {
                ImageObject.color = GlobalTheme.ThemeColor((int)theme);
            }
            if (ImageContrastObject != null) { ImageContrastObject.color = GlobalTheme.ThemeContrastColor((int)theme); }
            if (Target != null)
            {
                ColorBlock themedColors = Target.colors;
                themedColors.highlightedColor = GlobalTheme.ThemeColor((int)theme);
                Target.colors = themedColors;
            }
            if (TextWithTheme != null) { TextWithTheme.color = GlobalTheme.ThemeColor((int)theme); }
            if (TextContrastTheme != null) { TextContrastTheme.color = GlobalTheme.ThemeContrastColor((int)theme); }
        }

        public static void UpdateTheme(ThemeType theme)
        {
            foreach (ThemeApplier applier in Instances)
                applier.ApplyTheme(theme);
        }
    }

}
