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
        public float x = 0;
        public float z = 0;
        public string name;

        public Vector3 healthPlacement;
    }
}