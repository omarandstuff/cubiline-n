using UnityEngine.UI;

public class SetUpCoopActionController : ActionContentController
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public Slider sizeSlider;
	public Slider speedSlider;
	public Toggle hardToggle;
	public EaseTextOpasity[] emergentTexts;
	public EaseImageOpasity[] emergentImages;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		sizeSlider.value = CubilineApplication.coopCubeSize;
		speedSlider.value = CubilineApplication.coopLineSpeed;
		hardToggle.isOn = CubilineApplication.coopHardMove;
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

		CubilineApplication.coopCubeSize = (uint)sizeSlider.value;
		CubilineApplication.coopLineSpeed = (uint)speedSlider.value;
		CubilineApplication.coopHardMove = hardToggle.isOn;
	}

	public void CancelAction()
	{
		Unselect();
		sizeSlider.value = CubilineApplication.coopCubeSize;
		speedSlider.value = CubilineApplication.coopLineSpeed;
		hardToggle.isOn = CubilineApplication.coopHardMove;
	}
}
