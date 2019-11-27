using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
	Player playerToFollow;
	public bool sliding;
    // Start is called before the first frame update
    void Start()
    {
		sliding = false;
    }

    // Update is called once per frame
    void Update()
    {
		if (!playerToFollow)
		{
			playerToFollow = FindObjectOfType<Player>();
		}
		SlidingCamera();
    }

	public void SlidingCamera()
	{
		if (sliding)
		{
			gameObject.transform.position = new Vector3(playerToFollow.gameObject.transform.position.x + 0.5f, gameObject.transform.position.y, gameObject.transform.position.z);
		} else
		{
			gameObject.transform.position = new Vector3(playerToFollow.gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
		}
		//gameObject.transform.Translate(0.5f * horizontal, 0, 0);
	}
}
