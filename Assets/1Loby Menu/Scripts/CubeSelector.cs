using UnityEngine;
using UnityEngine.UI;

public class CubeSelector : ActionContentController
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public Button okButton;
	public Button cancelButton;

	//////////////////////////////////////////////////////////////
	///////////////////// CONTROL VARIABLES //////////////////////
	//////////////////////////////////////////////////////////////


	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{

	}

	public override void Select()
	{
		okButton.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
		okButton.transform.GetChild(0).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
		cancelButton.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
		cancelButton.transform.GetChild(0).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
		GetComponent<GraphicRaycaster>().enabled = true;
	}

	public override void Unselect()
	{
		okButton.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		okButton.transform.GetChild(0).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		cancelButton.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		cancelButton.transform.GetChild(0).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		GetComponent<GraphicRaycaster>().enabled = false;
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
}
