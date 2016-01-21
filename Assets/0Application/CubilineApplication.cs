using System;
using System.Collections;
using System.Collections.Generic;
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

	public uint grayCount;
	public bool blackCubeAchieve;
	public bool diceAchieve;
	public bool blackDiceAchieve;
	public bool toyAchieve;
	public bool blackToyAchieve;
	public bool paperAchieve;
	public bool blackPaperAchieve;
	public bool incognitAchieve;
	public bool blackIncognitAchieve;


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

	[XmlIgnore]
	public uint blackCubeTarget = 100;
	[XmlIgnore]
	public bool diceCheck1;
	[XmlIgnore]
	public bool diceCheck2;
	[XmlIgnore]
	public bool diceCheck3;
	[XmlIgnore]
	public bool blackdiceCheck1;
	[XmlIgnore]
	public bool blackdiceCheck2;
	[XmlIgnore]
	public bool blackdiceCheck3;
	[XmlIgnore]
	public uint toyTarget = 100;
	[XmlIgnore]
	public uint blacKToyTarget = 200;
	[XmlIgnore]
	public uint blackIncognitTarget = 1;
}

public class SettingsData
{
	public uint player1ColorIndex;
	public uint player2ColorIndex;
	public uint arcadeLevelIndex;
	public uint coopLevelIndex;
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

	public GameObject blackLevelAchivementPortalPrefab;
	public GameObject diceLevelAchivementPortalPrefab;
	public GameObject blackDiceLevelAchivementPortalPrefab;
	public GameObject toyLevelAchivementPortalPrefab;
	public GameObject blackToyLevelAchivementPortalPrefab;
	public GameObject boxLevelAchivementPortalPrefab;
	public GameObject blackBoxLevelAchivementPortalPrefab;
	public GameObject knowledgeLevelAchivementPortalPrefab;
	public GameObject blackKnowledgeLevelAchivementPortalPrefab;


	[Serializable]
	public struct Level { public GameObject levelPrefav; public string levelName; public string levelLeyend; }
	public Level[] levels;

	private static bool loaded;

	private static CubilineApplication thisone;
	public static CubilineApplication singleton { get { return thisone; } }

	private Stack<GameObject> inStack = new Stack<GameObject>();
	public Stack waithStack = new Stack();

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
				inStack.Push(blueAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
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
				inStack.Push(orangeAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
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
				inStack.Push(greenAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
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
				inStack.Push(yellowAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
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
				inStack.Push(redAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SaveAchievements();
			}
		}
	}

	public void CheckPurpleColorAchievement()
	{
		if (!achievements.purpleAchieve)
		{
			if (achievements.purpleCount >= achievements.purpleColorTraget)
			{
				achievements.purpleAchieve = true;
				inStack.Push(purpleAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
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
				inStack.Push(scoreColorAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
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
				inStack.Push(lengthColorAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
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
				inStack.Push(fillColorAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SaveAchievements();
			}
		}
	}

	public void CheckBlackLevelAchievement()
	{
		if (!achievements.blackCubeAchieve)
		{
			if (achievements.grayCount >= achievements.blackCubeTarget)
			{
				achievements.blackCubeAchieve = true;
				inStack.Push(blackLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SaveAchievements();
			}
		}
	}

	public void CheckDiceLevelAchievement()
	{
		if (!achievements.diceAchieve)
		{
			if (achievements.diceCheck1 && achievements.diceCheck2 && achievements.diceCheck3)
			{
				achievements.diceAchieve = true;
				inStack.Push(diceLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SaveAchievements();
			}
		}
	}

	public void CheckBlackDiceLevelAchievement()
	{
		if (!achievements.blackDiceAchieve)
		{
			if (achievements.blackdiceCheck1 && achievements.blackdiceCheck2 && achievements.blackdiceCheck3)
			{
				achievements.blackDiceAchieve = true;
				inStack.Push(blackDiceLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SaveAchievements();
			}
		}
	}

	public void CheckToyLevelAchievement()
	{
		if (!achievements.toyAchieve)
		{
			if ((player.lastArcadeScore >= 100 && player.lastArcadeLength == 3) || (player.lastCoopScore >= 100 && player.lastCoopLength == 6))
			{
				achievements.toyAchieve = true;
				inStack.Push(toyLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SaveAchievements();
			}
		}
	}

	public void CheckBlackToyLevelAchievement()
	{
		if (!achievements.blackToyAchieve)
		{
			if ((player.lastArcadeScore >= 200 && player.lastArcadeLength == 3) || (player.lastCoopScore >= 200 && player.lastCoopLength == 6))
			{
				achievements.blackToyAchieve = true;
				inStack.Push(blackToyLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SaveAchievements();
			}
		}
	}

	public void CheckBoxLevelAchievement()
	{
		if (!achievements.paperAchieve)
		{
			achievements.paperAchieve = true;
			inStack.Push(boxLevelAchivementPortalPrefab);
			if (waithStack.Count == 0) ShowPortal();
			SaveAchievements();
		}
	}

	public void CheckBlackBoxLevelAchievement()
	{
		if (!achievements.blackPaperAchieve)
		{
			if(achievements.blueAchieve && achievements.orangeAchieve && achievements.greenAchieve && achievements.yellowAchieve && achievements.redAchieve && achievements.purpleAchieve && achievements.byScoreColorAchieve && achievements.byLengthColorAchieve && achievements.byFillColorAchieve)
			{
				achievements.blackPaperAchieve = true;
				inStack.Push(blackBoxLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SaveAchievements();
			}
		}
	}

	public void CheckKnowledgeLevelAchievement()
	{
		if (!achievements.incognitAchieve)
		{
			achievements.incognitAchieve = true;
			inStack.Push(knowledgeLevelAchivementPortalPrefab);
			if (waithStack.Count == 0) ShowPortal();
			SaveAchievements();
		}
	}

	public void CheckBlackKnowledgeLevelAchievement()
	{
		if (!achievements.blackIncognitAchieve)
		{
			if(player.arcadeTimePlayed + player.coopTimePlayed >= 36000)
			{
				achievements.blackIncognitAchieve = true;
				inStack.Push(blackKnowledgeLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SaveAchievements();
			}
		}
	}

	public void ShowPortal()
	{
		if (inStack.Count == 0) return;
		waithStack.Push(0);

		GameObject ap = Instantiate(inStack.Pop());
		ap.transform.SetParent(transform);
		ap.GetComponent<RectTransform>().offsetMax = Vector2.zero;
		ap.GetComponent<RectTransform>().offsetMin = Vector2.zero;
	}
}
