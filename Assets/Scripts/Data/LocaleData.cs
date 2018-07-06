using System;

namespace TempestWave.Data
{
    [Serializable]
    public class LocaleData
    {
        public string language;
        public LocaleItem[] items;
    }

    [Serializable]
    public class LocaleItem
    {
        public string key;
        public string value;
    }
}
