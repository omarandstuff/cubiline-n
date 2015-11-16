using UnityEngine;
using UnityEngine.UI;

public class FinishMenuController : MonoBehaviour {

	public Image scoreHolder;
	public Text scoreText;
	public float smoothTime;


	public int score
	{
		set
		{
			targetScore = value + 0.8f;
		}
	}

	private float targetScore = 150.8f;
	private float currentScore;
	private float velocity;

	void Update ()
	{
		if (currentScore <= targetScore - 0.8f)
		{
			currentScore = Mathf.SmoothDamp(currentScore, targetScore, ref velocity, smoothTime);
			scoreText.text = ((int)currentScore).ToString();
		}
		else
		{
			//scoreHolder.GetComponent<EaseTransform>().inScale = new Vector3(0.2f, 0.2f, 0.2f);
		}
	}
}
