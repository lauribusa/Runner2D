using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTimes : MonoBehaviour
{
	Animator anim;

	Dictionary<string, float> timesDict = new Dictionary<string, float>();

	// Start is called before the first frame update
	void Start()
	{
		anim = GetComponent<Animator>();

		CalculateAnimationTimes();
	}

	void CalculateAnimationTimes()
	{
		// Get all clips contained in the animator
		AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

		// Fill a dictionary with the name and length of all clips
		// {"idle": 0.75,
		//  "hit" : 0.3,
		//  ...}
		foreach (AnimationClip clip in clips)
		{
			timesDict.Add(clip.name, clip.length);
		}
	}

	public float GetTime(string clipName)
	{
		return timesDict[clipName];
	}
}
