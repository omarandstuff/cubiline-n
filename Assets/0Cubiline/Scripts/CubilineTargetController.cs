﻿using UnityEngine;
using System.Collections.Generic;

public class CubilineTargetController : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public GameObject commonTargetBase;
	public SpecialTarget[] specialTargets;
	public CubilineUIController uiController;
	public CubilineCoopUIController coopUIController;

	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////

	public CubilinePlayerController.PLAYER_KIND gameKind;
	public uint commonTargetCount = 1;
	public float specialCommonTime = 10.0f;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	private CubilineSlotController slotController;

	[System.Serializable]
	public struct SpecialTarget { public GameObject specialTargetBase; public float minimumWaitingTime; public float maximumWaitingTime; public float inShowTime; }
	private struct TargetInf { public bool waiting; public float selectedRandomTime, currentTime, showTime; public GameObject inGameObject; public int slotIndex; public CubilinePlayerController.PLACE place; }

	private Dictionary<int, TargetInf> commonTargets = new Dictionary<int, TargetInf>(); // List of common tagets in the arena.
	private TargetInf[] specialTargetInfs;

	private float bigCurrentTime;
	private Transform magnetTarget;

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
		if (bigCurrentTime > 0)
		{
			bigCurrentTime -= Time.deltaTime;

			if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE)
				uiController.specialCommonTime = bigCurrentTime / specialCommonTime;
			else if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE_COOP)
				coopUIController.specialCommonTime = bigCurrentTime / specialCommonTime;

			if (bigCurrentTime <= 0.0f)
			{
				commonTargetCount = 6;
				bigCurrentTime = 0.0f;

				if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE)
					uiController.specialCommon = false;
				else if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE_COOP)
					coopUIController.specialCommon = false;
			}
		}

		while (commonTargets.Count < commonTargetCount && slotController.freeSlots > 0)
			spawnCommon();

		while (commonTargets.Count > commonTargetCount)
		{
			Dictionary<int, TargetInf>.Enumerator enumer = commonTargets.GetEnumerator();
			enumer.MoveNext();
			DismissCommon(enumer.Current.Value.slotIndex);
		}

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
		if (!commonTargets.ContainsKey(index)) return;
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
		newTarget.place = slot.place;

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
		specialTargetInfs[index].place = slot.place;

		specialTargetInfs[index].inGameObject.transform.parent = transform;
	}

	void TakeOutTarget(TargetInf target)
	{
		if (target.inGameObject != null)
		{
			if(target.inGameObject.GetComponent<CubilineTarget>().activated)
			{
				if (target.inGameObject.GetComponent<CubilineTarget>().targetTag == "Big Target")
				{
					commonTargetCount = CubilineApplication.cubeSize * (CubilineApplication.cubeSize / 4);
					bigCurrentTime = specialCommonTime;

					if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE)
						uiController.specialCommon = true;
					else if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE_COOP)
						coopUIController.specialCommon = true;
				}
				else if (target.inGameObject.GetComponent<CubilineTarget>().targetTag == "Magnet")
				{
					magnetTarget = target.inGameObject.GetComponent<CubilineTarget>().activator;
					int index = 0;
					foreach (KeyValuePair<int, TargetInf> ti in commonTargets)
					{
						ti.Value.inGameObject.GetComponent<CubilineGrabity>().targetPlanet = magnetTarget;
						ti.Value.inGameObject.GetComponent<Rigidbody>().isKinematic = false;
						ti.Value.inGameObject.GetComponent<Rigidbody>().AddExplosionForce(CubilineApplication.cubeSize * 1000, Vector3.zero, CubilineApplication.cubeSize);
						if (index++ == 100) break;
					}
					foreach (TargetInf ti in specialTargetInfs)
					{
						if(ti.inGameObject != null)
						{
							ti.inGameObject.GetComponent<CubilineGrabity>().targetPlanet = magnetTarget;
							ti.inGameObject.GetComponent<Rigidbody>().isKinematic = false;
							ti.inGameObject.GetComponent<Rigidbody>().AddExplosionForce(CubilineApplication.cubeSize * 1000, Vector3.zero, CubilineApplication.cubeSize);
						}
						
					}
				}
			}
			Destroy(target.inGameObject, 1.0f);
			target.inGameObject.GetComponent<CubilineTarget>().targetScale = Vector3.zero;
			target.inGameObject.GetComponent<CubilineTarget>().pingPong = false;
			slotController.FreeSlot(target.slotIndex);
		}
	}

}
