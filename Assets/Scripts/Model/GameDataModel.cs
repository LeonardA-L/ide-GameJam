using System;

namespace MarsFrenzy
{
    [System.Serializable]
    public class GameDataModel
    {
        public ResourceModel[] resources;
        public int gameClock = 1;
        public ModuleHealthThreshold[] moduleHealthThresholds;
        public ResourceModel ductTape;
    }
}