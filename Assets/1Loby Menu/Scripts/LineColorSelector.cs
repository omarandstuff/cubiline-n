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
	private bool unlocked = true;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		toggles[CubilineApplication.singleton.player.player1ColorIndex].isOn = true;
		SelectColor(CubilineApplication.singleton.player.player1ColorIndex.ToString());
		ApplyColor();

		if(!CubilineApplication.singleton.player.blueAchieve)
		{
			blockBlue.SetActive(true);
			lockIcon.SetActive(true);
			leyend.gameObject.SetActive(true);
			unlocked = false;
		}
		if (!CubilineApplication.singleton.player.orangeAchieve)
		{
			blockOrange.SetActive(true);
			lockIcon.SetActive(true);
			leyend.gameObject.SetActive(true);
			unlocked = false;
		}
		if (!CubilineApplication.singleton.player.greenAchieve)
		{
			blockGreen.SetActive(true);
			lockIcon.SetActive(true);
			leyend.gameObject.SetActive(true);
			unlocked = false;
		}
		if (!CubilineApplication.singleton.player.yellowAchieve)
		{
			blockYellow.SetActive(true);
			lockIcon.SetActive(true);
			leyend.gameObject.SetActive(true);
			unlocked = false;
		}
		if (!CubilineApplication.singleton.player.redAchieve)
		{
			blockRed.SetActive(true);
			lockIcon.SetActive(true);
			leyend.gameObject.SetActive(true);
			unlocked = false;
		}
		if (!CubilineApplication.singleton.player.purpleAchieve)
		{
			blockPurple.SetActive(true);
			lockIcon.SetActive(true);
			leyend.gameObject.SetActive(true);
			unlocked = false;
		}
		if (!CubilineApplication.singleton.player.byScoreColorAchieve)
		{
			blockScore.SetActive(true);
			lockIcon.SetActive(true);
			leyend.gameObject.SetActive(true);
			unlocked = false;
		}
		if (!CubilineApplication.singleton.player.byLengthColorAchieve)
		{
			blockLength.SetActive(true);
			lockIcon.SetActive(true);
			leyend.gameObject.SetActive(true);
			unlocked = false;
		}
		if (!CubilineApplication.singleton.player.byFillColorAchieve)
		{
			blockFill.SetActive(true);
			lockIcon.SetActive(true);
			leyend.gameObject.SetActive(true);
			unlocked = false;
		}
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

		if (!unlocked )
		{
			leyend.text = "!";
			lockIcon.SetActive(true);
		}
		
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
		CubilineApplication.singleton.player.player1ColorIndex = selectedIndex;
		CubilineApplication.singleton.SaveSettings();
		Unselect();
	}

	public void CancelAction()
	{
		toggles[CubilineApplication.singleton.player.player1ColorIndex].isOn = true;
		Unselect();
	}

	private void ApplyColor()
	{
		CubilineApplication.singleton.player.securePlayer1Color = selectedColor;
		player1Material.color = selectedColor;
	}
}
