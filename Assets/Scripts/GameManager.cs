using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.IO;
using EZCameraShake;
using UnityEngine.EventSystems;
using IdleWorks;
using System;
using Timestamp = System.Double;

namespace MarsFrenzy
{
    public class GameManager : MonoBehaviour
    {
        protected static GameManager instance;
        public GameState _gameState;

        private Timestamp time;
        public bool timeRuns = false;
        public bool gameOver = false;

        public CharacterLife character;
        public EndScreen endScreen;

        public int onboardingStep = 0;

        public Animator cameraAnimator;
        public Animator uiAnimator;

        public NavMeshAgent playerAgent;
        public Transform player;
        public Vector3 lastPlayerPosition;
        private bool playerWalking;
        private Animator playerAnimator;
        private float agentSpeed;

        public int[] crateSlots;

        private bool storm;
        private int stormTicks = 0;
        public Animator stormAnimator;

        public ModuleManager waterModule;
        public ModuleManager potatoesModule;
        public ModuleManager electricityModule;

        private CameraShakeInstance stormShake;

        public GameObject workbenchUI;

        public bool pauseMenu;

        public Transform marsBase;
        public Vector3 maxDistanceToBase = new Vector3(6.0f, 0, 24.0f);

        public static int OnboardingFirstSection = 50;

        private Clock idleWorksClock;
        private Storage globalStorage;

        public List<Vector3> crateDropPoints = new List<Vector3>();

        // Unserialized managers
        private AnimatorsManager m_animatorsManager = new AnimatorsManager();

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
            storm = false;
            pauseMenu = false;
            agentSpeed = playerAgent.speed;

            DialogManager.Instance.Init();

            _gameState = GameState.Load(Constants.SAVE_PATH);
            idleWorksClock = _gameState.GetClock();

            var storages = StorageManager.Instance.GetAllStorages();

            time = TimeUtils.Timestamp();

            if (_gameState.newGame)
            {  // New game
                Debug.Log("New game");
                _InitNewGame();
            } else
            {
                Debug.Log("Loaded game");

                foreach (Storage s in storages)
                {
                    if (s.Id == "Main")
                    {
                        globalStorage = s;
                    }
                }
            }


            // Crates drop spots
            crateDropPoints.Add(new Vector3(-2.03f, 0.0f, 30.91f));
            crateDropPoints.Add(new Vector3(5.38f, 0.0f, 36.09f));
            crateDropPoints.Add(new Vector3(3.59f, 0.0f, -2.85f));
            crateDropPoints.Add(new Vector3(14.52f, 0.0f, 4.09f));

            /*
            int i = 0;
            for (; i < _gameState.resources.Count; i++)
            {
                int prevI = i - 1;
                if (prevI < 0)
                {
                    prevI += _gameState.resources.Count;
                }
                ModuleManager module = modules[_gameState.resources[i].name].GetComponent<ModuleManager>();
                module.id = i;
                module.Init(i, _gameState.resources[i], _gameState.resources[prevI]);
            }
            */

            playerAnimator = player.gameObject.GetComponent<Animator>();
            lastPlayerPosition = player.position;

            AnimatorsManager.Instance.RegisterAnimator(playerAnimator);
            AnimatorsManager.Instance.RegisterAnimator(cameraAnimator);
            AnimatorsManager.Instance.RegisterAnimator(uiAnimator);
            AnimatorsManager.Instance.RegisterAnimator(stormAnimator);

            crateSlots = new int[crateDropPoints.Count];

            GameObject[] particlesGOs;
            particlesGOs = GameObject.FindGameObjectsWithTag("Particles");
            foreach (GameObject particle in particlesGOs)
            {
                AnimatorsManager.Instance.RegisterParticleSystem(particle.GetComponent<ParticleSystem>());
            }

            HideWorkbench();

            timeRuns = true;
        }

