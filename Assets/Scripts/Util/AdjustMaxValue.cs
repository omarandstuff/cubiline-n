using UnityEngine;
using UnityEngine.UI;

public class AdjustMaxValue : MonoBehaviour
{
	public Slider reference;

	void Start()
	{
		Changemax();
	}

	public void Changemax()
	{
		GetComponent<Slider>().maxValue = (int)reference.value / 2;
	}
	
}
