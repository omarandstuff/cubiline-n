using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class CubilineSinglePlayer : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public GameObject gameInstance;
	public CubilineArenaController arenaController;
	public CubilinePlayerController player;
	public OrbitAndLook followCamera;
	public PauseMenuController pauseMenuBase;
	public EaseTransform outTarget;
	public EaseTransform score;
	public EaseTransform bestScore;

	//////////////////////////////////////////////////////////////
	///////////////////////// PARAMETERS /////////////////////////
	//////////////////////////////////////////////////////////////

	public uint arenaSize = 5;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	private PauseMenuController pauseMenu;
	private bool menuKey;

	private bool getOut;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Awake()
	{
		Reset();
	}

	void Update()
	{
		if(getOut)
		{
			if(outTarget.transform.position == outTarget.inPosition)
			{
				Application.LoadLevelAsync(0);
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
				
				GoOut();
			}
		}

		if (player.status == CubilinePlayerController.STATUS.FINISH)
		{

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

		arenaController.Reset(arenaSize);
		followCamera.transform.localPosition = new Vector3(0.0f, 0.0f, -arenaSize * 2.0f);

		player.Reset(arenaSize);
	}

	void GoOut()
	{
		outTarget.outPosition = followCamera.target.transform.localPosition;
		outTarget.inPosition = followCamera.transform.localPosition + (followCamera.transform.localPosition - followCamera.target.transform.localPosition).normalized * 5;
		outTarget.easePosition = true;
		outTarget.Reset();
		followCamera.target = outTarget.transform;
		score.inScale = new Vector3(1.3f, 1.3f, 1.3f);
		bestScore.inScale = new Vector3(1.3f, 1.3f, 1.3f);
		score.GetComponent<EaseOpasity>().inOpasity = 0.0f;
		getOut = true;
	}
}
