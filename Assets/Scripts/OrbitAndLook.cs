using UnityEngine;
using System.Collections;

public class OrbitAndLook : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public Transform target;
	public Transform targetArea;
	public Transform ghost;
	public Transform pivot;

	//////////////////////////////////////////////////////////////
	///////////////////////// PARAMETERS /////////////////////////
	//////////////////////////////////////////////////////////////
	public bool automaticDistance = true;
	public float distance = 22.0f;
	public float smoothTime = 1.0f;

	public PLACE fixedPlace = PLACE.FRONT;
	public PLACE currentUp = PLACE.TOP;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	public enum PLACE { FRONT, BACK, RIGHT, LEFT, TOP, BOTTOM };
	private Vector3 curretGhostUp = Vector3.zero, targetGhostUp = new Vector3(0.0f, 1.0f, 0.0f);
	private Vector3 ghostVelocityUp = Vector3.zero;
	private Vector3 velocity = Vector3.zero;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void FixedUpdate()
	{
		// Distance
		if (automaticDistance)
		{
			Camera cam = GetComponent<Camera>();
			float sideSize = targetArea.transform.localScale.x;
			float frustumHeight = sideSize + 2.0f;
			if (cam.pixelHeight > cam.pixelWidth )
			{
				frustumHeight = frustumHeight / cam.aspect;
			}

			distance = frustumHeight * 0.5f / Mathf.Tan(GetComponent<Camera>().fieldOfView * 0.5f * Mathf.Deg2Rad) + Mathf.Sqrt(sideSize * sideSize + sideSize * sideSize) / 2.0f;
		}

		PLACE place = GetPlace();
		SetGhostUp(fixedPlace, place);
		SetGhostPosition(place);
		RotateFromTraget(place);

		fixedPlace = place;

		transform.position = Vector3.SmoothDamp(transform.position, ghost.position, ref velocity, smoothTime);
		curretGhostUp = Vector3.SmoothDamp(curretGhostUp, targetGhostUp, ref ghostVelocityUp, smoothTime * 2.0f);
		transform.LookAt(target, curretGhostUp);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////// ROTATION AND LOOK ////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	PLACE GetPlace()
	{
		float x = target.transform.localPosition.x;
		float y = target.transform.localPosition.y;
		float z = target.transform.localPosition.z;
		float absX = Mathf.Abs(x);
		float absY = Mathf.Abs(y);
		float absZ = Mathf.Abs(z);


		if ((absZ > absX && absZ > absY) || (absZ >= absX && absZ > absY) || (absZ > absX && absZ >= absY))
			return z >= 0.0f ? PLACE.BACK : PLACE.FRONT;

		if ((absX > absZ && absX > absY) || (absX >= absZ && absX > absY) || (absX > absZ && absX >= absY))
			return x >= 0.0f ? PLACE.RIGHT : PLACE.LEFT;

		if ((absY > absX && absY > absZ) || (absY >= absX && absY > absZ) || (absY > absX && absY >= absZ))
			return y >= 0.0f ? PLACE.TOP : PLACE.BOTTOM;

		return 0;
	}

	void SetGhostPosition(PLACE place)
	{
		if (place == PLACE.FRONT)
		{
			ghost.localPosition = new Vector3(0.0f, 0.0f, -distance);
		}
		else if (place == PLACE.BACK)
		{
			ghost.localPosition = new Vector3(0.0f, 0.0f, distance);
		}
		else if (place == PLACE.RIGHT)
		{
			ghost.localPosition = new Vector3(distance, 0.0f, 0.0f);
		}
		else if (place == PLACE.LEFT)
		{
			ghost.localPosition = new Vector3(-distance, 0.0f, 0.0f);
		}
		else if (place == PLACE.TOP)
		{
			ghost.localPosition = new Vector3(0.0f, distance, 0.0f);
		}
		else if (place == PLACE.BOTTOM)
		{
			ghost.localPosition = new Vector3(0.0f, -distance, 0.0f);
		}
	}

	void SetGhostUp(PLACE lastPlace, PLACE place)
	{
		if (lastPlace == place) return;

		if(lastPlace == PLACE.FRONT)
		{
			if (place == PLACE.RIGHT)
			{
				if (currentUp == PLACE.RIGHT)
				{
					currentUp = PLACE.BACK;
					targetGhostUp = new Vector3(0.0f, 0.0f, 1.0f);
				}
				else if (currentUp == PLACE.LEFT)
				{
					currentUp = PLACE.FRONT;
					targetGhostUp = new Vector3(0.0f, 0.0f, -1.0f);
				}
			}
			else if (place == PLACE.LEFT)
			{
				if (currentUp == PLACE.RIGHT)
				{
					currentUp = PLACE.FRONT;
					targetGhostUp = new Vector3(0.0f, 0.0f, -1.0f);
				}
				else if (currentUp == PLACE.LEFT)
				{
					currentUp = PLACE.BACK;
					targetGhostUp = new Vector3(0.0f, 0.0f, 1.0f);
				}
			}
			else if (place == PLACE.TOP)
			{
				if (currentUp == PLACE.TOP)
				{
					currentUp = PLACE.BACK;
					targetGhostUp = new Vector3(0.0f, 0.0f, 1.0f);
				}
				else if (currentUp == PLACE.BOTTOM)
				{
					currentUp = PLACE.FRONT;
					targetGhostUp = new Vector3(0.0f, 0.0f, -1.0f);
				}
			}
			else if (place == PLACE.BOTTOM)
			{
				if (currentUp == PLACE.TOP)
				{
					currentUp = PLACE.FRONT;
					targetGhostUp = new Vector3(0.0f, 0.0f, -1.0f);
				}
				else if (currentUp == PLACE.BOTTOM)
				{
					currentUp = PLACE.BACK;
					targetGhostUp = new Vector3(0.0f, 0.0f, 1.0f);
				}
			}
		}
		else if (lastPlace == PLACE.BACK)
		{
			if (place == PLACE.RIGHT)
			{
				if (currentUp == PLACE.RIGHT)
				{
					currentUp = PLACE.FRONT;
					targetGhostUp = new Vector3(0.0f, 0.0f, -1.0f);
				}
				else if (currentUp == PLACE.LEFT)
				{
					currentUp = PLACE.BACK;
					targetGhostUp = new Vector3(0.0f, 0.0f, 1.0f);
				}
			}
			else if (place == PLACE.LEFT)
			{
				if (currentUp == PLACE.RIGHT)
				{
					currentUp = PLACE.BACK;
					targetGhostUp = new Vector3(0.0f, 0.0f, 1.0f);
				}
				else if (currentUp == PLACE.LEFT)
				{
					currentUp = PLACE.FRONT;
					targetGhostUp = new Vector3(0.0f, 0.0f, -1.0f);
				}
			}
			else if (place == PLACE.TOP)
			{
				if (currentUp == PLACE.TOP)
				{
					currentUp = PLACE.FRONT;
					targetGhostUp = new Vector3(0.0f, 0.0f, -1.0f);
				}
				else if (currentUp == PLACE.BOTTOM)
				{
					currentUp = PLACE.BACK;
					targetGhostUp = new Vector3(0.0f, 0.0f, 1.0f);
				}
			}
			else if (place == PLACE.BOTTOM)
			{
				if (currentUp == PLACE.TOP)
				{
					currentUp = PLACE.BACK;
					targetGhostUp = new Vector3(0.0f, 0.0f, 1.0f);
				}
				else if (currentUp == PLACE.BOTTOM)
				{
					currentUp = PLACE.FRONT;
					targetGhostUp = new Vector3(0.0f, 0.0f, -1.0f);
				}
			}
		}
		else if (lastPlace == PLACE.RIGHT)
		{
			if (place == PLACE.FRONT)
			{
				if (currentUp == PLACE.FRONT)
				{
					currentUp = PLACE.LEFT;
					targetGhostUp = new Vector3(-1.0f, 0.0f, 0.0f);
				}
				else if (currentUp == PLACE.BACK)
				{
					currentUp = PLACE.RIGHT;
					targetGhostUp = new Vector3(1.0f, 0.0f, 0.0f);
				}
			}
			else if (place == PLACE.BACK)
			{
				if (currentUp == PLACE.FRONT)
				{
					currentUp = PLACE.RIGHT;
					targetGhostUp = new Vector3(1.0f, 0.0f, 0.0f);
				}
				else if (currentUp == PLACE.BACK)
				{
					currentUp = PLACE.LEFT;
					targetGhostUp = new Vector3(-1.0f, 0.0f, 0.0f);
				}
			}
			else if (place == PLACE.TOP)
			{
				if (currentUp == PLACE.TOP)
				{
					currentUp = PLACE.LEFT;
					targetGhostUp = new Vector3(-1.0f, 0.0f, 0.0f);
				}
				else if (currentUp == PLACE.BOTTOM)
				{
					currentUp = PLACE.RIGHT;
					targetGhostUp = new Vector3(1.0f, 0.0f, 0.0f);
				}
			}
			else if (place == PLACE.BOTTOM)
			{
				if (currentUp == PLACE.TOP)
				{
					currentUp = PLACE.RIGHT;
					targetGhostUp = new Vector3(1.0f, 0.0f, 0.0f);
				}
				else if (currentUp == PLACE.BOTTOM)
				{
					currentUp = PLACE.LEFT;
					targetGhostUp = new Vector3(-1.0f, 0.0f, 0.0f);
				}
			}
		}
		else if (lastPlace == PLACE.LEFT)
		{
			if (place == PLACE.FRONT)
			{
				if (currentUp == PLACE.FRONT)
				{
					currentUp = PLACE.RIGHT;
					targetGhostUp = new Vector3(1.0f, 0.0f, 0.0f);
				}
				else if (currentUp == PLACE.BACK)
				{
					currentUp = PLACE.LEFT;
					targetGhostUp = new Vector3(-1.0f, 0.0f, 0.0f);
				}
			}
			else if (place == PLACE.BACK)
			{
				if (currentUp == PLACE.FRONT)
				{
					currentUp = PLACE.LEFT;
					targetGhostUp = new Vector3(-1.0f, 0.0f, 0.0f);
				}
				else if (currentUp == PLACE.BACK)
				{
					currentUp = PLACE.RIGHT;
					targetGhostUp = new Vector3(1.0f, 0.0f, 0.0f);
				}
			}
			else if (place == PLACE.TOP)
			{
				if (currentUp == PLACE.TOP)
				{
					currentUp = PLACE.RIGHT;
					targetGhostUp = new Vector3(1.0f, 0.0f, 0.0f);
				}
				else if (currentUp == PLACE.BOTTOM)
				{
					currentUp = PLACE.LEFT;
					targetGhostUp = new Vector3(-1.0f, 0.0f, 0.0f);
				}
			}
			else if (place == PLACE.BOTTOM)
			{
				if (currentUp == PLACE.TOP)
				{
					currentUp = PLACE.LEFT;
					targetGhostUp = new Vector3(-1.0f, 0.0f, 0.0f);
				}
				else if (currentUp == PLACE.BOTTOM)
				{
					currentUp = PLACE.RIGHT;
					targetGhostUp = new Vector3(1.0f, 0.0f, 0.0f);
				}
			}
		}
		else if (lastPlace == PLACE.TOP)
		{
			if (place == PLACE.FRONT)
			{
				if (currentUp == PLACE.FRONT)
				{
					currentUp = PLACE.BOTTOM;
					targetGhostUp = new Vector3(0.0f, -1.0f, 0.0f);
				}
				else if (currentUp == PLACE.BACK)
				{
					currentUp = PLACE.TOP;
					targetGhostUp = new Vector3(0.0f, 1.0f, 0.0f);
				}
			}
			else if (place == PLACE.BACK)
			{
				if (currentUp == PLACE.FRONT)
				{
					currentUp = PLACE.TOP;
					targetGhostUp = new Vector3(0.0f, 1.0f, 0.0f);
				}
				else if (currentUp == PLACE.BACK)
				{
					currentUp = PLACE.BOTTOM;
					targetGhostUp = new Vector3(0.0f, -1.0f, 0.0f);
				}
			}
			else if (place == PLACE.RIGHT)
			{
				if (currentUp == PLACE.RIGHT)
				{
					currentUp = PLACE.BOTTOM;
					targetGhostUp = new Vector3(0.0f, -1.0f, 0.0f);
				}
				else if (currentUp == PLACE.LEFT)
				{
					currentUp = PLACE.TOP;
					targetGhostUp = new Vector3(0.0f, 1.0f, 0.0f);
				}
			}
			else if (place == PLACE.LEFT)
			{
				if (currentUp == PLACE.RIGHT)
				{
					currentUp = PLACE.TOP;
					targetGhostUp = new Vector3(0.0f, 1.0f, 0.0f);
				}
				else if (currentUp == PLACE.LEFT)
				{
					currentUp = PLACE.BOTTOM;
					targetGhostUp = new Vector3(0.0f, -1.0f, 0.0f);
				}
			}
		}
		else if (lastPlace == PLACE.BOTTOM)
		{
			if (place == PLACE.FRONT)
			{
				if (currentUp == PLACE.FRONT)
				{
					currentUp = PLACE.TOP;
					targetGhostUp = new Vector3(0.0f, 1.0f, 0.0f);
				}
				else if (currentUp == PLACE.BACK)
				{
					currentUp = PLACE.BOTTOM;
					targetGhostUp = new Vector3(0.0f, -1.0f, 0.0f);
				}
			}
			else if (place == PLACE.BACK)
			{
				if (currentUp == PLACE.FRONT)
				{
					currentUp = PLACE.BOTTOM;
					targetGhostUp = new Vector3(0.0f, -1.0f, 0.0f);
				}
				else if (currentUp == PLACE.BACK)
				{
					currentUp = PLACE.TOP;
					targetGhostUp = new Vector3(0.0f, 1.0f, 0.0f);
				}
			}
			else if (place == PLACE.RIGHT)
			{
				if (currentUp == PLACE.RIGHT)
				{
					currentUp = PLACE.TOP;
					targetGhostUp = new Vector3(0.0f, 1.0f, 0.0f);
				}
				else if (currentUp == PLACE.LEFT)
				{
					currentUp = PLACE.BOTTOM;
					targetGhostUp = new Vector3(0.0f, -1.0f, 0.0f);
				}
			}
			else if (place == PLACE.LEFT)
			{
				if (currentUp == PLACE.RIGHT)
				{
					currentUp = PLACE.BOTTOM;
					targetGhostUp = new Vector3(0.0f, -1.0f, 0.0f);
				}
				else if (currentUp == PLACE.LEFT)
				{
					currentUp = PLACE.TOP;
					targetGhostUp = new Vector3(0.0f, 1.0f, 0.0f);
				}
			}
		}
	}

	void RotateFromTraget(PLACE place)
	{
		Vector3 targetPosition = target.transform.localPosition;
		Vector2 newRotation;
		if (place == PLACE.BACK)
		{
			newRotation = CameraRotationBase(targetPosition.z, targetPosition.x, -targetPosition.y);
			pivot.rotation = Quaternion.Euler(newRotation.x, newRotation.y, 0.0f);
		}
		if (place == PLACE.FRONT)
		{
			newRotation = CameraRotationBase(targetPosition.z, targetPosition.x, targetPosition.y);
			pivot.rotation = Quaternion.Euler(newRotation.x, newRotation.y, 0.0f);
		}
		if (place == PLACE.RIGHT)
		{
			newRotation = CameraRotationBase(targetPosition.x, targetPosition.y, -targetPosition.z);
			pivot.rotation = Quaternion.Euler(0.0f, newRotation.x, newRotation.y);
		}
		if (place == PLACE.LEFT)
		{
			newRotation = CameraRotationBase(targetPosition.x, targetPosition.y, targetPosition.z);
			pivot.rotation = Quaternion.Euler(0.0f, newRotation.x, newRotation.y);
		}
		if (place == PLACE.TOP)
		{
			newRotation = CameraRotationBase(targetPosition.y, -targetPosition.x, targetPosition.z);
			pivot.rotation = Quaternion.Euler(newRotation.x, 0.0f, newRotation.y);
		}
		if (place == PLACE.BOTTOM)
		{
			newRotation = CameraRotationBase(targetPosition.y, -targetPosition.x, -targetPosition.z);
			pivot.rotation = Quaternion.Euler(newRotation.x, 0.0f, newRotation.y);
		}
	}

	Vector2 CameraRotationBase(float baseValue, float horizontal, float vertical)
	{
		float anglexz;
		float spherey;

		anglexz = horizontal == 0.0f || baseValue == 0.0f ? 0.0f : horizontal / baseValue;
	
		// New value for the sphere y angle.
		spherey = Mathf.Rad2Deg * Mathf.Atan(anglexz);
	
		// New value for the sphere x angle.
		float auxhy = Mathf.Sqrt(Mathf.Pow(horizontal, 2.0f) + Mathf.Pow(baseValue, 2.0f));
		float xangle = Mathf.Rad2Deg * Mathf.Atan(vertical / auxhy);
	
		// Set the new pivot rotation base these angles.
		return new Vector2(xangle, spherey);
	}

}
