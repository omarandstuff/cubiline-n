using System.Collections;

public class CubilineScoreController
{
	//////////////////////////////////////////////////////////////
	///////////////////////// PROPERTIES /////////////////////////
	//////////////////////////////////////////////////////////////
	public static bool newArcadeRecord { get { return _newArcadeRecord; } }
	public static uint bestArcadeScore { get { return _bestArcadeScore; } }
	public static uint currentArcadeScore
	{
		get { return _currentArcadeScore; }
		set
		{
			_currentArcadeScore = value;
			FillArcadeScores();
			if(_currentArcadeScore > _bestArcadeScore)
			{
				_newArcadeRecord = true;
				_bestArcadeScore = _currentArcadeScore;
				FillBestArcadeScores();
				CubilinePlayerData.bestArcadeScore = _bestArcadeScore;
			}
			else
			{
				_newArcadeRecord = false;
			}
		}
	}

	public static ArrayList arcadeScoreDependencies = new ArrayList();
	public static ArrayList bestArcadeScoreDependencies = new ArrayList();

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	static private bool _newArcadeRecord;
	static private uint _bestArcadeScore;
	static private uint _currentArcadeScore;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// SCORE CONTROL /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	static void FillArcadeScores()
	{
		foreach(EaseScore score in arcadeScoreDependencies)
		{
			score.score = _currentArcadeScore;
		}
	}

	static void FillBestArcadeScores()
	{
		foreach (EaseScore score in bestArcadeScoreDependencies)
		{
			score.score = _bestArcadeScore;
		}
	}
}
