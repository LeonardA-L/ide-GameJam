using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFrenzy
{
    public class EndScreen : MonoBehaviour
    {
        public GameObject background;
        public GameObject lost;
        public GameObject win;
        public GameObject pause;

        public Text winTitle;
        public Text winSub;
        public Text loseTitle;
        public Text loseSub;

        // Use this for initialization
        void Start()
        {
            Unpause();

        }

        private void SetTexts()
        {
            winTitle.text = I18n.Instance.__("victoryTitle");
            winSub.text = I18n.Instance.__("victorySub");
            loseTitle.text = I18n.Instance.__("defeatTitle");
            loseSub.text = I18n.Instance.__("defeatSub");
        }

        public void GameOver()
        {
            SetTexts();
            background.SetActive(true);
            lost.SetActive(true);
        }

        public void Victory()
        {
            SetTexts();
            background.SetActive(true);
            win.SetActive(true);
        }

        public void Pause()
        {
            SetTexts();
            background.SetActive(true);
            pause.SetActive(true);
        }

        public void Unpause()
        {
            SetTexts();
            background.SetActive(false);
            lost.SetActive(false);
            win.SetActive(false);
            pause.SetActive(false);
        }
    }
}