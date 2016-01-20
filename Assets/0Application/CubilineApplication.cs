using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class AchievementsData
{
	public uint blueCount;
	public uint greenCount;
	public uint orangeCount;
	public uint yellowCount;
	public uint purpleCount;
	public bool blueAchieve;
	public bool greenAchieve;
	public bool orangeAchieve;
	public bool yellowAchieve;
	public bool redAchieve;
	public bool purpleAchieve;
	public bool byScoreColorAchieve;
	public bool byLengthColorAchieve;
	public bool byFillColorAchieve;

	[XmlIgnore]
	public uint blueColorTraget = 1000;
	[XmlIgnore]
	public uint orangeColorTraget = 100;
	[XmlIgnore]
	public uint greenColorTraget = 100;
	[XmlIgnore]
	public uint yellowColorTraget = 100;
	[XmlIgnore]
	public uint redColorTraget = 100;
	[XmlIgnore]
	public uint purpleColorTraget = 100;
	[XmlIgnore]
	public uint scoreColorTarget = 5000;
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
	public string lastComment;
	public AchievementsData achievements;
	public SettingsData settings;
	public PlayerData player;

	public GameObject blueAchivementPortalPrefab;
	public GameObject orangeAchivementPortalPrefab;
	public GameObject greenAchivementPortalPrefab;
	public GameObject yellowAchivementPortalPrefab;
	public GameObject redAchivementPortalPrefab;
	public GameObject purpleAchivementPortalPrefab;
	public GameObject scoreColorAchivementPortalPrefab;
	public GameObject lengthColorAchivementPortalPrefab;
	public GameObject fillColorAchivementPortalPrefab;

	private static bool loaded;

	private static CubilineApplication thisone;
	public static CubilineApplication singleton { get { return thisone; } }

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

		thisone = this;
		DontDestroyOnLoad(gameObject);
		achievements = new AchievementsData();
		settings = new SettingsData();
		player = new PlayerData();
		LoadAll();
	}

	public void LoadAll()
	{
		LoadAchievements();
		LoadSettings();
		LoadPlayer();
	}

	public void SaveAll()
	{
		SaveAchievements();
		SaveSettings();
		SavePlayer();
	}

	public void SaveAchievements()
	{
		XmlSerializer serializer = new XmlSerializer(typeof(AchievementsData));
		FileStream stream = new FileStream(Application.persistentDataPath + "/c_a_d.dat", FileMode.Create);
		serializer.Serialize(stream, achievements);
#if !UNITY_WSA_10_0 || UNITY_EDITOR
		stream.Close();
#endif
	}

	public void SaveSettings()
	{
		XmlSerializer serializer = new XmlSerializer(typeof(SettingsData));
		FileStream stream = new FileStream(Application.persistentDataPath + "/c_s_d.dat", FileMode.Create);
		serializer.Serialize(stream, settings);
#if !UNITY_WSA_10_0 || UNITY_EDITOR
		stream.Close();
#endif
	}

	public void SavePlayer()
	{
		XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
		FileStream stream = new FileStream(Application.persistentDataPath + "/c_p_d.dat", FileMode.Create);
		serializer.Serialize(stream, player);
#if !UNITY_WSA_10_0 || UNITY_EDITOR
		stream.Close();
#endif
	}

	public void LoadAchievements()
	{
		if (!File.Exists(Application.persistentDataPath + "/c_a_d.dat")) return;
		XmlSerializer serializer = new XmlSerializer(typeof(AchievementsData));
		FileStream stream = new FileStream(Application.persistentDataPath + "/c_a_d.dat", FileMode.Open);
		achievements = serializer.Deserialize(stream) as AchievementsData;

#if !UNITY_WSA_10_0 || UNITY_EDITOR
		stream.Close();
#endif
	}

	public void LoadSettings()
	{
		if (!File.Exists(Application.persistentDataPath + "/c_s_d.dat")) return;
		XmlSerializer serializer = new XmlSerializer(typeof(SettingsData));
		FileStream stream = new FileStream(Application.persistentDataPath + "/c_s_d.dat", FileMode.Open);
		settings = serializer.Deserialize(stream) as SettingsData;

#if !UNITY_WSA_10_0 || UNITY_EDITOR
		stream.Close();
#endif
	}

	public void LoadPlayer()
	{
		if (!File.Exists(Application.persistentDataPath + "/c_p_d.dat")) return;
		XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
		FileStream stream = new FileStream(Application.persistentDataPath + "/c_p_d.dat", FileMode.Open);
		player = serializer.Deserialize(stream) as PlayerData;

#if !UNITY_WSA_10_0 || UNITY_EDITOR
		stream.Close();
#endif
	}


	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// ACHIEVEMENTS //////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void CheckBlueColorAchievement()
	{
		if (!achievements.blueAchieve)
		{
			if (achievements.blueCount >= achievements.blueColorTraget)
			{
				achievements.blueAchieve = true;
				GameObject ap = Instantiate(blueAchivementPortalPrefab);
				ap.transform.SetParent(transform);
				ap.GetComponent<RectTransform>().offsetMax = Vector2.zero;
				ap.GetComponent<RectTransform>().offsetMin = Vector2.zero;
				SaveAchievements();
			}
		}
	}

	public void CheckOrangeColorAchievement()
	{
		if (!achievements.orangeAchieve)
		{
			if (achievements.orangeCount >= achievements.orangeColorTraget)
			{
				achievements.orangeAchieve = true;
				GameObject ap = Instantiate(orangeAchivementPortalPrefab);
				ap.transform.SetParent(transform);
				ap.GetComponent<RectTransform>().offsetMax = Vector2.zero;
				ap.GetComponent<RectTransform>().offsetMin = Vector2.zero;
				SaveAchievements();
			}
		}
	}

	public void CheckGreenColorAchievement()
	{
		if (!achievements.greenAchieve)
		{
			if (achievements.greenCount >= achievements.greenColorTraget)
			{
				achievements.greenAchieve = true;
				GameObject ap = Instantiate(greenAchivementPortalPrefab);
				ap.transform.SetParent(transform);
				ap.GetComponent<RectTransform>().offsetMax = Vector2.zero;
				ap.GetComponent<RectTransform>().offsetMin = Vector2.zero;
				SaveAchievements();
			}
		}
	}

	public void CheckYellowColorAchievement()
	{
		if (!achievements.yellowAchieve)
		{
			if (achievements.yellowCount >= achievements.yellowColorTraget)
			{
				achievements.yellowAchieve = true;
				GameObject ap = Instantiate(yellowAchivementPortalPrefab);
				ap.transform.SetParent(transform);
				ap.GetComponent<RectTransform>().offsetMax = Vector2.zero;
				ap.GetComponent<RectTransform>().offsetMin = Vector2.zero;
				SaveAchievements();
			}
		}
	}

	public void CheckRedColorAchievement()
	{
		if (!achievements.redAchieve)
		{
			if (player.arcadeGamesPlayed + player.coopGamesPlayed >= achievements.redColorTraget)
			{
				achievements.redAchieve = true;
				GameObject ap = Instantiate(redAchivementPortalPrefab);
				ap.transform.SetParent(transform);
				ap.GetComponent<RectTransform>().offsetMax = Vector2.zero;
				ap.GetComponent<RectTransform>().offsetMin = Vector2.zero;
				SaveAchievements();
			}
		}
	}

	public void CheckPurpleColorAchievement()
	{
		if (!achievements.purpleAchieve)
		{
			if (achievements.orangeCount >= achievements.purpleColorTraget)
			{
				achievements.purpleAchieve = true;
				GameObject ap = Instantiate(purpleAchivementPortalPrefab);
				ap.transform.SetParent(transform);
				ap.GetComponent<RectTransform>().offsetMax = Vector2.zero;
				ap.GetComponent<RectTransform>().offsetMin = Vector2.zero;
				SaveAchievements();
			}
		}
	}

	public void CheckScoreColorAchievement()
	{
		if (!achievements.byScoreColorAchieve)
		{
			if (player.lastArcadeScore >= achievements.scoreColorTarget || player.lastCoopScore >= achievements.scoreColorTarget)
			{
				achievements.byScoreColorAchieve = true;
				GameObject ap = Instantiate(scoreColorAchivementPortalPrefab);
				ap.transform.SetParent(transform);
				ap.GetComponent<RectTransform>().offsetMax = Vector2.zero;
				ap.GetComponent<RectTransform>().offsetMin = Vector2.zero;
				SaveAchievements();
			}
		}
	}

	public void CheckLengthColorAchievement()
	{
		if (!achievements.byLengthColorAchieve)
		{
			if (player.totalArcadeLength >= achievements.lengthColorTraget || player.totalCoopLength >= achievements.lengthColorTraget)
			{
				achievements.byLengthColorAchieve = true;
				GameObject ap = Instantiate(lengthColorAchivementPortalPrefab);
				ap.transform.SetParent(transform);
				ap.GetComponent<RectTransform>().offsetMax = Vector2.zero;
				ap.GetComponent<RectTransform>().offsetMin = Vector2.zero;
				SaveAchievements();
			}
		}
	}

	public void CheckFillColorAchievement()
	{
		if (!achievements.byFillColorAchieve)
		{
			if (player.lastArcadeLength >= achievements.fillColorTarget || player.lastCoopLength >= achievements.fillColorTarget)
			{
				achievements.byFillColorAchieve = true;
				GameObject ap = Instantiate(fillColorAchivementPortalPrefab);
				ap.transform.SetParent(transform);
				ap.GetComponent<RectTransform>().offsetMax = Vector2.zero;
				ap.GetComponent<RectTransform>().offsetMin = Vector2.zero;
				SaveAchievements();
			}
		}
	}
}
