using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace MarsFrenzy
{
    public class GameManager : MonoBehaviour
    {
        protected static GameManager instance;
        public GameDataModel data;
        private List<ModuleManager> modules;

        public bool timeRuns = false;
        public bool gameOver = false;
        public float timer = 0;
        public int frame = 0;
        private float lastTime;

        private Text ductTapeStock;
        private Text timerText;

        public CharacterLife character;
        public EndScreen endScreen;

        private int prevFrame = 0;

        private int onboardingStep = 0;
        public int OnboardingStep
        {
            get
            {
                return onboardingStep;
            }

            set
            {
                onboardingStep = value;
            }
        }

        // Use this for initialization
        void Start()
        {
            Debug.Log("Init GameManager");
            setInstance(this);
            timeRuns = false;
            timer = 0;
            frame = 0;
            lastTime = 0;

            data = LoadGameData("gamedata.json");

            modules = new List<ModuleManager>();

            int i = 0;
            for (; i < data.resources.Length; i++)
            {
                int prevI = i - 1;
                if (prevI < 0)
                {
                    prevI += data.resources.Length;
                }
                GameObject res = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Module_prefab"));
                res.transform.position = new Vector3(0, 0, 0);
                res.name = "Module_" + data.resources[i].name;
                ModuleManager module = res.GetComponent<ModuleManager>();
                module.id = i;
                module.Init(i, data.resources[i], data.resources[prevI], this);
                modules.Add(module);
            }

            character.Init(this, data.playerHungerStart, data.playerThirstStart, data.starvationDecay);

            GameObject ductTapeStockObj = GameObject.Find("/UI_prefab/MainCanvas/Resources/ductTape_Stock");
            ductTapeStock = ductTapeStockObj.GetComponent<Text>();

            GameObject timerTextObj = GameObject.Find("/UI_prefab/MainCanvas/Timer");
            timerText = timerTextObj.GetComponent<Text>();

            timeRuns = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (timeRuns)
            {
                timer += Time.deltaTime;
                frame++;
            }

            if (timeRuns && (timer - lastTime) > data.gameClock / (1.0f * data.clockSmoothing))
            {
                prevFrame = frame;
                lastTime = timer;
                Tick();
            }

            ductTapeStock.text = "" + data.ductTape.amount;

            timerText.text = "T:" + timer.ToString("0.00") + "s";
        }

        private void Tick()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Tick();
            }

            character.Tick();
        }

        private static void setInstance(GameManager _instance)
        {
            instance = _instance;
        }

        public static GameManager Instance
        {
            get
            {
                return instance;
            }
        }

        private GameDataModel LoadGameData(string path)
        {
            // Path.Combine combines strings into a file path
            // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
            string filePath = Path.Combine(Application.streamingAssetsPath, path);

            if (File.Exists(filePath))
            {
                // Read the json from the file into a string
                string dataAsJson = File.ReadAllText(filePath);
                // Pass the json to JsonUtility, and tell it to create a GameData object from it
                GameDataModel loadedData = JsonUtility.FromJson<GameDataModel>(dataAsJson);

                return loadedData;
            }
            else
            {
                Debug.LogError("Cannot load game data!");
                throw new System.Exception("Cannot load data");
            }
        }

        public void ToggleTime()
        {
            if(!timeRuns && gameOver)
            {
                return;
            }
            timeRuns = !timeRuns;
        }

        public void Pause()
        {
            timeRuns = false;
        }

        public void Stop()
        {
            timeRuns = false;
            gameOver = true;
        }

        public void Play()
        {
            if(gameOver)
            {
                return;
            }
            timeRuns = true;
        }

        public void EndDialog()
        {
            Play();
        }

        public bool IsActive(string name)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i].res.name == name)
                {
                    return modules[i].activated;
                }
            }
            return false;
        }

        public float GetModuleHealth(string name)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i].res.name == name)
                {
                    return modules[i].moduleHealth;
                }
            }
            return -1.0f;
        }

        public float GetPlayerHunger()
        {
            return character.hunger;
        }

        public float GetPlayerThirst()
        {
            return character.thirst;
        }

        public bool IsPlayerDead()
        {
            return character.dead;
        }

        public void TriggerGameOver()
        {
            Stop();
            endScreen.GameOver();
        }

        public void TriggerVictory()
        {
            Stop();
            endScreen.Victory();
        }
    }

}