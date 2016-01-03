using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
	public Image[] images;
	public Text[] texts;

	public void GoOut()
	{
		foreach(Image image in images)
		{
			image.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
			image.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		}
		foreach (Text text in texts)
		{
			text.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
			text.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		}
	}

	public void GoIn()
	{
		foreach (Image image in images)
		{
			image.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
			image.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
		}
		foreach (Text text in texts)
		{
			text.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.IN;
			text.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
		}
	}


}
