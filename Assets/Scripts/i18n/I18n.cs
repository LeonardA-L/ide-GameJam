using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFrenzy
{
    public class I18n : Mgl.I18n
    {
        protected static readonly I18n instance = new I18n();

        // Customize your languages here
        protected static string[] locales = new string[] {
          "en-US",
          "fr-FR"
        };

        public static I18n Instance
        {
            get
            {
                return instance;
            }
        }
    }
}