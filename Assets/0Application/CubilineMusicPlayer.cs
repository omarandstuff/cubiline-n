using System.Collections;
using UnityEngine;

public class CubilineMusicPlayer : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public static CubilineMusicPlayer singleton;
	
	public Song[] songs;
	public GameObject arcadeFinishWavePrefab;

	[System.Serializable]
	public struct Song { public AudioClip songFile; public string name; public string artist; public string album; }


	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////



	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	public static bool loaded = false;
	public static bool inMenu = true;
	private bool lastInMenu;

	private uint currentIndex;
	private uint[] indexList = new uint[0];



	void Awake()
	{
		if (loaded)
		{
			Destroy(gameObject);
			return;
		}
		loaded = true;
		DontDestroyOnLoad(transform.gameObject);
		currentIndex = 0;
		singleton = this;
	}

	void Start ()
	{
	}
	
	void Update ()
	{
		if (!lastInMenu && inMenu)
			PlaySong(0);

		if (inMenu && !GetComponent<AudioSource>().isPlaying)
			PlaySong(0);
		if((!inMenu && !GetComponent<AudioSource>().isPlaying) || (lastInMenu && !inMenu))
		{
			if(indexList.Length == 0 || currentIndex == songs.Length)
			{
				uint lastIndexValue = 0;
				if (indexList.Length != 0)
					lastIndexValue = indexList[currentIndex - 1];

				ArrayList indexAvailables = new ArrayList();
				for (uint i = 0; i < songs.Length; i++)
					indexAvailables.Add(i);

				indexAvailables.Remove(lastIndexValue);

				indexList = new uint[songs.Length];
				uint index = 0;
				while (indexAvailables.Count > 0)
				{
					indexList[index] = (uint)indexAvailables[(int)(Random.value * (indexAvailables.Count - 1))];
					indexAvailables.Remove(indexList[index]);
					index++;
				}
				indexList[index] = lastIndexValue;
				currentIndex = 0;
			}
			PlaySong(indexList[currentIndex++]);
		}

		lastInMenu = inMenu;
	}


	void PlaySong(uint index)
	{
		GetComponent<AudioSource>().clip = songs[index].songFile;
		GetComponent<AudioSource>().Play();
	}

	public void ArcadeFinishWave()
	{
		GameObject fw = Instantiate(arcadeFinishWavePrefab);
		fw.transform.SetParent(transform);
		Destroy(fw, 5);
	}
}
