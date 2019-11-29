using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	Animator anim;
	AnimationTimes animationTimes;
	SpriteRenderer spriteRenderer;
	public bool isDeathZone;
	// Start is called before the first frame update
	void Start()
    {
		spriteRenderer = GetComponent<SpriteRenderer>();
		animationTimes = GetComponent<AnimationTimes>();
		anim = GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void PlayAttack()
	{
		if (isDeathZone)
		{
			return;
		}
		anim.Play("attack");
	}
}
