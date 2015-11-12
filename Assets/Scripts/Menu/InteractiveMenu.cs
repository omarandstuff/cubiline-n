using UnityEngine;

public class InteractiveMenu : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public OrbitAndLook menuCamera;
	public EaseTransform focalTarget;
	public EaseTransform topLight;
	public EaseTransform cubilineTilte;
	public EaseTransform actionTitleBase;
	public TextMesh actionTitle;
	public EaseTransform playModel;

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
	private EaseTransform currentModelAction;
	private bool actionReady;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		menuCamera.transform.localPosition = new Vector3(-10, 0, -50);
		SetAction();
		goingAction = MENU_ACTION.NONE;
	}

	void FixedUpdate()
	{
		if(goingAction == MENU_ACTION.NONE)
		{
			currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation + inRotation, ref rotationVelocity, 0.15f);
			transform.localRotation = Quaternion.Euler(currentRotation);
		}
		else if(goingAction == MENU_ACTION.PLAY)
		{
			if (menuCamera.transform.position.z >= -22)
				Application.LoadLevel(1);
		}
	}

	void OnMouseDown()
	{
		actionReady = true;
		if (currentModelAction != null) currentModelAction.easeFace = EaseTransform.EASE_FACE.OUT;
	}

	void OnMouseDrag()
	{
		float axis = Input.GetAxis("Horizontal Mouse");
		inRotation.y -= axis * 0.3f;
		actionRotation.y = targetRotation.y + (inRotation.y > 0 ? Mathf.Ceil(((int)inRotation.y / 45) / 2.0f) : Mathf.Floor(((int)inRotation.y / 45) / 2.0f)) * 90;
		SetAction();
		if (axis != 0)
		{
			actionReady = false;
			if (currentModelAction != null) currentModelAction.easeFace = EaseTransform.EASE_FACE.IN;
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
				cubilineTilte.easeFace = EaseTransform.EASE_FACE.OUT;
				actionTitleBase.easeFace = EaseTransform.EASE_FACE.OUT;
				menuCamera.automaticDistance = false;
				goingAction = MENU_ACTION.PLAY;
				playModel.easePosition = true;
				playModel.easeFace = EaseTransform.EASE_FACE.OUT;
				topLight.easeFace = EaseTransform.EASE_FACE.OUT;
				focalTarget.easeFace = EaseTransform.EASE_FACE.OUT;
				menuCamera.distance = 20;
				GetComponent<EaseTransform>().outScale = new Vector3(1, 1, 1);
				GetComponent<EaseTransform>().easeFace = EaseTransform.EASE_FACE.OUT;
				GetComponent<Collider>().enabled = false;
			}
		}
		else
		{
			if (currentModelAction != null) currentModelAction.easeFace = EaseTransform.EASE_FACE.IN;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////// ACTION ////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void SetAction()
	{
		float fixedY = Mathf.Repeat(actionRotation.y, 360.0f);
		if(fixedY == 0.0f || fixedY == 360.0f)
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
