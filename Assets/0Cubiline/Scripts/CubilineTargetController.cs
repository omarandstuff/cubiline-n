using UnityEngine;
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
	public bool enableCommonTargets = true;
	public bool enableSpecialTargets = true;

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

	private uint arenaSize;

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// SETUP ////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void Reset(float arena_size)
	{
		slotController = GetComponent<CubilineSlotController>();

		arenaSize = (uint)arena_size;

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

			if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE || gameKind == CubilinePlayerController.PLAYER_KIND.TUTORIAL)
				uiController.specialCommonTime = bigCurrentTime / specialCommonTime;
			else if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE_COOP)
				coopUIController.specialCommonTime = bigCurrentTime / specialCommonTime;

			if (bigCurrentTime <= 0.0f)
			{
				commonTargetCount = gameKind == CubilinePlayerController.PLAYER_KIND.TUTORIAL ? 0u : 6u;
				bigCurrentTime = 0.0f;

				if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE || gameKind == CubilinePlayerController.PLAYER_KIND.TUTORIAL)
					uiController.specialCommon = false;
				else if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE_COOP)
					coopUIController.specialCommon = false;

				AchievementsData.diceCheck2 = false;
				AchievementsData.blackdiceCheck2 = false;
			}
		}

		if(enableCommonTargets)
		{
			while (commonTargets.Count < commonTargetCount && slotController.freeSlots > 0)
				spawnCommon();

			while (commonTargets.Count > commonTargetCount)
			{
				Dictionary<int, TargetInf>.Enumerator enumer = commonTargets.GetEnumerator();
				enumer.MoveNext();
				DismissCommon(enumer.Current.Value.slotIndex);
			}
		}
		if(enableSpecialTargets)
		{
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
			foreach (Collider cl in target.inGameObject.GetComponents<Collider>())
				cl.enabled = false;
			target.inGameObject.GetComponent<CubilineTarget>().targetScale = Vector3.zero;
			target.inGameObject.GetComponent<CubilineTarget>().pingPong = false;
			Destroy(target.inGameObject, 1.0f);
			slotController.FreeSlot(target.slotIndex);
		}
	}

	public void ApplyBigBlue()
	{
		commonTargetCount = arenaSize * (arenaSize / 4);

		bigCurrentTime = specialCommonTime;

		if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE || gameKind == CubilinePlayerController.PLAYER_KIND.TUTORIAL)
			uiController.specialCommon = true;
		else if (gameKind == CubilinePlayerController.PLAYER_KIND.ARCADE_COOP)
			coopUIController.specialCommon = true;

		AchievementsData.diceCheck2 = true;
		AchievementsData.blackdiceCheck2 = true;
	}

	public void ApplyMagnet(Transform activator)
	{
		magnetTarget = activator;
		int index = 0;
		foreach (KeyValuePair<int, TargetInf> ti in commonTargets)
		{
			ti.Value.inGameObject.GetComponent<CubilineGrabity>().targetPlanet = magnetTarget;
			ti.Value.inGameObject.GetComponent<Rigidbody>().isKinematic = false;
			ti.Value.inGameObject.GetComponent<Rigidbody>().AddExplosionForce(arenaSize * 1000, Vector3.zero, arenaSize);
			if (index++ == 100) break;
		}
		foreach (TargetInf ti in specialTargetInfs)
		{
			if (ti.inGameObject != null)
			{
				ti.inGameObject.GetComponent<CubilineGrabity>().targetPlanet = magnetTarget;
				ti.inGameObject.GetComponent<Rigidbody>().isKinematic = false;
				ti.inGameObject.GetComponent<Rigidbody>().AddExplosionForce(arenaSize * 1000, Vector3.zero, arenaSize);
			}
		}

		AchievementsData.diceCheck3 = true;
		AchievementsData.blackdiceCheck3 = true;
		CubilineApplication.singleton.CheckDiceLevelAchievement();
		CubilineApplication.singleton.CheckBlackDiceLevelAchievement();
		AchievementsData.diceCheck3 = false;
		AchievementsData.blackdiceCheck3 = false;
	}

}
