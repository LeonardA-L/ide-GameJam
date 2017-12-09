using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFrenzy
{
    public class PopulateData : MonoBehaviour
    {
        public static GameDataModel Init()
        {
            GameDataModel gdm = new GameDataModel
            {
                //------------ Don't touch
                moduleHealthThresholds = new List<ModuleHealthThreshold>(),
                crateDropPoints = new List<Vector3>(),
                resources = new List<ResourceModel>(),
                //------------




                // Generic data
                gameClock = 2.0f,
                clockSmoothing = 2,
                clockSubSmoothing = 10,

                playerHungerStart = 100.0f,
                playerRegen = 0.01f,
                playerThirstStart = 100.0f,
                starvationDecay = 5.0f,

                stormDamage = 1.5f,
                stormDuration = 60,

                // Upgrades
                upgradeConsumptionFactor = 0.9f,
                upgradeEfficiencyFactor = 1.5f,

                // DuctTape
                ductTape = new ResourceModel()
                {
                    name = "ductTape",
                    amount = 50.0f,
                    efficiency = 5.0f
                },

                // Scrap
                scrap = new ResourceModel()
                {
                    name = "scrap",
                    amount = 75.0f
                }
        };


            


            // Module Health Thresholds
            gdm.moduleHealthThresholds.Add(new ModuleHealthThreshold(20.0f, 0.3f));
            gdm.moduleHealthThresholds.Add(new ModuleHealthThreshold(30.0f, 0.5f));
            gdm.moduleHealthThresholds.Add(new ModuleHealthThreshold(60.0f, 0.7f));
            gdm.moduleHealthThresholds.Add(new ModuleHealthThreshold(70.0f, 0.9f));
            gdm.moduleHealthThresholds.Add(new ModuleHealthThreshold(80.0f, 1.1f));
            gdm.moduleHealthThresholds.Add(new ModuleHealthThreshold(90.0f, 1.2f));
            gdm.moduleHealthThresholds.Add(new ModuleHealthThreshold(100.0f, 1.3f));

            // Crates drop spots
            gdm.crateDropPoints.Add(new Vector3(-2.03f, 0.0f, 30.91f));
            gdm.crateDropPoints.Add(new Vector3(5.38f, 0.0f, 36.09f));
            gdm.crateDropPoints.Add(new Vector3(3.59f, 0.0f, -2.85f));
            gdm.crateDropPoints.Add(new Vector3(14.52f, 0.0f, 4.09f));





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
            gdm.resources.Add(water);
            gdm.resources.Add(potatoes);
            gdm.resources.Add(electricity);

            return gdm;
        }
    }
}