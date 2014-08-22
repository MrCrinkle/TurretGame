using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class GaussProjectile : Projectile
	{
		#region Variables

		public float fadeTime = 0.2f;
		public float pierceDamageMult = 0.8f;
		public float maxPierceTargetDist = 10.0f;
		public float maxPiercerTargetAngle = 40.0f;

		protected float fadeTimer = 0.0f;
		protected Transform shotModel = null;
		protected float shotSize = 25.0f;

		Entity target = null;
		bool isSecondProjectile = false;

		#endregion

		public Entity Target 
		{ 
			get { return target; }
			set { target = value; }
		}

		public bool IsSecondProjectile
		{
			get { return isSecondProjectile; }
			set { isSecondProjectile = value; }
		}

		#region Properties

		#endregion
		
		#region initialization
		
		public override void Start()
		{
			base.Start();

			projectileType = ProjectileType.GaussShot;

			shotModel = transform.Find("BulletPlane");

			ResizeShotToTarget();
		}
		
		#endregion
		
		#region Game Loop
		
		public override void Update ()
		{
			base.Update();

			fadeTimer += Time.deltaTime;

			float alpha = 1.0f - Mathf.Min(1.0f, fadeTimer / fadeTime);

			Color color = shotModel.renderer.material.GetColor("_Color");
			color.a = alpha;
			shotModel.renderer.material.SetColor("_Color", color);;

			if (fadeTimer >= fadeTime)
				Destroy(gameObject);;
		}
		
		#endregion
		
		#region Public Methods

		#endregion
		
		#region Private Methods

		void ResizeShotToTarget()
		{
			if (target == null || (target.GetComponent<DamageTaker>() != null && !target.GetComponent<DamageTaker>().IsAlive))
			{
				FindShotDistance();
			}
			else
			{
				if (target.collider != null)
				{
					Ray ray = new Ray(new Vector3(transform.position.x, World.Instance.standardCollisionHeight, transform.position.z), transform.forward);
					RaycastHit hit;

					Collider colliderToCheck = target.collider;
					if (target.transform.Find("ProjectileCollider") != null)
						colliderToCheck = target.transform.Find("ProjectileCollider").collider;

					if(colliderToCheck.Raycast(ray, out hit, 100.0f))
					{
						Vector3 hitPos = hit.point;
						hitPos.y = World.Instance.standardCollisionHeight;

						shotSize = Vector3.Distance(transform.position, hitPos) * 0.5f;

						HitEnemy(hit, transform.forward);
					}
				}
			}

			if (shotModel != null)
			{
				shotModel.localScale = new Vector3(shotModel.localScale.x, shotSize, shotModel.localScale.z);
				shotModel.localPosition = new Vector3(0.0f, 0.0f, shotSize);
			}
		}

		void FindShotDistance()
		{
			RaycastHit hit;
			Vector3 position = transform.position;
			position.y = World.Instance.standardCollisionHeight;

			if (checkRaycast(position, transform.forward, out hit, 0))
			{
				Vector3 hitPos = hit.point;
				hitPos.y = World.Instance.standardCollisionHeight;
				
				shotSize = Vector3.Distance(position, hitPos) * 0.5f;
			}
		}

		// checkRaycast will call itself recursively if it hits an enemy and the enemy is dead
		bool checkRaycast(Vector3 startPos, Vector3 direction, out RaycastHit hit, int iterationNumber)
		{
			bool hitSomething = false;
			bool startedAlive = false;

			if(Physics.Raycast(startPos, direction, out hit))
			{
				Transform hitTransform = hit.transform;

				if (hitTransform.name == "ProjectileCollider")
					hitTransform = hitTransform.parent;

				if (hitTransform.GetComponent<Enemy>() != null)
				{
					DamageTaker damageTaker = (DamageTaker)hit.transform.GetComponent<DamageTaker>();

					startedAlive = (damageTaker != null && damageTaker.IsAlive);

					HitEnemy(hit, direction);

					if (damageTaker != null)
					{
						if (startedAlive)
							hitSomething = true;
						else if (iterationNumber < 5)// set an iteration limit to prevent an infinite loop
							hitSomething = checkRaycast(hit.point + direction, direction, out hit, ++iterationNumber);
					}
				}
				else
				{
					hitSomething = true;
				}
			}

			return hitSomething;
		}

		void HitEnemy(RaycastHit hit, Vector3 direction)
		{
			Transform enemy = hit.transform;
			DamageTaker damageTaker = (DamageTaker)enemy.GetComponent<DamageTaker>();

			if (damageTaker != null)
			{
				if (damageTaker.IsAlive)
				{
					damageTaker.TakeDamage(transform.GetComponent<DamageDealer>().GetDamage());

					if (source.GetComponent<Turret>().Modifier.SubType == (int)TurretModifierType.Piercing)
						CheckPierceShot(hit, direction);

					if (!damageTaker.IsAlive)
					{
						if (enemy.rigidbody != null)
							enemy.rigidbody.AddForceAtPosition(new Vector3(direction.x, 0.0f, direction.z) * hitForce, hit.point);
					}
				}
			}
		}

		void CheckPierceShot(RaycastHit hit, Vector3 direction)
		{
			ArrayList enemyList = EnemyManager.Instance.EnemyList;
			ArrayList enemiesInRange = new ArrayList();
			Transform enemy = null;
			float dist = 0.0f;
			float closestDist = maxPierceTargetDist;
			Transform closestEnemy = null;

			for (int i = 0; i < enemyList.Count; i++)
			{
				enemy = ((Enemy)enemyList[i]).transform;

				if (enemy == hit.transform || !enemy.GetComponent<DamageTaker>().IsAlive)
					continue;

				dist = Vector3.Distance(hit.point, enemy.position);

				if (dist < maxPierceTargetDist)
				{
					Vector2 enemyVector = new Vector2(enemy.position.x - hit.point.x, enemy.position.z - hit.point.z);
					Vector2 shotVector = new Vector2(direction.x, direction.z);

					float angle = Mathf.Abs(Vector2.Angle(enemyVector, shotVector));

					if (angle < maxPiercerTargetAngle && dist < closestDist)
					{
						closestDist = dist;
						closestEnemy = enemy;
					}
				}
			}

			if (closestEnemy != null)
			{
				Quaternion shotRotation = Quaternion.LookRotation(closestEnemy.position - hit.point);

				GameObject newProjectile = (GameObject)GameObject.Instantiate(source.GetComponent<Turret>().projectilePrefab, hit.point, shotRotation);
				newProjectile.transform.GetComponent<Projectile>().Source = source;
				newProjectile.GetComponent<GaussProjectile>().IsSecondProjectile = true;
				newProjectile.GetComponent<GaussProjectile>().target = closestEnemy.GetComponent<Entity>();
				newProjectile.GetComponent<DamageDealer>().maxDamage *= pierceDamageMult;
				newProjectile.GetComponent<DamageDealer>().minDamage *= pierceDamageMult;
				
				//newProjectile.transform.Find("BulletPlane").renderer.material.SetColor("_Color", Color.red);
			}
		}

		#endregion
	}
}