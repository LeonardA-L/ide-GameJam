using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFrenzy
{
    public class ViewScript : MonoBehaviour
    {

        public ModuleManager manager;

        void OnMouseDown()
        {
            manager.OnClick(gameObject);
        }
    }
}