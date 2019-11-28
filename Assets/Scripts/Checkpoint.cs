using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	public GameObject playerPrefab;
	// Start is called before the first frame update


	public void Spawn()
	{
		Instantiate(playerPrefab, transform.position, Quaternion.identity);
	}
}
