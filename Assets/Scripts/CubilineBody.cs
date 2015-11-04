using UnityEngine;
using System.Collections;

public class CubilineBody : MonoBehaviour
{
	public CubilineController.PLACE bodyZone;
	public CubilineController.PLACE bodyDirection;

	public void initialize(CubilineController.PLACE zone, CubilineController.PLACE direction, Vector3 bornPosition, float size)
	{
		Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);

		bodyZone = zone;
		bodyDirection = direction;

		if (bodyDirection == CubilineController.PLACE.FRONT)
		{
			scale.z = size;
			bornPosition.z += size / 2.0f + 0.5f;
		}
		if (bodyDirection == CubilineController.PLACE.BACK)
		{
			scale.z = size;
			bornPosition.z -= size / 2.0f + 0.5f;
		}
		if (bodyDirection == CubilineController.PLACE.RIGHT)
		{
			scale.x = size;
			bornPosition.x -= size / 2.0f + 0.5f;
		}
		if (bodyDirection == CubilineController.PLACE.LEFT)
		{
			scale.x = size;
			bornPosition.x += size / 2.0f + 0.5f;
		}
		if (bodyDirection == CubilineController.PLACE.TOP)
		{
			scale.y = size;
			bornPosition.y -= size / 2.0f + 0.5f;
		}
		if (bodyDirection == CubilineController.PLACE.BOTTOM)
		{
			scale.y = size;
			bornPosition.y += size / 2.0f + 0.5f;
		}

		if (size == 0.0f)
			GetComponent<MeshRenderer>().enabled = false;

		// Set initial transformations.
		transform.localScale = scale;
		transform.localPosition = bornPosition;
	}

	public float Grow(float delta)
	{
		Vector3 scale = transform.localScale;
		Vector3 position = transform.localPosition;
		float final = 0.0f;

		if (bodyDirection == CubilineController.PLACE.FRONT)
		{
			scale.z += delta;
			final = scale.z;

			position.z -= Mathf.Abs(delta / 2.0f);
		}
		else if (bodyDirection == CubilineController.PLACE.BACK)
		{
			scale.z += delta;
			final = scale.z;

			position.z += Mathf.Abs(delta / 2.0f);
		}
		else if (bodyDirection == CubilineController.PLACE.RIGHT)
		{
			scale.x += delta;
			final = scale.x;

			position.x += Mathf.Abs(delta / 2.0f);
		}
		else if (bodyDirection == CubilineController.PLACE.LEFT)
		{
			scale.x += delta;
			final = scale.x;

			position.x -= Mathf.Abs(delta / 2.0f);
		}
		else if (bodyDirection == CubilineController.PLACE.TOP)
		{
			scale.y += delta;
			final = scale.y;

			position.y += Mathf.Abs(delta / 2.0f);
		}
		else if (bodyDirection == CubilineController.PLACE.BOTTOM)
		{
			scale.y += delta;
			final = scale.y;

			position.y -= Mathf.Abs(delta / 2.0f);
		}

		if (scale.x == 0.0f || scale.y == 0.0f || scale.z == 0.0f)
			GetComponent<MeshRenderer>().enabled = false;
		else
			GetComponent<MeshRenderer>().enabled = true;

		transform.localScale = scale;
		transform.localPosition = position;

		return final;
	}
}
