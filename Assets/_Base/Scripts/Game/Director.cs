using UnityEngine;
using UnityEngine.SceneManagement;


public class Director : MonoBehaviour
{
	#region Variables
	public GameManager gameManager;
	public CameraManager cameraManager;
	//public Player player;
	public MapManager mapManager;
	public EntityManager entityManager;
	//public WavesManager waveManager;
	public InputManager inputManager;
	public UIManager uiManager;
	//public ScoreManager scoreManager;

	//public Transform playerTransform;

	public Structs.ViewMode currentViewMode { private set; get; }
	public Structs.GameMode currentGameMode { private set; get; }
	public Structs.GameScene currentScene;
	#endregion


	#region Singleton
	private static Director instance;

	public static Director Instance
	{
		get { return instance; }
	}

	static Director()
	{
		GameObject obj = GameObject.Find( "Director" );

		if( obj == null )
		{
			obj = new GameObject( "Director", typeof( Director ) );
		}

		instance = obj.GetComponent<Director>();
	}
	#endregion


	#region Monobehaviour
	private void Awake()
	{
		DontDestroyOnLoad( this.gameObject );
	}

	#endregion


	#region Scene management
	private void ChangeScene( Structs.GameScene to )
	{
		currentScene = to;

		switch( currentScene )
		{
			case Structs.GameScene.GAME_FIRST_TIME:

				mapManager.Init();
				InitMap();
				entityManager.Init();

				GameInitialize();
				break;

			case Structs.GameScene.GAME_INIT:

				InitPlayer();
				InitCamera();
				SetCameraOnPlayer();

				GameStart();
				break;

			case Structs.GameScene.GAME_RUNNING:

				inputManager.SetEvents();
				uiManager.UpdateUI();

				break;

			case Structs.GameScene.GAME_END:

				entityManager.Reset();

				break;

			case Structs.GameScene.GAME_LAST_TIME:
				Application.Quit();
				break;
		}
	}
	#endregion


	#region Game settings
	public void SetMode( Structs.ViewMode viewMode, Structs.GameMode gameMode )
	{
		currentViewMode = viewMode;
		currentGameMode = gameMode;
	}
	#endregion


	#region Game cycle
	public void EverythingBeginsHere()
	{
		ChangeScene( Structs.GameScene.GAME_FIRST_TIME );
	}

	public void GameInitialize()
	{
		ChangeScene( Structs.GameScene.GAME_INIT );
	}

	private void GameStart()
	{
		ChangeScene( Structs.GameScene.GAME_RUNNING );
	}

	public void GameEnd()
	{
		ChangeScene( Structs.GameScene.GAME_END );
	}

	public void GameRestart()
	{
		ChangeScene( Structs.GameScene.MAIN_MENU );
	}


	private void PauseGame()
	{
		Time.timeScale = 0.0f;
	}

	private void UnpauseGame()
	{
		Time.timeScale = 1.0f;
	}
	#endregion


	#region DEBUG
	public void GenerateEnemy()
	{
		entityManager.CreateEntity( EntityManager.EntityTypes.ENEMY, mapManager.currentMapScript.enemyGenerator );
	}

	public void MapPrevious()
	{
		GameEnd();
		mapManager.LoadPreviousMap();
		GameInitialize();
	}

	public void MapNext()
	{
		GameEnd();
		mapManager.LoadNextMap();
		GameInitialize();
	}
	private void InitMap()
	{
		mapManager.LoadFirstMap();
	}


	private void InitPlayer()
	{
		entityManager.CreateEntity( EntityManager.EntityTypes.PLAYER, mapManager.currentMapScript.playerGenerator );
	}

	private void InitCamera()
	{
		cameraManager.SetTarget( entityManager.player.transform );
	}

	private void SetCameraOnPlayer()
	{
		entityManager.player.GetComponent<Player>().Configure( cameraManager.camera.transform );
	}

	public void PlayerMoveForward()
	{
		entityManager.player.GetComponent<Player>().MoveForward();
	}

	public void PlayerMoveBackward()
	{
		entityManager.player.GetComponent<Player>().MoveBackward();
	}

	public void PlayerMoveLeft()
	{
		entityManager.player.GetComponent<Player>().MoveLeft();
	}

	public void PlayerMoveRight()
	{
		entityManager.player.GetComponent<Player>().MoveRight();
	}

	public void PlayerJump()
	{
		entityManager.player.GetComponent<Player>().Jump();
	}


	#endregion

}
