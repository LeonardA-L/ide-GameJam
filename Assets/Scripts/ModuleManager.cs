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
        private float efficiencyModifier = 1;

        private GameManager gm;
        // Use this for initialization
        void Start()
        {
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

            ViewScript vs = view.GetComponent<ViewScript>();
            vs.manager = this;

            transform.position = new Vector3(res.x, 0.0f, res.z);

            GameObject stockObj = GameObject.Find("/UI_prefab/MainCanvas/Resources/" + res.name + "_Stock");
            stock = stockObj.GetComponent<Text>();

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

            health.localScale = new Vector3(1.0f, moduleHealth / 100.0f, 1.0f);

            // Update efficiency
            efficiencyModifier = 1.0f;
            for (int i = 0; i < gm.data.moduleHealthThresholds.Length; i++)
            {
                ModuleHealthThreshold thr = gm.data.moduleHealthThresholds[i];
                if (moduleHealth <= thr.threshold)
                {
                    efficiencyModifier = thr.modifier;
                    break;
                }
            }

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
            }

            // PRODUCTION
            if (activated && fuel.amount > 0)
            {
                res.amount += res.efficiency * efficiencyModifier * smoothingFactor;
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
            if (clicked.tag == "ModuleView")
            {
                activated = !activated;
            }
            else if (clicked.tag == "HealthView")
            {
                repairing = true;
            }
        }
    }
}