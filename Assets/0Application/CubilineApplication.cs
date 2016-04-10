using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine;

[Serializable]
public class SettingsData
{
	public int qualityIndex = -1;
	public bool particles = true;
	public bool depthOfField;
	public bool ambientOcclusion;
	public float masterSoundLevel = 1;
}

[Serializable]
public class PlayerData
{
	public string nickName = "Awesome Player";
	public uint bestArcadeScore = 0;
	public uint lastArcadeScore = 0;
	public float arcadeTimePlayed = 0;
	public uint arcadeGamesPlayed = 0;
	public uint totalArcadeLength = 0;
	public uint bestArcadeLength = 0;
	public uint lastArcadeLength = 0;
	public float lastArcadeTime = 0;
	public DateTime bestArcadeScoreDateTime;
	public DateTime lastArcadeScoreDateTime;

	public uint bestCoopScore = 0;
	public uint lastCoopScore = 0;
	public float coopTimePlayed = 0;
	public uint coopGamesPlayed = 0;
	public uint totalCoopLength = 0;
	public uint bestCoopLength = 0;
	public uint lastCoopLength = 0;
	public float lastCoopTime = 0;
	public DateTime bestCoopScoreDateTime;
	public DateTime lastCoopScoreDateTime;

	[XmlIgnore]
	[NonSerialized]
	public bool newRecord;
	[XmlIgnore]
	[NonSerialized]
	public bool newLengthRecord;
	[XmlIgnore]
	[NonSerialized]
	public bool coopNewRecord;
	[XmlIgnore]
	[NonSerialized]
	public bool coopNewLengthRecord;

	// Settings
	public uint player1ColorIndex = 0;
	public uint player2ColorIndex = 0;
	public uint arcadeLevelIndex = 0;
	public uint coopLevelIndex = 0;
	public uint arcadeLineSpeed = 4;
	public uint arcadeCubeSize = 15;
	public bool arcadeHardMove = false;
	public uint coopLineSpeed = 4;
	public uint coopCubeSize = 15;
	public bool coopHardMove = false;
	public bool notFirstTime = false;
	[XmlIgnore]
	[NonSerialized]
	public Color securePlayer1Color;
	[XmlIgnore]
	[NonSerialized]
	public Color securePlayer2Color;

	// Achievements
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
}

public class AchievementsData
{
	public static uint blueColorTraget = 1000;
	public static uint orangeColorTraget = 100;
	public static uint greenColorTraget = 100;
	public static uint yellowColorTraget = 100;
	public static uint redColorTraget = 100;
	public static uint purpleColorTraget = 100;
	public static uint scoreColorTarget = 5000;
	public static uint lengthColorTraget = 10000;
	public static uint fillColorTarget = 500;
	public static uint blackCubeTarget = 100;
	public static bool diceCheck1;
	public static bool diceCheck2;
	public static bool diceCheck3;
	public static bool blackdiceCheck1;
	public static bool blackdiceCheck2;
	public static bool blackdiceCheck3;
	public static uint toyTarget = 100;
	public static uint blacKToyTarget = 200;
	public static uint blackIncognitTarget = 36000;
}

