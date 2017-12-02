using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFrenzy
{
    public class CameraController : MonoBehaviour
    {
        public Transform character;
        private Vector3 positionOffset;

        public bool lockX = true;
        public bool lockZ = true;
        public bool active = false;

        public Vector3 disp;

        // Use this for initialization
        void Start()
        {
            positionOffset = transform.position - character.position;
            active = true;

            lockX = true;
            lockZ = true;
        }

        // Update is called once per frame
        void Update()
        {
            disp = (character.position + positionOffset) - transform.position;

            disp = transform.InverseTransformVector(disp);
            disp.x = lockX ? 0 : disp.x;
            disp.z = lockZ ? 0 : disp.z;
            disp.y = lockZ ? 0 : disp.y;

            transform.localPosition += disp;
        }
    }
}