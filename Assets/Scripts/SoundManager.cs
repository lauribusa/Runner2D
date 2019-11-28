using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	AudioSource audioSource;
	public AudioClip[] audioClips;

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
				audioSource.PlayOneShot(audioClips[0]);
				break;
			case "playerAttack":
				audioSource.PlayOneShot(audioClips[1]);
				break;
			case "jewel":
				audioSource.PlayOneShot(audioClips[2]);
				break;
			case "footstep":
				audioSource.PlayOneShot(audioClips[3]);
				break;
			default:
				break;
		}
	}

	public void PlayFootstep()
	{
		audioSource.PlayOneShot(playerFootsteps[Random.Range(0, playerFootsteps.Length - 1)]);
		
	}
	private static SoundManager _I;
	public static SoundManager I => _I;
	public SoundManager()
	{
		_I = this;
	}
}
