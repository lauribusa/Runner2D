using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
	/*Score scoreDisplay;
	SoundManager sndManager;*/
	public int point = 1;
	// Start is called before the first frame update
	void Start()
	{
		/*sndManager = FindObjectOfType<SoundManager>();
		scoreDisplay = FindObjectOfType<Score>();*/
	}

	// Update is called once per frame
	void Update()
	{
		//OnTouchJewel();
	}
	public void Respawn()
	{
		gameObject.SetActive(true);
	}
	public void OnTouchJewel()
	{
		SoundManager.I.PlayClip("jewel");
		//sndManager.PlayClip("jewel");
		//scoreDisplay.UpdateScore(point);
		gameObject.SetActive(false);
		//Destroy(gameObject);
	}
}