        private void _InitNewGame()
        {
            PopulateData.Init(_gameState);

            globalStorage = new Storage(Constants.STORAGE_MAIN);

            const double maxModuleHealth = 5;
            const double moduleFrequency = 2 * 1000.0f;

            // Primary resources
            GeneratorManager.Instance.RegisterGeneratorClass(Constants.WATER, new Generator(Constants.WATER, null, new GenerationUtils.GenerateLinear(), new CostsUtils.CostsStandard(0, 0), false, false, null, null, 0));
            GeneratorManager.Instance.RegisterGeneratorClass(Constants.POTATO, new Generator(Constants.POTATO, null, new GenerationUtils.GenerateLinear(), new CostsUtils.CostsStandard(0, 0), false, false, null, null, 0));
            GeneratorManager.Instance.RegisterGeneratorClass(Constants.ELECTRICITY, new Generator(Constants.ELECTRICITY, null, new GenerationUtils.GenerateLinear(), new CostsUtils.CostsStandard(0, 0), false, false, null, null, 0));

            GeneratorManager.Instance.RegisterGeneratorClass(Constants.DUCTTAPE, new Generator(Constants.DUCTTAPE, null, new GenerationUtils.GenerateLinear(), new CostsUtils.CostsStandard(0, 0), false, false, null, null, 0));
            GeneratorManager.Instance.RegisterGeneratorClass(Constants.SCRAP, new Generator(Constants.SCRAP, null, new GenerationUtils.GenerateLinear(), new CostsUtils.CostsStandard(0, 0), false, false, null, null, 0));

            // Generators Modules
            RegisterModule(Constants.WATER + Constants.MODULE, Constants.WATER, Constants.ELECTRICITY, maxModuleHealth, moduleFrequency, 1.8f, 0.1f, 2 * 1000.0f, 5, 3.0f * 1000.0f);
            RegisterModule(Constants.POTATO + Constants.MODULE, Constants.POTATO, Constants.WATER, maxModuleHealth, moduleFrequency, 1.9f, 0.1f, 2 * 1000.0f, 5, 6.0f * 1000.0f);
            RegisterModule(Constants.ELECTRICITY + Constants.MODULE, Constants.ELECTRICITY, Constants.POTATO, maxModuleHealth, moduleFrequency, 1.8f, 0.1f, 2 * 1000.0f, 5, 3.0f * 1000.0f);

            // Player Health stats
            Generator playerConsumptionHunger = new Generator(Constants.PLAYER_CONSUMPTION + Constants.POTATO, new GenerationIntervalUtils.IntervalPowered(moduleFrequency), new GenerationUtils.GenerateLinear(0, 1), new CostsUtils.CostsStandard(), false, false);
            playerConsumptionHunger.AddFuel(Constants.POTATO, 0.7f, globalStorage);
            playerConsumptionHunger.SetAllowPartialConsumption(true);
            GeneratorManager.Instance.RegisterGeneratorClass(Constants.PLAYER_CONSUMPTION + Constants.POTATO, playerConsumptionHunger);

            Generator playerConsumptionThirst = new Generator(Constants.PLAYER_CONSUMPTION + Constants.WATER, new GenerationIntervalUtils.IntervalPowered(moduleFrequency), new GenerationUtils.GenerateLinear(0, 1), new CostsUtils.CostsStandard(), false, false);
            playerConsumptionThirst.AddFuel(Constants.WATER, 0.7f, globalStorage);
            playerConsumptionThirst.SetAllowPartialConsumption(true);
            GeneratorManager.Instance.RegisterGeneratorClass(Constants.PLAYER_CONSUMPTION + Constants.WATER, playerConsumptionThirst);

            Generator hunger = new Generator(Constants.HUNGER, new GenerationIntervalUtils.IntervalPowered(2.0f * 1000), new GenerationUtils.GenerateLinear(0,1), new CostsUtils.CostsStandard(), false, false);
            hunger.AddFuel(Constants.HUNGER, _gameState.starvationDecay, globalStorage);
            hunger.SetAllowPartialConsumption(true);
            hunger.SetClampingValues(0, _gameState.playerHungerStart);
            GeneratorManager.Instance.RegisterGeneratorClass(Constants.HUNGER, hunger);

            Generator thirst = new Generator(Constants.THIRST, new GenerationIntervalUtils.IntervalPowered(2.0f * 1000), new GenerationUtils.GenerateLinear(0,1), new CostsUtils.CostsStandard(), false, false);
            thirst.AddFuel(Constants.THIRST, _gameState.starvationDecay, globalStorage);
            thirst.SetAllowPartialConsumption(true);
            thirst.SetClampingValues(0, _gameState.playerThirstStart);
            GeneratorManager.Instance.RegisterGeneratorClass(Constants.THIRST, thirst);

            Generator regen = new Generator(Constants.REGEN_THIRST, new GenerationIntervalUtils.IntervalPowered(2.0f * 1000), new GenerationUtils.GenerateLinear(), new CostsUtils.CostsStandard(), true, false);
            regen.AddOutput(Constants.THIRST, _gameState.playerRegen, globalStorage);
            GeneratorManager.Instance.RegisterGeneratorClass(Constants.REGEN_THIRST, regen);

            Generator regenHunger = new Generator(Constants.REGEN_HUNGER, new GenerationIntervalUtils.IntervalPowered(2.0f * 1000), new GenerationUtils.GenerateLinear(), new CostsUtils.CostsStandard(), true, false);
            regenHunger.AddOutput(Constants.HUNGER, _gameState.playerRegen, globalStorage);
            GeneratorManager.Instance.RegisterGeneratorClass(Constants.REGEN_HUNGER, regenHunger);

            // Add Start values
            globalStorage.Add(Constants.WATER, 20);
            globalStorage.Add(Constants.POTATO, 15);
            globalStorage.Add(Constants.ELECTRICITY, 20);

            globalStorage.Add(Constants.DUCTTAPE, 50);
            globalStorage.Add(Constants.SCRAP, 75);

            globalStorage.Add(Constants.HUNGER, _gameState.playerHungerStart);
            globalStorage.Add(Constants.THIRST, _gameState.playerThirstStart);

            globalStorage.Add(Constants.WATER + Constants.MODULE, 1);
            globalStorage.Add(Constants.POTATO + Constants.MODULE, 1);
            globalStorage.Add(Constants.ELECTRICITY + Constants.MODULE, 1);

            globalStorage.Add(Constants.WATER + Constants.MODULE + Constants.MODULE_HEALTH_SUFFIX, 5);
            globalStorage.Add(Constants.POTATO + Constants.MODULE + Constants.MODULE_HEALTH_SUFFIX, 5);
            globalStorage.Add(Constants.ELECTRICITY + Constants.MODULE + Constants.MODULE_HEALTH_SUFFIX, 5);

            globalStorage.Add(Constants.WATER + Constants.MODULE + Constants.MODULE_HEALTH_REPAIR, 1);
            globalStorage.Add(Constants.POTATO + Constants.MODULE + Constants.MODULE_HEALTH_REPAIR, 1);
            globalStorage.Add(Constants.ELECTRICITY + Constants.MODULE + Constants.MODULE_HEALTH_REPAIR, 1);

            globalStorage.Add(Constants.WATER + Constants.MODULE + Constants.MODULE_HEALTH_DAMAGE, 1);
            globalStorage.Add(Constants.POTATO + Constants.MODULE + Constants.MODULE_HEALTH_DAMAGE, 1);
            globalStorage.Add(Constants.ELECTRICITY + Constants.MODULE + Constants.MODULE_HEALTH_DAMAGE, 1);

            globalStorage.Add(Constants.REGEN_HUNGER, 1);
            globalStorage.Add(Constants.REGEN_THIRST, 1);
            globalStorage.Add(Constants.PLAYER_CONSUMPTION + Constants.POTATO, 1);
            globalStorage.Add(Constants.PLAYER_CONSUMPTION + Constants.WATER, 1);
        }

