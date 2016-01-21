using System;
using UnityEngine;
using UnityEngine.UI;

public class MiniSelectorFixer : MonoBehaviour
{
	public Text nameText;
	public Text leyendText;

	public Transform content;
	public int levelIndex
	{
		set
		{
			if (value == _levelIndex || value < 0 || value > CubilineApplication.singleton.levels.Length - 1) return;


			if(_levelIndex != -1)
				GetComponent<AudioSource>().Play(0);

			_levelIndex = value;

			float targetX = _levelIndex * -4.8f;
			targetPosition = new Vector3(targetX, 0.0f, 0.0f);

			if (unlocked[_levelIndex])
			{
				nameText.text = CubilineApplication.singleton.levels[_levelIndex].levelName;
				leyendText.text = "";
			}
				
			else
			{
				nameText.text = "?";
				leyendText.text = CubilineApplication.singleton.levels[_levelIndex].levelLeyend;
			}
		}
		get
		{
			return _levelIndex;
		}
	}
	public bool[] unlocked;

	private Vector3 targetPosition = Vector3.zero;
	private Vector3 velocity = Vector3.zero;
	private bool interactig;

	private int _levelIndex = -1;

	private bool inCommand;

	void Update ()
	{
		if(!interactig)
			content.localPosition = Vector3.SmoothDamp(content.localPosition, targetPosition, ref velocity, 0.1f);

		if (!Input.GetMouseButton(0) && Input.touchCount == 0)
		{
			interactig = false;
		}
			
	}

	public void Interacting()
	{
		if (inCommand)
		{
			if (Math.Abs(content.localPosition.x + _levelIndex * 4.8) < 0.3f)
			{
				inCommand = false;
				return;
			}
			else
			{
				interactig = false;
				return;
			}
		}
		interactig = true;
		levelIndex = -(int)Mathf.Round(content.localPosition.x / 4.8f);
	}

	public void SetLevelIndex(int index)
	{
		inCommand = true;
		interactig = false;
		levelIndex = index;
	}

	public void PlusLevel()
	{
		inCommand = true;
		interactig = false;
		levelIndex = (int)Mathf.Repeat((float)levelIndex + 1,  (float)CubilineApplication.singleton.levels.Length - 1);
	}

	public void MinusLevel()
	{
		inCommand = true;
		interactig = false;
		levelIndex = (int)Mathf.Repeat((float)levelIndex - 1, (float)CubilineApplication.singleton.levels.Length - 1);
	}

}
