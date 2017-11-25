using System;
using UnityEngine;

namespace MarsFrenzy
{
    [System.Serializable]
    public class GameDataModel
    {
        public ResourceModel[] resources;
        public int gameClock = 1;
        public int clockSmoothing = 1;
        public ModuleHealthThreshold[] moduleHealthThresholds;
        public ResourceModel ductTape;
        public ResourceModel scrap;

        public float playerHungerStart = 100.0f;
        public float playerThirstStart = 100.0f;
        public float starvationDecay = 10.0f;

        public float upgradeCostResource;
        public float upgradeCostScrap;
        public float upgradeConsumptionFactor;
        public float upgradeEfficiencyFactor;

        public Vector3[] crateDropPoints;
    }
}