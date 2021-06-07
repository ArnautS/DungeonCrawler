using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float attackRate = 2f;
    [SerializeField] private int attackPower = 10;
    [SerializeField] private LayerMask enemyLayers;

    [SerializeField] PolygonCollider2D swordHitbox;

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
        List<Collider2D> hitEnemies = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayers);        
        swordHitbox.OverlapCollider(filter, hitEnemies);

        // Apply damage
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            enemy.GetComponent<EnemyHealth>().TakeDamage(attackPower);
        }
    }

    public void FlipSwordHitbox()
    {
        swordHitbox.transform.localScale = new Vector2(-swordHitbox.transform.localScale.x, swordHitbox.transform.localScale.y);
    }

}
