using UnityEngine;

public class CubilineGrabity : MonoBehaviour
{
	public bool nearAdditiveForce = false;
	public float nearAdditiveDistance = 100;
	public float grabityForce = 9.8f;
	public Transform targetPlanet;

	void Update ()
	{
		Vector3 direction = targetPlanet != null ? (targetPlanet.position - transform.position).normalized : - transform.position.normalized;
		float distance = targetPlanet != null ? (targetPlanet.position - transform.position).magnitude : -transform.position.magnitude;

		GetComponent<Rigidbody>().AddForce(direction * grabityForce * (nearAdditiveForce ? nearAdditiveDistance * distance : 1.0f));
	}
}
