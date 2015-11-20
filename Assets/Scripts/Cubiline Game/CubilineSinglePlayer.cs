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
			if (menuKey) return;
			menuKey = true;
			if (e.keyCode == KeyCode.Escape)
			{
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
