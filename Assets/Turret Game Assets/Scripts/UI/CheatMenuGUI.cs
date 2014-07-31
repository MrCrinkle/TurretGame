using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public enum CheatMenuButtonType { Turret, TurretModifier, BaseAddOn, BaseEnhancement };

	public class CheatMenuGUI : MonoBehaviour
	{
		#region Variables

		private static CheatMenuGUI instance;

		public float buttonSize = 40.0f;
		public float upgradeButtonSize = 30.0f;
		public float buttonSpacing = 3.0f;

		public Texture2D[] turretIconList;
		public Texture2D[] modifierIconList;
		
		private bool isOpen = false;

		public delegate void OnCheatBtnDownDelegate(CheatMenuButtonType buttonType, int buttonIndex, int miscInfo);
		public static event OnCheatBtnDownDelegate OnCheatBtnDownEvent;

		#endregion
		
		#region Properties

		public CheatMenuGUI Instance
		{
			get;
			private set;
		}

		#endregion
		
		#region initialization

		void Awake()
		{
			if (instance != null && instance != this)
			{
				Destroy(gameObject);
			}

			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

		void Start ()
		{

		}
		
		#endregion
		
		#region Game Loop
		
		void Update ()
		{
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				// if the menu is open, close it and unpause the game
				if  (isOpen)
				{
					isOpen = false;
					Time.timeScale = 1;
				}
				else // othewise open the menu and pause the game
				{
					isOpen = true;
					Time.timeScale = 0;
				}
			}
		}
		
		#endregion
		
		#region Public Methods
		
		void OnGUI()
		{
			if (isOpen)
			{
				float border = 10.0f;
				float doubleBorder = border * 2.0f;
				float turretSelectorSpacing = 130.0f;
				
				float width = 0.0f;
				float height = 0.0f;
				float x = 0.0f;
				float y = 0.0f;
				
				height = Screen.height - doubleBorder;
				width = height;
				x = (Screen.width * 0.5f) - (width * 0.5f);
				y = border;
				
				GUI.BeginGroup(new Rect(x, y, width, height));
				
				GUI.Box(new Rect(0.0f, 0.0f, width, height), "");
				
				// Turret Buttons
				createTurretSelectorGUI(width * 0.5f, height * 0.5f - (turretSelectorSpacing * 1.2f), TurretLocation.Top);
				createTurretSelectorGUI(width * 0.5f - turretSelectorSpacing * 0.9f, height * 0.5f + (turretSelectorSpacing * 1.2f), TurretLocation.Left);
				createTurretSelectorGUI(width * 0.5f + turretSelectorSpacing * 0.9f, height * 0.5f + (turretSelectorSpacing * 1.2f), TurretLocation.Right);
				
				// Base Buttons
				createBaseSelectorGUI(width * 0.5f, height * 0.5f);
				
				GUI.EndGroup();
			}
		}

		protected void createTurretSelectorGUI(float x, float y, TurretLocation turretLocation)
		{
			float halfTurretBtnSize = buttonSize * 0.5f;
			int num = (int)TurretType.Max;
			float startPos = x - ((num * 0.5f) * buttonSize + ((num * 0.5f - 0.5f) * buttonSpacing));
			Turret turret = TurretManager.Instance.GetTurretComponentByLocation(turretLocation);
			
			// Turret Options
			for (int i = 0; i < num; i++)
			{
				if ((int)turret.TurretType == i)
				{
					GUI.color = Color.black;
					GUI.Box(new Rect(startPos + (buttonSize + buttonSpacing) * i, y - halfTurretBtnSize, buttonSize, buttonSize), turretIconList[i]);
				}
				else
				{
					GUI.color = Color.white;
					if(GUI.Button(new Rect(startPos + (buttonSize + buttonSpacing) * i, y - halfTurretBtnSize, buttonSize, buttonSize), turretIconList[i]))
						onTurretButtonPressed(turretLocation, i);
				}
			}
			
			// Modifier Options
			float halfUpgradeBtnSize = upgradeButtonSize * 0.5f;
			num = (int)TurretModifierType.Max;
			startPos = x - ((num * 0.5f) * upgradeButtonSize + ((num * 0.5f - 0.5f) * buttonSpacing));
			
			for (int i = 0; i < num; i++)
			{
				if ((int)turret.Modifier.SubType == i)
				{
					GUI.color = Color.black;
					GUI.Box(new Rect(startPos + (upgradeButtonSize + buttonSpacing) * i, y - halfTurretBtnSize -  upgradeButtonSize - buttonSpacing, upgradeButtonSize, upgradeButtonSize), modifierIconList[i]);
				}
				else
				{
					GUI.color = Color.white;
					if(GUI.Button(new Rect(startPos + (upgradeButtonSize + buttonSpacing) * i, y - halfTurretBtnSize -  upgradeButtonSize - buttonSpacing, upgradeButtonSize, upgradeButtonSize), modifierIconList[i]))
						onTurretModifierButtonPressed(turretLocation, i);
				}
			}
		}

		protected void createBaseSelectorGUI(float x, float y)
		{
			int totalButtons = (int)BaseAddOnType.Max;
			float startPos = x - ((totalButtons * 0.5f) * buttonSize + ((totalButtons * 0.5f - 0.5f) * buttonSpacing));

			for (int i = 0; i < totalButtons; i++)
			{
				if (i == (int)BaseAddOnType.AutoTurret)
				{
					if(GUI.Button(new Rect(startPos + (buttonSize + buttonSpacing) * i, y - buttonSize * 0.5f, buttonSize, buttonSize), "AT"))
						onBaseAddonButtonPressed(i);
				}
				else
				{
					//if(GUI.Button(new Rect(startPos + (buttonSize + buttonSpacing) * i, y - buttonSize * 0.5f, buttonSize, buttonSize), enhancementIconList[0]))
					//	onBaseAddonButtonPressed(i);
				}
			}
		}
		
		protected void onTurretButtonPressed(TurretLocation turretLocation, int buttonIndex)
		{
			OnCheatBtnDownEvent(CheatMenuButtonType.Turret, buttonIndex, (int)turretLocation);
		}
		
		protected void onTurretModifierButtonPressed(TurretLocation turretLocation, int buttonIndex)
		{
			OnCheatBtnDownEvent(CheatMenuButtonType.TurretModifier, buttonIndex, (int)turretLocation);
		}
		
		protected void onBaseAddonButtonPressed(int buttonIndex)
		{
			OnCheatBtnDownEvent(CheatMenuButtonType.BaseAddOn, buttonIndex, 0);

		}
		
		#endregion
		
		#region Private Methods
		
		#endregion
	}
}
