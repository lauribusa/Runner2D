using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	AudioSource audioSource;

	public AudioClip[] jumps;
	public AudioClip slide;
	public AudioClip candle;
	public AudioClip hurt;
	public AudioClip monsterInArea;

	[Header("Thunderclaps")]
	public AudioClip[] thunderclaps;
	[Header("Footsteps")]
	public AudioClip[] playerFootsteps;
	// Start is called before the first frame update
	void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void PlayClip(string sound)
	{
		switch (sound)
		{
			case "jump":
				audioSource.PlayOneShot(jumps[Random.Range(0, jumps.Length-1)]);
				break;
			case "slide":
				audioSource.PlayOneShot(slide);
				break;
			case "candle":
				audioSource.PlayOneShot(candle);
				break;
			case "hurt":
				audioSource.PlayOneShot(hurt);
				break;
			default:
				break;
		}
	}
	public void PlayMonsterWarning()
	{
		audioSource.PlayOneShot(monsterInArea);
	}
	public void PlayFootstep()
	{
		PlayRandom(playerFootsteps);		
	}
	void PlayRandom(AudioClip[] audioclips)
	{
		audioSource.PlayOneShot(audioclips[Random.Range(0, audioclips.Length - 1)]);
	}
	public void PlayRandomClap()
	{
		PlayRandom(thunderclaps);
	}
	private static SoundManager _I;
	public static SoundManager I => _I;
	public SoundManager()
	{
		_I = this;
	}
}
