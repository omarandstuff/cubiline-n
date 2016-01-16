using UnityEngine;

public class EaseVector3 : MonoBehaviour
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

	public Vector3 inValues;
	public Vector3 outValues;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	public enum EASE_FACE { IN, OUT }
	public enum EASE_TYPE { SMOOTH, TIME, SPEED }

	protected Vector3 currentValues;
	private Vector3 velocity = Vector3.zero;
	private float easeCurretTime;

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// EASE /////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	protected void UpdateEase()
	{
		if (easeType == EASE_TYPE.SMOOTH)
		{
			currentValues = Vector3.SmoothDamp(currentValues, easeFace == EASE_FACE.IN ? inValues : outValues, ref velocity, easeSmoothTime);
		}
		else if (easeType == EASE_TYPE.TIME)
		{
			easeCurretTime += easeFace == EASE_FACE.IN ? -Time.deltaTime : Time.deltaTime;
			easeCurretTime = Mathf.Clamp(easeCurretTime, 0.0f, 1.0f);
			currentValues = Vector3.Lerp(inValues, outValues, easeCurretTime / easeTime);
		}
		else
		{
			currentValues += Vector3.Normalize(inValues - outValues) * easeSpeed * Time.deltaTime;
		}
	}

	public virtual void Reset()
	{
		if (easeFace == EASE_FACE.IN)
		{
			if (forceInitialFace)
			{
				currentValues = outValues;
				easeCurretTime = easeTime;
			}
			else
			{
				currentValues = inValues;
				easeCurretTime = 0.0f;
			}
		}
		else
		{
			if (forceInitialFace)
			{
				currentValues = inValues;
				easeCurretTime = 0.0f;
			}
			else
			{
				currentValues = outValues;
				easeCurretTime = easeTime;
			}
		}
	}
}
