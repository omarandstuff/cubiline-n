using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CubilineVisualMusic : MonoBehaviour
{
	public Text nameText;
	public Text artistText;
	public Text albumText;
	public EasePosition container;

	public float showTime = 5.0f;

	private float currentTime;

	IEnumerator Start()
	{
		yield return new WaitForSeconds(showTime);
		container.easeFace = EaseVector3.EASE_FACE.OUT;
		nameText.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		artistText.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		albumText.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		Destroy(gameObject, 2.0f);
	}


}
