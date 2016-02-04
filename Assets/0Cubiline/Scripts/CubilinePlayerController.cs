using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CubilinePlayerController : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public CubilineTutorialFase1 tutorialHolder;
	public CubilineSlotController slotController;
	public CubilineTargetController targetController;
	public Follow ghost;
	public Transform smoothHead;
	public Transform head;
	public CubilineBody baseBody;
	public GameObject finishParticle;
	public CubilineUIController uiController;
	public CubilineCoopUIController coopUIController;
	public CubilinePlayerController player1;

	//////////////////////////////////////////////////////////////
	//////////////////// CUBILINE PARAMETERS /////////////////////
	//////////////////////////////////////////////////////////////

	public bool test;

	public enum PLACE { TOP, BOTTOM, RIGHT, LEFT, FRONT, BACK, NONE }
	public enum TURN { UP, DOWN, RIGHT, LEFT, NONE }
	public enum STATUS { PLAYING, PAUSED, FINISH }

	public STATUS status = STATUS.PLAYING;

	public PLACE headDirection = PLACE.RIGHT; // Directon of the head.
	public PLACE headPlace = PLACE.FRONT; // Where the head is.
	public PLACE headUp = PLACE.TOP; // Direction to where the screen is up.

	public float speed = 4.0f; // Units per second.
	public bool hardMove;

	public enum PLAYER_KIND { ARCADE, ARCADE_COOP, VS, TUTORIAL}
	public PLAYER_KIND playerKind;
	public uint playerNumber;

	public float multiplerTime = 10.0f;
	public int multipler; // Score multipler

	public int coopLength // For coop porpuses
	{
		set
		{
			_coopLength = value;
			CubilineApplication.singleton.player.lastCoopLength = (uint)bodyLength + (uint)_coopLength;
			if (CubilineApplication.singleton.player.lastCoopLength > CubilineApplication.singleton.player.bestCoopLength)
			{
				CubilineApplication.singleton.player.bestCoopLength = CubilineApplication.singleton.player.lastCoopLength;
				CubilineApplication.singleton.player.coopNewLengthRecord = true;
			}
		}
		get
		{
			return _coopLength;
		}
	}
	private int _coopLength;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	private uint arcadeScore;

	private float arenaLogicalLimit; // The side limit plus the 0.5 units offset for the head to be aout of the side.
	private float arenaPlaceLimit; // The side limit minus 0.5 that is the tolerance distance to made a turn.

	////////////////////// DIRECTION CONTROL /////////////////////

	PLACE nextHeadDirection; // Based on the current head position in te arena when a turn is requested this is the direction that whas actually requested.
	private Vector3 directionVector; // Unit vector for direction.
	private Vector3 toTurnPosition; // When it has to turn where is it gonna be.
	private Queue turnsQueue = new Queue(); // Acumulated turns;
	private bool turning; // A turn has been appled so wait until it happens.
	private bool noPalce; // When it reach the far the side of the arena is a no zono so it can't turn at all.

	//////////////////////// INPUT CONTROL //////////////////////

	private TURN lastTurn = TURN.NONE;

	/////////////////////// BODY CONTROL ////////////////////////

	private Vector3 lastHeadPosition; // To now how much the head has been avanzado.
	private bool toNew; // Use this wen forced position; Manage the body add the new part and activtae this because this frame the body has been managed already.

	private bool eating; // Use this when the body has to get longer;---------------|
	private float toGrow; // When eating how much has to be grown.					|
	private float stepGrown; // How much has been grown since eating started.		|-- These two are simultaneus
	private bool unEating; // Use this when the body has to get shorter.			|	so it can get bigger and 
	private float toUnGrow; // When uneating how much has be reduced.				|	shorter at the same time.
	private float stepUnGrown; // How muc has been ungrown since uneating started.--|

	private Queue<CubilineBody> bodyQueue = new Queue<CubilineBody>(); // Body parts queue.
	CubilineBody lastBody; // Last body in the queue.

	private Queue<int> usedSlots = new Queue<int>(); // Positions used by the body.
	private Vector3 lastSlotUsed; // Keep traking of where was the las time it take a slot from the arena.

	private int bodyLength; // measure how much the body length.
	private float multiplerCurrentTime; // If it is greater than 0 then time to multply the score earned.

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void Start()
	{
		if(test)
		{
			GameObject.Find("Follow Camera").GetComponent<OrbitAndLook>().targetArea = GameObject.Find("Cube").transform;
			Reset(11);
		}
	}

	public void Update()
	{
		if(test)
		{
			Go();
		}
	}

	public void Go()
	{
		if (status != STATUS.PLAYING) return;

		// Unit directiom vector base the head direction var.
		directionVector = new Vector3(headDirection == PLACE.RIGHT ? 1.0f : (headDirection == PLACE.LEFT ? -1.0f : 0.0f), headDirection == PLACE.TOP ? 1.0f : (headDirection == PLACE.BOTTOM ? -1.0f : 0.0f), headDirection == PLACE.BACK ? 1.0f : (headDirection == PLACE.FRONT ? -1.0f : 0.0f));

		// Move the head one step.
		smoothHead.localPosition += directionVector * speed * Time.deltaTime;

		if(hardMove) // Keep the looking of move every unit.
		{
			Vector3 fixedHeadPosition = smoothHead.localPosition;
			if (headDirection == PLACE.FRONT)
			{
				fixedHeadPosition.z -= arenaLogicalLimit;
				fixedHeadPosition.z = (int)fixedHeadPosition.z - 1;
				fixedHeadPosition.z += arenaLogicalLimit;
			}
			else if (headDirection == PLACE.BACK)
			{
				fixedHeadPosition.z += arenaLogicalLimit;
				fixedHeadPosition.z = (int)fixedHeadPosition.z + 1;
				fixedHeadPosition.z -= arenaLogicalLimit;
			}
			else if (headDirection == PLACE.RIGHT)
			{
				fixedHeadPosition.x += arenaLogicalLimit;
				fixedHeadPosition.x = (int)fixedHeadPosition.x + 1;
				fixedHeadPosition.x -= arenaLogicalLimit;
			}
			else if (headDirection == PLACE.LEFT)
			{
				fixedHeadPosition.x -= arenaLogicalLimit;
				fixedHeadPosition.x = (int)fixedHeadPosition.x - 1;
				fixedHeadPosition.x += arenaLogicalLimit;
			}
			else if (headDirection == PLACE.TOP)
			{
				fixedHeadPosition.y += arenaLogicalLimit;
				fixedHeadPosition.y = (int)fixedHeadPosition.y + 1;
				fixedHeadPosition.y -= arenaLogicalLimit;
			}
			else if (headDirection == PLACE.BOTTOM)
			{
				fixedHeadPosition.y -= arenaLogicalLimit;
				fixedHeadPosition.y = (int)fixedHeadPosition.y - 1;
				fixedHeadPosition.y += arenaLogicalLimit;
			}
			head.localPosition = fixedHeadPosition;
		}
		else
		{
			head.localPosition = smoothHead.localPosition;
		}

		// Control i fwe are in the no place zone
		ControlNoPlace();

		// Do turn if are there some to do.
		ControlTurn();

		// Control what zone is the head on.
		ControlPlaceChange();

		// Control the body if it wasn't control for a turn or zone change already.
		if (!toNew)
		{
			ControlBody();
			ControlSlots();
		}
		toNew = false;

		// This is step is the last known head's position.
		lastHeadPosition = head.localPosition;

		// Multipler system
		if (playerNumber == 0)
		{
			if (multiplerCurrentTime > 0)
			{
				multiplerCurrentTime -= Time.deltaTime;

				if (playerKind == PLAYER_KIND.ARCADE || playerKind == PLAYER_KIND.TUTORIAL)
					uiController.multiplerTime = multiplerCurrentTime / multiplerTime;
				else if (playerKind == PLAYER_KIND.ARCADE_COOP)
					coopUIController.multiplerTime = multiplerCurrentTime / multiplerTime;
			}
			else
			{
				multipler = 0;
				if (playerKind == PLAYER_KIND.ARCADE || playerKind == PLAYER_KIND.TUTORIAL)
					uiController.multipler = multipler;
				else if (playerKind == PLAYER_KIND.ARCADE_COOP)
					coopUIController.multipler = multipler;

				AchievementsData.diceCheck1 = false;
				AchievementsData.blackdiceCheck1 = false;
			}
		}
		else
			multipler = player1.multipler;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// SETUP ////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void Reset(float arenaSize)
	{
		// Set this player to the fornt of the arena.
		Vector3 initialPosition = new Vector3();

		// Arena logical limit is the space limit of the arena plus 0.5 for the siz of the head.
		arenaLogicalLimit = arenaSize / 2.0f + 0.5f;

		// Arena Zone limit.
		arenaPlaceLimit = arenaLogicalLimit - 1.0f;

		// Ghost speed.
		ghost.followSpeed = speed;

		initialPosition.x = headPlace == PLACE.RIGHT ? arenaLogicalLimit : (headPlace == PLACE.LEFT ? -arenaLogicalLimit : 0.0f);
		initialPosition.y = headPlace == PLACE.TOP ? arenaLogicalLimit : (headPlace == PLACE.BOTTOM ? -arenaLogicalLimit : 0.0f);
		initialPosition.z = headPlace == PLACE.FRONT ? -arenaLogicalLimit : (headPlace == PLACE.BACK ? arenaLogicalLimit : 0.0f);

		// Clear body parts.
		while (bodyQueue.Count != 0)
		{
			Destroy(bodyQueue.Dequeue().gameObject);
		}
		lastBody = null;

		// Clear turns;
		nextHeadDirection = PLACE.NONE;
		turnsQueue.Clear();
		turning = false;
		noPalce = false;

		// Locate the head at the initial position.
		smoothHead.localPosition = initialPosition;
		head.localPosition = initialPosition;
		lastHeadPosition = head.localPosition;

		// First body part with size of 2 units.
		AddBody(2.0f);

		// Sots
		usedSlots.Clear();

		// Fill collisions base the new place and direction.
		lastSlotUsed = initialPosition;
		if (headPlace == PLACE.FRONT || headPlace == PLACE.BACK)
		{
			if (headDirection == PLACE.RIGHT)
				lastSlotUsed.x -= 3.0f;
			else if (headDirection == PLACE.LEFT)
				lastSlotUsed.x += 3.0f;
			else if (headDirection == PLACE.TOP)
				lastSlotUsed.y -= 3.0f;
			else if (headDirection == PLACE.LEFT)
				lastSlotUsed.y += 3.0f;
		}
		else if (headPlace == PLACE.RIGHT || headPlace == PLACE.LEFT)
		{
			if (headDirection == PLACE.FRONT)
				lastSlotUsed.z += 3.0f;
			else if (headDirection == PLACE.BACK)
				lastSlotUsed.z -= 3.0f;
			else if (headDirection == PLACE.TOP)
				lastSlotUsed.y -= 3.0f;
			else if (headDirection == PLACE.LEFT)
				lastSlotUsed.y += 3.0f;
		}
		else if (headPlace == PLACE.TOP || headPlace == PLACE.BOTTOM)
		{
			if (headDirection == PLACE.FRONT)
				lastSlotUsed.z += 3.0f;
			else if (headDirection == PLACE.BACK)
				lastSlotUsed.z -= 3.0f;
			else if (headDirection == PLACE.RIGHT)
				lastSlotUsed.x -= 3.0f;
			else if (headDirection == PLACE.LEFT)
				lastSlotUsed.x += 3.0f;
		}

		// Reset control
		eating = false;
		unEating = false;
		toNew = false;
		toGrow = 0.0f;
		toUnGrow = 0.0f;
		stepGrown = 0.0f;
		stepUnGrown = 0.0f;
		bodyLength = 3;
		if (playerKind == PLAYER_KIND.ARCADE_COOP) coopLength = 3;
			

		// Update slots.
		ControlSlots();

		// Arcade
		arcadeScore = 0;
		CubilineApplication.singleton.player.newRecord = false;
		CubilineApplication.singleton.player.coopNewRecord = false;
		CubilineApplication.singleton.player.newLengthRecord = false;
		CubilineApplication.singleton.player.coopNewLengthRecord = false;

		if (playerKind == PLAYER_KIND.ARCADE)
		{
			uiController.score = arcadeScore;
			uiController.length = (uint)bodyLength;
		}
		else if (playerKind == PLAYER_KIND.TUTORIAL)
		{
			uiController.score = arcadeScore;
		}
		else if (playerKind == PLAYER_KIND.ARCADE_COOP)
		{
			coopUIController.score = arcadeScore;
			coopUIController.length = (uint)bodyLength + (uint)coopLength;
		}

	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////// INPUT CONTROL ////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void AddTurn(TURN turn)
	{
		if (lastTurn == turn) return;

		lastTurn = turn;
		turnsQueue.Enqueue(turn);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////// COLLISION CONTROL //////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void ColliderEnter(Collider other)
	{
		if (other.tag == "Target")
		{
			CubilineTarget target = other.GetComponent<CubilineTarget>();
			target.activated = true;
			target.activator = head;

			if (playerKind == PLAYER_KIND.ARCADE || playerKind == PLAYER_KIND.ARCADE_COOP || playerKind == PLAYER_KIND.TUTORIAL)
			{
				if (target.toGrow >= 0)
				{
					Grow(target.toGrow);
				}
				else
					UnGrow(-target.toGrow);
			}

			if (playerKind == PLAYER_KIND.TUTORIAL)
				StartCoroutine(tutorialHolder.BlueTouched());

			targetController.DismissCommon(target.index);

			if (playerNumber == 0)
				AddScore((uint)target.score);
			else
				player1.AddScore((uint)target.score);

		}
		if (other.tag == "Special Target")
		{
			CubilineTarget target = other.GetComponent<CubilineTarget>();
			target.activated = true;
			target.activator = head;

			if (playerKind == PLAYER_KIND.ARCADE || playerKind == PLAYER_KIND.TUTORIAL)
			{
				if (target.toGrow >= 0)
				{
					Grow(target.toGrow);
				}
				else
					UnGrow(-target.toGrow);

				if (target.targetTag == "2x")
					Start2Xmultipler();
				else if (target.targetTag == "4x")
					Start4Xmultipler();

				if (playerKind == PLAYER_KIND.TUTORIAL)
				{
					if (target.targetTag == "Green")
						StartCoroutine(tutorialHolder.GreenTouched());
					else if (target.targetTag == "Big Green")
						StartCoroutine(tutorialHolder.BigGreenTouched());
					else if (target.targetTag == "Orange")
						StartCoroutine(tutorialHolder.OrangeTouched());
					else if (target.targetTag == "Big Orange")
						StartCoroutine(tutorialHolder.BigOrangeTouched());
					else if (target.targetTag == "Gray")
						StartCoroutine(tutorialHolder.GrayTouched());
					else if (target.targetTag == "Big Gray")
						StartCoroutine(tutorialHolder.BigGrayTouched());
				}
					
			}
			else if (playerKind == PLAYER_KIND.ARCADE_COOP)
			{
				if (target.toGrow >= 0)
				{
					Grow(target.toGrow);
				}
				else
					UnGrow(-target.toGrow);

				if(playerNumber == 0)
				{
					if (target.targetTag == "2x")
						Start2Xmultipler();
					else if (target.targetTag == "4x")
						Start4Xmultipler();
				}
				else
				{
					if (target.targetTag == "2x")
						player1.Start2Xmultipler();
					else if (target.targetTag == "4x")
						player1.Start4Xmultipler();
				}
			}

			targetController.DismissSpecial(target.index);

			if (playerNumber == 0)
				AddScore((uint)target.score);
			else
				player1.AddScore((uint)target.score);

			if (target.activated)
			{
				if (target.targetTag == "Big Target")
				{
					targetController.ApplyBigBlue();
					if(playerKind == PLAYER_KIND.TUTORIAL)
						StartCoroutine(tutorialHolder.BigBlueTouched());
				}
				else if (target.targetTag == "Magnet")
				{
					targetController.ApplyMagnet(head);
					if (playerKind == PLAYER_KIND.TUTORIAL)
						StartCoroutine(tutorialHolder.MagnetTouched());
				}
			}
		}
		if (other.tag == "Finish")
		{
			if(playerKind == PLAYER_KIND.ARCADE || playerKind == PLAYER_KIND.ARCADE_COOP)
			{
				if (CubilineApplication.singleton.settings.particles) Destroy(Instantiate(finishParticle, head.position, Quaternion.identity), 8.0f);
				status = STATUS.FINISH;
			}
			else if (playerKind == PLAYER_KIND.TUTORIAL)
			{
				Destroy(Instantiate(finishParticle, head.position, Quaternion.identity), 8.0f);
				UnGrow(10000);
			}
		}
	}

	public void Grow(int units)
	{
		eating = true;
		toGrow += units;
		bodyLength += units;
		if (playerKind == PLAYER_KIND.ARCADE)
		{
			uiController.plusLength = units;

			CubilineApplication.singleton.player.totalArcadeLength += (uint)units;
			CubilineApplication.singleton.player.lastArcadeLength = (uint)bodyLength;
			if (bodyLength > CubilineApplication.singleton.player.bestArcadeLength)
			{
				CubilineApplication.singleton.player.bestArcadeLength = (uint)bodyLength;
				CubilineApplication.singleton.player.newLengthRecord = true;
			}
		}
		else if (playerKind == PLAYER_KIND.TUTORIAL)
		{
			uiController.plusLength = units;
			return;
		}
		else if (playerKind == PLAYER_KIND.ARCADE_COOP)
		{
			coopUIController.plusLength = units;

			CubilineApplication.singleton.player.totalCoopLength += (uint)units;

			if (player1 != null)
				player1.coopLength += units;
			else
			{
				CubilineApplication.singleton.player.lastCoopLength = (uint)bodyLength + (uint)_coopLength;

				coopUIController.length = CubilineApplication.singleton.player.lastCoopLength;

				if (CubilineApplication.singleton.player.lastCoopLength > CubilineApplication.singleton.player.bestCoopLength)
				{
					CubilineApplication.singleton.player.bestCoopLength = CubilineApplication.singleton.player.lastCoopLength;
					CubilineApplication.singleton.player.coopNewLengthRecord = true;
				}
			}
		}
		CubilineApplication.singleton.CheckToyLevelAchievement();
		CubilineApplication.singleton.CheckBlackToyLevelAchievement();
	}

	public void UnGrow(int units)
	{
		int realUngorw = (bodyLength - units) >= 3 ? units : (bodyLength > 3 ? bodyLength - 3 : 0);
		unEating = true;
		toUnGrow += realUngorw;
		bodyLength -= realUngorw;

		if(playerKind == PLAYER_KIND.ARCADE)
		{
			uiController.plusLength = -realUngorw;
			CubilineApplication.singleton.player.lastArcadeLength = (uint)bodyLength;
		}
		else if (playerKind == PLAYER_KIND.TUTORIAL)
		{
			uiController.plusLength = -realUngorw;
			return;
		}
		else if (playerKind == PLAYER_KIND.ARCADE_COOP)
		{
			coopUIController.plusLength = -realUngorw;

			if (player1 != null)
				player1.coopLength -= realUngorw;
			else
			{
				CubilineApplication.singleton.player.lastCoopLength = (uint)bodyLength + (uint)_coopLength;

				coopUIController.length = CubilineApplication.singleton.player.lastCoopLength;

				if (CubilineApplication.singleton.player.lastCoopLength > CubilineApplication.singleton.player.bestCoopLength)
				{
					CubilineApplication.singleton.player.bestCoopLength = CubilineApplication.singleton.player.lastCoopLength;
					CubilineApplication.singleton.player.coopNewLengthRecord = true;
				}
			}
		}

		CubilineApplication.singleton.CheckToyLevelAchievement();
		CubilineApplication.singleton.CheckBlackToyLevelAchievement();
	}

	public void Start2Xmultipler()
	{
		if(multipler == 0)
			multipler = 2;
		else
			multipler *= 2;

		multiplerCurrentTime = multiplerTime;

		if(playerKind == PLAYER_KIND.ARCADE)
			uiController.multipler = multipler;
		else if (playerKind == PLAYER_KIND.TUTORIAL)
		{
			uiController.multipler = multipler;
			StartCoroutine(tutorialHolder.CombinedTouched());
			StartCoroutine(tutorialHolder.X2Touched());
			return;
		}
		else if (playerKind == PLAYER_KIND.ARCADE_COOP)
			coopUIController.multipler = multipler;

		AchievementsData.diceCheck1 = true;
		if (multipler >= 8)
			AchievementsData.blackdiceCheck1 = true;
	}

	public void Start4Xmultipler()
	{
		if (multipler == 0)
			multipler = 4;
		else
			multipler *= 4;

		multiplerCurrentTime = multiplerTime;

		if (playerKind == PLAYER_KIND.ARCADE)
			uiController.multipler = multipler;
		else if (playerKind == PLAYER_KIND.TUTORIAL)
		{
			uiController.multipler = multipler;
			StartCoroutine(tutorialHolder.CombinedTouched());
			StartCoroutine(tutorialHolder.X4Touched());
			return;
		}
		else if (playerKind == PLAYER_KIND.ARCADE_COOP)
			coopUIController.multipler = multipler;

		AchievementsData.diceCheck1 = true;
		if (multipler >= 8)
			AchievementsData.blackdiceCheck1 = true;
	}

	public void AddScore(uint score)
	{
		if (playerKind == PLAYER_KIND.ARCADE)
		{
			arcadeScore += score * (uint)(multiplerCurrentTime > 0 ? multipler : 1);
			CubilineApplication.singleton.player.lastArcadeScore = arcadeScore;
			CubilineApplication.singleton.player.lastArcadeScoreDateTime = DateTime.Now;

			uiController.plusScore = score * (uint)(multiplerCurrentTime > 0 ? multipler : 1);
			uiController.score = arcadeScore;
			uiController.length = (uint)bodyLength;

			if (CubilineApplication.singleton.player.bestArcadeScore < arcadeScore)
			{
				CubilineApplication.singleton.player.bestArcadeScore = arcadeScore;
				CubilineApplication.singleton.player.bestArcadeScoreDateTime = DateTime.Now;
				CubilineApplication.singleton.player.newRecord = true;
			}
		}
		else if (playerKind == PLAYER_KIND.TUTORIAL)
		{
			arcadeScore += score * (uint)(multiplerCurrentTime > 0 ? multipler : 1);

			uiController.plusScore = score * (uint)(multiplerCurrentTime > 0 ? multipler : 1);
			uiController.score = arcadeScore;
			uiController.length = (uint)bodyLength;

			return;
		}
		else if (playerKind == PLAYER_KIND.ARCADE_COOP)
		{
			arcadeScore += score * (uint)(multiplerCurrentTime > 0 ? multipler : 1);
			CubilineApplication.singleton.player.lastCoopScore = arcadeScore;
			CubilineApplication.singleton.player.lastCoopScoreDateTime = DateTime.Now;

			if(score > 0) coopUIController.plusScore = score * (uint)(multiplerCurrentTime > 0 ? multipler : 1);
			coopUIController.score = arcadeScore;

			if (CubilineApplication.singleton.player.bestCoopScore < arcadeScore)
			{
				CubilineApplication.singleton.player.bestCoopScore = arcadeScore;
				CubilineApplication.singleton.player.bestCoopScoreDateTime = DateTime.Now;
				CubilineApplication.singleton.player.coopNewRecord = true;
			}
		}
		CubilineApplication.singleton.CheckScoreColorAchievement();
		CubilineApplication.singleton.CheckLengthColorAchievement();
		CubilineApplication.singleton.CheckFillColorAchievement();
	}

	public void SetScore(int score)
	{
		arcadeScore = (uint)score;

		uiController.score = arcadeScore;
		uiController.length = (uint)bodyLength;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////// SLOTS CONTROL ////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void ControlSlots()
	{
		if (slotController == null) return;
		Vector3 headPosition = head.localPosition;
		int units = (int)(lastSlotUsed - headPosition).magnitude;

		if (units > 0)
		{
			for (int i = 0; i < units; i++)
			{
				usedSlots.Enqueue(slotController.TakeSlot(headPlace, headDirection, ref lastSlotUsed));
			}
		}

		if(usedSlots.Count > bodyLength)
		{
			while (usedSlots.Count != bodyLength)
				slotController.FreeSlot(usedSlots.Dequeue());
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////// DIRECTION CONTROL //////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void ControlTurn(bool special = false)
	{
		if (!turning)
		{
			if (turnsQueue.Count != 0 && (!noPalce || special))
			{
				bool secureTurn = false;

				do
				{
					TURN turn = (TURN)turnsQueue.Dequeue();

					if (turn == TURN.UP)
						secureTurn = TurnUp();
					else if (turn == TURN.DOWN)
						secureTurn = TurnDown();
					else if (turn == TURN.RIGHT)
						secureTurn = TurnRight();
					else if (turn == TURN.LEFT)
						secureTurn = TurnLeft();

					if (turnsQueue.Count == 0) break;
				}
				while (!secureTurn);

				if (secureTurn)
					turning = true;
			}
		}

		// Look for a turn if all lead to that even the last code.
		if (turning)
		{
			LookForTurn();
		}
	}

	void LookForTurn()
	{
		bool doTurn = false;

		Vector3 playerPostion = smoothHead.localPosition;
		if (headDirection == PLACE.FRONT)
		{
			if (playerPostion.z <= toTurnPosition.z)
				doTurn = true;
		}
		else if (headDirection == PLACE.BACK)
		{
			if (playerPostion.z >= toTurnPosition.z)
				doTurn = true;
		}
		else if (headDirection == PLACE.RIGHT)
		{
			if (playerPostion.x >= toTurnPosition.x)
				doTurn = true;
		}
		else if (headDirection == PLACE.LEFT)
		{
			if (playerPostion.x <= toTurnPosition.x)
				doTurn = true;
		}
		else if (headDirection == PLACE.TOP)
		{
			if (playerPostion.y >= toTurnPosition.y)
				doTurn = true;
		}
		else if (headDirection == PLACE.BOTTOM)
		{
			if (playerPostion.y <= toTurnPosition.y)
				doTurn = true;
		}

		if (doTurn)
		{
			smoothHead.localPosition = toTurnPosition;
			head.localPosition = toTurnPosition;

			ControlBody();
			ControlSlots();
			toNew = true;

			headDirection = nextHeadDirection;

			AddBody(0.0f);

			turning = false;
		}
	}


	bool TurnUp()
	{
		PLACE up = GetUpOfPlace(headPlace, headUp);
		if (headDirection == up || headDirection == GetDownOfPlace(headPlace, headUp)) return false;

		nextHeadDirection = up;

		setTurn();

		return true;
	}

	bool TurnDown()
	{
		PLACE down = GetDownOfPlace(headPlace, headUp);
		if (headDirection == down || headDirection == GetUpOfPlace(headPlace, headUp)) return false;

		nextHeadDirection = down;

		setTurn();

		return true;
	}

	bool TurnRight()
	{
		PLACE right = GetRightOfPlace(headPlace, headUp);
		if (headDirection == right || headDirection == GetLeftOfPlace(headPlace, headUp)) return false;

		nextHeadDirection = right;

		setTurn();

		return true;
	}

	bool TurnLeft()
	{
		PLACE left = GetLeftOfPlace(headPlace, headUp);
		if (headDirection == left || headDirection == GetRightOfPlace(headPlace, headUp)) return false;

		nextHeadDirection = left;

		setTurn();

		return true;
	}

	void setTurn()
	{
		toTurnPosition = smoothHead.localPosition;

		if (headDirection == PLACE.FRONT)
		{
			toTurnPosition.z -= arenaLogicalLimit;
			toTurnPosition.z = (int)toTurnPosition.z - 1;
			toTurnPosition.z += arenaLogicalLimit;
		}
		else if (headDirection == PLACE.BACK)
		{
			toTurnPosition.z += arenaLogicalLimit;
			toTurnPosition.z = (int)toTurnPosition.z + 1;
			toTurnPosition.z -= arenaLogicalLimit;
		}
		else if (headDirection == PLACE.RIGHT)
		{
			toTurnPosition.x += arenaLogicalLimit;
			toTurnPosition.x = (int)toTurnPosition.x + 1;
			toTurnPosition.x -= arenaLogicalLimit;
		}
		else if (headDirection == PLACE.LEFT)
		{
			toTurnPosition.x -= arenaLogicalLimit;
			toTurnPosition.x = (int)toTurnPosition.x - 1;
			toTurnPosition.x += arenaLogicalLimit;
		}
		else if (headDirection == PLACE.TOP)
		{
			toTurnPosition.y += arenaLogicalLimit;
			toTurnPosition.y = (int)toTurnPosition.y + 1;
			toTurnPosition.y -= arenaLogicalLimit;
		}
		else if (headDirection == PLACE.BOTTOM)
		{
			toTurnPosition.y -= arenaLogicalLimit;
			toTurnPosition.y = (int)toTurnPosition.y - 1;
			toTurnPosition.y += arenaLogicalLimit;
		}
	}

	void ControlNoPlace()
	{
		Vector3 headPosition = smoothHead.localPosition;

		// headDirection and stuff.
		if (headPlace == PLACE.FRONT)
		{
			noPalce = false;
			// Take care of the no zone stuff.
			if (headPosition.x > arenaPlaceLimit || headPosition.x < -arenaPlaceLimit || headPosition.y > arenaPlaceLimit || headPosition.y < -arenaPlaceLimit)
				noPalce = true;
		}
		else if (headPlace == PLACE.BACK)
		{
			noPalce = false;
			// Take care of the no zone stuff.
			if (headPosition.x > arenaPlaceLimit || headPosition.x < -arenaPlaceLimit || headPosition.y > arenaPlaceLimit || headPosition.y < -arenaPlaceLimit)
				noPalce = true;
		}
		else if (headPlace == PLACE.RIGHT)
		{
			noPalce = false;
			// Take care of the no zone stuff.
			if (headPosition.z > arenaPlaceLimit || headPosition.z < -arenaPlaceLimit || headPosition.y > arenaPlaceLimit || headPosition.y < -arenaPlaceLimit)
				noPalce = true;
		}
		else if (headPlace == PLACE.LEFT)
		{
			noPalce = false;
			// Take care of the no zone stuff.
			if (headPosition.z > arenaPlaceLimit || headPosition.z < -arenaPlaceLimit || headPosition.y > arenaPlaceLimit || headPosition.y < -arenaPlaceLimit)
				noPalce = true;
		}
		else if (headPlace == PLACE.TOP)
		{
			noPalce = false;
			// Take care of the no zone stuff.
			if (headPosition.z > arenaPlaceLimit || headPosition.z < -arenaPlaceLimit || headPosition.x > arenaPlaceLimit || headPosition.x < -arenaPlaceLimit)
				noPalce = true;
		}
		else if (headPlace == PLACE.BOTTOM)
		{
			noPalce = false;
			// Take care of the no zone stuff.
			if (headPosition.z > arenaPlaceLimit || headPosition.z < -arenaPlaceLimit || headPosition.x > arenaPlaceLimit || headPosition.x < -arenaPlaceLimit)
				noPalce = true;
		}
	}

	void ControlPlaceChange()
	{
		Vector3 headPosition = smoothHead.localPosition;

		// headDirection and stuff.
		if (headPlace == PLACE.FRONT)
		{
			if (headPosition.x >= arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.RIGHT)
					headUp = PLACE.BACK;
				else if (headUp == PLACE.LEFT)
					headUp = PLACE.FRONT;

				// Keep in the boundaries.
				headPosition.x = arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.BACK;
				headPlace = PLACE.RIGHT;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.x <= -arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.RIGHT)
					headUp = PLACE.FRONT;
				else if (headUp == PLACE.LEFT)
					headUp = PLACE.BACK;

				// Keep in the boundaries.
				headPosition.x = -arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.BACK;
				headPlace = PLACE.LEFT;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.y >= arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.TOP)
					headUp = PLACE.BACK;
				else if (headUp == PLACE.BOTTOM)
					headUp = PLACE.FRONT;

				// Keep in the boundaries.
				headPosition.y = arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.BACK;
				headPlace = PLACE.TOP;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.y <= -arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.TOP)
					headUp = PLACE.FRONT;
				else if (headUp == PLACE.BOTTOM)
					headUp = PLACE.BACK;

				// Keep in the boundaries.
				headPosition.y = -arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.BACK;
				headPlace = PLACE.BOTTOM;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
		}
		else if (headPlace == PLACE.BACK)
		{
			if (headPosition.x >= arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.RIGHT)
					headUp = PLACE.FRONT;
				else if (headUp == PLACE.LEFT)
					headUp = PLACE.BACK;

				// Keep in the boundaries.
				headPosition.x = arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.FRONT;
				headPlace = PLACE.RIGHT;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.x <= -arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.RIGHT)
					headUp = PLACE.BACK;
				else if (headUp == PLACE.LEFT)
					headUp = PLACE.FRONT;

				// Keep in the boundaries.
				headPosition.x = -arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.FRONT;
				headPlace = PLACE.LEFT;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.y >= arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.TOP)
					headUp = PLACE.FRONT;
				else if (headUp == PLACE.BOTTOM)
					headUp = PLACE.BACK;

				// Keep in the boundaries.
				headPosition.y = arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.FRONT;
				headPlace = PLACE.TOP;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.y <= -arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.TOP)
					headUp = PLACE.BACK;
				else if (headUp == PLACE.BOTTOM)
					headUp = PLACE.FRONT;

				// Keep in the boundaries.
				headPosition.y = -arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.FRONT;
				headPlace = PLACE.BOTTOM;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
		}
		else if (headPlace == PLACE.RIGHT)
		{
			if (headPosition.z <= -arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.FRONT)
					headUp = PLACE.LEFT;
				else if (headUp == PLACE.BACK)
					headUp = PLACE.RIGHT;

				// Keep in the boundaries.
				headPosition.z = -arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.LEFT;
				headPlace = PLACE.FRONT;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.z >= arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.FRONT)
					headUp = PLACE.RIGHT;
				else if (headUp == PLACE.BACK)
					headUp = PLACE.LEFT;

				// Keep in the boundaries.
				headPosition.z = arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.LEFT;
				headPlace = PLACE.BACK;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.y >= arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.TOP)
					headUp = PLACE.LEFT;
				else if (headUp == PLACE.BOTTOM)
					headUp = PLACE.RIGHT;

				// Keep in the boundaries.
				headPosition.y = arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.LEFT;
				headPlace = PLACE.TOP;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.y <= -arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.TOP)
					headUp = PLACE.RIGHT;
				else if (headUp == PLACE.BOTTOM)
					headUp = PLACE.LEFT;

				// Keep in the boundaries.
				headPosition.y = -arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.LEFT;
				headPlace = PLACE.BOTTOM;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
		}
		else if (headPlace == PLACE.LEFT)
		{
			if (headPosition.z <= -arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.FRONT)
					headUp = PLACE.RIGHT;
				else if (headUp == PLACE.BACK)
					headUp = PLACE.LEFT;

				// Keep in the boundaries.
				headPosition.z = -arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.RIGHT;
				headPlace = PLACE.FRONT;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.z >= arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.FRONT)
					headUp = PLACE.LEFT;
				else if (headUp == PLACE.BACK)
					headUp = PLACE.RIGHT;

				// Keep in the boundaries.
				headPosition.z = arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.RIGHT;
				headPlace = PLACE.BACK;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.y >= arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.TOP)
					headUp = PLACE.RIGHT;
				else if (headUp == PLACE.BOTTOM)
					headUp = PLACE.LEFT;

				// Keep in the boundaries.
				headPosition.y = arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.RIGHT;
				headPlace = PLACE.TOP;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.y <= -arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.TOP)
					headUp = PLACE.LEFT;
				else if (headUp == PLACE.BOTTOM)
					headUp = PLACE.RIGHT;

				// Keep in the boundaries.
				headPosition.y = -arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.RIGHT;
				headPlace = PLACE.BOTTOM;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
		}
		else if (headPlace == PLACE.TOP)
		{
			if (headPosition.z <= -arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.FRONT)
					headUp = PLACE.BOTTOM;
				else if (headUp == PLACE.BACK)
					headUp = PLACE.TOP;

				// Keep in the boundaries.
				headPosition.z = -arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.BOTTOM;
				headPlace = PLACE.FRONT;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.z >= arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.FRONT)
					headUp = PLACE.TOP;
				else if (headUp == PLACE.BACK)
					headUp = PLACE.BOTTOM;

				// Keep in the boundaries.
				headPosition.z = arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.BOTTOM;
				headPlace = PLACE.BACK;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.x >= arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.RIGHT)
					headUp = PLACE.BOTTOM;
				else if (headUp == PLACE.LEFT)
					headUp = PLACE.TOP;

				// Keep in the boundaries.
				headPosition.x = arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.BOTTOM;
				headPlace = PLACE.RIGHT;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.x <= -arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.RIGHT)
					headUp = PLACE.TOP;
				else if (headUp == PLACE.LEFT)
					headUp = PLACE.BOTTOM;

				// Keep in the boundaries.
				headPosition.x = -arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.BOTTOM;
				headPlace = PLACE.LEFT;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
		}
		else if (headPlace == PLACE.BOTTOM)
		{
			if (headPosition.z <= -arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.FRONT)
					headUp = PLACE.TOP;
				else if (headUp == PLACE.BACK)
					headUp = PLACE.BOTTOM;

				// Keep in the boundaries.
				headPosition.z = -arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.TOP;
				headPlace = PLACE.FRONT;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.z >= arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.FRONT)
					headUp = PLACE.BOTTOM;
				else if (headUp == PLACE.BACK)
					headUp = PLACE.TOP;

				// Keep in the boundaries.
				headPosition.z = arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.TOP;
				headPlace = PLACE.BACK;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.x >= arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.RIGHT)
					headUp = PLACE.TOP;
				else if (headUp == PLACE.LEFT)
					headUp = PLACE.BOTTOM;

				// Keep in the boundaries.
				headPosition.x = arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.TOP;
				headPlace = PLACE.RIGHT;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.x <= -arenaLogicalLimit)
			{
				// New up.
				if (headUp == PLACE.RIGHT)
					headUp = PLACE.BOTTOM;
				else if (headUp == PLACE.LEFT)
					headUp = PLACE.TOP;

				// Keep in the boundaries.
				headPosition.x = -arenaLogicalLimit;

				// Apply boundaries.
				smoothHead.localPosition = headPosition;
				head.localPosition = headPosition;

				// Control body stats
				ControlBody();
				ControlSlots();
				toNew = true;

				// New arena positional info.
				headDirection = PLACE.TOP;
				headPlace = PLACE.LEFT;

				// At turn add body.
				AddBody(0.0f);

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
		}
	}

	PLACE GetUpOfPlace(PLACE place, PLACE up)
	{
		return up;
	}

	PLACE GetDownOfPlace(PLACE place, PLACE up)
	{
		if (place == PLACE.FRONT || place == PLACE.BACK)
		{
			if (up == PLACE.RIGHT)
				return PLACE.LEFT;
			if (up == PLACE.LEFT)
				return PLACE.RIGHT;
			if (up == PLACE.TOP)
				return PLACE.BOTTOM;
			if (up == PLACE.BOTTOM)
				return PLACE.TOP;
		}
		if (place == PLACE.RIGHT || place == PLACE.LEFT)
		{
			if (up == PLACE.FRONT)
				return PLACE.BACK;
			if (up == PLACE.BACK)
				return PLACE.FRONT;
			if (up == PLACE.TOP)
				return PLACE.BOTTOM;
			if (up == PLACE.BOTTOM)
				return PLACE.TOP;
		}
		if (place == PLACE.TOP || place == PLACE.BOTTOM)
		{
			if (up == PLACE.FRONT)
				return PLACE.BACK;
			if (up == PLACE.BACK)
				return PLACE.FRONT;
			if (up == PLACE.RIGHT)
				return PLACE.LEFT;
			if (up == PLACE.LEFT)
				return PLACE.RIGHT;
		}
		return 0;
	}

	PLACE GetRightOfPlace(PLACE place, PLACE up)
	{
		if (place == PLACE.FRONT)
		{
			if (up == PLACE.RIGHT)
				return PLACE.BOTTOM;
			if (up == PLACE.LEFT)
				return PLACE.TOP;
			if (up == PLACE.TOP)
				return PLACE.RIGHT;
			if (up == PLACE.BOTTOM)
				return PLACE.LEFT;
		}
		if (place == PLACE.BACK)
		{
			if (up == PLACE.RIGHT)
				return PLACE.TOP;
			if (up == PLACE.LEFT)
				return PLACE.BOTTOM;
			if (up == PLACE.TOP)
				return PLACE.LEFT;
			if (up == PLACE.BOTTOM)
				return PLACE.RIGHT;
		}
		if (place == PLACE.RIGHT)
		{
			if (up == PLACE.FRONT)
				return PLACE.TOP;
			if (up == PLACE.BACK)
				return PLACE.BOTTOM;
			if (up == PLACE.TOP)
				return PLACE.BACK;
			if (up == PLACE.BOTTOM)
				return PLACE.FRONT;
		}
		if (place == PLACE.LEFT)
		{
			if (up == PLACE.FRONT)
				return PLACE.BOTTOM;
			if (up == PLACE.BACK)
				return PLACE.TOP;
			if (up == PLACE.TOP)
				return PLACE.FRONT;
			if (up == PLACE.BOTTOM)
				return PLACE.BACK;
		}
		if (place == PLACE.TOP)
		{
			if (up == PLACE.FRONT)
				return PLACE.LEFT;
			if (up == PLACE.BACK)
				return PLACE.RIGHT;
			if (up == PLACE.RIGHT)
				return PLACE.FRONT;
			if (up == PLACE.LEFT)
				return PLACE.BACK;
		}
		if (place == PLACE.BOTTOM)
		{
			if (up == PLACE.FRONT)
				return PLACE.RIGHT;
			if (up == PLACE.BACK)
				return PLACE.LEFT;
			if (up == PLACE.RIGHT)
				return PLACE.BACK;
			if (up == PLACE.LEFT)
				return PLACE.FRONT;
		}
		return 0;
	}

	PLACE GetLeftOfPlace(PLACE place, PLACE up)
	{
		if (place == PLACE.FRONT)
		{
			if (up == PLACE.RIGHT)
				return PLACE.TOP;
			if (up == PLACE.LEFT)
				return PLACE.BOTTOM;
			if (up == PLACE.TOP)
				return PLACE.LEFT;
			if (up == PLACE.BOTTOM)
				return PLACE.RIGHT;
		}
		if (place == PLACE.BACK)
		{
			if (up == PLACE.RIGHT)
				return PLACE.BOTTOM;
			if (up == PLACE.LEFT)
				return PLACE.TOP;
			if (up == PLACE.TOP)
				return PLACE.RIGHT;
			if (up == PLACE.BOTTOM)
				return PLACE.LEFT;
		}
		if (place == PLACE.RIGHT)
		{
			if (up == PLACE.FRONT)
				return PLACE.BOTTOM;
			if (up == PLACE.BACK)
				return PLACE.TOP;
			if (up == PLACE.TOP)
				return PLACE.FRONT;
			if (up == PLACE.BOTTOM)
				return PLACE.BACK;
		}
		if (place == PLACE.LEFT)
		{
			if (up == PLACE.FRONT)
				return PLACE.TOP;
			if (up == PLACE.BACK)
				return PLACE.BOTTOM;
			if (up == PLACE.TOP)
				return PLACE.BACK;
			if (up == PLACE.BOTTOM)
				return PLACE.FRONT;
		}
		if (place == PLACE.TOP)
		{
			if (up == PLACE.FRONT)
				return PLACE.RIGHT;
			if (up == PLACE.BACK)
				return PLACE.LEFT;
			if (up == PLACE.RIGHT)
				return PLACE.BACK;
			if (up == PLACE.LEFT)
				return PLACE.FRONT;
		}
		if (place == PLACE.BOTTOM)
		{
			if (up == PLACE.FRONT)
				return PLACE.LEFT;
			if (up == PLACE.BACK)
				return PLACE.RIGHT;
			if (up == PLACE.RIGHT)
				return PLACE.FRONT;
			if (up == PLACE.LEFT)
				return PLACE.BACK;
		}
		return 0;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////// BODY CONTROL /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void AddBody(float size)
	{
		CubilineBody newBody = Instantiate(baseBody);
		newBody.transform.parent = transform;
		newBody.initialize(headPlace, headDirection, head.localPosition, size);
		newBody.GetComponent<CubilineHeadCollider>().cubilineController = this;
		bodyQueue.Enqueue(newBody);
		lastBody = newBody;
	}

	void ControlBody()
	{
		float delta = (lastHeadPosition - head.localPosition).magnitude;

		lastBody.Grow(delta);

		CubilineBody first = bodyQueue.Peek();

		if (eating)
		{
			stepGrown += delta;

			if (stepGrown >= toGrow)
			{
				first.Grow(-(stepGrown - toGrow));
				eating = false;
				stepGrown = 0.0f;
				toGrow = 0.0f;
			}
		}
		else
		{
			float deltaX = first.Grow(-delta);

			if (deltaX <= 0.0f)
			{
				bodyQueue.Dequeue();

				Destroy(first.gameObject);

				bodyQueue.Peek().Grow(deltaX);
			}
		}

		first = bodyQueue.Peek();

		if (unEating)
		{
			stepUnGrown += delta;

			if (stepUnGrown >= toUnGrow)
			{
				delta -= (stepUnGrown - toUnGrow);
				unEating = false;
				stepUnGrown = 0.0f;
				toUnGrow = 0;
			}

			delta = first.Grow(-delta);

			if (delta <= 0.0f)
			{
				bodyQueue.Dequeue();
				Destroy(first.gameObject);
				bodyQueue.Peek().Grow(delta);
			}
		}
	}
}
