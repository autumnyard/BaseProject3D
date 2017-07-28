using UnityEngine;
using System.Collections.Generic;


public class EntityManager : MonoBehaviour
{
	#region Variables
	public enum EntityTypes
	{
		NONE = -1,
		PLAYER = 0,
		ENEMY,
		MAX_VALUES
	}

	[SerializeField]
	private GameObject playerPrefab = null;
	[SerializeField]
	private GameObject enemyPrefab = null;
	private GameObject[] entityPrefabs = new GameObject[(int)EntityTypes.MAX_VALUES];

	// Player
	public GameObject player { private set; get; }

	// Enemies
	private const int maxEnemyNumber = 32;
	private List<GameObject> enemies;

	#endregion


	#region Monobehaviour
	private void Awake()
	{
		Director.Instance.entityManager = this;

		entityPrefabs[(int)EntityTypes.PLAYER] = playerPrefab;
		entityPrefabs[(int)EntityTypes.ENEMY] = enemyPrefab;
	}
	#endregion


	#region Player
	private void InstantiatePlayer( Transform position )
	{
		player = Instantiate( entityPrefabs[(int)EntityTypes.PLAYER], position.position, Quaternion.identity, transform ) as GameObject;
	}

	public void RemovePlayer()
	{
		if( player != null )
		{
			Destroy( player );
			player = null;
		}
	}
	#endregion


	#region Enemy
	private void InstantiateEnemy( Transform position )
	{
		if( enemies.Count < enemies.Capacity )
		{
			var temp = Instantiate( entityPrefabs[(int)EntityTypes.ENEMY], position.position, Quaternion.identity, transform ) as GameObject;

			// TODO: Better id system.
			int id = Random.Range( 1, 99999 );
			temp.name = temp.name + id.ToString();
			enemies.Add( temp );
		}
		else
		{
			//Debug.Log("Max enemy quantity limit reached.");
		}
	}

	public void RemoveEnemy( GameObject which )
	{
		DestroyEntity( which );
		enemies.Remove( which );
	}

	#endregion


	#region Entity management
	public void Init()
	{
		enemies = new List<GameObject>( maxEnemyNumber );
	}

	public void CreateEntity( EntityTypes type, Transform position )
	{

		if( type == EntityManager.EntityTypes.PLAYER )
		{
			InstantiatePlayer( position );
		}
		else
		{
			InstantiateEnemy( position );
		}
	}

	private void DestroyEntity( GameObject which )
	{
		Destroy( which );
	}

	public void Reset()
	{
		if( enemies != null )
		{
			for( int i = 0; i < enemies.Count; i++ )
			{
				//enemies[i].GetComponent<EntityBase>().Die();
				RemoveEnemy( enemies[i] );
			}
			enemies.Clear();
		}

		RemovePlayer();
	}
	#endregion
}
