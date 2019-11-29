using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Warp : MonoBehaviour
{
	[Tooltip("Exact name of the scene to warp to (ex: CreditsScene)")]
	public string sceneName;
    // Start is called before the first frame update
    
	public void ChangeScene()
	{
		SceneManager.LoadScene(sceneName);
	}

}
