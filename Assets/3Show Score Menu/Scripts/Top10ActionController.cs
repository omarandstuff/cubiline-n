using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Xml.Linq;

public class Top10ActionController : ActionContentController
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public ScrollRect scrollView;
	public Button okButton;
	public EaseImageOpasity buttonBack;
	public EaseTextOpasity buttonText;
	public Text[] names;
	public Text[] scores;

	//////////////////////////////////////////////////////////////
	///////////////////// CONTROL VARIABLES //////////////////////
	//////////////////////////////////////////////////////////////

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		StartCoroutine(LoadScores());
	}

	public override void Select()
	{
		scrollView.enabled = true;
		buttonBack.easeFace = EaseFloat.EASE_FACE.IN;
		buttonText.easeFace = EaseFloat.EASE_FACE.IN;
		okButton.enabled = true;
	}

	public override void Unselect()
	{
		scrollView.enabled = false;
		buttonBack.easeFace = EaseFloat.EASE_FACE.OUT;
		buttonText.easeFace = EaseFloat.EASE_FACE.OUT;
		okButton.enabled = false;
		base.Unselect();
	}

	public override void Enter()
	{

	}

	public override void Leave()
	{
	}

	IEnumerator LoadScores()
	{
		// Create a download object.
		WWW download = new WWW("https://cubiline.com/MGODS");

		// Wait until the download is done
		yield return download;

		if (!string.IsNullOrEmpty(download.error))
		{
			print("Error Geting Scores: " + download.error);
			names[0].text = download.error;
		}
		else
		{
			XDocument xdoc = XDocument.Parse(download.text);
			var rootCategory = xdoc.Root;

			int i = 0;
			foreach (XElement score in rootCategory.Elements())
			{
				names[i].text = (string)score.Element("player");
				scores[i++].text = (string)score.Element("score");
			}
		}
	}
}
