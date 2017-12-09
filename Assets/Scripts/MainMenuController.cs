using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MarsFrenzy
{
    public class MainMenuController : MonoBehaviour
    {
        private static int gameSceneIndex = 1;
        private Animator uiAnimator;

        // Use this for initialization
        void Start()
        {
            uiAnimator = GetComponent<Animator>();
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
    }
}