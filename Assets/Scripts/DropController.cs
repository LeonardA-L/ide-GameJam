using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFrenzy
{
    public class DropController : MonoBehaviour
    {
        GameManager gm;
        public float water;
        public float potatoes;
        public float electricity;
        public float scrap;
        public float ductTape;
        public int slot;

        public GameObject flowPrefab;

        public void SetValues(float _water, float _potatoes, float _electricity, float _scrap, float _ductTape, int _slot)
        {
            water = _water;
            potatoes = _potatoes;
            electricity = _electricity;
            scrap = _scrap;
            ductTape = _ductTape;
            slot = _slot;
        }

        // Update is called once per frame
        void Update()
        {

            if(gm == null)
            {
                gm = GameManager.Instance;
                return;
            }

            if((gm.player.position - transform.position).magnitude < 1.0f)
            {
                Collect();
            }
        }

        void OnMouseDown()
        {
            GameManager.Instance.SetPlayerAction(transform.position);
        }

        private void Collect()
        {
            gm.CollectCrate(this);

            /*water = _water;
            potatoes = _potatoes;
            electricity = _electricity;
            scrap = _scrap;
            ductTape = _ductTape;*/

            int offset = 0;
            if(water != 0.0f)
            {
                SpawnFlow("water", water, offset);
                offset--;
            }

            if (potatoes != 0.0f)
            {
                SpawnFlow("potatoes", potatoes, offset);
                offset--;
            }

            if (electricity != 0.0f)
            {
                SpawnFlow("electricity", electricity, offset);
                offset--;
            }

            if (scrap != 0.0f)
            {
                SpawnFlow("scrap", scrap, offset);
                offset--;
            }

            if (ductTape != 0.0f)
            {
                SpawnFlow("ductTape", ductTape, offset);
                offset--;
            }

            Destroy(gameObject);
        }

        public void SpawnFlow(string _resourceName, float _amount, int _offset)
        {
            GameObject flowObj = Instantiate(flowPrefab, GameManager.Instance.gameObject.transform);
            flowObj.transform.localPosition =  transform.position + new Vector3(-0.75f, 2.0f, 0);

            FlowController flow = flowObj.GetComponent<FlowController>();
            flow.Init(_resourceName, _amount, _offset);

            GameManager.Instance.RegisterAnimator(flowObj.GetComponent<Animator>());
        }
    }
}