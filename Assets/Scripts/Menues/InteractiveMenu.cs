using UnityEngine;

public class InteractiveMenu : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public OrbitAndLook menuCamera;
	public EasePosition cubilineTilte;
	public EasePosition actionTitleBase;
	public TextMesh actionTitle;
	public GameObject playModel;
	public EasePosition focalTarget;

	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////

	public enum MENU_ACTION { PLAY, NONE }

	public MENU_ACTION selectedAction;
	public MENU_ACTION goingAction;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	//////////////////////// CUBE ROTATION ///////////////////////

	private Vector2 actionRotation = Vector3.zero;
	private Vector3 currentRotation = Vector3.zero;
	private Vector3 targetRotation = Vector3.zero;
	private Vector3 inRotation = Vector3.zero;
	private Vector3 rotationVelocity = Vector3.zero;

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

	void FixedUpdate()
	{
		if (goingAction == MENU_ACTION.NONE)
		{
			currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation + inRotation, ref rotationVelocity, 0.15f);
			transform.localRotation = Quaternion.Euler(currentRotation);
		}
		else if (goingAction == MENU_ACTION.PLAY)
		{
			if (focalTarget.transform.localPosition == focalTarget.outValues)
				Application.LoadLevel(3);
		}
	}

	void OnMouseDown()
	{
		actionReady = true;
		if (currentModelAction != null)
			currentModelAction.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
	}

	void OnMouseDrag()
	{
		float axis = Input.GetAxis("Horizontal Mouse");
		inRotation.y -= axis * 0.1f;
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
			if (selectedAction == MENU_ACTION.PLAY)
			{
				cubilineTilte.easeFace = EaseVector3.EASE_FACE.OUT;
				actionTitleBase.easeFace = EaseVector3.EASE_FACE.OUT;
				focalTarget.easeFace = EaseVector3.EASE_FACE.OUT;
				menuCamera.automaticDistance = false;
				goingAction = MENU_ACTION.PLAY;
			}
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
		else
		{
			selectedAction = MENU_ACTION.NONE;
			currentModelAction = null;
			actionTitle.text = "None";
		}
	}

}