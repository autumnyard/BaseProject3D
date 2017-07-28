using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
	public enum MovementType
	{
		None = -1,
		AbsoluteWithoutRotation = 0,
		AbsoluteWithRotation,
		RelativeToCamera,
		MAX_VALUES
	}

	public enum Direction
	{
		UP = 0,
		DOWN,
		FORWARD,
		BACKWARD,
		LEFT,
		RIGHT
	}
	private float maxSpeed = 5f;

	//[Header( "Physical variables" ), Range( 1f, 50f ), SerializeField]
	private float speed = 30f;
	//[Range( 20f, 1000f ), SerializeField]
	private float jump = 350f;
	private bool onGround = true;

	private Rigidbody body;

	// Relative to camera
	private MovementType relativeToCamera;
	new private Transform camera;

	private void Awake()
	{
		if( body == null )
		{
			body = GetComponent<Rigidbody>();
		}
		if( body == null )
		{
			body = GetComponentInChildren<Rigidbody>();
		}
		if( body == null )
		{
			body = gameObject.AddComponent<Rigidbody>();
		}

	}

	void FixedUpdate()
	{

		//LimitVelocity1();
		LimitVelocity2();
	}

	private void LimitVelocity1()
	{

		if( body.velocity.magnitude > maxSpeed )
		{
			body.velocity = body.velocity.normalized * maxSpeed;
		}
	}

	private void LimitVelocity2()
	{
		Vector2 xzVel = new Vector2( body.velocity.x, body.velocity.z );
		if( xzVel.magnitude > maxSpeed )
		{
			xzVel = xzVel.normalized * maxSpeed;
			body.velocity = new Vector3( xzVel.x, body.velocity.y, xzVel.y );
		}
	}

	public void Move( Direction direction )
	{
		// Calculo la fuerza del impulso
		//float fuerza = movementContSpeed * forceMultiplier;

		Vector3 haciaDonde = GetDirection( direction ) * speed;

		//body.AddForce( haciaDonde, ForceMode.Impulse ); // lo de antes
		body.AddForce( haciaDonde, ForceMode.Force ); // lo de antes

		// This makes a non-physics slide
		//transform.Translate( haciaDonde );
	}

	public void Jump( Direction direction = PlayerMovement.Direction.UP )
	{
		if( onGround )
		{
			Vector3 haciaDonde = GetDirection( direction ) * jump;

			//body.AddForce( haciaDonde, ForceMode.VelocityChange ); // lo d eantes
			body.AddForce( haciaDonde, ForceMode.Force ); // lo d eantes
		}
	}

	private Vector3 GetDirection( Direction direction )
	{
		switch( relativeToCamera )
		{
			case MovementType.RelativeToCamera:
				Vector3 temp = Vector3.zero;

				switch( direction )
				{
					case Direction.UP:
						temp = new Vector3( 0f, transform.up.normalized.y, 0f );
						break;

					case Direction.DOWN:
						temp = new Vector3( 0f, -transform.up.normalized.y, 0f );
						break;

					case Direction.RIGHT:
						temp = camera.right.normalized;
						temp.y = 0f;
						break;

					case Direction.LEFT:
						temp = camera.right.normalized * -1;
						temp.y = 0f;
						break;

					case Direction.FORWARD:
						temp = camera.forward.normalized;
						temp.y = 0f;
						break;

					case Direction.BACKWARD:
						temp = camera.forward.normalized * -1;
						temp.y = 0f;
						break;
				}

				return temp;

			default:
			case MovementType.AbsoluteWithoutRotation:
				switch( direction )
				{
					case Direction.UP:
						return transform.up.normalized;
					case Direction.DOWN:
						return transform.up.normalized * -1;
					case Direction.RIGHT:
						return transform.right.normalized;
					case Direction.LEFT:
						return transform.right.normalized * -1;
					default:
					case Direction.FORWARD:
						return transform.forward.normalized;
					case Direction.BACKWARD:
						return transform.forward.normalized * -1;
				}
		}
	}

	#region Public

	public void SetCamera( Transform cam )
	{
		relativeToCamera = MovementType.RelativeToCamera;
		camera = cam;
	}
	#endregion
}
