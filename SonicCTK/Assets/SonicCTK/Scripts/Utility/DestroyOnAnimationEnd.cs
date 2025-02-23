using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SonicCTK.Core
{
    public class DestroyOnAnimationEnd : MonoBehaviour
    {
        private Animator animator;

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) 
                Destroy(gameObject); 
        }
    }

}