using UnityEngine;
using UnityEngine.UI;

public class EaseOpasity : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////
	public bool forceInitialFace = true;
	public EASE_FACE easeFace = EASE_FACE.IN;
	public EASE_TYPE easeType = EASE_TYPE.SMOOTH;
	public float easeSmoothTime = 0.5f;
	public float easeTime = 1.0f;
	public float easeSpeed = 2.0f;

	public float inOpasity;
	public float outOpasity;

	public Image[] imageChildren;
	public Text[] textChildren;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	public enum EASE_FACE { IN, OUT }
	public enum EASE_TYPE { SMOOTH, TIME, SPEED }

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	private float currentOpasity;
	private float velocity;
	private float easeCurretTime;


	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		Reset();
	}

	void FixedUpdate()
	{
		if (easeType == EASE_TYPE.SMOOTH)
		{
			currentOpasity = Mathf.SmoothDamp(currentOpasity, easeFace == EASE_FACE.IN ? inOpasity : outOpasity, ref velocity, easeSmoothTime);
		}
		else if (easeType == EASE_TYPE.TIME)
		{
			easeCurretTime += easeFace == EASE_FACE.IN ? -Time.deltaTime : Time.deltaTime;
			currentOpasity = Mathf.Lerp(inOpasity, outOpasity, easeCurretTime / easeTime);
		}
		else
		{
			currentOpasity += easeSpeed * Time.deltaTime;
		}

		foreach (Image img in imageChildren)
		{
			Color imageColor = img.color;
			imageColor.a = currentOpasity;
			img.color = imageColor;
		}
		foreach (Text txt in textChildren)
		{
			Color imageColor = txt.color;
			imageColor.a = currentOpasity;
			txt.color = imageColor;
		}
	}

	public void Reset()
	{
		if (easeFace == EASE_FACE.IN)
		{
			if (forceInitialFace)
			{
				currentOpasity = outOpasity;
				easeCurretTime = easeTime;
			}
			else
			{
				currentOpasity = inOpasity;
				easeCurretTime = 0.0f;
			}
		}
		else
		{
			if (forceInitialFace)
			{
				currentOpasity = inOpasity;
				easeCurretTime = 0.0f;
			}
			else
			{
				currentOpasity = outOpasity;
				easeCurretTime = easeTime;
			}
		}
	}
}