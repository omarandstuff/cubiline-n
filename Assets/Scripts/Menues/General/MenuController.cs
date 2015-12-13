using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public Transform cubeMenu;
	public Camera menuCamera;
	public Text ActionText;
	public EasePosition focalTarget;
	public GameObject backButtonPrefab;
	[System.Serializable]public struct Sides { public string frontActionText; public GameObject frontActionContentPrefab; public string backActionText; public GameObject backActionContentPrefab; public string rightActionText; public GameObject rightActionContentPrefab; public string leftActionText; public GameObject leftActionContentPrefab; }
	public Sides[] sides;

	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////
	public float bigPanoramaDistance = 2.0f;
	public float smallPanoramaDistance = 1.0f;
	public enum STATUS { SELECTING, IN_ACTION }
	public STATUS status = STATUS.SELECTING;
	public float slideSencibility = 1.0f;
	public float cubeRotationSmoothTime = 0.15f;
	public string backScene;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	//////////////////////// SIDE CONTENTS ///////////////////////
	private GameObject frontActionCountent;
	private GameObject backActionCountent;
	private GameObject righttActionCountent;
	private GameObject leftActionCountent;
	private string frontActionText;
	private string backActionText;
	private string rightActionText;
	private string leftActionText;

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
	private Sides currentSides;
	private GameObject backButton;
	private Stack<int> backStack = new Stack<int>();

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
			if(currentModelAction != null)
				Application.LoadLevel(currentModelAction.GetComponent<ActionContentController>().SceneName);
			else
				Application.LoadLevel(backScene);
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
		if (joyInput != lastJoyStick)
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

	void LoadSides(int index)
	{
		if (index >= sides.Length) return;
		frontActionText = "None";
		backActionText = "None";
		rightActionText = "None";
		leftActionText = "None";
		if (frontActionCountent != null) Destroy(frontActionCountent);
		if (backActionCountent != null) Destroy(backActionCountent);
		if (righttActionCountent != null) Destroy(righttActionCountent);
		if (leftActionCountent != null) Destroy(leftActionCountent);
		if (sides[index].frontActionContentPrefab != null)
		{
			frontActionCountent = Instantiate(sides[index].frontActionContentPrefab, Vector3.back * 5.01f, Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f))) as GameObject;
			frontActionCountent.transform.parent = cubeMenu;
			frontActionText = sides[index].frontActionText;
		}
		if (sides[index].backActionContentPrefab != null)
		{
			backActionCountent = Instantiate(sides[index].backActionContentPrefab, Vector3.forward * 5.01f, Quaternion.identity) as GameObject;
			backActionCountent.transform.parent = cubeMenu;
			backActionText = sides[index].backActionText;
		}
		if (sides[index].rightActionContentPrefab != null)
		{
			righttActionCountent = Instantiate(sides[index].rightActionContentPrefab, Vector3.right * 5.01f, Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f))) as GameObject;
			righttActionCountent.transform.parent = cubeMenu;
			rightActionText = sides[index].rightActionText;
		}
		if (sides[index].leftActionContentPrefab != null)
		{
			leftActionCountent = Instantiate(sides[index].leftActionContentPrefab, Vector3.left * 5.01f, Quaternion.Euler(new Vector3(0.0f, 270.0f, 0.0f))) as GameObject;
			leftActionCountent.transform.parent = cubeMenu;
			leftActionText = sides[index].leftActionText;
		}
		SetAction();
	}

	void SetupMenu()
	{
		LoadSides(0);
		menuCamera.GetComponent<OrbitAndLook>().automaticDistanceOffset = bigPanoramaDistance;
		if (backButtonPrefab != null)
		{
			backButton = Instantiate(backButtonPrefab);
			backButton.GetComponent<BackButtonDelegate>().parentMenu = this;
			backStack.Push(0);
		}

	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////// ACTION ////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void SelectAction()
	{
		if (currentModelAction != null)
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
			else if (action.contentType == ActionContentController.CONTENT_TYPE.LOAD_SIDE)
			{
				focalTarget.GetComponent<QuickCircle>().DoCircle();
				LoadSides(action.sideIndex);
				backStack.Push(action.sideIndex);
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
			if (righttActionCountent != currentModelAction)
			{
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Leave();
				currentModelAction = righttActionCountent;
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Enter();
			}
		}
		else if (fixedY == 180.0f)
		{
			ActionText.text = backActionText;
			if (backActionCountent != currentModelAction)
			{
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Leave();
				currentModelAction = backActionCountent;
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Enter();
			}
		}
		else
		{
			ActionText.text = leftActionText;
			if (leftActionCountent != currentModelAction)
			{
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Leave();
				currentModelAction = leftActionCountent;
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Enter();
			}
		}
	}

	public void Back()
	{
		backStack.Pop();
		if(backStack.Count != 0)
		{
			LoadSides(backStack.Peek());
			focalTarget.GetComponent<QuickCircle>().DoCircle();
		}
		else
		{
			focalTarget.easeFace = EaseVector3.EASE_FACE.OUT;
			menuCamera.GetComponent<OrbitAndLook>().automaticDistance = false;
			outGoing = true;
			GetComponent<Collider>().enabled = false;
			currentModelAction = null;
		}
	}
}
