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

		yield return new WaitForSeconds(timeToScore);
		if (CubilineApplication.singleton.lastComment == "arcade_scene") score.score = CubilineApplication.singleton.player.lastArcadeScore;
		if (CubilineApplication.singleton.lastComment == "coop_2p_scene") score.score = CubilineApplication.singleton.player.lastCoopScore;
		score.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
		score.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;

		yield return new WaitForSeconds(timeToScore + timeToStatsLapse);
		if (CubilineApplication.singleton.lastComment == "arcade_scene") length.score = CubilineApplication.singleton.player.lastArcadeLength;
		if (CubilineApplication.singleton.lastComment == "coop_2p_scene") length.score = CubilineApplication.singleton.player.lastCoopLength;
		length.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
		length.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;

		bool newRecord = CubilineApplication.singleton.lastComment == "arcade_scene" ? CubilineApplication.singleton.player.newRecord : CubilineApplication.singleton.player.coopNewRecord;

		if (newRecord)
		{
			newScoreRecord.GetComponent<EaseRotation>().easeFace = EaseVector3.EASE_FACE.IN;
			newScoreRecord.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
			newScoreRecord.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
			if (CubilineApplication.singleton.settings.particles) Destroy(Instantiate(NewRecordParticlesPrefab, newScoreRecord.transform.position, Quaternion.identity), 8);
		}

		yield return new WaitForSeconds(timeToScore + timeToStatsLapse * 2);
		if (CubilineApplication.singleton.lastComment == "arcade_scene") time.time = (uint)CubilineApplication.singleton.player.lastArcadeTime;
		if (CubilineApplication.singleton.lastComment == "coop_2p_scene") time.time = (uint)CubilineApplication.singleton.player.lastCoopTime;
		time.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
		time.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;

		newRecord = CubilineApplication.singleton.lastComment == "arcade_scene" ? CubilineApplication.singleton.player.newLengthRecord : CubilineApplication.singleton.player.coopNewLengthRecord;

		if (newRecord)
		{
			newLengthRecord.GetComponent<EaseRotation>().easeFace = EaseVector3.EASE_FACE.IN;
			newLengthRecord.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
			newLengthRecord.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
			if (CubilineApplication.singleton.settings.particles) Destroy(Instantiate(NewLengthRecordParticlesPrefab, newLengthRecord.transform.position, Quaternion.identity), 8);
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
		WWWForm form = new WWWForm();
		form.AddField("arcade[player]", CubilineApplication.singleton.player.nickName);
		if (CubilineApplication.singleton.lastComment == "arcade_scene") form.AddField("arcade[mode]", "arcade");
		if (CubilineApplication.singleton.lastComment == "coop_2p_scene") form.AddField("arcade[mode]", "coop");

		if (CubilineApplication.singleton.lastComment == "arcade_scene") form.AddField("arcade[score]", CubilineApplication.singleton.player.lastArcadeScore.ToString());
		if (CubilineApplication.singleton.lastComment == "coop_2p_scene") form.AddField("arcade[score]", CubilineApplication.singleton.player.lastCoopScore.ToString());

		form.AddField("arcade[device]", SystemInfo.operatingSystem);
		form.AddField("arcade[device_id]", CubilineApplication.deviceID);

		if (CubilineApplication.singleton.lastComment == "arcade_scene") form.AddField("arcade[token]", (CubilineApplication.singleton.player.lastArcadeScore * 13 + SystemInfo.operatingSystem.Length + SystemInfo.deviceUniqueIdentifier.Length).ToString());
		if (CubilineApplication.singleton.lastComment == "coop_2p_scene") form.AddField("arcade[token]", (CubilineApplication.singleton.player.lastCoopScore * 13 + SystemInfo.operatingSystem.Length + SystemInfo.deviceUniqueIdentifier.Length).ToString());

#if UNITY_WSA_10_0
#endif


		// Create a download object
		WWW download = new WWW("http://cubiline.com/MGOD", form);

		// Wait until the download is done
		yield return new WaitForSeconds(1);

		if (!string.IsNullOrEmpty(download.error))
		{
			print("Error downloading: " + download.error);
		}
	}
}
