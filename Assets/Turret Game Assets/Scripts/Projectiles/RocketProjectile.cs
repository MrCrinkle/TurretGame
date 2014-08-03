using System;
using System.Collections;
using UnityEngine;

namespace AssemblyCSharp
{
	public class RocketProjectile : Projectile
	{
		public GameObject explosionParticlePrefab = null;
		public float pierceDamageMult = 0.5f;
		public float pierceRadiusMult = 0.8f;
		public  int maxPiercingTargets = 3;

		private DamageDealer damageDealer = null;
		private float startMaxTurnSpeed = 0;

		public override void Start ()
		{
			projectileType = ProjectileType.Rocket;

			damageDealer = transform.GetComponent<DamageDealer>();
			
			if (damageDealer)
			{
				// rocket has custom damage model
				// DamageDealer properties are still used (splash radius, damage etc.)
				damageDealer.enabled = false;
			}
			
			base.Start();
		}
		
		public override void Update ()
		{
			base.Update();

			FollowTarget followTarget = transform.GetComponent<FollowTarget>();

			if (followTarget != null && followTarget.target != null)
			{
				float ratio = Math.Min(timeAlive / 3.0f, 1.0f);
				float turnSpeed = (360.0f - startMaxTurnSpeed) * ratio + startMaxTurnSpeed;

				if (turnSpeed > followTarget.maxTurnSpeed)
					followTarget.maxTurnSpeed = turnSpeed;

				/*
				float distToTarget = Vector3.Distance(transform.position, followTarget.target.position);

				if (distToTarget < 15.0f)
				{
					if (startMaxTurnSpeed == 0)
						startMaxTurnSpeed = followTarget.maxTurnSpeed;

					float distRatio = distToTarget / 15.0f;
					float turnSpeed = (360.0f - startMaxTurnSpeed) * distRatio + startMaxTurnSpeed;

					if (turnSpeed > followTarget.maxTurnSpeed)
						followTarget.maxTurnSpeed = turnSpeed;
				}
				*/
			}
		}
		
		protected override void OnCollision(Collision collision, Collider other, Transform hitObject)
		{
			DamageTaker otherDamageTaker = hitObject.GetComponent<DamageTaker>();
			
			// only count hit if the target has a damage taker and is alive
			if (otherDamageTaker != null)
			{
				if (otherDamageTaker.IsAlive)
				{
					numTargetsHit++;
				}
				else
				{
					if(otherDamageTaker.solidWhenDead)
						OnHitSolidObject(collision, other, hitObject);

					return;
				}
			}

			RocketLauncher rocketLauncher = source.GetComponent<RocketLauncher>();

			if (hitObject.GetComponent<Enemy>() != null && 
			    rocketLauncher.Modifier.SubType == (int)TurretModifierType.Piercing 
			    && numTargetsHit < maxPiercingTargets)
			{
				Explode(false);
			}
		    else if (otherDamageTaker == null || (otherDamageTaker != null && (otherDamageTaker.IsAlive || otherDamageTaker.solidWhenDead)))
			{
				OnHitSolidObject(collision, other, hitObject);
			}

			if (otherDamageTaker != null)
			{
				otherDamageTaker.TakeDamage(damageDealer.GetDamage());
			
				if (!otherDamageTaker.IsAlive)
				{
					Vector3 rocketDir = transform.forward;
					Vector3 rocketPos = transform.position - rocketDir * 1.0f;
					Vector3 hitPos = hitObject.rigidbody.ClosestPointOnBounds(rocketPos);
					Vector3 direction = Vector3.Normalize(hitPos - rocketPos);

					hitObject.rigidbody.AddForceAtPosition(direction * hitForce, hitPos);
				}
			}
		}

		protected override void OnHitSolidObject(Collision collision, Collider other, Transform hitObject)
		{
			Explode(true);
		}

		protected void Explode(bool destroy)
		{
			// check for 
			ArrayList enemyList = EnemyManager.Instance.EnemyList;
			Enemy enemy = null;
			Vector3 closestPoint = Vector3.zero;
			float distance = 0.0f;
			float distanceMult = 1.0f;
			bool piercingMod = source.GetComponent<RocketLauncher>().Modifier.SubType == (int)TurretModifierType.Piercing;
			float damageRadius = splashAOE;

			if (piercingMod)
				damageRadius = damageRadius * pierceRadiusMult;

			for (int i = 0; i < enemyList.Count; i++)
			{
				enemy = (Enemy)enemyList[i];
				
				//if(enemy.gameObject == hitObject.gameObject)
				//	continue;
				
				DamageTaker enemyDamageTaker = enemy.GetComponent<DamageTaker>();
				
				if (enemyDamageTaker != null)
				{
					closestPoint = enemy.rigidbody.ClosestPointOnBounds(transform.position);
					distance = Vector3.Distance(closestPoint, transform.position);
					distanceMult = 1.0f - (distance / damageRadius);
					
					if (distanceMult > 0.0f)
					{
						if (enemyDamageTaker.IsAlive)
						{
							float finalDamage = damageDealer.GetDamage();

							if (piercingMod)
								finalDamage = finalDamage * pierceDamageMult;

							finalDamage = distanceMult * finalDamage;
							
							if (finalDamage > 0.0f)
								enemyDamageTaker.TakeDamage(finalDamage);
						}
						
						if (!enemyDamageTaker.IsAlive)
						{
							float finalForce = distanceMult * hitForce;
							Vector3 direction = Vector3.Normalize(closestPoint - transform.position);
							
							if (finalForce > 0.0f)
								enemy.rigidbody.AddForceAtPosition(direction * finalForce, closestPoint);
						}
					}
				}
			}
			
			GameObject explosion = (GameObject)GameObject.Instantiate(explosionParticlePrefab, transform.position, Quaternion.identity);
			Destroy(explosion, 1.0f);

			if (destroy)
				Destroy(gameObject);
		}
	}
}