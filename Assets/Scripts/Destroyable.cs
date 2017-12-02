using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFrenzy
{
    public class Destroyable : MonoBehaviour {

        public void DestroyMe()
        {
            Animator animator = gameObject.GetComponent<Animator>();
            if (animator != null) {
                GameManager.Instance.UnregisterAnimator(animator);
            }
            Destroy(gameObject);
        }
    }

}