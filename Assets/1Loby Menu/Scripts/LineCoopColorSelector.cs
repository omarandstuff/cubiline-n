using UnityEngine;
using UnityEngine.UI;

public class LineCoopColorSelector : ActionContentController
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public Button okButton;
	public Button cancelButton;
	public MiniDemoPlayerController miniDemo;
	public Material player1Material;
	public Material player2Material;
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

	public bool player1Selecting
	{
		set
		{
			_player1Selecting = value;

			if(value)
			{
				toggles[selectedPlayer1Index].isOn = true;
				miniDemo.color = selectedPlayer1Color;
			}
		}
		get
		{
			return _player1Selecting;
		}
	}

	public bool player2Selecting
	{
		set
		{
			_player2Selecting = value;

			if(value)
			{
				toggles[selectedPlayer2Index].isOn = true;
				miniDemo.color = selectedPlayer2Color;
			}
		}
		get
		{
			return _player2Selecting;
		}
	}
	private bool _player1Selecting;
	private bool _player2Selecting;
	//////////////////////////////////////////////////////////////
	///////////////////// CONTROL VARIABLES //////////////////////
	//////////////////////////////////////////////////////////////

	private uint selectedPlayer1Index;
	private Color selectedPlayer1Color;

	private uint selectedPlayer2Index;
	private Color selectedPlayer2Color;

	private bool unloked = true;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		toggles[CubilineApplication.singleton.settings.player1ColorIndex].isOn = true;
		_player1Selecting = true;

		selectedPlayer1Index = CubilineApplication.singleton.settings.player1ColorIndex;
		selectedPlayer1Color = toggles[selectedPlayer1Index].transform.GetChild(1).GetComponent<Image>().color;

		selectedPlayer2Index = CubilineApplication.singleton.settings.player2ColorIndex;
		selectedPlayer2Color = toggles[selectedPlayer2Index].transform.GetChild(1).GetComponent<Image>().color;

		ApplyPlayer1Color();
		ApplyPlayer2Color();

		if (!CubilineApplication.singleton.achievements.blueAchieve)
		{
			blockBlue.SetActive(true);
			lockIcon.SetActive(true);
			leyend.gameObject.SetActive(true);
			unloked = false;
		}
		if (!CubilineApplication.singleton.achievements.orangeAchieve)
		{
			blockOrange.SetActive(true);
			lockIcon.SetActive(true);
			leyend.gameObject.SetActive(true);
			unloked = false;
		}
		if (!CubilineApplication.singleton.achievements.greenAchieve)
		{
			blockGreen.SetActive(true);
			lockIcon.SetActive(true);
			leyend.gameObject.SetActive(true);
			unloked = false;
		}
		if (!CubilineApplication.singleton.achievements.yellowAchieve)
		{
			blockYellow.SetActive(true);
			lockIcon.SetActive(true);
			leyend.gameObject.SetActive(true);
			unloked = false;
		}
		if (!CubilineApplication.singleton.achievements.redAchieve)
		{
			blockRed.SetActive(true);
			lockIcon.SetActive(true);
			leyend.gameObject.SetActive(true);
			unloked = false;
		}
		if (!CubilineApplication.singleton.achievements.purpleAchieve)
		{
			blockPurple.SetActive(true);
			lockIcon.SetActive(true);
			leyend.gameObject.SetActive(true);
			unloked = false;
		}
		if (!CubilineApplication.singleton.achievements.byScoreColorAchieve)
		{
			blockScore.SetActive(true);
			lockIcon.SetActive(true);
			leyend.gameObject.SetActive(true);
			unloked = false;
		}
		if (!CubilineApplication.singleton.achievements.byLengthColorAchieve)
		{
			blockLength.SetActive(true);
			lockIcon.SetActive(true);
			leyend.gameObject.SetActive(true);
			unloked = false;
		}
		if (!CubilineApplication.singleton.achievements.byFillColorAchieve)
		{
			blockFill.SetActive(true);
			lockIcon.SetActive(true);
			leyend.gameObject.SetActive(true);
			unloked = false;
		}
	}

	public override void Select()
	{
		selectedPlayer1Index = CubilineApplication.singleton.settings.player1ColorIndex;
		selectedPlayer1Color = toggles[selectedPlayer1Index].transform.GetChild(1).GetComponent<Image>().color;

		selectedPlayer2Index = CubilineApplication.singleton.settings.player2ColorIndex;
		selectedPlayer2Color = toggles[selectedPlayer2Index].transform.GetChild(1).GetComponent<Image>().color;

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

		if(!unloked)
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
		if(_player1Selecting)
		{
			selectedPlayer1Index = uint.Parse(selected);
			selectedPlayer1Color = toggles[selectedPlayer1Index].transform.GetChild(1).GetComponent<Image>().color;
			miniDemo.color = selectedPlayer1Color;
		}
		else
		{
			selectedPlayer2Index = uint.Parse(selected);
			selectedPlayer2Color = toggles[selectedPlayer2Index].transform.GetChild(1).GetComponent<Image>().color;
			miniDemo.color = selectedPlayer2Color;
		}
		
	}

	public void OkAction()
	{
		ApplyPlayer1Color();
		ApplyPlayer2Color();
		CubilineApplication.singleton.settings.player1ColorIndex = selectedPlayer1Index;
		CubilineApplication.singleton.settings.player2ColorIndex = selectedPlayer2Index;
		CubilineApplication.singleton.SaveSettings();
		Unselect();
	}

	public void CancelAction()
	{
		if (_player1Selecting)
			toggles[CubilineApplication.singleton.settings.player1ColorIndex].isOn = true;
		else
			toggles[CubilineApplication.singleton.settings.player2ColorIndex].isOn = true;

		Unselect();
	}

	private void ApplyPlayer1Color()
	{
		player1Material.color = selectedPlayer1Color;
	}

	private void ApplyPlayer2Color()
	{
		player2Material.color = selectedPlayer2Color;
	}
}
