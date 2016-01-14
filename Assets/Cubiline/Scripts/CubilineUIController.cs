using UnityEngine;

public class CubilineUIController : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public EaseScore scoreText;
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
		scoreText.score = CubilineScoreController.currentArcadeScore;
		CubilineScoreController.arcadeScoreDependencies.Add(scoreText);
	}

	public void GoOut ()
	{
		scoreText.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
		scoreText.GetComponent<EaseTextOpasity>().easeFace = EaseTextOpasity.EASE_FACE.OUT;
		CubilineScoreController.arcadeScoreDependencies.Remove(scoreText);
		if (enableVerticalDivision) verticalDivision.easeFace = EaseFloat.EASE_FACE.OUT;
		if (enableHorizontalDivision) horizontalDivision.easeFace = EaseFloat.EASE_FACE.OUT;
	}
}
