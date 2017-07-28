using UnityEngine;


public class UIManager : MonoBehaviour
{

	[Header( "Components" ),SerializeField]
	private PanelBase panelMenu;
	[SerializeField]
	private PanelBase panelGame;
	[SerializeField]
	private PanelBase panelHUD;


	#region Monobehaviour
	void Awake()
	{
		Director.Instance.uiManager = this;
	}
	#endregion


	#region Panel management
	public void UpdateUI()
	{
		switch( Director.Instance.currentScene )
		{
			case Structs.GameScene.GAME_RUNNING:
				panelMenu.Hide();
				panelGame.Show();
				panelHUD.Hide();
				break;

			default:
				panelMenu.Hide();
				panelGame.Hide();
				panelHUD.Hide();
				break;
		}
	}
	#endregion


	#region Helpers
	#endregion
}
