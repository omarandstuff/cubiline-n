using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerData
{
	public uint bestScore;
	public uint lastScore;
	public float timePlayed;
	public uint gamesPlayed;
	public uint totalLength;
	public DateTime bestScoreDateTime;
	public DateTime lastScoreDateTime;

	public PlayerData()
	{

	}

	public PlayerData(uint best_score, uint last_score, float time_played, uint games_played, uint total_length, DateTime best_score_date_time, DateTime last_score_date_time)
	{
		bestScore = best_score;
		lastScore = last_score;
		timePlayed = time_played;
		gamesPlayed = games_played;
		totalLength = total_length;
		bestScoreDateTime = best_score_date_time;
		lastScoreDateTime = last_score_date_time;
	}
}

public class CubilinePlayerData
{
	//////////////////////////////////////////////////////////////
	//////////////////////////// ARCADE //////////////////////////
	//////////////////////////////////////////////////////////////

	public static uint bestArcadeScore;
	public static uint lastArcadeScore;
	public static float arcadeTimePlayed;
	public static uint arcadeGamesPlayed;
	public static uint totalArcadeLength;
	public static DateTime bestArcadeScoreDateTime;
	public static DateTime lastArcadeScoreDateTime;


	public static void Save()
	{
		PlayerData pd = new PlayerData(bestArcadeScore, lastArcadeScore, arcadeTimePlayed, arcadeGamesPlayed, totalArcadeLength, bestArcadeScoreDateTime, lastArcadeScoreDateTime);
		XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
		FileStream stream = new FileStream(Application.persistentDataPath + "/cubiline.dat", FileMode.Create);
		serializer.Serialize(stream, pd);
		stream.Close();
	}

	public static void Load()
	{
		if (!File.Exists(Application.persistentDataPath + "/cubiline.dat")) return;
		XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
		FileStream stream = new FileStream(Application.persistentDataPath + "/cubiline.dat", FileMode.Open);
		PlayerData pd = serializer.Deserialize(stream) as PlayerData;
		
		stream.Close();

		bestArcadeScore = pd.bestScore;
		lastArcadeScore = pd.lastScore;
		arcadeTimePlayed = pd.timePlayed;
		arcadeGamesPlayed = pd.gamesPlayed;
		totalArcadeLength = pd.totalLength;
		bestArcadeScoreDateTime = pd.bestScoreDateTime;
		lastArcadeScoreDateTime = pd.lastScoreDateTime;

		CubilineScoreController.currentArcadeScore = bestArcadeScore;
		CubilineScoreController.currentArcadeScore = 0;
	}

}
