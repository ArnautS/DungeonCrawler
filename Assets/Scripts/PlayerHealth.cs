using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
	[SerializeField] private int maxHealth = 100;
	private int health;
	private Animator animator;
	private PlayerMovement movement;

	private void Start()
    {
		health = maxHealth;
		UIManager.UpdateHealthText(health);
		animator = gameObject.GetComponent<Animator>();
		movement = gameObject.GetComponent<PlayerMovement>();
	}

    // Update is called once per frame
    void Update()
    {
		if (gameObject.transform.position.y < -6) {
			Die();
		}

    }

	void OnTriggerEnter2D(Collider2D collision)
    {
		Debug.Log($"{collision.gameObject.name} has collided with player");

		if (collision.gameObject.CompareTag("Enemy"))
		{
			TakeDamage(25, collision);
			
		}


		//foreach (ContactPoint2D contact in collision.contacts)
		//{
		//	Debug.DrawRay(contact.point, contact.normal, Color.white);
		//};


	}

	public void TakeDamage(int damage, Collider2D collision)
	{
		health -= damage;
		UIManager.UpdateHealthText(health);

		// Play damage animation
		animator.SetTrigger("Hit");

		// Add knockback to player
		Rigidbody2D enemy = collision.gameObject.GetComponent<Rigidbody2D>();
		if (enemy != null)
        {
			movement.Knockback((transform.position - enemy.transform.position).normalized);
		}
			

		// Add i-frames


		if (health <= 0)
		{
			Die();
		}
	}

	public void Die() {

		animator.SetBool("IsDead", true);
		//DataManagement.dataManagement.deathCount++;
		//Debug.Log("Current death count: " + DataManagement.dataManagement.deathCount);
		SceneManager.LoadScene("LevelCave");
		health = maxHealth;
	}

}
