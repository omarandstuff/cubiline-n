using UnityEngine;

public class CubilineMultyPlayerController : MonoBehaviour
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
	public CubilineUIController uiController;
	public Follow followTarget1;
	public Follow followTarget2;
	public Transform outTarget1;
	public Transform outTarget2;

	public EaseImageOpasity division;

	//////////////////////////////////////////////////////////////
	///////////////////////// PARAMETERS /////////////////////////
	//////////////////////////////////////////////////////////////

	public uint arenaSize = 5;
	public float speed = 5;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	private enum STATUS { PLAYING, GIONG_OUT, SHOW_SCORE }
	private STATUS status;

	private PauseMenuController pauseMenu;
	private bool menuKey;

	private Touch[] touchAtBegin = new Touch[10];
	private Resolution lastResolution;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		DoScreenOrientation();
		Reset();
	}

	void Update()
	{
		if (status != STATUS.PLAYING)
		{
			if ((followTarget1.transform.position - outTarget1.position).magnitude < arenaSize)
			{
				if (status == STATUS.GIONG_OUT)
				{
					Application.LoadLevel(0);
				}
				else if (status == STATUS.SHOW_SCORE)
				{
					Application.LoadLevel(2);
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
				uiController.GoOut();
				GoOut();
			}
		}

		if (player1.status == CubilinePlayerController.STATUS.FINISH)
		{
			status = STATUS.SHOW_SCORE;
			uiController.GoOut();
			GoOut();
		}
		else if (player2.status == CubilinePlayerController.STATUS.FINISH)
		{
			status = STATUS.SHOW_SCORE;
			uiController.GoOut();
			GoOut();
		}
	}

	void OnGUI()
	{
		Event e = Event.current;
		if (e.type == EventType.KeyDown)
		{
			if (e.keyCode == KeyCode.A)
				player1.AddTurn(CubilinePlayerController.TURN.LEFT);
			else if (e.keyCode == KeyCode.D)
				player1.AddTurn(CubilinePlayerController.TURN.RIGHT);
			else if (e.keyCode == KeyCode.W)
				player1.AddTurn(CubilinePlayerController.TURN.UP);
			else if (e.keyCode == KeyCode.S)
				player1.AddTurn(CubilinePlayerController.TURN.DOWN);
			else if (e.keyCode == KeyCode.LeftArrow)
				player2.AddTurn(CubilinePlayerController.TURN.LEFT);
			else if (e.keyCode == KeyCode.RightArrow)
				player2.AddTurn(CubilinePlayerController.TURN.RIGHT);
			else if (e.keyCode == KeyCode.UpArrow)
				player2.AddTurn(CubilinePlayerController.TURN.UP);
			else if (e.keyCode == KeyCode.DownArrow)
				player2.AddTurn(CubilinePlayerController.TURN.DOWN);
			else if (e.keyCode == KeyCode.Escape && !menuKey) // Menu
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
		}
		else
		{
			followCamera1.GetComponent<Camera>().rect = new Rect(0, 0, 1.0f, 0.5f);
			followCamera2.GetComponent<Camera>().rect = new Rect(0, 0.5f, 1.0f, 0.5f);
		}

		lastResolution = resolution;
	}

	void DoTouch()
	{
		if (SystemInfo.deviceType != DeviceType.Handheld) return;
		for(int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);

			if (touch.phase == TouchPhase.Began)
			{
				touchAtBegin[touch.fingerId] = touch;
			}
			else if (touch.phase == TouchPhase.Ended)
			{
				Vector2 delta = touch.position - touchAtBegin[touch.fingerId].position;

				CubilinePlayerController player = player1;

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
					if (delta.x > 0)
						player.AddTurn(CubilinePlayerController.TURN.RIGHT);
					else
						player.AddTurn(CubilinePlayerController.TURN.LEFT);
				}
				else
				{
					if (delta.y > 0)
						player.AddTurn(CubilinePlayerController.TURN.UP);
					else
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
		// Keep size odd
		if (arenaSize < 5)
			arenaSize = 5;
		if (arenaSize % 2 == 0)
			arenaSize += 1;

		CubilineScoreController.currentScore = 0;
		CubilineScoreController.currentNumberOfPlayers = 2;

		arenaController.Reset(arenaSize);
		followCamera1.transform.localPosition = new Vector3(0.0f, 0.0f, -arenaSize * 2.0f);
		followCamera1.transform.localPosition = new Vector3(0.0f, 0.0f, -arenaSize * 2.0f);

		player1.Reset(arenaSize);
		player1.speed = speed;
		
		player2.Reset(arenaSize);
		player2.speed = speed;

		followTarget1.transform.position = new Vector3(-arenaSize * 2, 0, -arenaSize / 2);
		followTarget2.transform.position = new Vector3(-arenaSize * 2, 0, -arenaSize / 2);
	}

	void GoOut()
	{
		division.easeFace = EaseFloat.EASE_FACE.OUT;

		outTarget1.transform.position = followCamera1.transform.localPosition + (followCamera1.transform.localPosition - followCamera1.target.transform.localPosition).normalized * arenaSize;
		followTarget1.target = outTarget1;
		followTarget1.followSmoothTime = 0.8f;

		outTarget2.transform.position = followCamera2.transform.localPosition + (followCamera2.transform.localPosition - followCamera2.target.transform.localPosition).normalized * arenaSize;
		followTarget2.target = outTarget2;
		followTarget2.followSmoothTime = 0.8f;
	}
}
