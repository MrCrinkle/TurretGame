using UnityEngine;

namespace AssemblyCSharp
{
	public class BulletProjectile : Projectile
	{
		float damageMultiplier = 1.0f;
		float pierceDamageLoss = 0.25f;
		
		public override void Start ()
		{
			base.Start();

			projectileType = ProjectileType.Bullet;
		}
		
		public override void Update ()
		{
			base.Update();
		}
		
		protected override void OnCollision(Collision collision, Collider other, Transform hitObject)
		{
			DamageTaker otherDamageTaker = hitObject.GetComponent<DamageTaker>();
			DamageDealer damageDealer = transform.GetComponent<DamageDealer>();

			// only count hit if the target has a damage taker and is alive
			if (otherDamageTaker != null && otherDamageTaker.IsAlive)
			{
				numTargetsHit++;
				damageMultiplier -= pierceDamageLoss;
				
				if (damageMultiplier < 0.0f)
					damageMultiplier = 0.0f;
			
				float bulletScale = (0.8f * damageMultiplier) + 0.2f;
				transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, bulletScale);

				if (damageDealer != null)
				{
					otherDamageTaker.TakeDamage(damageDealer.GetDamage());

					if (!otherDamageTaker.IsAlive && hitObject.GetComponent<Enemy>() != null)
					{
						Vector3 direction = transform.rotation * Vector3.forward;
						Vector3 position = other.ClosestPointOnBounds(transform.position);

						hitObject.rigidbody.AddForceAtPosition(direction * hitForce, position);
					}
				}
			}
			// if theres no damage taker, just destroy it
			else if (otherDamageTaker == null)
			{
				OnHitSolidObject(collision, other, hitObject);
			}
			
			// if the damage is too low with piercing mod, destroy it
			MachineGun machineGun = source.GetComponent<MachineGun>();
			
			if (machineGun == null || machineGun.Modifier.SubType != (int)TurretModifierType.Piercing || damageMultiplier <= 0.0f)
			{
				if (otherDamageTaker == null || (otherDamageTaker != null && (otherDamageTaker.IsAlive || otherDamageTaker.solidWhenDead)))
					OnHitSolidObject(collision, collider, hitObject);
			}
		}

		protected override void OnHitSolidObject(Collision collision, Collider other, Transform hitObject)
		{
			if (destroyOnCollision && hitObject.GetComponent<Projectile>() == null)
				Destroy(gameObject);
		}
	}
}