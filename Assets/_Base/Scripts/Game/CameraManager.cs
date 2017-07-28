using UnityEngine;


public class CameraManager : MonoBehaviour
{
	new public MainCamera camera;

	#region Monobehaviour
	void Awake()
	{
		Director.Instance.cameraManager = this;
	}
	#endregion

	public void SetTarget( Transform target )
	{
		camera.target = target;
	}
}
