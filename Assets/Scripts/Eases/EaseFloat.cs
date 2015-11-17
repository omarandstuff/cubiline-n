using UnityEngine;

public class EaseFloat : MonoBehaviour
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

	public float inValue;
	public float outValue;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	public enum EASE_FACE { IN, OUT }
	public enum EASE_TYPE { SMOOTH, TIME, SPEED }

	protected float currentValue;
	private float velocity;
	private float easeCurretTime;


	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// EASE /////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	protected void UpdateEase()
	{
		if (easeType == EASE_TYPE.SMOOTH)
		{
			currentValue = Mathf.SmoothDamp(currentValue, easeFace == EASE_FACE.IN ? inValue : outValue, ref velocity, easeSmoothTime);
		}
		else if (easeType == EASE_TYPE.TIME)
		{
			easeCurretTime += easeFace == EASE_FACE.IN ? -Time.deltaTime : Time.deltaTime;
			currentValue = Mathf.Lerp(inValue, outValue, easeCurretTime / easeTime);
		}
		else
		{
			currentValue += easeSpeed * Time.deltaTime;
		}
	}

	public virtual void Reset()
	{
		if (easeFace == EASE_FACE.IN)
		{
			if (forceInitialFace)
			{
				currentValue = outValue;
				easeCurretTime = easeTime;
			}
			else
			{
				currentValue = inValue;
				easeCurretTime = 0.0f;
			}
		}
		else
		{
			if (forceInitialFace)
			{
				currentValue = inValue;
				easeCurretTime = 0.0f;
			}
			else
			{
				currentValue = outValue;
				easeCurretTime = easeTime;
			}
		}
	}
}
