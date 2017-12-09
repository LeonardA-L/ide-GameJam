using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFrenzy
{
    [System.Serializable]
    public class SaveModel
    {
        // Game state
        public bool timeRuns;
        public float timer;
        public float lastTime;
        public float lastSmoothTime;
        public float lastDialog = 0;
        public int onboardingStep = 0;

        public float playerX;
        public float playerY;
        public float playerZ;

        // Resources
        public float waterAmount;
        public float potatoesAmount;
        public float electricityAmount;
        public float ductTapeAmount;
        public float scrapAmount;

        // Modules
        public float waterHealth;
        public float waterLevel;
        public bool waterActive;
        public float potatoesHealth;
        public float potatoesLevel;
        public bool potatoesActive;
        public float electricityHealth;
        public float electricityLevel;
        public bool electricityActive;

        public List<bool> eventsFlags;
    }
}