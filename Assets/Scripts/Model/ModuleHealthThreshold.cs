using System;

namespace MarsFrenzy
{
    [System.Serializable]
    public class ModuleHealthThreshold
    {
        public float threshold;
        public float modifier;

        public ModuleHealthThreshold(float thr, float mod)
        {
            threshold = thr;
            modifier = mod;
        }
    }
}