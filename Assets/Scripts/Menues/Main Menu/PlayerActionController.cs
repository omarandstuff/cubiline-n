using UnityEngine.UI;
using Facebook.Unity;
using UnityEngine;
using System.Collections.Generic;

public class PlayerActionController : ActionContentController
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public ScrollRect scrollView;
	public EaseTextOpasity[] emergentTexts;
	public EaseImageOpasity[] emergentImages;
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
		CubilinePlayer.LogInPlayer(logInCallback);
	}

	void Start()
	{
		currentLogView = Instantiate(notLoggedInViewPrefab);
		currentLogView.transform.SetParent(contentView.transform);
		currentLogView.transform.localRotation = transform.localRotation;
		currentLogView.GetComponent<RectTransform>().position = Vector3.zero;
	}

	public override void Select()
	{
		GetComponent<GraphicRaycaster>().enabled = true;
		foreach (EaseImageOpasity ei in emergentImages)
			ei.easeFace = EaseFloat.EASE_FACE.IN;
		foreach (EaseTextOpasity et in emergentTexts)
			et.easeFace = EaseFloat.EASE_FACE.IN;
	}

	public override void Unselect()
	{
		GetComponent<GraphicRaycaster>().enabled = false;
		foreach (EaseImageOpasity ei in emergentImages)
			ei.easeFace = EaseFloat.EASE_FACE.OUT;
		foreach (EaseTextOpasity et in emergentTexts)
			et.easeFace = EaseFloat.EASE_FACE.OUT;
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
}
