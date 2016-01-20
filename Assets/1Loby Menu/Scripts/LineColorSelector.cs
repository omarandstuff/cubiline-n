using UnityEngine;
using UnityEngine.UI;

public class LineColorSelector : ActionContentController
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public Button okButton;
	public Button cancelButton;
	public MiniDemoPlayerController miniDemo;
	public Material player1Material;
	public Toggle[] toggles;

	//////////////////////////////////////////////////////////////
	///////////////////// CONTROL VARIABLES //////////////////////
	//////////////////////////////////////////////////////////////

	private uint selectedIndex;
	private Color selectedColor;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		toggles[CubilineApplication.settings.player1ColorIndex].isOn = true;
		ApplyColor();
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

	public void SelectColor(string selected)
	{
		selectedIndex = uint.Parse(selected);
		selectedColor = toggles[selectedIndex].transform.GetChild(1).GetComponent<Image>().color;
		miniDemo.color = selectedColor;
	}

	public void OkAction()
	{
		ApplyColor();
		CubilineApplication.settings.player1ColorIndex = selectedIndex;
		CubilineApplication.SaveSettings();
		Unselect();
	}

	public void CancelAction()
	{
		toggles[CubilineApplication.settings.player1ColorIndex].isOn = true;
		Unselect();
	}

	private void ApplyColor()
	{
		player1Material.color = selectedColor;
	}
}
