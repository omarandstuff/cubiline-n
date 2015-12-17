using UnityEngine.UI;

public class CastActionController : ActionContentController
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public ScrollRect scrollView;
	public CastController castController;
	public EaseImageOpasity buttonBack;
	public EaseTextOpasity buttonText;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		castController.Reset();
	}

	public override void Select()
	{
		castController.interacting = true;
		scrollView.enabled = true;
		buttonBack.easeFace = EaseFloat.EASE_FACE.IN;
		buttonText.easeFace = EaseFloat.EASE_FACE.IN;
	}

	public override void Unselect()
	{
		castController.interacting = false;
		scrollView.enabled = false;
		buttonBack.easeFace = EaseFloat.EASE_FACE.OUT;
		buttonText.easeFace = EaseFloat.EASE_FACE.OUT;
		base.Unselect();
	}

	public override void Enter()
	{
		castController.enabled = true;
	}

	public override void Leave()
	{
		castController.Reset();
		castController.enabled = false;
	}
}
