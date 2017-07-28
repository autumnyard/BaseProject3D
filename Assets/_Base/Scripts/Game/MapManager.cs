using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour
{
	#region Variables

	private int currentMapId;
	[HideInInspector]
	public Map currentMapScript { private set; get; }
	[SerializeField]
	private GameObject[] map;
	public int maxMapNumber { private set; get; }

	//Internal management
	private GameObject loadedMap;

	#endregion


	#region Monobehaviour
	void Awake()
	{
		Director.Instance.mapManager = this;

	}
	#endregion



	#region Private
	private void InstantiateCurrentMap()
	{
		loadedMap = Instantiate( map[currentMapId] );
		currentMapScript = loadedMap.GetComponent<Map>();
	}

	private void DestroyCurrentMap()
	{
		if( loadedMap != null )
		{
			Destroy( loadedMap );
			loadedMap = null;
		}
	}
	#endregion


	#region Public
	public void Init()
	{
		maxMapNumber = map.Length;
	}

	private void LoadMap( int which )
	{
		Debug.Log( "LoadMap: " + which + "" );
		DestroyCurrentMap();
		InstantiateCurrentMap();
	}

	public void LoadFirstMap()
	{
		currentMapId = 0;
		LoadMap( currentMapId );
	}

	public void LoadPreviousMap()
	{
		currentMapId--;
		if( currentMapId < 0 )
		{
			Debug.LogWarning( "Couldn't load map number " + currentMapId );
			currentMapId = 0;
		}
		LoadMap( currentMapId );
	}

	public void LoadNextMap()
	{
		currentMapId++;
		if( currentMapId >= maxMapNumber )
		{
			Debug.LogWarning( "Couldn't load map number " + currentMapId );
			currentMapId = maxMapNumber - 1;
		}
		LoadMap( currentMapId );
	}
	#endregion

}
