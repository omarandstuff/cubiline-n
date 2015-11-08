using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubilineController : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public GameObject arena;
	public GameObject head;
	public CubilineBody baseBody;
	public GameObject commonTarget;
	public GameObject collision;

	//////////////////////////////////////////////////////////////
	//////////////////// CUBILINE PARAMETERS /////////////////////
	//////////////////////////////////////////////////////////////

	public enum PLACE { TOP, BOTTOM, RIGHT, LEFT, FRONT, BACK, NONE };
	public enum TURN { UP, DOWN, RIGHT, LEFT, NONE };

	public bool inputEnabled = true; // The Cube can muve but can or not resive input.
	public bool playing = true; // The cube can or not move.

	public PLACE headDirection = PLACE.RIGHT; // Directon of the head.
	public PLACE headPlace = PLACE.FRONT; // Where the head is.
	public PLACE headUp = PLACE.TOP; // Direction to where the screen is up.

	public float speed = 4.0f; // Units per second.
	public float arenaSize = 11.0f; // Units per side of the arena.

	public int commonTargetCount = 1;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

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

	private TURN lastKey;

	/////////////////////// BODY CONTROL ////////////////////////

	private Vector3 lastHeadPosition; // To now how much the head has been avanzado.
	private bool toNew; // Use this wen forced position; Manage the body add the new part and activtae this because this frame the body has been managed already.

	private bool eating; // Use this when the body has to get longer;---------------|
	private float toGrow; // When eating how much has to be grown.					|
	private float stepGrown; // How much has been grown since eating started.		|-- These two are simultaneus
	private bool unEating; // Use this when the body has to get shorter.			|	so it can get bigger and 
	private float toUnGrow; // When uneating how much has be reduced.				|	shorter at the same time.
	private float stepUnGrown; // How muc has been ungrown since uneating started.--|

	private Queue bodyQueue = new Queue(); // Body parts queue.
	CubilineBody lastBody; // Last body in the queue.

	private int bodyLength;

	//////////////////////// TARGET CONTROL /////////////////////

	private struct slotInf { public PLACE place;  public bool enabled; public Vector3 position; public object collition; }
	private List<slotInf> freeSlots = new List<slotInf>();
	private Queue<int> usedSlots = new Queue<int>();
	private Vector3 lastSlotUsed;
	private ArrayList commonTargets = new ArrayList();

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		NewGame();
	}

	void FixedUpdate()
	{
		if (playing)
		{
			if (inputEnabled)
				GetInput();

			Play();
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// SETUP ////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void NewGame()
	{
		// Set this player to the fornt of the arena.
		Vector3 initialPosition = new Vector3();

		// Sey initial arena size.
		SetArenaSize(arenaSize);

		initialPosition.x = headPlace == PLACE.RIGHT ? arenaLogicalLimit : (headPlace == PLACE.LEFT ? -arenaLogicalLimit : 0.0f);
		initialPosition.y = headPlace == PLACE.TOP ? arenaLogicalLimit : (headPlace == PLACE.BOTTOM ? -arenaLogicalLimit : 0.0f);
		initialPosition.z = headPlace == PLACE.FRONT ? -arenaLogicalLimit : (headPlace == PLACE.BACK ? arenaLogicalLimit : 0.0f);

		// Clear body parts.
		bodyQueue.Clear();
		lastBody = null;

		// Locate the head at the initial position.
		head.transform.localPosition = initialPosition;
		lastHeadPosition = head.transform.localPosition;

		// First body part with size of 2 units.
		AddBody(2.0f);

		// Sots
		freeSlots.Clear();
		usedSlots.Clear();

		// Fill free slots with the position information of every slot.
		slotInf currentSlot;
		currentSlot.enabled = true;
		currentSlot.collition = null;

		// Front and Back
		for (int j = 0; j < arenaSize; j++)
		{
			for (int k = 0; k < arenaSize; k++)
			{
				currentSlot.place = PLACE.FRONT;
				currentSlot.position = new Vector3(-arenaPlaceLimit + k, arenaPlaceLimit - j, -arenaLogicalLimit);
				freeSlots.Add(currentSlot);
			}
		}
		for (int j = 0; j < arenaSize; j++)
		{
			for (int k = 0; k < arenaSize; k++)
			{
				currentSlot.place = PLACE.BACK;
				currentSlot.position = new Vector3(-arenaPlaceLimit + k, arenaPlaceLimit - j, arenaLogicalLimit);
				freeSlots.Add(currentSlot);
			}
		}
		// Right and Left
		for (int j = 0; j < arenaSize; j++)
		{
			for (int k = 0; k < arenaSize; k++)
			{
				currentSlot.place = PLACE.RIGHT;
				currentSlot.position = new Vector3(arenaLogicalLimit, arenaPlaceLimit - j, -arenaPlaceLimit + k);
				freeSlots.Add(currentSlot);
			}
		}
		for (int j = 0; j < arenaSize; j++)
		{
			for (int k = 0; k < arenaSize; k++)
			{
				currentSlot.place = PLACE.LEFT;
				currentSlot.position = new Vector3(-arenaLogicalLimit, arenaPlaceLimit - j, -arenaPlaceLimit + k);
				freeSlots.Add(currentSlot);
			}
		}
		// Top and Bottom
		for (int j = 0; j < arenaSize; j++)
		{
			for (int k = 0; k < arenaSize; k++)
			{
				currentSlot.place = PLACE.TOP;
				currentSlot.position = new Vector3(-arenaPlaceLimit + k, arenaLogicalLimit, arenaPlaceLimit - j);
				freeSlots.Add(currentSlot);
			}
		}
		for (int j = 0; j < arenaSize; j++)
		{
			for (int k = 0; k < arenaSize; k++)
			{
				currentSlot.place = PLACE.BOTTOM;
				currentSlot.position = new Vector3(-arenaPlaceLimit + k, -arenaLogicalLimit, arenaPlaceLimit - j);
				freeSlots.Add(currentSlot);
			}
		}

		// Fill collisions base the new place and direction.
		lastSlotUsed = initialPosition;
		if (headPlace == PLACE.FRONT || headPlace == PLACE.BACK)
		{
			if(headDirection == PLACE.RIGHT)
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

		// Update slots.
		ControlSlots();

		// Targets.
		commonTargets.Clear();

		for (int i = 0; i < commonTargetCount; i++)
			SpawnCommonTarget();
	}

	public void SetArenaSize(float size)
	{
		// Keep size odd
		if (size < 5)
			arenaSize = 5;
		if (size % 2 == 0)
			arenaSize += 1;

		// Arena logical limit is the space limit of the arena plus 0.5 for the siz of the head.
		arenaLogicalLimit = size / 2.0f + 0.5f;

		// Arena Zone limit.
		arenaPlaceLimit = arenaLogicalLimit - 1.0f;

		// Set the scale of the arena object.
		arena.transform.localScale = new Vector3(arenaSize, arenaSize, arenaSize);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////// INPUT CONTROL ////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void GetInput()
	{
		TURN key = TURN.NONE;

		if (Input.GetAxis("Vertical") > 0)
			key = TURN.UP;
		else if (Input.GetAxis("Vertical") < 0)
			key = TURN.DOWN;
		else if (Input.GetAxis("Horizontal") > 0)
			key = TURN.RIGHT;
		else if (Input.GetAxis("Horizontal") < 0)
			key = TURN.LEFT;
		
		if(Input.GetButtonUp("Fire2"))
		{
			Grow(1);
		}

		if (Input.GetButtonUp("Fire3"))
		{
			UnGrow(1);
		}

		if (lastKey != key)
			AddTurn(key);

		lastKey = key;
	}

	void AddTurn(TURN turn)
	{
		turnsQueue.Enqueue(turn);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////// COLLISION CONTROL //////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void ControlTargets()
	{

	}

	public void ColliderEnter(Collider other)
	{
		if(other.tag == "Target")
		{
			CubilineTarget target = other.GetComponent<CubilineTarget>();
			if (target.toGrow >= 0)
				Grow(target.toGrow);
			else
				UnGrow(-target.toGrow);
			SpawnCommonTarget(other.gameObject);
		}
		
	}

	void Grow(int units)
	{
		eating = true;
		toGrow += units;
		bodyLength += units;
	}

	void UnGrow(int units)
	{
		int realUngorw = (bodyLength - units) >= 3 ? units : (bodyLength > 3 ? bodyLength - 3 : 0);
		unEating = true;
		toUnGrow += realUngorw;
		bodyLength -= realUngorw;
	}

	void SpawnCommonTarget(GameObject old = null)
	{
		if(old != null)
		{
			
		}
		else
		{
			if(freeSlots.Count > 0)
			{
				//int index = (int)(Random.value * freeSlots.Count);

				//Dictionary<string, Vector3>.Enumerator freeSlotsEnumerator = freeSlots.GetEnumerator();

				//commonTargets.Add(Instantiate(commonTarget, position, Quaternion.identity));
			}
		}
		
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////// SLOTS CONTROL ////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void ControlSlots()
	{
		Vector3 headPosition = head.transform.localPosition;
		int units = (int)(lastSlotUsed - headPosition).magnitude;

		if(units > 0)
		{
			for (int i = 0; i < units; i++)
			{
				TakeSlot();
			}
		}
	}

	void TakeSlot()
	{
		if (headDirection == PLACE.FRONT)
			lastSlotUsed.z -= 1;
		else if (headDirection == PLACE.BACK)
			lastSlotUsed.z += 1;
		else if (headDirection == PLACE.RIGHT)
			lastSlotUsed.x += 1;
		else if (headDirection == PLACE.LEFT)
			lastSlotUsed.x -= 1;
		else if (headDirection == PLACE.TOP)
			lastSlotUsed.y += 1;
		else if (headDirection == PLACE.BOTTOM)
			lastSlotUsed.y -= 1;

		int slotIndex = GetSlotIndex(headPlace, lastSlotUsed);
		slotInf slot = new slotInf();

		if (slotIndex != -1)
			slot = freeSlots[slotIndex];

		if (slot.enabled)
		{
			usedSlots.Enqueue(slotIndex);
			slot.enabled = false;
			slot.collition = Instantiate(collision, lastSlotUsed, Quaternion.identity);
			freeSlots[slotIndex] = slot;
		}
		else
		{
			usedSlots.Enqueue(slotIndex);
		}

		if(usedSlots.Count > bodyLength)
		{
			for (int i = 0; usedSlots.Count != bodyLength; i++)
				FreeSlot();
		}
	}

	void FreeSlot()
	{
		int slotIndex = usedSlots.Dequeue();
		if(slotIndex != -1)
		{
			slotInf slot = freeSlots[slotIndex];
			slot.enabled = true;
			Destroy((GameObject)slot.collition);
			slot.collition = null;
			freeSlots[slotIndex] = slot;
		}
	}

	int GetSlotIndex(PLACE place, Vector3 slot)
	{
		int slotsPerFace = (int)arenaSize * (int)arenaSize;

		if (place == PLACE.FRONT)
		{
			if (slot.x > arenaPlaceLimit || slot.x < -arenaPlaceLimit || slot.y > arenaPlaceLimit || slot.y < -arenaPlaceLimit) return -1;
			int index = (int)((arenaPlaceLimit - slot.y) * arenaSize + (slot.x + arenaPlaceLimit));
			return index;
		}
		else if (place == PLACE.BACK)
		{
			if (slot.x > arenaPlaceLimit || slot.x < -arenaPlaceLimit || slot.y > arenaPlaceLimit || slot.y < -arenaPlaceLimit) return -1;
			int index = slotsPerFace + (int)((arenaPlaceLimit - slot.y) * arenaSize + (slot.x + arenaPlaceLimit));
			return index;
		}
		else if (place == PLACE.RIGHT)
		{
			if (slot.z > arenaPlaceLimit || slot.z < -arenaPlaceLimit || slot.y > arenaPlaceLimit || slot.y < -arenaPlaceLimit) return -1;
			int index = slotsPerFace * 2 + (int)((arenaPlaceLimit - slot.y) * arenaSize + (arenaPlaceLimit + slot.z));
			return index;
		}
		else if (place == PLACE.LEFT)
		{
			if (slot.z > arenaPlaceLimit || slot.z < -arenaPlaceLimit || slot.y > arenaPlaceLimit || slot.y < -arenaPlaceLimit) return -1;
			int index = slotsPerFace * 3 + (int)((arenaPlaceLimit - slot.y) * arenaSize + (arenaPlaceLimit + slot.z));
			return index;
		}
		else if (place == PLACE.TOP)
		{
			if (slot.z > arenaPlaceLimit || slot.z < -arenaPlaceLimit || slot.x > arenaPlaceLimit || slot.x < -arenaPlaceLimit) return -1;
			int index = slotsPerFace * 4 + (int)((arenaPlaceLimit - slot.z) * arenaSize + (arenaPlaceLimit + slot.x));
			return index;
		}
		else if (place == PLACE.BOTTOM)
		{
			if (slot.z > arenaPlaceLimit || slot.z < -arenaPlaceLimit || slot.x > arenaPlaceLimit || slot.x < -arenaPlaceLimit) return -1;
			int index = slotsPerFace * 5 + (int)((arenaPlaceLimit - slot.z) * arenaSize + (arenaPlaceLimit + slot.x));
			return index;
		}

		return -1;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////// DIRECTION CONTROL //////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Play()
	{
		// Unit directiom vector base the head direction var.
		directionVector = new Vector3(headDirection == PLACE.RIGHT ? 1.0f : (headDirection == PLACE.LEFT ? -1.0f : 0.0f), headDirection == PLACE.TOP ? 1.0f : (headDirection == PLACE.BOTTOM ? -1.0f : 0.0f), headDirection == PLACE.BACK ? 1.0f : (headDirection == PLACE.FRONT ? -1.0f : 0.0f));

		// Move the head one step.
		head.transform.localPosition += directionVector * speed * Time.deltaTime;

		// Control what zone is the head on.
		ControlPlaceChange();

		// Do turn if are there some to do.
		ControlTurn();

		// Control the body if it wasn't control for a turn or zone change already.
		if (!toNew)
		{
			ControlBody();
			ControlSlots();
		}
		toNew = false;

		// Control targets spawning.
		ControlTargets();

		// This is step is the last known head's position.
		lastHeadPosition = head.transform.localPosition;
	}

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
						secureTurn =  TurnUp();
					else if (turn == TURN.DOWN)
						secureTurn = TurnDown();
					else if (turn == TURN.RIGHT)
						secureTurn = TurnRight();
					else if (turn == TURN.LEFT)
						secureTurn = TurnLeft();

					if (turnsQueue.Count == 0) break;
				}
				while (!secureTurn);

				if(secureTurn)
					turning = true;
			}
		}
		
		// Look for a turn if all lead to that even the last code.
		if(turning)
		{
			LookForTurn();
		}
	}

	void LookForTurn()
	{
		bool doTurn = false;

		Vector3 playerPostion = head.transform.localPosition;
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
			head.transform.localPosition = toTurnPosition;

			ControlBody();
			ControlSlots();
			toNew = true;

			headDirection = nextHeadDirection;

			AddBody(0.0f);

			turning = false;
		}
	}


	private bool TurnUp()
	{
		PLACE up = GetUpOfPlace(headPlace, headUp);
		if (headDirection == up || headDirection == GetDownOfPlace(headPlace, headUp)) return false;

		nextHeadDirection = up;

		setTurn();

		return true;
	}

	private bool TurnDown()
	{
		PLACE down = GetDownOfPlace(headPlace, headUp);
		if (headDirection == down || headDirection == GetUpOfPlace(headPlace, headUp)) return false;

		nextHeadDirection = down;

		setTurn();

		return true;
	}

	private bool TurnRight()
	{
		PLACE right = GetRightOfPlace(headPlace, headUp);
		if (headDirection == right || headDirection == GetLeftOfPlace(headPlace, headUp)) return false;

		nextHeadDirection = right;

		setTurn();

		return true;
	}

	private bool TurnLeft()
	{
		PLACE left = GetLeftOfPlace(headPlace, headUp);
		if (headDirection == left || headDirection == GetRightOfPlace(headPlace, headUp)) return false;

		nextHeadDirection = left;

		setTurn();

		return true;
	}

	void setTurn()
	{
		toTurnPosition = head.transform.localPosition;

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

	void ControlPlaceChange()
	{
		Vector3 headPosition = head.transform.localPosition;

		// headDirection and stuff.
		if (headPlace == PLACE.FRONT)
		{
			noPalce = false;
			// Take care of the no zone stuff.
			if (headPosition.x > arenaPlaceLimit || headPosition.x < -arenaPlaceLimit || headPosition.y > arenaPlaceLimit || headPosition.y < -arenaPlaceLimit)
				noPalce = true;

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
				head.transform.localPosition = headPosition;

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
				head.transform.localPosition = headPosition;

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
				head.transform.localPosition = headPosition;

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
				head.transform.localPosition = headPosition;

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
			noPalce = false;
			// Take care of the no zone stuff.
			if (headPosition.x > arenaPlaceLimit || headPosition.x < -arenaPlaceLimit || headPosition.y > arenaPlaceLimit || headPosition.y < -arenaPlaceLimit)
				noPalce = true;

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
				head.transform.localPosition = headPosition;

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
				head.transform.localPosition = headPosition;

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
				head.transform.localPosition = headPosition;

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
				head.transform.localPosition = headPosition;

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
			noPalce = false;
			// Take care of the no zone stuff.
			if (headPosition.z > arenaPlaceLimit || headPosition.z < -arenaPlaceLimit || headPosition.y > arenaPlaceLimit || headPosition.y < -arenaPlaceLimit)
				noPalce = true;

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
				head.transform.localPosition = headPosition;

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
				head.transform.localPosition = headPosition;

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
				head.transform.localPosition = headPosition;

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
				head.transform.localPosition = headPosition;

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
			noPalce = false;
			// Take care of the no zone stuff.
			if (headPosition.z > arenaPlaceLimit || headPosition.z < -arenaPlaceLimit || headPosition.y > arenaPlaceLimit || headPosition.y < -arenaPlaceLimit)
				noPalce = true;

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
				head.transform.localPosition = headPosition;

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
				head.transform.localPosition = headPosition;

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
				head.transform.localPosition = headPosition;

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
				head.transform.localPosition = headPosition;

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
			noPalce = false;
			// Take care of the no zone stuff.
			if (headPosition.z > arenaPlaceLimit || headPosition.z < -arenaPlaceLimit || headPosition.x > arenaPlaceLimit || headPosition.x < -arenaPlaceLimit)
				noPalce = true;

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
				head.transform.localPosition = headPosition;

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
				head.transform.localPosition = headPosition;

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
				head.transform.localPosition = headPosition;

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
				head.transform.localPosition = headPosition;

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
			noPalce = false;
			// Take care of the no zone stuff.
			if (headPosition.z > arenaPlaceLimit || headPosition.z < -arenaPlaceLimit || headPosition.x > arenaPlaceLimit || headPosition.x < -arenaPlaceLimit)
				noPalce = true;

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
				head.transform.localPosition = headPosition;

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
				head.transform.localPosition = headPosition;

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
				head.transform.localPosition = headPosition;

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
				head.transform.localPosition = headPosition;

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
		newBody.initialize(headPlace, headDirection, head.transform.localPosition, size);
		bodyQueue.Enqueue(newBody);
		lastBody = newBody;
	}

	void ControlBody()
	{
		float delta = (lastHeadPosition - head.transform.localPosition).magnitude;

		lastBody.Grow(delta);

		CubilineBody first = (CubilineBody)bodyQueue.Peek();

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

				((CubilineBody)bodyQueue.Peek()).Grow(deltaX);
			}
		}

		first = (CubilineBody)bodyQueue.Peek();

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
				((CubilineBody)bodyQueue.Peek()).Grow(delta);
			}
		}
	}
}
