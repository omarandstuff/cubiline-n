using UnityEngine.UI;
using UnityEngine;

public class PlayerActionController : ActionContentController
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public ScrollRect scrollView;
	public Button okButton;
	public Button cancelButton;
	public GameObject contentView;
	public GameObject notLoggedInViewPrefab;
	public GameObject loggedInViewPrefab;

	//////////////////////////////////////////////////////////////
	///////////////////// CONTROL VARIABLES //////////////////////
	//////////////////////////////////////////////////////////////

	private GameObject currentLogView;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Awake()
	{
	}

	void Start()
	{
		loadView(notLoggedInViewPrefab);
	}

	public override void Select()
	{
		okButton.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
		okButton.transform.GetChild(0).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
	}

	public override void Unselect()
	{
		okButton.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		okButton.transform.GetChild(0).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		base.Unselect();
	}

	public override void Enter()
	{

	}

	public override void Leave()
	{

	}

	public void OkAction()
	{
		Unselect();
	}

	public void CancelAction()
	{
		Unselect();
	}

	private void logInCallback(bool logedin)
	{

	}

	private void loadView(GameObject view)
	{
		Quaternion lastRotation = transform.localRotation;
		Vector3 lastPosition = transform.localPosition;

		transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
		transform.localPosition = Vector3.zero;

		currentLogView = Instantiate(notLoggedInViewPrefab);
		currentLogView.transform.SetParent(contentView.transform);

		transform.localPosition = lastPosition;
		transform.localRotation = lastRotation;
	}
}
