using System.Collections;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class CubilineTutorialFase1 : MonoBehaviour
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

	public Transform canvasInf;
	public GameObject welcomeInfPrefab;
	public GameObject keyboardInfPrefab;
	public GameObject keyboardSpeedInfPrefab;
	public GameObject touchInfPrefab;
	public GameObject touchSpeedInfPrefab;

	public GameObject blueBlockPrefab;

	public GameObject bigBlueTarget;
	public GameObject bigBlueInfoPrefab;

	public GameObject greenTarget;
	public GameObject greenBlockInfPrefab;

	public GameObject bigGreenTarget;
	public GameObject bigGreenBlockInfPrefab;

	public GameObject orangeTarget;
	public GameObject orangeBlockInfPrefab;

	public GameObject bigOrangeTarget;
	public GameObject bigOrangeBlockInfPrefab;

	public GameObject grayTarget;
	public GameObject grayBlockInfPrefab;

	public GameObject bigGrayTarget;
	public GameObject bigGrayBlockInfPrefab;

	public GameObject x2Target;
	public GameObject x4Target;
	public GameObject x2BlockInfPrefab;
	public GameObject x4BlockInfPrefab;
	public GameObject combinedBlockInfPrefab;

	public GameObject purpleTarget;
	public GameObject purpleBlockInfPrefab;

	public GameObject readyBlockInfPrefab;


	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	private enum STATUS { PLAYING, GIONG_OUT, SHOW_SCORE }
	private STATUS status;

	private PauseMenuController pauseMenu;
	private bool menuKey;

	private Touch touchAtBegin;

	private float timeOfGame;

	private GameObject currentInf;

	private bool welcomeDone;
	private bool moveDone;
	private bool moveFinish;
	private bool speedDone;
	private bool blueDone;
	private bool bigBlueDone;
	private bool greenDone;
	private bool bigGreenDone;
	private bool orangeDone;
	private bool bigOrangeDone;
	private bool grayDone;
	private bool bigGrayDone;
	private bool x2Done;
	private bool x4Done;
	private bool combinedDone;
	private bool purpleDone;
	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	IEnumerator Start()
	{
		// Used for register de time the palyer has playing.
		timeOfGame = Time.time;

		CubilineApplication.singleton.settings.notFirstTime = true;
		CubilineApplication.singleton.SaveSettings();

		// Audio directive.
		CubilineMusicPlayer.inMenu = false;
		CubilineMusicPlayer.singleton.Stop();

		// Ensecure player color
		player1Material.color = new Color(49.0f / 255.0f, 66.0f / 255.0f, 89.0f / 255.0f);

		Reset();

		yield return new WaitForSeconds(1);
		AddInf(welcomeInfPrefab);

		yield return new WaitForSeconds(3);
		currentInf.GetComponent<PortalInf>().TakeOut();
		Destroy(currentInf, 2.0f);

		yield return new WaitForSeconds(0.5f);
		if (InputMedaitor.singleton.currentInput == InputMedaitor.INPUT_KIND.TOUCH)
			AddInf(touchInfPrefab);
		else
			AddInf(keyboardInfPrefab);

		welcomeDone = true;

	}

	public IEnumerator GotMoved()
	{
		if (!moveDone && welcomeDone)
		{
			moveDone = true;
			yield return new WaitForSeconds(3);
			currentInf.GetComponent<PortalInf>().TakeOut();
			Destroy(currentInf, 2.0f);
			moveFinish = true;

			yield return new WaitForSeconds(0.5f);
			if (InputMedaitor.singleton.currentInput == InputMedaitor.INPUT_KIND.TOUCH)
				AddInf(touchSpeedInfPrefab);
			else
				AddInf(keyboardSpeedInfPrefab);
		}
		else
		{
			yield return new WaitForSeconds(0);
		}
	}

	public IEnumerator GotSpeed()
	{
		if (!speedDone && moveFinish)
		{
			speedDone = true;
			yield return new WaitForSeconds(3);
			currentInf.GetComponent<PortalInf>().TakeOut();
			Destroy(currentInf, 2.0f);

			yield return new WaitForSeconds(0.5f);
			arenaController.GetComponent<CubilineTargetController>().commonTargetCount = 6;
			AddInf(blueBlockPrefab);

			uiController.ShowUI();
		}
		else
		{
			yield return new WaitForSeconds(0);
		}
	}

	public IEnumerator BlueTouched()
	{
		if (!blueDone)
		{
			blueDone = true;
			yield return new WaitForSeconds(8);
			currentInf.GetComponent<PortalInf>().TakeOut();
			Destroy(currentInf, 2.0f);

			arenaController.GetComponent<CubilineTargetController>().commonTargetCount = 0;
			player.UnGrow(100);
			player.SetScore(0);

			yield return new WaitForSeconds(0.5f);
			AddInf(bigBlueInfoPrefab);
			AllocatePrefabInArena(bigBlueTarget);
		}
		else
		{
			yield return new WaitForSeconds(0);
		}
	}

	public IEnumerator BigBlueTouched()
	{
		if (!bigBlueDone)
		{
			bigBlueDone = true;
			yield return new WaitForSeconds(3);
			currentInf.GetComponent<PortalInf>().TakeOut();
			Destroy(currentInf, 2.0f);

			yield return new WaitForSeconds(13);
			player.UnGrow(100);
			player.SetScore(0);

			yield return new WaitForSeconds(0.5f);
			AddInf(greenBlockInfPrefab);
			AllocatePrefabInArena(greenTarget);
		}
		else
		{
			yield return new WaitForSeconds(0);
		}
	}

	public IEnumerator GreenTouched()
	{
		if (!greenDone)
		{
			greenDone = true;
			yield return new WaitForSeconds(1);
			currentInf.GetComponent<PortalInf>().TakeOut();
			Destroy(currentInf, 2.0f);

			yield return new WaitForSeconds(1);
			player.SetScore(0);

			yield return new WaitForSeconds(0.5f);
			AddInf(grayBlockInfPrefab);
			AllocatePrefabInArena(grayTarget , 1.0f);
		}
		else
		{
			yield return new WaitForSeconds(0);
		}
	}

	public IEnumerator BigGreenTouched()
	{
		if (!bigGreenDone)
		{
			bigGreenDone = true;
			yield return new WaitForSeconds(1);
			currentInf.GetComponent<PortalInf>().TakeOut();
			Destroy(currentInf, 2.0f);

			yield return new WaitForSeconds(1);
			player.SetScore(0);

			yield return new WaitForSeconds(0.5f);
			AddInf(bigGrayBlockInfPrefab);
			AllocatePrefabInArena(bigGrayTarget, -1.0f);
		}
		else
		{
			yield return new WaitForSeconds(0);
		}
	}

	public IEnumerator OrangeTouched()
	{
		if (!orangeDone)
		{
			orangeDone = true;
			yield return new WaitForSeconds(1);
			currentInf.GetComponent<PortalInf>().TakeOut();
			Destroy(currentInf, 2.0f);

			yield return new WaitForSeconds(1);
			player.SetScore(0);

			yield return new WaitForSeconds(0.5f);
			AddInf(bigOrangeBlockInfPrefab);
			AllocatePrefabInArena(bigOrangeTarget, 1.0f);
		}
		else
		{
			yield return new WaitForSeconds(0);
		}
	}

	public IEnumerator BigOrangeTouched()
	{
		if (!bigOrangeDone)
		{
			bigOrangeDone = true;
			yield return new WaitForSeconds(1);
			currentInf.GetComponent<PortalInf>().TakeOut();
			Destroy(currentInf, 2.0f);

			yield return new WaitForSeconds(1);
			player.SetScore(0);

			yield return new WaitForSeconds(0.5f);
			AddInf(x2BlockInfPrefab);
			AllocatePrefabInArena(x2Target);
		}
		else
		{
			yield return new WaitForSeconds(0);
		}
	}

	public IEnumerator GrayTouched()
	{
		if (!grayDone)
		{
			grayDone = true;
			yield return new WaitForSeconds(1);
			currentInf.GetComponent<PortalInf>().TakeOut();
			Destroy(currentInf, 2.0f);

			yield return new WaitForSeconds(1);
			player.SetScore(0);

			yield return new WaitForSeconds(0.5f);
			AddInf(bigGreenBlockInfPrefab);
			AllocatePrefabInArena(bigGreenTarget);
		}
		else
		{
			yield return new WaitForSeconds(0);
		}
	}

	public IEnumerator BigGrayTouched()
	{
		if (!bigGrayDone)
		{
			bigGrayDone = true;
			yield return new WaitForSeconds(1);
			currentInf.GetComponent<PortalInf>().TakeOut();
			Destroy(currentInf, 2.0f);

			yield return new WaitForSeconds(1);
			player.SetScore(0);

			yield return new WaitForSeconds(0.5f);
			AddInf(orangeBlockInfPrefab);
			AllocatePrefabInArena(orangeTarget);
		}
		else
		{
			yield return new WaitForSeconds(0);
		}
	}

	public IEnumerator X2Touched()
	{
		if (!x2Done)
		{
			x2Done = true;
			arenaController.GetComponent<CubilineTargetController>().commonTargetCount = 6;
			yield return new WaitForSeconds(3);
			currentInf.GetComponent<PortalInf>().TakeOut();
			Destroy(currentInf, 2.0f);

			yield return new WaitForSeconds(7.2f);
			player.UnGrow(100);
			player.SetScore(0);
			arenaController.GetComponent<CubilineTargetController>().commonTargetCount = 0;

			yield return new WaitForSeconds(0.5f);
			AddInf(x4BlockInfPrefab);
			AllocatePrefabInArena(x4Target, 1.0f);
		}
		else
		{
			yield return new WaitForSeconds(0);
		}
	}

	public IEnumerator X4Touched()
	{
		if (!x4Done)
		{
			x4Done = true;
			arenaController.GetComponent<CubilineTargetController>().commonTargetCount = 6;
			yield return new WaitForSeconds(3);
			currentInf.GetComponent<PortalInf>().TakeOut();
			Destroy(currentInf, 2.0f);

			yield return new WaitForSeconds(7.5f);
			player.UnGrow(100);
			player.SetScore(0);

			yield return new WaitForSeconds(0.5f);
			AddInf(combinedBlockInfPrefab);
			AllocatePrefabInArena(x2Target, -0.5f);
			AllocatePrefabInArena(x4Target , 0.5f);
		}
		else
		{
			yield return new WaitForSeconds(0);
		}
	}

	public IEnumerator CombinedTouched()
	{
		if (!combinedDone && x2Done && x4Done)
		{
			combinedDone = true;
			arenaController.GetComponent<CubilineTargetController>().commonTargetCount = 6;
			yield return new WaitForSeconds(3);
			currentInf.GetComponent<PortalInf>().TakeOut();
			Destroy(currentInf, 2.0f);

			yield return new WaitForSeconds(7.5f);
			player.UnGrow(100);

			yield return new WaitForSeconds(0.5f);
			AddInf(purpleBlockInfPrefab);
			AllocatePrefabInArena(purpleTarget);
		}
		else
		{
			yield return new WaitForSeconds(0);
		}
	}

	public IEnumerator MagnetTouched()
	{
		if (!purpleDone)
		{
			purpleDone = true;
			yield return new WaitForSeconds(3);
			currentInf.GetComponent<PortalInf>().TakeOut();
			Destroy(currentInf, 2.0f);

			arenaController.GetComponent<CubilineTargetController>().commonTargetCount = 0;

			yield return new WaitForSeconds(0.5f);
			AddInf(readyBlockInfPrefab);

			yield return new WaitForSeconds(2);
			currentInf.GetComponent<PortalInf>().TakeOut();
			Destroy(currentInf, 2.0f);

			player.UnGrow(100);
			player.SetScore(0);

			arenaController.GetComponent<CubilineTargetController>().commonTargetCount = 6;
			arenaController.GetComponent<CubilineTargetController>().enableSpecialTargets = true;
			player.playerKind = CubilinePlayerController.PLAYER_KIND.ARCADE;
			arenaController.GetComponent<CubilineTargetController>().gameKind = CubilinePlayerController.PLAYER_KIND.ARCADE;
			GetComponent<AudioSource>().Stop();
			CubilineMusicPlayer.singleton.Play();
		}
		else
		{
			yield return new WaitForSeconds(0);
		}
	}

	void AllocatePrefabInArena(GameObject target, float offset = 0.0f)
	{
		Vector3 position = new Vector3();
		if(player.headPlace == CubilinePlayerController.PLACE.FRONT)
		{
			position.z = -8;
			position.x = offset;
		}
		else if (player.headPlace == CubilinePlayerController.PLACE.BACK)
		{
			position.z = 8;
			position.x = offset;
		}
		else if (player.headPlace == CubilinePlayerController.PLACE.RIGHT)
		{
			position.x = 8;
			position.z = offset;
		}
		else if (player.headPlace == CubilinePlayerController.PLACE.LEFT)
		{
			position.x = -8;
			position.z = offset;
		}
		else if (player.headPlace == CubilinePlayerController.PLACE.TOP)
		{
			position.y = 8;
			position.x = offset;
		}
		else if (player.headPlace == CubilinePlayerController.PLACE.BOTTOM)
		{
			position.y = -8;
			position.x = offset;
		}

		GameObject t = Instantiate(target, position, Quaternion.identity) as GameObject;
		t.GetComponent<CubilineTarget>().touchBody = false;
	}

	void AddInf(GameObject prefab)
	{
		currentInf = Instantiate(prefab);
		currentInf.transform.SetParent(canvasInf);
		currentInf.GetComponent<RectTransform>().offsetMax = Vector2.zero;
		currentInf.GetComponent<RectTransform>().offsetMin = Vector2.zero;
	}

	void Update()
	{
		if(status != STATUS.PLAYING)
		{
			if((followTarget.transform.position - outTarget.position).magnitude < CubilineApplication.singleton.settings.arcadeCubeSize)
			{
				// Player game inf
				CubilineApplication.singleton.player.arcadeTimePlayed += Time.time - timeOfGame;
				CubilineApplication.singleton.SavePlayer();

				CubilineApplication.singleton.CheckBlackKnowledgeLevelAchievement();
				CubilineApplication.singleton.SaveAchievements();

				CubilineMusicPlayer.singleton.Pause(true);

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
				GetComponent<AudioSource>().Play();
			}
			else if(pauseMenu.status == PauseMenuController.STATUS.MAIN_MENU)
			{
				status = STATUS.GIONG_OUT;
				uiController.GoOut();
				CubilineApplication.singleton.SavePlayer();
				GoOut();
				GetComponent<AudioSource>().Pause();
				if(currentInf != null)
				{
					currentInf.GetComponent<PortalInf>().TakeOut();
					Destroy(currentInf, 2.0f);
				}
				
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
			CubilineApplication.singleton.SavePlayer();

			CubilineApplication.singleton.CheckRedColorAchievement();
			CubilineApplication.singleton.CheckBlackKnowledgeLevelAchievement();
			CubilineApplication.singleton.SaveAchievements();

			CubilineApplication.singleton.lastComment = "arcade_scene";

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
				if (e.keyCode == KeyCode.A || e.keyCode == KeyCode.LeftArrow)
				{
					player.AddTurn(CubilinePlayerController.TURN.LEFT);
					if (player.playerKind == CubilinePlayerController.PLAYER_KIND.TUTORIAL)
						StartCoroutine(GotMoved());
				}
				else if (e.keyCode == KeyCode.D || e.keyCode == KeyCode.RightArrow)
				{
					player.AddTurn(CubilinePlayerController.TURN.RIGHT);
					if (player.playerKind == CubilinePlayerController.PLAYER_KIND.TUTORIAL)
						StartCoroutine(GotMoved());
				}
				else if (e.keyCode == KeyCode.W || e.keyCode == KeyCode.UpArrow)
				{
					player.AddTurn(CubilinePlayerController.TURN.UP);
					if (player.playerKind == CubilinePlayerController.PLAYER_KIND.TUTORIAL)
						StartCoroutine(GotMoved());
				}
				else if (e.keyCode == KeyCode.S || e.keyCode == KeyCode.DownArrow)
				{
					player.AddTurn(CubilinePlayerController.TURN.DOWN);
					if (player.playerKind == CubilinePlayerController.PLAYER_KIND.TUTORIAL)
						StartCoroutine(GotMoved());
				}
				else if (e.keyCode == KeyCode.LeftArrow)
				{
					player.AddTurn(CubilinePlayerController.TURN.LEFT);
					if (player.playerKind == CubilinePlayerController.PLAYER_KIND.TUTORIAL)
						StartCoroutine(GotMoved());
				}
				else if (e.keyCode == KeyCode.RightArrow)
				{
					player.AddTurn(CubilinePlayerController.TURN.RIGHT);
					if (player.playerKind == CubilinePlayerController.PLAYER_KIND.TUTORIAL)
						StartCoroutine(GotMoved());
				}
				else if (e.keyCode == KeyCode.UpArrow)
				{
					player.AddTurn(CubilinePlayerController.TURN.UP);
					if (player.playerKind == CubilinePlayerController.PLAYER_KIND.TUTORIAL)
						StartCoroutine(GotMoved());
				}
				else if (e.keyCode == KeyCode.DownArrow)
				{
					player.AddTurn(CubilinePlayerController.TURN.DOWN);
					if (player.playerKind == CubilinePlayerController.PLAYER_KIND.TUTORIAL)
						StartCoroutine(GotMoved());
				}
				else if (e.keyCode == KeyCode.Space)
				{
					player.speed = 8;
					if (player.playerKind == CubilinePlayerController.PLAYER_KIND.TUTORIAL)
						StartCoroutine(GotSpeed());
				}
			}
			if (e.keyCode == KeyCode.Escape && !menuKey) // Menu
			{
				menuKey = true;

				if (pauseMenu != null)
				{
					player.status = CubilinePlayerController.STATUS.PLAYING;
					pauseMenu.status = PauseMenuController.STATUS.CONTINUE;
					arenaController.status = CubilinePlayerController.STATUS.PLAYING;
					Destroy(pauseMenu.gameObject, 0.5f);
					pauseMenu = null;
					GetComponent<AudioSource>().Play();
				}
				else
				{
					player.status = CubilinePlayerController.STATUS.PAUSED;
					arenaController.status = CubilinePlayerController.STATUS.PAUSED;
					pauseMenu = Instantiate(pauseMenuBase);
					GetComponent<AudioSource>().Pause();
				}
			}
		}
		else if (e.type == EventType.keyUp)
		{
			if (e.keyCode == KeyCode.Space)
				player.speed = 4;
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
				if (player.playerKind == CubilinePlayerController.PLAYER_KIND.TUTORIAL)
					StartCoroutine(GotSpeed());
				player.speed = 8;
			}
			else
			{
				if (player.playerKind == CubilinePlayerController.PLAYER_KIND.TUTORIAL)
					StartCoroutine(GotSpeed());
				player.speed = 4;
			}

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
					{
						player.AddTurn(CubilinePlayerController.TURN.RIGHT);
						if (player.playerKind == CubilinePlayerController.PLAYER_KIND.TUTORIAL)
							StartCoroutine(GotMoved());
					}
					else if (delta.x < -5)
					{
						player.AddTurn(CubilinePlayerController.TURN.LEFT);
						if (player.playerKind == CubilinePlayerController.PLAYER_KIND.TUTORIAL)
							StartCoroutine(GotMoved());
					}
				}
				else
				{
					if (delta.y > 5)
					{
						player.AddTurn(CubilinePlayerController.TURN.UP);
						if (player.playerKind == CubilinePlayerController.PLAYER_KIND.TUTORIAL)
							StartCoroutine(GotMoved());
					}
					else if (delta.y < -5)
					{
						player.AddTurn(CubilinePlayerController.TURN.DOWN);
						if (player.playerKind == CubilinePlayerController.PLAYER_KIND.TUTORIAL)
							StartCoroutine(GotMoved());
					}
				}
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// SETUP ////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void Reset()
	{
		arenaController.Reset(15);
		followCamera.transform.localPosition = new Vector3(0.0f, 0.0f, -15 * 2.0f);

		player.Reset(15);
		player.speed = 4;
		player.hardMove = false;

		followCamera.GetComponent<UnityStandardAssets.ImageEffects.DepthOfField>().enabled = CubilineApplication.singleton.settings.depthOfField;
		followCamera.GetComponent<UnityStandardAssets.ImageEffects.ScreenSpaceAmbientOcclusion>().enabled = CubilineApplication.singleton.settings.ambientOcclusion;

		followTarget.transform.position = new Vector3(-30, 0, -7.5f);
	}

	void GoOut()
	{
		outTarget.transform.position = followCamera.transform.localPosition + (followCamera.transform.localPosition - followCamera.target.transform.localPosition).normalized * 15;
		followTarget.target = outTarget;
		followTarget.followSmoothTime = 0.8f;
	}
}
