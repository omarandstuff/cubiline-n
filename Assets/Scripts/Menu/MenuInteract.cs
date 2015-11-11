using UnityEngine;
using System.Collections;

public class MenuInteract : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public Transform cubeMenu;
	public Camera menuCamera;
	public GameObject playModel;

	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	public enum MenuAction { PLAY, NONE}

	private MenuAction action;

	private Vector3 currentRotation = Vector3.zero;
	private Vector3 targetRotation = Vector3.zero;
	private Vector3 inRotation = Vector3.zero;
	private Vector3 rotationVelocity = Vector3.zero;

	private GameObject targetModel;
	private Vector3 baseScale;
	private Vector3 targetScale;
	private Vector3 curretScale;
	private Vector3 scaleVelocity = Vector3.zero;

	private Vector3 playModelBaseScale;

	private MenuAction pressAction;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		menuCamera.transform.position = new Vector3(-20, 0, -80);
		targetModel = playModel;
		
		playModelBaseScale = playModel.transform.localScale;
		baseScale = playModelBaseScale;
		targetScale = baseScale;
		curretScale = targetScale;
	}

	void FixedUpdate()
	{
		currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation + inRotation, ref rotationVelocity, 0.15f);
		curretScale = Vector3.SmoothDamp(curretScale, targetScale, ref scaleVelocity, 0.08f);

		cubeMenu.localRotation = Quaternion.Euler(currentRotation);
		targetModel.transform.localScale = curretScale;
	}

	void OnMouseDown()
	{
		targetScale = baseScale * 0.7f;
	}

	void OnMouseDrag()
	{
		float axis = Input.GetAxis("Horizontal Mouse");
		inRotation.y -= axis * 0.3f;
		if (axis != 0)
			targetScale = baseScale;
	}

	void OnMouseUp()
	{
		targetRotation.y += (inRotation.y > 0 ? Mathf.Ceil(((int)inRotation.y / 45) / 2.0f) : Mathf.Floor(((int)inRotation.y / 45) / 2.0f)) * 90;
		inRotation.y = 0;
		targetScale = baseScale;
	}

	void OnMouseUpAsButton()
	{
		if(pressAction == action)
		{
			if (action == MenuAction.PLAY)
				Application.LoadLevelAsync(1);
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// MENU /////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void SetAction(Vector3 rotation)
	{
		rotation.y = Mathf.Repeat(rotation.y, 360);

		if(rotation.y == 0 || rotation.y == 360)
		{
			action = MenuAction.PLAY;
		}
		else
		{
			action = MenuAction.NONE;
		}
	}
}
