using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	Item[] levelItems;
    // Start is called before the first frame update
    void Start()
    {
		levelItems = FindObjectsOfType<Item>();

	}

    // Update is called once per frame
    void Update()
    {
        
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
