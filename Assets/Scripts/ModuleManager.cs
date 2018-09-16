using IdleWorks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MarsFrenzy
{
    public class ModuleManager : MonoBehaviour
    {
        public bool repairing = false;

        public Transform health;
        private Animator healthAnimator;
        private Text stock;

        private ModuleHealthThreshold efficiencyModifier = null;

        public Animator viewAnimator;

        public GameObject tools;
        public Transform flowSpawner;
        public GameObject flowPrefab;

        public float level = 1;

        public bool clicking = false;
        public double clickingTime = 0;
        public float timeToRepair = 0.4f * 1000.0f;

        public string queuedAction = null;
        private Vector3 playerTarget;

        public Animator alarmAnimator;
        public Animator lifeAnimator;


        public Vector3 playerTargetOffset;

        private Generator module;
        private Generator moduleHealth;
        private Generator moduleHealthRepair;
        private Generator moduleHealthDamage;
        public string generatorName;

        // Use this for initialization
        void Start()
        {
            healthAnimator = health.gameObject.GetComponent<Animator>();
            tools.SetActive(false);
            playerTarget = transform.position + playerTargetOffset;

            AnimatorsManager.Instance.RegisterAnimator(viewAnimator);
            AnimatorsManager.Instance.RegisterAnimator(healthAnimator);
            AnimatorsManager.Instance.RegisterAnimator(alarmAnimator);
            if (lifeAnimator != null)
            {
                AnimatorsManager.Instance.RegisterAnimator(lifeAnimator);
            }

            var storage = StorageManager.Instance.GetStorage(Constants.STORAGE_MAIN);

            module = storage.GetGenerator(generatorName);
            Debug.Assert(module != null);
            module.SetAfterProductionHook(PostGenerationHook);
            module.SetBeforeProductionHook(PreGenerationHook);

            // Get health related modules
            moduleHealth = storage.GetGenerator(generatorName + Constants.MODULE_HEALTH_SUFFIX);
            moduleHealthRepair = storage.GetGenerator(generatorName + Constants.MODULE_HEALTH_REPAIR);
            moduleHealthDamage = storage.GetGenerator(generatorName + Constants.MODULE_HEALTH_DAMAGE);
            Debug.Assert(moduleHealth != null);
            Debug.Assert(moduleHealthRepair != null);
            Debug.Assert(moduleHealthDamage != null);
        }

        // Update is called once per frame
        void Update()
        {
            if (GameManager.Instance == null)
            {
                return;
            }
            //stock.text = res.amount.ToString("0.00");

            updateEfficiency();
            UpdateHealthView();

            alarmAnimator.SetFloat("health", (float)module.Amount);

            if (lifeAnimator != null)
            {
                lifeAnimator.SetBool("alert", module.Amount <= 0.0f);
            }

            // Stop repairing
            if (!Input.GetMouseButton(0))
            {
                StopAction();
            }

            if (!repairing && clicking && GameManager.Instance.CurrentTime - clickingTime > timeToRepair)
            {
                queuedAction = "repair";
            }
            
            if(queuedAction != null && (GameManager.Instance.Player.position - playerTarget).magnitude < 1.0f)
            {
                executeQueuedAction();
            }

            // Enable damage
            if(module.IsActive && !moduleHealthDamage.IsActive)
            {
                moduleHealthDamage.SetActive(true);
            }
            if (!module.IsActive && moduleHealthDamage.IsActive)
            {
                moduleHealthDamage.SetActive(false);
            }

            if (moduleHealth.Amount <= 0.0f)
            {
                moduleHealth.SetAmount(0.0f);
                SetActive(false);
            }
        }

        public void StopAction()
        {
            if (clicking && !repairing && queuedAction != "repair")
            {
                queuedAction = "toggle";
            }
            else if (queuedAction == "repair")
            {
                GameManager.Instance.SetPlayerAction(GameManager.Instance.Player.position);
                queuedAction = null;
            }
            if (repairing)
            {
                AudioManager.Instance.StopSound("moduleRepairs");
            }
            repairing = false;

            clicking = false;
            tools.SetActive(false);
            moduleHealthRepair.SetActive(false);
        }

        public void Tick()
        {

            // BREAK & REPAIR
            /*if (activated)
            {
                moduleHealth -= res.damageRate * smoothingFactor;
            }

            bool notAt100percent = (moduleHealth + gm.GameState.ductTape.efficiency * smoothingFactor) <= 100.0f;
            if (repairing && gm.GameState.ductTape.amount >= gm.GameState.ductTape.efficiency && notAt100percent)
            {
                moduleHealth += gm.GameState.ductTape.efficiency * smoothingFactor;
                gm.GameState.ductTape.amount -= (level == 1 ? 1.0f : gm.GameState.upgradeConsumptionFactor) * smoothingFactor;

                if (gm.GameState.ductTape.amount < 0.0f)
                {
                    gm.GameState.ductTape.amount = 0.0f;
                }
            } else if(repairing && gm.GameState.ductTape.amount >= gm.GameState.ductTape.efficiency && !notAt100percent)
            {
                StopAction();
            }
            */

        }

        private bool PostGenerationHook(Generator _g, double _ts, List<StreamData> _streams)
        {
            SpawnFlows(_streams, false);
            return true;
        }

        private bool PreGenerationHook(Generator _g, double _ts, List<StreamData> _streams)
        {
            SpawnFlows(_streams, true);
            return true;
        }

        private void SpawnFlows(List<StreamData> _streams, bool _fuel)
        {
            int idx = _fuel ? 1 : 0;
            foreach (var stream in _streams)
            {
                SpawnFlow(stream.m_generator, (float)stream.m_amount, idx);
            }
        }

        public void OnClick(GameObject clicked)
        {
            if(EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            if(DialogManager.Instance.IsActive())
            {
                return;
            }
            if (clicked.tag == "ModuleView")
            {
                clicking = true;
                clickingTime = GameManager.Instance.CurrentTime;
                GameManager.Instance.SetPlayerAction(playerTarget);
            }
        }

        private void UpdateHealthView()
        {
            health.localScale = new Vector3(Mathf.Clamp((float)(moduleHealth.Amount / moduleHealth.ClampMax), 0.05f, 1.0f), 1.0f, 1.0f);
            if (healthAnimator)
            {
                healthAnimator.SetFloat("health", Mathf.Ceil((float)moduleHealth.Amount));
            }
        }

        private void updateEfficiency()
        {
            // Update efficiency
            /*efficiencyModifier = GameManager.Instance.GameState.moduleHealthThresholds[GameManager.Instance.GameState.moduleHealthThresholds.Count - 1];
            for (int i = 0; i < GameManager.Instance.GameState.moduleHealthThresholds.Count; i++)
            {
                ModuleHealthThreshold thr = GameManager.Instance.GameState.moduleHealthThresholds[i];
                if (moduleHealth <= thr.threshold)
                {
                    efficiencyModifier = thr;
                    break;
                }
            }*/
            efficiencyModifier = null;
        }

        public void UpgradeModule()
        {
            throw new System.NotImplementedException();
        }

        private void executeQueuedAction()
        {
            if(queuedAction == "toggle")
            {
                SetActive(!module.IsActive);
                if (module.IsActive)
                {
                    //AudioManager.Instance.PlaySound("module" + res.name);
                } else
                {
                    AudioManager.Instance.PlaySound("moduleStopProduction");
                }

            }
            else if(queuedAction == "repair")
            {
                repairing = true;
                AudioManager.Instance.PlaySound("moduleRepairs");

                moduleHealthRepair.SetActive(true);
                tools.SetActive(true);
            }
            queuedAction = null;
        }

        public void AddHealth(float _howMuch)
        {
            moduleHealth.Add(_howMuch);
        }

        public void SetActive(bool _newValue)
        {
            viewAnimator.SetBool("activated", _newValue);
            module.SetActive(_newValue);
        }

        public void SpawnFlow(string _resourceName, float _amount, int _offset)
        {
            GameObject flowObj = Instantiate(flowPrefab, flowSpawner);
            flowObj.transform.localPosition = new Vector3(0, 0, 0);

            FlowController flow = flowObj.GetComponent<FlowController>();
            flow.Init(_resourceName, _amount, _offset);

            AnimatorsManager.Instance.RegisterAnimator(flowObj.GetComponent<Animator>());
        }
    }
}