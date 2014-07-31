using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class MachineGun : Turret
	{
		public override void Start ()
		{
			base.Start();
			
			turretType = TurretType.MachineGun;
			fireType = FireType.Auto;
		}
		
		public override void Update ()
		{
			base.Update();
		}
		
		public override void SetTurretModifierType(TurretModifierType type)
		{
			base.SetTurretModifierType(type);
		}
	}
}