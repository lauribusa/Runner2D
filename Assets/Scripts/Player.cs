using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	delegate void AnimDelegate();
	AnimDelegate UpdateAnimation;
	public LayerMask enemyLayer;

	public int playerLives = 3;

	Camera mainCamera;
	CameraBehavior camBehavior;

	[Header("Run speed")]
	[Tooltip("Number of meter by second")]
	public float maxSpeed;
	[Tooltip("Time to get to maximum speed")]
	public float timeToMaxSpeed;
	[HideInInspector]
	public float brakeQuotient = 1;

	float speedModifier = 1f;
	[Tooltip("Activates Acceleration Mode or constant running speed")]
	public bool accelerationMode;
	[Header("Slide")]
	[Tooltip("Can the player slide forever or not?")]
	public bool slideTimeLimited;
	[Tooltip("The time (in seconds) during which the player slides before standing up again.")]
	public float slideTime;
	bool sliding;
	[HideInInspector]
	public uint maxAirJump;
	[Header("Jump")]
	[Tooltip("Unity value of max jump height")]
	public float jumpHeight;
	[Tooltip("Minimum time on ascension in jump")]
	public float minAscensionTime;
	[Tooltip("Time in seconds to reach the jump height")]
	public float timeToMaxJump;
	[Tooltip("Can i change direction in air?")]
	[Range(0, 1)]
	public float airControl;
	


	[Header("Other")]
	public bool animationByParameters;

	float acceleration;
	float minSpeedThreshold;

	float gravity;
	float jumpForce;
	float maxFallingSpeed;
	int horizontal;

	bool hurt;

	bool bouncing;

	public Animator anim;
	SpriteRenderer spriteRenderer;
	BoxCollider2D boxCollider;

	AudioSource audioSource;

	Vector2 velocity = new Vector2();
	MovementController movementController;
	//PlayerAttack playerAttack;
	AnimationTimes animationTimes;

	private static Player _I;
	public static Player I => _I;
	public Player()
	{
		_I = this;
	}

	public bool freeze { get; private set; }

	private void Start()
	{
		mainCamera = Camera.main;
		camBehavior = mainCamera.GetComponent<CameraBehavior>();
		acceleration = (2f * maxSpeed) / Mathf.Pow(timeToMaxSpeed, 2);
		horizontal = 0;
		minSpeedThreshold = acceleration / Application.targetFrameRate * 2f;
		movementController = GetComponent<MovementController>();
		anim = GetComponent<Animator>();
		boxCollider = GetComponent<BoxCollider2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		animationTimes = GetComponent<AnimationTimes>();

		// Math calculation for gravity and jumpForce
		gravity = -(2 * jumpHeight) / Mathf.Pow(timeToMaxJump, 2);
		jumpForce = Mathf.Abs(gravity) * timeToMaxJump;
		maxFallingSpeed = -jumpForce;

		if (animationByParameters)
			UpdateAnimation = UpdateAnimationByParameters;
		else
			UpdateAnimation = UpdateAnimationByCode;
	}
	void Update()
	{
		if (GameManager.I.gamePaused)
		{
			return;
		}
		AlwaysRunning();
		//UpdateHorizontalControl();
		
		UpdateGravity();
		UpdateSlide();
		UpdateJump();
		UpdateFlip();
		//UpdateAttack();
		
		movementController.Move(velocity * Time.deltaTime);

		UpdateAnimation();
		if (movementController._collisions.right && !CameraBehavior.I.playerStuck)
		{
			CameraBehavior.I.playerStuck = true;
		}
		else
		if (!movementController._collisions.right && CameraBehavior.I.playerStuck)
		{
			CameraBehavior.I.playerStuck = false;
		}
		if (Input.GetKeyDown(KeyCode.Backspace))
		{
			Restart();
		}
		if(gameObject.transform.position.y <= -10)
		{
			Die();
		}

	}
	private void OnBecameInvisible()
	{
		Debug.Log("out of camera");
	}
	public void AnimationPlayFootStep()
	{
		SoundManager.I.PlayFootstep();
	}
	IEnumerator IsSliding(float time)
	{
		while (true)
		{
			freeze = true;
			yield return new WaitForSeconds(time);
			freeze = false;
			sliding = false;
			gameObject.transform.Translate(0, 0.5f, 0);
			yield break;
		}
	}
	void UpdateSlide()
	{
		if (freeze || !movementController._collisions.bottom)
		{
			return;
		}
		if (movementController._collisions.bottom && movementController._collisions.top)
		{
			sliding = true;
		}
		if (slideTimeLimited)
		{
			if (sliding)
			{
				
				StartCoroutine(IsSliding(slideTime));
			}
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				sliding = true;
				SoundManager.I.PlayClip("slide");
				gameObject.transform.Translate(0, -0.5f, 0);
			}
		} else
		{
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				sliding = true;
				SoundManager.I.PlayClip("slide");
				
				//anim.Play("slide");
				gameObject.transform.Translate(0, -0.5f, 0);
				camBehavior.sliding = sliding;
				//gameObject.transform.Translate(-0.5f, -0.5f, 0);
			}
			if (Input.GetKeyUp(KeyCode.DownArrow) && !movementController._collisions.top)
			{
				sliding = false;
				camBehavior.sliding = sliding;
				//anim.Play("run");
				//gameObject.transform.Translate(0.5f, 0.5f, 0);
				gameObject.transform.Translate(0, 0.5f, 0);
			}

		}
		if (movementController._collisions.bottom && movementController._collisions.top)
		{
			sliding = true;
		}
		if (sliding)
		{
			boxCollider.size = new Vector2(1, 1);
			//gameObject.transform.localScale = new Vector3(2, 1, 1);
			movementController.ReCalculateBounds();
		}
		else
		{
			boxCollider.size = new Vector2(1, 2);
			//gameObject.transform.localScale = new Vector3(1, 2, 1);
			movementController.ReCalculateBounds();
		}
	}
	void AlwaysRunning()
	{
		horizontal = 1;
		//float controlModifier = 1f;
		speedModifier += Time.deltaTime;
		float controlModifier = 1f;
		if (accelerationMode)
		{

			velocity.x += horizontal * acceleration * controlModifier * Time.deltaTime;

		}
		else
		{
			velocity.x += horizontal * maxSpeed * speedModifier;
		}

		if (Mathf.Abs(velocity.x) > maxSpeed)
			velocity.x = maxSpeed * horizontal;
	}
	void UpdateHorizontalControl()
	{
		if (hurt)
		{
			return;
		}
		// Reset velocity at start of frame is hitting wall
		// Then I will add one frame of velocity to stay sticking on wall for example
		// But I want my speed to stop when reaching wall
		if ((velocity.x > 0 && movementController._collisions.right) ||
			(velocity.x < 0 && movementController._collisions.left))
		{
			velocity.x = 0;
		}
		if (!movementController._collisions.bottom)
			return;

		horizontal = 0;

		if (Input.GetKey(KeyCode.RightArrow) && !freeze && !(movementController._collisions.bottom))
		{
			horizontal += 1;
		}
		if (Input.GetKey(KeyCode.LeftArrow) && !freeze && !(movementController._collisions.bottom))
		{
			horizontal -= 1;
		}
		
		float controlModifier = 1f;
		if (!movementController._collisions.bottom)
		{
			controlModifier = airControl;
		}
		
		if (accelerationMode)
		{
			velocity.x += horizontal * acceleration * controlModifier * Time.deltaTime;

		}
		else
		{
			velocity.x += horizontal * maxSpeed * controlModifier;
		}

		if (Mathf.Abs(velocity.x) > maxSpeed)
			velocity.x = maxSpeed * horizontal;


		if (horizontal == 0 && accelerationMode)
		{
			if (velocity.x > minSpeedThreshold)
				velocity.x -= acceleration * brakeQuotient * Time.deltaTime;
			else if (velocity.x < -minSpeedThreshold)
				velocity.x += acceleration * brakeQuotient * Time.deltaTime;
			else
				velocity.x = 0;
		}
		else
		if (horizontal == 0 && !accelerationMode)
		{
			velocity.x = 0;
		}
	}

	void UpdateGravity()
	{
		if (movementController._collisions.bottom || movementController._collisions.top)
			velocity.y = 0;


		/*if ((movementController._collisions.left || movementController._collisions.right) && velocity.y < 0)
		{
			velocity.y += gravity * Time.deltaTime / 3f;
		}
		else
		{
		}*/
		velocity.y += gravity * Time.deltaTime;

		if (velocity.y < maxFallingSpeed)
		{
			velocity.y = maxFallingSpeed;
		}
	}

	void UpdateAnimationByCode()
	{
		if(!movementController._collisions.bottom && sliding)
		{
			if (velocity.y > 0)
				anim.Play("ascend"); // ascending
			else if (velocity.y < 0)
				anim.Play("descend"); // falling
		}
		if (freeze)
			return;

		// Au sol
		if (movementController._collisions.bottom)
		{
			if (!sliding)
			{
				anim.Play("run");
			} else
			{
				anim.Play("runtoslide");
			}
		}
		// En l'air
		else
		{
				if (velocity.y > 0)
					anim.Play("ascend"); // ascending
				else if (velocity.y < 0)
					anim.Play("descend"); // falling
		}
	}

	void UpdateAnimationByParameters()
	{
		int vertical = (int)Mathf.Sign(velocity.y);
		if (movementController._collisions.bottom)
			vertical = 0;

		anim.SetInteger("horizontal", horizontal);
		anim.SetInteger("vertical", vertical);
		anim.SetBool("grounded", movementController._collisions.bottom);
		anim.SetBool("bouncing", bouncing);
	}

	void UpdateFlip()
	{
		if (freeze)
			return;

		if (velocity.x > 0)
		{
			// regarde vers la droite
			spriteRenderer.flipX = false;
		}
		else if (velocity.x < 0)
		{
			// regarde vers la gauche
			spriteRenderer.flipX = true;
		}
	}

	void UpdateJump()
	{
		if (movementController._collisions.bottom)
		{

			bouncing = false;
		}
		if(sliding && Input.GetKeyDown(KeyCode.Space) && freeze)
		{
			Jump();
		}
		if (Input.GetKeyDown(KeyCode.Space) && !freeze)
		{
			// Normal jump
			if (movementController._collisions.bottom)
			{
				//sndManager.PlayClip("jump");
				Jump();
				
			}
			// Wall jump
			
		}
	}
	Coroutine jumpCoroutine;
	void Jump()
	{
		SoundManager.I.PlayClip("jump");
		CameraBehavior.I.HardResetCamera();
		if (jumpCoroutine != null)
		{
			StopCoroutine(jumpCoroutine);
		}

		jumpCoroutine = StartCoroutine(JumpCoroutine());
	}

	IEnumerator JumpCoroutine()
	{
		velocity.y = jumpForce;

		float time = 0;
		while (time < minAscensionTime)
		{
			time += Time.deltaTime;
			yield return null;
		}

		while (true)
		{
			if (!Input.GetKey(KeyCode.Space))
			{
				break;
			}
			if (velocity.y <= 0)
			{
				break;
			}
			yield return null;
		}

		velocity.y = 0;
	}
	/*public void Bounce()
	{
		bouncing = true;
		anim.Play("spin");
		horizontal = spriteRenderer.flipX ? 1 : -1;
		velocity.y = jumpForce * verticalBounceForce;
		velocity.x = horizontalBounceForce * maxSpeed * horizontal;
	}*/
	void WallJump()
	{
		int direction = movementController._collisions.left ? 1 : -1;
		velocity.x = maxSpeed * direction;
		Jump();
	}
	void Restart()
	{
		Checkpoint checkpoint = FindObjectOfType<Checkpoint>();
		checkpoint.Spawn();
		Destroy(gameObject);
	}
	/*void DoubleJump()
	{
		StartCoroutine(DoubleJumpCoroutine());
	}

	IEnumerator DoubleJumpCoroutine()
	{
		Jump();
		doubleJumpCount++;
		doubleJumping = true;
		anim.Play("FrogDoubleJumping");

		while (!anim.GetCurrentAnimatorStateInfo(0).IsName("jump"))
		{
			yield return null;
		}

		while (true)
		{
			if (!anim.GetCurrentAnimatorStateInfo(0).IsName("jump") ||
				movementController._collisions.bottom)
				break;
			yield return null;
		}
		doubleJumping = false;
	}*/

	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Enemy enemy = collision.gameObject.GetComponent<Enemy>();
		Item item = collision.gameObject.GetComponent<Item>();
		Enemy trap = collision.gameObject.GetComponent<Enemy>();
		//Enemy trap = collision.gameObject.layer
		//Warp warp = collision.gameObject.GetComponent<Warp>();
		if(collision.gameObject.layer == 10)
		{
			switch (collision.gameObject.tag)
			{
				case "thunder":
					Debug.Log("Lightning triggered");
					GameManager.I.TriggerLightning();
					break;
				case "setLowLight":
					GlobalLightScript.I.SetMinimum();
					break;
				case "setHighLight":
					GlobalLightScript.I.SetMaximum();
					break;
				case "warp":
					Warp warp = collision.gameObject.GetComponent<Warp>();
					warp.ChangeScene();
					break;
				default:
					break;
			}
			
		}
		if(trap != null)
		{
			Debug.Log("lives: "+playerLives);
			if (!trap.isDeathZone)
			{
				trap.PlayAttack();
			}
			if(playerLives <= 0)
			{
				Die();
			} else
			{
				playerLives--;
			}
		}
		/*if (enemy != null && !playerAttack.isAttacking)
		{
			HitEnemy(enemy);
		}
		else
		if (enemy != null && playerAttack.isAttacking)
		{
			enemy.Damage();
		}*/
		if (item != null)
		{
			item.OnTouchJewel();
			//gameManager.PitchUp();
		}
		//warp?.NextLevel();

	}
	void Die()
	{
		Restart();
		CameraBehavior.I.HardResetCamera();
		GameManager.I.RespawnItems();
	}
	/*Coroutine hitEnemy;
	void HitEnemy(Enemy enemy)
	{
		if (hitEnemy == null)
		{
			hitEnemy = StartCoroutine(HitEnemyCoroutine(enemy));
			//gameManager.PitchDown();
		}
	}*/
	/*IEnumerator BounceCoroutine()
	{
		while (true)
		{
			anim.Play("spin");
			if (movementController._collisions.bottom)
			{
				anim.Play("idle");
				yield break;
			}
			yield return null;
		}
	}*/
	/*IEnumerator HitEnemyCoroutine(Enemy enemy)
	{
		// XXX pushback
		velocity.x = enemy.pushBackForce * Mathf.Sign(transform.position.x - enemy.transform.position.x);
		anim.Play("hurt");
		freeze = true;

		// Wait for the time of the Hurt animation to be finished
		yield return new WaitForSeconds(animationTimes.GetTime("player_hurt"));

		Checkpoint checkpoint = FindObjectOfType<Checkpoint>();
		checkpoint.Spawn();

		freeze = false;
		Destroy(gameObject);
	}*/
}
