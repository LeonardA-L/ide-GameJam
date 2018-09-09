using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFrenzy
{
    public class PopulateData
    {
        public static void Init(GameState gameState)
        {
            //------------ Don't touch
            gameState.moduleHealthThresholds = new List<ModuleHealthThreshold>();
            gameState.resources = new List<ResourceModel>();
            //------------




            // Generic data
            gameState.gameClock = 2.0f;
            gameState.clockSmoothing = 2;
            gameState.clockSubSmoothing = 10;

            gameState.playerHungerStart = 100.0f;
            gameState.playerRegen = 0.01f;
            gameState.playerThirstStart = 100.0f;
            gameState.starvationDecay = 5.0f;

            gameState.stormDamage = 1.5f;
            gameState.stormDuration = 60;

            // Upgrades
            gameState.upgradeConsumptionFactor = 0.9f;
            gameState.upgradeEfficiencyFactor = 1.5f;

            // DuctTape
            gameState.ductTape = new ResourceModel()
            {
                name = "ductTape",
                amount = 50.0f,
                efficiency = 5.0f
            };

            // Scrap
            gameState.scrap = new ResourceModel()
            {
                name = "scrap",
                amount = 75.0f
            };





            // Module Health Thresholds
            gameState.moduleHealthThresholds.Add(new ModuleHealthThreshold(20.0f, 0.3f));
            gameState.moduleHealthThresholds.Add(new ModuleHealthThreshold(30.0f, 0.5f));
            gameState.moduleHealthThresholds.Add(new ModuleHealthThreshold(60.0f, 0.7f));
            gameState.moduleHealthThresholds.Add(new ModuleHealthThreshold(70.0f, 0.9f));
            gameState.moduleHealthThresholds.Add(new ModuleHealthThreshold(80.0f, 1.1f));
            gameState.moduleHealthThresholds.Add(new ModuleHealthThreshold(90.0f, 1.2f));
            gameState.moduleHealthThresholds.Add(new ModuleHealthThreshold(100.0f, 1.3f));





            // Resources
            ResourceModel water = new ResourceModel()
            {
                name = "water",
                amount = 20.0f,
                decay = 0.7f,
                efficiency = 1.8f,
                damageRate = 0.3f,
                startHealth = 100.0f,
                playerTarget = new Vector3(0.8f, 0.0f, -1.4f),
                upgradeResCostRatio = 1.5f,
                upgradeScrapCostRatio = 1.1f,
                upgradeResCostStarter = 50.0f,
                upgradeScrapCostStarter = 50.0f
            };

            ResourceModel potatoes = new ResourceModel()
            {
                name = "potatoes",
                amount = 15.0f,
                decay = 0.6f,
                efficiency = 1.9f,
                damageRate = 0.5f,
                startHealth = 100.0f,
                playerTarget = new Vector3(4.0f, 0.0f, -2.0f),
                upgradeResCostRatio = 1.5f,
                upgradeScrapCostRatio = 1.1f,
                upgradeResCostStarter = 50.0f,
                upgradeScrapCostStarter = 50.0f
            };

            ResourceModel electricity = new ResourceModel()
            {
                name = "electricity",
                amount = 20.0f,
                decay = 0.0f,
                efficiency = 1.8f,
                damageRate = 0.6f,
                startHealth = 90.0f,
                playerTarget = new Vector3(-2.5f, 0.0f, 3.4f),
                upgradeResCostRatio = 1.5f,
                upgradeScrapCostRatio = 1.1f,
                upgradeResCostStarter = 50.0f,
                upgradeScrapCostStarter = 50.0f
            };


            // Don't touch
            gameState.resources.Add(water);
            gameState.resources.Add(potatoes);
            gameState.resources.Add(electricity);

        }
    }
}