using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

	[SerializeField] private int maxHealth;

	private int health;
	private Animator animator;
	private EnemyMovement movement;

	void Start()
    {
		health = maxHealth;
		animator = gameObject.GetComponent<Animator>();
		movement = GetComponent<EnemyMovement>();
	}

    

	public void TakeDamage(int damage)
    {
		health -= damage;

		// Play damage animation
		animator.SetTrigger("Hit");
		movement.Knockback();

		if(health <= 0)
        {
			Die();
        }
    }

	public void Die() {
		// Play dying animation
		animator.SetBool("IsDead", true);

		GetComponent<Rigidbody2D>().simulated = false;
		GetComponent<Collider2D>().enabled = false;
		GetComponent<EnemyMovement>().enabled = false;
		this.enabled = false;
		//Destroy(gameObject);
	}
}
