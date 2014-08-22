using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class Base : Entity
	{
		#region Variables

		public Transform autoTurret;

		protected Upgrade addOn;
		protected Upgrade enhancement;

		bool autoTurretIsActive = false; 

		#endregion
		
		#region Properties

		public Upgrade AddOn { get {return addOn; } }
		public Upgrade Enhancement { get { return enhancement; } }
		public bool AutoturretIsActive { get {return autoTurretIsActive; }}

		#endregion
		
		#region initialization

		void Awake()
		{
			autoTurret.gameObject.SetActive(false);

			CheatMenuGUI.OnCheatBtnDownEvent += OnCheatMenuBtnDown;
		}

		void Start ()
		{
			addOn = new Upgrade(UpgradeType.BaseAddOn);
			enhancement = new Upgrade(UpgradeType.BaseEnhancement);
		}
		
		#endregion
		
		#region Game Loop
		
		void Update ()
		{

		}
		
		#endregion
		
		#region Public Methods

		public void OnCheatMenuBtnDown(CheatMenuButtonType buttonType, int buttonIndex, int miscInfo)
		{
			if (buttonType == CheatMenuButtonType.BaseAddOn)
			{
				switch ((BaseAddOnType)buttonIndex)
				{
				case BaseAddOnType.AutoTurret:
					toggleAutoTurret();
					break;
				}
			}
		}

		public void toggleAutoTurret()
		{
			if (!autoTurretIsActive)
				enableAutoTurret();
			else
				disableAutoTurret();
		}
		
		public void enableAutoTurret()
		{
			((AutoTurret)autoTurret.GetComponent<AutoTurret>()).Reset();
			autoTurret.gameObject.SetActive(true);
			autoTurretIsActive = true;
		}
		
		public void disableAutoTurret()
		{
			((AutoTurret)autoTurret.GetComponent<AutoTurret>()).Reset();
			autoTurret.gameObject.SetActive(false);
			autoTurretIsActive = false;
		}

		#endregion
		
		#region Private Methods

		#endregion
	}
}

