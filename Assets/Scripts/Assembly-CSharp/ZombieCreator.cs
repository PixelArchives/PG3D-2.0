using System.Collections;
using UnityEngine;

public class ZombieCreator : MonoBehaviour
{
	public GameObject[] teleports;

	public Transform[] zombiePrefabs;

	private int _numOfLiveZombies;

	private int _numOfDeadZombies;

	private int _numOfDeadZombsSinceLastFast;

	public float curInterval = 10f;

	public int NumOfLiveZombies
	{
		get
		{
			return _numOfLiveZombies;
		}
		set
		{
			_numOfLiveZombies = value;
		}
	}

	public int NumOfDeadZombies
	{
		get
		{
			return _numOfDeadZombies;
		}
		set
		{
			int num = value - _numOfDeadZombies;
			_numOfDeadZombies = value;
			NumOfLiveZombies -= num;
			_numOfDeadZombsSinceLastFast += num;
			if (_numOfDeadZombsSinceLastFast == 12)
			{
				if (curInterval > 5f)
				{
					curInterval -= 5f;
				}
				_numOfDeadZombsSinceLastFast = 0;
			}
			if (_numOfDeadZombies >= GlobalGameController.EnemiesToKill)
			{
				GameObject[] array = teleports;
				foreach (GameObject gameObject in array)
				{
					gameObject.SetActive(true);
				}
			}
		}
	}

	private void Start()
	{
		curInterval = Mathf.Max(1f, curInterval - (float)GlobalGameController.AllLevelsCompleted);
		StartCoroutine(AddZombies());
	}

	private void Update()
	{
	}

	private IEnumerator AddZombies()
	{
		int numOfZombsToAdd = GlobalGameController.ZombiesInWave;
		float halfLLength = 17f;
		float radius = 2.5f;
		float[] ys = new float[3] { 1f, 0.5f, 2.5f };
		do
		{
			numOfZombsToAdd = Mathf.Min(numOfZombsToAdd, GlobalGameController.EnemiesToKill - (NumOfDeadZombies + NumOfLiveZombies));
			ArrayList points = new ArrayList();
			for (int j = 0; j < numOfZombsToAdd; j++)
			{
				points.Add(new Vector3(0f - halfLLength + Random.Range(0f, radius), 1f, 0f - radius + Random.Range(0f, 2f * radius)));
				points.Add(new Vector3(0f - radius + Random.Range(0f, 2f * radius), 1f, halfLLength - Random.Range(0f, radius)));
				points.Add(new Vector3(halfLLength - Random.Range(0f, radius), 1f, 0f - radius + Random.Range(0f, 2f * radius)));
				points.Add(new Vector3(0f - radius + Random.Range(0f, 2f * radius), 1f, 0f - halfLLength + Random.Range(0f, radius)));
			}
			for (int i = 0; i < numOfZombsToAdd; i++)
			{
				int typeOfZomb = Random.Range(0, 3);
				float y = ys[typeOfZomb];
				Object.Instantiate(zombiePrefabs[typeOfZomb], new Vector3(((Vector3)points[i])[0], y, ((Vector3)points[i])[2]), Quaternion.identity);
			}
			yield return new WaitForSeconds(curInterval);
		}
		while (NumOfDeadZombies + NumOfLiveZombies < GlobalGameController.EnemiesToKill);
	}
}
