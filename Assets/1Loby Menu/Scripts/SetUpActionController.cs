using UnityEngine.UI;

public class SetUpActionController : ActionContentController
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
		if(CubilineApplication.lastComment == "coop")
		{
			sizeSlider.value = CubilineApplication.coopCubeSize;
			speedSlider.value = CubilineApplication.coopLineSpeed;
 
		}
		else if(CubilineApplication.lastComment == "arcade")
		{
			sizeSlider.value = CubilineApplication.cubeSize;
			speedSlider.value = CubilineApplication.lineSpeed;
			hardToggle.isOn = CubilineApplication.hardMove;
		}
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
		if (CubilineApplication.lastComment == "coop")
		{
			CubilineApplication.coopCubeSize = (uint)sizeSlider.value;
			CubilineApplication.coopLineSpeed = (uint)speedSlider.value;
			CubilineApplication.coopHardMove = hardToggle.isOn;
		}
		else if (CubilineApplication.lastComment == "arcade")
		{
			CubilineApplication.cubeSize = (uint)sizeSlider.value;
			CubilineApplication.lineSpeed = (uint)speedSlider.value;
			CubilineApplication.hardMove = hardToggle.isOn;
		}
	}

	public void CancelAction()
	{
		Unselect();
		if (CubilineApplication.lastComment == "coop")
		{
			sizeSlider.value = CubilineApplication.coopCubeSize;
			speedSlider.value = CubilineApplication.coopLineSpeed;
			hardToggle.isOn = CubilineApplication.coopHardMove;
		}
		else if (CubilineApplication.lastComment == "arcade")
		{
			sizeSlider.value = CubilineApplication.cubeSize;
			speedSlider.value = CubilineApplication.lineSpeed;
			hardToggle.isOn = CubilineApplication.hardMove;
		}
	}
}
