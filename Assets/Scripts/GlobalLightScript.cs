using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class GlobalLightScript : MonoBehaviour
{
	public float maxIntensity;
	public float minIntensity;
	Light2D globalLight;
    // Start is called before the first frame update
    void Start()
    {
		globalLight = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void SetMinimum()
	{
		globalLight.intensity = minIntensity;
	}

	public void SetMaximum()
	{
		globalLight.intensity = maxIntensity;
	}
	private static GlobalLightScript _I;
	public static GlobalLightScript I => _I;
	public GlobalLightScript()
	{
		_I = this;
	}
}
