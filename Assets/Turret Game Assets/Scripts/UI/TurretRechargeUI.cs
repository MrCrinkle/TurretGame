using System;
using UnityEngine;

namespace AssemblyCSharp
{
	[RequireComponent(typeof(Turret))]
	public class TurretRechargeUI : GUIBar
	{
		protected int shotsLeft = 1;

		protected GUIStyle shotsLeftLabelStyle;
		protected GUIContent shotsLeftLabelContent;

		protected Turret turretComponent = null;

		public override void Start()
		{
			shotsLeftLabelStyle = new GUIStyle();
			shotsLeftLabelStyle.fontSize = 15;
			shotsLeftLabelStyle.normal.textColor = Color.white;

			shotsLeftLabelContent = new GUIContent(shotsLeft.ToString());

			turretComponent = transform.GetComponent<Turret>();

			base.Start();
		}
		
		public override void Update()
		{
			base.Update();

			shotsLeft = turretComponent.CurrentAmmo;
			maxAmount = turretComponent.ammoRechargeRate;
			currentAmount = turretComponent.AmmoRechargeTimer;
		}
		
		public override void OnGUI () 
		{
			Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);

			shotsLeftLabelContent.text = shotsLeft.ToString();
			float minWidth = 0.0f;
			float maxWidth = 0.0f;
			shotsLeftLabelStyle.CalcMinMaxWidth(shotsLeftLabelContent, out minWidth, out maxWidth);

			GUI.Label(new Rect(screenPos.x - (minWidth * 0.5f), Screen.height - screenPos.y, 10.0f, 10.0f), shotsLeft.ToString(), shotsLeftLabelStyle);

			base.OnGUI();
		}
	}
}