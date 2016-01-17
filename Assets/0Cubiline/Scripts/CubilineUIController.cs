﻿using UnityEngine;
using UnityEngine.UI;

public class CubilineUIController : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public EaseScore scoreText;
	public EaseScore lengthText;
	public GameObject multiplerBar;
	public GameObject multiplerHolder;
	public GameObject multiplerText;
	public bool enableHorizontalDivision;
	public EaseImageOpasity horizontalDivision;
	public bool enableVerticalDivision;
	public EaseImageOpasity verticalDivision;
	public PlusScore plusScorePrefab;
	public PlusScore plusLengthPrefab;

	//////////////////////////////////////////////////////////////
	///////////////////////// PARAMETERS /////////////////////////
	//////////////////////////////////////////////////////////////

	public float timeToApear = 1.0f;

	public uint score { set { scoreText.score = value; } }
	public uint length { set { lengthText.score = value; } }
	public int multipler
	{
		set
		{
			if(value == 0)
			{
				multiplerBar.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
				multiplerHolder.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
				multiplerBar.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
				multiplerHolder.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
				multiplerText.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
			}
			else
			{
				multiplerBar.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
				multiplerHolder.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
				multiplerBar.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
				multiplerHolder.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
				multiplerText.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
				multiplerText.GetComponent<Text>().text = "x" + value.ToString();
			}
		}
	}

	public float multiplerTime
	{
		set
		{
			Vector3 barSize = new Vector3(value, 1, 1);
			multiplerBar.GetComponent<EaseScale>().inValues = barSize;
		}
	}

	public uint plusScore
	{
		set
		{
			if(value > 0)
			{
				PlusScore ps = Instantiate(plusScorePrefab);
				ps.transform.SetParent(transform);
				ps.text.text = "+" + value.ToString();
				ps.GetComponent<RectTransform>().offsetMax = Vector2.zero;
				ps.GetComponent<RectTransform>().offsetMin = Vector2.zero;
			}
		}
	}

	public int plusLength
	{
		set
		{
			PlusScore ps = Instantiate(plusLengthPrefab);
			ps.transform.SetParent(transform);
			if (value < 0)
				ps.text.text = value.ToString();
			else
				ps.text.text = "+" + value.ToString();
			ps.GetComponent<RectTransform>().offsetMax = Vector2.zero;
			ps.GetComponent<RectTransform>().offsetMin = Vector2.zero;
		}
	}

	//////////////////////////////////////////////////////////////
	///////////////////// CONTROL VARIABLES //////////////////////
	//////////////////////////////////////////////////////////////

	private float currentTime = 0;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////
	void Start()
	{
		if (enableVerticalDivision) verticalDivision.easeFace = EaseFloat.EASE_FACE.IN;
		if (enableHorizontalDivision) horizontalDivision.easeFace = EaseFloat.EASE_FACE.IN;
	}

	void Update ()
	{
		if (currentTime > timeToApear) return;
		currentTime += Time.deltaTime;
		if (currentTime >= timeToApear)
			ShowUI();
	}

	public void ShowUI()
	{
		scoreText.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
		scoreText.GetComponent<EaseTextOpasity>().easeFace = EaseTextOpasity.EASE_FACE.IN;

		lengthText.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
		lengthText.GetComponent<EaseTextOpasity>().easeFace = EaseTextOpasity.EASE_FACE.IN;
	}

	public void GoOut ()
	{
		scoreText.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
		scoreText.GetComponent<EaseTextOpasity>().easeFace = EaseTextOpasity.EASE_FACE.OUT;

		lengthText.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
		lengthText.GetComponent<EaseTextOpasity>().easeFace = EaseTextOpasity.EASE_FACE.OUT;

		multiplerBar.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		multiplerHolder.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		multiplerBar.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
		multiplerHolder.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
		multiplerText.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;

		if (enableVerticalDivision) verticalDivision.easeFace = EaseFloat.EASE_FACE.OUT;
		if (enableHorizontalDivision) horizontalDivision.easeFace = EaseFloat.EASE_FACE.OUT;
	}
}
