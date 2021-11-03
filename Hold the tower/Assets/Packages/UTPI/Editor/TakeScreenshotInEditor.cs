using UnityEngine;
using UnityEditor;

public class TakeScreenshotInEditor : ScriptableObject
{
	public static string fileName = "Editor Screenshot ";
	public static int startNumber = 1;

	[MenuItem("Tools/Take Screenshot of Game View %^s")]
	static void TakeScreenshot()
	{
		if (!System.IO.Directory.Exists("Assets/ScreenShots"))
		{
			System.IO.Directory.CreateDirectory("Assets/ScreenShots");
		}

		int number = startNumber;
		string name = "" + number;

		while (System.IO.File.Exists("Assets/ScreenShots/" + fileName + name + ".png"))
		{
			number++;
			name = "" + number;
		}

		startNumber = number + 1;

		ScreenCapture.CaptureScreenshot("Assets/ScreenShots/" + fileName + name + ".png");

		AssetDatabase.Refresh();
		EditorApplication.RepaintHierarchyWindow();
	}
}