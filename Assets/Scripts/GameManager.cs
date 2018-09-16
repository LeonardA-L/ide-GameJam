using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EZCameraShake;
using UnityEngine.EventSystems;
using IdleWorks;
using System;
using Timestamp = System.Double;

namespace MarsFrenzy
{
    public class GameManager : MonoBehaviour
    {
        protected static GameManager m_instance;
        public GameState _gameState;

        private Timestamp m_time;
        public bool m_timeRuns = false;
        public bool m_gameOver = false;

        public CharacterLife m_character;
        public EndScreen m_endScreen;

        public int m_onboardingStep = 0;

        public Animator m_cameraAnimator;
        public Animator m_uiAnimator;

        public NavMeshAgent m_playerAgent;
        public Transform m_player;
        public Vector3 m_lastPlayerPosition;
        private bool m_playerWalking;
        private Animator m_playerAnimator;
        private float m_agentSpeed;

        public int[] m_crateSlots;

        private bool m_storm;
        private int m_stormTicks = 0;
        public Animator m_stormAnimator;

        public ModuleManager m_waterModule;
        public ModuleManager m_potatoesModule;
        public ModuleManager m_electricityModule;

        private CameraShakeInstance m_stormShake;

        public GameObject m_workbenchUI;

        public bool m_pauseMenu;

        public Transform m_marsBase;
        public Vector3 m_maxDistanceToBase = new Vector3(6.0f, 0, 24.0f);

        public static int m_onboardingFirstSection = 50;

        private Clock m_idleWorksClock;
        private Storage m_globalStorage;

        public List<Vector3> m_crateDropPoints = new List<Vector3>();

        // Unserialized managers
        private AnimatorsManager m_animatorsManager = new AnimatorsManager();

        public int OnboardingStep
        {
            get
            {
                return m_onboardingStep;
            }

            set
            {
                m_onboardingStep = value;
            }
        }

