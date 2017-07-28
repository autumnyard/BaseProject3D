using UnityEngine;
using UnityEngine.UI;


public class PanelBase : MonoBehaviour
{
	private CanvasRenderer canvas;

	private void Awake()
	{
		canvas = GetComponent<CanvasRenderer>();

		if( canvas == null )
		{
			Debug.LogWarning( "UI menu without Unity Canvas: " + name );
		}
	}

	public void Show()
	{
		gameObject.SetActive( true );
		//canvas.SetAlpha( 1f );
	}

	public void Hide()
	{
		//canvas.SetAlpha( 0f );
		gameObject.SetActive( false );
	}
}
