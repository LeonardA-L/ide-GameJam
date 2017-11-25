using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
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
        public float lastDialog = 0;

        private Text ductTapeStock;
        private Text scrapStock;

        public CharacterLife character;
        public EndScreen endScreen;

        private int onboardingStep = 0;

        public Animator cameraAnimator;
        public Animator uiAnimator;

        public NavMeshAgent playerAgent;
        public Transform player;
        public Vector3 lastPlayerPosition;
        public Animator playerAnimator;
        private float agentSpeed;

        public int[] crateSlots;

        private List<ParticleSystem> particles;

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

        private List<Animator> animators;

        // Use this for initialization
        void Start()
        {
            Debug.Log("Init GameManager");
            setInstance(this);
            timeRuns = false;
            timer = 0;
            frame = 0;
            lastTime = 0;

            animators = new List<Animator>();

            particles = new List<ParticleSystem>();

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

            GameObject ductTapeStockObj = GameObject.Find("/UI_prefab/MainCanvas/Resources/BackgroundBlue/ductTape/ductTape_Stock");
            ductTapeStock = ductTapeStockObj.GetComponent<Text>();

            GameObject scrapStockObj = GameObject.Find("/UI_prefab/MainCanvas/Resources/BackgroundBlue/scrap/scrap_Stock");
            scrapStock = scrapStockObj.GetComponent<Text>();

            playerAnimator = player.gameObject.GetComponent<Animator>();
            lastPlayerPosition = player.position;

            RegisterAnimator(playerAnimator);
            RegisterAnimator(cameraAnimator);
            RegisterAnimator(uiAnimator);

            crateSlots = new int[data.crateDropPoints.Length];

            GameObject[] particlesGOs;
            particlesGOs = GameObject.FindGameObjectsWithTag("Particles");
            foreach (GameObject particle in particlesGOs)
            {
                particles.Add(particle.GetComponent<ParticleSystem>());
            }

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
                lastTime = timer;
                Tick();
            }

            ductTapeStock.text = "" + data.ductTape.amount.ToString("0.00");
            scrapStock.text = "" + data.scrap.amount.ToString("0");
            
            playerAnimator.SetFloat("speed", (player.position - lastPlayerPosition).magnitude / Time.deltaTime);
            lastPlayerPosition = player.position;
        }

        private void Tick()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Tick();
            }

            character.Tick();
        }

        public void SetPlayerAction(Vector3 goal)
        {
            playerAgent.SetDestination(goal);
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

        public void Pause()
        {
            timeRuns = false;
            agentSpeed = playerAgent.speed;
            playerAgent.speed = 0;

            foreach(Animator anim in animators) {
                anim.enabled = false;
            }

            foreach (ParticleSystem particle in particles)
            {
                particle.Pause();
            }
        }

        public void PauseMenu()
        {
            Pause();
            endScreen.Pause();
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
            playerAgent.speed = agentSpeed;

            foreach (Animator anim in animators)
            {
                anim.enabled = true;
            }

            foreach (ParticleSystem particle in particles)
            {
                particle.Play();
            }
        }

        public void EndDialog()
        {
            Play();
            lastDialog = timer;
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

        public void SetActive(string name, bool _active)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i].res.name == name)
                {
                    modules[i].SetActive(_active);
                }
            }
        }

        public void AddAmount(string name, float _howMuch)
        {
            if (name == "ductTape")
            {
                data.ductTape.amount += _howMuch;
                if (data.ductTape.amount < 0.0f)
                {
                    data.ductTape.amount = 0.0f;
                }
                return;
            }

            if (name == "scrap")
            {
                data.scrap.amount += _howMuch;
                if (data.scrap.amount < 0.0f)
                {
                    data.scrap.amount = 0.0f;
                }
                return;
            }

            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i].res.name == name)
                {
                    modules[i].res.amount += _howMuch;
                    if(modules[i].res.amount < 0.0f)
                    {
                        modules[i].res.amount = 0.0f;
                    }
                }
            }
        }

        public float GetAmount(string name)
        {
            if (name == "ductTape")
            {
                return data.ductTape.amount;
            }

            if (name == "scrap")
            {
                return data.scrap.amount;
            }

            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i].res.name == name)
                {
                    return modules[i].res.amount;
                }
            }
            return 0.0f;
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

        public void AddModuleHealth(string name, float _howMuch)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i].res.name == name)
                {
                    modules[i].moduleHealth += _howMuch;
                    if(modules[i].moduleHealth < 0.0f)
                    {
                        modules[i].moduleHealth = 0.0f;
                    }
                }
            }
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

        public void WidenView()
        {
            cameraAnimator.SetBool("wide", true);
        }

        public void ShowUI()
        {
            uiAnimator.SetBool("uiActive", true);
        }

        public void CreateCrate(float _water, float _potatoes, float _electricity, float _scrap, float _ductTape)
        {

            int baseRandSlot = (int)(Random.value * data.crateDropPoints.Length);
            int successSlot = -1;
            for (int i = 0; i < data.crateDropPoints.Length; i++)
            {
                int slot = (baseRandSlot + i) % data.crateDropPoints.Length;
                if (crateSlots[slot] != 1)
                {
                    successSlot = slot;
                }
            }
            if(successSlot == -1)
            {
                Debug.Log("Stacking crates, exiting.");
                return;
            }
            GameObject crate = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Drop_prefab"));
            crateSlots[successSlot] = 1;
            crate.name = "Drop_Crate_"+ successSlot;

            crate.transform.position = new Vector3(data.crateDropPoints[successSlot].x, 15.0f, data.crateDropPoints[successSlot].z);
            crate.transform.Rotate(new Vector3(0.0f, 180.0f * Random.value, 0.0f));

            DropController dc = crate.GetComponent<DropController>();
            dc.SetValues(_water <= 0 ? 0 : _water + GetRandomCrateChange(),
                         _potatoes <= 0 ? 0 : _potatoes + GetRandomCrateChange(),
                         _electricity <= 0 ? 0 : _electricity + GetRandomCrateChange(),
                         _scrap <= 0 ? 0 : _scrap + GetRandomCrateChange(),
                         _ductTape <= 0 ? 0 : _ductTape + GetRandomCrateChange(),
                         successSlot);
        }

        private float GetRandomCrateChange()
        {
            return (float)System.Math.Round(Random.value * 2.0f - 1.0f, 2);
        }

        public void CollectCrate(DropController _crate)
        {
            modules[0].res.amount += _crate.water;
            modules[1].res.amount += _crate.potatoes;
            modules[2].res.amount += _crate.electricity;

            data.ductTape.amount += _crate.ductTape;
            data.scrap.amount += _crate.scrap;
        }

        public void RegisterAnimator(Animator _animator)
        {
            animators.Add(_animator);
        }
    }

}