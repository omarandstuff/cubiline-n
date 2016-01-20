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
	public Text leyend;
	public GameObject lockIcon;
	public GameObject blockBlue;
	public GameObject blockOrange;
	public GameObject blockGreen;
	public GameObject blockYellow;
	public GameObject blockRed;
	public GameObject blockPurple;
	public GameObject blockScore;
	public GameObject blockLength;
	public GameObject blockFill;

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
		toggles[CubilineApplication.singleton.settings.player1ColorIndex].isOn = true;
		SelectColor(CubilineApplication.singleton.settings.player1ColorIndex.ToString());
		ApplyColor();

		blockBlue.SetActive(!CubilineApplication.singleton.achievements.blueAchieve);
		blockOrange.SetActive(!CubilineApplication.singleton.achievements.orangeAchieve);
		blockGreen.SetActive(!CubilineApplication.singleton.achievements.greenAchieve);
		blockYellow.SetActive(!CubilineApplication.singleton.achievements.yellowAchieve);
		blockRed.SetActive(!CubilineApplication.singleton.achievements.redAchieve);
		blockPurple.SetActive(!CubilineApplication.singleton.achievements.purpleAchieve);
		blockScore.SetActive(!CubilineApplication.singleton.achievements.byScoreColorAchieve);
		blockLength.SetActive(!CubilineApplication.singleton.achievements.byLengthColorAchieve);
		blockFill.SetActive(!CubilineApplication.singleton.achievements.byFillColorAchieve);
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
		leyend.text = "!";
		lockIcon.SetActive(true);
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
		CubilineApplication.singleton.settings.player1ColorIndex = selectedIndex;
		CubilineApplication.singleton.SaveSettings();
		Unselect();
	}

	public void CancelAction()
	{
		toggles[CubilineApplication.singleton.settings.player1ColorIndex].isOn = true;
		Unselect();
	}

	private void ApplyColor()
	{
		player1Material.color = selectedColor;
	}
}
