using UnityEngine;

public class ActionContentController : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////
	public MenuController parentMenu;
	public GameObject smallContentPrefab;
	public GameObject bigContentPrefab;

	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////

	public enum CONTENT_TYPE { TO_SCENE, LOAD_SIDE, SMALL_CONTENT, BIG_CONTENT};
	public CONTENT_TYPE contentType;
	public string SceneName;
	public int sideIndex;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	private GameObject smallContent;
	private GameObject bigContent;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Awake()
	{
		if (smallContentPrefab != null)
		{
			smallContent = Instantiate(smallContentPrefab) as GameObject;
			smallContent.transform.SetParent(transform);
			smallContent.transform.localPosition = Vector3.zero;
			smallContent.transform.localRotation = transform.localRotation;
			smallContent.GetComponent<SmallActionController>().parentAction = this;
		}
	}

	public void Enter()
	{
		if(smallContent != null) smallContent.GetComponent<SmallActionController>().Enter();
	}

	public void Leave()
	{
		if (smallContent != null) smallContent.GetComponent<SmallActionController>().Leave();
	}

	public void Select()
	{
		if (smallContent != null) smallContent.GetComponent<SmallActionController>().Select();
		if (contentType == CONTENT_TYPE.BIG_CONTENT)
		{
			bigContent = Instantiate(bigContentPrefab);
			Destroy(smallContent, 1.0f);
		}
	}

	public void Unselect()
	{
		parentMenu.BackFromAction();
	}
}
