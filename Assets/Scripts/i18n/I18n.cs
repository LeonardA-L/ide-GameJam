using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFrenzy
{
    public class I18n : Mgl.I18n
    {
        protected static readonly new I18n instance = new I18n();
    
        I18n()
        {
            Debug.Log("Init i18n");

            string locale = PlayerPrefs.GetString("locale");
            if (locale == null || locale == "")
            {
                locale = "en-US";
                if(Application.systemLanguage == SystemLanguage.French)
                {
                    locale = "fr-FR";
                }
                Debug.Log("Locale null, defaulting to " + locale);
                UpdateLocale(locale);
            } else
            {
                Debug.Log("User locale preference: " + locale);
                SetLocale(locale);
            }
        }

        // Customize your languages here
        protected static new string[] locales = new string[] {
          "en-US",
          "fr-FR"
        };

        public static new I18n Instance
        {
            get
            {
                return instance;
            }
        }

        public void UpdateLocale(string _locale)
        {
            PlayerPrefs.SetString("locale", _locale);
            PlayerPrefs.Save();
            SetLocale(_locale);
        }
    }
}