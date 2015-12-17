using UnityEngine;
using UnityEngine.UI;

public class SmallSetUpActionController : SmallActionController
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public EaseTextOpasity[] emergentTexts;
	public EaseImageOpasity[] emergentImages;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
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

		parentAction.Unselect();
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
