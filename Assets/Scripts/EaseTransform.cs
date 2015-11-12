using UnityEngine;

public class EaseTransform : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////
	public EASE_FACE easeFace = EASE_FACE.IN;
	public EASE_TYPE easeType = EASE_TYPE.SMOOTH;
	public float easeSmoothTime = 0.5f;
	public float easeTime = 1.0f;
	public float easeSpeed = 2.0f;

	public bool easePosition;
	public bool easeRotation;
	public bool easeScale;

	public Transform targetInPosition;
	public Vector3 inPosition;
	public Transform targetOutPosition;
	public Vector3 outPosition;

	public Transform targetInRotation;
	public Vector3 inRotation;
	public Transform targetOutRotation;
	public Vector3 outRotation;

	public Transform targetInScale;
	public Vector3 inScale;
	public Transform targetOutScale;
	public Vector3 outScale;

	public Transform targetIn;
	public Transform targetOut;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	public enum EASE_FACE { IN, OUT }
	public enum EASE_TYPE { SMOOTH, TIME, SPEED }

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	private Vector3 currentPosition;
	private Vector3 positionVelocity = Vector3.zero;
	private Vector3 currentRotation;
	private Vector3 rotationVelocity = Vector3.zero;
	private Vector3 currentScale;
	private Vector3 scaleVelocity = Vector3.zero;

	private float easeCurretTime;


	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start ()
	{
		if (targetIn != null)
		{
			inPosition = targetIn.transform.localPosition;
			inRotation = targetIn.localRotation.eulerAngles;
			inScale = targetIn.localScale;
		}
		else
		{
			if (targetInPosition != null)
				inPosition = targetInPosition.localPosition;

			if (targetInRotation != null)
				inRotation = targetInPosition.localRotation.eulerAngles;

			if (targetInScale != null)
				inScale = targetInScale.localScale;
		}
		if (targetOut != null)
		{
			outPosition = targetOut.transform.localPosition;
			outRotation = targetOut.localRotation.eulerAngles;
			outScale = targetOut.localScale;
		}
		else
		{
			if (targetOutPosition != null)
				outPosition = targetOutPosition.localPosition;

			if (targetOutRotation != null)
				currentRotation = targetOutPosition.localRotation.eulerAngles;

			if (targetOutScale != null)
				currentScale = targetOutScale.localScale;
		}

		if(easeFace == EASE_FACE.IN)
		{
			currentPosition = outPosition;
			currentRotation = outRotation;
			currentScale = outScale;
			easeCurretTime = easeTime;
		}
		else
		{
			currentPosition = inPosition;
			currentRotation = inRotation;
			currentScale = inScale;
			easeCurretTime = 0.0f;
		}
	}

	void FixedUpdate ()
	{
		if (easeType == EASE_TYPE.SMOOTH)
		{
			if (easePosition) currentPosition = Vector3.SmoothDamp(currentPosition, easeFace == EASE_FACE.IN ? inPosition : outPosition, ref positionVelocity, easeSmoothTime);
			if (easeRotation) currentRotation = Vector3.SmoothDamp(currentRotation, easeFace == EASE_FACE.IN ? inRotation : outRotation, ref rotationVelocity, easeSmoothTime);
			if (easeScale) currentScale = Vector3.SmoothDamp(currentScale, easeFace == EASE_FACE.IN ? inScale : outScale, ref scaleVelocity, easeSmoothTime);
		}
		else if (easeType == EASE_TYPE.TIME)
		{
			easeCurretTime += easeFace == EASE_FACE.IN ? -Time.deltaTime : Time.deltaTime;
			if (easePosition) currentPosition = Vector3.Lerp(inPosition, outPosition, easeCurretTime / easeTime);
			if (easeRotation) currentRotation = Vector3.Lerp(inRotation, outRotation, easeCurretTime / easeTime);
			if (easeScale) currentScale = Vector3.Lerp(inScale, outScale, easeCurretTime / easeTime);
		}
		else
		{
			if (easePosition) currentPosition += Vector3.Normalize(inPosition - outPosition) * easeSpeed;
			if (easeRotation) currentRotation += Vector3.Normalize(inRotation - outRotation) * easeSpeed;
			if (easeScale) currentScale += Vector3.Normalize(inScale - outScale) * easeSpeed;
		}

		if (easePosition) transform.localPosition = currentPosition;
		if (easeRotation) transform.localRotation = Quaternion.Euler(currentRotation);
		if (easeScale) transform.localScale = currentScale;
	}

}
