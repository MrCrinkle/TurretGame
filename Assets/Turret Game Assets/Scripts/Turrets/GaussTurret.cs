using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class GaussTurret : Turret
	{
		#region Variables

		Entity currentPrecisionTarget = null;

		#endregion
		
		#region Properties
		
		#endregion
		
		#region initialization
		
		public override void Start ()
		{
			base.Start();
			
			turretType = TurretType.Gauss;
			fireType = FireType.Auto;
		}
		
		#endregion
		
		#region Game Loop
		
		public override void Update ()
		{
			currentPrecisionTarget = null;

			if (TurretManager.Instance.CurrentTurret == gameObject && modifier.SubType == (int)TurretModifierType.Precision)
			{
			    if (Input.GetButtonDown("Fire1"))
				{
					Enemy target = EnemyManager.Instance.CheckForEnemyAtMousePosition();

					if (target != null)
					{
						DamageTaker damageTaker = target.transform.GetComponent<DamageTaker>();

						if(damageTaker != null && damageTaker.IsAlive)
							currentPrecisionTarget = target;
					}
				}
			}

			base.Update();
		}
		
		#endregion
		
		#region Public Methods


		#endregion
		
		#region Private Methods
		
		#endregion
	}
}