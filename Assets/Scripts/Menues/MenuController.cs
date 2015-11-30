using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public Transform cubeMenu;
	public Camera menuCamera;
	public Text ActionText;
	public GameObject frontActionCoutentPrefab;
	public GameObject BackActionCoutentPrefab;
	public GameObject RightActionCoutentPrefab;
	public GameObject LeftActionCoutentPrefab;

	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////

	public enum MENU_ACTION { NONE_ACTION, FRONT_ACTION, RIGHT_ACTION, LEFT_ACTION, BACK_ACTION }
	private MENU_ACTION selectedAction;
	private MENU_ACTION goingAction;
	public float slideSencibility = 1.0f;
	public float cubeRotationSmoothTime = 0.15f;

	public string frontActionText;
	public string backActionText;
	public string rightActionText;
	public string leftActionText;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	//////////////////////// SIDE CONTENTS ///////////////////////

	private GameObject frontActionCountent;
	private GameObject BackActionCountent;
	private GameObject RighttActionCountent;
	private GameObject LeftActionCountent;

	//////////////////////// CUBE ROTATION ///////////////////////

	private int lastJoyStick;
	
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
		SetupMenu();
	}

	void Update()
	{
		// Check joystick input.
		JoyStickInput();

		if (goingAction == MENU_ACTION.NONE_ACTION) // Rotate cube while not action called.
		{
			currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation + inRotation, ref rotationVelocity, cubeRotationSmoothTime);
			cubeMenu.localRotation = Quaternion.Euler(currentRotation);
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
			else if (e.keyCode == KeyCode.Space)
			{
				goingAction = selectedAction;
			}
		}
	}

	void OnMouseDown()
	{
		lastMousePosition = Input.mousePosition;
		actionReady = true;
		if (currentModelAction != null) currentModelAction.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
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
		inRotation.y -= ((90.0f * axis) / menuCamera.pixelWidth) * slideSencibility;
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
			goingAction = selectedAction;
		}
		else
		{
			if (currentModelAction != null) currentModelAction.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
		}
	}

	void JoyStickInput()
	{
		int joyInput = Input.GetAxis("Horizontal") > 0.5f ? 1 : (Input.GetAxis("Horizontal") < -0.5f ? -1 : 0);
		if(joyInput != lastJoyStick)
		{
			if (joyInput > 0)
			{
				targetRotation.y += 90;
				actionRotation.y += 90;
			}
			else if (joyInput < 0)
			{
				targetRotation.y -= 90;
				actionRotation.y -= 90;
			}
			SetAction();
			lastJoyStick = joyInput;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////// SETUP ////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void SetupMenu()
	{
		if(frontActionCoutentPrefab != null)
		{
			frontActionCountent = Instantiate(frontActionCoutentPrefab, Vector3.back * 5, Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f))) as GameObject;
			frontActionCountent.transform.parent = cubeMenu;
		}
		else if (BackActionCoutentPrefab != null)
		{
			BackActionCountent = Instantiate(BackActionCoutentPrefab, Vector3.forward * 5, Quaternion.identity) as GameObject;
			BackActionCountent.transform.parent = cubeMenu;
		}
		else if (RighttActionCountent != null)
		{
			RighttActionCountent = Instantiate(RightActionCoutentPrefab, Vector3.right * 5, Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f))) as GameObject;
			RighttActionCountent.transform.parent = cubeMenu;
		}
		else if (LeftActionCoutentPrefab != null)
		{
			LeftActionCountent = Instantiate(LeftActionCoutentPrefab, Vector3.left * 5, Quaternion.Euler(new Vector3(0.0f, 270.0f, 0.0f))) as GameObject;
			LeftActionCountent.transform.parent = cubeMenu;
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
			selectedAction = MENU_ACTION.FRONT_ACTION;
			ActionText.text = frontActionText;
		}
		else if (fixedY == 90.0f)
		{
			selectedAction = MENU_ACTION.RIGHT_ACTION;
			ActionText.text = rightActionText;
		}
		else if (fixedY == 180.0f)
		{
			selectedAction = MENU_ACTION.BACK_ACTION;
			ActionText.text = backActionText;
		}
		else
		{
			selectedAction = MENU_ACTION.LEFT_ACTION;
			ActionText.text = leftActionText;
		}
	}
}