public class CubilineApplication : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// PARAMETERS /////////////////////////
	//////////////////////////////////////////////////////////////
	public string lastComment;
	public SettingsData settings;
	public PlayerData player;
	public ulong playTime;

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

	public static string deviceID;

	// Social //
	public bool logedIn;

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

		settings = new SettingsData();
		player = new PlayerData();
		LoadAll();


		deviceID = SystemInfo.deviceUniqueIdentifier;
		AudioListener.volume = settings.masterSoundLevel;


		// First Level of quality
		if (settings.qualityIndex == -1)
		{
			settings.qualityIndex = 1;

			SaveSettings();
		}


		// Social API if any.
		StartSocialAPI();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////// SOCIAL API ///////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void StartSocialAPI()
	{
		LogIn ();
	}

	private void LogIn()
	{
		GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
		
		Social.localUser.Authenticate( success => {
			if (success)
			{
				CheckBoxLevelAchievement();
				if(player.nickName == "Awesome Player")
				   player.nickName = Social.localUser.userName;
			}
			else
			{
				Debug.Log ("Failed to authenticate");
			}
		});
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////// SAVE / LOAD //////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void LoadAll()
	{
		LoadSettings();
		LoadPlayer();
	}

	public void SaveAll()
	{
		SaveSettings();
		SavePlayer();
	}

	public void SaveSettings()
	{
		XmlSerializer serializer = new XmlSerializer(typeof(SettingsData));
		FileStream stream = new FileStream(Application.persistentDataPath + "/c_s_d.dat", FileMode.Create);
		serializer.Serialize(stream, settings);
		stream.Close();
	}

	public void SavePlayer()
	{
		IFormatter formatter = new BinaryFormatter();
		Stream stream = new FileStream(Application.persistentDataPath + "/c_p_d.dat", FileMode.Create, FileAccess.Write, FileShare.None);
		formatter.Serialize(stream, player);
		stream.Close();

		Social.ReportScore(player.lastArcadeScore, "high_score", (result) => {});
		Social.ReportScore(player.lastCoopScore, "high_score", (result) => {});
		Social.ReportScore(player.lastArcadeLength, "longest", (result) => {});
		Social.ReportScore(player.lastCoopLength, "longest", (result) => {});
	}

	public void LoadSettings()
	{
		if (!File.Exists(Application.persistentDataPath + "/c_s_d.dat")) return;
		XmlSerializer serializer = new XmlSerializer(typeof(SettingsData));
		FileStream stream = new FileStream(Application.persistentDataPath + "/c_s_d.dat", FileMode.Open);
		settings = serializer.Deserialize(stream) as SettingsData;
		stream.Close();
	}

	public void LoadPlayer()
	{
		if (!File.Exists(Application.persistentDataPath + "/c_p_d.dat")) return;
		IFormatter formatter = new BinaryFormatter();
		Stream stream = new FileStream(Application.persistentDataPath + "/c_p_d.dat", FileMode.Open, FileAccess.Read, FileShare.Read);
		player = (PlayerData)formatter.Deserialize(stream);
		stream.Close();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// ACHIEVEMENTS //////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void CheckBlueColorAchievement()
	{
		if (!player.blueAchieve)
		{
			GKAchievementReporter.ReportAchievement ("blue_eater", (float)Math.Min (player.blueCount * 100.0 / AchievementsData.blueColorTraget, 100.0), true);
			if (player.blueCount >= AchievementsData.blueColorTraget) {
				player.blueAchieve = true;
				inStack.Push (blueAchivementPortalPrefab);
				if (waithStack.Count == 0)
					ShowPortal ();
				SavePlayer ();

				CheckBlackBoxLevelAchievement ();
			}
		}
		else
		{
			GKAchievementReporter.ReportAchievement ("blue_eater", 100.0f, true);
		}
	}

	public void CheckOrangeColorAchievement()
	{
		if (!player.orangeAchieve)
		{
			GKAchievementReporter.ReportAchievement( "orange_eater", (float)Math.Min(player.orangeCount * 100.0 / AchievementsData.orangeColorTraget, 100.0), true);
			if (player.orangeCount >= AchievementsData.orangeColorTraget)
			{
				player.orangeAchieve = true;
				inStack.Push(orangeAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();

				CheckBlackBoxLevelAchievement();
			}
		}
		else
		{
			GKAchievementReporter.ReportAchievement ("orange_eater", 100.0f, true);
		}
	}

	public void CheckGreenColorAchievement()
	{
		if (!player.greenAchieve)
		{
			GKAchievementReporter.ReportAchievement( "green_eater", (float)Math.Min(player.greenCount * 100.0 / AchievementsData.greenColorTraget, 100.0), true);
			if (player.greenCount >= AchievementsData.greenColorTraget)
			{
				player.greenAchieve = true;
				inStack.Push(greenAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();

				CheckBlackBoxLevelAchievement();
			}
		}
		else
		{
			GKAchievementReporter.ReportAchievement ("green_eater", 100.0f, true);
		}
	}
			
	public void CheckYellowColorAchievement()
	{
		if (!player.yellowAchieve)
		{
			GKAchievementReporter.ReportAchievement( "multiplier_eater", (float)Math.Min(player.yellowCount * 100.0 / AchievementsData.yellowColorTraget, 100.0), true);
			if (player.yellowCount >= AchievementsData.yellowColorTraget)
			{
				player.yellowAchieve = true;
				inStack.Push(yellowAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();

				CheckBlackBoxLevelAchievement();
			}
		}
		else
		{
			GKAchievementReporter.ReportAchievement ("multiplier_eater", 100.0f, true);
		}
	}
			
	public void CheckRedColorAchievement()
	{
		if (!player.redAchieve)
		{
			GKAchievementReporter.ReportAchievement( "cubiline_player", (float)Math.Min((player.arcadeGamesPlayed + player.coopGamesPlayed) * 100.0 / AchievementsData.redColorTraget, 100.0), true);
			if (player.arcadeGamesPlayed + player.coopGamesPlayed >= AchievementsData.redColorTraget)
			{
				player.redAchieve = true;
				inStack.Push(redAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();

				CheckBlackBoxLevelAchievement();
			}
		}
		else
		{
			GKAchievementReporter.ReportAchievement ("cubiline_player", 100.0f, true);
		}
	}
	
	public void CheckPurpleColorAchievement()
	{
		if (!player.purpleAchieve)
		{
			GKAchievementReporter.ReportAchievement( "magnet_eater", (float)Math.Min(player.purpleCount * 100.0 / AchievementsData.purpleColorTraget, 100.0), true);
			if (player.purpleCount >= AchievementsData.purpleColorTraget)
			{
				player.purpleAchieve = true;
				inStack.Push(purpleAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();

				CheckBlackBoxLevelAchievement();
			}
		}
		else
		{
			GKAchievementReporter.ReportAchievement ("magnet_eater", 100.0f, true);
		}
	}
			
	public void CheckScoreColorAchievement()
	{
		if (!player.byScoreColorAchieve)
		{
			if (player.lastArcadeScore >= AchievementsData.scoreColorTarget || player.lastCoopScore >= AchievementsData.scoreColorTarget)
			{
				GKAchievementReporter.ReportAchievement( "big_player", 100.0f, true);

				player.byScoreColorAchieve = true;
				inStack.Push(scoreColorAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();

				CheckBlackBoxLevelAchievement();
			}
		}
		else
		{
			GKAchievementReporter.ReportAchievement ("big_player", 100.0f, true);
		}
	}
			
	public void CheckLengthColorAchievement()
	{
		if (!player.byLengthColorAchieve)
		{
			if (player.totalArcadeLength >= AchievementsData.lengthColorTraget || player.totalCoopLength >= AchievementsData.lengthColorTraget)
			{
				GKAchievementReporter.ReportAchievement( "anaconda", 100.0f, true);

				player.byLengthColorAchieve = true;
				inStack.Push(lengthColorAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();

				CheckBlackBoxLevelAchievement();
			}
		}
		else
		{
			GKAchievementReporter.ReportAchievement ("anaconda", 100.0f, true);
		}
	}

	public void CheckFillColorAchievement()
	{
		if (!player.byFillColorAchieve)
		{
			if (player.lastArcadeLength >= AchievementsData.fillColorTarget || player.lastCoopLength >= AchievementsData.fillColorTarget)
			{
				GKAchievementReporter.ReportAchievement( "indianapolis_500", 100.0f, true);

				player.byFillColorAchieve = true;
				inStack.Push(fillColorAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();

				CheckBlackBoxLevelAchievement();
			}
		}
		else
		{
			GKAchievementReporter.ReportAchievement ("indianapolis_500", 100.0f, true);
		}
	}

	public void CheckBlackLevelAchievement()
	{
		if (!player.blackCubeAchieve)
		{
			GKAchievementReporter.ReportAchievement( "gray_eater", (float)Math.Min(player.grayCount * 100.0 / AchievementsData.blackCubeTarget, 100.0), true);
			if (player.grayCount >= AchievementsData.blackCubeTarget)
			{
				player.blackCubeAchieve = true;
				inStack.Push(blackLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();
			}
		}
		else
		{
			GKAchievementReporter.ReportAchievement ("gray_eater", 100.0f, true);
		}
	}

	public void CheckDiceLevelAchievement()
	{
		if (!player.diceAchieve)
		{
			if (AchievementsData.diceCheck1 && AchievementsData.diceCheck2 && AchievementsData.diceCheck3)
			{
				GKAchievementReporter.ReportAchievement( "lucky_player", 100.0f, true);

				player.diceAchieve = true;
				inStack.Push(diceLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();
			}
		}
		else
		{
			GKAchievementReporter.ReportAchievement ("lucky_player", 100.0f, true);
		}
	}

	public void CheckBlackDiceLevelAchievement()
	{
		if (!player.blackDiceAchieve)
		{
			if (AchievementsData.blackdiceCheck1 && AchievementsData.blackdiceCheck2 && AchievementsData.blackdiceCheck3)
			{
				GKAchievementReporter.ReportAchievement( "luckiest_player", 100.0f, true);

				player.blackDiceAchieve = true;
				inStack.Push(blackDiceLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();
			}
		}
		else
		{
			GKAchievementReporter.ReportAchievement ("luckiest_player", 100.0f, true);
		}
	}
			
	public void CheckToyLevelAchievement()
	{
		if (!player.toyAchieve)
		{
			if ((player.lastArcadeScore >= 100 && player.lastArcadeLength == 3) || (player.lastCoopScore >= 100 && player.lastCoopLength == 6))
			{
				GKAchievementReporter.ReportAchievement( "logical_player", 100.0f, true);

				player.toyAchieve = true;
				inStack.Push(toyLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();
			}
		}
		else
		{
			GKAchievementReporter.ReportAchievement ("logical_player", 100.0f, true);
		}
	}
			
	public void CheckBlackToyLevelAchievement()
	{
		if (!player.blackToyAchieve)
		{
			if ((player.lastArcadeScore >= 200 && player.lastArcadeLength == 3) || (player.lastCoopScore >= 200 && player.lastCoopLength == 6))
			{
				GKAchievementReporter.ReportAchievement( "strategist_player", 100.0f, true);

				player.blackToyAchieve = true;
				inStack.Push(blackToyLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();
			}
		}
		else
		{
			GKAchievementReporter.ReportAchievement ("strategist_player", 100.0f, true);
		}
	}
			
	public void CheckBoxLevelAchievement()
	{
		if (!player.paperAchieve)
		{
			GKAchievementReporter.ReportAchievement( "open_the_box", 100.0f, true);

			player.paperAchieve = true;
			inStack.Push(boxLevelAchivementPortalPrefab);
			if (waithStack.Count == 0) ShowPortal();
			SavePlayer();
		}
		else
		{
			GKAchievementReporter.ReportAchievement ("open_the_box", 100.0f, true);
		}
	}
			
	public void CheckBlackBoxLevelAchievement()
	{
		if (!player.blackPaperAchieve)
		{
			if(player.blueAchieve && player.orangeAchieve && player.greenAchieve && player.yellowAchieve && player.redAchieve && player.purpleAchieve && player.byScoreColorAchieve && player.byLengthColorAchieve && player.byFillColorAchieve)
			{
				GKAchievementReporter.ReportAchievement( "colorfull", 100.0f, true);

				player.blackPaperAchieve = true;
				inStack.Push(blackBoxLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();
			}
		}
		else
		{
			GKAchievementReporter.ReportAchievement ("colorfull", 100.0f, true);
		}
	}
			
	public void CheckKnowledgeLevelAchievement()
	{
		if (!player.incognitAchieve)
		{
			GKAchievementReporter.ReportAchievement( "wiseman", 100.0f, true);

			player.incognitAchieve = true;
			inStack.Push(knowledgeLevelAchivementPortalPrefab);
			if (waithStack.Count == 0) ShowPortal();
			SavePlayer();
		}
		else
		{
			GKAchievementReporter.ReportAchievement ("wiseman", 100.0f, true);
		}
	}
			
	public void CheckBlackKnowledgeLevelAchievement()
	{
		if (!player.blackIncognitAchieve)
		{
			if(player.arcadeTimePlayed + player.coopTimePlayed >= AchievementsData.blackIncognitTarget)
			{
				GKAchievementReporter.ReportAchievement( "cubiline_fan", 100.0f, true);

				player.blackIncognitAchieve = true;
				inStack.Push(blackKnowledgeLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();
			}
		}
		else
		{
			GKAchievementReporter.ReportAchievement ("cubiline_fan", 100.0f, true);
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
