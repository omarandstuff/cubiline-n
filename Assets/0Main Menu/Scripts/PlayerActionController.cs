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

	public Text arcadeHighScore;
	public Text arcadeObtained;
	public Text arcadeLastGame;
	public Text arcadeGamesPlayed;
	public Text arcadeTimePlayed;
	public Text arcadeTotalLength;


	//////////////////////////////////////////////////////////////
	///////////////////// CONTROL VARIABLES //////////////////////
	//////////////////////////////////////////////////////////////

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		arcadeHighScore.text = CubilineApplication.player.bestArcadeScore.ToString();
		if(CubilineApplication.player.bestArcadeScoreDateTime.Year != 1)
			SetDateLeyend(arcadeObtained, CubilineApplication.player.bestArcadeScoreDateTime);
		if (CubilineApplication.player.lastArcadeScoreDateTime.Year != 1)
			SetDateLeyend(arcadeLastGame, CubilineApplication.player.lastArcadeScoreDateTime);
		arcadeGamesPlayed.text = CubilineApplication.player.arcadeGamesPlayed.ToString();

		int hours = (int)CubilineApplication.player.arcadeTimePlayed / 3600;
		int seconds = (int)CubilineApplication.player.arcadeTimePlayed - hours * 3600;
		int minutes = seconds / 60;
		seconds -= minutes * 60;

		arcadeTimePlayed.text = hours.ToString("D" + 2) + ":" + minutes.ToString("D" + 2) + ":" + seconds.ToString("D" + 2);

		arcadeTotalLength.text = CubilineApplication.player.totalArcadeLength.ToString() + "m";

	}

	public override void Select()
	{
		GetComponent<GraphicRaycaster>().enabled = true;

		okButton.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
		okButton.transform.GetChild(0).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;

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
		Unselect();
	}

	public void CancelAction()
	{
		Unselect();
	}

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
