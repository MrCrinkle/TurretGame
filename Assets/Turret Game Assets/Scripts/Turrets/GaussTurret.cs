using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class GaussTurret : Turret
	{
		#region Variables

		Transform currentPrecisionTarget = null;

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
					Transform target = EnemyManager.Instance.CheckForEnemyAtMousePosition();

					if (target != null && target.GetComponent<Enemy>() != null)
					{
						DamageTaker damageTaker = target.GetComponent<DamageTaker>();

						if(damageTaker != null && damageTaker.IsAlive)
							currentPrecisionTarget = target;
					}
				}
			}

			base.Update();
		}
		
		#endregion
		
		#region Public Methods

		/*
		protected override GameObject SpawnProjectile()
		{
			GameObject newProjectile = base.SpawnProjectile();
			GaussProjectile gaussProjectile = newProjectile.GetComponent<GaussProjectile>();

			if (gaussProjectile != null)
				gaussProjectile.Target = currentPrecisionTarget;

			return newProjectile;
		}
		*/

		#endregion
		
		#region Private Methods
		
		#endregion
	}
}