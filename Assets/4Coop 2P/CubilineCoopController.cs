using UnityEngine;

public class CubilineCoopController : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public CubilineArenaController arenaController;
	public CubilinePlayerController player1;
	public CubilinePlayerController player2;
	public OrbitAndLook followCamera1;
	public OrbitAndLook followCamera2;
	public PauseMenuController pauseMenuBase;
	public CubilineCoopUIController coopUIController;
	public Follow followTarget1;
	public Follow followTarget2;
	public Transform outTarget1;
	public Transform outTarget2;
	public Material player1Material;
	public Material player2Material;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	private enum STATUS { PLAYING, GIONG_OUT, SHOW_SCORE }
	private STATUS status;

	private PauseMenuController pauseMenu;
	private bool menuKey;

	private Touch[] touchAtBegin = new Touch[10];
	private Resolution lastResolution;

	private float timeOfGame;

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
		player1Material.color = CubilineApplication.singleton.settings.securePlayer1Color;
		player2Material.color = CubilineApplication.singleton.settings.securePlayer2Color;

		DoScreenOrientation();
		Reset();
	}

	void Update()
	{
		if (status != STATUS.PLAYING)
		{
			if ((followTarget1.transform.position - outTarget1.position).magnitude < CubilineApplication.singleton.settings.coopCubeSize)
			{
				// Player game inf
				CubilineApplication.singleton.player.coopTimePlayed += Time.time - timeOfGame;
				CubilineApplication.singleton.SavePlayer();

				CubilineApplication.singleton.CheckBlackKnowledgeLevelAchievement();
				CubilineApplication.singleton.SaveAchievements();

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

		DoScreenOrientation();

		DoTouch();
		player1.Go();
		player2.Go();
		arenaController.ManageArena();

		if (pauseMenu != null)
		{
			if (pauseMenu.status == PauseMenuController.STATUS.CONTINUE)
			{
				player1.status = CubilinePlayerController.STATUS.PLAYING;
				player2.status = CubilinePlayerController.STATUS.PLAYING;
				pauseMenu.status = PauseMenuController.STATUS.CONTINUE;
				arenaController.status = CubilinePlayerController.STATUS.PLAYING;
				Destroy(pauseMenu.gameObject, 0.5f);
				pauseMenu = null;
			}
			else if (pauseMenu.status == PauseMenuController.STATUS.MAIN_MENU)
			{
				status = STATUS.GIONG_OUT;
				coopUIController.GoOut();
				GoOut();
			}
		}

		if (player1.status == CubilinePlayerController.STATUS.FINISH || player2.status == CubilinePlayerController.STATUS.FINISH)
		{
			status = STATUS.SHOW_SCORE;
			coopUIController.GoOut();
			Time.timeScale = 0.2f;

			if (CubilineMusicPlayer.singleton != null) CubilineMusicPlayer.singleton.ArcadeFinishWave();
			if (CubilineMusicPlayer.singleton != null) CubilineMusicPlayer.singleton.Stop();

			// Player game inf
			CubilineApplication.singleton.player.coopGamesPlayed++;
			CubilineApplication.singleton.player.coopTimePlayed += Time.time - timeOfGame;
			CubilineApplication.singleton.player.lastCoopTime = Time.time - timeOfGame;
			CubilineApplication.singleton.SavePlayer();

			CubilineApplication.singleton.CheckRedColorAchievement();
			CubilineApplication.singleton.CheckBlackKnowledgeLevelAchievement();
			CubilineApplication.singleton.SaveAchievements();

			CubilineApplication.singleton.lastComment = "coop_2p_scene";

			GoOut();
		}
	}

	void OnGUI()
	{
		Event e = Event.current;
		if (e.type == EventType.KeyDown)
		{
			if(pauseMenu == null)
			{
				if (e.keyCode == KeyCode.A)
					player1.AddTurn(CubilinePlayerController.TURN.LEFT);
				else if (e.keyCode == KeyCode.D)
					player1.AddTurn(CubilinePlayerController.TURN.RIGHT);
				else if (e.keyCode == KeyCode.W)
					player1.AddTurn(CubilinePlayerController.TURN.UP);
				else if (e.keyCode == KeyCode.S)
					player1.AddTurn(CubilinePlayerController.TURN.DOWN);
				else if (e.keyCode == KeyCode.Space)
					player1.speed = CubilineApplication.singleton.settings.coopLineSpeed * 2.0f;
				else if (e.keyCode == KeyCode.LeftArrow)
					player2.AddTurn(CubilinePlayerController.TURN.LEFT);
				else if (e.keyCode == KeyCode.RightArrow)
					player2.AddTurn(CubilinePlayerController.TURN.RIGHT);
				else if (e.keyCode == KeyCode.UpArrow)
					player2.AddTurn(CubilinePlayerController.TURN.UP);
				else if (e.keyCode == KeyCode.DownArrow)
					player2.AddTurn(CubilinePlayerController.TURN.DOWN);
				else if (e.keyCode == KeyCode.RightControl)
					player2.speed = CubilineApplication.singleton.settings.coopLineSpeed * 2.0f;
			}
			if (e.keyCode == KeyCode.Escape && !menuKey) // Menu
			{
				menuKey = true;
				if (pauseMenu != null)
				{
					player1.status = CubilinePlayerController.STATUS.PLAYING;
					player2.status = CubilinePlayerController.STATUS.PLAYING;
					pauseMenu.status = PauseMenuController.STATUS.CONTINUE;
					arenaController.status = CubilinePlayerController.STATUS.PLAYING;
					Destroy(pauseMenu.gameObject, 0.5f);
					pauseMenu = null;
				}
				else
				{
					player1.status = CubilinePlayerController.STATUS.PAUSED;
					player2.status = CubilinePlayerController.STATUS.PAUSED;
					arenaController.status = CubilinePlayerController.STATUS.PAUSED;
					pauseMenu = Instantiate(pauseMenuBase);
				}
			}
		}
		else if (e.type == EventType.keyUp)
		{
			if (e.keyCode == KeyCode.Space)
				player1.speed = CubilineApplication.singleton.settings.coopLineSpeed;
			if (e.keyCode == KeyCode.RightControl)
				player2.speed = CubilineApplication.singleton.settings.coopLineSpeed;
			else if (e.keyCode == KeyCode.Escape)
				menuKey = false;
		}
	}

	void DoScreenOrientation()
	{
		Resolution resolution = Screen.currentResolution;
		if (resolution.width == lastResolution.width) return;

		if(resolution.width > resolution.height)
		{
			followCamera1.GetComponent<Camera>().rect = new Rect(0, 0, 0.5f, 1.0f);
			followCamera2.GetComponent<Camera>().rect = new Rect(0.5f, 0, 0.5f, 1.0f);
			coopUIController.enableVerticalDivision = true;
			coopUIController.enableHorizontalDivision = false;
		}
		else
		{
			followCamera1.GetComponent<Camera>().rect = new Rect(0, 0, 1.0f, 0.5f);
			followCamera2.GetComponent<Camera>().rect = new Rect(0, 0.5f, 1.0f, 0.5f);
			coopUIController.enableHorizontalDivision = true;
			coopUIController.enableVerticalDivision = false;
		}

		lastResolution = resolution;
	}

	void DoTouch()
	{
		if (SystemInfo.deviceType != DeviceType.Handheld || pauseMenu != null) return;

		player1.speed = CubilineApplication.singleton.settings.coopLineSpeed;
		player2.speed = CubilineApplication.singleton.settings.coopLineSpeed;

		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);

			if (touch.phase == TouchPhase.Stationary)
			{
				CubilinePlayerController player = null;

				if (lastResolution.width > lastResolution.height)
				{
					if (touch.position.x < lastResolution.width / 2)
						player = player1;
					else
						player = player2;
				}
				else
				{
					if (touch.position.y < lastResolution.height / 2)
						player = player1;
					else
						player = player2;
				}

				player.speed = CubilineApplication.singleton.settings.coopLineSpeed * 2;
			}

			if (touch.phase == TouchPhase.Began)
			{
				touchAtBegin[touch.fingerId] = touch;
			}
			else if (touch.phase == TouchPhase.Ended)
			{
				Vector2 delta = touch.position - touchAtBegin[touch.fingerId].position;

				CubilinePlayerController player = null;

				if(lastResolution.width > lastResolution.height)
				{
					if (touchAtBegin[touch.fingerId].position.x < lastResolution.width / 2)
						player = player1;
					else
						player = player2;
				}
				else
				{
					if (touchAtBegin[touch.fingerId].position.y < lastResolution.height / 2)
						player = player1;
					else
						player = player2;
				}

				if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
				{
					if (delta.x > 5)
						player.AddTurn(CubilinePlayerController.TURN.RIGHT);
					else if (delta.x < -5)
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

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// SETUP ////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void Reset()
	{
		arenaController.Reset(CubilineApplication.singleton.settings.coopCubeSize);
		followCamera1.transform.localPosition = new Vector3(0.0f, 0.0f, -CubilineApplication.singleton.settings.coopCubeSize * 2.0f);
		followCamera1.transform.localPosition = new Vector3(0.0f, 0.0f, -CubilineApplication.singleton.settings.coopCubeSize * 2.0f);

		player1.Reset(CubilineApplication.singleton.settings.coopCubeSize);
		player1.speed = CubilineApplication.singleton.settings.coopLineSpeed;
		
		player2.Reset(CubilineApplication.singleton.settings.coopCubeSize);
		player2.speed = CubilineApplication.singleton.settings.coopLineSpeed;

		player1.hardMove = CubilineApplication.singleton.settings.coopHardMove;
		player2.hardMove = CubilineApplication.singleton.settings.coopHardMove;

		followTarget1.transform.position = new Vector3(-CubilineApplication.singleton.settings.coopCubeSize * 2, 0, -CubilineApplication.singleton.settings.coopCubeSize / 2);
		followTarget2.transform.position = new Vector3(-CubilineApplication.singleton.settings.coopCubeSize * 2, 0, -CubilineApplication.singleton.settings.coopCubeSize / 2);

		DoScreenOrientation();
		coopUIController.timeToApear = 1.0f;
	}

	void GoOut()
	{
		outTarget1.transform.position = followCamera1.transform.localPosition + (followCamera1.transform.localPosition - followCamera1.target.transform.localPosition).normalized * CubilineApplication.singleton.settings.coopCubeSize;
		followTarget1.target = outTarget1;
		followTarget1.followSmoothTime = 0.8f;

		outTarget2.transform.position = followCamera2.transform.localPosition + (followCamera2.transform.localPosition - followCamera2.target.transform.localPosition).normalized * CubilineApplication.singleton.settings.coopCubeSize;
		followTarget2.target = outTarget2;
		followTarget2.followSmoothTime = 0.8f;
	}
}
