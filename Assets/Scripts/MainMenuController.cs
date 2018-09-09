using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFrenzy
{
    public class MainMenuController : MonoBehaviour
    {
        private static I18n i18n;
        private Animator uiAnimator;
        public GameObject continueButton;


        // Translatable UI elements
        public Text uiStartGame;
        public Text uiOptions;
        public Text uiLanguagePrefs;
        public Text uiExit;
        public Text uiContinue;

        // Use this for initialization
        void Start()
        {
            i18n = I18n.Instance;
            uiAnimator = GetComponent<Animator>();
            UpdateI18N();

            string savePath = Application.persistentDataPath + GameManager.savePath;
            if (File.Exists(savePath))
            {
                continueButton.SetActive(true);
            }
        }

        public void ContinueGame()
        {
            PlayerPrefs.SetInt("shouldLoadGame", 1);
            PlayerPrefs.Save();

            GetComponent<Scene>().LaunchGameScene();
        }

        public void GotoOptions()
        {
            uiAnimator.SetBool("options", true);
        }

        public void GotoMenu()
        {
            uiAnimator.SetBool("options", false);
        }

        public void SetLocale(string _locale)
        {
            i18n.UpdateLocale(_locale);
            UpdateI18N();
        }

        public void UpdateI18N()
        {
            uiStartGame.text = i18n.__("MainMenuNewGame");
            uiOptions.text = i18n.__("MainMenuOptions");
            uiLanguagePrefs.text = i18n.__("MainMenuLanguage");
            uiExit.text = i18n.__("MainMenuExit");
            uiContinue.text = i18n.__("MainMenuContinue");
        }
    }
}