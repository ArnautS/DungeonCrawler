using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	// serializable variables
	[SerializeField] private int playerJumpPower;
	[SerializeField] private int playerSpeed;
	[SerializeField] private float knockbackPower;

	[SerializeField] private Transform groundCheck;
	[SerializeField] private float groundCheckRadius;
	[SerializeField] private LayerMask whatIsGround;
	[SerializeField] private LayerMask interactables;
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
	private int timesJumped = 0;
	private int maxJumps = 1;

	private bool isGrounded;
	private bool isJumping;
	private bool canJump;
	private bool isOnSlope;
	private bool canWalkOnSlope;
	private bool oldFlipX;
	private bool isKnockedBack = false;

	private Vector2 velocity;
	private Vector2 colliderSize;
	private Vector2 colliderOffset;
	private Vector2 slopeNormalPerp;
	private Vector2 slopeSideNormalPerp;

	private InputMaster controls;
	

	// components
	private Rigidbody2D rb;
	private SpriteRenderer sr;
	private CapsuleCollider2D cc;
	private Animator animator;
	private PlayerCombat combat;

	private void Awake()
	{
		controls = new InputMaster();
	}

	void Start()
	{
		rb = gameObject.GetComponent<Rigidbody2D>();
		sr = gameObject.GetComponent<SpriteRenderer>();
		cc = gameObject.GetComponent<CapsuleCollider2D>();
		animator = gameObject.GetComponent<Animator>();
		combat = gameObject.GetComponent<PlayerCombat>();


		colliderSize = cc.size;
		colliderOffset = cc.offset;

		velocity = rb.velocity;
		oldFlipX = sr.flipX;
	}

	private void OnEnable()
	{
		controls.Enable();
	}

	private void OnDisable()
	{
		controls.Disable();
	}

	// Player controls
	void OnJump()
	{
		if (canJump && !isKnockedBack)
		{
			timesJumped++;
			if (timesJumped >= maxJumps)
            {
				canJump = false;
			}
			isJumping = true;
			rb.velocity = Vector2.zero;
			rb.AddForce(Vector2.up * playerJumpPower, ForceMode2D.Impulse);
		}
	}

	void OnMovement(InputValue value)
	{
		moveX = value.Get<float>();
	}


	private void Update()
	{
		//Debug print
		UIManager.UpdateDebugText($"grounded: {isGrounded}", 0);
		UIManager.UpdateDebugText($"canWalkOnSlope: {canWalkOnSlope}", 1);
		UIManager.UpdateDebugText($"isOnSlope: {isOnSlope}", 2);

		animator.SetFloat("Speed", Mathf.Abs(moveX));
		animator.SetBool("IsJumping", isJumping);
		animator.SetBool("IsGrounded", isGrounded);
		animator.SetBool("IsOnSlope", isOnSlope);
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

		if (oldFlipX != sr.flipX)
		{
			combat.FlipAttackpoint();
		}

		oldFlipX = sr.flipX;

		//set player velocity		
		if (isKnockedBack)
        {

        }
		else if (isGrounded && !isOnSlope && !isJumping)
		{
			velocity.Set(moveX * playerSpeed * Time.fixedDeltaTime, 0.0f);
			rb.velocity = velocity;
			//Debug.Log($"Applying normal speed, x={rb.velocity.x}, y={rb.velocity.y}, total={rb.velocity.magnitude}, time={Time.fixedDeltaTime}");
		}
		else if (isGrounded && isOnSlope && !isJumping && canWalkOnSlope)
		{

			if (moveX == slopeDirection && slopeNormalPerp == Vector2.left)
			{
				velocity.Set(-moveX * playerSpeed * slopeSideNormalPerp.x * Time.fixedDeltaTime, -moveX * playerSpeed * slopeSideNormalPerp.y * Time.fixedDeltaTime);
			}
			else
			{
				velocity.Set(-moveX * playerSpeed * slopeNormalPerp.x * Time.fixedDeltaTime, -moveX * playerSpeed * slopeNormalPerp.y * Time.fixedDeltaTime);
			}

			rb.velocity = velocity;
			//Debug.Log($"Applying slope speed, x={rb.velocity.x}, y={rb.velocity.y}, total={rb.velocity.magnitude}, time={Time.fixedDeltaTime}");
		}
		else if (!isGrounded)
		{
			velocity.Set(moveX * playerSpeed * Time.fixedDeltaTime, rb.velocity.y);
			rb.velocity = velocity;
			//Debug.Log("Applying jumping speed");
		}
		else
		{
			//Debug.Log("Can't apply normal movement");
		}

		animator.SetFloat("VelocityY", rb.velocity.y);

	}

	public void Knockback(Vector2 direction)
    {
		rb.velocity = Vector2.zero;
		rb.AddForce(direction * knockbackPower, ForceMode2D.Impulse);
		rb.sharedMaterial = noFriction;
		rb.gravityScale = 0;
		isKnockedBack = true;
		animator.SetBool("IsKnockedBack", true);

	}

	private void SetKnockbackFalse()
    {
		isKnockedBack = false;
		rb.sharedMaterial = fullFriction;
		rb.gravityScale = 7;
		animator.SetBool("IsKnockedBack", false);
	}

	private void CheckGround()
	{
		isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
		

		if (rb.velocity.y <= velocityOffset)
		{
			isJumping = false;
		}

		if (isGrounded && !isJumping) // && canWalkOnSlope)
		{
			canJump = true;
			timesJumped = 0;
		}

		// falling mechanics
		if (!isGrounded)
        {
			rb.sharedMaterial = noFriction;
		}
		


	}

	private void CheckSlope()
    {
		Vector2 checkPos = transform.position - new Vector3(0.0f, colliderSize.y / 2 - colliderOffset.y);

		CheckSlopeHorizontal(checkPos);
		CheckSlopeVertical(checkPos);

		if ((slopeSideAngle == 0 || slopeSideAngle > maxSlopeAngle) && slopeDownAngle == 0)
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

		// Set material friction to prevent sliding of a slope
		if (isOnSlope && moveX == 0  && isGrounded && !isKnockedBack)
        {
			rb.sharedMaterial = fullFriction;
        }
		else
        {
			rb.sharedMaterial = noFriction;
        }
    }

	public int PlayerDirection()
    {
		if (sr.flipX)
        {
			return -1;
        }
        else
        {
			return 1;
        }
    }


	public void ActivateDoubleJump()
    {
		maxJumps = 2;
    }

	

}
