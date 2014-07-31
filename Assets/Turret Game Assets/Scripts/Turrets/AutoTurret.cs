using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class AutoTurret : MonoBehaviour
	{
		#region Variables

		public float fireRate = 2.0f;
		public float burstRate = 0.2f;
		public int shotBurstCount = 3;
		public float range = 20.0f;

		Transform currentTarget = null;
		float shotTimer = 0.0f;
		float shotBurstCounter = 0;
		bool isShooting = false;

		Transform turretGun = null;
		LookAtTarget standLookAtTargetComponent = null;
		LookAtTarget gunLookAtTargetComponent = null;

		Transform leftFlare = null;
		Transform rightFlare = null;

		DamageDealer damageDealerComponent = null;

		#endregion

		public Transform CurrentTarget { get { return currentTarget; } }

		#region Properties

		#endregion

		#region initialization

		void Start ()
		{
			turretGun = transform.Find("Gun");

			standLookAtTargetComponent = (LookAtTarget)GetComponent<LookAtTarget>();
			gunLookAtTargetComponent = (LookAtTarget)turretGun.GetComponent<LookAtTarget>();

			leftFlare = transform.Find("Gun").Find("Left Flare");
			rightFlare = transform.Find("Gun").Find("Right Flare");

			damageDealerComponent = transform.GetComponent<DamageDealer>();

			DamageTaker.OnDeathEvent += OnEntityDeath;

			FindNewTarget();
		}

		void OnDestroy()
		{
			DamageTaker.OnDeathEvent -= OnEntityDeath;
		}

		#endregion

		#region Game Loop

		void Update ()
		{
			bool startShooting = false;

			if (currentTarget == null)
			{
				if (!FindNewTarget())
				{
					// if there's no target available, rotate towards idle position
					Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward);
					turretGun.localRotation = Quaternion.RotateTowards(turretGun.localRotation, targetRotation, 100 * Time.deltaTime);
				}
			}

			shotTimer += Time.deltaTime;

			if (currentTarget != null && !isShooting && shotTimer > fireRate)
			{
				startShooting = true;
			}

			if (startShooting || (isShooting && shotTimer > burstRate))
			{
				shotBurstCounter++;
				shotTimer = 0.0f;
				isShooting = true;

				if (shotBurstCounter >= shotBurstCount)
				{
					isShooting = false;
					shotBurstCounter = 0;
					FindNewTarget();
				}

				Shoot();
			}
		}

		#endregion

		#region Public Methods

		public void Reset()
		{
			OnTargetLost();

			if (leftFlare != null && rightFlare != null)
			{
				leftFlare.Find("Flare").particleSystem.Clear();
				rightFlare.Find("Flare").particleSystem.Clear();
			}

			transform.rotation = Quaternion.identity;

			if (turretGun != null) turretGun.localRotation = Quaternion.identity;
		}

		#endregion

		#region Private Methods

		bool FindNewTarget()
		{
			ArrayList enemyList = EnemyManager.Instance.EnemyList;
			ArrayList possibleTargetList = new ArrayList();
			Transform enemy = null;
			bool foundTarget = false;

			// add all enemyies that are in range into an array
			for (int i = 0; i < enemyList.Count; i++)
			{
				enemy = ((Enemy)enemyList[i]).transform;

				if (Vector3.Distance(enemy.position, transform.position) < range && enemy.GetComponent<DamageTaker>() != null && ((DamageTaker)enemy.GetComponent<DamageTaker>()).IsAlive)
					possibleTargetList.Add(enemy);
			}

			currentTarget = null;

			if (possibleTargetList.Count != 0)
			{
				// randomly select a target out of the list
				int targetIndex = Random.Range(0, possibleTargetList.Count - 1);
				currentTarget = (Transform)possibleTargetList[targetIndex];

				if (standLookAtTargetComponent != null) standLookAtTargetComponent.target = currentTarget;
				if (gunLookAtTargetComponent != null) gunLookAtTargetComponent.target = currentTarget;

				foundTarget = true;
			}

			return foundTarget;
		}

		void Shoot()
		{
			if (currentTarget == null)
				return;

			((ParticleSystem)leftFlare.Find("Flare").GetComponent<ParticleSystem>()).Play();
			((ParticleSystem)rightFlare.Find("Flare").GetComponent<ParticleSystem>()).Play();

			if (damageDealerComponent != null)
			{
				DamageTaker damageTaker = currentTarget.GetComponent<DamageTaker>();

				if (damageTaker != null)
				{
					float damage = damageDealerComponent.GetDamage() / shotBurstCount;
					damageTaker.TakeDamage(damage);
				}
			}
		}

		void OnTargetLost()
		{
			shotTimer = 0.0f;
			shotBurstCounter = 0;
			isShooting = false;
			currentTarget = null;

			if (standLookAtTargetComponent != null) standLookAtTargetComponent.target = null;
			if (gunLookAtTargetComponent != null) gunLookAtTargetComponent.target = null;
		}

		void OnEntityDeath(GameObject obj)
		{
			if (obj.transform == currentTarget)
				OnTargetLost();
		}

		#endregion
	}
}