using UnityEngine;
using System.Collections.Generic;

public class CubilineTargetController : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public GameObject commonTargetBase;
	public SpecialTarget[] specialTargets;

	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////

	public uint commonTargetCount = 1;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	private CubilineSlotController slotController;

	[System.Serializable]
	public struct SpecialTarget { public GameObject specialTargetBase; public float minimumWaitingTime; public float maximumWaitingTime; public float inShowTime; }
	private struct TargetInf { public bool waiting; public float selectedRandomTime, currentTime, showTime; public GameObject inGameObject; public int slotIndex; }

	private Dictionary<int, TargetInf> commonTargets = new Dictionary<int, TargetInf>(); // List of common tagets in the arena.
	private TargetInf[] specialTargetInfs;

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// SETUP ////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void Reset(float arenaSize)
	{
		slotController = GetComponent<CubilineSlotController>();

		for (int i = 0; i < commonTargets.Count; i++)
		{
			CubilineTarget target = commonTargets[i].inGameObject.GetComponent<CubilineTarget>();
			target.targetScale = Vector3.zero;
			Destroy(commonTargets[i].inGameObject, 1.0f);
		}

		commonTargets.Clear();

		if (specialTargetInfs == null)
			specialTargetInfs = new TargetInf[specialTargets.Length];

		for (int i = 0; i < specialTargets.Length; i++)
		{
			specialTargetInfs[i].showTime = specialTargets[i].inShowTime * arenaSize;
			DismissSpecial(i);
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////// TARGETS ///////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void ManageTargets()
	{
		while (commonTargets.Count != commonTargetCount && slotController.freeSlots > 0)
			spawnCommon();

		while (commonTargets.Count > commonTargetCount)
			DismissCommon(commonTargets.Count - 1);

		for (int i = 0; i < specialTargets.Length; i++)
		{
			if (specialTargetInfs[i].waiting)
			{
				specialTargetInfs[i].currentTime += Time.deltaTime;
				if (specialTargetInfs[i].currentTime >= specialTargetInfs[i].selectedRandomTime)
				{
					specialTargetInfs[i].waiting = false;
					specialTargetInfs[i].currentTime = 0.0f;
					spawnSpecial(i);
				}
			}
			else
			{
				specialTargetInfs[i].currentTime += Time.deltaTime;
				if (specialTargetInfs[i].currentTime >= specialTargetInfs[i].showTime)
				{
					DismissSpecial(i);
				}
			}
		}
	}

	public void DismissCommon(int index)
	{
		TakeOutTarget(commonTargets[index]);
		commonTargets.Remove(index);
	}

	public void DismissSpecial(int index)
	{
		specialTargetInfs[index].waiting = true;
		specialTargetInfs[index].currentTime = 0.0f;
		specialTargetInfs[index].selectedRandomTime = Random.value * (specialTargets[index].maximumWaitingTime - specialTargets[index].minimumWaitingTime) + specialTargets[index].minimumWaitingTime;
		TakeOutTarget(specialTargetInfs[index]);
		specialTargetInfs[index].inGameObject = null;
	}

	public void spawnCommon()
	{
		TargetInf newTarget = new TargetInf();
		CubilineSlotController.Slot slot = slotController.GetFreeSlot();

		if (slot.position == Vector3.zero) return;

		slotController.TakeSlotAt(slot.index);

		newTarget.slotIndex = slot.index;
		newTarget.inGameObject = (GameObject)Instantiate(commonTargetBase, slot.position, Quaternion.identity);
		newTarget.inGameObject.GetComponent<CubilineTarget>().index = slot.index;

		newTarget.inGameObject.transform.parent = transform;

		commonTargets.Add(slot.index, newTarget);
	}

	public void spawnSpecial(int index)
	{
		CubilineSlotController.Slot slot = slotController.GetFreeSlot();

		if (slot.position == Vector3.zero)
		{
			DismissSpecial(index);
			return;
		}

		slotController.TakeSlotAt(slot.index);

		specialTargetInfs[index].slotIndex = slot.index;
		specialTargetInfs[index].inGameObject = (GameObject)Instantiate(specialTargets[index].specialTargetBase, slot.position, Quaternion.identity);

		specialTargetInfs[index].inGameObject.transform.parent = transform;
	}

	void TakeOutTarget(TargetInf target)
	{
		if (target.inGameObject != null)
		{
			Destroy(target.inGameObject, 1.0f);
			target.inGameObject.GetComponent<CubilineTarget>().targetScale = Vector3.zero;
			slotController.FreeSlot(target.slotIndex);
		}
	}

}
