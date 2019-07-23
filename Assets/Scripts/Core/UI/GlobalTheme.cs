using System.Collections;
using System.Collections.Generic;
using TempestWave.Data;
using UnityEngine;

namespace TempestWave.Core.UI
{
    public class GlobalTheme
    {
        private static Color ThemeColorInnate(ThemeType theme, RuntimePlatform platform)
        {
            if (theme.Equals(ThemeType.Default))
            {
                if (platform.Equals(RuntimePlatform.WindowsPlayer)) { return new Color32(0, 120, 214, 255); }
                else if (platform.Equals(RuntimePlatform.Android)) { return new Color32(139, 195, 74, 255); }
                else if (platform.Equals(RuntimePlatform.IPhonePlayer)) { return new Color32(189, 189, 189, 255); }
                else { return new Color32(255, 255, 255, 255); }
            }
            else if (theme.Equals(ThemeType.Spring)) { return new Color32(253, 222, 217, 255); }
            else if (theme.Equals(ThemeType.Summer)) { return new Color32(2, 120, 0, 255); }
            else if (theme.Equals(ThemeType.Autumn)) { return new Color32(182, 83, 0, 255); }
            else if (theme.Equals(ThemeType.Winter)) { return new Color32(220, 230, 255, 255); }
            else if (theme.Equals(ThemeType.University)) { return new Color32(212, 205, 153, 255); }
            else if (theme.Equals(ThemeType.Princess)) { return new Color32(255, 78, 164, 255); }
            else if (theme.Equals(ThemeType.Fairy)) { return new Color32(4, 112, 255, 255); }
            else if (theme.Equals(ThemeType.Angel)) { return new Color32(255, 206, 1, 255); }
            else if (theme.Equals(ThemeType.GameConsole1)) { return new Color32(15, 51, 152, 255); }
            else if (theme.Equals(ThemeType.GameConsole2)) { return new Color32(228, 18, 23, 255); }
            else if (theme.Equals(ThemeType.GameConsole3)) { return new Color32(16, 124, 15, 255); }
            else { return new Color32(255, 255, 255, 255); }
        }

        public static Color ThemeColor()
        {
            ThemeType theme = ThemeType.Default;
            if (PlayerPrefs.HasKey("theme")) { theme = (ThemeType)PlayerPrefs.GetInt("theme"); }
            return ThemeColorInnate(theme, Application.platform);
        }

        public static Color ThemeColor(RuntimePlatform platform)
        {
            return ThemeColorInnate(ThemeType.Default, platform);
        }

        public static Color ThemeColor(int index)
        {
            ThemeType theme = (ThemeType)index;
            return ThemeColorInnate(theme, Application.platform);
        }

        private static Color ThemeContrastColorInnate(ThemeType theme, RuntimePlatform platform)
        {
            if (theme.Equals(ThemeType.Default))
            {
                if (platform.Equals(RuntimePlatform.WindowsPlayer)) { return new Color32(255, 255, 255, 255); }
                else if (platform.Equals(RuntimePlatform.Android)) { return new Color32(0, 0, 0, 255); }
                else if (platform.Equals(RuntimePlatform.IPhonePlayer)) { return new Color32(0, 0, 0, 255); }
                else { return new Color32(0, 0, 0, 255); }
            }
            else if (theme.Equals(ThemeType.Spring)) { return new Color32(0, 0, 0, 255); }
            else if (theme.Equals(ThemeType.Summer)) { return new Color32(180, 227, 245, 255); }
            else if (theme.Equals(ThemeType.Autumn)) { return new Color32(86, 50, 50, 255); }
            else if (theme.Equals(ThemeType.Winter)) { return new Color32(74, 74, 74, 255); }
            else if (theme.Equals(ThemeType.University)) { return new Color32(0, 57, 127, 255); }
            else if (theme.Equals(ThemeType.Princess)) { return new Color32(89, 27, 58, 255); }
            else if (theme.Equals(ThemeType.Fairy)) { return new Color32(1, 39, 89, 255); }
            else if (theme.Equals(ThemeType.Angel)) { return new Color32(89, 71, 0, 255); }
            else if (theme.Equals(ThemeType.GameConsole1)) { return new Color32(255, 255, 255, 255); }
            else if (theme.Equals(ThemeType.GameConsole2)) { return new Color32(255, 255, 255, 255); }
            else if (theme.Equals(ThemeType.GameConsole3)) { return new Color32(255, 255, 255, 255); }
            else { return new Color32(0, 0, 0, 255); }
        }

        public static Color ThemeContrastColor()
        {
            ThemeType theme = ThemeType.Default;
            if (PlayerPrefs.HasKey("theme")) { theme = (ThemeType)PlayerPrefs.GetInt("theme"); }
            return ThemeContrastColorInnate(theme, Application.platform);
        }

        public static Color ThemeContrastColor(RuntimePlatform platform)
        {
            return ThemeContrastColorInnate(ThemeType.Default, platform);
        }

        public static Color ThemeContrastColor(int index)
        {
            ThemeType theme = (ThemeType)index;
            return ThemeContrastColorInnate(theme, Application.platform);
        }

        public static string ThemeName(ThemeType theme)
        {
            if (theme.Equals(ThemeType.Default)) { return LocaleManager.instance.GetLocaleText("theme_default"); }
            else if (theme.Equals(ThemeType.Spring)) { return LocaleManager.instance.GetLocaleText("theme_spring"); }
            else if (theme.Equals(ThemeType.Summer)) { return LocaleManager.instance.GetLocaleText("theme_summer"); }
            else if (theme.Equals(ThemeType.Autumn)) { return LocaleManager.instance.GetLocaleText("theme_autumn"); }
            else if (theme.Equals(ThemeType.Winter)) { return LocaleManager.instance.GetLocaleText("theme_winter"); }
            else if (theme.Equals(ThemeType.University)) { return LocaleManager.instance.GetLocaleText("theme_university"); }
            else if (theme.Equals(ThemeType.Princess)) { return LocaleManager.instance.GetLocaleText("theme_princess"); }
            else if (theme.Equals(ThemeType.Fairy)) { return LocaleManager.instance.GetLocaleText("theme_fairy"); }
            else if (theme.Equals(ThemeType.Angel)) { return LocaleManager.instance.GetLocaleText("theme_angel"); }
            else if (theme.Equals(ThemeType.GameConsole1)) { return LocaleManager.instance.GetLocaleText("theme_gameconsole1"); }
            else if (theme.Equals(ThemeType.GameConsole2)) { return LocaleManager.instance.GetLocaleText("theme_gameconsole2"); }
            else if (theme.Equals(ThemeType.GameConsole3)) { return LocaleManager.instance.GetLocaleText("theme_gameconsole3"); }
            else { return "Unknown"; }
        }

        public static string Platform()
        {
            if (Application.platform.Equals(RuntimePlatform.WindowsPlayer)) { return "Windows PC"; }
            else if (Application.platform.Equals(RuntimePlatform.Android)) { return "Android"; }
            else if (Application.platform.Equals(RuntimePlatform.IPhonePlayer)) { return "iOS"; }
            else { return "Undefined"; }
        }
    }
}