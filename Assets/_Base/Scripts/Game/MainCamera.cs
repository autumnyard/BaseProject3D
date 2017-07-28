using UnityEngine;


public class MainCamera : MonoBehaviour
{
	public enum Type
	{
		//FixedPoint,
		//FixedAxis,
		//FixedPlane,
		RelativeOffset,
		RelativeFixedAxis,
		OrbitXAxis,
		OrbitFree
	}

	#region Variables

	[Header( "Common settings" )]
	public Type type;
	new private Transform camera;
	[HideInInspector]
	public Transform target;

	// Optional settings
	[SerializeField]
	private bool cursorLocking = true;
	private CursorLocking cursorLock;

	// Relative offset
	float distance = 3.0f;
	float height = 3.0f;
	float damping = 5.0f;
	bool smoothRotation = false;
	float rotationDamping = 10.0f;

	// Relative fixed axis
	private Vector3 axisOffset = new Vector3( 0f, 6f, -6f );
	private float speed = 10f;

	// Orbit X axis
	private Vector3 mouseOffset = Vector3.zero;
	private Vector3 mouseOffsetInit = new Vector3( 0f, 4f, 4f );
	private float mouseSpeed = 3f;

	// Orbit free
	[Header( "Orbit free" ), SerializeField]
	private Vector2 orbitSpeed = new Vector2( 20f, 20f ); // Default is 20f 20f
	[SerializeField]
	private float minHeight = 10f; // Default is 10f
	[SerializeField]
	private float maxHeight = 50f; // Default is 50f

	private float distanceMin = .5f;
	private float distanceMax = 15f;

	private float orbitDeleceration = 6f;

	float horizontalMov = 0.0f;
	float verticalMov = 0.0f;

	private Vector2 orbitAcceleration = Vector2.zero;
	//float orbitXAcceleration = 0.0f;
	//float orbitYAcceleration = 0.0f;


	#endregion


	#region Monobehaviour
	void Start()
	{
		if( camera == null )
		{
			camera = transform;
		}

		if( cursorLock == null )
		{
			cursorLock = GetComponent<CursorLocking>();
		}
		if( cursorLock == null )
		{
			cursorLock = GetComponentInChildren<CursorLocking>();
		}


		// Initialization
		switch( type )
		{
			case Type.OrbitFree:
				Vector3 angles = transform.eulerAngles;
				horizontalMov = angles.y;
				verticalMov = angles.x;
				break;

			case Type.OrbitXAxis:
				mouseOffset = mouseOffsetInit;
				break;
		}

	}

	void FixedUpdate()
	{
		if( camera != null && target != null )
		{
			switch( type )
			{
				case Type.RelativeOffset:
					CameraFixedOffset();
					break;

				case Type.RelativeFixedAxis:
					CameraFixedAxisSmoother();
					break;


				case Type.OrbitXAxis:
					MouseSmoother();
					break;

				case Type.OrbitFree:
					MouseSmoothedFree();
					break;

			}
		}
	}
	#endregion


	#region Cameras
	private void CameraFixedOffset()
	{
		Vector3 wantedPosition = target.TransformPoint( 0, height, -distance );
		camera.position = Vector3.Lerp( camera.position, wantedPosition, Time.deltaTime * damping );

		if( smoothRotation )
		{
			Quaternion wantedRotation = Quaternion.LookRotation( target.position - camera.position, target.up );
			camera.rotation = Quaternion.Slerp( camera.rotation, wantedRotation, Time.deltaTime * rotationDamping );
		}
		else
		{
			camera.LookAt( target.transform, target.up );
		}
	}

	private void CameraFixedAxisSmoother()
	{

		Vector3 targetPos = target.position;
		Vector3 offset = axisOffset;

		float cameraAngle = camera.eulerAngles.y;
		float targetAngle = target.eulerAngles.y;

		// Prevent the camera from rotating 180 degrees when moving backwards
		if( Input.GetAxisRaw( "Vertical" ) < 0.2f )
		{
			targetAngle = cameraAngle;
		}

		targetAngle = Mathf.LerpAngle( cameraAngle, targetAngle, speed * Time.deltaTime );
		offset = Quaternion.Euler( 0, targetAngle, 0 ) * offset;

		camera.position = Vector3.Lerp( camera.position, targetPos + offset, speed * Time.deltaTime );
		camera.LookAt( targetPos );
	}

	private void MouseSmoother()
	{
		mouseOffset = Quaternion.AngleAxis( Input.GetAxis( "Mouse X" ) * mouseSpeed, Vector3.up ) * mouseOffset;

		//if( type == Type.MouseSmoothedFree )
		//{
		//	This WIP code was still WIP when commented. Very WIP.
		//	float ySpan = Input.GetAxis( "Mouse Y" );
		//	//mouseOffset = Quaternion.AngleAxis( Input.GetAxis( "Mouse Y" ) * mouseSpeed , Vector3.right ) * mouseOffset;
		//	rotationXAxis = ClampAngle( rotationXAxis, minHeight, maxHeight );
		//	mouseOffset = Quaternion.AngleAxis( ySpan * mouseSpeed, Vector3.right ) * mouseOffset;
		//	//Debug.Log( mouseOffset );

		//}

		transform.position = target.position + mouseOffset;
		transform.LookAt( target.position );
	}

	private void MouseSmoothedFree()
	{
		// Horizontal movement
		orbitAcceleration.x += orbitSpeed.x * Input.GetAxis( "Mouse X" ) * 0.02f;
		horizontalMov += orbitAcceleration.x;

		// Vertical movement
		orbitAcceleration.y += orbitSpeed.y * Input.GetAxis( "Mouse Y" ) * 0.02f;
		verticalMov -= orbitAcceleration.y;

		// Limit height
		verticalMov = ClampAngle( verticalMov, minHeight, maxHeight );

		//Quaternion fromRotation = Quaternion.Euler( transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0 );
		Quaternion toRotation = Quaternion.Euler( verticalMov, horizontalMov, 0 );
		Quaternion rotation = toRotation;

		Vector3 negDistance = new Vector3( 0.0f, 0.0f, -distance );
		Vector3 position = rotation * negDistance + target.position;

		camera.rotation = rotation;
		camera.position = position;

		orbitAcceleration.x = Mathf.Lerp( orbitAcceleration.x, 0, Time.deltaTime * orbitDeleceration );
		orbitAcceleration.y = Mathf.Lerp( orbitAcceleration.y, 0, Time.deltaTime * orbitDeleceration );
	}



	public static float ClampAngle( float angle, float min, float max )
	{
		if( angle < -360F )
		{
			angle += 360F;
		}
		if( angle > 360F )
		{
			angle -= 360F;
		}
		return Mathf.Clamp( angle, min, max );
	}
	#endregion



	#region Mouse
	public void CursorLock()
	{
		if( cursorLock != null && cursorLocking )
		{
			cursorLock.Lock();
		}
	}

	public void CursorUnlock()
	{
		if( cursorLock != null && cursorLocking )
		{
			cursorLock.Unlock();
		}
	}
	#endregion


	#region Forced movement
	public void OrbitLeft()
	{
	}

	public void OrbitRight()
	{
	}
	#endregion
}
