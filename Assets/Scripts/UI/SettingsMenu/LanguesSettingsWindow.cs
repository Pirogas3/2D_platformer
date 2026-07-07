using SheetXExample;
using UnityEngine;

namespace Assets.Scripts.UI.SettingsMenu
{
    public class LanguesSettingsWindow : AnimatedWindow
    {
        public void SetLanguageEnglish()
        {
            LocalizationsManager.CurrentLanguage = "english";
            PlayerPrefs.SetString("Language", "english");
            PlayerPrefs.Save();
        }

        public void SetLanguageRussian()
        {
            LocalizationsManager.CurrentLanguage = "russian";
            PlayerPrefs.SetString("Language", "russian");
            PlayerPrefs.Save();
        }
    }
}
