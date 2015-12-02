using UnityEngine;

public class CastController : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////
	
	public float waitTime;
	public float castSpeed = 1.0f;
	public float bottomPosition;
	public bool interacting;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	private float currentWaitTime;
	private bool isWaiting = true;
	private bool isFinalWaiting = false;

	void Start ()
	{
		Reset();
	}

	void Update ()
	{
		Vector3 position = GetComponent<RectTransform>().localPosition;
		if (isWaiting || isFinalWaiting)
		{
			if ((isWaiting && position.y > 0) || (isFinalWaiting && position.y < bottomPosition))
			{
				isFinalWaiting = false;
				isWaiting = false;
				currentWaitTime = 0;
				return;
			}
			currentWaitTime += Time.deltaTime;
			if(currentWaitTime >= waitTime)
			{
				currentWaitTime = 0;
				if (isFinalWaiting)
				{
					GetComponent<RectTransform>().localPosition = Vector3.zero;
					currentWaitTime = waitTime / 2.0f;
				}
				isWaiting = isFinalWaiting;
				isFinalWaiting = false;
			}
		}
		else
		{
			if (Input.GetMouseButton(0) && interacting) return;
			
			GetComponent<RectTransform>().localPosition = position += Vector3.up * Time.deltaTime * castSpeed;
			if (position.y >= bottomPosition)
			{
				isFinalWaiting = true;
			}
		}
	}

	public void Reset()
	{
		currentWaitTime = 0;
		isWaiting = true;
		isFinalWaiting = false;
		GetComponent<RectTransform>().localPosition = Vector3.zero;
	}
}
