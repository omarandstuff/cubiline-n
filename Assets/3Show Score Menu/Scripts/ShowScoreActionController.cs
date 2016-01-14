using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowScoreActionController : ActionContentController
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public EaseScore score;

	//////////////////////////////////////////////////////////////
	///////////////////// CONTROL VARIABLES //////////////////////
	//////////////////////////////////////////////////////////////

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	IEnumerator Start()
	{
		StartCoroutine(PublicScore());

		yield return new WaitForSeconds(0.5f);
		score.score = CubilineScoreController.currentArcadeScore;
		score.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
		score.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
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
		form.AddField("demo_score[players]", "Jose");//CubilineApplication.player1Name + " Feat. " + CubilineApplication.player2Name);
		form.AddField("demo_score[score]", CubilineScoreController.bestArcadeScore.ToString());

		// Create a download object
		WWW download = new WWW("http://www.cubiline.com/demo_score", form);

		// Wait until the download is done
		yield return download;

		if (!string.IsNullOrEmpty(download.error))
		{
			print("Error downloading: " + download.error);
		}
	}
}
