using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_move_c : MonoBehaviour
{
	public GUISkin MySkin;

	public Texture2D AimTexture;

	public int AimTextureWidth = 50;

	public int AimTextureHeight = 50;

	public Transform GunFlash;

	public Transform bullet;

	public int BulletForce = 5000;

	public int MaxAmmoCount = 10;

	public int CurAmmoCount = 10;

	public int InventoryAmmoCount = 120;

	private GameObject _leftJoystick;

	private int _curHealth;

	public int MaxHealth;

	private int DifferenceAmmo;

	public int AmmoBoxWidth = 100;

	public int AmmoBoxHeight = 100;

	public int AmmoBoxOffset = 10;

	public int ScoreBoxWidth = 100;

	public int ScoreBoxHeight = 100;

	public int ScoreBoxOffset = 10;

	private float GunFlashLifetime;

	public GUIStyle ScoreBox;

	public GUIStyle AmmoBox;

	public GUIStyle HealthBox;

	public GUIStyle pauseStyle;

	public GUIStyle pauseFonStyle;

	public GUIStyle resumeStyle;

	public GUIStyle menuStyle;

	public GUIStyle soundStyle;

	public GUIStyle buyStyle;

	private GameObject _fpc;

	public Texture HeartTexture;

	public GameObject damage;

	private bool damageShown;

	public Texture pauseFon;

	private Pauser _pauser;

	public Texture pauseTitle;

	public GameObject[] weapons;

	public int CurrentWeaponIndex;

	private GameObject _gameController;

	private WeaponSounds _currentWeaponSounds;

	private List<StoreKitProduct> _products = new List<StoreKitProduct>();

	private GameObject _inAppGameObject;

	private StoreKitEventListener _listener;

	public int CurHealth
	{
		get
		{
			return _curHealth;
		}
		set
		{
			_curHealth = value;
		}
	}

	public int curHealthProp
	{
		get
		{
			return CurHealth;
		}
		set
		{
			if (CurHealth > value && !damageShown)
			{
				StartCoroutine(FlashWhenHit());
			}
			CurHealth = value;
		}
	}

	private void WalkAnimation()
	{
		weapons[CurrentWeaponIndex].GetComponent<Animation>().CrossFade("Walk");
	}

	private void IdleAnimation()
	{
		weapons[CurrentWeaponIndex].GetComponent<Animation>().CrossFade("Idle");
	}

	public void ChangeWeapon(int newWeaponIndex, bool shouldSetMaxAmmo = true)
	{
		weapons[CurrentWeaponIndex].active = false;
		CurrentWeaponIndex = newWeaponIndex;
		weapons[CurrentWeaponIndex].active = true;
		MaxAmmoCount = WeaponsManager.ammoInClipForWeapon(CurrentWeaponIndex);
		if (shouldSetMaxAmmo)
		{
			InventoryAmmoCount = WeaponsManager.maxAmmoInClipForWeapon(CurrentWeaponIndex);
		}
		else
		{
			InventoryAmmoCount = WeaponsManager.InitialAmmoInClipForWeapon(CurrentWeaponIndex);
		}
		CurAmmoCount = MaxAmmoCount;
		weapons[CurrentWeaponIndex].GetComponent<Animation>()["Reload"].layer = 1;
		weapons[CurrentWeaponIndex].GetComponent<Animation>()["Shoot"].layer = 1;
		_currentWeaponSounds = weapons[CurrentWeaponIndex].GetComponent<WeaponSounds>();
	}

	public bool NeedAmmo()
	{
		return InventoryAmmoCount < WeaponsManager.maxAmmoInClipForWeapon(CurrentWeaponIndex);
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void Start()
	{
		NewInput.mouseLocked = true;
		_inAppGameObject = GameObject.FindGameObjectWithTag("InAppGameObject");
		_listener = _inAppGameObject.GetComponent<StoreKitEventListener>();
		if (StoreKitBinding.canMakePayments())
		{
			StoreKitManager.productListReceivedEvent += delegate(List<StoreKitProduct> allProducts)
			{
				Debug.Log("received total products: " + allProducts.Count);
				_products = allProducts;
			};
			string[] productIdentifiers = new string[1] { "bigammopack" };
			StoreKitBinding.requestProductData(productIdentifiers);
		}
		weapons[CurrentWeaponIndex].GetComponent<Animation>()["Reload"].layer = 1;
		weapons[CurrentWeaponIndex].GetComponent<Animation>().Stop();
		CurAmmoCount = MaxAmmoCount;
		GunFlash.gameObject.active = false;
		CurHealth = MaxHealth;
		_fpc = GameObject.FindGameObjectWithTag("FirstPersonControl");
		Color color = damage.GetComponent<GUITexture>().color;
		color.a = 0f;
		damage.GetComponent<GUITexture>().color = color;
		_gameController = GameObject.FindGameObjectWithTag("GameController");
		_pauser = GameObject.FindGameObjectWithTag("GameController").GetComponent<Pauser>();
		_leftJoystick = GameObject.Find("LeftTouchPad");
		ChangeWeapon(0, false);
	}

	private void ReloadPressed()
	{
		DifferenceAmmo = MaxAmmoCount - CurAmmoCount;
		if (InventoryAmmoCount > 0 && CurAmmoCount != MaxAmmoCount)
		{
			weapons[CurrentWeaponIndex].GetComponent<Animation>().Stop("Empty");
			weapons[CurrentWeaponIndex].GetComponent<Animation>().CrossFade("Shoot");
			weapons[CurrentWeaponIndex].GetComponent<Animation>().Play("Reload");
			if (InventoryAmmoCount >= DifferenceAmmo)
			{
				CurAmmoCount += DifferenceAmmo;
				InventoryAmmoCount -= DifferenceAmmo;
			}
			else
			{
				CurAmmoCount += InventoryAmmoCount;
				InventoryAmmoCount = 0;
			}
			base.GetComponent<AudioSource>().PlayOneShot(_currentWeaponSounds.reload);
			_leftJoystick.SendMessage("HasAmmo");
		}
	}

	private void ShotPressed()
	{
		if (weapons[CurrentWeaponIndex].GetComponent<Animation>().IsPlaying("Shoot") || weapons[CurrentWeaponIndex].GetComponent<Animation>().IsPlaying("Reload"))
		{
			return;
		}
		weapons[CurrentWeaponIndex].GetComponent<Animation>().Stop();
		if (CurAmmoCount > 0)
		{
			weapons[CurrentWeaponIndex].GetComponent<Animation>().Play("Shoot");
			_fpc.SendMessage("shootS");
			CurAmmoCount--;
			if (CurAmmoCount == 0)
			{
				_leftJoystick.SendMessage("NoAmmo");
			}
			base.GetComponent<AudioSource>().PlayOneShot(_currentWeaponSounds.shoot);
			GunFlash.gameObject.active = true;
			GunFlashLifetime = 0.1f;
		}
		else
		{
			weapons[CurrentWeaponIndex].GetComponent<Animation>().Play("Empty");
			base.GetComponent<AudioSource>().PlayOneShot(_currentWeaponSounds.empty);
		}
	}

	private IEnumerator Fade(float start, float end, float length, GameObject currentObject)
	{
		for (float i = 0f; i < 1f; i += Time.deltaTime / length)
		{
			Color rgba = currentObject.GetComponent<GUITexture>().color;
			rgba.a = Mathf.Lerp(start, end, i);
			currentObject.GetComponent<GUITexture>().color = rgba;
			yield return 0;
			Color rgba_ = currentObject.GetComponent<GUITexture>().color;
			rgba_.a = end;
			currentObject.GetComponent<GUITexture>().color = rgba_;
		}
	}

	private IEnumerator FlashWhenHit()
	{
		damageShown = true;
		Color rgba = damage.GetComponent<GUITexture>().color;
		rgba.a = 0f;
		damage.GetComponent<GUITexture>().color = rgba;
		float danageTime = 0.15f;
		yield return StartCoroutine(Fade(0f, 1f, danageTime, damage));
		yield return new WaitForSeconds(0.01f);
		yield return StartCoroutine(Fade(1f, 0f, danageTime, damage));
		damageShown = false;
	}

	private void Update()
	{
		if (NewInput.pause) SetPause();

		if (!_pauser.paused)
		{
			if (NewInput.mouseLocked && NewInput.Shooting(CurAmmoCount)) ShotPressed();
			if (NewInput.reload) ReloadPressed();
		}

		if (GunFlashLifetime > 0f)
		{
			GunFlashLifetime -= Time.deltaTime;
		}
		if (GunFlashLifetime <= 0f)
		{
			GunFlash.gameObject.active = false;
		}
		if (CurHealth <= 0)
		{
			if (GlobalGameController.Score > PlayerPrefs.GetInt(Defs.BestScoreSett, 0))
			{
				PlayerPrefs.SetInt(Defs.BestScoreSett, GlobalGameController.Score);
				PlayerPrefs.Save();
			}
			GameCenterSingleton.Instance.ReportScore(GlobalGameController.Score);
			Application.LoadLevel("GameOver");
		}
	}

	private void OnGUI()
	{
		GUI.DrawTexture(new Rect(Screen.width / 2 - AimTextureWidth / 2, Screen.height / 2 - AimTextureHeight / 2, AimTextureWidth, AimTextureHeight), AimTexture);
		GUI.skin = MySkin;
		Rect position = new Rect((float)Screen.width - AmmoBox.fixedWidth, 0f, 1f, 50f);
		GUI.Box(position, CurAmmoCount + "/" + InventoryAmmoCount, AmmoBox);
		Rect position2 = new Rect(0f, 0f, 1f, 50f);
		GUI.Box(position2, "SCORE " + GlobalGameController.Score, ScoreBox);
		float num = ((1 == 0) ? ScoreBox.fixedWidth : (ScoreBox.fixedWidth + (position.x - ScoreBox.fixedWidth) / 2f - HealthBox.fixedWidth / 2f));
		GUI.Box(new Rect(num, 0f, 1f, 1f), string.Empty, HealthBox);
		float num2 = 45.4f - (float)HeartTexture.width;
		float top = HealthBox.fixedHeight / 2f - (float)HeartTexture.height / 2f;
		for (int i = 0; i < CurHealth; i++)
		{
			GUI.DrawTexture(new Rect(num + num2 * 0.5f + (float)i * ((float)HeartTexture.width + num2), top, HeartTexture.width, HeartTexture.height), HeartTexture);
		}
		if (NewInput.isMobile && GUI.Button(new Rect(0f, HealthBox.fixedHeight, pauseStyle.normal.background.width, pauseStyle.fixedHeight), string.Empty, pauseStyle) && CurHealth > 0)
		{
			SetPause();
		}
		if (_products.Count > 0 && GUI.Button(new Rect(Screen.width - buyStyle.normal.background.width, HealthBox.fixedHeight, buyStyle.normal.background.width, buyStyle.fixedHeight), string.Empty, buyStyle))
		{
			StoreKitProduct storeKitProduct = _products[0];
			StoreKitManager.purchaseSuccessfulEvent += purchaseSuccessful;
			Debug.Log("preparing to purchase product: " + storeKitProduct.productIdentifier);
			StoreKitBinding.purchaseProduct(storeKitProduct.productIdentifier, 1);
		}
		if ((bool)_pauser && _pauser.paused)
		{
			Rect position3 = new Rect(0f, 0f, Screen.width, Screen.height);
			GUI.DrawTexture(position3, pauseFon, ScaleMode.ScaleAndCrop);
			float num3 = 15f;
			Vector2 vector = new Vector2(176f, 150f - num3);
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) + vector.x - (float)(pauseTitle.width / 2), (float)(Screen.height / 2) - vector.y - num3 - (float)pauseTitle.height, pauseTitle.width, pauseTitle.height), pauseTitle);
			if (GUI.Button(new Rect((float)(Screen.width / 2) + vector.x - resumeStyle.fixedWidth / 2f, (float)Screen.height / 2f - vector.y, resumeStyle.fixedWidth, resumeStyle.fixedHeight), string.Empty, resumeStyle))
			{
				SetPause();
			}
			if (GUI.Button(new Rect((float)(Screen.width / 2) + vector.x - menuStyle.fixedWidth / 2f, (float)Screen.height / 2f - vector.y + resumeStyle.fixedHeight + num3, menuStyle.fixedWidth, menuStyle.fixedHeight), string.Empty, menuStyle))
			{
				Time.timeScale = 1f;
				Application.LoadLevel("Restart");
			}
			float num4 = 15f;
			bool @bool = PlayerPrefsX.GetBool(PlayerPrefsX.SndSetting, true);
			@bool = GUI.Toggle(new Rect(num4, (float)Screen.height - num4 - soundStyle.fixedHeight, soundStyle.fixedWidth, soundStyle.fixedHeight), @bool, string.Empty, soundStyle);
			AudioListener.volume = (@bool ? 1 : 0);
			PlayerPrefsX.SetBool(PlayerPrefsX.SndSetting, @bool);
			PlayerPrefs.Save();
		}
	}

	private void SetPause()
	{
		_pauser.paused = !_pauser.paused;
		if (_pauser.paused)
		{
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = 1f;
		}

		NewInput.mouseLocked = !_pauser.paused;
	}

	private void purchaseSuccessful(StoreKitTransaction transaction)
	{
		Debug.Log("purchased product: " + transaction);
		_listener.ProvideContent();
		ChangeWeapon(CurrentWeaponIndex);
	}

	private void OnDestroy()
	{
		NewInput.mouseLocked = false;
	}
}
