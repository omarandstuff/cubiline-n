using UnityEngine;
using System.Collections;

public class CubilineHeadCollider : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public CubilinePlayerController cubilineController;

	void OnTriggerEnter(Collider other)
	{
		cubilineController.ColliderEnter(other);
	}
}
