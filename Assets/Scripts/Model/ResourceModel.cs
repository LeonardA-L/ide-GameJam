using System;
using UnityEngine;

namespace MarsFrenzy
{
    [System.Serializable]
    public class ResourceModel : System.Object
    {
        public float amount = 0;
        public float decay = 0;
        public float efficiency = 0;
        public float damageRate = 0;
        public float startHealth = 100;
        public string name;

        public float upgradeResCostRatio = 1.0f;
        public float upgradeScrapCostRatio = 1.0f;
        public float upgradeResCostStarter = 1.0f;
        public float upgradeScrapCostStarter = 1.0f;

        public Vector3 playerTarget;
    }
}