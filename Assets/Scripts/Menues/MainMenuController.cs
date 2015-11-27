﻿using UnityEngine;

public class MainMenuController : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public Transform cubeMenu;
	public EasePosition cubilineTilte;
	public TextMesh actionTitle;
	public GameObject playModel;
	public GameObject coopModel;
	public EasePosition focalTarget;
	public OrbitAndLook menuCamera;

	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////

	public enum MENU_ACTION { PLAY, COOP, NONE }

	public MENU_ACTION selectedAction;
	public MENU_ACTION goingAction;
	public float slideSencibility = 0.15f;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	//////////////////////// CUBE ROTATION ///////////////////////

	private Vector2 actionRotation = Vector3.zero;
	private Vector3 currentRotation = Vector3.zero;
	private Vector3 targetRotation = Vector3.zero;
	private Vector3 inRotation = Vector3.zero;
	private Vector3 rotationVelocity = Vector3.zero;
	private Vector2 lastMousePosition;

	/////////////////////////// ACTION ///////////////////////////
	private GameObject currentModelAction;
	private bool actionReady;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		SetAction();
		goingAction = MENU_ACTION.NONE;
	}

	void Update()
	{
		if (goingAction == MENU_ACTION.NONE)
		{
			currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation + inRotation, ref rotationVelocity, 0.15f);
			cubeMenu.localRotation = Quaternion.Euler(currentRotation);
		}
		else if (focalTarget.transform.localPosition == focalTarget.outValues)
		{
			if (goingAction == MENU_ACTION.PLAY)
				Application.LoadLevel(4);
			else if (goingAction == MENU_ACTION.COOP)
				Application.LoadLevel(3);
		}
	}

	void OnGUI()
	{
		Event e = Event.current;
		if (e.type == EventType.keyDown)
		{
			if (e.keyCode == KeyCode.RightArrow)
			{
				targetRotation.y += 90;
				actionRotation.y += 90;
				SetAction();
			}
			else if (e.keyCode == KeyCode.LeftArrow)
			{
				targetRotation.y -= 90;
				actionRotation.y -= 90;
				SetAction();
			}
			else if(e.keyCode == KeyCode.Space)
			{
				cubilineTilte.easeFace = EaseVector3.EASE_FACE.OUT;
				actionTitle.GetComponent<EasePosition>().easeFace = EaseVector3.EASE_FACE.OUT;
				focalTarget.easeFace = EaseVector3.EASE_FACE.OUT;
				menuCamera.automaticDistance = false;
				goingAction = selectedAction;
			}
		}
	}

	void OnMouseDown()
	{
		lastMousePosition = Input.mousePosition;
		actionReady = true;
		if (currentModelAction != null)
			currentModelAction.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
	}

	void OnMouseDrag()
	{
		float axis = 0;
		if (Input.touchCount > 0)
		{
			axis = Input.GetTouch(0).deltaPosition.x;
		}
		else
		{
			Vector2 currentMousePosition = Input.mousePosition;
			axis = (currentMousePosition - lastMousePosition).x;
			lastMousePosition = currentMousePosition;
		}
		inRotation.y -= axis * slideSencibility;
		actionRotation.y = targetRotation.y + (inRotation.y > 0 ? Mathf.Ceil(((int)inRotation.y / 45) / 2.0f) : Mathf.Floor(((int)inRotation.y / 45) / 2.0f)) * 90;
		SetAction();
		if (axis != 0)
		{
			actionReady = false;
			if (currentModelAction != null) currentModelAction.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
		}
	}

	void OnMouseUp()
	{
		targetRotation.y += (inRotation.y > 0 ? Mathf.Ceil(((int)inRotation.y / 45) / 2.0f) : Mathf.Floor(((int)inRotation.y / 45) / 2.0f)) * 90;
		inRotation.y = 0;
	}

	void OnMouseUpAsButton()
	{
		if (actionReady)
		{
			cubilineTilte.easeFace = EaseVector3.EASE_FACE.OUT;
			actionTitle.GetComponent<EasePosition>().easeFace = EaseVector3.EASE_FACE.OUT;
			focalTarget.easeFace = EaseVector3.EASE_FACE.OUT;
			menuCamera.automaticDistance = false;
			goingAction = selectedAction;
		}
		else
		{
			if (currentModelAction != null) currentModelAction.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////// ACTION ////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void SetAction()
	{
		float fixedY = Mathf.Repeat(actionRotation.y, 360.0f);
		if (fixedY == 0.0f || fixedY == 360.0f)
		{
			selectedAction = MENU_ACTION.PLAY;
			currentModelAction = playModel;
			actionTitle.text = "Play";
		}
		else if (fixedY == 90.0f)
		{
			selectedAction = MENU_ACTION.COOP;
			currentModelAction = coopModel;
			actionTitle.text = "Coop";
		}
		else
		{
			selectedAction = MENU_ACTION.NONE;
			currentModelAction = null;
			actionTitle.text = "None";
		}
	}

}