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

        public Transform health;    // TMP TODO
        private Text stock;

        public float moduleHealth = 100.0f;
        private ModuleHealthThreshold efficiencyModifier = null;

        private Animator animator;

        private GameManager gm;

        private GameObject healthyView;
        private GameObject brokenView;

        public float level = 1;

        public GameObject upgradeUI;
        // Use this for initialization
        void Start()
        {
            animator.SetBool("activated", activated);
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

            animator = view.GetComponent<Animator>();

            ViewScript vs = view.GetComponent<ViewScript>();
            vs.manager = this;

            transform.position = new Vector3(res.x, 0.0f, res.z);

            GameObject stockObj = GameObject.Find("/UI_prefab/MainCanvas/Resources/" + res.name + "_Stock");
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
                repairing = false;
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
                res.amount += res.efficiency * efficiencyModifier.modifier * smoothingFactor;
                fuel.amount -= 1 * smoothingFactor;
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

            if (repairing && gm.data.ductTape.amount >= gm.data.ductTape.efficiency && (moduleHealth + gm.data.ductTape.efficiency) * smoothingFactor <= 100.0f)
            {
                moduleHealth += gm.data.ductTape.efficiency * smoothingFactor;
                gm.data.ductTape.amount -= 1 * smoothingFactor;

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
                activated = !activated;
                animator.SetBool("activated", activated);
            }
            else if (clicked.tag == "HealthView")
            {
                repairing = true;
            }
            else if (clicked.name == "Upgrade")
            {
                UpgradeModule();
            }
        }

        private void updateHealthView()
        {
            health.localScale = new Vector3(1.0f, moduleHealth / 100.0f, 1.0f);
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
            if(level == 1 && !upgradeUI.activeSelf && res.amount >= gm.data.upgradeCostResource && gm.data.scrap.amount >= gm.data.upgradeCostScrap)
            {
                upgradeUI.SetActive(true);
            }
            if (level > 1 && upgradeUI.activeSelf)
            {
                upgradeUI.SetActive(false);
            }
        }

        private void UpgradeModule()
        {
            level = 2;
        }
    }
}