using System.Collections;
using UnityEngine;

public class BonusCreator : MonoBehaviour
{
	public GameObject[] bonusPrefabs;

	public float creationInterval = 10f;

	public float weaponCreationInterval = 30f;

	public GameObject[] weaponPrefabs;

	private int _lastWeapon = -1;

	private ArrayList bonuses = new ArrayList();

	private ArrayList _weapons = new ArrayList();

	private void Start()
	{
		StartCoroutine(AddBonus());
		StartCoroutine(AddWeapon());
	}

	private IEnumerator AddBonus()
	{
		while (true)
		{
			yield return new WaitForSeconds(creationInterval);
			Vector3 pos = new Vector3(0f - Defs.HalfLength + Random.Range(0f, 2f * Defs.HalfLength), 0.24f, 0f - Defs.HalfLength + Random.Range(0f, 2f * Defs.HalfLength));
			int type = Random.Range(0, 11);
			Object newBonus = (GameObject)Object.Instantiate(bonusPrefabs[(type >= 9) ? 1u : 0u], pos, Quaternion.identity);
			bonuses.Add(newBonus);
			if (bonuses.Count > 5)
			{
				Object.Destroy((GameObject)bonuses[0]);
				bonuses.RemoveAt(0);
			}
		}
	}

	private IEnumerator AddWeapon()
	{
		while (true)
		{
			yield return new WaitForSeconds(weaponCreationInterval);
			Vector3 pos = new Vector3(0f - Defs.HalfLength + Random.Range(0f, 2f * Defs.HalfLength), 0.24f, 0f - Defs.HalfLength + Random.Range(0f, 2f * Defs.HalfLength));
			int weaponNumber;
			do
			{
				int type = Random.Range(0, 11);
				weaponNumber = ((type >= 5) ? ((type < 8) ? 1 : 2) : 0);
			}
			while (weaponNumber == _lastWeapon);
			Object newBonus = (GameObject)Object.Instantiate(weaponPrefabs[weaponNumber], pos, (weaponNumber != 0) ? Quaternion.identity : Quaternion.Euler(270f, 90f, 0f));
			_weapons.Add(newBonus);
			if (_weapons.Count > 5)
			{
				Object.Destroy((GameObject)_weapons[0]);
				_weapons.RemoveAt(0);
			}
		}
	}
}
