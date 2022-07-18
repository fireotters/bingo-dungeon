using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PauseMenuAnimatorController : MonoBehaviour
{
    [SerializeField] Animator animator;
    void OnEnable()
    {
        animator.SetBool("open", true);
    }

    void OnDisable()
    {
        animator.SetBool("open", false);
    }
}
