using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class MineLauncher : Turret
	{
		#region Variables

		Vector3 currentTargetPosition = Vector3.zero;

		#endregion
		
		#region Properties
		
		#endregion
		
		#region initialization
	
		public override void Start ()
		{
			base.Start();
			
			turretType = TurretType.Mine;
			fireType = FireType.SingleFire;
		}
	
		#endregion
		
		#region Game Loop
	
		public override void Update ()
		{
			base.Update();
		}
	
		#endregion
		
		#region Public Methods


		#endregion
		
		#region Private Methods


		#endregion
	}
}