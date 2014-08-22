using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class MineProjectile : Projectile
	{
		#region Variables

		public float fallSpeed = 40.0f;
		public float triggerRadius = 4.0f;
		public float armTime = 0.5f;
		public float explosionForce = 5000.0f;
		public float pierceModDelay = 4.0f;
		public int maxPierceExplosions = 3;

		public GameObject explosionParticlePrefab = null;

		private Vector3 targetPosition = Vector3.zero;
		private Vector3 startPosition = Vector3.zero;
		private bool inAir = true;
		private bool armed = false;
		private bool triggered = false;
		private float armTimer = 0.0f;
		private float pierceModTimer = 0.0f;
		private int totalExplosions = 0;

		private Transform radiusIndicator = null;

		#endregion
		
		#region Properties

		public Vector3 StartPosition
		{
			get { return startPosition; }
			set 
			{
				startPosition = value;
				
				MovingObject movingObject = transform.GetComponent<MovingObject>();
				movingObject.direction = Vector3.Normalize(targetPosition - startPosition);
				movingObject.ResetVelocity();

				transform.position = value;
			}
		}

		public Vector3 TargetPosition
		{
			get { return targetPosition; }
			set 
			{
				targetPosition = value;

				MovingObject movingObject = transform.GetComponent<MovingObject>();
				movingObject.direction = Vector3.Normalize(targetPosition - startPosition);
				movingObject.ResetVelocity();
			}
		}

		#endregion
		
		#region initialization
	
		void Awake()
		{
			radiusIndicator = transform.Find("RadiusIndicator");
			radiusIndicator.renderer.enabled = false;
			radiusIndicator.localScale = new Vector3(triggerRadius, triggerRadius, 1.0f);

			MovingObject movingObject = transform.GetComponent<MovingObject>();
			movingObject.useLookDirection = false;
			movingObject.ResetVelocity();
		}

		public override void Start()
		{
			base.Start();

			projectileType = ProjectileType.Mine;
		}
	
		#endregion
		
		#region Game Loop
	
		public override void Update()
		{
			base.Update();
		}

		void FixedUpdate()
		{

			MovingObject m = transform.GetComponent<MovingObject>();

			if (inAir)
			{
				if (transform.position.y <= 0.2f)
				{
					transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
					inAir = false;

					MovingObject movingObject = transform.GetComponent<MovingObject>();
					
					movingObject.MoveSpeed = 0.0f;
					movingObject.Velocity = Vector3.zero;

					CheckOtherMineRadius();
				}
			}
			else if (!armed && triggered && ModifierType == TurretModifierType.Piercing)
			{
				pierceModTimer += Time.deltaTime;

				if (pierceModTimer > pierceModDelay)
				{
					armed = true;
					radiusIndicator.renderer.enabled = true;
				}
			}
			else if (!inAir && !armed)
			{
				armTimer += Time.deltaTime;

				if (armTimer > armTime)
				{
					armed = true;
					radiusIndicator.renderer.enabled = true;
				}
			}
			else if (armed && (!triggered || ModifierType == TurretModifierType.Piercing))
			{
				ArrayList enemies = EnemyManager.Instance.EnemyList;
				Transform enemy = null;

				for (int i = 0; i < enemies.Count; i++)
				{
					enemy = ((Enemy)enemies[i]).transform;

					if (Vector3.Distance(enemy.position, transform.position) < triggerRadius)
					{
						OnTriggered(enemy.GetComponent<Enemy>());
					}
				}
			}
		}
	
		#endregion
		
		#region Public Methods

		public void Explode()
		{
			ArrayList enemies = EnemyManager.Instance.EnemyList;
			Transform enemy = null;
			DamageTaker damageTaker = null;
			DamageDealer damageDealer = transform.GetComponent<DamageDealer>();
			
			for (int i = 0; i < enemies.Count; i++)
			{
				enemy = ((Enemy)enemies[i]).transform;
				damageTaker = enemy.GetComponent<DamageTaker>();
				
				if (Vector3.Distance(enemy.position, transform.position) < splashAOE)
				{
					if (damageDealer != null && damageTaker != null)
					{
						damageTaker.TakeDamage(damageDealer.GetDamage());
						
						if (!damageTaker.IsAlive)
						{
							Vector3 force = Vector3.Normalize(enemy.position - transform.position) * explosionForce;
							enemy.rigidbody.AddForce(force);
						}
					}
				}
			}
			
			GameObject explosion = (GameObject)GameObject.Instantiate(explosionParticlePrefab, transform.position, Quaternion.identity);
			Destroy(explosion, 1.0f);
			
			totalExplosions++;
			
			if (ModifierType != TurretModifierType.Piercing || totalExplosions > maxPierceExplosions)
			{
				Destroy(gameObject);
			}
			else
			{
				armed = false;
				radiusIndicator.renderer.enabled = false;
			}
		}

		#endregion
		
		#region Private Methods

		protected override void OnCollision(Collision collision, Collider other, Transform hitObject)
		{
			if (armed && hitObject.GetComponent<Enemy>() != null)
			{
				Explode();
			}
		}

		protected void OnTriggered(Entity target)
		{
			if (!triggered || ModifierType == TurretModifierType.Piercing)
			{
				triggered = true;
			
				radiusIndicator.renderer.enabled = false;

				if (source.GetComponent<Turret>().Modifier.SubType == (int)TurretModifierType.Precision)
				{
					FollowTarget followTarget = (FollowTarget)gameObject.AddComponent(typeof(FollowTarget));
					followTarget.Target = target;
					followTarget.maxTurnSpeed = 0.0f;

					MovingObject movingObject = transform.GetComponent<MovingObject>();
					movingObject.ResetVelocity();
					movingObject.useLookDirection = true;
				}
				else
				{
					Explode();
				}
			}
		}

		protected void CheckOtherMineRadius()
		{
			// check for other mines
			// if this mine is in the radius of another, make it explode
			ArrayList mineList = ProjectileManager.Instance.GetAllProjectilesOfType(ProjectileType.Mine);
			MineProjectile otherMine = null;
			
			if (mineList != null && mineList.Count > 0)
			{
				for (int i = 0; i < mineList.Count; i++)
				{
					otherMine = ((MineProjectile)mineList[i]);

					if (otherMine.transform == transform)
						continue;

					float dist = Vector3.Distance(transform.position, otherMine.transform.position);
					
					if (dist < otherMine.triggerRadius)
						otherMine.Explode();
				}
			}
		}

		#endregion
	}
}