        private void RegisterModule(string _moduleName, string _outputName, string _fuelName, double _maxHealth, double _frequency, double _efficiency, double _damageRate, double _damageFreq, double _repairCost, double _repairFreq)
        {
            // The module
            Generator waterModule = new Generator(_moduleName, new GenerationIntervalUtils.IntervalPowered(_frequency), new GenerationUtils.GenerateLinear(), new CostsUtils.CostsStandard(), false, false);
            waterModule.AddFuel(_fuelName, 1, globalStorage);
            waterModule.AddOutput(_outputName, _efficiency, globalStorage);
            GeneratorManager.Instance.RegisterGeneratorClass(_moduleName, waterModule);

            // Health stuff
            GeneratorManager.Instance.RegisterGeneratorClass(_moduleName + Constants.MODULE_HEALTH_SUFFIX, new Generator(_moduleName + Constants.MODULE_HEALTH_SUFFIX, null, new GenerationUtils.GenerateLinear(), new CostsUtils.CostsStandard(0, 0), false, false, null, null, 0.0f, _maxHealth));

            // Repairing unit
            Generator waterModuleRepair = new Generator(_moduleName + Constants.MODULE_HEALTH_REPAIR, new GenerationIntervalUtils.IntervalPowered(_repairFreq), new GenerationUtils.GenerateLinear(), new CostsUtils.CostsStandard(), false, true);
            waterModuleRepair.AddFuel(Constants.DUCTTAPE, _repairCost, globalStorage);
            waterModuleRepair.AddOutput(_moduleName + Constants.MODULE_HEALTH_SUFFIX, 1, globalStorage);
            GeneratorManager.Instance.RegisterGeneratorClass(_moduleName + Constants.MODULE_HEALTH_REPAIR, waterModuleRepair);

            // Damaging unit
            Generator waterModuleDamage = new Generator(_moduleName + Constants.MODULE_HEALTH_DAMAGE, new GenerationIntervalUtils.IntervalPowered(_damageFreq), new GenerationUtils.GenerateLinear(), new CostsUtils.CostsStandard(), false, false);
            waterModuleDamage.AddFuel(_moduleName + Constants.MODULE_HEALTH_SUFFIX, _damageRate, globalStorage);
            GeneratorManager.Instance.RegisterGeneratorClass(_moduleName + Constants.MODULE_HEALTH_DAMAGE, waterModuleDamage);
        }

