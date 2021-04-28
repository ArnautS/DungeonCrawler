using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // serializable variables
	[SerializeField] private int playerJumpPower;
	[SerializeField] private int playerSpeed;

	[SerializeField] private Transform groundCheck;
	[SerializeField] private float groundCheckRadius;
	[SerializeField] private LayerMask whatIsGround;
	[SerializeField] private float slopeCheckDistance;
	[SerializeField] private float maxSlopeAngle;

	[SerializeField] private PhysicsMaterial2D noFriction;
	[SerializeField] private PhysicsMaterial2D fullFriction;

	// private variables
	private float moveX;
	private float velocityOffset = 0.01f;
	private float slopeDownAngle;
	private float slopeDownAngleOld;
	private float slopeSideAngle;

	private int slopeDirection;
	
	private bool isGrounded;
	private bool isJumping;
	private bool canJump;
	private bool isOnSlope;
	private bool canWalkOnSlope;

	private Vector2 velocity;
	private Vector2 colliderSize;
	private Vector2 colliderOffset;
	private Vector2 slopeNormalPerp;
	private Vector2 slopeSideNormalPerp;


	// components
	private Rigidbody2D rb;
	private SpriteRenderer sr;
	private CapsuleCollider2D cc;

	// Start is called before the first frame update
	void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
		cc = gameObject.GetComponent<CapsuleCollider2D>();

		colliderSize = cc.size;
		colliderOffset = cc.offset;

		velocity = rb.velocity;
    }

    // Update is called once per frame
    private void Update()
    {
		//Debug print
		UIManager.UpdateDebugText(isOnSlope.ToString());

		moveX = Input.GetAxisRaw("Horizontal");

		if (Input.GetButtonDown("Jump"))
		{
			Jump(playerJumpPower);
		}


	}

    void FixedUpdate()
    {
		CheckGround();
		CheckSlope();
		PlayerMove();
    }

	void PlayerMove()
	{

		//set player direction
		if (moveX < 0)
		{
			sr.flipX = true;
		}
		else if (moveX > 0)
		{
			sr.flipX = false;
		}

		//set player velocity		
		if (isGrounded && !isOnSlope && !isJumping)
		{
			velocity.Set(moveX * playerSpeed * Time.fixedDeltaTime, 0.0f);
			rb.velocity = velocity;
			Debug.Log($"Applying normal speed, x={rb.velocity.x}, y={rb.velocity.y}, total={rb.velocity.magnitude}, time={Time.fixedDeltaTime}");
		}
		else if (isGrounded && isOnSlope && !isJumping && canWalkOnSlope)
		{
			
			if ( moveX == slopeDirection)//slopeNormalPerp == Vector2.left &&
			{
				velocity.Set(-moveX * playerSpeed * slopeSideNormalPerp.x * Time.fixedDeltaTime, -moveX * playerSpeed * slopeSideNormalPerp.y * Time.fixedDeltaTime);
			}
			else
            {
				velocity.Set(-moveX * playerSpeed * slopeNormalPerp.x * Time.fixedDeltaTime, -moveX * playerSpeed * slopeNormalPerp.y * Time.fixedDeltaTime);
			}

			rb.velocity = velocity;
			Debug.Log($"Applying slope speed, x={rb.velocity.x}, y={rb.velocity.y}, total={rb.velocity.magnitude}, time={Time.fixedDeltaTime}");
		}
		else if (!isGrounded)
		{
			velocity.Set(moveX * playerSpeed * Time.fixedDeltaTime, rb.velocity.y);
			rb.velocity = velocity;
			Debug.Log("Applying jumping speed");
		}
		
	}

	private void CheckGround()
	{
		// TODO: check ground from capsule collider (same as check slope)
		isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
		

		if (rb.velocity.y <= velocityOffset)
		{
			isJumping = false;
		}
		// TODO: jump sometimes still works on too steep slopes
		if (isGrounded && !isJumping && canWalkOnSlope)
		{
			canJump = true;
		}

	}

	private void CheckSlope()
    {
		Vector2 checkPos = transform.position - new Vector3(0.0f, colliderSize.y / 2 - colliderOffset.y);

		CheckSlopeHorizontal(checkPos);
		CheckSlopeVertical(checkPos);

		if (slopeSideAngle == 0 && slopeDownAngle == 0)
        {
			isOnSlope = false;
        }
	}

	private void CheckSlopeHorizontal(Vector2 checkPos)
    {
		RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsGround);
		RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsGround);

		if (slopeHitFront)
        {
			isOnSlope = true;
			slopeDirection = 1;
			slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
			slopeSideNormalPerp = Vector2.Perpendicular(slopeHitFront.normal).normalized;
			Debug.DrawRay(slopeHitFront.point, slopeSideNormalPerp, Color.blue);

		}
		else if (slopeHitBack)
        {
			isOnSlope = true;
			slopeDirection = -1;
			slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
			slopeSideNormalPerp = Vector2.Perpendicular(slopeHitBack.normal).normalized;
			Debug.DrawRay(slopeHitBack.point, slopeSideNormalPerp, Color.blue);
		}
		else
        {
			slopeDirection = 0;
			slopeSideAngle = 0.0f;
        }
	}

	private void CheckSlopeVertical(Vector2 checkPos)
    {
		RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);

		if (hit)
        {
			slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
			slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

			if (slopeDownAngle != slopeDownAngleOld)
            {
				isOnSlope = true;
            }

			slopeDownAngleOld = slopeDownAngle;
			Debug.DrawRay(hit.point, slopeNormalPerp, Color.green);
			Debug.DrawRay(hit.point, hit.normal, Color.red);
        }

		if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
			canWalkOnSlope = false;
        }
		else
        {
			canWalkOnSlope = true;
        }

		if (isOnSlope && moveX == 0 && canWalkOnSlope)
        {
			rb.sharedMaterial = fullFriction;
        }
		else
        {
			rb.sharedMaterial = noFriction;
        }
    }

	void Jump(int power)
	{
		if (canJump)
        {
			canJump = false;
			isJumping = true;
			rb.velocity = Vector2.zero;
			rb.AddForce(Vector2.up * power, ForceMode2D.Impulse);
		}
		
	}

}
