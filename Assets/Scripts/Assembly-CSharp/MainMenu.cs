using UnityEngine;

public class MainMenu : MonoBehaviour
{
	public Texture fon;

	public GUIStyle playStyle;

	public GUIStyle soundStyle;

	public GUIStyle bestScoreStyle;

	private GameCenterSingleton _gc;

	private void Start()
	{
		GlobalGameController.ResetParameters();
		GlobalGameController.Score = 0;
		_gc = GameCenterSingleton.Instance;
	}

	private void OnGUI()
	{
		GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), fon, ScaleMode.ScaleAndCrop);
		if (GUI.Button(new Rect((float)(Screen.width / 2) - playStyle.fixedWidth / 2f, (float)Screen.height * 2f / 3f - playStyle.fixedHeight / 2f, playStyle.fixedWidth, playStyle.fixedHeight), string.Empty, playStyle))
		{
			Application.LoadLevel("Loading");
		}
		float num = 15f;
		bool @bool = PlayerPrefsX.GetBool(PlayerPrefsX.SndSetting, true);
		@bool = GUI.Toggle(new Rect(num, (float)Screen.height - num - soundStyle.fixedHeight, soundStyle.fixedWidth, soundStyle.fixedHeight), @bool, string.Empty, soundStyle);
		AudioListener.volume = (@bool ? 1 : 0);
		PlayerPrefsX.SetBool(PlayerPrefsX.SndSetting, @bool);
		PlayerPrefs.Save();
		GUI.Box(new Rect((float)(Screen.width / 2) - bestScoreStyle.fixedWidth / 2f, (float)Screen.height * 0.85f - bestScoreStyle.fixedHeight / 2f, bestScoreStyle.fixedWidth, bestScoreStyle.fixedHeight), "BEST SCORE " + PlayerPrefs.GetInt(Defs.BestScoreSett, 0), bestScoreStyle);
	}
}
