using UnityEngine;
using System.Collections;

public class ColliderEnterSound : MonoBehaviour
{

	void OnTriggerEnter(Collider other)
	{
		other.GetComponent<AudioSource>().Play();
	}
}
