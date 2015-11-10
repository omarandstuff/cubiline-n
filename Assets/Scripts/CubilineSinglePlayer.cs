﻿using UnityEngine;

public class CubilineSinglePlayer : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public CubilineArenaController arenaController;
	public CubilinePlayerController player1;
	public CubilinePlayerController player2;

	//////////////////////////////////////////////////////////////
	///////////////////////// PARAMETERS /////////////////////////
	//////////////////////////////////////////////////////////////

	public uint arenaSize = 5;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////


	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start ()
	{
		Reset(); // First Reset
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		player1.Go();
		player2.Go();
		arenaController.ManageArena();
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

		player1.Reset(arenaSize);
		player2.Reset(arenaSize);
	}
}
