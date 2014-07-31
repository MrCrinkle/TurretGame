using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public enum ProjectileType { Generic, Bullet, Rocket, Lightning, GaussShot, Mine, Max };

	[RequireComponent(typeof(MovingObject))]
	public class Projectile : MonoBehaviour
	{
		#region Variables

		public float projectileSize = 5.0f;
		public float splashAOE = 0.0f;
		public bool destroyOnCollision = true;
		public float hitForce = 3000.0f;

		protected ProjectileType projectileType = ProjectileType.Generic;

		protected float timeAlive = 0.0f;
		protected int numTargetsHit = 0;
		
		protected Transform source;

		private TurretModifierType modifierType = TurretModifierType.None;

		#endregion
		
		#region Properties

		public Transform Source { get { return source; } set { source = value; } }
		public int NumTargetsHit { get { return numTargetsHit; } }
		public ProjectileType ProjectileType { get { return projectileType; } }

		public TurretModifierType ModifierType { get {return modifierType; } }

		#endregion
		
		#region initialization

		public virtual void Start()
		{
			DamageTaker.OnDeathEvent += OnEntityDeath;

			if (source != null && source.GetComponent<Turret>() != null)
			{
				modifierType = (TurretModifierType)source.GetComponent<Turret>().Modifier.SubType;
			}

			ProjectileManager.Instance.AddProjectile(this);
		}
		
		public virtual void OnDestroy()
		{
			DamageTaker.OnDeathEvent -= OnEntityDeath;

			if (collider != null)
				collider.enabled = false;

			ProjectileManager.Instance.RemoveProjectile(this, false);
		}

		#endregion
		
		#region Game Loop

		public virtual void Update()
		{
			timeAlive += Time.deltaTime;
			
			if (gameObject.transform.position.magnitude > 75.0f)
			{
				Destroy(gameObject);
			}
		}

		#endregion
		
		#region Public Methods

		public virtual void OnTriggerEnter(Collider other)
		{
			Transform hitObject = other.transform;

			if (hitObject.name == "ProjectileCollider")
				hitObject = other.transform.parent;
			else if (other.transform.Find("ProjectileCollider") != null)
				return;

			OnCollision(null, other, hitObject);
		}
		
		public virtual void OnCollisionEnter(Collision collision)
		{
			// only call on collision if there isn't a seperate projectile collider
			if (collision.transform.Find("ProjectileCollider") == null)
				OnCollision(collision, collision.collider, collision.transform);
		}
		
		protected virtual void OnCollision(Collision collision, Collider other, Transform hitObject)
		{
			DamageDealer damageDealer = transform.GetComponent<DamageDealer>();
			DamageTaker damageTaker = hitObject.GetComponent<DamageTaker>();

			if (damageDealer != null && damageDealer.enabled && damageTaker != null && damageTaker.IsAlive)
			{
				damageTaker.TakeDamage(damageDealer.GetDamage());
			}

			if (destroyOnCollision && hitObject.GetComponent<Projectile>() == null)
			{	
				Destroy(gameObject);
			}
		}
		
		public void OnEntityDeath(GameObject obj)
		{
			FollowTarget followTarget = transform.GetComponent<FollowTarget>();
			
			if (followTarget != null && followTarget.target != null && followTarget.target == obj.transform)
				followTarget.target = null;
		}

		protected virtual void OnHitSolidObject(Collision collision, Collider other, Transform hitObject)
		{
			
		}

		#endregion
		
		#region Private Methods
		
		
		
		#endregion
	}
}

