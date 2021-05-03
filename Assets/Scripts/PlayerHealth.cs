using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
	[SerializeField] private int maxHealth = 100;
	private int health;
	private Animator animator;

	private void Start()
    {
		health = maxHealth;
		UIManager.UpdateHealthText(health);
		animator = gameObject.GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update()
    {
		if (gameObject.transform.position.y < -6) {
			Die();
		}

    }

	void OnCollisionEnter2D(Collision2D collision)
	{

		if (collision.gameObject.CompareTag("Enemy"))
		{
			TakeDamage(25);
			UIManager.UpdateHealthText(health);
		}


		foreach (ContactPoint2D contact in collision.contacts)
		{
			Debug.DrawRay(contact.point, contact.normal, Color.white);
		};


	}

	public void TakeDamage(int damage)
	{
		health -= damage;

		// Play damage animation
		animator.SetTrigger("Hit");


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
