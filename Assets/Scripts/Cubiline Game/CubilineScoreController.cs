using UnityEngine;
using System.Collections;

public class CubilineScoreController
{
	//////////////////////////////////////////////////////////////
	///////////////////////// PROPERTIES /////////////////////////
	//////////////////////////////////////////////////////////////
	static public uint bestScore { get { return _bestScore; } }
	static public uint currentScore
	{
		get { return _currentScore; }
		set
		{
			_currentScore = value;
			FillScores();
			if(_currentScore > _bestScore)
			{
				_bestScore = _currentScore;
				FillBestScores();
			}
		}
	}

	static public uint totalLength { get { return _totalLength; } }
	public static ArrayList scoreDependencies = new ArrayList();
	public static ArrayList bestScoreDependencies = new ArrayList();

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	static private uint _bestScore;
	static private uint _currentScore;
	static private uint _totalLength;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// SCORE CONTROL /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	static void FillScores()
	{
		foreach(EaseScore score in scoreDependencies)
		{
			score.score = _currentScore;
		}
	}

	static void FillBestScores()
	{
		foreach (EaseScore score in bestScoreDependencies)
		{
			score.score = _bestScore;
		}
	}
}
