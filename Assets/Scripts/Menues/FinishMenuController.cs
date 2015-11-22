using UnityEngine;
using System.Collections;

public class FinishMenuController : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public OrbitAndLook finishCamera;
	public EasePosition focalTarget;
	public EaseScore scoreText;
	public EaseScore scoreText2;
	public GameObject newRecordText;
	public GameObject retryModel;
	public GameObject mainMenuModel;

	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////
	public enum MENU_ACTION { SCORE, RETRY, MAIN_MENU, NONE }

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

	/////////////////////////// ACTION ///////////////////////////
	private GameObject currentModelAction;
	private bool actionReady;

	/////////////////////////// SCORE ///////////////////////////

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		SetAction();
		goingAction = MENU_ACTION.NONE;
		StartCoroutine(ShowScores());
	}

	void Update()
	{
		if (goingAction == MENU_ACTION.NONE)
		{
			// Touch
			if(Input.touchCount > 0)
			{
				Touch touch = Input.GetTouch(0);

				if(touch.phase == TouchPhase.Began)
				{
					actionReady = true;
					if (currentModelAction != null)
						currentModelAction.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
				}
				else if (touch.phase == TouchPhase.Moved)
				{
					float axis = touch.deltaPosition.x;
					inRotation.y -= axis * slideSencibility;
					actionRotation.y = targetRotation.y + (inRotation.y > 0 ? Mathf.Ceil(((int)inRotation.y / 45) / 2.0f) : Mathf.Floor(((int)inRotation.y / 45) / 2.0f)) * 90;
					SetAction();
					if (axis != 0)
					{
						actionReady = false;
						if (currentModelAction != null) currentModelAction.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
					}
				}
				else if(touch.phase == TouchPhase.Ended)
				{
					targetRotation.y += (inRotation.y > 0 ? Mathf.Ceil(((int)inRotation.y / 45) / 2.0f) : Mathf.Floor(((int)inRotation.y / 45) / 2.0f)) * 90;
					inRotation.y = 0;

					if (actionReady && selectedAction != MENU_ACTION.NONE)
					{
						focalTarget.easeFace = EaseVector3.EASE_FACE.OUT;
						goingAction = selectedAction;
					}
					else
					{
						if (currentModelAction != null) currentModelAction.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
					}
				}
			}

			currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation + inRotation, ref rotationVelocity, 0.15f);
			transform.localRotation = Quaternion.Euler(currentRotation);

		}
		else if (focalTarget.transform.localPosition == focalTarget.outValues)
		{
			if (goingAction == MENU_ACTION.MAIN_MENU)
				Application.LoadLevel(0);
			else if (goingAction == MENU_ACTION.RETRY)
			{
				if (CubilineScoreController.currentNumberOfPlayers == 1)
					Application.LoadLevel(1);
				else
					Application.LoadLevel(3);
			}
		}
	}

	IEnumerator ShowScores()
	{
		yield return new WaitForSeconds(2.0f);

		scoreText.score = CubilineScoreController.currentScore;
		scoreText2.score = CubilineScoreController.bestScore;
		newRecordText.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
		if (CubilineScoreController.newRecord) newRecordText.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
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
				if(selectedAction != MENU_ACTION.NONE) focalTarget.easeFace = EaseVector3.EASE_FACE.OUT;
				goingAction = selectedAction;
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////// ACTION ////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void SetAction()
	{
		float fixedY = Mathf.Repeat(actionRotation.y, 360.0f);
		if (fixedY == 0.0f || fixedY == 360.0f || fixedY == 180.0)
		{
			selectedAction = MENU_ACTION.NONE;
			currentModelAction = null;
		}
		else if (fixedY == 90.0f)
		{
			selectedAction = MENU_ACTION.MAIN_MENU;
			currentModelAction = mainMenuModel;
		}
		else if (fixedY == 270.0f)
		{
			selectedAction = MENU_ACTION.RETRY;
			currentModelAction = retryModel;
		}
	}
}
