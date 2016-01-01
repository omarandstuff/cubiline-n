using UnityEngine;
using UnityEngine.UI;

public class CheckNamesScript : MonoBehaviour
{
	public CubilineCoopController coop;
	public GameObject rootOfThis;
	public InputField player1Input;
	public InputField player2Input;
	public EaseFloat[] transparencies;
	public EaseScale quad;

	void Start()
	{
		if (CubilineApplication.player1Name != "")
		{
			coop.Play();
			Destroy(rootOfThis);
		}
	}

	public void Check()
	{
		if (player1Input.text == "" || player2Input.text == "") GetComponent<Button>().interactable = false; else GetComponent<Button>().interactable = true;
	}

	public void DoNames()
	{
		CubilineApplication.player1Name = player1Input.text;
		CubilineApplication.player2Name = player2Input.text;

		foreach(EaseFloat ef in transparencies)
		{
			ef.easeFace = EaseFloat.EASE_FACE.OUT;
		}

		quad.easeFace = EaseVector3.EASE_FACE.OUT;


		Destroy(rootOfThis, 1.0f);
	}

}
