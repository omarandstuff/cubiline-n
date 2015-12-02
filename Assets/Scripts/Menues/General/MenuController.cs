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
	public EasePosition focalTarget;
	public GameObject frontActionContentPrefab;
	public GameObject BackActionContentPrefab;
	public GameObject RightActionContentPrefab;
	public GameObject LeftActionContentPrefab;

	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////

	public float bigPanoramaDistance = 2.0f;
	public float smallPanoramaDistance = 1.0f;
	public enum STATUS { SELECTING, IN_ACTION }
	public STATUS status = STATUS.SELECTING;
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
	private bool outGoing;

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
		if (!outGoing) // Rotate cube while not action called.
		{
			currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation + inRotation, ref rotationVelocity, cubeRotationSmoothTime);
			cubeMenu.localRotation = Quaternion.Euler(currentRotation);
		}
		else if (focalTarget.transform.localPosition == focalTarget.outValues)
		{
			Application.LoadLevel(currentModelAction.GetComponent<ActionContentController>().SceneName);
		}
	}

	void OnGUI()
	{
		if (status != STATUS.SELECTING && !outGoing) return;

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
				SelectAction();
			}
		}
	}

	void OnMouseDown()
	{
		lastMousePosition = Input.mousePosition;
		actionReady = true;
		if (currentModelAction != null && currentModelAction.GetComponent<EaseScale>() != null) currentModelAction.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
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
			if (currentModelAction != null && currentModelAction.GetComponent<EaseScale>() != null) currentModelAction.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
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
			SelectAction();
		else if (currentModelAction != null && currentModelAction.GetComponent<EaseScale>() != null)
			currentModelAction.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
	}

	void JoyStickInput()
	{
		if (status != STATUS.SELECTING && !outGoing) return;

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
		if(frontActionContentPrefab != null)
		{
			frontActionCountent = Instantiate(frontActionContentPrefab, Vector3.back * 5.01f, Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f))) as GameObject;
			frontActionCountent.transform.parent = cubeMenu;
		}
		if (BackActionContentPrefab != null)
		{
			BackActionCountent = Instantiate(BackActionContentPrefab, Vector3.forward * 5.01f, Quaternion.identity) as GameObject;
			BackActionCountent.transform.parent = cubeMenu;
		}
		if (RightActionContentPrefab != null)
		{
			RighttActionCountent = Instantiate(RightActionContentPrefab, Vector3.right * 5.01f, Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f))) as GameObject;
			RighttActionCountent.transform.parent = cubeMenu;
		}
		if (LeftActionContentPrefab != null)
		{
			LeftActionCountent = Instantiate(LeftActionContentPrefab, Vector3.left * 5.01f, Quaternion.Euler(new Vector3(0.0f, 270.0f, 0.0f))) as GameObject;
			LeftActionCountent.transform.parent = cubeMenu;
		}
		SetAction();
		menuCamera.GetComponent<OrbitAndLook>().automaticDistanceOffset = bigPanoramaDistance;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////// ACTION ////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void SelectAction()
	{
		if(currentModelAction != null)
		{
			ActionContentController action = currentModelAction.GetComponent<ActionContentController>();
			if (action.contentType == ActionContentController.CONTENT_TYPE.SMALL_CONTENT)
			{
				menuCamera.GetComponent<OrbitAndLook>().automaticDistanceOffset = smallPanoramaDistance;
				action.Select();
				GetComponent<Collider>().enabled = false;
			}
			else if (action.contentType == ActionContentController.CONTENT_TYPE.TO_SCENE)
			{
	
				focalTarget.easeFace = EaseVector3.EASE_FACE.OUT;
				menuCamera.GetComponent<OrbitAndLook>().automaticDistance = false;
				outGoing = true;
				GetComponent<Collider>().enabled = false;
			}
		}
	}

	void BackFromAction()
	{
		menuCamera.GetComponent<OrbitAndLook>().automaticDistanceOffset = bigPanoramaDistance;
		currentModelAction.GetComponent<ActionContentController>().Unselect();
		GetComponent<Collider>().enabled = false;
	}

	void SetAction()
	{
		float fixedY = Mathf.Repeat(actionRotation.y, 360.0f);
		if (fixedY == 0.0f || fixedY == 360.0f)
		{
			ActionText.text = frontActionText;
			if (frontActionCountent != currentModelAction)
			{
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Leave();
				currentModelAction = frontActionCountent;
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Enter();
			}
		}
		else if (fixedY == 90.0f)
		{
			ActionText.text = rightActionText;
			if (RighttActionCountent != currentModelAction)
			{
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Leave();
				currentModelAction = RighttActionCountent;
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Enter();
			}
		}
		else if (fixedY == 180.0f)
		{
			ActionText.text = backActionText;
			if (BackActionCountent != currentModelAction)
			{
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Leave();
				currentModelAction = BackActionCountent;
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Enter();
			}
		}
		else
		{
			ActionText.text = leftActionText;
			if (LeftActionCountent != currentModelAction)
			{
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Leave();
				currentModelAction = LeftActionCountent;
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Enter();
			}
		}
	}
}
