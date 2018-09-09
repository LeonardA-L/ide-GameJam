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

namespace MarsFrenzy
{
    public class GameManager : MonoBehaviour
    {
        public static readonly string savePath = "/HCC_state.sav";

        protected static GameManager instance;
        public GameState _gameState;
        private Dictionary<string, ModuleManager> modules;
        private Dictionary<string, bool> switches;

        public bool timeRuns = false;
        public bool gameOver = false;
        public float timer = 0;
        public float lastTime;
        public float lastSmoothTime;
        public float lastDialog = 0;

        private Text ductTapeStock;
        private Text scrapStock;

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

        private List<ParticleSystem> particles;

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

        public List<Vector3> crateDropPoints = new List<Vector3>();

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
            lastTime = 0;
            lastSmoothTime = 0;
            storm = false;
            pauseMenu = false;
            agentSpeed = playerAgent.speed;

            DialogManager.Instance.Init();

            animators = new List<Animator>();

            particles = new List<ParticleSystem>();


            _gameState = GameState.Load(savePath);
            idleWorksClock = _gameState.GetClock();

            var storages = StorageManager.Instance.GetAllStorages();

            if (_gameState.newGame)
            {  // New game
                Debug.Log("New game");
                _InitNewGame();
            } else
            {

            }


            // Crates drop spots
            crateDropPoints.Add(new Vector3(-2.03f, 0.0f, 30.91f));
            crateDropPoints.Add(new Vector3(5.38f, 0.0f, 36.09f));
            crateDropPoints.Add(new Vector3(3.59f, 0.0f, -2.85f));
            crateDropPoints.Add(new Vector3(14.52f, 0.0f, 4.09f));

            switches = new Dictionary<string, bool>();

            modules = new Dictionary<string, ModuleManager>();
            modules.Add("water", waterModule);
            modules.Add("potatoes", potatoesModule);
            modules.Add("electricity", electricityModule);

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

            character.Init(this, _gameState.playerHungerStart, _gameState.playerThirstStart, _gameState.starvationDecay, _gameState.playerRegen);

            GameObject ductTapeStockObj = GameObject.Find("/UI_prefab/MainCanvas/Resources/BackgroundBlue/ductTape/ductTape_Stock");
            ductTapeStock = ductTapeStockObj.GetComponent<Text>();

            GameObject scrapStockObj = GameObject.Find("/UI_prefab/MainCanvas/Resources/BackgroundBlue/scrap/scrap_Stock");
            scrapStock = scrapStockObj.GetComponent<Text>();

            playerAnimator = player.gameObject.GetComponent<Animator>();
            lastPlayerPosition = player.position;

            RegisterAnimator(playerAnimator);
            RegisterAnimator(cameraAnimator);
            RegisterAnimator(uiAnimator);
            RegisterAnimator(stormAnimator);

            crateSlots = new int[crateDropPoints.Count];

            GameObject[] particlesGOs;
            particlesGOs = GameObject.FindGameObjectsWithTag("Particles");
            foreach (GameObject particle in particlesGOs)
            {
                particles.Add(particle.GetComponent<ParticleSystem>());
            }

            HideWorkbench();

            timeRuns = true;
        }

        private void _InitNewGame()
        {
            PopulateData.Init(_gameState);
        }

        // Update is called once per frame
        void Update()
        {
            if (timeRuns)
            {
                timer += Time.deltaTime;
                
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

            if (timeRuns && (timer - lastTime) > _gameState.gameClock / (1.0f * _gameState.clockSmoothing))
            {
                lastTime = timer;
                Tick();
            }
            if (timeRuns && (timer - lastSmoothTime) > _gameState.gameClock / (1.0f * _gameState.clockSubSmoothing))
            {
                lastSmoothTime = timer;
                SubSmoothTick();
            }

            ductTapeStock.text = "" + _gameState.ductTape.amount.ToString("0.00");
            scrapStock.text = "" + _gameState.scrap.amount.ToString("0");
            
            playerAnimator.SetFloat("speed", (player.position - lastPlayerPosition).magnitude / Time.deltaTime);
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

        private void Tick()
        {
            waterModule.Tick();
            potatoesModule.Tick();
            electricityModule.Tick();

            character.Tick();
        }

        private void SubSmoothTick()
        {
            // STOOOOORM
            if(storm)
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
            }
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

            foreach(Animator anim in animators) {
                anim.enabled = false;
            }

            foreach (ParticleSystem particle in particles)
            {
                particle.Pause();
            }

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

                _gameState.Save(savePath);
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

            animators.RemoveAll(item => item == null);

            foreach (Animator anim in animators)
            {
                anim.enabled = true;
            }

            foreach (ParticleSystem particle in particles)
            {
                particle.Play();
            }
        }

        public void GotoMainMenu()
        {
            GetComponent<Scene>().LaunchMenuScene();
        }

        public void EndDialog()
        {
            Play();
            lastDialog = timer;
        }

        public bool IsActive(string name)
        {
            ModuleManager module = modules[name];
            return module.activated;
        }

        public void SetActive(string name, bool _active)
        {
            ModuleManager module = modules[name];
            module.SetActive(_active);
        }

        public void AddAmount(string name, float _howMuch)
        {
            if (name == "ductTape")
            {
                _gameState.ductTape.amount += _howMuch;
                if (_gameState.ductTape.amount < 0.0f)
                {
                    _gameState.ductTape.amount = 0.0f;
                }
                return;
            }

            if (name == "scrap")
            {
                _gameState.scrap.amount += _howMuch;
                if (_gameState.scrap.amount < 0.0f)
                {
                    _gameState.scrap.amount = 0.0f;
                }
                return;
            }

            ModuleManager module = modules[name];
            module.res.amount += _howMuch;
            if (module.res.amount < 0.0f)
            {
                module.res.amount = 0.0f;
            }
        }

        public float GetAmount(string name)
        {
            if (name == "ductTape")
            {
                return _gameState.ductTape.amount;
            }

            if (name == "scrap")
            {
                return _gameState.scrap.amount;
            }

            ModuleManager module = modules[name];
            return module.res.amount;
        }

        public float GetModuleHealth(string name)
        {
            ModuleManager module = modules[name];
            return module.moduleHealth;
        }

        public void AddModuleHealth(string name, float _howMuch)
        {
            ModuleManager module = modules[name];
            module.AddHealth(_howMuch);
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
            waterModule.res.amount += _crate.water;
            potatoesModule.res.amount += _crate.potatoes;
            electricityModule.res.amount += _crate.electricity;

            _gameState.ductTape.amount += _crate.ductTape;
            _gameState.scrap.amount += _crate.scrap;
        }

        public void RegisterAnimator(Animator _animator)
        {
            animators.Add(_animator);
        }

        public void UnregisterAnimator(Animator _animator)
        {
            animators.Remove(_animator);
        }

        public bool ReadSwitch(string _name)
        {
            bool ret = false;
            switches.TryGetValue(_name, out ret);
            return ret;
        }

        public void SetSwitch(string _name, bool _value)
        {
            switches.Add(_name, _value);
        }

        public void ActivateBase()
        {
            OnboardingStep = OnboardingFirstSection;
            WidenView();
            ShowUI();
            CameraController.Instance.SetModeBase();
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
    }

}