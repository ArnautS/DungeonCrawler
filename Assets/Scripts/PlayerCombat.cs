using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    private Animator animator;

    // Update is called once per frame
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    void OnAttack()
    {
        // Play attack animation
        animator.SetTrigger("Attack");

        // Detect enemies in range

        // Apply damage
    }

}
