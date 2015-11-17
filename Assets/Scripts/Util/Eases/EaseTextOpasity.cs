using UnityEngine;
using UnityEngine.UI;

public class EaseTextOpasity : EaseFloat
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
		Color color = GetComponent<Text>().color;
		color.a = currentValue;
		GetComponent<Text>().color = color;
	}

	public override void Reset()
	{
		base.Reset();
		if (forceInitialFace)
		{
			Color color = GetComponent<Text>().color;
			color.a = currentValue;
			GetComponent<Text>().color = color;
		}
	}
}
