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

        private Animator animator;

        private GameManager gm;

        private GameObject healthyView;
        private GameObject brokenView;
        public GameObject tools;

        public float level = 1;

        public GameObject upgradeUI;

        public bool clicking = false;
        public float clickingTime = 0;
        public float timeToRepair = 0.4f;

        public string queuedAction = null;
        private Vector3 playerTarget;

        // Use this for initialization
        void Start()
        {
            animator.SetBool("activated", activated);
            healthAnimator = health.gameObject.GetComponent<Animator>();
        }

        public void Init(int _id, ResourceModel _resource, ResourceModel _fuelResource, GameManager _manager)
        {
            Debug.Log("Init " + id + " " + _resource.name + " " + _fuelResource);
            id = _id;
            res = _resource;
            fuel = _fuelResource;
            moduleHealth = res.startHealth;

            Debug.Log(res.name + "_view_prefab");
            GameObject view = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Views/" + res.name + "_view_prefab"));
            view.transform.position = new Vector3(0, 0, 0);
            view.transform.parent = transform;
            view.name = "View";

            healthBarWrap.position = res.healthPlacement;

            foreach (Transform child in view.transform)
            {
                if (child.gameObject.tag == "ModuleBroken")
                {
                    brokenView = child.gameObject;
                }
                if (child.gameObject.tag == "ModuleHealthy")
                {
                    healthyView = child.gameObject;
                }
            }

            if (healthyView != null)
            {
                healthyView.SetActive(true);
                brokenView.SetActive(false);
            }
            tools.SetActive(false);

            animator = view.GetComponent<Animator>();

            ViewScript vs = view.GetComponent<ViewScript>();
            vs.manager = this;

            transform.position = new Vector3(res.x, 0.0f, res.z);
            playerTarget = transform.position + res.playerTarget;

            GameObject stockObj = GameObject.Find("/UI_prefab/MainCanvas/Resources/BackgroundOrange/" + res.name + "/"+ res.name+"_Stock");
            stock = stockObj.GetComponent<Text>();

            // Upgrade UI
            upgradeUI.transform.localPosition = res.upgradePlacement;
            upgradeUI.SetActive(false);
            foreach (Transform child in upgradeUI.transform)
            {
                if (child.gameObject.name == res.name + "_logo")
                {
                    child.gameObject.SetActive(true);
                }

                if (child.gameObject.name == "scrap_amount")
                {
                    child.gameObject.GetComponent<TextMesh>().text = "" + _manager.data.upgradeCostScrap;
                }
                if (child.gameObject.name == "resource_amount")
                {
                    child.gameObject.GetComponent<TextMesh>().text = "" + _manager.data.upgradeCostResource;
                }
            }


            gm = _manager;
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

            // Stop repairing
            if (!Input.GetMouseButton(0))
            {
                if (clicking && !repairing && queuedAction != "repair")
                {
                    queuedAction = "toggle";
                } else if(queuedAction == "repair")
                {
                    gm.SetPlayerAction(gm.player.position);
                    queuedAction = null;
                }
                repairing = false;
                clicking = false;
                tools.SetActive(false);
            }

            if (clicking && gm.timer - clickingTime > timeToRepair)
            {
                queuedAction = "repair";
            }

            if(queuedAction != null && (gm.player.position - playerTarget).magnitude < 1.0f)
            {
                executeQueuedAction();
            }
        }

        public void Tick()
        {
            float smoothingFactor = 1.0f / (1.0f * gm.data.clockSmoothing);

            // deactivate if health is 0
            if (moduleHealth <= 0.0f)
            {
                moduleHealth = 0.0f;
                activated = false;
                animator.SetBool("activated", activated);
            }

            // PRODUCTION
            if (activated && fuel.amount > 0)
            {
                res.amount += res.efficiency * efficiencyModifier.modifier * smoothingFactor * (level == 1 ? 1.0f : gm.data.upgradeEfficiencyFactor);
                fuel.amount -= (level == 1 ? 1.0f : gm.data.upgradeConsumptionFactor) * smoothingFactor;
            }

            // DECAY
            res.amount -= res.decay * smoothingFactor;

            if (res.amount < 0.0f)
            {
                res.amount = 0.0f;
            }

            // BREAK & REPAIR
            if (activated)
            {
                moduleHealth -= res.damageRate * smoothingFactor;
            }

            if (repairing && gm.data.ductTape.amount >= gm.data.ductTape.efficiency && (moduleHealth + gm.data.ductTape.efficiency * smoothingFactor) <= 100.0f)
            {
                moduleHealth += gm.data.ductTape.efficiency * smoothingFactor;
                gm.data.ductTape.amount -= (level == 1 ? 1.0f : gm.data.upgradeConsumptionFactor) * smoothingFactor;

                if (gm.data.ductTape.amount < 0.0f)
                {
                    gm.data.ductTape.amount = 0.0f;
                }
            }
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
            if (healthyView != null)
            {
                float healhyBrokenThreshold = gm.data.moduleHealthThresholds[2].threshold;  // 60%
                if (moduleHealth < healhyBrokenThreshold && healthyView.activeSelf)
                {
                    healthyView.SetActive(false);
                    brokenView.SetActive(true);
                }
                if (moduleHealth > healhyBrokenThreshold && !healthyView.activeSelf)
                {
                    healthyView.SetActive(true);
                    brokenView.SetActive(false);
                }
            }
        }

        private void updateEfficiency()
        {
            // Update efficiency
            efficiencyModifier = gm.data.moduleHealthThresholds[gm.data.moduleHealthThresholds.Length - 1];
            for (int i = 0; i < gm.data.moduleHealthThresholds.Length; i++)
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
            level = 2;
        }

        private void executeQueuedAction()
        {
            if(queuedAction == "toggle")
            {
                activated = !activated;
                animator.SetBool("activated", activated);
            }
            else
            {
                repairing = true;
                tools.SetActive(true);
            }
            queuedAction = null;
        }
    }
}