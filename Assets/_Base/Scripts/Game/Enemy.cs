using UnityEngine;
using System.Collections;

public class Enemy : EntityBase
{

	#region Public
	public override void Die()
	{
		base.Die();
		Director.Instance.entityManager.RemoveEnemy( this.gameObject );
	}
	#endregion

	void OnCollisionEnter( Collision col )
	{
		// This should be for enemies only
		if( col.gameObject.CompareTag( "Player" ) )
		{
			var script = col.gameObject.GetComponent<EntityBase>();
			if( script.OnDamaged != null )
			{
				script.OnDamaged( damage );
			}
		}
	}
}
