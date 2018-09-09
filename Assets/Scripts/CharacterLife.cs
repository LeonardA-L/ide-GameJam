using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFrenzy
{
    public class CharacterLife : MonoBehaviour
    {
        private GameManager gm = GameManager.Instance;
        private ResourceModel food;
        private ResourceModel water;

        public float hunger;
        public float thirst;
        public float starveDecay;
        public float regen;
        public bool dead = false;

        public Text hungerGauge;
        public Text thirstGauge;

        // Use this for initialization
        void Start()
        {
            dead = false;
        }

        // Update is called once per frame
        void Update()
        {
            hungerGauge.text = "" + hunger.ToString("0") + "%";
            thirstGauge.text = "" + thirst.ToString("0") + "%";
        }

        public void Init(GameManager _gm, float _hunger, float _thirst, float _starveDecay, float _regen)
        {
            gm = _gm;
            starveDecay = _starveDecay;
            hunger = _hunger;
            thirst = _thirst;
            regen = _regen;
            for (int i = 0; i < gm.GameState.resources.Count; i++)
            {
                if (gm.GameState.resources[i].name == "potatoes")
                {
                    food = gm.GameState.resources[i];
                }
                if (gm.GameState.resources[i].name == "water")
                {
                    water = gm.GameState.resources[i];
                }
            }
        }

        public void Tick()
        {
            if(food.amount <= 0.0f)
            {
                hunger -= starveDecay;
            } else
            {
                hunger += regen;
            }
            if (water.amount <= 0.0f)
            {
                thirst -= starveDecay;
            }
            else
            {
                thirst += regen;
            }

            if (hunger <= 0.0f || thirst <= 0.0f)
            {
                dead = true;
            }

            thirst = Mathf.Clamp(thirst, 0.0f, 100.0f);
            hunger = Mathf.Clamp(hunger, 0.0f, 100.0f);
        }
    }
}