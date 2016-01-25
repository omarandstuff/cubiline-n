﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public GameObject backSoundPrefab;
	public GameObject flipSoundPrefab;
	public GameObject contentSoundPrefab;
	public GameObject sceneSoundPrefab;
	public GameObject sideSoundPrefab;
	public Transform cubeMenu; // Cube
	public Camera menuCamera; // Camera with the propertie of enclousere the cube menu.
	public Text ActionText; // Text that show the name of the menu side.
	public EasePosition focalTarget; // What the camera will be focusing (can be move when outing the scene).
	public GameObject backButtonPrefab; // Optionaly can present a button to enable the player to back in scene and sides.
	public GameObject navigationLeft; // Optionaly can acces a button that enables the rotation navigation to the left.
	public GameObject navigationRight; // Optionaly can acces a button that enables the rotation navigation to the right.
	[System.Serializable]public struct Sides { public string frontActionText; public GameObject frontActionContentPrefab; public string backActionText; public GameObject backActionContentPrefab; public string rightActionText; public GameObject rightActionContentPrefab; public string leftActionText; public GameObject leftActionContentPrefab; }
	public Sides[] sides; // Prefas of every side of the menu in every stage.
	/*
	|          /-- Side[1] --- Side[2]
	| Side[0]----- Side[3]
	|          \-- Side[4] --- Side[5]
	|
	*/

	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////
	public float bigPanoramaDistance = 2.0f; // When viewing the menu whats the correct offset distance to show all the menu and ttiles.
	public float smallPanoramaDistance = 1.0f; // When viewing a content whats the correct distance to see all the side.
	public enum STATUS { SELECTING, IN_ACTION }
	public STATUS status = STATUS.SELECTING; // Status [Selecting sides or viewing a content].
	public float slideSencibility = 1.0f; // 1 means sliding the whole screen width the cube will rotate 90°.
	public float cubeRotationSmoothTime = 0.15f; // Smooth cube rotation.
	public string backScene; // When there is not sides to load back wish scene is the previous to this one.

	public static string nextScene; // This attrubute will be accesed for the actions controllers to tell the current menu wish scene is next when outing.

	public bool inMenuMusic = true;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	//////////////////////// SIDE CONTENTS ///////////////////////
	private GameObject frontActionCountent; // ----\
	private GameObject backActionCountent; // ------- Current ActionContents loaded form the current side.
	private GameObject righttActionCountent; // ---/
	private GameObject leftActionCountent; // ----/
	private string frontActionText; //--\
	private string backActionText; //----- Text to display for every side for the current sides loaded.
	private string rightActionText; //--/
	private string leftActionText; //--/

	//////////////////////// CUBE ROTATION ///////////////////////
	private int lastJoyStick; // When using joystick wish was the list side of the thumb stick whas used.

	private Vector2 actionRotation = Vector3.zero; // Can be selected a side when the cube get 45° close to the target rotation for that side without realy targeting that rotation unless the cube is released in that state.
	private Vector3 currentRotation = Vector3.zero; // The cube will follow this rotation that will smoothly try to reah the target rotation.
	private Vector3 targetRotation = Vector3.zero; // When a side is selected this is the correct rotation of the cube to be in that side.
	private Vector3 inRotation = Vector3.zero; // This is generated by the position of the X Coord of the input used for rotation. Will be fixed hen release for apropriate target rotation.
	private Vector3 rotationVelocity = Vector3.zero; // Used for SmoothDamp
	private Vector2 lastMousePosition; // When using mouse get the last mouse X coord to add to the InRotation.
	private bool flipReady; // Play the sound of their people?
	private Vector2 lastTouchPos;

	/////////////////////////// ACTION ///////////////////////////
	private GameObject currentModelAction; // In teh selected side, this model is the active action.
	private bool actionReady; // When trying to activate an action don't move the cube any more or the action will be not ready.
	private bool outGoing; // The menu is outing.
	private int currentSideIndex; // From the sides component whish sides will be loaded. 
	private GameObject backButton; // If it is the case the back button.
	private Stack<BackState> backStack = new Stack<BackState>(); // When backing sides, needs to pop the last loaded and reload the one that was before it.
	private struct BackState { public int sideIndex; public Vector3 rotation; public BackState(int side_index, Vector3 _rotation) { sideIndex = side_index; rotation = _rotation; } } // The state of the last sides loaded befor load new ones (rotation nad so on).

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		// First things to do.
		SetupMenu();

		flipReady = true;

		// Reset time scale.
		Time.timeScale = 1.0f;

		
		CubilineMusicPlayer.singleton.Play();

		// Check For the preciois one.
		CubilineApplication.singleton.CheckBoxLevelAchievement();

		// Audio directive
		CubilineMusicPlayer.inMenu = inMenuMusic;
	}

	void Update()
	{
		// Check joystick input.
		JoyStickInput();
		if (!outGoing) // Rotate cube while not action called.
		{
			// Smoothly try to reach target rotation.
			currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation + inRotation, ref rotationVelocity, cubeRotationSmoothTime);
			// Cube has a qaternion rotation form the current rotation euler angles.
			cubeMenu.localRotation = Quaternion.Euler(currentRotation);
		}
		else if (focalTarget.transform.localPosition == focalTarget.outValues) // If outgoing and the focal target gets to the side where the menu is not visible.
		{
			// Load the next scene.
			Application.LoadLevel(nextScene);
		}
	}

	void OnGUI()
	{
		if (status != STATUS.SELECTING && !outGoing) return; // This will be not nesesary if not selecting

		Event e = Event.current;
		if (e.type == EventType.keyDown) // Rotate the cube.
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
			else if(e.keyCode == KeyCode.Escape)
			{
				if(backButton != null)
				{
					Back();
				}
			}
			else if (e.keyCode == KeyCode.Space) // Like the click or touch but with the keyboard :).
			{
				SelectAction();
			}
		}
	}

	void OnMouseDown()
	{
		// If using or simulating mouse with collider get the coordinates to get letter the delta position. 
		lastMousePosition = Input.mousePosition;
		if (Input.touchCount > 0) // If touching get position of touch.
		{
			lastTouchPos = Input.GetTouch(0).position;
		}
		// Can be call to the action.
		actionReady = true;
		// If it is a kind of action as just a model action its ease scale for effect of pressing.
		if (currentModelAction != null && currentModelAction.GetComponent<EaseScale>() != null) currentModelAction.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
	}

	void OnMouseDrag()
	{
		float axis = 0;
		if (Input.touchCount > 0) // If touching get the delta coords to add to InRotation.
		{
			axis = (Input.GetTouch(0).position - lastTouchPos).x / menuCamera.pixelWidth;
			lastTouchPos = Input.GetTouch(0).position;
		}
		else // Then get the delta mouse coordinates position using the last mouse position coords.
		{
			Vector2 currentMousePosition = Input.mousePosition;
			axis = (currentMousePosition - lastMousePosition).x / menuCamera.pixelWidth;
			lastMousePosition = currentMousePosition;
		}
		// Add that delta position at the maner of geting a proportion from the screen width in  deegres.
		inRotation.y -= (90.0f * axis) * slideSencibility;

		// Action rotation needs to know if we are reachign a target rotation 45° before.
		actionRotation.y = targetRotation.y + (inRotation.y > 0 ? Mathf.Ceil(((int)inRotation.y / 45) / 2.0f) : Mathf.Floor(((int)inRotation.y / 45) / 2.0f)) * 90;

		// Select the correct action form actionRotation.
		SetAction();

		// If the player keeps rotating the cube means he don't want to activate the action, he is just rotating.
		if (axis != 0)
		{
			actionReady = false;
			if (currentModelAction != null && currentModelAction.GetComponent<EaseScale>() != null) currentModelAction.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
		}
	}

	void OnMouseUp()
	{
		// Target a correct rotation from the rotation generated by the imput.
		targetRotation.y += (inRotation.y > 0 ? Mathf.Ceil(((int)inRotation.y / 45) / 2.0f) : Mathf.Floor(((int)inRotation.y / 45) / 2.0f)) * 90;

		// Reset the rotation generated by the imput
		inRotation.y = 0;
	}

	void OnMouseUpAsButton()
	{
		if (actionReady) // If the player just press the action the call it.
			SelectAction();
		else if (currentModelAction != null && currentModelAction.GetComponent<EaseScale>() != null) // Then just restore the icon of that side if it is the case.
			currentModelAction.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
	}

	void JoyStickInput()
	{
		if (status != STATUS.SELECTING && !outGoing) return; // If is not selecting checking the joystick will be not necesary.

		int joyInput = Input.GetAxis("Horizontal") > 0.5f ? 1 : (Input.GetAxis("Horizontal") < -0.5f ? -1 : 0); // The side of the stick if the stick reach the half of its pssible moviemnnt.
		if (joyInput != lastJoyStick) // If the stick is in the same side the do nothing , just a rotation per stck moviment.
		{
			if (joyInput > 0)
			{
				TurnRight();
			}
			else if (joyInput < 0)
			{
				TurnLeft();
			}
			SetAction();
			lastJoyStick = joyInput;
		}
	}

	public void TurnRight()
	{
		targetRotation.y += 90;
		actionRotation.y += 90;
		SetAction();
	}

	public void TurnLeft()
	{
		targetRotation.y -= 90;
		actionRotation.y -= 90;
		SetAction();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////// SETUP ////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void LoadSides(int index)
	{
		if (index >= sides.Length) return; // Just load sides in the range.

		// Clean the text for testing porpouses.
		if (ActionText != null) frontActionText = "None";
		if (ActionText != null) backActionText = "None";
		if (ActionText != null) rightActionText = "None";
		if (ActionText != null) leftActionText = "None";

		// Destroy last sides loaded if there were.
		if (frontActionCountent != null) Destroy(frontActionCountent);
		if (backActionCountent != null) Destroy(backActionCountent);
		if (righttActionCountent != null) Destroy(righttActionCountent);
		if (leftActionCountent != null) Destroy(leftActionCountent);

		if (sides[index].frontActionContentPrefab != null) // Fron side
		{
			// Instantiate and transform to present correctly in this side.
			frontActionCountent = Instantiate(sides[index].frontActionContentPrefab, Vector3.back * 5.01f, Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f))) as GameObject;
			
			// Parenting the cube to it so the rotation affect this content as well.
			frontActionCountent.transform.SetParent(cubeMenu);

			// This menu is the parent of the action. (Used to call the back action)
			frontActionCountent.GetComponent<ActionContentController>().parentMenu = this;

			// The text of this side acordly to the side information.
			frontActionText = sides[index].frontActionText;
		}

		if (sides[index].backActionContentPrefab != null) // Back Side
		{
			backActionCountent = Instantiate(sides[index].backActionContentPrefab, Vector3.forward * 5.01f, Quaternion.identity) as GameObject;
			backActionCountent.transform.SetParent(cubeMenu);
			backActionCountent.GetComponent<ActionContentController>().parentMenu = this;
			backActionText = sides[index].backActionText;
		}
		if (sides[index].rightActionContentPrefab != null) // Right side
		{
			righttActionCountent = Instantiate(sides[index].rightActionContentPrefab, Vector3.right * 5.01f, Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f))) as GameObject;
			righttActionCountent.transform.SetParent(cubeMenu);
			righttActionCountent.GetComponent<ActionContentController>().parentMenu = this;
			rightActionText = sides[index].rightActionText;
		}
		if (sides[index].leftActionContentPrefab != null) // Left Side
		{
			leftActionCountent = Instantiate(sides[index].leftActionContentPrefab, Vector3.left * 5.01f, Quaternion.Euler(new Vector3(0.0f, 270.0f, 0.0f))) as GameObject;
			leftActionCountent.transform.SetParent(cubeMenu);
			leftActionCountent.GetComponent<ActionContentController>().parentMenu = this;
			leftActionText = sides[index].leftActionText;
		}

		// When loading needs to select the fornt side.
		SetAction();
	}

	void SetupMenu()
	{
		// Load the first sides.
		LoadSides(0);

		// Configure the camera to see the whole menu.
		menuCamera.GetComponent<OrbitAndLook>().automaticDistanceOffset = bigPanoramaDistance;


		// Load the back button if it is the case.
		if (backButtonPrefab != null)
		{
			backButton = Instantiate(backButtonPrefab);
			backButton.GetComponent<BackButtonDelegate>().parentMenu = this;
		}

	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////// ACTION ////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void SelectAction()
	{
		if (currentModelAction != null) // If it was an action loaded for thsi side.
		{
			// Get the pointer to the action component.
			ActionContentController action = currentModelAction.GetComponent<ActionContentController>();

			if (action.contentType == ActionContentController.CONTENT_TYPE.CONTENT) // The actions can be visualize to manipulate it.
			{
				// Play the sound of their people.
				Destroy(Instantiate(contentSoundPrefab), 2);

				// The menu is not selecting right now.
				status = STATUS.IN_ACTION;

				// See the whole cube side of this content.
				menuCamera.GetComponent<OrbitAndLook>().automaticDistanceOffset = smallPanoramaDistance;

				// Call the select action since this action was selected daa.
				action.Select();

				if(backButton != null) // If the back button was loaded then desable it.
				{
					backButton.transform.GetChild(0).GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
					backButton.transform.GetChild(1).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
					backButton.GetComponent<Collider>().enabled = false;
				}

				if (navigationLeft != null) // If exist navigation left.
				{
					navigationLeft.transform.GetChild(0).GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
					navigationLeft.GetComponent<Collider>().enabled = false;
				}

				if (navigationRight != null) // If exist navigation right.
				{
					navigationRight.transform.GetChild(0).GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
					navigationRight.GetComponent<Collider>().enabled = false;
				}

				// Desable the text behind the cube too.
				if (ActionText != null) ActionText.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
				
				// And do not recive calls form the collider.
				GetComponent<Collider>().enabled = false;
			}
			else if (action.contentType == ActionContentController.CONTENT_TYPE.TO_SCENE) // Have to go now.
			{
				// Call the select action to know whats next "segun" the action.
				action.Select();

				// Play the sound of their people.
				Destroy(Instantiate(sceneSoundPrefab), 2);

				// Tell the focal target to go somewhere the cube will be out of sigt.
				focalTarget.easeFace = EaseVector3.EASE_FACE.OUT;

				// Disable the automatic distance to keep it naturaly.
				menuCamera.GetComponent<OrbitAndLook>().automaticDistance = false;

				// We are now outing.
				outGoing = true;

				//Next Scene
				nextScene = action.SceneName;

				// Not more collider calls.
				GetComponent<Collider>().enabled = false;
			}
			else if (action.contentType == ActionContentController.CONTENT_TYPE.LOAD_SIDE) // Load new sides (mora actions) in this bare menu.
			{
				// Before loading keep the state of these sides.
				backStack.Push(new BackState(currentSideIndex, targetRotation));

				// Play the sound of their people.
				Destroy(Instantiate(sideSoundPrefab), 2);

				// This new sides index.
				currentSideIndex = action.sideIndex;

				// Reset the rotations
				cubeMenu.localRotation = Quaternion.identity;
				targetRotation = Vector3.zero;
				actionRotation = Vector3.zero;
				currentRotation = new Vector3(-90.0f, 0.0f, 0.0f); // Simulate like if the cube comes from the top side.

				flipReady = false;

				// Now can be loaded the new sides. (The cube rotation is now reseted so the sides are well loaded).
				LoadSides(action.sideIndex);

				flipReady = true;
			}
		}
	}

	public void BackFromAction()
	{
		// The player is now selecting
		status = STATUS.SELECTING;

		// See the whole menu.
		menuCamera.GetComponent<OrbitAndLook>().automaticDistanceOffset = bigPanoramaDistance;

		// The collider can send calls now.
		GetComponent<Collider>().enabled = true;

		if (backButton != null) // Enable the back button if it is the case.
		{
			backButton.transform.GetChild(0).GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
			backButton.transform.GetChild(1).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
			backButton.GetComponent<Collider>().enabled = true;
		}

		if (navigationLeft != null) // If exist navigation left.
		{
			navigationLeft.transform.GetChild(0).GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
			navigationLeft.GetComponent<Collider>().enabled = true;
		}

		if (navigationRight != null) // If exist navigation right.
		{
			navigationRight.transform.GetChild(0).GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
			navigationRight.GetComponent<Collider>().enabled = true;
		}

		// And visualize the text behind the cube again.
		if (ActionText != null) ActionText.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
	}

	void SetAction()
	{
		float fixedY = Mathf.Repeat(actionRotation.y, 360.0f); // Get a roation between 0 and 360 to really know whish side select.

		if (fixedY == 0.0f || fixedY == 360.0f) //  Front side
		{
			// Load the text for this side.
			if (ActionText != null) ActionText.text = frontActionText;

			// Just send teh enter and leave calls if the side really changed.
			if (frontActionCountent != currentModelAction)
			{
				if(flipReady) Destroy(Instantiate(flipSoundPrefab), 2);
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Leave();
				currentModelAction = frontActionCountent;
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Enter();
			}
		}
		else if (fixedY == 90.0f) // Right side.
		{
			if (ActionText != null) ActionText.text = rightActionText;
			if (righttActionCountent != currentModelAction)
			{
				if (flipReady) Destroy(Instantiate(flipSoundPrefab), 2);
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Leave();
				currentModelAction = righttActionCountent;
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Enter();
			}
		}
		else if (fixedY == 180.0f) // Back side.
		{
			if (ActionText != null) ActionText.text = backActionText;
			if (backActionCountent != currentModelAction)
			{
				if (flipReady) Destroy(Instantiate(flipSoundPrefab), 2);
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Leave();
				currentModelAction = backActionCountent;
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Enter();
			}
		}
		else // And left side.
		{
			if (ActionText != null) ActionText.text = leftActionText;
			if (leftActionCountent != currentModelAction)
			{
				if (flipReady) Destroy(Instantiate(flipSoundPrefab), 2);
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Leave();
				currentModelAction = leftActionCountent;
				if (currentModelAction != null) currentModelAction.GetComponent<ActionContentController>().Enter();
			}
		}
	}

	public void Back()
	{
		if(backStack.Count != 0) // Previosu sides can be loaded
		{
			// Last back.
			BackState backState = backStack.Pop();

			// Rotate the cube to this previous rotation
			targetRotation = backState.rotation;

			// The ation is the same as the target 'cause stuff. (And because the target is the correct action rotation sice target rotation is the natural roatation for that side is close less that 45° to it self).
			actionRotation = targetRotation;

			// Play the sound of their people.
			Destroy(Instantiate(backSoundPrefab), 2);

			float fixedY = Mathf.Repeat(actionRotation.y, 360.0f); // Get a roation between 0 and 360 to really know whish side select.

			// Simulate the cube comes form the bottom side.
			if (fixedY == 0.0f || fixedY == 360.0f) //  Front side
			{
				currentRotation = targetRotation + new Vector3(90.0f, 0.0f, 0.0f);
			}
			else if (fixedY == 90.0f) // Right side.
			{
				currentRotation = targetRotation + new Vector3(0.0f, 0.0f, 90.0f);
			}
			else if (fixedY == 180.0f) // Back side.
			{
				currentRotation = targetRotation + new Vector3(-90.0f, 0.0f, 0.0f);
			}
			else // And left side.
			{
				currentRotation = targetRotation + new Vector3(00.0f, 0.0f, -90.0f);
			}

			// This side index.
			currentSideIndex = backState.sideIndex;

			// Rotate the Phisical cube.
			cubeMenu.localRotation = Quaternion.identity;

			flipReady = false;

			// Load last sides.
			LoadSides(backState.sideIndex);

			flipReady = true;
		}
		else
		{
			// The target moves so the menu cube gets out of sigt.
			focalTarget.easeFace = EaseVector3.EASE_FACE.OUT;

			// Keep it natural.
			menuCamera.GetComponent<OrbitAndLook>().automaticDistance = false;

			// The menu is outing.
			outGoing = true;

			// No more collider calls.
			GetComponent<Collider>().enabled = false;

			// Use the previous scene.
			nextScene = backScene;
		}
	}
}
