using System;
using System.Collections;
using UnityEngine;

namespace AssemblyCSharp
{
	public enum TurretLocation { Top, Right, Left, Max };
	
	public class TurretManager : MonoBehaviour
	{
		private static TurretManager instance;
		
		public Transform[] turretList;
		public GameObject[] turretPrefabList;
		
		public Transform selectorRing = null;
		
		Transform currentTurret = null;
		TurretLocation currentTurretLocation = TurretLocation.Top;
		int turretCount = 0;
		
		bool isScalingSelector = false;
		float selectorScaleTime = 0.0f;
		float selectorScaleDuration = 0.25f;
		float selectorScaleSize = 2.2f;
		
		public TurretManager ()
		{
			
		}
		
		public static TurretManager Instance
		{
			get;
			private set;
		}
		
		public Transform CurrentTurret
		{
			get { return currentTurret; }	
		}
		
		public int CurrentTurretLocation
		{
			get { return CurrentTurretLocation; }	
		}
		
		public int TurretCount
		{
			get { return turretCount; }	
		}

		void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
			}
			
			Instance = this;	
			DontDestroyOnLoad(gameObject);
			
			if (turretList.Length == 0)
				return;
			
			turretCount = turretList.Length;
			currentTurret = turretList[0];

			CheatMenuGUI.OnCheatBtnDownEvent += OnCheatMenuBtnDown;
		}
		
		void Start()
		{
			Transform turret = null;
			bool isCurrentTurret = false;

			for (int i = 0; i < turretList.Length; i++)
			{
				turret = (Transform)turretList[i];
				isCurrentTurret = currentTurret == turret;

				GetTurretComponent(turret).IsSelected = isCurrentTurret;
				turret.GetComponent<TopDownAim>().enabled = isCurrentTurret;

				GetTurretComponent(CurrentTurret).OnSelectTurret();
			}
		}
		
		void Update()
		{
			GetInput();

			if (isScalingSelector)
			{
				selectorScaleTime += Time.deltaTime;
				
				float animationProgress = selectorScaleTime / selectorScaleDuration;
				
				if (animationProgress < 1.0f)
				{
					float currentSelectorScale = ((selectorScaleSize - 1.4f) * (1.0f - animationProgress)) + 1.4f;
					
					selectorRing.transform.localScale = new Vector3(currentSelectorScale, currentSelectorScale, 1.0f);
				}
				else
				{
					isScalingSelector = false;
					selectorRing.transform.localScale = new Vector3(1.4f, 1.4f, 1.0f);
				}
			}
		}
		
		public void changeTurretSelection(TurretLocation newLocation)
		{
			if (currentTurretLocation != newLocation)
			{
				if (currentTurret != null)
				{
					currentTurret.GetComponent<TopDownAim>().enabled = false;
					GetTurretComponent(currentTurret).OnDeselectTurret();
				}
				
				currentTurret = turretList[(int)newLocation];
				GetTurretComponent(CurrentTurret).OnSelectTurret();

				currentTurretLocation = newLocation;
				currentTurret.GetComponent<TopDownAim>().enabled = true;

				selectorRing.transform.position = currentTurret.transform.position;
				
				isScalingSelector = true;
				selectorScaleTime = 0.0f;
				selectorRing.transform.localScale = new Vector3(1.4f, 1.4f, 1.0f);
			}
		}

		public void OnCheatMenuBtnDown(CheatMenuButtonType buttonType, int buttonIndex, int miscInfo)
		{
			switch(buttonType)
			{
			case CheatMenuButtonType.Turret:
				ChangeTurretType((TurretLocation)miscInfo, (TurretType)buttonIndex);
				break;
			case CheatMenuButtonType.TurretModifier:
				ChangeTurretModifier((TurretLocation)miscInfo, (TurretType)buttonIndex);
				break;
			}
		}

		public void ChangeTurretType(TurretLocation location, TurretType newType)
		{
			Transform turret = turretList[(int)location];

			Transform oldTurret = turret.Find("TurretContainer").Find("Turret");
			bool isSelected = turret == currentTurret;
				
			GameObject newTurret = (GameObject)GameObject.Instantiate(turretPrefabList[(int)newType], Vector3.zero, Quaternion.identity);
			
			Destroy(oldTurret.gameObject);
			newTurret.transform.parent = turret.Find("TurretContainer");
			newTurret.transform.localPosition = Vector3.zero;
			newTurret.transform.localRotation = Quaternion.identity;
			newTurret.name = "Turret";

			newTurret.transform.GetComponent<Turret>().IsSelected = isSelected;
		}
		
		public void ChangeTurretModifier(TurretLocation location, TurretType newModifierType)
		{
			Turret turret = GetTurretComponent(turretList[(int)location]);
			turret.SetTurretModifierType((TurretModifierType)newModifierType);
		}
		
		public Transform GetTurretByLocation(TurretLocation location)
		{
			return turretList[(int)location];
		}

		public Turret GetTurretComponentByLocation(TurretLocation location)
		{
			return GetTurretComponent(GetTurretByLocation(location));
		}

		public void OnApplicationQuit()
		{
			instance = null;	
		}

		protected void GetInput()
		{
			float controllerAngle = -1.0f;
			float controllerXValue = Input.GetAxis("LeftStickX");
			float controllerYValue = -Input.GetAxis("LeftStickY");
			
			if (Mathf.Abs(controllerXValue) >= 0.65f || Mathf.Abs(controllerYValue) >= 0.65f)
				controllerAngle = (Mathf.Atan2(controllerXValue, controllerYValue) * Mathf.Rad2Deg) + 180.0f;

			if (Input.GetKeyDown(KeyCode.W) || (controllerAngle != -1.0f && controllerAngle > 120 && controllerAngle < 240))
				changeTurretSelection(TurretLocation.Top);
			else if (Input.GetKeyDown(KeyCode.D) || (controllerAngle != -1.0f && controllerAngle > 240 && controllerAngle < 360))
				changeTurretSelection(TurretLocation.Right);
			else if (Input.GetKeyDown(KeyCode.A) || (controllerAngle != -1.0f && controllerAngle > 0 && controllerAngle < 120))
				changeTurretSelection(TurretLocation.Left);
			else if (Input.GetKeyDown(KeyCode.S))
				changeTurretSelection((currentTurretLocation == TurretLocation.Left ? TurretLocation.Right : TurretLocation.Left));
		
			if (Input.GetButtonDown("RightBumper"))
			{
				int type = (int)GetTurretComponent(currentTurret).TurretType;
				type++;

				if (type >= (int)TurretType.Max)
					type = 0;

				ChangeTurretType(currentTurretLocation, (TurretType)type);
			}
		}

		protected Turret GetTurretComponent(Transform turret)
		{
			Transform a = turret.Find("TurretContainer");
			Transform b = a.Find("Turret");
			Turret t = b.GetComponent<Turret>();

			return turret.Find("TurretContainer").Find("Turret").GetComponent<Turret>();
		}
	}
}

