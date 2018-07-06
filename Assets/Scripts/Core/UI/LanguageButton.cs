using TempestWave.Data;

namespace TempestWave.Core.UI
{
    public class LanguageButton : MessageBoxButton
    {
        public int Index { get; set; }

        public override void SetText()
        {
            ButtonText.text = LocaleManager.instance.Locales[Index].language;
        }

        public override void Clicked()
        {
            Parent.LanguageIndex = Index;
            Parent.SetResult(Type);
        }
    }

}