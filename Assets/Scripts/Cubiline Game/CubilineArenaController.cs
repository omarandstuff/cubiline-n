﻿using UnityEngine;
using System.Collections;

public class CubilineArenaController : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public EaseScale arenaCube;
	public CubilinePlayerController.STATUS status;

	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	private CubilineSlotController slotController;
	private CubilineTargetController targetController;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void ManageArena()
	{
		if(status == CubilinePlayerController.STATUS.PLAYING)
			targetController.ManageTargets();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// SETUP ////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void Reset(float arenaSize)
	{
		slotController = GetComponent<CubilineSlotController>();
		slotController.Reset(arenaSize);

		targetController = GetComponent<CubilineTargetController>();
		targetController.Reset(arenaSize);

		// Set size of arena
		arenaCube.outValues = new Vector3(arenaSize / 2.0f, arenaSize / 2.0f, arenaSize / 2.0f);
		arenaCube.inValues = new Vector3(arenaSize, arenaSize, arenaSize);
		arenaCube.Reset();
		arenaCube.easeFace = EaseVector3.EASE_FACE.IN;
	}
}
