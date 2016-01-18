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

	public uint bestCoopScore;
	public uint lastCoopScore;
	public float coopTimePlayed;
	public uint coopGamesPlayed;
	public uint totalCoopLength;
	public uint bestCoopLength;
	public uint lastCoopLength;
	public DateTime bestCoopScoreDateTime;
	public DateTime lastCoopScoreDateTime;
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

	public static uint bestCoopScore;
	public static uint lastCoopScore;
	public static float coopTimePlayed;
	public static uint coopGamesPlayed;
	public static uint totalCoopLength;
	public static uint bestCoopLength;
	public static uint lastCoopLength;
	public static DateTime bestCoopScoreDateTime;
	public static DateTime lastCoopScoreDateTime;


	public static void Save()
	{
		PlayerData pd = new PlayerData();
		pd.bestArcadeScore = bestArcadeScore;
		pd.lastArcadeScore = lastArcadeScore;
		pd.arcadeTimePlayed = arcadeTimePlayed;
		pd.arcadeGamesPlayed = arcadeGamesPlayed;
		pd.totalArcadeLength = totalArcadeLength;
		pd.bestArcadeScoreDateTime = bestArcadeScoreDateTime;
		pd.lastArcadeScoreDateTime = lastArcadeScoreDateTime;

		pd.bestCoopScore = bestCoopScore;
		pd.lastCoopScore = lastCoopScore;
		pd.coopTimePlayed = coopTimePlayed;
		pd.coopGamesPlayed = coopGamesPlayed;
		pd.totalCoopLength = totalCoopLength;
		pd.bestCoopScoreDateTime = bestCoopScoreDateTime;
		pd.lastCoopScoreDateTime = lastCoopScoreDateTime;

		XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
		FileStream stream = new FileStream(Application.persistentDataPath + "/cubiline.dat", FileMode.Create);
		serializer.Serialize(stream, pd);
	}

	public static void Load()
	{
		if (!File.Exists(Application.persistentDataPath + "/cubiline.dat")) return;
		XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
		FileStream stream = new FileStream(Application.persistentDataPath + "/cubiline.dat", FileMode.Open);
		PlayerData pd = serializer.Deserialize(stream) as PlayerData;

		bestArcadeScore = pd.bestArcadeScore;
		lastArcadeScore = pd.lastArcadeScore;
		arcadeTimePlayed = pd.arcadeTimePlayed;
		arcadeGamesPlayed = pd.arcadeGamesPlayed;
		totalArcadeLength = pd.totalArcadeLength;
		bestArcadeScoreDateTime = pd.bestArcadeScoreDateTime;
		lastArcadeScoreDateTime = pd.lastArcadeScoreDateTime;

		bestCoopScore = pd.bestCoopScore;
		lastCoopScore = pd.lastCoopScore;
		coopTimePlayed = pd.coopTimePlayed;
		coopGamesPlayed = pd.coopGamesPlayed;
		totalCoopLength = pd.totalCoopLength;
		bestCoopScoreDateTime = pd.bestCoopScoreDateTime;
		lastCoopScoreDateTime = pd.lastCoopScoreDateTime;
	}

}
