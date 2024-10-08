﻿using UnityEngine;
using System.Collections;

public class CubilineTarget : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public GameObject particlePrefab;
	public bool touchBody = true;

	//////////////////////////////////////////////////////////////
	///////////////////////// PARAMETERS /////////////////////////
	//////////////////////////////////////////////////////////////

	public string targetTag;
	public bool activated = false;
	public Transform activator;
	public int index;
	public int toGrow = 1;
	public int score = 1;
	public Vector3 targetScale;
	public bool pingPong = false;
	public float pinpongMagnitude = 0.2f;
	public float scaleTime = 0.3f;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	private Vector3 pingPongTargetScale;
	private Vector3 scaleVelocity = Vector3.zero;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR ////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		transform.localScale = Vector3.zero;
		pingPongTargetScale = targetScale;
	}

	void FixedUpdate()
	{
		if(pingPong)
		{
			float scalePoned = targetScale.x - Mathf.PingPong(Time.time, pinpongMagnitude);
			pingPongTargetScale = new Vector3(scalePoned, scalePoned, scalePoned);
		}
		else
		{
			pingPongTargetScale = targetScale;
		}

		if (transform.localScale != pingPongTargetScale) transform.localScale = Vector3.SmoothDamp(transform.localScale, pingPongTargetScale, ref scaleVelocity, scaleTime);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" || (other.tag == "Finish" && touchBody))
		{
			Destroy(gameObject, 1.0f);
			GetComponent<CubilineTarget>().targetScale = Vector3.zero;
			GetComponent<CubilineTarget>().pingPong = false;

			if (GetComponent<ControlAchievement>() != null)
				GetComponent<ControlAchievement>().Check();

			if(CubilineApplication.singleton.settings.particles) Destroy(Instantiate(particlePrefab, transform.position, Quaternion.identity), 8.0f);

			foreach (Collider c in GetComponents<Collider>())
				c.enabled = false;
			if(GetComponent<MeshRenderer>() != null) GetComponent<MeshRenderer>().enabled = false;
			GetComponent<AudioSource>().Play();
		}
	}
}
