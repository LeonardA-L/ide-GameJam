using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IdleWorks;

namespace MarsFrenzy
{
    public class CharacterLife : MonoBehaviour
    {
        private GameManager gm = GameManager.Instance;
        private Generator food;
        private Generator water;
        private Generator hunger;
        private Generator thirst;
        private Generator regenHunger;
        private Generator regenThirst;

        public bool dead = false;

        // Use this for initialization
        void Start()
        {
            dead = false;

            var globalStorage = StorageManager.Instance.GetStorage(Constants.STORAGE_MAIN);

            food = globalStorage.GetGenerator(Constants.POTATO);
            water = globalStorage.GetGenerator(Constants.WATER);
            thirst = globalStorage.GetGenerator(Constants.THIRST);
            hunger = globalStorage.GetGenerator(Constants.HUNGER);
            regenHunger = globalStorage.GetGenerator(Constants.REGEN_HUNGER);
            regenThirst = globalStorage.GetGenerator(Constants.REGEN_THIRST);

            Debug.Assert(food != null);
            Debug.Assert(water != null);
            Debug.Assert(thirst != null);
            Debug.Assert(hunger != null);
            Debug.Assert(regenThirst != null);
            Debug.Assert(regenHunger != null);
        }

        // Update is called once per frame
        void Update()
        {
            if(!thirst.IsActive && water.Amount <= 0.0f)
            {
                thirst.SetActive(true);
                regenThirst.SetActive(false);
            }
            if(thirst.IsActive && water.Amount > 0.0f)
            {
                thirst.SetActive(false);
                regenThirst.SetActive(true);
            }

            if (!hunger.IsActive && food.Amount <= 0.0f)
            {
                hunger.SetActive(true);
                regenHunger.SetActive(false);
            }
            if (hunger.IsActive && food.Amount > 0.0f)
            {
                hunger.SetActive(false);
                regenHunger.SetActive(true);
            }

            if (Hunger <= 0.0f || Thirst <= 0.0f)
            {
                dead = true;
            }
        }
        /*
        public void Tick()
        {
            if(food.Amount <= 0.0f)
            {
                hunger -= starveDecay;
            } else
            {
                hunger += regen;
            }
            if (water.Amount <= 0.0f)
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
        }*/

        public double Thirst
        {
            get
            {
                return thirst.Amount;
            }
        }

        public double Hunger
        {
            get
            {
                return hunger.Amount;
            }
        }
    }
}