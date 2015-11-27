using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
	public Slider slider;

	public void SliderChange()
	{
		GetComponent<Text> ().text = ((int)slider.value).ToString("D2");
	}
}
