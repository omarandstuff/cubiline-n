using UnityEngine;

public class CubilineSinglePlayer : MonoBehaviour
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

	private Touch touchAtBegin;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		Reset();
	}

	void Update()
	{
		if(status != STATUS.PLAYING)
		{
			if((followTarget.transform.position - outTarget.position).magnitude < arenaSize)
			{
				if(status == STATUS.GIONG_OUT)
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
				GoOut();
			}
		}

		if (player.status == CubilinePlayerController.STATUS.FINISH)
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
			if (e.keyCode == KeyCode.A || e.keyCode == KeyCode.LeftArrow)
				player.AddTurn(CubilinePlayerController.TURN.LEFT);
			else if (e.keyCode == KeyCode.D || e.keyCode == KeyCode.RightArrow)
				player.AddTurn(CubilinePlayerController.TURN.RIGHT);
			else if (e.keyCode == KeyCode.W || e.keyCode == KeyCode.UpArrow)
				player.AddTurn(CubilinePlayerController.TURN.UP);
			else if (e.keyCode == KeyCode.S || e.keyCode == KeyCode.DownArrow)
				player.AddTurn(CubilinePlayerController.TURN.DOWN);
			else if (e.keyCode == KeyCode.Escape && !menuKey) // Menu
			{
				menuKey = true;

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
			if (e.keyCode == KeyCode.Escape)
				menuKey = false;
		}
	}

	void DoTouch()
	{
		if (SystemInfo.deviceType != DeviceType.Handheld) return;
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);

			if (touch.phase == TouchPhase.Began)
			{
				touchAtBegin = touch;
			}
			else if (touch.phase == TouchPhase.Ended)
			{
				Vector2 delta = touch.position - touchAtBegin.position;

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
		CubilineScoreController.currentNumberOfPlayers = 1;

		arenaController.Reset(arenaSize);
		followCamera.transform.localPosition = new Vector3(0.0f, 0.0f, -arenaSize * 2.0f);

		player.Reset(arenaSize);
		player.speed = speed;

		followTarget.transform.position = new Vector3(-arenaSize * 2, 0, -arenaSize / 2);
	}

	void GoOut()
	{
		outTarget.transform.position = followCamera.transform.localPosition + (followCamera.transform.localPosition - followCamera.target.transform.localPosition).normalized * arenaSize;
		followTarget.target = outTarget;
		followTarget.followSmoothTime = 0.8f;
	}
}
