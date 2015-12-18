using UnityEngine;

public class CubillineStepController : MonoBehaviour
{
	public GameObject stepOnPrefab;

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			GetComponent<AudioSource>().Play();
			Destroy(Instantiate(stepOnPrefab, transform.position, transform.rotation), 1.0f);
		}
	}
}
