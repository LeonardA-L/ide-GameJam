using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFrenzy
{
    public class ModuleManager : MonoBehaviour
    {
        public int id = 0;

        public ResourceModel res;
        public ResourceModel fuel;

        public bool activated = false;
        public bool repairing = false;

        public Transform health;
        public Transform healthBarWrap;
        public Animator healthAnimator;
        private Text stock;

        public float moduleHealth = 100.0f;
        private ModuleHealthThreshold efficiencyModifier = null;

        public Animator viewAnimator;

        private GameManager gm;
        
        public GameObject tools;
        public Transform flowSpawner;
        public GameObject flowPrefab;

        public float level = 1;

        public GameObject upgradeUI;

        public bool clicking = false;
        public float clickingTime = 0;
        public float timeToRepair = 0.4f;

        public string queuedAction = null;
        private Vector3 playerTarget;

        public Animator alarmAnimator;

        // Use this for initialization
        void Start()
        {
            SetActive(false);
            healthAnimator = health.gameObject.GetComponent<Animator>();
        }

        public void Init(int _id, ResourceModel _resource, ResourceModel _fuelResource)
        {
            Debug.Log("Init " + id + " " + _resource.name + " " + _fuelResource);
            gm = GameManager.Instance;
            id = _id;
            res = _resource;
            fuel = _fuelResource;
            moduleHealth = res.startHealth;
            
            tools.SetActive(false);

            gm.RegisterAnimator(viewAnimator);
            gm.RegisterAnimator(healthAnimator);
            gm.RegisterAnimator(alarmAnimator);
            
            playerTarget = transform.position + res.playerTarget;

            GameObject stockObj = GameObject.Find("/UI_prefab/MainCanvas/Resources/BackgroundOrange/" + res.name + "/"+ res.name+"_Stock");
            stock = stockObj.GetComponent<Text>();

            // Upgrade UI
            upgradeUI.SetActive(false);
            foreach (Transform child in upgradeUI.transform)
            {
                if (child.gameObject.name == res.name + "_logo")
                {
                    child.gameObject.SetActive(true);
                }

                if (child.gameObject.name == "scrap_amount")
                {
                    child.gameObject.GetComponent<TextMesh>().text = "" + gm.data.upgradeCostScrap;
                }
                if (child.gameObject.name == "resource_amount")
                {
                    child.gameObject.GetComponent<TextMesh>().text = "" + gm.data.upgradeCostResource;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (gm == null)
            {
                return;
            }
            stock.text = res.amount.ToString("0.00");

            updateEfficiency();
            updateHealthView();
            updateUpgradeUI();

            alarmAnimator.SetFloat("health", res.amount);

            // Stop repairing
            if (!Input.GetMouseButton(0))
            {
                StopAction();
            }

            if (!repairing && clicking && gm.timer - clickingTime > timeToRepair)
            {
                queuedAction = "repair";
            }

            if(queuedAction != null && (gm.player.position - playerTarget).magnitude < 1.0f)
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
                gm.SetPlayerAction(gm.player.position);
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
            float smoothingFactor = 1.0f / (1.0f * gm.data.clockSmoothing);

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

            // PRODUCTION
            if (activated && fuel.amount > 0)
            {
                resDiff = res.efficiency * efficiencyModifier.modifier * smoothingFactor * (level == 1 ? 1.0f : gm.data.upgradeEfficiencyFactor);
                fuelDiff = (level == 1 ? 1.0f : gm.data.upgradeConsumptionFactor) * smoothingFactor;

                res.amount += resDiff;
                totalResDiff += resDiff;
                fuel.amount -= fuelDiff;
                totalFuelDiff -= fuelDiff;
            }

            // DECAY
            resDiff = res.decay * smoothingFactor;
            res.amount -= resDiff;
            //totalResDiff -= resDiff;

            if (res.amount < 0.0f)
            {
                res.amount = 0.0f;
            }

            // BREAK & REPAIR
            if (activated)
            {
                moduleHealth -= res.damageRate * smoothingFactor;
            }

            bool notAt100percent = (moduleHealth + gm.data.ductTape.efficiency * smoothingFactor) <= 100.0f;
            if (repairing && gm.data.ductTape.amount >= gm.data.ductTape.efficiency && notAt100percent)
            {
                moduleHealth += gm.data.ductTape.efficiency * smoothingFactor;
                gm.data.ductTape.amount -= (level == 1 ? 1.0f : gm.data.upgradeConsumptionFactor) * smoothingFactor;

                if (gm.data.ductTape.amount < 0.0f)
                {
                    gm.data.ductTape.amount = 0.0f;
                }
            } else if(repairing && gm.data.ductTape.amount >= gm.data.ductTape.efficiency && !notAt100percent)
            {
                StopAction();
            }

            // Show flows
            if(Mathf.Abs(totalResDiff) >= 0.1f)
                SpawnFlow(res.name, totalResDiff, 0);
            if (Mathf.Abs(totalFuelDiff) >= 0.1f)
                SpawnFlow(fuel.name, totalFuelDiff, 1);

        }

        public void OnClick(GameObject clicked)
        {
            if(DialogManager.Instance.IsActive())
            {
                return;
            }
            if (clicked.tag == "ModuleView")
            {
                clicking = true;
                clickingTime = gm.timer;
                gm.SetPlayerAction(playerTarget);
            }
            else if (clicked.name == "Upgrade")
            {
                UpgradeModule();
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
            efficiencyModifier = gm.data.moduleHealthThresholds[gm.data.moduleHealthThresholds.Count - 1];
            for (int i = 0; i < gm.data.moduleHealthThresholds.Count; i++)
            {
                ModuleHealthThreshold thr = gm.data.moduleHealthThresholds[i];
                if (moduleHealth <= thr.threshold)
                {
                    efficiencyModifier = thr;
                    break;
                }
            }
        }

        private void updateUpgradeUI()
        {
            if(upgradeUI.activeSelf)
            {
                if(level > 1)
                {
                    upgradeUI.SetActive(false);
                }
                if(res.amount < gm.data.upgradeCostResource || gm.data.scrap.amount < gm.data.upgradeCostScrap)
                {
                    upgradeUI.SetActive(false);
                }
            }
            else {
                if (level == 1 && res.amount >= gm.data.upgradeCostResource && gm.data.scrap.amount >= gm.data.upgradeCostScrap)
                {
                    upgradeUI.SetActive(true);
                }
            }
        }

        private void UpgradeModule()
        {
            res.amount -= gm.data.upgradeCostResource;
            gm.data.scrap.amount -= gm.data.upgradeCostScrap;
            level = 2;
            AudioManager.Instance.PlaySound("moduleUpdate");

        }

        private void executeQueuedAction()
        {
            if(queuedAction == "toggle")
            {
                SetActive(!activated);
                if (activated)
                {
                    AudioManager.Instance.PlaySound("module" + res.name);
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
            activated = _newValue;
            viewAnimator.SetBool("activated", activated);
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