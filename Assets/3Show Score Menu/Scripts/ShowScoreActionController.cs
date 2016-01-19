using UnityEngine;
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

		CubilineApplication.lastComment = "arcade_scene";
		CubilinePlayerData.lastArcadeScore = 650;
		CubilinePlayerData.lastArcadeLength = 325;
		CubilinePlayerData.lastArcadeTime = 156;

		CubilineApplication.newRecord = true;
		CubilineApplication.newLengthRecord = true;

		if (CubilineApplication.lastComment == "arcade_scene")
		{
			yield return new WaitForSeconds(timeToScore);
			score.score = CubilinePlayerData.lastArcadeScore;
			score.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
			score.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;

			yield return new WaitForSeconds(timeToScore + timeToStatsLapse);
			length.score = CubilinePlayerData.lastArcadeLength;
			length.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
			length.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
			
			if(CubilineApplication.newRecord)
			{
				newScoreRecord.GetComponent<EaseRotation>().easeFace = EaseVector3.EASE_FACE.IN;
				newScoreRecord.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
				newScoreRecord.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
				Destroy(Instantiate(NewRecordParticlesPrefab, newScoreRecord.transform.position, Quaternion.identity), 8);
			}

			yield return new WaitForSeconds(timeToScore + timeToStatsLapse * 2);
			time.time = (uint)CubilinePlayerData.lastArcadeTime;
			time.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
			time.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;

			if (CubilineApplication.newLengthRecord)
			{
				newLengthRecord.GetComponent<EaseRotation>().easeFace = EaseVector3.EASE_FACE.IN;
				newLengthRecord.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
				newLengthRecord.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
				Destroy(Instantiate(NewLengthRecordParticlesPrefab, newLengthRecord.transform.position, Quaternion.identity), 8);
			}

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
		//form.AddField("demo_score[score]", CubilinePlayerData.lastArcadeScore.ToString());

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
