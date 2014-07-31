using UnityEngine;
using System.Collections;
using System.Xml;

namespace AssemblyCSharp
{
	public class Attack
	{
		#region Variables

		protected string attackName = "";
		protected Vector2 damage = new Vector2(1.0f, 1.0f);
		protected float delay = 0.5f;
		protected float minRange = 0.0f;
		protected float maxRange = 0.5f;
		protected bool isMelee = true;
		protected float accuracy = 0.0f;
		protected float projectileSpeedMultiplier = 1.0f;

		protected string projectilePrefabName = "null";

		protected GameObject target = null;
		protected Vector3 targetPosition = Vector3.zero;
		protected string xmlFileName = "";
		protected Transform owner = null;

		#endregion
		
		#region Properties
		
		public GameObject Target { get { return target; } set { target = value; } }
		public Transform Owner { get { return owner; } set { owner = value; } }
		public string AttackName { get { return attackName; } set { attackName = value; } }
		public Vector2 Damage { get { return damage; } set { damage = value; } }
		public float Delay { get { return delay; } set { delay = value; } }
		public float MinRange { get { return minRange; } set { minRange = value; } }
		public float MaxRange { get { return maxRange; } set { maxRange = value; } }
		public bool IsMelee { get { return isMelee; } set { isMelee = value; } }
		public float Accuracy { get { return accuracy; } set { accuracy = value; } }
		public float ProjectileSpeedMultiplier { get { return projectileSpeedMultiplier; } set { projectileSpeedMultiplier = value; } }
		public string ProjectilePrefabName { get { return projectilePrefabName; } set { projectilePrefabName = value; } }

		#endregion
		
		#region Initialization
		
		#endregion
		
		#region Game Loop
		
		#endregion
		
		#region Public Methods

		public virtual ArrayList StartAttack(Quaternion direction, Vector3 position)
		{
			if (!isMelee && projectilePrefabName != "null" && projectilePrefabName != "")
			{
				ArrayList projectiles = new ArrayList();

				projectiles.Add(SpawnProjectile(direction, position));

				return projectiles;
			}

			return null;
		}

		#endregion
		
		#region Private Methods

		protected virtual GameObject SpawnProjectile(Quaternion direction, Vector3 position)
		{
			GameObject newProjectile = (GameObject)GameObject.Instantiate(Resources.Load(projectilePrefabName));
			newProjectile.transform.GetComponent<Projectile>().Source = owner;

			MovingObject movingObject = newProjectile.GetComponent<MovingObject>();
			movingObject.MoveSpeed = movingObject.MoveSpeed * projectileSpeedMultiplier;

			newProjectile.transform.position = position;
			newProjectile.transform.rotation = direction;
			newProjectile.transform.GetComponent<Projectile>().Source = owner;
			
			if (accuracy > 0.0f)
			{
				float angleModifier = Random.Range(0.0f, accuracy);
				float newRotation = direction.eulerAngles.y;
				newRotation = newRotation + angleModifier - (accuracy * 0.5f);
				
				Vector3 newDirection = new Vector3(Mathf.Sin(newRotation * Mathf.Deg2Rad), 0.0f, Mathf.Cos(newRotation * Mathf.Deg2Rad));
				newProjectile.transform.rotation = Quaternion.LookRotation(newDirection);
			}
			else
			{
				newProjectile.transform.rotation = Quaternion.LookRotation(direction * Vector3.forward);
			}
			
			return newProjectile;
		}
		
		#endregion
	}
}