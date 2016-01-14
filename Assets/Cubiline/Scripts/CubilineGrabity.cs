using UnityEngine;

public class CubilineGrabity : MonoBehaviour
{
	public float grabityForce = 9.8f;
	public Transform targetPlanet;

	void Update ()
	{
		Vector3 direction = targetPlanet != null ? (targetPlanet.position - transform.position).normalized : - transform.position.normalized;

		GetComponent<Rigidbody>().AddForce(direction * grabityForce);
	}
}
