using UnityEngine;
using System.Collections;


public class EntityBase : MonoBehaviour
{
	#region Variables
	protected enum States
	{
		INIT = 0,
		APPEARING,
		NORMAL,
		HURTING,
		DYING,
		DEAD,
		MAX_VALUES
	}

	protected enum Animations
	{
		APPEAR = 0,
		IDLE,
		HURTING,
		DYING,
		MAX_VALUES
	}

	public delegate void HurtDelegateMethod( int damage );
	public HurtDelegateMethod OnDamaged;

	protected EntityManager.EntityTypes type;

	protected States currentState { private set; get; }

	private Animator animator;
	new private Rigidbody rigidbody;
	new private Collider collider;
	new private Renderer renderer;

	private bool isInvulnerable;
	private const uint invulnerabilityFrames = 5u;

	protected int health;
	protected const int healthMax = 20;

	private float timeAlive;

	protected int damage = 5;
	#endregion


	#region Monobehaviour
	private void Awake()
	{
		if( animator == null )
		{
			animator = GetComponent<Animator>();
		}
		if( animator == null )
		{
			animator = GetComponentInChildren<Animator>();
		}
		if( animator == null )
		{
			Debug.LogWarning( "Animator wasn't setted in " + this.gameObject.name );
		}

		if( rigidbody == null )
		{
			rigidbody = transform.GetComponent<Rigidbody>();
		}
		if( rigidbody == null )
		{
			rigidbody = transform.GetComponentInChildren<Rigidbody>();
		}
		if( rigidbody == null )
		{
			Debug.LogWarning( "Rigidbody wasn't setted in " + this.gameObject.name );
		}

		if( renderer == null )
		{
			renderer = GetComponent<Renderer>();
		}
		if( renderer == null )
		{
			renderer = GetComponentInChildren<Renderer>();
		}
		if( renderer == null )
		{
			Debug.LogWarning( "Renderer wasn't setted in " + this.gameObject.name );
		}

		if( collider == null )
		{
			collider = GetComponent<Collider>();
		}
		if( collider == null )
		{
			collider = GetComponentInChildren<Collider>();
		}
		if( collider == null )
		{
			Debug.LogWarning( "Collider wasn't setted in " + this.gameObject.name );
		}

	}

	private void Start()
	{
		ChangeState( States.INIT );
	}

	private void Update()
	{
		UpdateTimeAlive( Time.deltaTime );

		if( currentState == States.NORMAL )
		{
			AiCheckOnUpdate();
		}
	}
	#endregion


	#region Entity management
	protected void ChangeState( States to )
	{
		currentState = to;
		RunState();
	}

	protected void RunState()
	{
		switch( currentState )
		{
			case States.INIT:
				timeAlive = 0.0f;
				health = healthMax;
				ChangeState( States.APPEARING );
				break;

			case States.APPEARING:
				SetRender( true );
				PlayAnimation( Animations.APPEAR ); // This triggers the OnAppearAnimationEnd event
				break;

			case States.NORMAL:
				SetCollisions( true );
				SetDamageTrigger( true );
				PlayAnimation( Animations.IDLE );
				AiStart();
				break;

			case States.HURTING:
				SetCollisions( false );
				SetDamageTrigger( false );
				PlayAnimation( Animations.HURTING );
				AiCheckOnHurt();
				ChangeState( States.NORMAL );
				break;

			case States.DYING:
				SetCollisions( false );
				SetDamageTrigger( false );
				PlayAnimation( Animations.DYING ); // This triggers the OnDyingAnimationEnd event
				break;

			case States.DEAD:
				Die();
				break;
		}
	}

	protected void Hurt( int damage )
	{
		if( !isInvulnerable )
		{
			// Play a few frames of invulnerability
			StartCoroutine( Invulnerability() );

			health -= damage;

			if( health <= 0 )
			{
				ChangeState( States.DYING );
			}
			else
			{
				ChangeState( States.HURTING );
			}
		}
	}

	public virtual void Die()
	{
	}
	#endregion


	#region AI
	protected virtual void AiStart() { }

	protected virtual void AiCheckOnUpdate() { }

	protected virtual void AiCheckOnHurt() { }
	#endregion


	#region Events
	private void OnAppearAnimationEnd()
	{
		ChangeState( States.NORMAL );
	}

	private void OnDyingAnimationEnd()
	{
		ChangeState( States.DEAD );
	}
	#endregion


	#region Helpers
	protected void PlayAnimation( Animations to )
	{
		if( animator == null )
		{
			return;
		}

		switch( to )
		{
			case Animations.APPEAR:
				iTween.ScaleTo( gameObject, iTween.Hash(
					"scale", Vector3.one * 1,
					"time", 1f,
					"easetype", iTween.EaseType.easeOutElastic,
					"oncomplete", "OnAppearAnimationEnd"
					) );
				break;

			case Animations.IDLE:
				break;

			case Animations.HURTING:
				break;

			case Animations.DYING:
				iTween.ScaleTo( gameObject, iTween.Hash(
					"scale", Vector3.zero,
					"time", 1f,
					"easetype", iTween.EaseType.easeOutElastic,
					"oncomplete", "OnDyingAnimationEnd"
					) );
				break;
		}
	}

	private IEnumerator Invulnerability()
	{
		isInvulnerable = true;
		for( uint i = 0; i < invulnerabilityFrames; i++ )
		{
			yield return new WaitForEndOfFrame();
		}
		isInvulnerable = false;
	}

	protected void SetCollisions( bool to )
	{
		if( collider != null )
		{
			collider.enabled = to;
		}
	}

	private void SetDamageTrigger( bool to )
	{
		if( to == true )
		{
			if( OnDamaged == null )
			{
				OnDamaged += Hurt;
			}
		}
		else
		{
			if( OnDamaged != null )
			{
				OnDamaged -= Hurt;
			}
		}
	}

	protected void SetRender( bool to )
	{
		if( renderer != null )
		{
			renderer.enabled = to;
		}
	}

	protected void UpdateTimeAlive( float increment )
	{
		timeAlive += increment;
	}
	#endregion


	#region COLLISIONS

	void OnTriggerExit( Collider col )
	{
		if( col.gameObject.CompareTag( "Boundary" ) )
		{
			Die();
		}
	}
	#endregion

}
