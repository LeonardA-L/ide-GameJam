using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFrenzy
{
    [System.Serializable]
    public class GameDataModel
    {
        public List<ResourceModel> resources;
        public float gameClock = 1;
        public int clockSmoothing = 1;
        public int clockSubSmoothing = 1;
        public List<ModuleHealthThreshold> moduleHealthThresholds;
        public ResourceModel ductTape;
        public ResourceModel scrap;

        public float playerHungerStart = 100.0f;
        public float playerThirstStart = 100.0f;
        public float starvationDecay = 10.0f;
        public float playerRegen = 1.0f;

        public float upgradeCostResource;
        public float upgradeCostScrap;
        public float upgradeConsumptionFactor;
        public float upgradeEfficiencyFactor;

        public float stormDamage = 0.0f;
        public int stormDuration = 10;

        public List<Vector3> crateDropPoints;
    }
}