        // Use this for initialization
        void Start()
        {
            Debug.Log("Init GameManager");
            setInstance(this);
            m_timeRuns = false;
            m_storm = false;
            m_pauseMenu = false;
            m_agentSpeed = m_playerAgent.speed;

            DialogManager.Instance.Init();

            _gameState = GameState.Load(Constants.SAVE_PATH);
            m_idleWorksClock = _gameState.GetClock();

            var storages = StorageManager.Instance.GetAllStorages();

            m_time = TimeUtils.Timestamp();

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
                        m_globalStorage = s;
                    }
                }
            }


            // Crates drop spots
            m_crateDropPoints.Add(new Vector3(-2.03f, 0.0f, 30.91f));
            m_crateDropPoints.Add(new Vector3(5.38f, 0.0f, 36.09f));
            m_crateDropPoints.Add(new Vector3(3.59f, 0.0f, -2.85f));
            m_crateDropPoints.Add(new Vector3(14.52f, 0.0f, 4.09f));

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

            m_playerAnimator = m_player.gameObject.GetComponent<Animator>();
            m_lastPlayerPosition = m_player.position;

            AnimatorsManager.Instance.RegisterAnimator(m_playerAnimator);
            AnimatorsManager.Instance.RegisterAnimator(m_cameraAnimator);
            AnimatorsManager.Instance.RegisterAnimator(m_uiAnimator);
            AnimatorsManager.Instance.RegisterAnimator(m_stormAnimator);

            m_crateSlots = new int[m_crateDropPoints.Count];

            GameObject[] particlesGOs;
            particlesGOs = GameObject.FindGameObjectsWithTag("Particles");
            foreach (GameObject particle in particlesGOs)
            {
                AnimatorsManager.Instance.RegisterParticleSystem(particle.GetComponent<ParticleSystem>());
            }

            HideWorkbench();

            m_timeRuns = true;
        }

        private void _InitNewGame()
        {
            PopulateData.Init(_gameState);

            m_globalStorage = new Storage(Constants.STORAGE_MAIN);

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
            playerConsumptionHunger.AddFuel(Constants.POTATO, 0.7f, m_globalStorage);
            playerConsumptionHunger.SetAllowPartialConsumption(true);
            GeneratorManager.Instance.RegisterGeneratorClass(Constants.PLAYER_CONSUMPTION + Constants.POTATO, playerConsumptionHunger);

            Generator playerConsumptionThirst = new Generator(Constants.PLAYER_CONSUMPTION + Constants.WATER, new GenerationIntervalUtils.IntervalPowered(moduleFrequency), new GenerationUtils.GenerateLinear(0, 1), new CostsUtils.CostsStandard(), false, false);
            playerConsumptionThirst.AddFuel(Constants.WATER, 0.7f, m_globalStorage);
            playerConsumptionThirst.SetAllowPartialConsumption(true);
            GeneratorManager.Instance.RegisterGeneratorClass(Constants.PLAYER_CONSUMPTION + Constants.WATER, playerConsumptionThirst);

            Generator hunger = new Generator(Constants.HUNGER, new GenerationIntervalUtils.IntervalPowered(2.0f * 1000), new GenerationUtils.GenerateLinear(0,1), new CostsUtils.CostsStandard(), false, false);
            hunger.AddFuel(Constants.HUNGER, _gameState.starvationDecay, m_globalStorage);
            hunger.SetAllowPartialConsumption(true);
            hunger.SetClampingValues(0, _gameState.playerHungerStart);
            GeneratorManager.Instance.RegisterGeneratorClass(Constants.HUNGER, hunger);

            Generator thirst = new Generator(Constants.THIRST, new GenerationIntervalUtils.IntervalPowered(2.0f * 1000), new GenerationUtils.GenerateLinear(0,1), new CostsUtils.CostsStandard(), false, false);
            thirst.AddFuel(Constants.THIRST, _gameState.starvationDecay, m_globalStorage);
            thirst.SetAllowPartialConsumption(true);
            thirst.SetClampingValues(0, _gameState.playerThirstStart);
            GeneratorManager.Instance.RegisterGeneratorClass(Constants.THIRST, thirst);

            Generator regen = new Generator(Constants.REGEN_THIRST, new GenerationIntervalUtils.IntervalPowered(2.0f * 1000), new GenerationUtils.GenerateLinear(), new CostsUtils.CostsStandard(), true, false);
            regen.AddOutput(Constants.THIRST, _gameState.playerRegen, m_globalStorage);
            GeneratorManager.Instance.RegisterGeneratorClass(Constants.REGEN_THIRST, regen);

            Generator regenHunger = new Generator(Constants.REGEN_HUNGER, new GenerationIntervalUtils.IntervalPowered(2.0f * 1000), new GenerationUtils.GenerateLinear(), new CostsUtils.CostsStandard(), true, false);
            regenHunger.AddOutput(Constants.HUNGER, _gameState.playerRegen, m_globalStorage);
            GeneratorManager.Instance.RegisterGeneratorClass(Constants.REGEN_HUNGER, regenHunger);

            // Add Start values
            m_globalStorage.Add(Constants.WATER, 20);
            m_globalStorage.Add(Constants.POTATO, 15);
            m_globalStorage.Add(Constants.ELECTRICITY, 20);

            m_globalStorage.Add(Constants.DUCTTAPE, 50);
            m_globalStorage.Add(Constants.SCRAP, 75);

            m_globalStorage.Add(Constants.HUNGER, _gameState.playerHungerStart);
            m_globalStorage.Add(Constants.THIRST, _gameState.playerThirstStart);

            m_globalStorage.Add(Constants.WATER + Constants.MODULE, 1);
            m_globalStorage.Add(Constants.POTATO + Constants.MODULE, 1);
            m_globalStorage.Add(Constants.ELECTRICITY + Constants.MODULE, 1);

            m_globalStorage.Add(Constants.WATER + Constants.MODULE + Constants.MODULE_HEALTH_SUFFIX, 5);
            m_globalStorage.Add(Constants.POTATO + Constants.MODULE + Constants.MODULE_HEALTH_SUFFIX, 5);
            m_globalStorage.Add(Constants.ELECTRICITY + Constants.MODULE + Constants.MODULE_HEALTH_SUFFIX, 5);

            m_globalStorage.Add(Constants.WATER + Constants.MODULE + Constants.MODULE_HEALTH_REPAIR, 1);
            m_globalStorage.Add(Constants.POTATO + Constants.MODULE + Constants.MODULE_HEALTH_REPAIR, 1);
            m_globalStorage.Add(Constants.ELECTRICITY + Constants.MODULE + Constants.MODULE_HEALTH_REPAIR, 1);

            m_globalStorage.Add(Constants.WATER + Constants.MODULE + Constants.MODULE_HEALTH_DAMAGE, 1);
            m_globalStorage.Add(Constants.POTATO + Constants.MODULE + Constants.MODULE_HEALTH_DAMAGE, 1);
            m_globalStorage.Add(Constants.ELECTRICITY + Constants.MODULE + Constants.MODULE_HEALTH_DAMAGE, 1);

            m_globalStorage.Add(Constants.REGEN_HUNGER, 1);
            m_globalStorage.Add(Constants.REGEN_THIRST, 1);
            m_globalStorage.Add(Constants.PLAYER_CONSUMPTION + Constants.POTATO, 1);
            m_globalStorage.Add(Constants.PLAYER_CONSUMPTION + Constants.WATER, 1);
        }

        private void RegisterModule(string _moduleName, string _outputName, string _fuelName, double _maxHealth, double _frequency, double _efficiency, double _damageRate, double _damageFreq, double _repairCost, double _repairFreq)
        {
            // The module
            Generator waterModule = new Generator(_moduleName, new GenerationIntervalUtils.IntervalPowered(_frequency), new GenerationUtils.GenerateLinear(), new CostsUtils.CostsStandard(), false, false);
            waterModule.AddFuel(_fuelName, 1, m_globalStorage);
            waterModule.AddOutput(_outputName, _efficiency, m_globalStorage);
            GeneratorManager.Instance.RegisterGeneratorClass(_moduleName, waterModule);

            // Health stuff
            GeneratorManager.Instance.RegisterGeneratorClass(_moduleName + Constants.MODULE_HEALTH_SUFFIX, new Generator(_moduleName + Constants.MODULE_HEALTH_SUFFIX, null, new GenerationUtils.GenerateLinear(), new CostsUtils.CostsStandard(0, 0), false, false, null, null, 0.0f, _maxHealth));

            // Repairing unit
            Generator waterModuleRepair = new Generator(_moduleName + Constants.MODULE_HEALTH_REPAIR, new GenerationIntervalUtils.IntervalPowered(_repairFreq), new GenerationUtils.GenerateLinear(), new CostsUtils.CostsStandard(), false, true);
            waterModuleRepair.AddFuel(Constants.DUCTTAPE, _repairCost, m_globalStorage);
            waterModuleRepair.AddOutput(_moduleName + Constants.MODULE_HEALTH_SUFFIX, 1, m_globalStorage);
            GeneratorManager.Instance.RegisterGeneratorClass(_moduleName + Constants.MODULE_HEALTH_REPAIR, waterModuleRepair);

            // Damaging unit
            Generator waterModuleDamage = new Generator(_moduleName + Constants.MODULE_HEALTH_DAMAGE, new GenerationIntervalUtils.IntervalPowered(_damageFreq), new GenerationUtils.GenerateLinear(), new CostsUtils.CostsStandard(), false, false);
            waterModuleDamage.AddFuel(_moduleName + Constants.MODULE_HEALTH_SUFFIX, _damageRate, m_globalStorage);
            GeneratorManager.Instance.RegisterGeneratorClass(_moduleName + Constants.MODULE_HEALTH_DAMAGE, waterModuleDamage);
        }

        // Update is called once per frame
        void Update()
        {
            m_time = TimeUtils.Timestamp();

            if (m_timeRuns)
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

            m_idleWorksClock.Update();

            //ductTapeStock.text = "" + _gameState.ductTape.amount.ToString("0.00");
            //scrapStock.text = "" + _gameState.scrap.amount.ToString("0");

            m_playerAnimator.SetFloat("speed", (m_player.position - m_lastPlayerPosition).magnitude / ((float)Time.deltaTime));
            m_lastPlayerPosition = m_player.position;

            if(m_playerWalking && m_playerAgent.remainingDistance < 0.1f)
            {
                m_playerWalking = false;
                AudioManager.Instance.StopSound("characterWalking");
            }

            if (OnboardingStep < m_onboardingFirstSection)
            {
                if ((m_player.position - m_marsBase.position).magnitude < 2.0f)
                {
                    ActivateBase();
                }
            }
            else
            {
                Vector3 diffWithBase = (m_marsBase.position - m_player.position);
                if (CameraController.Instance.mode == CameraMode.EXPLORE && (Mathf.Abs(diffWithBase.x) < m_maxDistanceToBase.x || Mathf.Abs(diffWithBase.z) < m_maxDistanceToBase.z))
                {
                    ReachBase();
                }
                if (CameraController.Instance.mode == CameraMode.BASE && (Mathf.Abs(diffWithBase.x) > m_maxDistanceToBase.x || Mathf.Abs(diffWithBase.z) > m_maxDistanceToBase.z))
                {
                    LeaveBase();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (m_pauseMenu)
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
            m_playerWalking = true;
            AudioManager.Instance.PlaySound("characterWalking");
            m_playerAgent.SetDestination(goal);
        }

        private static void setInstance(GameManager _instance)
        {
            m_instance = _instance;
        }

        public static GameManager Instance
        {
            get
            {
                return m_instance;
            }
        }

        public void Pause()
        {
            m_timeRuns = false;
            m_playerAgent.speed = 0;

            AnimatorsManager.Instance.Pause();

            m_waterModule.StopAction();
            m_potatoesModule.StopAction();
            m_electricityModule.StopAction();
        }

        public void PauseMenu()
        {
            if (!m_pauseMenu)
            {
                Pause();
                m_endScreen.Pause();
                m_pauseMenu = true;

                _gameState.Save();
            }
            else
            {
                m_endScreen.Unpause();
                m_pauseMenu = false;
                Play();
            }
        }

        public void Stop()
        {
            m_timeRuns = false;
            m_gameOver = true;
        }

        public void Play()
        {
            if(m_gameOver)
            {
                return;
            }
            m_timeRuns = true;
            m_playerAgent.speed = m_agentSpeed;

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
            return m_globalStorage.GetGenerator(name).IsActive;
        }

        public void SetActive(string name, bool _active)
        {
            m_globalStorage.GetGenerator(name).SetActive(_active);
        }

        public void AddAmount(string name, double _howMuch)
        {
            m_globalStorage.Add(name, _howMuch);
        }

        public double GetAmount(string name)
        {
            return m_globalStorage.GetAmountOf(name);
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
            return m_character.Hunger;
        }

        public double GetPlayerThirst()
        {
            return m_character.Thirst;
        }

        public bool IsPlayerDead()
        {
            return m_character.dead;
        }

        public void TriggerGameOver()
        {
            Stop();
            AudioManager.Instance.PlaySound("gameOver");
            m_endScreen.GameOver();
        }

        public void TriggerVictory()
        {
            Stop();
            AudioManager.Instance.PlaySound("win");
            m_endScreen.Victory();
        }

        public void WidenView()
        {
            m_cameraAnimator.SetBool("wide", true);
        }

        public void ShowUI()
        {
            m_uiAnimator.SetBool("uiActive", true);
        }

        public void ShowWorkbench()
        {
            Debug.Log("Showing Workbench");
            m_workbenchUI.SetActive(true);
        }

        public void HideWorkbench()
        {
            Debug.Log("Hiding Workbench");
            m_workbenchUI.SetActive(false);
        }

        public void StartStorm()
        {
            m_storm = true;
            m_stormAnimator.SetBool("activated", m_storm);
            m_stormTicks = 0;

            m_stormShake = CameraShaker.Instance.StartShake(4.5f, 7, 10);
            //CameraShaker.Instance.ShakeOnce(Magnitude, Roughness, 0, FadeOutTime);
        }

        public void StopStorm()
        {
            m_storm = false;
            m_stormAnimator.SetBool("activated", m_storm);
            m_stormShake.StartFadeOut(5f);
        }

        public void RestoreGame()
        {
            m_uiAnimator.SetBool("uiActive", true);
            m_cameraAnimator.SetBool("wide", true);
        }

        public void CreateCrate(float _water, float _potatoes, float _electricity, float _scrap, float _ductTape)
        {

            int baseRandSlot = (int)(UnityEngine.Random.value * m_crateDropPoints.Count);
            int successSlot = -1;
            for (int i = 0; i < m_crateDropPoints.Count; i++)
            {
                int slot = (baseRandSlot + i) % m_crateDropPoints.Count;
                if (m_crateSlots[slot] != 1)
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
            m_crateSlots[successSlot] = 1;
            crate.name = "Drop_Crate_"+ successSlot;

            crate.transform.position = new Vector3(m_crateDropPoints[successSlot].x, 15.0f, m_crateDropPoints[successSlot].z);
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
            OnboardingStep = m_onboardingFirstSection;
            WidenView();
            ShowUI();
            CameraController.Instance.SetModeBase();
            // start player decay
            m_globalStorage.GetGenerator(Constants.PLAYER_CONSUMPTION + Constants.WATER).SetActive(true);
            m_globalStorage.GetGenerator(Constants.PLAYER_CONSUMPTION + Constants.POTATO).SetActive(true);
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
                return m_time;
            }
        }
        
        public Transform Player
        {
            get
            {
                return m_player;
            }
        }
    }

}