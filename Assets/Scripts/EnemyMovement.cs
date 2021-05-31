using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

	[SerializeField] private int EnemySpeed = 1;
	[SerializeField] private int XMoveDirection = 1;
	[SerializeField] private LayerMask whatIsGround;
	[SerializeField] private float knockbackPower = 2.0f;
	[SerializeField] private float detectionRange = 0.05f;

	private float collisionRange;

	private bool isKnockedBack = false;
	private bool isColliding = false;
	
	private Rigidbody2D rb;
	private CapsuleCollider2D cc;
	private SpriteRenderer sr;
	private PlayerMovement pm;
	

    // Start is called before the first frame update
    void Start() 
	{
		rb = gameObject.GetComponent<Rigidbody2D>();
		cc = gameObject.GetComponent<CapsuleCollider2D>();
		sr = gameObject.GetComponent<SpriteRenderer>();
		pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

		collisionRange = cc.size.x / 2 + 0.001f;
	}

    // Update is called once per frame
    void Update() 
	{	
		if (!isKnockedBack)
        {
			rb.velocity = new Vector2(XMoveDirection * EnemySpeed, rb.velocity.y);
		}
		else if (rb.velocity.x == 0)
        {
			isKnockedBack = false;
        }

		
		RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + XMoveDirection * collisionRange + cc.offset.x, transform.position.y + cc.offset.y), new Vector2(XMoveDirection, 0), detectionRange, whatIsGround);
		Debug.DrawRay(transform.position, hit.normal, Color.red);
		

		if (hit)
		{
			ReverseMovement();
        }
    }

    private void ReverseMovement()
    {
		XMoveDirection = -XMoveDirection;
		sr.flipX = !sr.flipX;
		cc.offset = new Vector2(-cc.offset.x, cc.offset.y);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		//if (isColliding) return;
		isColliding = true;
		if (collision.gameObject.CompareTag("Enemy"))
		{
			ReverseMovement();
			Debug.Log("Reversing movement");
		}
		StartCoroutine(Reset());
	}

	
	IEnumerator Reset()
	{
		yield return new WaitForEndOfFrame();
		isColliding = false;
	}
	public void Knockback()
    {
		rb.velocity = Vector2.zero;
		rb.AddForce(new Vector2(pm.PlayerDirection() * knockbackPower, -rb.velocity.y), ForceMode2D.Impulse); ;
		isKnockedBack = true;
    }



	private void OnDrawGizmosSelected()
	{
		if (cc == null) return;

		Gizmos.DrawRay(new Vector2(transform.position.x + XMoveDirection * collisionRange + cc.offset.x, transform.position.y + cc.offset.y), new Vector2(XMoveDirection, 0));
	}




}
