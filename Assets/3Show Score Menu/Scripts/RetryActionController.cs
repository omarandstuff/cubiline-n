using UnityEngine;
using System.Collections;

public class RetryActionController : ActionContentController
{
	void Start()
	{
		SceneName = CubilineApplication.lastComment;
	}
}
