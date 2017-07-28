using UnityEngine;

public class Player : EntityBase
{
	#region Variables
	private PlayerMovement movementHandler;
	#endregion


	#region Monobehaviour
	void Awake()
	{
		if( movementHandler == null )
		{
			movementHandler = GetComponent<PlayerMovement>();
		}
	}
	#endregion


	#region Private
	#endregion


	#region Public
	public void Configure( Transform camera )
	{
		movementHandler.SetCamera( camera );
	}

	public override void Die()
	{
		base.Die();
		Director.Instance.GameEnd();
	}
	#endregion


	#region Input
	public void MoveLeft()
	{
		movementHandler.Move( PlayerMovement.Direction.LEFT );
	}

	public void MoveRight()
	{
		movementHandler.Move( PlayerMovement.Direction.RIGHT );
	}

	public void MoveForward()
	{
		movementHandler.Move( PlayerMovement.Direction.FORWARD );
	}

	public void MoveBackward()
	{
		movementHandler.Move( PlayerMovement.Direction.BACKWARD );
	}

	public void Jump()
	{
		movementHandler.Jump();
	}
	#endregion
}
