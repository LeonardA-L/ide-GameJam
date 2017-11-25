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
                Debug.Log("Touch");
                gm.CollectCrate(this);
                Destroy(gameObject);
            }
        }

        void OnMouseDown()
        {
            Debug.Log("Touch2");
            GameManager.Instance.SetPlayerAction(transform.position);
        }
    }
}