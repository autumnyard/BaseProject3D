using UnityEngine;
using System.Collections;

public class PanelUI : PanelBase {

	public void ButtonNext()
	{
		Director.Instance.MapNext();
	}

	public void ButtonPrevious()
	{
		Director.Instance.MapPrevious();
	}
}
