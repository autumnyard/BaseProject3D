using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
	#region Variables
	#endregion


	#region Monobehaviour
	void Awake()
	{
		Director.Instance.gameManager = this;
	}

	private void Start()
	{
		Director.Instance.EverythingBeginsHere();
	}
	#endregion



}
