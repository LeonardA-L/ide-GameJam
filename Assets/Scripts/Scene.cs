using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MarsFrenzy
{
    public class Scene : MonoBehaviour
    {
        private static int menuSceneIndex = 0;
        private static int gameSceneIndex = 1;

        IEnumerator LoadSceneAsync(int _sceneIndex)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneIndex);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        public void LaunchGameScene()
        {
            StartCoroutine(LoadSceneAsync(gameSceneIndex));
        }

        public void LaunchMenuScene()
        {
            StartCoroutine(LoadSceneAsync(menuSceneIndex));
        }

    }
}