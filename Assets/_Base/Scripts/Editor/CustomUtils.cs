using UnityEditor;
using UnityEngine;

public class CustomUtils : EditorWindow
{

	[MenuItem( "Tools/Reset Playerprefs" )]

	public static void DeletePlayerPrefs()
	{
		PlayerPrefs.DeleteAll();
	}

}