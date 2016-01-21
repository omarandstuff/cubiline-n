using UnityEngine;
using UnityEngine.UI;

public class CubeSelector : ActionContentController
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public ScrollRect scroll;
	public Button okButton;
	public Button cancelButton;
	public MiniSelectorFixer selectorFixer;

	public CubilinePlayerController.PLAYER_KIND gameKind;

	public Image blackCube;
	public Image diceCube;
	public Image blackDiceCube;
	public Image toyCube;
	public Image blackToyCube;
	public Image boxCube;
	public Image blackBoxCube;
	public Image knowledgeCube;
	public Image blacknowledgeCube;

	public GameObject blackCubeLock;
	public GameObject diceCubeLock;
	public GameObject blackDiceCubeLock;
	public GameObject toyCubeLock;
	public GameObject blackToyCubeLock;
	public GameObject boxCubeLock;
	public GameObject blackBoxCubeLock;
	public GameObject knowledgeCubeLock;
	public GameObject blacknowledgeCubeLock;


	//////////////////////////////////////////////////////////////
	///////////////////// CONTROL VARIABLES //////////////////////
	//////////////////////////////////////////////////////////////

	private bool[] unlocked = new bool[10];


	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		GetUnlocked();
		selectorFixer.unlocked = unlocked;
		if(gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE)
			selectorFixer.SetLevelIndex((int)CubilineApplication.singleton.settings.arcadeLevelIndex);
		else if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE_COOP)
			selectorFixer.SetLevelIndex((int)CubilineApplication.singleton.settings.coopLevelIndex);
	}

	public override void Select()
	{
		okButton.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
		okButton.transform.GetChild(0).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
		cancelButton.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
		cancelButton.transform.GetChild(0).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
		scroll.inertia = true;
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

	public void OkAction()
	{
		scroll.inertia = false;
		if (unlocked[selectorFixer.levelIndex])
		{
			if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE)
				CubilineApplication.singleton.settings.arcadeLevelIndex = (uint)selectorFixer.levelIndex;
			else if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE_COOP)
				CubilineApplication.singleton.settings.coopLevelIndex = (uint)selectorFixer.levelIndex;
			
			CubilineApplication.singleton.SaveSettings();
		}
		else
		{
			if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE)
				selectorFixer.SetLevelIndex((int)CubilineApplication.singleton.settings.arcadeLevelIndex);
			else if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE_COOP)
				selectorFixer.SetLevelIndex((int)CubilineApplication.singleton.settings.coopLevelIndex);
		}
		Unselect();
	}

	public void CancelAction()
	{
		scroll.inertia = false;
		if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE)
			selectorFixer.SetLevelIndex((int)CubilineApplication.singleton.settings.arcadeLevelIndex);
		else if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE_COOP)
			selectorFixer.SetLevelIndex((int)CubilineApplication.singleton.settings.coopLevelIndex);
		Unselect();
	}

	private void GetUnlocked()
	{
		unlocked[0] = true;
		unlocked[1] = CubilineApplication.singleton.achievements.blackCubeAchieve;
		unlocked[2] = CubilineApplication.singleton.achievements.diceAchieve;
		unlocked[3] = CubilineApplication.singleton.achievements.blackDiceAchieve;
		unlocked[4] = CubilineApplication.singleton.achievements.toyAchieve;
		unlocked[5] = CubilineApplication.singleton.achievements.blackToyAchieve;
		unlocked[6] = CubilineApplication.singleton.achievements.paperAchieve;
		unlocked[7] = CubilineApplication.singleton.achievements.blackPaperAchieve;
		unlocked[8] = CubilineApplication.singleton.achievements.incognitAchieve;
		unlocked[9] = CubilineApplication.singleton.achievements.blackIncognitAchieve;

		if(unlocked[1])
		{
			blackCube.color = Color.white;
			blackCubeLock.SetActive(false);
		}
		if (unlocked[2])
		{
			diceCube.color = Color.white;
			diceCubeLock.SetActive(false);
		}
		if (unlocked[3])
		{
			blackDiceCube.color = Color.white;
			blackDiceCubeLock.SetActive(false);
		}
		if (unlocked[4])
		{
			toyCube.color = Color.white;
			toyCubeLock.SetActive(false);
		}
		if (unlocked[5])
		{
			blackToyCube.color = Color.white;
			blackToyCubeLock.SetActive(false);
		}
		if (unlocked[6])
		{
			boxCube.color = Color.white;
			boxCubeLock.SetActive(false);
		}
		if (unlocked[7])
		{
			blackBoxCube.color = Color.white;
			blackBoxCubeLock.SetActive(false);
		}
		if (unlocked[8])
		{
			knowledgeCube.color = Color.white;
			knowledgeCubeLock.SetActive(false);
		}
		if (unlocked[9])
		{
			blacknowledgeCube.color = Color.white;
			blacknowledgeCubeLock.SetActive(false);
		}
	}
}
