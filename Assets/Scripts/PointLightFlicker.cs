using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class PointLightFlicker : MonoBehaviour
{
	Light2D light2D;
	[Header("Light range")]
	public float minLightRange = 3f;
	public float maxLightRange = 4f;
	[Header("Delay between each light range updates")]
	[Range(0,1000)]
	public float delayInSeconds = 0f;
	//Light twolight;
	// Start is called before the first frame update
	void Start()
    {
		light2D = GetComponent<Light2D>();
		StartCoroutine(flickerCoroutine());

	}

	// Update is called once per frame
	
	

	IEnumerator flickerCoroutine()
	{
		while (true)
		{
			light2D.pointLightOuterRadius = Random.Range(3f, 4f);
			yield return new WaitForSeconds(delayInSeconds);
			

		}
	}
}
