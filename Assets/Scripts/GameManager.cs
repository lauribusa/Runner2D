using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	Item[] levelItems;
	Camera main;
	GameObject cameraObject;

	[Header("Thunder settings")]
	[Tooltip("Minimum light intensity of thunder")]
	public float minIntensity;
	[Tooltip("Maximum light intensity of thunder")]
	public float maxIntensity;
	[Tooltip("Delay between flashes")]
	public float strikesDelay;
	[Tooltip("Number of thunder flashes")]
	public int numberOfFlashes;

	public string mainMenuScene;
	public bool gamePaused { get; private set; }

	[Tooltip("Canvas for pause menu")]
	public GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
		levelItems = FindObjectsOfType<Item>();
		main = Camera.main;
		cameraObject = main.gameObject;
		canvas.SetActive(false);
	}

    // Update is called once per frame
    void Update()
    {
		if (!gamePaused && Input.GetKeyDown(KeyCode.Escape))
		{
			PauseGame();
		}
    }
	public void TriggerLightning()
	{
		Light2D[] lightnings = cameraObject.GetComponentsInChildren<Light2D>();
		StartCoroutine(LightningCoroutine(lightnings[0], minIntensity, maxIntensity, strikesDelay, numberOfFlashes));

	}
	public void PauseGame()
	{
		canvas.SetActive(true);
		gamePaused = true;
		Player.I.anim.enabled = false;
	}
	public void ResumeGame()
	{
		gamePaused = false;
		canvas.SetActive(false);
		Player.I.anim.enabled = true;
	}

	public void MainMenuSwitch()
	{
		SceneManager.LoadScene(mainMenuScene);
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	IEnumerator LightningCoroutine(Light2D light, float minIntensity, float maxIntensity, float delay, int numberOfFlashes)
	{
		int i = 0;
		while (true)
		{
			if(i >= numberOfFlashes)
			{
				light.intensity = 0;
				yield break;
			}
			light.intensity = Random.Range(minIntensity, maxIntensity);
			yield return new WaitForSeconds(delay);
			light.intensity = Random.Range(minIntensity, maxIntensity);
			yield return new WaitForSeconds(delay);
			i++;
			yield return null;
			
		}
	}
	void findObjects()
	{
		levelItems = FindObjectsOfType<Item>();
	}
	public void RespawnItems()
	{
		foreach (Item item in levelItems)
		{
			item.gameObject.SetActive(true);
		}
	}
	#region singleton
		private static GameManager _I;
		public static GameManager I => _I;
		public GameManager()
		{
			_I = this;
		}
	#endregion
}
