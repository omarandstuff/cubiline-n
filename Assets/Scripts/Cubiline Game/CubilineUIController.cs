using UnityEngine;

public class CubilineUIController : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public EaseScore scoreText;
	public EaseScore bestScoreText;
	public bool enableHorizontalDivision;
	public EaseImageOpasity horizontalDivision;
	public bool enableVerticalDivision;
	public EaseImageOpasity verticalDivision;

	//////////////////////////////////////////////////////////////
	///////////////////////// PARAMETERS /////////////////////////
	//////////////////////////////////////////////////////////////

	public float timeToApear = 1.0f;

	//////////////////////////////////////////////////////////////
	///////////////////// CONTROL VARIABLES //////////////////////
	//////////////////////////////////////////////////////////////

	private float currentTime = 0;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////
	void Start()
	{
		if (enableVerticalDivision) verticalDivision.easeFace = EaseFloat.EASE_FACE.IN;
		if (enableHorizontalDivision) horizontalDivision.easeFace = EaseFloat.EASE_FACE.IN;
	}

	void Update ()
	{
		if (currentTime > timeToApear) return;
		currentTime += Time.deltaTime;
		if (currentTime >= timeToApear)
			ShowUI();
	}

	public void ShowUI()
	{
		scoreText.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
		scoreText.GetComponent<EaseTextOpasity>().easeFace = EaseTextOpasity.EASE_FACE.IN;
		bestScoreText.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
		bestScoreText.GetComponent<EaseTextOpasity>().easeFace = EaseTextOpasity.EASE_FACE.IN;
		scoreText.score = CubilineScoreController.currentScore;
		CubilineScoreController.scoreDependencies.Add(scoreText);
		bestScoreText.score = CubilineScoreController.bestScore;
		CubilineScoreController.bestScoreDependencies.Add(bestScoreText);
	}

	public void GoOut ()
	{
		scoreText.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
		scoreText.GetComponent<EaseTextOpasity>().easeFace = EaseTextOpasity.EASE_FACE.OUT;
		bestScoreText.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
		bestScoreText.GetComponent<EaseTextOpasity>().easeFace = EaseTextOpasity.EASE_FACE.OUT;
		CubilineScoreController.scoreDependencies.Remove(scoreText);
		CubilineScoreController.bestScoreDependencies.Remove(bestScoreText);
		if (enableVerticalDivision) verticalDivision.easeFace = EaseFloat.EASE_FACE.OUT;
		if (enableHorizontalDivision) horizontalDivision.easeFace = EaseFloat.EASE_FACE.OUT;
	}
}
