using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MarsFrenzy
{
    public class SaveManager : MonoBehaviour
    {

        private static string savePath;

        // Use this for initialization
        void Start()
        {
        }

        public void Save()
        {
            savePath = Application.persistentDataPath + "/SCC_savegame.sav";
            SaveModel save = new SaveModel();
            GameManager gm = GameManager.Instance;

            if (gm.gameOver)
                return;

            if (gm.OnboardingStep < 20)
                return;

            save.playerX = gm.player.position.x;
            save.playerY = gm.player.position.y;
            save.playerZ = gm.player.position.z;

            save.waterAmount = gm.data.resources[0].amount;
            save.potatoesAmount = gm.data.resources[1].amount;
            save.electricityAmount = gm.data.resources[2].amount;

            save.ductTapeAmount = gm.data.ductTape.amount;
            save.scrapAmount= gm.data.scrap.amount;

            save.timeRuns = gm.timeRuns;
            save.timer = gm.timer;
            save.lastTime = gm.lastTime;
            save.lastSmoothTime = gm.lastSmoothTime;
            save.lastDialog = gm.lastDialog;
            save.onboardingStep = gm.onboardingStep;

            save.waterHealth = gm.waterModule.moduleHealth;
            save.waterLevel = gm.waterModule.level;
            save.waterActive = gm.waterModule.activated;
            save.potatoesHealth = gm.potatoesModule.moduleHealth;
            save.potatoesLevel = gm.potatoesModule.level;
            save.potatoesActive = gm.potatoesModule.activated;
            save.electricityHealth = gm.electricityModule.moduleHealth;
            save.electricityLevel = gm.electricityModule.level;
            save.electricityActive = gm.electricityModule.activated;

            save.eventsFlags = new List<bool>();
            foreach (DialogEvent ev in DialogManager.Instance.events)
            {
                save.eventsFlags.Add(ev.isDone());
            }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(savePath);

            bf.Serialize(file, save);
            file.Close();
        }

        public void Load()
        {
            savePath = Application.persistentDataPath + "/SCC_savegame.sav";
            if (!File.Exists(savePath))
            {
                Debug.Log("Savegame doesn't exist");
                return;
            }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);

            SaveModel save = (SaveModel) bf.Deserialize(file);
            file.Close();

            if(save == null)
            {
                Debug.Log("Cannot load savegame");
                return;
            }

            GameManager gm = GameManager.Instance;

            gm.player.position = new Vector3(save.playerX, save.playerY, save.playerZ);

            gm.data.resources[0].amount = save.waterAmount;
            gm.data.resources[1].amount = save.potatoesAmount;
            gm.data.resources[2].amount = save.electricityAmount;

            gm.data.ductTape.amount = save.ductTapeAmount;
            gm.data.scrap.amount = save.scrapAmount;

            gm.timeRuns = save.timeRuns;
            gm.timer = save.timer;
            gm.lastTime = save.lastTime;
            gm.lastSmoothTime = save.lastSmoothTime;
            gm.lastDialog = save.lastDialog;
            gm.onboardingStep = save.onboardingStep;

            gm.waterModule.moduleHealth = save.waterHealth;
            gm.waterModule.level = save.waterLevel;
            gm.waterModule.activated = save.waterActive;
            gm.potatoesModule.moduleHealth = save.potatoesHealth;
            gm.potatoesModule.level = save.potatoesLevel;
            gm.potatoesModule.activated = save.potatoesActive;
            gm.electricityModule.moduleHealth = save.electricityHealth;
            gm.electricityModule.level = save.electricityLevel;
            gm.electricityModule.activated = save.electricityActive;

            int i = 0;
            foreach (bool done in save.eventsFlags)
            {
                Debug.Log(done);
                Debug.Log(DialogManager.Instance.events[i]);
                if (DialogManager.Instance.events[i] != null)
                {
                    DialogManager.Instance.events[i].SetDone(true);
                    i++;
                }
            }

            gm.RestoreGame();
        }
    }
}