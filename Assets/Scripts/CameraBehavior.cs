using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
	Player playerToFollow;
	MovementController movementController;
	[HideInInspector]
	public bool sliding;
	public float cameraOffset = 0;

	int horizontal;
	float acceleration;
	float maxSpeed;
	float timeToMaxSpeed;

	bool resetting;
	[HideInInspector]
	public bool playerStuck;

	bool accelerationMode = true;
	Vector2 velocity = new Vector2();
    // Start is called before the first frame update
    void Start()
    {
		sliding = false;
		movementController = GetComponent<MovementController>();
		playerToFollow = FindObjectOfType<Player>();
		maxSpeed = playerToFollow.maxSpeed;
		timeToMaxSpeed = playerToFollow.timeToMaxSpeed;
		acceleration = (2f * maxSpeed) / Mathf.Pow(timeToMaxSpeed, 2);
		gameObject.transform.position = new Vector3((playerToFollow.gameObject.transform.position.x + cameraOffset), gameObject.transform.position.y, gameObject.transform.position.z);

	}

	// Update is called once per frame
	void Update()
    {
		if (GameManager.I.gamePaused)
		{
			return;
		}
		if (!playerToFollow)
		{
			playerToFollow = FindObjectOfType<Player>();
		}
		if (playerStuck)
		{
			AlwaysRunning();
			transform.Translate(velocity * Time.deltaTime);

		} else
		{
			SlidingCamera();
		}

		
	}
	public void HardResetCamera()
	{
		gameObject.transform.position = new Vector3((playerToFollow.gameObject.transform.position.x + cameraOffset), gameObject.transform.position.y, gameObject.transform.position.z);
	}
	public void ResetCamera()
	{
		StartCoroutine(ResettingCamera());
	}
	IEnumerator ResettingCamera()
	{
		resetting = true;
		while (true)
		{
			if(gameObject.transform.position == new Vector3((playerToFollow.gameObject.transform.position.x + cameraOffset), gameObject.transform.position.y, gameObject.transform.position.z))
			{
				resetting = false;
				yield break;
			}
			Vector3 movementToTarget = Vector3.MoveTowards(gameObject.transform.position, new Vector3(playerToFollow.gameObject.transform.position.x + cameraOffset, gameObject.transform.position.y, gameObject.transform.position.z), Time.deltaTime);
			gameObject.transform.position = movementToTarget;
			//if(movementToTarget.Distance())
			yield return null;
		}
	}
	void AlwaysRunning()
	{
		if (resetting)
		{
			return;
		}
		horizontal = 1;
		float controlModifier = 1f;
		if (accelerationMode)
		{

			velocity.x += horizontal * acceleration * controlModifier * Time.deltaTime;

		}

		if (Mathf.Abs(velocity.x) > maxSpeed)
			velocity.x = maxSpeed * horizontal;
	}
	public void CameraKeepsMoving()
	{
		AlwaysRunning();
	}
	public void SlidingCamera()
	{
		/*if (sliding)
		{
			gameObject.transform.position = new Vector3(playerToFollow.gameObject.transform.position.x + 0.5f, gameObject.transform.position.y, gameObject.transform.position.z);
		} else
		{*/
			gameObject.transform.position = new Vector3((playerToFollow.gameObject.transform.position.x + cameraOffset), gameObject.transform.position.y, gameObject.transform.position.z);
		//}
		//gameObject.transform.Translate(0.5f * horizontal, 0, 0);
	}

	private static CameraBehavior _I;
	public static CameraBehavior I => _I;
	public CameraBehavior()
	{
		_I = this;
	}
}
