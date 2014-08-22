using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class RocketLauncher : Turret
	{
		public override void Start ()
		{
			base.Start();
			
			turretType = TurretType.RocketLauncher;
			fireType = FireType.Auto;
			
			DamageTaker.OnDeathEvent += OnEntityDeath;
		}
		
		public void OnDestroy()
		{
			DamageTaker.OnDeathEvent -= OnEntityDeath;
		}
		
		public override void Update ()
		{
			base.Update();
			
			// implementation of precision modifier for rocket launcher, which targets the enemy you click on
			if (TurretManager.Instance.CurrentTurret == transform.parent.parent && modifier.SubType == (int)TurretModifierType.Precision)
			{
				if (Input.GetButtonDown("Fire2"))
				{
					Enemy targetEnemy = EnemyManager.Instance.CheckForEnemyAtMousePosition();

					setRocketTarget(targetEnemy);
				}
			}
		}
		
		public override bool Shoot()
		{
			return base.Shoot();
		}
		
		public void OnEntityDeath(Entity obj)
		{
			if (currentAttack.Target == obj)
				setRocketTarget(null);
		}
		
		public void setRocketTarget(Entity target)
		{
			if (modifier.SubType != (int)TurretModifierType.Precision)
				return;

			Entity originalTarget = currentAttack.Target;
				
			DamageTaker damageTaker = null;
			
			if (target != null)
				damageTaker = target.GetComponent<DamageTaker>();
			
			if (target == null || damageTaker == null || (damageTaker != null && damageTaker.IsAlive))
			{
				currentAttack.Target = target;
				
				if (currentAttack.Target != null)
					currentAttack.Target.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
			}
			else if (damageTaker != null && !damageTaker.IsAlive)
			{
				currentAttack.Target = null;
			}
			
			if (originalTarget != null && originalTarget != currentAttack.Target)
				originalTarget.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		}
		
		public override void OnDeselectTurret()
		{
			setRocketTarget(null);

			base.OnDeselectTurret();
		}
	}
}