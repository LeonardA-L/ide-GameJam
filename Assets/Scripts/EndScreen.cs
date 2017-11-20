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
        // Use this for initialization
        void Start()
        {
            background.SetActive(false);
            lost.SetActive(false);
            win.SetActive(false);
        }

        public void GameOver()
        {
            background.SetActive(true);
            lost.SetActive(true);
            win.SetActive(false);
        }

        public void Victory()
        {
            background.SetActive(true);
            lost.SetActive(false);
            win.SetActive(true);
        }
    }
}