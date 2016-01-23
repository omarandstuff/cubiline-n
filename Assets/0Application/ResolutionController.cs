using UnityEngine;
using UnityEngine.UI;

public class ResolutionController : MonoBehaviour
{

	public int[] levels = new int[5];

	private int lastLevel = -1;

	void Start ()
	{
		lastLevel = CubilineApplication.singleton.settings.qualityIndex;
		GetComponent<CanvasScaler>().dynamicPixelsPerUnit = levels[lastLevel];
	}

	void Update ()
	{
		if (lastLevel != CubilineApplication.singleton.settings.qualityIndex)
		{
			lastLevel = CubilineApplication.singleton.settings.qualityIndex;
			GetComponent<CanvasScaler>().dynamicPixelsPerUnit = levels[lastLevel];
		}
	}
}
