using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFrenzy
{
    public class EndScreen : MonoBehaviour
    {
        public GameObject background;
        public GameObject lost;
        public GameObject win;
        public GameObject pause;
        // Use this for initialization
        void Start()
        {
            background.SetActive(false);
            lost.SetActive(false);
            win.SetActive(false);
            pause.SetActive(false);
        }

        public void GameOver()
        {
            background.SetActive(true);
            lost.SetActive(true);
        }

        public void Victory()
        {
            background.SetActive(true);
            win.SetActive(true);
        }

        public void Pause()
        {
            background.SetActive(true);
            pause.SetActive(true);
        }
    }
}