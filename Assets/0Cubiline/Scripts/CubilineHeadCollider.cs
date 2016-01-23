using UnityEngine;
using System.Collections;

public class CubilineHeadCollider : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public CubilinePlayerController cubilineController;
	public bool imBody;

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Target" || other.tag == "Special Target" || other.tag == "Finish")
		{
			if (!imBody || (imBody && other.GetComponent<CubilineTarget>().touchBody))
				cubilineController.ColliderEnter(other);
		}
	}
}
