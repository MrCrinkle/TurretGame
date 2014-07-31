using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class LightningGun : Turret
	{
		public override void Start ()
		{
			base.Start();
			
			turretType = TurretType.Lightning;
			fireType = FireType.Auto;
		}
		
		public override void Update ()
		{
			base.Update();
		}
	}
}

