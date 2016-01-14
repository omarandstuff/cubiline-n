using UnityEngine;

public class SettingsLoader : MonoBehaviour
{
	private static bool loaded;

	void Awake ()
	{
		if(loaded)
		{
			Destroy(gameObject);
		}
		else
		{
			CubilinePlayerData.Load();
			Destroy(gameObject);
			loaded = true;
		}
	}

}
