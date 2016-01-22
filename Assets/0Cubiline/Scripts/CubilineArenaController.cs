using UnityEngine;
using System.Collections;

public class CubilineArenaController : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public Transform arenaCube;
	public CubilinePlayerController.STATUS status;
	public CubilinePlayerController.PLAYER_KIND gameKind;

	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	private CubilineSlotController slotController;
	private CubilineTargetController targetController;
	private GameObject level;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void ManageArena()
	{
		if(status == CubilinePlayerController.STATUS.PLAYING && targetController != null)
			targetController.ManageTargets();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// SETUP ////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void Reset(float arenaSize)
	{
		slotController = GetComponent<CubilineSlotController>();
		if (slotController != null) slotController.Reset(arenaSize);

		targetController = GetComponent<CubilineTargetController>();
		if (targetController != null) targetController.Reset(arenaSize);

		// Load level
		if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE)
		{
			if (level == null)
			{
				level = Instantiate(CubilineApplication.singleton.levels[CubilineApplication.singleton.settings.arcadeLevelIndex].levelPrefav);
			}
		}
		else if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE_COOP)
		{
			if (level == null)
			{
				level = Instantiate(CubilineApplication.singleton.levels[CubilineApplication.singleton.settings.coopLevelIndex].levelPrefav);
			}
		}
		else if (gameKind == CubilinePlayerController.PLAYER_KIND.TUTORIAL)
		{
			if (level == null)
			{
				level = Instantiate(CubilineApplication.singleton.levels[0].levelPrefav);
			}
		}
		level.GetComponent<EaseScale>().outValues = new Vector3(arenaSize / 2.0f, arenaSize / 2.0f, arenaSize / 2.0f);
		level.GetComponent<EaseScale>().inValues = new Vector3(arenaSize, arenaSize, arenaSize);
		level.GetComponent<EaseScale>().Reset();
		level.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;


		// Set size of arena
		arenaCube.localScale = new Vector3(arenaSize, arenaSize, arenaSize);
	}
}