        // Update is called once per frame
        void Update()
        {
            time = TimeUtils.Timestamp();

            if (timeRuns)
            {
                // Detect click on ground
                if (Input.GetMouseButtonDown(0))
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        return;
                    }
                    RaycastHit hit;

                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                    {
                        if(hit.transform.gameObject.tag == "Ground")
                        {
                            SetPlayerAction(hit.point);
                        }
                    }
                }
            }

            idleWorksClock.Update();

            //ductTapeStock.text = "" + _gameState.ductTape.amount.ToString("0.00");
            //scrapStock.text = "" + _gameState.scrap.amount.ToString("0");

            playerAnimator.SetFloat("speed", (player.position - lastPlayerPosition).magnitude / ((float)Time.deltaTime));
            lastPlayerPosition = player.position;

            if(playerWalking && playerAgent.remainingDistance < 0.1f)
            {
                playerWalking = false;
                AudioManager.Instance.StopSound("characterWalking");
            }

            if (OnboardingStep < OnboardingFirstSection)
            {
                if ((player.position - marsBase.position).magnitude < 2.0f)
                {
                    ActivateBase();
                }
            }
            else
            {
                Vector3 diffWithBase = (marsBase.position - player.position);
                if (CameraController.Instance.mode == CameraMode.EXPLORE && (Mathf.Abs(diffWithBase.x) < maxDistanceToBase.x || Mathf.Abs(diffWithBase.z) < maxDistanceToBase.z))
                {
                    ReachBase();
                }
                if (CameraController.Instance.mode == CameraMode.BASE && (Mathf.Abs(diffWithBase.x) > maxDistanceToBase.x || Mathf.Abs(diffWithBase.z) > maxDistanceToBase.z))
                {
                    LeaveBase();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (pauseMenu)
                {
                    GetComponent<Scene>().QuitGame();
                } else
                {
                    PauseMenu();
                }
            }
        }

        private void SubSmoothTick()
        {
            // STOOOOORM
            /*if(storm)
            {
                if (stormTicks > 30)
                {
                    foreach (KeyValuePair<string, ModuleManager> entry in modules)
                    {
                        ModuleManager module = entry.Value;
                        if(module.activated)
                        {
                            module.AddHealth(-_gameState.stormDamage);
                        }
                    }
                }
                stormTicks++;

                if(stormTicks > _gameState.stormDuration)
                {
                    StopStorm();
                }
            }*/
        }

        public void SetPlayerAction(Vector3 goal)
        {
            if (DialogManager.Instance.IsActive())
            {
                return;
            }
            playerWalking = true;
            AudioManager.Instance.PlaySound("characterWalking");
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

        public void Pause()
        {
            timeRuns = false;
            playerAgent.speed = 0;

            AnimatorsManager.Instance.Pause();

            waterModule.StopAction();
            potatoesModule.StopAction();
            electricityModule.StopAction();
        }

        public void PauseMenu()
        {
            if (!pauseMenu)
            {
                Pause();
                endScreen.Pause();
                pauseMenu = true;

                _gameState.Save();
            }
            else
            {
                endScreen.Unpause();
                pauseMenu = false;
                Play();
            }
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

            AnimatorsManager.Instance.Play();
        }

        public void GotoMainMenu()
        {
            GetComponent<Scene>().LaunchMenuScene();
        }

        public void EndDialog()
        {
            Play();
        }

        public bool IsActive(string name)
        {
            return globalStorage.GetGenerator(name).IsActive;
        }

        public void SetActive(string name, bool _active)
        {
            globalStorage.GetGenerator(name).SetActive(_active);
        }

        public void AddAmount(string name, double _howMuch)
        {
            globalStorage.Add(name, _howMuch);
        }

        public double GetAmount(string name)
        {
            return globalStorage.GetAmountOf(name);
        }

        public float GetModuleHealth(string name)
        {
            throw new NotImplementedException();
        }

        public void AddModuleHealth(string name, float _howMuch)
        {
            throw new NotImplementedException();
        }

        public double GetPlayerHunger()
        {
            return character.Hunger;
        }

        public double GetPlayerThirst()
        {
            return character.Thirst;
        }

        public bool IsPlayerDead()
        {
            return character.dead;
        }

        public void TriggerGameOver()
        {
            Stop();
            AudioManager.Instance.PlaySound("gameOver");
            endScreen.GameOver();
        }

        public void TriggerVictory()
        {
            Stop();
            AudioManager.Instance.PlaySound("win");
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

        public void ShowWorkbench()
        {
            Debug.Log("Showing Workbench");
            workbenchUI.SetActive(true);
        }

        public void HideWorkbench()
        {
            Debug.Log("Hiding Workbench");
            workbenchUI.SetActive(false);
        }

        public void StartStorm()
        {
            storm = true;
            stormAnimator.SetBool("activated", storm);
            stormTicks = 0;

            stormShake = CameraShaker.Instance.StartShake(4.5f, 7, 10);
            //CameraShaker.Instance.ShakeOnce(Magnitude, Roughness, 0, FadeOutTime);
        }

        public void StopStorm()
        {
            storm = false;
            stormAnimator.SetBool("activated", storm);
            stormShake.StartFadeOut(5f);
        }

        public void RestoreGame()
        {
            uiAnimator.SetBool("uiActive", true);
            cameraAnimator.SetBool("wide", true);
        }

        public void CreateCrate(float _water, float _potatoes, float _electricity, float _scrap, float _ductTape)
        {

            int baseRandSlot = (int)(UnityEngine.Random.value * crateDropPoints.Count);
            int successSlot = -1;
            for (int i = 0; i < crateDropPoints.Count; i++)
            {
                int slot = (baseRandSlot + i) % crateDropPoints.Count;
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

            crate.transform.position = new Vector3(crateDropPoints[successSlot].x, 15.0f, crateDropPoints[successSlot].z);
            crate.transform.Rotate(new Vector3(0.0f, 180.0f * UnityEngine.Random.value, 0.0f));

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
            return (float)System.Math.Round(UnityEngine.Random.value * 2.0f - 1.0f, 2);
        }

        public void CollectCrate(DropController _crate)
        {
            AddAmount(Constants.WATER, _crate.water);
            AddAmount(Constants.POTATO, _crate.potatoes);
            AddAmount(Constants.ELECTRICITY, _crate.electricity);

            AddAmount(Constants.SCRAP, _crate.scrap);
            AddAmount(Constants.DUCTTAPE, _crate.ductTape);
        }

        public void ActivateBase()
        {
            OnboardingStep = OnboardingFirstSection;
            WidenView();
            ShowUI();
            CameraController.Instance.SetModeBase();
            // start player decay
            globalStorage.GetGenerator(Constants.PLAYER_CONSUMPTION + Constants.WATER).SetActive(true);
            globalStorage.GetGenerator(Constants.PLAYER_CONSUMPTION + Constants.POTATO).SetActive(true);
        }

        public void ReachBase()
        {
            CameraController.Instance.SetModeBase();
        }

        public void LeaveBase()
        {
            CameraController.Instance.SetModeExplore();
        }

        public GameState GameState
        {
            get
            {
                return _gameState;
            }
        }

        public Timestamp CurrentTime
        {
            get
            {
                return time;
            }
        }
    }

}