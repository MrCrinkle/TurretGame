using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class Enemy : MonoBehaviour
	{
		public float size = 0.5f;
		public Attack[] attacks;
		public Transform target = null;
		public int spawnValue = 2; // "value" of the enemy when spawning. More difficult enemies have higher values.
		
		bool isAlive = true;
		
		public bool IsAlive
		{
			get { return isAlive; }
			set { isAlive = value;}
		}
		
		void Start()
		{
			DamageTaker.OnDeathEvent += OnEntityDeath;
			
			EnemyManager.Instance.AddEnemy(this);
		}
		
		void OnDestroy()
		{
			DamageTaker.OnDeathEvent -= OnEntityDeath;
		}
		
		void Update()
		{
			
		}

		void FixedUpdate()
		{

		}
		
		void OnTriggerEnter(Collider other)
		{
			
		}
		
		void OnEntityDeath(GameObject obj)
		{
			if (transform == obj.transform)
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
	}
}
