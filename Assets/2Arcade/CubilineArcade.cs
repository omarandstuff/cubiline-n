using UnityEngine;
//using UnityEngine.SceneManagement;

public class CubilineArcade : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public CubilineArenaController arenaController;
	public CubilinePlayerController player;
	public OrbitAndLook followCamera;
	public PauseMenuController pauseMenuBase;
	public CubilineUIController uiController;
	public Follow followTarget;
	public Transform outTarget;
	public Material player1Material;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	private enum STATUS { PLAYING, GIONG_OUT, SHOW_SCORE }
	private STATUS status;

	private PauseMenuController pauseMenu;
	private bool menuKey;

	private Touch touchAtBegin;

	private float timeOfGame;

	private uint arenaSize;

	public bool pauseAction;
	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		// Used for register de time the palyer has playing.
		timeOfGame = Time.time;

		// Audio directive.
		CubilineMusicPlayer.inMenu = false;

		// Ensecure player color
		player1Material.color = CubilineApplication.singleton.player.securePlayer1Color;

		Reset();
	}

	void Update()
	{
		if(status != STATUS.PLAYING)
		{
			if((followTarget.transform.position - outTarget.position).magnitude < CubilineApplication.singleton.player.arcadeCubeSize)
			{
				// Player game inf
				CubilineApplication.singleton.player.arcadeTimePlayed += Time.time - timeOfGame;
				CubilineApplication.singleton.CheckBlackKnowledgeLevelAchievement();
				CubilineApplication.singleton.SavePlayer();

				if (status == STATUS.GIONG_OUT)
				{
					Application.LoadLevel("main_menu_scene");
				}
				else if (status == STATUS.SHOW_SCORE)
				{
					Application.LoadLevel("show_score_scene");
				}
			}
			return;
		}

		DoTouch();
		player.Go();
		arenaController.ManageArena();

		if(pauseMenu != null)
		{
			if(pauseMenu.status == PauseMenuController.STATUS.CONTINUE)
			{
				player.status = CubilinePlayerController.STATUS.PLAYING;
				pauseMenu.status = PauseMenuController.STATUS.CONTINUE;
				arenaController.status = CubilinePlayerController.STATUS.PLAYING;
				Destroy(pauseMenu.gameObject, 0.5f);
				pauseMenu = null;
			}
			else if(pauseMenu.status == PauseMenuController.STATUS.MAIN_MENU)
			{
				status = STATUS.GIONG_OUT;
				uiController.GoOut();
				CubilineApplication.singleton.SavePlayer();
				GoOut();
			}
		}

		if (player.status == CubilinePlayerController.STATUS.FINISH)
		{
			status = STATUS.SHOW_SCORE;
			uiController.GoOut();
			Time.timeScale = 0.2f;

			if (CubilineMusicPlayer.singleton != null) CubilineMusicPlayer.singleton.ArcadeFinishWave();
			if (CubilineMusicPlayer.singleton != null) CubilineMusicPlayer.singleton.Stop();

			// Player game inf
			CubilineApplication.singleton.player.arcadeGamesPlayed++;
			CubilineApplication.singleton.player.arcadeTimePlayed += Time.time - timeOfGame;
			CubilineApplication.singleton.player.lastArcadeTime = Time.time - timeOfGame;

			CubilineApplication.singleton.CheckRedColorAchievement();
			CubilineApplication.singleton.CheckBlackKnowledgeLevelAchievement();

			CubilineApplication.singleton.SavePlayer();

			CubilineApplication.singleton.lastComment = "arcade_scene";

			GoOut();
		}
	}

	void OnGUI()
	{
		Event e = Event.current;
		if (e.type == EventType.KeyDown || pauseAction)
		{
			if(pauseMenu == null)
			{
				if (e.keyCode == KeyCode.A || e.keyCode == KeyCode.LeftArrow)
					player.AddTurn(CubilinePlayerController.TURN.LEFT);
				else if (e.keyCode == KeyCode.D || e.keyCode == KeyCode.RightArrow)
					player.AddTurn(CubilinePlayerController.TURN.RIGHT);
				else if (e.keyCode == KeyCode.W || e.keyCode == KeyCode.UpArrow)
					player.AddTurn(CubilinePlayerController.TURN.UP);
				else if (e.keyCode == KeyCode.S || e.keyCode == KeyCode.DownArrow)
					player.AddTurn(CubilinePlayerController.TURN.DOWN);
				else if (e.keyCode == KeyCode.LeftArrow)
					player.AddTurn(CubilinePlayerController.TURN.LEFT);
				else if (e.keyCode == KeyCode.RightArrow)
					player.AddTurn(CubilinePlayerController.TURN.RIGHT);
				else if (e.keyCode == KeyCode.UpArrow)
					player.AddTurn(CubilinePlayerController.TURN.UP);
				else if (e.keyCode == KeyCode.DownArrow)
					player.AddTurn(CubilinePlayerController.TURN.DOWN);
				else if (e.keyCode == KeyCode.Space)
					player.speed = CubilineApplication.singleton.player.arcadeLineSpeed * 2.0f;
			}
			if ((e.keyCode == KeyCode.Escape && !menuKey) || pauseAction) // Menu
			{
				menuKey = true;
				pauseAction= false;

				if (pauseMenu != null)
				{
					player.status = CubilinePlayerController.STATUS.PLAYING;
					pauseMenu.status = PauseMenuController.STATUS.CONTINUE;
					arenaController.status = CubilinePlayerController.STATUS.PLAYING;
					Destroy(pauseMenu.gameObject, 0.5f);
					pauseMenu = null;
				}
				else
				{
					player.status = CubilinePlayerController.STATUS.PAUSED;
					arenaController.status = CubilinePlayerController.STATUS.PAUSED;
					pauseMenu = Instantiate(pauseMenuBase);
				}
			}
		}
		else if (e.type == EventType.keyUp)
		{
			if (e.keyCode == KeyCode.Space)
				player.speed = CubilineApplication.singleton.player.arcadeLineSpeed;
			else if (e.keyCode == KeyCode.Escape)
				menuKey = false;
		}
	}

	void DoTouch()
	{
		if (SystemInfo.deviceType != DeviceType.Handheld) return;
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);

			if (touch.phase == TouchPhase.Stationary)
			{
				player.speed = CubilineApplication.singleton.player.arcadeLineSpeed * 2.0f;
			}
			else
				player.speed = CubilineApplication.singleton.player.arcadeLineSpeed;

			if (touch.phase == TouchPhase.Began)
			{
				touchAtBegin = touch;
			}
			else if (touch.phase == TouchPhase.Ended)
			{
				Vector2 delta = touch.position - touchAtBegin.position;

				if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
				{
					if (delta.x > 5)
						player.AddTurn(CubilinePlayerController.TURN.RIGHT);
					else if(delta.x < -5)
						player.AddTurn(CubilinePlayerController.TURN.LEFT);
				}
				else
				{
					if (delta.y > 5)
						player.AddTurn(CubilinePlayerController.TURN.UP);
					else if (delta.y < -5)
						player.AddTurn(CubilinePlayerController.TURN.DOWN);
				}
			}
		}
	}

	public void pauseActionButton()
	{
		pauseAction = true;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// SETUP ////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void Reset()
	{
		arenaSize = CubilineApplication.singleton.player.arcadeCubeSize + (CubilineApplication.singleton.player.arcadeCubeSize % 2 == 0 ? 1u : 0u);
		arenaController.Reset(arenaSize);
		followCamera.transform.localPosition = new Vector3(0.0f, 0.0f, -arenaSize * 2.0f);

		// Because othrt things.
		CubilineApplication.singleton.player.lastCoopScore = 0;
		CubilineApplication.singleton.player.lastCoopLength = 0;

		player.Reset(arenaSize);
		player.speed = CubilineApplication.singleton.player.arcadeLineSpeed;
		player.hardMove = CubilineApplication.singleton.player.arcadeHardMove;

		followCamera.GetComponent<UnityStandardAssets.ImageEffects.DepthOfField>().enabled = CubilineApplication.singleton.settings.depthOfField;
		followCamera.GetComponent<UnityStandardAssets.ImageEffects.ScreenSpaceAmbientOcclusion>().enabled = CubilineApplication.singleton.settings.ambientOcclusion;

		followTarget.transform.position = new Vector3(-arenaSize * 2, 0, -arenaSize / 2);
	}

	void GoOut()
	{
		outTarget.transform.position = followCamera.transform.localPosition + (followCamera.transform.localPosition - followCamera.target.transform.localPosition).normalized * arenaSize;
		followTarget.target = outTarget;
		followTarget.followSmoothTime = 0.8f;
	}
}
