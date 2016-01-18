using UnityEngine;
using UnityEngine.UI;

public class EaseTime : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////

	public bool showPosFixes;
	public float timeSmoothTime = 0.1f;
	public uint time
	{
		get
		{
			return totalScore;
		}
		set
		{
			totalScore = value;
			targetScore = totalScore + 0.5f;
		}
	}

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	private uint totalScore;
	private float targetScore;
	private float currentScore;
	private float velocity = 0;


	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void FixedUpdate()
	{
		currentScore = Mathf.SmoothDamp(currentScore, targetScore, ref velocity, timeSmoothTime);

		int hours = (int)currentScore / 3600;
		int seconds = (int)currentScore - hours * 3600;
		int minutes = seconds / 60;
		seconds -= minutes * 60;

		if (showPosFixes)
			GetComponent<Text>().text = hours.ToString("D" + 2) + (showPosFixes ? (hours == 1 ? " hour, " : " hours, ") : "") + minutes.ToString("D" + 2) + (showPosFixes ? (minutes == 1 ? " minute, " : " minutes, ") : "") + seconds.ToString("D" + 2) + (showPosFixes ? (seconds == 1 ? " second " : " seconds ") : "");
		else
			GetComponent<Text>().text = hours.ToString("D" + 2) + ":" + minutes.ToString("D" + 2) + ":" + seconds.ToString("D" + 2);
	}
}
