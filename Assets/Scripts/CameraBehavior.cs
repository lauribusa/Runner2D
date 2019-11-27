using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
	Player playerToFollow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (!playerToFollow)
		{
			playerToFollow = FindObjectOfType<Player>();
		}
		gameObject.transform.position = new Vector3(playerToFollow.gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
    }
}
