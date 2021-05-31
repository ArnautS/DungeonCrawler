using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
	[SerializeField] private int maxHealth = 100;
	[SerializeField] private Color flashColor;
	[SerializeField] private Color regularColor;
	[SerializeField] private float flashDuration;
	[SerializeField] private int numberOfFlashes;
	[SerializeField] private Collider2D triggerCollider;
	[SerializeField] private float deathDuration = 2;


	private int health;
	private bool isDead = false;
	private bool isColliding;
	private Animator animator;
	private PlayerMovement movement;
	private SpriteRenderer sr;

	private void Start()
    {
		health = maxHealth;
		UIManager.UpdateHealthText(health);
		animator = gameObject.GetComponent<Animator>();
		movement = gameObject.GetComponent<PlayerMovement>();
		sr = gameObject.GetComponent<SpriteRenderer>();
	}

    // Update is called once per frame
    void Update()
    {
		if (gameObject.transform.position.y < -100) {
			Die();
		}

    }

	void OnTriggerEnter2D(Collider2D collision)
    {
		if (isColliding) return;
		isColliding = true;
		Debug.Log($"{collision.gameObject.name} has collided with player");

		if (collision.gameObject.CompareTag("Enemy"))
		{
			TakeDamage(25, collision);
			
		}
		StartCoroutine(Reset());
	}
	IEnumerator Reset()
	{
		yield return new WaitForEndOfFrame();
		isColliding = false;
	}

	public void TakeDamage(int damage, Collider2D collision)
	{
		health -= damage;
		UIManager.UpdateHealthText(health);
		triggerCollider.enabled = false;

		// Play damage animation
		animator.SetTrigger("Hit");

		// Add knockback to player
		Rigidbody2D enemy = collision.gameObject.GetComponent<Rigidbody2D>();
		if (enemy != null)
        {
			movement.Knockback((transform.position - enemy.transform.position).normalized);
		}		

		if (health <= 0 && !isDead)
		{
			Die();
		}
		else
        {
			// Add i-frames
			StartCoroutine(FlashCo());
		}
	}

	private IEnumerator FlashCo()
    {
		int counter = 0;
		while(counter < numberOfFlashes)
        {
			sr.color = flashColor;
			yield return new WaitForSeconds(flashDuration);
			sr.color = regularColor;
			yield return new WaitForSeconds(flashDuration);
			counter++;
        }
		triggerCollider.enabled = true;
    }

	private void Die() {
		isDead = true;
		animator.SetBool("IsDead", true);
		GetComponent<Rigidbody2D>().simulated = false;
		GetComponent<Collider2D>().enabled = false;
		GetComponent<PlayerMovement>().enabled = false;
		StartCoroutine(RespawnCo());		
	}

	private IEnumerator RespawnCo()
    {
		yield return new WaitForSeconds(deathDuration);
		Destroy(gameObject);

		LevelManager.instance.Respawn();
	}

}
