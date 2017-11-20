using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public bool dead = false;

        // Use this for initialization
        void Start()
        {
            dead = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Init(GameManager _gm, float _hunger, float _thirst, float _starveDecay)
        {
            gm = _gm;
            starveDecay = _starveDecay;
            hunger = _hunger;
            thirst = _thirst;
            for (int i = 0; i < gm.data.resources.Length; i++)
            {
                if (gm.data.resources[i].name == "potatoes")
                {
                    food = gm.data.resources[i];
                }
                if (gm.data.resources[i].name == "water")
                {
                    water = gm.data.resources[i];
                }
            }
        }

        public void Tick()
        {
            Debug.Log(food.amount + " " + water.amount);
            if(food.amount <= 0.0f)
            {
                hunger -= starveDecay;
            }
            if (water.amount <= 0.0f)
            {
                thirst -= starveDecay;
            }

            if(hunger <= 0.0f || thirst <= 0.0f)
            {
                dead = true;
            }
        }
    }
}