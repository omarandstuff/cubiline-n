using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
#endif

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

		Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");

		settings = new SettingsData();
		player = new PlayerData();
		LoadAll();


		deviceID = SystemInfo.deviceUniqueIdentifier;
		AudioListener.volume = settings.masterSoundLevel;


		// First Level of quality
		if (settings.qualityIndex == -1)
		{
			settings.qualityIndex = 0;

#if UNITY_WSA_10_0 || UNITY_WSA_8_1
			settings.qualityIndex = 2;
			QualitySettings.SetQualityLevel(settings.qualityIndex, true);
#endif
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
#if UNITY_ANDROID
		StartGoogleGameServices();
		LogIn();
#endif
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////// GOOGLE PLAY GAME SERVICES ///////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

#if UNITY_ANDROID
	private void StartGoogleGameServices()
	{
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();

		PlayGamesPlatform.InitializeInstance(config);

		// Activate the Google Play Games platform
		PlayGamesPlatform.Activate();
	}
#endif

	private void LogIn()
	{
#if UNITY_ANDROID
		if (logedIn) Social.localUser.Authenticate((bool success) =>
		{
			if(success)
			{
				if (logedIn) Social.ReportProgress(GPGSIds.achievement_open_the_box, 100.0f, (bool success_) => { });
				logedIn = true;
			}
		});
#endif
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
#if !UNITY_WSA_10_0 || UNITY_EDITOR
		stream.Close();
#endif
	}

	public void SavePlayer()
	{
#if UNITY_WSA || UNITY_WP8_1
		XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
		FileStream stream = new FileStream(Application.persistentDataPath + "/c_p_d.dat", FileMode.Create);
		serializer.Serialize(stream, player);

#if !UNITY_WSA_10_0 || UNITY_EDITOR
		stream.Close();
#endif

#endif

#if UNITY_ANDROID
		IFormatter formatter = new BinaryFormatter();
		Stream stream = new FileStream(Application.persistentDataPath + "/c_p_d.dat", FileMode.Create, FileAccess.Write, FileShare.None);
		formatter.Serialize(stream, player);
		stream.Close();

		if (logedIn) if (logedIn) Social.ReportScore(Math.Max(player.bestArcadeScore, player.bestCoopScore), GPGSIds.leaderboard_high_score, (bool success) => { });
		if (logedIn) if (logedIn) Social.ReportScore(Math.Max(player.bestArcadeLength, player.bestCoopLength), GPGSIds.leaderboard_longest, (bool success) => {});
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
#if UNITY_WSA || UNITY_WP8_1
		XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
		FileStream stream = new FileStream(Application.persistentDataPath + "/c_p_d.dat", FileMode.Open);
		player = serializer.Deserialize(stream) as PlayerData;

#if !UNITY_WSA_10_0 || UNITY_EDITOR
		stream.Close();
#endif
#endif

#if UNITY_ANDROID
		IFormatter formatter = new BinaryFormatter();
		Stream stream = new FileStream(Application.persistentDataPath + "/c_p_d.dat", FileMode.Open, FileAccess.Read, FileShare.Read);
		player = (PlayerData)formatter.Deserialize(stream);
		stream.Close();
#endif
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// ACHIEVEMENTS //////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void CheckBlueColorAchievement()
	{
		if (!player.blueAchieve)
		{
#if UNITY_ANDROID
			if (logedIn) Social.ReportProgress(GPGSIds.achievement_blue_eater, Mathf.Min(player.blueCount, AchievementsData.blueColorTraget) / AchievementsData.blueColorTraget * 100, (bool success) => { });
#endif
			if (player.blueCount >= AchievementsData.blueColorTraget)
			{
				player.blueAchieve = true;
				inStack.Push(blueAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();

				CheckBlackBoxLevelAchievement();
			}
		}
	}

	public void CheckOrangeColorAchievement()
	{
		if (!player.orangeAchieve)
		{
#if UNITY_ANDROID
			if (logedIn) Social.ReportProgress(GPGSIds.achievement_orange_eater, Mathf.Min(player.orangeCount, AchievementsData.orangeColorTraget) / AchievementsData.orangeColorTraget * 100, (bool success) => { });
#endif
			if (player.orangeCount >= AchievementsData.orangeColorTraget)
			{
				player.orangeAchieve = true;
				inStack.Push(orangeAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();

				CheckBlackBoxLevelAchievement();
			}
		}
	}

	public void CheckGreenColorAchievement()
	{
		if (!player.greenAchieve)
		{
#if UNITY_ANDROID
			if (logedIn) Social.ReportProgress(GPGSIds.achievement_green_eater, Mathf.Min(player.greenCount, AchievementsData.greenColorTraget) / AchievementsData.greenColorTraget * 100, (bool success) => { });
#endif
			if (player.greenCount >= AchievementsData.greenColorTraget)
			{
				player.greenAchieve = true;
				inStack.Push(greenAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();

				CheckBlackBoxLevelAchievement();
			}
		}
	}

	public void CheckYellowColorAchievement()
	{
		if (!player.yellowAchieve)
		{
#if UNITY_ANDROID
			if (logedIn) Social.ReportProgress(GPGSIds.achievement_multiplier_eater, Mathf.Min(player.yellowCount, AchievementsData.yellowColorTraget) / AchievementsData.yellowColorTraget * 100, (bool success) => { });
#endif
			if (player.yellowCount >= AchievementsData.yellowColorTraget)
			{
				player.yellowAchieve = true;
				inStack.Push(yellowAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();

				CheckBlackBoxLevelAchievement();
			}
		}
	}

	public void CheckRedColorAchievement()
	{
		if (!player.redAchieve)
		{
#if UNITY_ANDROID
			if (logedIn) Social.ReportProgress(GPGSIds.achievement_cubiline_player, Mathf.Min(player.arcadeGamesPlayed + player.coopGamesPlayed, AchievementsData.redColorTraget) / AchievementsData.redColorTraget * 100, (bool success) => { });
#endif
			if (player.arcadeGamesPlayed + player.coopGamesPlayed >= AchievementsData.redColorTraget)
			{
				player.redAchieve = true;
				inStack.Push(redAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();

				CheckBlackBoxLevelAchievement();
			}
		}
	}

	public void CheckPurpleColorAchievement()
	{
		if (!player.purpleAchieve)
		{
#if UNITY_ANDROID
			if (logedIn) Social.ReportProgress(GPGSIds.achievement_magnet_eater, Mathf.Min(player.purpleCount, AchievementsData.purpleColorTraget) / AchievementsData.purpleColorTraget * 100, (bool success) => { });
#endif
			if (player.purpleCount >= AchievementsData.purpleColorTraget)
			{
				player.purpleAchieve = true;
				inStack.Push(purpleAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();

				CheckBlackBoxLevelAchievement();
			}
		}
	}

	public void CheckScoreColorAchievement()
	{
		if (!player.byScoreColorAchieve)
		{
			if (player.lastArcadeScore >= AchievementsData.scoreColorTarget || player.lastCoopScore >= AchievementsData.scoreColorTarget)
			{
#if UNITY_ANDROID
				if (logedIn) Social.ReportProgress(GPGSIds.achievement_big_player, 100.0f, (bool success) => { });
#endif
				player.byScoreColorAchieve = true;
				inStack.Push(scoreColorAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();

				CheckBlackBoxLevelAchievement();
			}
		}
	}

	public void CheckLengthColorAchievement()
	{
		if (!player.byLengthColorAchieve)
		{
			if (player.totalArcadeLength >= AchievementsData.lengthColorTraget || player.totalCoopLength >= AchievementsData.lengthColorTraget)
			{
#if UNITY_ANDROID
				if (logedIn) Social.ReportProgress(GPGSIds.achievement_anaconda, 100.0f, (bool success) => { });
#endif
				player.byLengthColorAchieve = true;
				inStack.Push(lengthColorAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();

				CheckBlackBoxLevelAchievement();
			}
		}
	}

	public void CheckFillColorAchievement()
	{
		if (!player.byFillColorAchieve)
		{
			if (player.lastArcadeLength >= AchievementsData.fillColorTarget || player.lastCoopLength >= AchievementsData.fillColorTarget)
			{
#if UNITY_ANDROID
				if (logedIn) Social.ReportProgress(GPGSIds.achievement_indianapolis_500, 100.0f, (bool success) => { });
#endif
				player.byFillColorAchieve = true;
				inStack.Push(fillColorAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();

				CheckBlackBoxLevelAchievement();
			}
		}
	}

	public void CheckBlackLevelAchievement()
	{
		if (!player.blackCubeAchieve)
		{
#if UNITY_ANDROID
			if (logedIn) Social.ReportProgress(GPGSIds.achievement_gray_eater, Mathf.Min(player.grayCount, AchievementsData.blackCubeTarget) / AchievementsData.blackCubeTarget * 100, (bool success) => { });
#endif
			if (player.grayCount >= AchievementsData.blackCubeTarget)
			{
				player.blackCubeAchieve = true;
				inStack.Push(blackLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();
			}
		}
	}

	public void CheckDiceLevelAchievement()
	{
		if (!player.diceAchieve)
		{
			if (AchievementsData.diceCheck1 && AchievementsData.diceCheck2 && AchievementsData.diceCheck3)
			{
#if UNITY_ANDROID
				if (logedIn) Social.ReportProgress(GPGSIds.achievement_lucky_player, 100.0f, (bool success) => { });
#endif
				player.diceAchieve = true;
				inStack.Push(diceLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();
			}
		}
	}

	public void CheckBlackDiceLevelAchievement()
	{
		if (!player.blackDiceAchieve)
		{
			if (AchievementsData.blackdiceCheck1 && AchievementsData.blackdiceCheck2 && AchievementsData.blackdiceCheck3)
			{
#if UNITY_ANDROID
				if (logedIn) Social.ReportProgress(GPGSIds.achievement_luckiest_player, 100.0f, (bool success) => { });
#endif
				player.blackDiceAchieve = true;
				inStack.Push(blackDiceLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();
			}
		}
	}

	public void CheckToyLevelAchievement()
	{
		if (!player.toyAchieve)
		{
			if ((player.lastArcadeScore >= 100 && player.lastArcadeLength == 3) || (player.lastCoopScore >= 100 && player.lastCoopLength == 6))
			{
#if UNITY_ANDROID
				if (logedIn) Social.ReportProgress(GPGSIds.achievement_logical_player, 100.0f, (bool success) => { });
#endif
				player.toyAchieve = true;
				inStack.Push(toyLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();
			}
		}
	}

	public void CheckBlackToyLevelAchievement()
	{
		if (!player.blackToyAchieve)
		{
			if ((player.lastArcadeScore >= 200 && player.lastArcadeLength == 3) || (player.lastCoopScore >= 200 && player.lastCoopLength == 6))
			{
#if UNITY_ANDROID
				if (logedIn) Social.ReportProgress(GPGSIds.achievement_strategist_player, 100.0f, (bool success) => { });
#endif
				player.blackToyAchieve = true;
				inStack.Push(blackToyLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();
			}
		}
	}

	public void CheckBoxLevelAchievement()
	{
		if (!player.paperAchieve)
		{
#if UNITY_ANDROID
			if (logedIn) Social.ReportProgress(GPGSIds.achievement_open_the_box, 100.0f, (bool success) => { });
#endif
			player.paperAchieve = true;
			inStack.Push(boxLevelAchivementPortalPrefab);
			if (waithStack.Count == 0) ShowPortal();
			SavePlayer();
		}
	}

	public void CheckBlackBoxLevelAchievement()
	{
		if (!player.blackPaperAchieve)
		{
			if(player.blueAchieve && player.orangeAchieve && player.greenAchieve && player.yellowAchieve && player.redAchieve && player.purpleAchieve && player.byScoreColorAchieve && player.byLengthColorAchieve && player.byFillColorAchieve)
			{
#if UNITY_ANDROID
				if (logedIn) Social.ReportProgress(GPGSIds.achievement_colorfull, 100.0f, (bool success) => { });
#endif
				player.blackPaperAchieve = true;
				inStack.Push(blackBoxLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();
			}
		}
	}

	public void CheckKnowledgeLevelAchievement()
	{
		if (!player.incognitAchieve)
		{
#if UNITY_ANDROID
			if (logedIn) Social.ReportProgress(GPGSIds.achievement_wiseman, 100.0f, (bool success) => { });
#endif
			player.incognitAchieve = true;
			inStack.Push(knowledgeLevelAchivementPortalPrefab);
			if (waithStack.Count == 0) ShowPortal();
			SavePlayer();
		}
	}

	public void CheckBlackKnowledgeLevelAchievement()
	{
		if (!player.blackIncognitAchieve)
		{
			if(player.arcadeTimePlayed + player.coopTimePlayed >= AchievementsData.blackIncognitTarget)
			{
#if UNITY_ANDROID
				if (logedIn) Social.ReportProgress(GPGSIds.achievement_cubiline_fan, 100.0f, (bool success) => { });
#endif
				player.blackIncognitAchieve = true;
				inStack.Push(blackKnowledgeLevelAchivementPortalPrefab);
				if (waithStack.Count == 0) ShowPortal();
				SavePlayer();
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
