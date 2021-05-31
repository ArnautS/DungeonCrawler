using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float attackRate = 2f;
    [SerializeField] private int attackPower = 10;
    [SerializeField] private LayerMask enemyLayers;

    private Animator animator;
    private float nextAttackTime = 0f;


    // Update is called once per frame
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    void OnAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1f / attackRate;
            
            // Play attack animation
            animator.SetTrigger("Attack");            
        } 
    }

    // Animation event from attack animation
    void AttackHit()
    {
        // Detect enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Apply damage
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            enemy.GetComponent<EnemyHealth>().TakeDamage(attackPower);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void FlipAttackpoint()
    {
        attackPoint.transform.localPosition = new Vector2(-attackPoint.transform.localPosition.x, attackPoint.transform.localPosition.y);
    }

}
