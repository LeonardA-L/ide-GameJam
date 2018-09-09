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
        public Transform healthBarWrap;
        public Animator healthAnimator;
        private Text stock;

        public float moduleHealth = 100.0f;
        private ModuleHealthThreshold efficiencyModifier = null;

        public Animator viewAnimator;

        public GameObject tools;
        public Transform flowSpawner;
        public GameObject flowPrefab;

        public float level = 1;

        public bool clicking = false;
        public float clickingTime = 0;
        public float timeToRepair = 0.4f;

        public string queuedAction = null;
        private Vector3 playerTarget;

        public Animator alarmAnimator;
        public Animator lifeAnimator;


        public Vector3 playerTargetOffset;

        private Generator module;
        public string generatorName;

        // Use this for initialization
        void Start()
        {
            healthAnimator = health.gameObject.GetComponent<Animator>();
            tools.SetActive(false);
            playerTarget = transform.position + playerTargetOffset;

            GameManager.Instance.RegisterAnimator(viewAnimator);
            GameManager.Instance.RegisterAnimator(healthAnimator);
            GameManager.Instance.RegisterAnimator(alarmAnimator);
            if (lifeAnimator != null)
            {
                GameManager.Instance.RegisterAnimator(lifeAnimator);
            }

            module = StorageManager.Instance.GetStorage(Constants.STORAGE_MAIN).GetGenerator(generatorName);
            Debug.Assert(module != null);
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
            updateHealthView();

            //alarmAnimator.SetFloat("health", res.amount);

            if (lifeAnimator != null)
            {
                //lifeAnimator.SetBool("alert", res.amount <= 0.0f);
            }

            // Stop repairing
            if (!Input.GetMouseButton(0))
            {
                StopAction();
            }

            if (!repairing && clicking && GameManager.Instance.timer - clickingTime > timeToRepair)
            {
                queuedAction = "repair";
            }

            if(queuedAction != null && (GameManager.Instance.player.position - playerTarget).magnitude < 1.0f)
            {
                executeQueuedAction();
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
                GameManager.Instance.SetPlayerAction(GameManager.Instance.player.position);
                queuedAction = null;
            }
            if (repairing)
            {
                AudioManager.Instance.StopSound("moduleRepairs");
            }
            repairing = false;

            clicking = false;
            tools.SetActive(false);
        }

        public void Tick()
        {
            float smoothingFactor = 1.0f / (1.0f * GameManager.Instance.GameState.clockSmoothing);

            float totalResDiff = 0.0f;
            float totalFuelDiff = 0.0f;
            float resDiff;
            float fuelDiff;

            // deactivate if health is 0
            if (moduleHealth <= 0.0f)
            {
                moduleHealth = 0.0f;
                SetActive(false);
            }

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

            // Show flows
            /*
            if(Mathf.Abs(totalResDiff) >= 0.1f)
                SpawnFlow(res.name, totalResDiff, 0);
            if (Mathf.Abs(totalFuelDiff) >= 0.1f)
                SpawnFlow(fuel.name, totalFuelDiff, 1);
                */

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
                clickingTime = GameManager.Instance.timer;
                GameManager.Instance.SetPlayerAction(playerTarget);
            }
        }

        private void updateHealthView()
        {
            health.localScale = new Vector3(Mathf.Clamp(moduleHealth / 100.0f, 0.05f, 1.0f), 1.0f, 1.0f);
            if (healthAnimator)
            {
                healthAnimator.SetFloat("health", moduleHealth);
            }
        }

        private void updateEfficiency()
        {
            // Update efficiency
            efficiencyModifier = GameManager.Instance.GameState.moduleHealthThresholds[GameManager.Instance.GameState.moduleHealthThresholds.Count - 1];
            for (int i = 0; i < GameManager.Instance.GameState.moduleHealthThresholds.Count; i++)
            {
                ModuleHealthThreshold thr = GameManager.Instance.GameState.moduleHealthThresholds[i];
                if (moduleHealth <= thr.threshold)
                {
                    efficiencyModifier = thr;
                    break;
                }
            }
        }

        private float computeUpgradeCost(float _ratio, float _starter)
        {
            return Mathf.Floor(_starter * Mathf.Pow(_ratio, level - 1));
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

                tools.SetActive(true);
            }
            queuedAction = null;
        }

        public void AddHealth(float _howMuch)
        {
            moduleHealth += _howMuch;
            moduleHealth = Mathf.Clamp(moduleHealth, 0.0f, 100.0f);
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

            GameManager.Instance.RegisterAnimator(flowObj.GetComponent<Animator>());
        }
    }
}