using UnityEngine;
using UnityEngine.UI;

public class EaseMaterialOpasity : EaseFloat
{
	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		Reset();
	}

	void FixedUpdate()
	{
		UpdateEase();
		Color color = GetComponent<MeshRenderer>().material.color;
		color.a = currentValue;
		GetComponent<MeshRenderer>().material.color = color;
	}

	public override void Reset()
	{
		base.Reset();
		if (forceInitialFace)
		{
			Color color = GetComponent<MeshRenderer>().material.color;
			color.a = currentValue;
			GetComponent<MeshRenderer>().material.color = color;
		}
	}
}
