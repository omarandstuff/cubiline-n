﻿using UnityEngine.UI;

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
		sizeSlider.value = CubilineApplication.singleton.settings.coopCubeSize;
		speedSlider.value = CubilineApplication.singleton.settings.coopLineSpeed;
		hardToggle.isOn = CubilineApplication.singleton.settings.coopHardMove;
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

		CubilineApplication.singleton.settings.coopCubeSize = (uint)sizeSlider.value;
		CubilineApplication.singleton.settings.coopLineSpeed = (uint)speedSlider.value;
		CubilineApplication.singleton.settings.coopHardMove = hardToggle.isOn;

		CubilineApplication.singleton.SaveSettings();
	}

	public void CancelAction()
	{
		Unselect();
		sizeSlider.value = CubilineApplication.singleton.settings.coopCubeSize;
		speedSlider.value = CubilineApplication.singleton.settings.coopLineSpeed;
		hardToggle.isOn = CubilineApplication.singleton.settings.coopHardMove;
	}
}
