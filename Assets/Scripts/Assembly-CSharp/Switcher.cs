using UnityEngine;

public class Switcher : MonoBehaviour
{
	public Texture fon;

	public Texture fonLevel1;

	public Texture fonLevel2;

	public Texture fonLevel3;

	public bool NoWait;

	public bool isGameOver;

	private Texture fonToDraw;

	private void Start()
	{
		switch (GlobalGameController.currentLevel)
		{
		case -1:
			fonToDraw = fon;
			break;
		case 0:
			fonToDraw = fonLevel1;
			break;
		case 1:
			fonToDraw = fonLevel2;
			break;
		case 2:
			fonToDraw = fonLevel3;
			break;
		default:
			fonToDraw = fon;
			break;
		}
		if (NoWait)
		{
			LoadMenu();
		}
		else
		{
			Invoke("LoadMenu", 2.5f);
		}
	}

	private void Method()
	{
	}

	private void OnGUI()
	{
		Rect position = new Rect(0f, 0f, Screen.width, Screen.height);
		GUI.DrawTexture(position, fonToDraw, ScaleMode.ScaleAndCrop);
	}

	private void LoadMenu()
	{
		if (isGameOver)
		{
			Application.LoadLevel("Restart");
			return;
		}
		string text;
		switch (GlobalGameController.currentLevel)
		{
		case -1:
			text = "Restart";
			break;
		case 0:
			text = "level1_prison";
			break;
		case 1:
			text = "Level2_maze";
			break;
		case 2:
			text = "Level3_city";
			break;
		default:
			text = "Restart";
			break;
		}
		GlobalGameController.currentLevel++;
		Application.LoadLevel(text);
	}
}
