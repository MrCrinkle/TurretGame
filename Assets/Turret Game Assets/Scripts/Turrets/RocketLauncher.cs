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
					Transform targetEnemy = EnemyManager.Instance.CheckForEnemyAtMousePosition();

					setRocketTarget(targetEnemy);

					/*
					// get a ray from the mouse position
					Vector3 mousePos = Input.mousePosition;
					Ray mouseVector = Camera.mainCamera.ScreenPointToRay(mousePos);
					
					ArrayList enemies = EnemyManager.Instance.EnemyList;
					Collider enemyCollider = null;
					Transform enemy = null;
					RaycastHit raycastHit
					
					ArrayList enemiesHit = new ArrayList();
					
					for (int i = 0; i < enemies.Count; i++)
					{
						enemy = ((Enemy)enemies[i]).transform;
						enemyCollider = enemy.collider;
						
						if (enemyCollider == null)
							continue;
						
						// check raycast against enemy and add it to the list if it hits
						if (enemyCollider.Raycast(mouseVector, out raycastHit, 1000.0f))
							enemiesHit.Add(enemy);
					}
					
					if (enemiesHit.Count != 0)
					{
						setRocketTarget((Transform)enemiesHit[0]);
					}
					else
					{
						setRocketTarget(null);
					}
					*/
				}
			}
		}
		
		public override bool Shoot()
		{
			return base.Shoot();
		}

		/*
		protected override GameObject SpawnProjectile()
		{
			GameObject projectile = base.SpawnProjectile();
			
			if (modifier.SubType == (int)TurretModifierType.Precision && rocketTarget != null)
			{
				FollowTarget followTarget = (FollowTarget)projectile.AddComponent(typeof(FollowTarget));
				followTarget.target = rocketTarget;
				followTarget.radius = 0.0f;
				
				MovingObject movingObject = projectile.GetComponent<MovingObject>();
				if (movingObject) movingObject.maxMoveSpeed = precisionModRocketSpeed;
			}
			
			return projectile;
		}
		*/
		
		public void OnEntityDeath(GameObject obj)
		{
			if (currentAttack.Target == obj.transform)
				setRocketTarget(null);
		}
		
		public void setRocketTarget(Transform target)
		{
			if (modifier.SubType != (int)TurretModifierType.Precision)
				return;

			Transform originalTarget = currentAttack.Target;
				
			DamageTaker damageTaker = null;
			
			if (target != null)
				damageTaker = target.GetComponent<DamageTaker>();
			
			if (target == null || damageTaker == null || (damageTaker != null && damageTaker.IsAlive))
			{
				currentAttack.Target = target;
				
				if (currentAttack.Target != null)
					currentAttack.Target.localScale = new Vector3(2.0f, 2.0f, 2.0f);
			}
			else if (damageTaker != null && !damageTaker.IsAlive)
			{
				currentAttack.Target = null;
			}
			
			if (originalTarget != null && originalTarget != currentAttack.Target)
				originalTarget.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		}
		
		public override void OnDeselectTurret()
		{
			setRocketTarget(null);

			base.OnDeselectTurret();
		}
	}
}