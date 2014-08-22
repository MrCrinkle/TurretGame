using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class Enemy : Entity
	{
		#region Variables

		public Attack[] attacks;
		public int spawnValue = 2; // "value" of the enemy when spawning. More difficult enemies have higher values.

		protected float attackTimer = 0.0f;
		protected bool readyToAttack = true;

		public string[] attackNameList;
		protected Dictionary<string, Attack> attackList;
		protected Attack currentAttack = null;

		protected Entity target = null;
		protected bool isAlive = true;

		#endregion

		#region Properties

		public Entity Target
		{
			get { return target; }
			set 
			{
				target = value;

				foreach(KeyValuePair<string, Attack> entry in attackList)
				{
					entry.Value.Target = target;
				}

				((LookAtTarget)transform.GetComponent<LookAtTarget>()).Target = target;
			}
		}

		public bool IsAlive
		{
			get { return isAlive; }
			set { isAlive = value;}
		}

		#endregion

		#region Initialization

		public override void Awake()
		{
			attackList = new Dictionary<string, Attack>();

			if (attackNameList.Length > 0)
			{
				AttackReader attackReader = new AttackReader();

				for (int i = 0; i < attackNameList.Length; i++)
				{
					Attack newAttack = attackReader.LoadAttack(attackNameList[i], this);

					attackList.Add(attackNameList[i], newAttack);
				}

				currentAttack = attackList[attackNameList[0]];
			}
		}

		public override void Start()
		{
			DamageTaker.OnDeathEvent += OnEntityDeath;
			
			EnemyManager.Instance.AddEnemy(this);
		}
		
		public override void OnDestroy()
		{
			DamageTaker.OnDeathEvent -= OnEntityDeath;
		}

		#endregion

		#region Game Loop

		public override void Update()
		{
			if (!readyToAttack)
			{
				attackTimer += Time.deltaTime;
				
				if (attackTimer >= currentAttack.Delay)
				{
					attackTimer = 0.0f;
					readyToAttack = true;
				}
			}

			else
			{
				if (target != null && IsInAttackRange())
				{
					currentAttack.StartAttack();
					readyToAttack = false;
				}
			}
		}

		void FixedUpdate()
		{

		}

		#endregion

		#region Public Methods

		public void OnTriggerEnter(Collider other)
		{
			
		}
		
		public void OnEntityDeath(Entity entity)
		{
			if (transform == entity.transform)
			{
				rigidbody.useGravity = true;
				rigidbody.isKinematic = false;
				collider.isTrigger = false;
				
				MovingObject movingObject = transform.GetComponent<MovingObject>();
				if (movingObject != null)
					movingObject.MoveSpeed = 0.0f;
				
				gameObject.GetComponent<MovingObject>().enabled = false;
				gameObject.GetComponent<FollowTarget>().enabled = false;
			}
		}

		public float GetDistanceToTarget()
		{
			if (target == null)
				return 0.0f;

			float dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(target.transform.position.x, target.transform.position.z));

			dist -= target.radius;

			return dist;
		}

		bool IsInAttackRange()
		{
			bool inRange = false;

			if (currentAttack != null)
				inRange = (GetDistanceToTarget() <= currentAttack.MaxRange && GetDistanceToTarget() >= currentAttack.MinRange);

			return inRange;
		}

		#endregion

		#region Private Methods

		#endregion
	}
}
