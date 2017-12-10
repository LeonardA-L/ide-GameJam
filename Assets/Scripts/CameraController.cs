using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFrenzy
{
    public enum CameraMode { EXPLORE, BASE };
    public class CameraController : MonoBehaviour
    {
        protected static CameraController instance;
        public Transform character;
        private Vector3 positionOffset;
        private Camera cam;

        public bool lockX = true;
        public bool lockZ = true;
        public bool active = false;

        public Vector3 disp;

        public CameraMode mode = CameraMode.EXPLORE;
        private Vector3 basePosition = new Vector3(6.33f, 26.14f, -30.92f);
        private Vector3 goal;


        // Use this for initialization
        void Start()
        {
            instance = this;
            cam = GetComponent<Camera>();
            positionOffset = transform.position - character.position;
            active = true;

            lockX = false;
            lockZ = true;
            goal = transform.position;
        }

        public static CameraController Instance
        {
            get
            {
                return instance;
            }
        }

        // Update is called once per frame
        void Update()
        {

            switch (mode)
            {
                case CameraMode.BASE:
                    goal = basePosition;
                    break;
                case CameraMode.EXPLORE:
                default:
                    goal = (character.position + positionOffset);
                    break;
            }


            disp = goal - transform.position;

            disp = transform.InverseTransformVector(disp);
            disp.x = lockX ? 0 : disp.x;
            disp.z = lockZ ? 0 : disp.z;
            disp.y = lockZ ? 0 : disp.y;

            transform.localPosition += Vector3.Lerp(Vector3.zero, disp, 0.06f);
        }

        public void SetModeBase()
        {
            mode = CameraMode.BASE;
        }

        public void SetModeExplore()
        {
            mode = CameraMode.EXPLORE;
            lockX = false;
            lockZ = false;
        }


    }
}