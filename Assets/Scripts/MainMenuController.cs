using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MarsFrenzy
{
    public class MainMenuController : MonoBehaviour
    {
        private static I18n i18n;
        private static int gameSceneIndex = 1;
        private Animator uiAnimator;


        // Translatable UI elements
        public Text uiStartGame;
        public Text uiOptions;
        public Text uiLanguagePrefs;

        // Use this for initialization
        void Start()
        {
            i18n = I18n.Instance;
            uiAnimator = GetComponent<Animator>();
            UpdateI18N();
        }

        IEnumerator LoadSceneAsync()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneIndex);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        private void LaunchGameScene()
        {
            StartCoroutine(LoadSceneAsync());
        }

        public void StartGame()
        {
            LaunchGameScene();
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
        }
    }
}