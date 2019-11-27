using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	public Player playerToSpawn;
	// Start is called before the first frame update


	public void Spawn()
	{
		Instantiate(playerToSpawn, transform.position, Quaternion.identity);
		playerToSpawn = FindObjectOfType<Player>();
	}
}
