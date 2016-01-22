using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerActionController : ActionContentController
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public ScrollRect scrollView;
	public Button okButton;
	public Button cancelButon;

	public InputField nickNameInput;

	public Text arcadeHighScore;
	public Text arcadeObtained;
	public Text arcadeLastGame;
	public Text arcadeGamesPlayed;
	public Text arcadeTimePlayed;
	public Text arcadeBestLength;
	public Text arcadeTotalLength;

	public Text coopHighScore;
	public Text coopObtained;
	public Text coopLastGame;
	public Text coopGamesPlayed;
	public Text coopTimePlayed;
	public Text coopBestLength;
	public Text coopTotalLength;

	public Text blueBlockCount;
	public Text orangeBlockCount;
	public Text greenBlockCount;
	public Text grayBlockCount;
	public Text purpleBlockCount;
	public Text multiplierBlockCount;

	public Text levels;
	public Text colors;


	//////////////////////////////////////////////////////////////
	///////////////////// CONTROL VARIABLES //////////////////////
	//////////////////////////////////////////////////////////////

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		nickNameInput.text = CubilineApplication.singleton.player.nickName;

		arcadeHighScore.text = CubilineApplication.singleton.player.bestArcadeScore.ToString();
		if(CubilineApplication.singleton.player.bestArcadeScoreDateTime.Year != 1)
			SetDateLeyend(arcadeObtained, CubilineApplication.singleton.player.bestArcadeScoreDateTime);
		if (CubilineApplication.singleton.player.lastArcadeScoreDateTime.Year != 1)
			SetDateLeyend(arcadeLastGame, CubilineApplication.singleton.player.lastArcadeScoreDateTime);
		arcadeGamesPlayed.text = CubilineApplication.singleton.player.arcadeGamesPlayed.ToString();

		int hours = (int)CubilineApplication.singleton.player.arcadeTimePlayed / 3600;
		int seconds = (int)CubilineApplication.singleton.player.arcadeTimePlayed - hours * 3600;
		int minutes = seconds / 60;
		seconds -= minutes * 60;

		arcadeTimePlayed.text = hours.ToString("D" + 2) + ":" + minutes.ToString("D" + 2) + ":" + seconds.ToString("D" + 2);

		arcadeBestLength.text = CubilineApplication.singleton.player.bestArcadeLength.ToString() + "m";
		arcadeTotalLength.text = CubilineApplication.singleton.player.totalArcadeLength.ToString() + "m";


		coopHighScore.text = CubilineApplication.singleton.player.bestCoopScore.ToString();
		if (CubilineApplication.singleton.player.bestCoopScoreDateTime.Year != 1)
			SetDateLeyend(coopObtained, CubilineApplication.singleton.player.bestCoopScoreDateTime);
		if (CubilineApplication.singleton.player.lastCoopScoreDateTime.Year != 1)
			SetDateLeyend(coopLastGame, CubilineApplication.singleton.player.lastCoopScoreDateTime);
		coopGamesPlayed.text = CubilineApplication.singleton.player.coopGamesPlayed.ToString();

		hours = (int)CubilineApplication.singleton.player.coopTimePlayed / 3600;
		seconds = (int)CubilineApplication.singleton.player.coopTimePlayed - hours * 3600;
		minutes = seconds / 60;
		seconds -= minutes * 60;

		coopTimePlayed.text = hours.ToString("D" + 2) + ":" + minutes.ToString("D" + 2) + ":" + seconds.ToString("D" + 2);

		coopBestLength.text = CubilineApplication.singleton.player.bestCoopLength.ToString() + "m";
		coopTotalLength.text = CubilineApplication.singleton.player.totalCoopLength.ToString() + "m";


		blueBlockCount.text = CubilineApplication.singleton.achievements.blueCount.ToString();
		orangeBlockCount.text = CubilineApplication.singleton.achievements.orangeCount.ToString();
		greenBlockCount.text = CubilineApplication.singleton.achievements.greenCount.ToString();
		grayBlockCount.text = CubilineApplication.singleton.achievements.grayCount.ToString();
		purpleBlockCount.text = CubilineApplication.singleton.achievements.purpleCount.ToString();
		multiplierBlockCount.text = CubilineApplication.singleton.achievements.yellowCount.ToString();

		int levelsCount = 1;
		if (CubilineApplication.singleton.achievements.blackCubeAchieve) levelsCount++;
		if (CubilineApplication.singleton.achievements.diceAchieve) levelsCount++;
		if (CubilineApplication.singleton.achievements.blackDiceAchieve) levelsCount++;
		if (CubilineApplication.singleton.achievements.toyAchieve) levelsCount++;
		if (CubilineApplication.singleton.achievements.blackToyAchieve) levelsCount++;
		if (CubilineApplication.singleton.achievements.paperAchieve) levelsCount++;
		if (CubilineApplication.singleton.achievements.blackPaperAchieve) levelsCount++;
		if (CubilineApplication.singleton.achievements.incognitAchieve) levelsCount++;
		if (CubilineApplication.singleton.achievements.blackIncognitAchieve) levelsCount++;

		levels.text = levelsCount.ToString() + "/10";

		int colorsCount = 1;
		if (CubilineApplication.singleton.achievements.blueAchieve) colorsCount++;
		if (CubilineApplication.singleton.achievements.orangeAchieve) colorsCount++;
		if (CubilineApplication.singleton.achievements.greenAchieve) colorsCount++;
		if (CubilineApplication.singleton.achievements.yellowAchieve) colorsCount++;
		if (CubilineApplication.singleton.achievements.redAchieve) colorsCount++;
		if (CubilineApplication.singleton.achievements.purpleAchieve) colorsCount++;
		if (CubilineApplication.singleton.achievements.byScoreColorAchieve) colorsCount += 6;
		if (CubilineApplication.singleton.achievements.byLengthColorAchieve) colorsCount += 6;
		if (CubilineApplication.singleton.achievements.byFillColorAchieve) colorsCount += 2;

		colors.text = colorsCount.ToString() + "/ 21";
	}

	public override void Select()
	{
		GetComponent<GraphicRaycaster>().enabled = true;

		okButton.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
		okButton.transform.GetChild(0).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;

		cancelButon.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
		cancelButon.transform.GetChild(0).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;

		scrollView.enabled = true;
	}

	public override void Unselect()
	{
		GetComponent<GraphicRaycaster>().enabled = false;

		okButton.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		okButton.transform.GetChild(0).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		cancelButon.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		cancelButon.transform.GetChild(0).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;

		scrollView.enabled = true;
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
		if(nickNameInput.text == "")
			nickNameInput.text = CubilineApplication.singleton.player.nickName;
		else
		{
			CubilineApplication.singleton.player.nickName = nickNameInput.text;
			CubilineApplication.singleton.SavePlayer();
		}
		Unselect();
	}

	public void CancelAction()
	{
		nickNameInput.text = CubilineApplication.singleton.player.nickName;
		Unselect();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////// UTIL //////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	private void SetDateLeyend(Text text, DateTime date)
	{
		DateTimeSpan dateSpan = DateTimeSpan.CompareDates(date, DateTime.Now);

		if (dateSpan.Years > 0)
			text.text = (dateSpan.Years == 1 ? "A Year Ago" : dateSpan.Years.ToString() + " Years Ago");
		else if (dateSpan.Months > 0)
			text.text = (dateSpan.Months == 1 ? "A Month Ago" : dateSpan.Months.ToString() + " Months Ago");
		else if (dateSpan.Days > 0)
			text.text = (dateSpan.Days == 1 ? "A Day Ago" : dateSpan.Days.ToString() + " Days Ago");
		else if (dateSpan.Hours > 0)
			text.text = (dateSpan.Hours == 1 ? "An Hour Ago" : dateSpan.Hours.ToString() + " Hours Ago");
		else if (dateSpan.Minutes > 0)
			text.text = (dateSpan.Minutes == 1 ? "A Minute Ago" : dateSpan.Minutes.ToString() + " Minutes Ago");
		else if (dateSpan.Seconds > 0)
			text.text = (dateSpan.Seconds == 1 ? "A Second Ago" : dateSpan.Seconds.ToString() + " Seconds Ago");
	}
}
