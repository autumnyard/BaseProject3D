using UnityEngine;
#if VR
using Valve.VR;
using HTC.UnityPlugin.Vive;
#endif

public class InputManager : MonoBehaviour
{
#region Variables

	// Traditional input
	public delegate void Delegate();
	public Delegate OnMouseClick;
	public Delegate OnKeyboardSpace;
	public Delegate OnKeyboardEnter;

	public Delegate OnKeyboard1;
	public Delegate OnKeyboard2;
	public Delegate OnKeyboard3;

	public Delegate OnKeyboardW;
	public Delegate OnKeyboardA;
	public Delegate OnKeyboardS;
	public Delegate OnKeyboardD;

#if VR
	// VR input
	public SteamVR_TrackedObject right;

	public Delegate OnVRClick;
	public Delegate OnVRClickDown;
	public Delegate OnVRTouchpadUp;
	public Delegate OnVRTouchpadCenter;
	public Delegate OnVRTouchpadDown;
#endif


	private enum MyMouse
	{
		NONE,
		Left,
		LeftMaintain,
		Middle,
		Right,
		Wheel_up,
		Wheel_down,
		Wheel_click,
		Lateral_1,
		Lateral_2,
		MAX_VALUES
	}

	private enum MyKeyboard
	{
		NONE,
		Space,
		Enter,
		W, A, S, D,
		Key1,
		Key2,
		Key3,
		MAX_VALUES
	}

	public Delegate[] OnMouse = new Delegate[(int)MyMouse.MAX_VALUES];
	public Delegate[] OnKeyboard = new Delegate[(int)MyKeyboard.MAX_VALUES];
#endregion


#region Monobehaviour
	void Awake()
	{
		Director.Instance.inputManager = this;
	}

	void LateUpdate()
	{
		CheckInput();
	}
#endregion


#region Input calling
	private void CheckInput()
	{
#if VR
		// TODO: Begin checking when VR input has eben initialized already
		if( right.index > 0 )
		{
			// VR Click trigger
			bool condition = ViveInput.GetPressDown( HandRole.RightHand, ControllerButton.Trigger );
			CallDelegate( OnVRClickDown, condition );

			// VR maintain trigger
			bool condition2 = ViveInput.GetPress( HandRole.RightHand, ControllerButton.Trigger );
			CallDelegate( OnVRClick, condition2 );

			// VR Wheel up
			Vector2 touchpad = ViveInput.GetPadTouchAxis( HandRole.RightHand );
			if( touchpad.y != 0 )
			{
				CallDelegate( OnVRTouchpadUp, (touchpad.y > 0.6) );
				CallDelegate( OnVRTouchpadCenter, (touchpad.y <= 0.6 && touchpad.y >= -0.6) );
				CallDelegate( OnVRTouchpadDown, (touchpad.y < -0.6) );
			}
		}
#endif

		CallDelegate( OnMouse[(int)MyMouse.LeftMaintain], Input.GetMouseButton( 0 ) );
		CallDelegate( OnMouse[(int)MyMouse.Left], Input.GetMouseButtonDown( 0 ) );
		//CallDelegate( OnMouse[(int)MyMouse.Wheel_up], Input.GetAxis( "Mouse ScrollWheel" ) );
		//CallDelegate( OnMouse[(int)MyMouse.Wheel_down], Input.GetMouseButton( 2 ) );
		CallDelegate( OnKeyboard[(int)MyKeyboard.W], Input.GetKeyDown( KeyCode.W ) );
		CallDelegate( OnKeyboard[(int)MyKeyboard.Key1], Input.GetKeyDown( KeyCode.Alpha1 ) );
		CallDelegate( OnKeyboard[(int)MyKeyboard.Key1], Input.GetKeyDown( KeyCode.Alpha1 ) );
		CallDelegate( OnKeyboard[(int)MyKeyboard.Key2], Input.GetKeyDown( KeyCode.Alpha2 ) );
		CallDelegate( OnKeyboard[(int)MyKeyboard.Key3], Input.GetKeyDown( KeyCode.Alpha3 ) );
	}

	private void CallDelegate( Delegate action, bool condition = true )
	{
		if( condition )
		{
			if( action != null )
			{
				action();
			}
		}
	}
#endregion


#region Input binding
	private void Bind( ref Delegate to, Delegate method )
	{
		to += method;
	}

	private void UnBind( ref Delegate to, Delegate method )
	{
		if( to != null )
		{
			to -= method;
		}
	}

	private void UnBindAll( ref Delegate from )
	{
		if( from != null )
		{
			foreach( Delegate d in from.GetInvocationList() )
			{
				UnBind( ref from, d );
			}
		}
	}

	private void UnBindAllEverything( ref Delegate[] froms )
	{
		for( int i = 0; i < froms.Length; i++ )
		{
			var from = froms[i];
			UnBindAll( ref from );
		}
	}
#endregion


#region Public
	public void SetEvents()
	{

#if VR
		UnBindAll( ref OnVRClick );
		UnBindAll( ref OnVRClickDown );
		UnBindAll( ref OnVRTouchpadDown );
		UnBindAll( ref OnVRTouchpadCenter );
		UnBindAll( ref OnVRTouchpadUp );
#endif
		UnBindAllEverything( ref OnMouse );
		UnBindAllEverything( ref OnKeyboard );

		switch( Director.Instance.currentScene )
		{
			case Structs.GameScene.GAME_RUNNING:
#if VR
				Bind( ref OnVRClick, Director.Instance.playerManager.ShootAutomatic );
				Bind( ref OnVRClickDown, Director.Instance.playerManager.Shoot );

				Bind( ref OnVRTouchpadUp, Director.Instance.playerManager.SelectWeapon1 );
				Bind( ref OnVRTouchpadCenter, Director.Instance.playerManager.SelectWeapon2 );
				Bind( ref OnVRTouchpadDown, Director.Instance.playerManager.SelectWeapon3 );

				//Bind( ref OnVRTouchpadDown, Director.Instance.playerManager.SelectPreviousWeapon );
				//Bind( ref OnVRTouchpadUp, Director.Instance.playerManager.SelectNextWeapon );
#else
				Bind( ref OnMouse[(int)MyMouse.Left], Director.Instance.playerManager.Shoot );
				Bind( ref OnMouse[(int)MyMouse.LeftMaintain], Director.Instance.playerManager.ShootAutomatic );
#endif
				Bind( ref OnKeyboard[(int)MyKeyboard.Key1], Director.Instance.playerManager.SelectWeapon1 );
				Bind( ref OnKeyboard[(int)MyKeyboard.Key2], Director.Instance.playerManager.SelectWeapon2 );
				Bind( ref OnKeyboard[(int)MyKeyboard.Key3], Director.Instance.playerManager.SelectWeapon3 );
				break;

			case Structs.GameScene.MAIN_MENU:
#if VR
				//Bind( ref OnVRClick, Director.Instance.GameInitialize );
				Bind( ref OnKeyboard[(int)MyKeyboard.Enter], Director.Instance.GameInitialize );
#else
				Bind( ref OnKeyboard[(int)MyKeyboard.Enter], Director.Instance.GameInitialize );
#endif
				break;


			case Structs.GameScene.GAME_RESULTS:
#if VR
				//Bind( ref OnVRClick, Director.Instance.Reset );
				Bind( ref OnKeyboard[(int)MyKeyboard.Enter], Director.Instance.GameInitialize );
#else
				Bind( ref OnKeyboard[(int)MyKeyboard.Enter], Director.Instance.GameReset );
#endif
				break;

			default:
				break;
		}
	}
#endregion
}
