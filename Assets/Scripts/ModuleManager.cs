﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

        public bool clicking = false;
        public float clickingTime = 0;
        public float timeToRepair = 0.4f;

        public string queuedAction = null;
        private Vector3 playerTarget;

        public Animator alarmAnimator;
        public Animator lifeAnimator;

        // Upgrade UI elements
        public Text upgrLevel;
        public Text upgrResCost;
        public Text upgrScrapCost;
        public Button upgrButton;
        // Static upgrade UI elements
        public Text upgrTitle;
        public Text upgrDescription;
        public Text upgrBtnText;


        public static I18n i18n;

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

            i18n = I18n.Instance;

            tools.SetActive(false);

            gm.RegisterAnimator(viewAnimator);
            gm.RegisterAnimator(healthAnimator);
            gm.RegisterAnimator(alarmAnimator);
            if (lifeAnimator != null)
            {
                gm.RegisterAnimator(lifeAnimator);
            }

            playerTarget = transform.position + res.playerTarget;

            GameObject stockObj = GameObject.Find("/UI_prefab/MainCanvas/Resources/BackgroundOrange/" + res.name + "/"+ res.name+"_Stock");
            stock = stockObj.GetComponent<Text>();

            initUpgradeUI();
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

            if (lifeAnimator != null)
            {
                lifeAnimator.SetBool("alert", res.amount <= 0.0f);
            }

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
            float smoothingFactor = 1.0f / (1.0f * gm.GameState.clockSmoothing);

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
                resDiff = res.efficiency * efficiencyModifier.modifier * smoothingFactor * Mathf.Pow(gm.GameState.upgradeEfficiencyFactor, level - 1);
                fuelDiff = Mathf.Pow(gm.GameState.upgradeConsumptionFactor, level - 1) * smoothingFactor;

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

            // Show flows
            if(Mathf.Abs(totalResDiff) >= 0.1f)
                SpawnFlow(res.name, totalResDiff, 0);
            if (Mathf.Abs(totalFuelDiff) >= 0.1f)
                SpawnFlow(fuel.name, totalFuelDiff, 1);

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
                clickingTime = gm.timer;
                gm.SetPlayerAction(playerTarget);
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
            efficiencyModifier = gm.GameState.moduleHealthThresholds[gm.GameState.moduleHealthThresholds.Count - 1];
            for (int i = 0; i < gm.GameState.moduleHealthThresholds.Count; i++)
            {
                ModuleHealthThreshold thr = gm.GameState.moduleHealthThresholds[i];
                if (moduleHealth <= thr.threshold)
                {
                    efficiencyModifier = thr;
                    break;
                }
            }
        }

        private void updateUpgradeUI()
        {
            if (upgrButton == null)
                return;
            upgrButton.interactable = hasEnoughForUpgrade();
        }

        private void initUpgradeUI()
        {
            if (upgrLevel == null)
                return;
            upgrLevel.text = i18n.__("UpgradeLevel") + " " + level;

            upgrTitle.text = i18n.__("UpgradeTitle" + res.name);
            upgrDescription.text = i18n.__("UpgradeDesc" + res.name);
            upgrBtnText.text = i18n.__("UpgradeBtn");

            upgrResCost.text = "" + computeUpgradeCost(res.upgradeResCostRatio, res.upgradeResCostStarter);
            upgrScrapCost.text = "" + computeUpgradeCost(res.upgradeScrapCostRatio, res.upgradeScrapCostStarter);
        }

        private bool hasEnoughForUpgrade()
        {
            return res.amount >= computeUpgradeCost(res.upgradeResCostRatio, res.upgradeResCostStarter)
                && gm.GameState.scrap.amount >= computeUpgradeCost(res.upgradeScrapCostRatio, res.upgradeScrapCostStarter);
        }

        private float computeUpgradeCost(float _ratio, float _starter)
        {
            return Mathf.Floor(_starter * Mathf.Pow(_ratio, level - 1));
        }

        public void UpgradeModule()
        {
            if (!hasEnoughForUpgrade())
                return;
            res.amount -= computeUpgradeCost(res.upgradeResCostRatio, res.upgradeResCostStarter);
            gm.GameState.scrap.amount -= computeUpgradeCost(res.upgradeScrapCostRatio, res.upgradeScrapCostStarter);
            level++;
            initUpgradeUI();
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