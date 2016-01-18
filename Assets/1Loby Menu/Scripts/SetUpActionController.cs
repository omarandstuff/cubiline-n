﻿using UnityEngine.UI;

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
		sizeSlider.value = CubilineApplication.cubeSize;
		speedSlider.value = CubilineApplication.lineSpeed;
		hardToggle.isOn = CubilineApplication.hardMove;
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

		CubilineApplication.cubeSize = (uint)sizeSlider.value;
		CubilineApplication.lineSpeed = (uint)speedSlider.value;
		CubilineApplication.hardMove = hardToggle.isOn;
	}

	public void CancelAction()
	{
		Unselect();
		sizeSlider.value = CubilineApplication.cubeSize;
		speedSlider.value = CubilineApplication.lineSpeed;
		hardToggle.isOn = CubilineApplication.hardMove;
	}
}
