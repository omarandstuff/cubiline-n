using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class AchievementsData
{
	public uint blueCount;
	public uint greenCount;
	public uint orangeCount;
	public uint purpleCount;
	public uint yellowCount;
	public uint redCount;
	public bool blueAchieve;
	public bool greenAchieve;
	public bool orangeAchieve;
	public bool purpleAchieve;
	public bool yellowAchieve;
	public bool redAchieve;
	public bool byScoreColorAchieve;
	public bool byLengthColorAchieve;
	public bool byFillColorAchieve;

	[XmlIgnore]
	public uint totalBlue = 1000;
	[XmlIgnore]
	public uint totalGreen = 100;
	[XmlIgnore]
	public uint totalOrange = 100;
	[XmlIgnore]
	public uint totalPurple = 100;
	[XmlIgnore]
	public uint totalYellow = 100;
	[XmlIgnore]
	public uint totalRed = 100;
	[XmlIgnore]
	public uint scoreColorTarget = 2000;
	[XmlIgnore]
	public uint lengthColorTraget = 10000;
	[XmlIgnore]
	public uint fillColorTarget = 500;
}

public class SettingsData
{
	public uint player1ColorIndex;
	public uint player2ColorIndex;
	public uint arcadeLineSpeed = 4;
	public uint arcadeCubeSize = 15;
	public bool arcadeHardMove = false;
	public uint coopLineSpeed = 4;
	public uint coopCubeSize = 15;
	public bool coopHardMove = false;
}

public class PlayerData
{
	public string nickName = "Guest";
	public uint bestArcadeScore;
	public uint lastArcadeScore;
	public float arcadeTimePlayed;
	public uint arcadeGamesPlayed;
	public uint totalArcadeLength;
	public uint bestArcadeLength;
	public uint lastArcadeLength;
	public float lastArcadeTime;
	public DateTime bestArcadeScoreDateTime;
	public DateTime lastArcadeScoreDateTime;

	public uint bestCoopScore;
	public uint lastCoopScore;
	public float coopTimePlayed;
	public uint coopGamesPlayed;
	public uint totalCoopLength;
	public uint bestCoopLength;
	public uint lastCoopLength;
	public float lastCoopTime;
	public DateTime bestCoopScoreDateTime;
	public DateTime lastCoopScoreDateTime;

	[XmlIgnore]
	public bool newRecord;
	[XmlIgnore]
	public bool newLengthRecord;
	[XmlIgnore]
	public bool coopNewRecord;
	[XmlIgnore]
	public bool coopNewLengthRecord;
}

public class CubilineApplication : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// PARAMETERS /////////////////////////
	//////////////////////////////////////////////////////////////
	public static string lastComment;
	public static AchievementsData achievements;
	public static SettingsData settings;
	public static PlayerData player;

	private static bool loaded;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void Awake()
	{
		if (loaded)
		{
			Destroy(gameObject);
			return;
		}
		loaded = true;

		DontDestroyOnLoad(gameObject);
		achievements = new AchievementsData();
		settings = new SettingsData();
		player = new PlayerData();
		LoadAll();
	}

	public static void LoadAll()
	{
		LoadAchievements();
		LoadSettings();
		LoadPlayer();
	}

	public static void SaveAll()
	{
		SaveAchievements();
		SaveSettings();
		SavePlayer();
	}

	public static void SaveAchievements()
	{
		XmlSerializer serializer = new XmlSerializer(typeof(AchievementsData));
		FileStream stream = new FileStream(Application.persistentDataPath + "/c_a_d.dat", FileMode.Create);
		serializer.Serialize(stream, achievements);
#if !UNITY_WSA_10_0 || UNITY_EDITOR
		stream.Close();
#endif
	}

	public static void SaveSettings()
	{
		XmlSerializer serializer = new XmlSerializer(typeof(SettingsData));
		FileStream stream = new FileStream(Application.persistentDataPath + "/c_s_d.dat", FileMode.Create);
		serializer.Serialize(stream, settings);
#if !UNITY_WSA_10_0 || UNITY_EDITOR
		stream.Close();
#endif
	}

	public static void SavePlayer()
	{
		XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
		FileStream stream = new FileStream(Application.persistentDataPath + "/c_p_d.dat", FileMode.Create);
		serializer.Serialize(stream, player);
#if !UNITY_WSA_10_0 || UNITY_EDITOR
		stream.Close();
#endif
	}

	public static void LoadAchievements()
	{
		if (!File.Exists(Application.persistentDataPath + "/c_a_d.dat")) return;
		XmlSerializer serializer = new XmlSerializer(typeof(AchievementsData));
		FileStream stream = new FileStream(Application.persistentDataPath + "/c_a_d.dat", FileMode.Open);
		achievements = serializer.Deserialize(stream) as AchievementsData;

#if !UNITY_WSA_10_0 || UNITY_EDITOR
		stream.Close();
#endif
	}

	public static void LoadSettings()
	{
		if (!File.Exists(Application.persistentDataPath + "/c_s_d.dat")) return;
		XmlSerializer serializer = new XmlSerializer(typeof(SettingsData));
		FileStream stream = new FileStream(Application.persistentDataPath + "/c_s_d.dat", FileMode.Open);
		settings = serializer.Deserialize(stream) as SettingsData;

#if !UNITY_WSA_10_0 || UNITY_EDITOR
		stream.Close();
#endif
	}

	public static void LoadPlayer()
	{
		if (!File.Exists(Application.persistentDataPath + "/c_p_d.dat")) return;
		XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
		FileStream stream = new FileStream(Application.persistentDataPath + "/c_p_d.dat", FileMode.Open);
		player = serializer.Deserialize(stream) as PlayerData;

#if !UNITY_WSA_10_0 || UNITY_EDITOR
		stream.Close();
#endif
	}

}
