using UnityEngine;
using UnityEngine.UI;

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
	private float waitTime;

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
			transform.localRotation = Quaternion.Euler(currentRotation);

			if(waitTime < 2.0f)
			{
				waitTime += Time.deltaTime;
				if(waitTime >= 2.0f)
				{
					scoreText.score = CubilineScoreController.currentScore;
					scoreText2.score = CubilineScoreController.bestScore;
					newRecordText.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
					if (CubilineScoreController.newRecord) newRecordText.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
				}
			}

		}
		else if (goingAction == MENU_ACTION.MAIN_MENU)
		{
			if (focalTarget.transform.localPosition == focalTarget.outValues)
				Application.LoadLevel(0);
		}
		else if (goingAction == MENU_ACTION.RETRY)
		{
			if (focalTarget.transform.localPosition == focalTarget.outValues)
				Application.LoadLevel(1);
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
			if (selectedAction == MENU_ACTION.MAIN_MENU)
			{
				focalTarget.easeFace = EaseVector3.EASE_FACE.OUT;
				goingAction = MENU_ACTION.MAIN_MENU;
			}
			else if (selectedAction == MENU_ACTION.RETRY)
			{
				focalTarget.easeFace = EaseVector3.EASE_FACE.OUT;
				goingAction = MENU_ACTION.RETRY;
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
		if (fixedY == 0.0f || fixedY == 360.0f || fixedY == 180.0)
		{
			selectedAction = MENU_ACTION.SCORE;
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
