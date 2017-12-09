using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.IO;
using EZCameraShake;
using UnityEngine.EventSystems;

namespace MarsFrenzy
{
    public class GameManager : MonoBehaviour
    {
        protected static GameManager instance;
        public GameDataModel data;
        private Dictionary<string, ModuleManager> modules;

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

            data = PopulateData.Init();

            modules = new Dictionary<string, ModuleManager>();
            modules.Add("water", waterModule);
            modules.Add("potatoes", potatoesModule);
            modules.Add("electricity", electricityModule);

            int i = 0;
            for (; i < data.resources.Count; i++)
            {
                int prevI = i - 1;
                if (prevI < 0)
                {
                    prevI += data.resources.Count;
                }
                ModuleManager module = modules[data.resources[i].name].GetComponent<ModuleManager>();
                module.id = i;
                module.Init(i, data.resources[i], data.resources[prevI]);
            }

            character.Init(this, data.playerHungerStart, data.playerThirstStart, data.starvationDecay, data.playerRegen);

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

            crateSlots = new int[data.crateDropPoints.Count];

            GameObject[] particlesGOs;
            particlesGOs = GameObject.FindGameObjectsWithTag("Particles");
            foreach (GameObject particle in particlesGOs)
            {
                particles.Add(particle.GetComponent<ParticleSystem>());
            }

            HideWorkbench();

            GetComponent<SaveManager>().Load();

            timeRuns = true;
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

            if (timeRuns && (timer - lastTime) > data.gameClock / (1.0f * data.clockSmoothing))
            {
                lastTime = timer;
                Tick();
            }
            if (timeRuns && (timer - lastSmoothTime) > data.gameClock / (1.0f * data.clockSubSmoothing))
            {
                lastSmoothTime = timer;
                SubSmoothTick();
            }

            ductTapeStock.text = "" + data.ductTape.amount.ToString("0.00");
            scrapStock.text = "" + data.scrap.amount.ToString("0");
            
            playerAnimator.SetFloat("speed", (player.position - lastPlayerPosition).magnitude / Time.deltaTime);
            lastPlayerPosition = player.position;

            if(playerWalking && playerAgent.remainingDistance < 0.1f)
            {
                playerWalking = false;
                AudioManager.Instance.StopSound("characterWalking");
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
                            module.AddHealth(-data.stormDamage);
                        }
                    }
                }
                stormTicks++;

                if(stormTicks > data.stormDuration)
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

                GetComponent<SaveManager>().Save();
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
                return data.ductTape.amount;
            }

            if (name == "scrap")
            {
                return data.scrap.amount;
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

            int baseRandSlot = (int)(Random.value * data.crateDropPoints.Count);
            int successSlot = -1;
            for (int i = 0; i < data.crateDropPoints.Count; i++)
            {
                int slot = (baseRandSlot + i) % data.crateDropPoints.Count;
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
            waterModule.res.amount += _crate.water;
            potatoesModule.res.amount += _crate.potatoes;
            electricityModule.res.amount += _crate.electricity;

            data.ductTape.amount += _crate.ductTape;
            data.scrap.amount += _crate.scrap;
        }

        public void RegisterAnimator(Animator _animator)
        {
            animators.Add(_animator);
        }

        public void UnregisterAnimator(Animator _animator)
        {
            animators.Remove(_animator);
        }

        public void GroundAction()
        {

        }
    }

}