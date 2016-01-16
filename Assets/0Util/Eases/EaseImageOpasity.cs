using UnityEngine;
using UnityEngine.UI;

public class EaseImageOpasity : EaseFloat
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
		Color color = GetComponent<Image>().color;
		color.a = currentValue;
		GetComponent<Image>().color = color;
	}

	public override void Reset()
	{
		base.Reset();
		if (forceInitialFace)
		{
			Color color = GetComponent<Image>().color;
			color.a = currentValue;
			GetComponent<Image>().color = color;
		}
	}
}
