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

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		Screen.orientation = ScreenOrientation.Landscape;
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
			if (menuKey) return;
			menuKey = true;
			if (e.keyCode == KeyCode.Escape)
			{
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
		outTarget1.transform.position = followCamera1.transform.localPosition + (followCamera1.transform.localPosition - followCamera1.target.transform.localPosition).normalized * arenaSize;
		followTarget1.target = outTarget1;
		followTarget1.followSmoothTime = 0.8f;

		outTarget2.transform.position = followCamera2.transform.localPosition + (followCamera2.transform.localPosition - followCamera2.target.transform.localPosition).normalized * arenaSize;
		followTarget2.target = outTarget2;
		followTarget2.followSmoothTime = 0.8f;

		Screen.orientation = ScreenOrientation.AutoRotation;
	}
}
