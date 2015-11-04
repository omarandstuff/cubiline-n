using UnityEngine;
using System.Collections;

public class CubilineController : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public GameObject arena;
	public GameObject head;
	public CubilineBody baseBody;

	//////////////////////////////////////////////////////////////
	//////////////////// CUBILINE PARAMETERS /////////////////////
	//////////////////////////////////////////////////////////////

	public PLACE headDirection = PLACE.RIGHT; // Directon of the head.
	public PLACE headZone = PLACE.FRONT; // Where the head is.
	public PLACE headUp = PLACE.TOP; // Direction to where the screen is up.
	public float arenaSize = 11.0f; // Units per side of the arena.
	public float speed = 4.0f; // Units per second.
	public enum PLACE { TOP, BOTTOM, RIGHT, LEFT, FRONT, BACK, NONE };
	public enum TURN { UP, DOWN, RIGHT, LEFT, NONE };
	public bool inputEnabled = true; // The Cube can muve but can or not resive input.
	public bool playing = true; // The cube can or not move.

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	private float arenaLogicalLimit; // The side limit plus the 0.5 units offset for the head to be aout of the side.
	private float arenaZoneLimit; // The side limit minus 0.5 that is the tolerance distance to made a turn.

	////////////////////// DIRECTION CONTROL /////////////////////

	PLACE nextHeadDirection; // Based on the current head position in te arena when a turn is requested this is the direction that whas actually requested.
	private Vector3 directionVector; // Unit vector for direction.
	private Vector3 toTurnPosition; // When it has to turn where is it gonna be.
	private Queue turnsQueue = new Queue(); // Acumulated turns;
	private bool turning; // A turn has been appled so wait until it happens.
	private bool noZone; // When it reach the far the side of the arena is a no zono so it can't turn at all.

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

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR ////////////////////////////////////////
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
				getInput();

			Play();
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// SetUp ////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void NewGame()
	{
		// Set this player to the fornt of the arena.
		Vector3 initialPosition = new Vector3();

		// Sey initial arena size.
		SetArenaSize(arenaSize);

		initialPosition.x = headZone == PLACE.RIGHT ? arenaLogicalLimit : (headZone == PLACE.LEFT ? -arenaLogicalLimit : 0.0f);
		initialPosition.y = headZone == PLACE.TOP ? arenaLogicalLimit : (headZone == PLACE.BOTTOM ? -arenaLogicalLimit : 0.0f);
		initialPosition.z = headZone == PLACE.FRONT ? -arenaLogicalLimit : (headZone == PLACE.BACK ? arenaLogicalLimit : 0.0f);

		// Clear body parts.
		bodyQueue.Clear();
		lastBody = null;

		// Locate the head at the initial position.
		head.transform.localPosition = initialPosition;
		lastHeadPosition = head.transform.localPosition;

		// First body part with size of 2 units.
		AddBody(2.0f);

		// Reset control
		toNew = false;
		toGrow = 0.0f;
		toUnGrow = 0.0f;
		stepGrown = 0.0f;
		stepUnGrown = 0.0f;
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
		arenaZoneLimit = arenaLogicalLimit - 1.0f;

		// Set the scale of the arena object.
		arena.transform.localScale = new Vector3(arenaSize, arenaSize, arenaSize);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////// Input Control ////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void getInput()
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

		if(lastKey != key)
			AddTurn(key);

		lastKey = key;
	}

	void AddTurn(TURN turn)
	{
		turnsQueue.Enqueue(turn);

		if(turn == TURN.UP)
		{
			print("Key Up");
		}
		if (turn == TURN.DOWN)
		{
			print("Key Down");
		}
		if (turn == TURN.RIGHT)
		{
			print("Key Right");
		}
		if (turn == TURN.LEFT)
		{
			print("Key Left");
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////// Direction Control //////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Play()
	{
		// Unit directiom vector base the head direction var.
		directionVector = new Vector3(headDirection == PLACE.RIGHT ? 1.0f : (headDirection == PLACE.LEFT ? -1.0f : 0.0f), headDirection == PLACE.TOP ? 1.0f : (headDirection == PLACE.BOTTOM ? -1.0f : 0.0f), headDirection == PLACE.BACK ? 1.0f : (headDirection == PLACE.FRONT ? -1.0f : 0.0f));

		// Move the head one step.
		head.transform.localPosition += directionVector * speed * Time.deltaTime;

		// Control what zone is the head on.
		ControlZoneChange();

		// Do turn if are there some to do.
		ControlTurn();

		// Control the body if it wasn't control for a turn or zone change already.
		if (!toNew)
			ControlBody();
		toNew = false;

		// This is step is the last known head's position.
		lastHeadPosition = head.transform.localPosition;
	}

	void ControlTurn(bool special = false)
	{
		if (!turning)
		{
			if (turnsQueue.Count != 0 && (!noZone || special))
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
		
		// Look for aturn if all guide to that even the last code.
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

			headDirection = nextHeadDirection;

			ControlBody();
			AddBody(0.0f);
			toNew = true;

			turning = false;
		}
	}


	private bool TurnUp()
	{
		PLACE up = GetUpOfZone(headZone, headUp);
		if (headDirection == up || headDirection == GetDownOfZone(headZone, headUp)) return false;

		nextHeadDirection = up;

		setTurn();

		print("Turn Up");

		return true;
	}

	private bool TurnDown()
	{
		PLACE down = GetDownOfZone(headZone, headUp);
		if (headDirection == down || headDirection == GetUpOfZone(headZone, headUp)) return false;

		nextHeadDirection = down;

		setTurn();

		print("Turn Down");

		return true;
	}

	private bool TurnRight()
	{
		PLACE right = GetRightOfZone(headZone, headUp);
		if (headDirection == right || headDirection == GetLeftOfZone(headZone, headUp)) return false;

		nextHeadDirection = right;

		setTurn();

		print("Turn Right");

		return true;
	}

	private bool TurnLeft()
	{
		PLACE left = GetLeftOfZone(headZone, headUp);
		if (headDirection == left || headDirection == GetRightOfZone(headZone, headUp)) return false;

		nextHeadDirection = left;

		setTurn();

		print("Turn Left");

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

	void ControlZoneChange()
	{
		Vector3 headPosition = head.transform.localPosition;

		// headDirection and stuff.
		if (headZone == PLACE.FRONT)
		{
			noZone = false;
			// Take care of the no zone stuff.
			if (headPosition.x > arenaZoneLimit || headPosition.x < -arenaZoneLimit || headPosition.y > arenaZoneLimit || headPosition.y < -arenaZoneLimit)
				noZone = true;

			if (headPosition.x >= arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.BACK;
				headZone = PLACE.RIGHT;

				// New up.
				if (headUp == PLACE.RIGHT)
					headUp = PLACE.BACK;
				else if (headUp == PLACE.LEFT)
					headUp = PLACE.FRONT;

				// Keep in the boundaries.
				headPosition.x = arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.x <= -arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.BACK;
				headZone = PLACE.LEFT;

				// New up.
				if (headUp == PLACE.RIGHT)
					headUp = PLACE.FRONT;
				else if (headUp == PLACE.LEFT)
					headUp = PLACE.BACK;

				// Keep in the boundaries.
				headPosition.x = -arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.y >= arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.BACK;
				headZone = PLACE.TOP;

				// New up.
				if (headUp == PLACE.TOP)
					headUp = PLACE.BACK;
				else if (headUp == PLACE.BOTTOM)
					headUp = PLACE.FRONT;

				// Keep in the boundaries.
				headPosition.y = arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.y <= -arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.BACK;
				headZone = PLACE.BOTTOM;

				// New up.
				if (headUp == PLACE.TOP)
					headUp = PLACE.FRONT;
				else if (headUp == PLACE.BOTTOM)
					headUp = PLACE.BACK;

				// Keep in the boundaries.
				headPosition.y = -arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
		}
		else if (headZone == PLACE.BACK)
		{
			noZone = false;
			// Take care of the no zone stuff.
			if (headPosition.x > arenaZoneLimit || headPosition.x < -arenaZoneLimit || headPosition.y > arenaZoneLimit || headPosition.y < -arenaZoneLimit)
				noZone = true;

			if (headPosition.x >= arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.FRONT;
				headZone = PLACE.RIGHT;

				// New up.
				if (headUp == PLACE.RIGHT)
					headUp = PLACE.FRONT;
				else if (headUp == PLACE.LEFT)
					headUp = PLACE.BACK;

				// Keep in the boundaries.
				headPosition.x = arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.x <= -arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.FRONT;
				headZone = PLACE.LEFT;

				// New up.
				if (headUp == PLACE.RIGHT)
					headUp = PLACE.BACK;
				else if (headUp == PLACE.LEFT)
					headUp = PLACE.FRONT;

				// Keep in the boundaries.
				headPosition.x = -arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.y >= arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.FRONT;
				headZone = PLACE.TOP;

				// New up.
				if (headUp == PLACE.TOP)
					headUp = PLACE.FRONT;
				else if (headUp == PLACE.BOTTOM)
					headUp = PLACE.BACK;

				// Keep in the boundaries.
				headPosition.y = arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.y <= -arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.FRONT;
				headZone = PLACE.BOTTOM;

				// New up.
				if (headUp == PLACE.TOP)
					headUp = PLACE.BACK;
				else if (headUp == PLACE.BOTTOM)
					headUp = PLACE.FRONT;

				// Keep in the boundaries.
				headPosition.y = -arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
		}
		else if (headZone == PLACE.RIGHT)
		{
			noZone = false;
			// Take care of the no zone stuff.
			if (headPosition.z > arenaZoneLimit || headPosition.z < -arenaZoneLimit || headPosition.y > arenaZoneLimit || headPosition.y < -arenaZoneLimit)
				noZone = true;

			if (headPosition.z <= -arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.LEFT;
				headZone = PLACE.FRONT;

				// New up.
				if (headUp == PLACE.FRONT)
					headUp = PLACE.LEFT;
				else if (headUp == PLACE.BACK)
					headUp = PLACE.RIGHT;

				// Keep in the boundaries.
				headPosition.z = -arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.z >= arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.LEFT;
				headZone = PLACE.BACK;

				// New up.
				if (headUp == PLACE.FRONT)
					headUp = PLACE.RIGHT;
				else if (headUp == PLACE.BACK)
					headUp = PLACE.LEFT;

				// Keep in the boundaries.
				headPosition.z = arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.y >= arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.LEFT;
				headZone = PLACE.TOP;

				// New up.
				if (headUp == PLACE.TOP)
					headUp = PLACE.LEFT;
				else if (headUp == PLACE.BOTTOM)
					headUp = PLACE.RIGHT;

				// Keep in the boundaries.
				headPosition.y = arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.y <= -arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.LEFT;
				headZone = PLACE.BOTTOM;

				// New up.
				if (headUp == PLACE.TOP)
					headUp = PLACE.RIGHT;
				else if (headUp == PLACE.BOTTOM)
					headUp = PLACE.LEFT;

				// Keep in the boundaries.
				headPosition.y = -arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
		}
		else if (headZone == PLACE.LEFT)
		{
			noZone = false;
			// Take care of the no zone stuff.
			if (headPosition.z > arenaZoneLimit || headPosition.z < -arenaZoneLimit || headPosition.y > arenaZoneLimit || headPosition.y < -arenaZoneLimit)
				noZone = true;

			if (headPosition.z <= -arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.RIGHT;
				headZone = PLACE.FRONT;

				// New up.
				if (headUp == PLACE.FRONT)
					headUp = PLACE.RIGHT;
				else if (headUp == PLACE.BACK)
					headUp = PLACE.LEFT;

				// Keep in the boundaries.
				headPosition.z = -arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.z >= arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.RIGHT;
				headZone = PLACE.BACK;

				// New up.
				if (headUp == PLACE.FRONT)
					headUp = PLACE.LEFT;
				else if (headUp == PLACE.BACK)
					headUp = PLACE.RIGHT;

				// Keep in the boundaries.
				headPosition.z = arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.y >= arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.RIGHT;
				headZone = PLACE.TOP;

				// New up.
				if (headUp == PLACE.TOP)
					headUp = PLACE.RIGHT;
				else if (headUp == PLACE.BOTTOM)
					headUp = PLACE.LEFT;

				// Keep in the boundaries.
				headPosition.y = arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.y <= -arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.RIGHT;
				headZone = PLACE.BOTTOM;

				// New up.
				if (headUp == PLACE.TOP)
					headUp = PLACE.LEFT;
				else if (headUp == PLACE.BOTTOM)
					headUp = PLACE.RIGHT;

				// Keep in the boundaries.
				headPosition.y = -arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
		}
		else if (headZone == PLACE.TOP)
		{
			noZone = false;
			// Take care of the no zone stuff.
			if (headPosition.z > arenaZoneLimit || headPosition.z < -arenaZoneLimit || headPosition.x > arenaZoneLimit || headPosition.x < -arenaZoneLimit)
				noZone = true;

			if (headPosition.z <= -arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.BOTTOM;
				headZone = PLACE.FRONT;

				// New up.
				if (headUp == PLACE.FRONT)
					headUp = PLACE.BOTTOM;
				else if (headUp == PLACE.BACK)
					headUp = PLACE.TOP;

				// Keep in the boundaries.
				headPosition.z = -arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.z >= arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.BOTTOM;
				headZone = PLACE.BACK;

				// New up.
				if (headUp == PLACE.FRONT)
					headUp = PLACE.TOP;
				else if (headUp == PLACE.BACK)
					headUp = PLACE.BOTTOM;

				// Keep in the boundaries.
				headPosition.z = arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.x >= arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.BOTTOM;
				headZone = PLACE.RIGHT;

				// New up.
				if (headUp == PLACE.RIGHT)
					headUp = PLACE.BOTTOM;
				else if (headUp == PLACE.LEFT)
					headUp = PLACE.TOP;

				// Keep in the boundaries.
				headPosition.x = arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.x <= -arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.BOTTOM;
				headZone = PLACE.LEFT;

				// New up.
				if (headUp == PLACE.RIGHT)
					headUp = PLACE.TOP;
				else if (headUp == PLACE.LEFT)
					headUp = PLACE.BOTTOM;

				// Keep in the boundaries.
				headPosition.x = -arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
		}
		else if (headZone == PLACE.BOTTOM)
		{
			noZone = false;
			// Take care of the no zone stuff.
			if (headPosition.z > arenaZoneLimit || headPosition.z < -arenaZoneLimit || headPosition.x > arenaZoneLimit || headPosition.x < -arenaZoneLimit)
				noZone = true;

			if (headPosition.z <= -arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.TOP;
				headZone = PLACE.FRONT;

				// New up.
				if (headUp == PLACE.FRONT)
					headUp = PLACE.TOP;
				else if (headUp == PLACE.BACK)
					headUp = PLACE.BOTTOM;

				// Keep in the boundaries.
				headPosition.z = -arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.z >= arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.TOP;
				headZone = PLACE.BACK;

				// New up.
				if (headUp == PLACE.FRONT)
					headUp = PLACE.BOTTOM;
				else if (headUp == PLACE.BACK)
					headUp = PLACE.TOP;

				// Keep in the boundaries.
				headPosition.z = arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.x >= arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.TOP;
				headZone = PLACE.RIGHT;

				// New up.
				if (headUp == PLACE.RIGHT)
					headUp = PLACE.TOP;
				else if (headUp == PLACE.LEFT)
					headUp = PLACE.BOTTOM;

				// Keep in the boundaries.
				headPosition.x = arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
			else if (headPosition.x <= -arenaLogicalLimit)
			{
				// New arena positional info.
				headDirection = PLACE.TOP;
				headZone = PLACE.LEFT;

				// New up.
				if (headUp == PLACE.RIGHT)
					headUp = PLACE.BOTTOM;
				else if (headUp == PLACE.LEFT)
					headUp = PLACE.TOP;

				// Keep in the boundaries.
				headPosition.x = -arenaLogicalLimit;

				// Apply boundaries.
				head.transform.localPosition = headPosition;

				// At turn add body.
				ControlBody();
				AddBody(0.0f);
				toNew = true;

				// Control if has o be a turn after change the zone.
				ControlTurn(true);
			}
		}
	}

	PLACE GetUpOfZone(PLACE zone, PLACE up)
	{
		return up;
	}

	PLACE GetDownOfZone(PLACE zone, PLACE up)
	{
		if (zone == PLACE.FRONT || zone == PLACE.BACK)
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
		if (zone == PLACE.RIGHT || zone == PLACE.LEFT)
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
		if (zone == PLACE.TOP || zone == PLACE.BOTTOM)
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

	PLACE GetRightOfZone(PLACE zone, PLACE up)
	{
		if (zone == PLACE.FRONT)
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
		if (zone == PLACE.BACK)
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
		if (zone == PLACE.RIGHT)
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
		if (zone == PLACE.LEFT)
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
		if (zone == PLACE.TOP)
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
		if (zone == PLACE.BOTTOM)
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

	PLACE GetLeftOfZone(PLACE zone, PLACE up)
	{
		if (zone == PLACE.FRONT)
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
		if (zone == PLACE.BACK)
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
		if (zone == PLACE.RIGHT)
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
		if (zone == PLACE.LEFT)
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
		if (zone == PLACE.TOP)
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
		if (zone == PLACE.BOTTOM)
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
		newBody.initialize(headZone, headDirection, head.transform.localPosition, size);
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
