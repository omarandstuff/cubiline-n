using UnityEngine;
using UnityEngine.UI;

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

	public bool hard
	{
		get
		{
			return _hard;
		}
		set
		{
			_hard = value;
		}
	}

	public Color color
	{
		set
		{
			line1.GetComponent<Image>().color = value;
			line2.GetComponent<Image>().color = value;
		}
	}

	private bool _hard;
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

	void Start ()
	{
		actualPos1 = -1.5f;
		actualPos2 = 3;
		line1.localPosition = new Vector3(actualPos1, 0.0f, 0.0f);
		line2.localPosition = new Vector3(actualPos2, 0.0f, 0.0f);
		Calculate();
	}

	void Update ()
	{
		actualPos1 += Time.deltaTime * actualSpeed;

		float delta = actualPos1 - (_size / 2.0f - 1.5f);

		if (delta >= 0)
			actualPos1 = -size / 2.0f - 1.5f + delta;

		actualPos2 = actualPos1 + _size;

		if (hard)
		{
			fixedPos1 = Mathf.Floor(actualPos1);
			fixedPos2 = Mathf.Floor(actualPos2);
		}
		else
		{
			fixedPos1 = actualPos1;
			fixedPos2 = actualPos2;
		}

		line1.localPosition = new Vector3(fixedPos1 * 3.0f / _size, 0.0f, 0.0f);
		line2.localPosition = new Vector3(fixedPos2 * 3.0f / _size, 0.0f, 0.0f);
	}

	private void Calculate()
	{
		actualSpeed = speed;
		float unit = 6.0f / _size;
		float fixedWidth = unit * 1.5f;
		line1.localScale = new Vector3(fixedWidth, unit, 0.0f);
		line2.localScale = new Vector3(fixedWidth, unit, 0.0f);
	}
}
