using UnityEngine;
using System.Collections;
using System.Xml;

namespace AssemblyCSharp
{
	public class MGMultiplyAttack : Attack
	{
		#region Variables
		
		float multiplyModAngle = 8.0f;
		float multiplyModDamage = 0.5f;
		
		#endregion
		
		#region Properties
		
		
		
		#endregion
		
		#region Initialization
		
		#endregion
		
		#region Game Loop
		
		#endregion
		
		#region Public Methods
		
		public override ArrayList StartAttack(Quaternion direction, Vector3 position)
		{
			if (!isMelee && projectilePrefabName != "null" && projectilePrefabName != "")
			{
				ArrayList projectiles = new ArrayList();
				
				projectiles.Add(SpawnProjectile(direction, position));

				float xDist = Mathf.Tan(Mathf.Deg2Rad * multiplyModAngle);

				GameObject leftShot = SpawnProjectile(direction, position);
				leftShot.transform.localScale = new Vector3(1.2f, 1.0f, 1.2f);
				leftShot.transform.rotation = Quaternion.LookRotation(direction * new Vector3(-xDist, 0.0f, 1.0f));
				((DamageDealer)leftShot.GetComponent<DamageDealer>()).MultiplyDamage(multiplyModDamage);
				
				GameObject rightShot = SpawnProjectile(direction, position);
				rightShot.transform.localScale = new Vector3(1.2f, 1.0f, 1.2f);
				rightShot.transform.rotation = Quaternion.LookRotation(direction * new Vector3(xDist, 0.0f, 1.0f));
				((DamageDealer)rightShot.GetComponent<DamageDealer>()).MultiplyDamage(multiplyModDamage);

				return projectiles;
			}
			
			return null;
		}

		/*
		public override bool Shoot()
		{
			bool canShoot = base.Shoot();

			if (canShoot && modifier.SubType == (int)TurretModifierType.Multiply)
			{
				float xDist = Mathf.Tan(Mathf.Deg2Rad * multiplyModAngle);

				GameObject leftShot = SpawnProjectile();
				leftShot.transform.localScale = new Vector3(1.2f, 1.0f, 1.2f);
				leftShot.transform.rotation = Quaternion.LookRotation(container.rotation * new Vector3(-xDist, 0.0f, 1.0f));
				((DamageDealer)leftShot.GetComponent<DamageDealer>()).MultiplyDamage(multiplyModDamage);

				GameObject rightShot = SpawnProjectile();
				rightShot.transform.localScale = new Vector3(1.2f, 1.0f, 1.2f);
				rightShot.transform.rotation = Quaternion.LookRotation(container.rotation * new Vector3(xDist, 0.0f, 1.0f));
				((DamageDealer)rightShot.GetComponent<DamageDealer>()).MultiplyDamage(multiplyModDamage);
			}

			return canShoot;
		}
		*/

		#endregion
		
		#region Private Methods
		
		#endregion
	}
}

