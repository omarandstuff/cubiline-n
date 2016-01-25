using UnityEngine;

public class CameraFacingBillboard : MonoBehaviour
{
	private Camera m_Camera;

	void Update()
	{
		m_Camera = Camera.main;
		transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward, m_Camera.transform.rotation * Vector3.up);
	}
}