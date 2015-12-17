﻿using UnityEngine;
using System.Collections;

public class MiniDemoPlayerController : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public RectTransform line1;
	public RectTransform line2;

	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////

	public float speed
	{
		get
		{
			return _speed;
		}
		set
		{
			_speed = value;
			Calculate();
		}
	}

	public float size
	{
		get
		{
			return _size;
		}
		set
		{
			_size = value;
			Calculate();
		}

	}

	public bool hard;

	private float _speed = 5;
	private float _size = 10;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	private float actualSpeed;
	private float actualPos1;
	private float actualPos2;
	private float fixedPos1;
	private float fixedPos2;

	private RectTransform current;
	private RectTransform secondary;

	void Start ()
	{
		actualPos1 = -1.5f;
		actualPos2 = 3;
		current = line1;
		secondary = line2;
		line1.localPosition = new Vector3(actualPos1, 0.0f, 0.0f);
		line2.localPosition = new Vector3(actualPos2, 0.0f, 0.0f);
		Calculate();
	}

	void Update ()
	{
		actualPos1 += Time.deltaTime * actualSpeed;
		actualPos2 += Time.deltaTime * actualSpeed;

		if(hard)
		{

		}
		else
		{
			fixedPos1 = actualPos1;
			fixedPos2 = actualPos2;
		}

		current.localPosition = new Vector3(fixedPos1, 0.0f, 0.0f);
		secondary.localPosition = new Vector3(fixedPos2, 0.0f, 0.0f);
	}

	private void Calculate()
	{
		actualSpeed = ((_size / _speed) * 6.0f) / _size;
		float unit = 6.0f / _size;
		float fixedWidth = unit * 1.5f;
		line1.localScale = new Vector3(fixedWidth, unit, 0.0f);
	}
}
