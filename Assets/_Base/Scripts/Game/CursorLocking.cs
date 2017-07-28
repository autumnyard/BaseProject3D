using UnityEngine;

public class CursorLocking : MonoBehaviour
{
	public static bool isLocked;

	private void Update()
	{
		isLocked = Cursor.lockState == CursorLockMode.Locked;

		if( Director.Instance.currentScene == Structs.GameScene.GAME_RUNNING )
		{

			if( Input.GetMouseButtonDown( 0 ) )
			{
				Lock();
			}

			if( Input.GetKeyDown( KeyCode.Escape ) )
			{
				Unlock();
			}
		}
		else if( Director.Instance.currentScene == Structs.GameScene.GAME_INIT )
		{
			Lock();
		}
		else
		{
			Unlock();
		}
	}

	public void Lock()
	{
		Debug.Log( "LOCK" );
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void Unlock()
	{
		Debug.Log( "UNLOCK" );
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
}
