using UnityEngine;
using System.Collections.Generic;

public class CubilineSlotController : MonoBehaviour
{
	//public GameObject colliderBase;
	public uint slotsTaken;
	public uint freeSlots;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	public struct Slot { public CubilinePlayerController.PLACE place; public bool enabled; public Vector3 position; public GameObject collision; public int index;}
	private List<Slot> slots = new List<Slot>(); // Filled with all the position posibilities of be in the cube.

	private float arenaSize;
	private float arenaLogicalLimit; // The side limit plus the 0.5 units offset for the head to be aout of the side.
	private float arenaPlaceLimit; // The side limit minus 0.5 that is the tolerance distance to made a turn.

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// SETUP ////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void Reset(float arena_size)
	{
		arenaSize = arena_size;

		// Arena logical limit is the space limit of the arena plus 0.5 for the siz of the head.
		arenaLogicalLimit = arenaSize / 2.0f + 0.5f;

		// Arena Zone limit.
		arenaPlaceLimit = arenaLogicalLimit - 1.0f;

		// Sots
		slots.Clear();

		slotsTaken = 0;

		// Fill free slots with the position information of every slot.
		Slot currentSlot;
		currentSlot.enabled = true;
		currentSlot.collision = null;

		// Front and Back
		for (int j = 0; j < arenaSize; j++)
		{
			for (int k = 0; k < arenaSize; k++)
			{
				currentSlot.index = slots.Count;
				currentSlot.place = CubilinePlayerController.PLACE.FRONT;
				currentSlot.position = new Vector3(-arenaPlaceLimit + k, arenaPlaceLimit - j, -arenaLogicalLimit);
				slots.Add(currentSlot);
			}
		}
		for (int j = 0; j < arenaSize; j++)
		{
			for (int k = 0; k < arenaSize; k++)
			{
				currentSlot.index = slots.Count;
				currentSlot.place = CubilinePlayerController.PLACE.BACK;
				currentSlot.position = new Vector3(-arenaPlaceLimit + k, arenaPlaceLimit - j, arenaLogicalLimit);
				slots.Add(currentSlot);
			}
		}
		// Right and Left
		for (int j = 0; j < arenaSize; j++)
		{
			for (int k = 0; k < arenaSize; k++)
			{
				currentSlot.index = slots.Count;
				currentSlot.place = CubilinePlayerController.PLACE.RIGHT;
				currentSlot.position = new Vector3(arenaLogicalLimit, arenaPlaceLimit - j, -arenaPlaceLimit + k);
				slots.Add(currentSlot);
			}
		}
		for (int j = 0; j < arenaSize; j++)
		{
			for (int k = 0; k < arenaSize; k++)
			{
				currentSlot.index = slots.Count;
				currentSlot.place = CubilinePlayerController.PLACE.LEFT;
				currentSlot.position = new Vector3(-arenaLogicalLimit, arenaPlaceLimit - j, -arenaPlaceLimit + k);
				slots.Add(currentSlot);
			}
		}
		// Top and Bottom
		for (int j = 0; j < arenaSize; j++)
		{
			for (int k = 0; k < arenaSize; k++)
			{
				currentSlot.index = slots.Count;
				currentSlot.place = CubilinePlayerController.PLACE.TOP;
				currentSlot.position = new Vector3(-arenaPlaceLimit + k, arenaLogicalLimit, arenaPlaceLimit - j);
				slots.Add(currentSlot);
			}
		}
		for (int j = 0; j < arenaSize; j++)
		{
			for (int k = 0; k < arenaSize; k++)
			{
				currentSlot.index = slots.Count;
				currentSlot.place = CubilinePlayerController.PLACE.BOTTOM;
				currentSlot.position = new Vector3(-arenaPlaceLimit + k, -arenaLogicalLimit, arenaPlaceLimit - j);
				slots.Add(currentSlot);
			}
		}

		freeSlots = (uint)slots.Count;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////// SLOTS /////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public Slot GetFreeSlot()
	{
		if (slots.Count != slotsTaken)
		{
			int index = (int)(Random.value * slots.Count);
			Slot slot = new Slot();

			for (int i = 0; i < 5; i++)
			{
				if (slots[index].enabled) break;
				index = (int)(Random.value * slots.Count);
			}

			if (!slots[index].enabled)
			{
				List<Slot>.Enumerator e = slots.GetEnumerator();
				while (true)
				{
					if (e.Current.enabled)
					{
						slot = e.Current;
						break;
					}
					e.MoveNext();
				}
			}
			else
			{
				slot = slots[index];
			}

			return slot;
		}

		return new Slot();
	}

	public int TakeSlot(CubilinePlayerController.PLACE place, CubilinePlayerController.PLACE direction, ref Vector3 lastUsedPosition)
	{
		if (direction == CubilinePlayerController.PLACE.FRONT)
			lastUsedPosition.z -= 1;
		else if (direction == CubilinePlayerController.PLACE.BACK)
			lastUsedPosition.z += 1;
		else if (direction == CubilinePlayerController.PLACE.RIGHT)
			lastUsedPosition.x += 1;
		else if (direction == CubilinePlayerController.PLACE.LEFT)
			lastUsedPosition.x -= 1;
		else if (direction == CubilinePlayerController.PLACE.TOP)
			lastUsedPosition.y += 1;
		else if (direction == CubilinePlayerController.PLACE.BOTTOM)
			lastUsedPosition.y -= 1;

		int slotIndex = GetSlotIndex(place, lastUsedPosition);
		Slot slot = new Slot();

		if (slotIndex == -1) return -1;

		slot = slots[slotIndex];

		if (slot.enabled)
		{
			slot.enabled = false;
			//slot.collision = (GameObject)Instantiate(colliderBase, lastUsedPosition, Quaternion.identity);
			slots[slotIndex] = slot;
			slotsTaken++;
			freeSlots--;
			return slotIndex;
		}

		return -1;
	}

	public void TakeSlotAt(int index)
	{
		Slot slot = slots[index];

		slot.enabled = false;
		//slot.collision = (GameObject)Instantiate(colliderBase, slot.position, Quaternion.identity);
		slots[index] = slot;
		slotsTaken++;
		freeSlots--;
	}

	public void FreeSlot(int index)
	{
		if (index != -1)
		{
			Slot slot = slots[index];
			slot.enabled = true;
			//Destroy(slot.collision);
			//slot.collision = null;
			slots[index] = slot;
			slotsTaken--;
			freeSlots++;
		}
	}

	public int GetSlotIndex(CubilinePlayerController.PLACE place, Vector3 slot)
	{
		int slotsPerFace = (int)arenaSize * (int)arenaSize;

		if (place == CubilinePlayerController.PLACE.FRONT)
		{
			if (slot.x > arenaPlaceLimit || slot.x < -arenaPlaceLimit || slot.y > arenaPlaceLimit || slot.y < -arenaPlaceLimit) return -1;
			int index = (int)((arenaPlaceLimit - slot.y) * arenaSize + (slot.x + arenaPlaceLimit));
			return index;
		}
		else if (place == CubilinePlayerController.PLACE.BACK)
		{
			if (slot.x > arenaPlaceLimit || slot.x < -arenaPlaceLimit || slot.y > arenaPlaceLimit || slot.y < -arenaPlaceLimit) return -1;
			int index = slotsPerFace + (int)((arenaPlaceLimit - slot.y) * arenaSize + (slot.x + arenaPlaceLimit));
			return index;
		}
		else if (place == CubilinePlayerController.PLACE.RIGHT)
		{
			if (slot.z > arenaPlaceLimit || slot.z < -arenaPlaceLimit || slot.y > arenaPlaceLimit || slot.y < -arenaPlaceLimit) return -1;
			int index = slotsPerFace * 2 + (int)((arenaPlaceLimit - slot.y) * arenaSize + (arenaPlaceLimit + slot.z));
			return index;
		}
		else if (place == CubilinePlayerController.PLACE.LEFT)
		{
			if (slot.z > arenaPlaceLimit || slot.z < -arenaPlaceLimit || slot.y > arenaPlaceLimit || slot.y < -arenaPlaceLimit) return -1;
			int index = slotsPerFace * 3 + (int)((arenaPlaceLimit - slot.y) * arenaSize + (arenaPlaceLimit + slot.z));
			return index;
		}
		else if (place == CubilinePlayerController.PLACE.TOP)
		{
			if (slot.z > arenaPlaceLimit || slot.z < -arenaPlaceLimit || slot.x > arenaPlaceLimit || slot.x < -arenaPlaceLimit) return -1;
			int index = slotsPerFace * 4 + (int)((arenaPlaceLimit - slot.z) * arenaSize + (arenaPlaceLimit + slot.x));
			return index;
		}
		else if (place == CubilinePlayerController.PLACE.BOTTOM)
		{
			if (slot.z > arenaPlaceLimit || slot.z < -arenaPlaceLimit || slot.x > arenaPlaceLimit || slot.x < -arenaPlaceLimit) return -1;
			int index = slotsPerFace * 5 + (int)((arenaPlaceLimit - slot.z) * arenaSize + (arenaPlaceLimit + slot.x));
			return index;
		}

		return -1;
	}
}