public class GlobalGameController
{
	private static int NumOfLevels = 3;

	private static int _currentLevel = -1;

	private static int _allLevelsCompleted;

	private static int score;

	public static int currentLevel
	{
		get
		{
			return _currentLevel;
		}
		set
		{
			_currentLevel = value;
			if (_currentLevel >= NumOfLevels || _currentLevel < 0)
			{
				_currentLevel = 0;
				AllLevelsCompleted++;
			}
		}
	}

	public static int AllLevelsCompleted
	{
		get
		{
			return _allLevelsCompleted;
		}
		set
		{
			_allLevelsCompleted = value;
		}
	}

	public static int ZombiesInWave
	{
		get
		{
			return 4;
		}
	}

	public static int EnemiesToKill
	{
		get
		{
			return 50 + 20 * AllLevelsCompleted;
		}
	}

	public static int Score
	{
		get
		{
			return score;
		}
		set
		{
			score = value;
		}
	}

	public static void ResetParameters()
	{
		currentLevel = 0;
		AllLevelsCompleted = 0;
	}
}
