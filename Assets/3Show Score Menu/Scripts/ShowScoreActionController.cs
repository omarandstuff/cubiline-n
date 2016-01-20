﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowScoreActionController : ActionContentController
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public EaseScore score;
	public EaseScore length;
	public EaseTime time;
	public GameObject newScoreRecord;
	public GameObject newLengthRecord;
	public GameObject NewRecordParticlesPrefab;
	public GameObject NewLengthRecordParticlesPrefab;
	public float timeToScore = 0.5f;
	public float timeToStatsLapse = 0.2f;

	//////////////////////////////////////////////////////////////
	///////////////////// CONTROL VARIABLES //////////////////////
	//////////////////////////////////////////////////////////////

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	IEnumerator Start()
	{
		StartCoroutine(PublicScore());

		yield return new WaitForSeconds(timeToScore);
		if (CubilineApplication.lastComment == "arcade_scene") score.score = CubilineApplication.player.lastArcadeScore;
		if (CubilineApplication.lastComment == "coop_2p_scene") score.score = CubilineApplication.player.lastCoopScore;
		score.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
		score.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;

		yield return new WaitForSeconds(timeToScore + timeToStatsLapse);
		if (CubilineApplication.lastComment == "arcade_scene") length.score = CubilineApplication.player.lastArcadeLength;
		if (CubilineApplication.lastComment == "coop_2p_scene") score.score = CubilineApplication.player.lastCoopLength;
		length.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
		length.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;

		if (CubilineApplication.player.newRecord)
		{
			newScoreRecord.GetComponent<EaseRotation>().easeFace = EaseVector3.EASE_FACE.IN;
			newScoreRecord.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
			newScoreRecord.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
			Destroy(Instantiate(NewRecordParticlesPrefab, newScoreRecord.transform.position, Quaternion.identity), 8);
		}

		yield return new WaitForSeconds(timeToScore + timeToStatsLapse * 2);
		if (CubilineApplication.lastComment == "arcade_scene") time.time = (uint)CubilineApplication.player.lastArcadeTime;
		if (CubilineApplication.lastComment == "coop_2p_scene") time.time = (uint)CubilineApplication.player.lastCoopTime;
		time.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
		time.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;

		if (CubilineApplication.player.newLengthRecord)
		{
			newLengthRecord.GetComponent<EaseRotation>().easeFace = EaseVector3.EASE_FACE.IN;
			newLengthRecord.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
			newLengthRecord.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
			Destroy(Instantiate(NewLengthRecordParticlesPrefab, newLengthRecord.transform.position, Quaternion.identity), 8);
		}
	}

	public override void Select()
	{
	}

	public override void Unselect()
	{
		base.Unselect();
	}

	public override void Enter()
	{

	}

	public override void Leave()
	{
	}

	private IEnumerator PublicScore()
	{
		// Create a form object for sending high score data to the server
		//WWWForm form = new WWWForm();
		//form.AddField("demo_score[players]", "Jose");//CubilineApplication.player1Name + " Feat. " + CubilineApplication.player2Name);
		//form.AddField("demo_score[score]", CubilineApplication.lastArcadeScore.ToString());

		// Create a download object
		//WWW download = new WWW("http://www.cubiline.com/demo_score", form);

		// Wait until the download is done
		yield return new WaitForSeconds(1);

		//if (!string.IsNullOrEmpty(download.error))
		//{
			//print("Error downloading: " + download.error);
		//}
	}
}
