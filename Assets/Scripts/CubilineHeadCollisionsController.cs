using UnityEngine;
using System.Collections;

public class CubilineHeadCollisionsController : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public CubilineController cubilineController;

	void OnTriggerEnter(Collider other)
    {
		cubilineController.ColliderEnter(other);
	}
}
