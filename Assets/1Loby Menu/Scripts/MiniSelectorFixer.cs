using UnityEngine;
using UnityEngine.UI;

public class MiniSelectorFixer : MonoBehaviour
{

	public Transform content;

	private Vector3 targetPosition = Vector3.zero;
	private Vector3 velocity = Vector3.zero;
	private bool interactig;

	void Start ()
	{
	
	}

	void Update ()
	{
		if(!interactig)
			content.localPosition = Vector3.SmoothDamp(content.localPosition, targetPosition, ref velocity, 0.1f);

		if (!Input.GetMouseButton(0) && Input.touchCount == 0)
		{
			interactig = false;
		}
			
	}

	public void ResetVelocity()
	{
		interactig = true;

		float targetX = Mathf.Round(content.localPosition.x / 4.8f) * 4.8f;

		targetPosition = new Vector3(targetX, 0.0f, 0.0f);
	}
}
