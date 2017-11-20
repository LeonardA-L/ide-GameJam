using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFrenzy
{
    public class GameManager : MonoBehaviour
    {
        protected static readonly GameManager instance = new GameManager();

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public static GameManager Instance
        {
            get
            {
                return instance;
            }
        }
    }

}