using System.Collections;

public class CubilineScoreController
{
	//////////////////////////////////////////////////////////////
	///////////////////////// PROPERTIES /////////////////////////
	//////////////////////////////////////////////////////////////
	public static bool newRecord { get { return _newRecord; } }
	public static uint bestScore { get { return _bestScore; } }
	public static uint currentScore
	{
		get { return _currentScore; }
		set
		{
			_currentScore = value;
			FillScores();
			if(_currentScore > _bestScore)
			{
				_newRecord = true;
				_bestScore = _currentScore;
				FillBestScores();
			}
			else
			{
				_newRecord = false;
			}
		}
	}

	public static uint currentNumberOfPlayers;

	public static ArrayList scoreDependencies = new ArrayList();
	public static ArrayList bestScoreDependencies = new ArrayList();

	public static uint[] versusScores = new uint[4];

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	static private bool _newRecord;
	static private uint _bestScore;
	static private uint _currentScore;

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
