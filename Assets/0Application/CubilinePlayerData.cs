using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerData
{
	public uint bestArcadeScore;
	public uint lastArcadeScore;
	public float arcadeTimePlayed;
	public uint arcadeGamesPlayed;
	public uint totalArcadeLength;
	public uint bestArcadeLength;
	public uint lastArcadeLength;
	public DateTime bestArcadeScoreDateTime;
	public DateTime lastArcadeScoreDateTime;

	public PlayerData()
	{

	}

	public PlayerData(uint best_arcade_score, uint last_arcade_score, float arcade_time_played, uint arcade_games_played, uint total_arcade_length, uint best_arcade_length, uint last_arcade_length, DateTime best_arcade_score_date_time, DateTime last_arcade_score_date_time)
	{
		bestArcadeScore = best_arcade_score;
		lastArcadeScore = last_arcade_score;
		arcadeTimePlayed = arcade_time_played;
		arcadeGamesPlayed = arcade_games_played;
		totalArcadeLength = total_arcade_length;
		bestArcadeLength = best_arcade_length;
		lastArcadeLength = last_arcade_length;
		bestArcadeScoreDateTime = best_arcade_score_date_time;
		lastArcadeScoreDateTime = last_arcade_score_date_time;
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
	public static uint bestArcadeLength;
	public static uint lastArcadeLength;
	public static DateTime bestArcadeScoreDateTime;
	public static DateTime lastArcadeScoreDateTime;


	public static void Save()
	{
		PlayerData pd = new PlayerData(bestArcadeScore, lastArcadeScore, arcadeTimePlayed, arcadeGamesPlayed, totalArcadeLength, bestArcadeLength, lastArcadeLength, bestArcadeScoreDateTime, lastArcadeScoreDateTime);
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

		bestArcadeScore = pd.bestArcadeScore;
		lastArcadeScore = pd.lastArcadeScore;
		arcadeTimePlayed = pd.arcadeTimePlayed;
		arcadeGamesPlayed = pd.arcadeGamesPlayed;
		totalArcadeLength = pd.totalArcadeLength;
		bestArcadeScoreDateTime = pd.bestArcadeScoreDateTime;
		lastArcadeScoreDateTime = pd.lastArcadeScoreDateTime;
	}

}
