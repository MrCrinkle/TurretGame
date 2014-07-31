using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class LightningCollider : MonoBehaviour
	{
		#region Variables
		
		protected ArrayList collidingEnemies;
		protected ArrayList ignoreList;
		protected bool hasCheckedCollision = false;
		protected bool hasHitEnemies = false;
		protected int updateCount = 0;

		#endregion
		
		#region Properties
		
		public ArrayList CollidingEnemies { get { return collidingEnemies; } }
		public bool HasCheckedCollision { get { return hasCheckedCollision; } }
		public bool HasHitEnemies { get { return hasHitEnemies; } }

		public void AddIgnoreEnemy(Transform enemy)
		{
			if (enemy.GetComponent<Enemy>() != null)
				ignoreList.Add(enemy);
		}

		#endregion
		
		#region initialization
		
		void Awake()
		{
			collidingEnemies = new ArrayList();
			ignoreList = new ArrayList();
		}
		
		#endregion
		
		#region Game Loop
		
		void Update ()
		{
		
		}
		
		public void FixedUpdate()
		{
			updateCount++;
			
			if (updateCount == 2)
			{
				hasCheckedCollision = true;
				
				if (collidingEnemies.Count > 0)
					hasHitEnemies = true;
			}
		}
		
		#endregion
		
		#region Public Methods
		
		public virtual void OnCollisionEnter(Collision collision)
		{
			if (collision.collider.transform.GetComponent<Enemy>() != null && !ignoreList.Contains(collision.transform))
			{
				collidingEnemies.Add(collider.transform);
			}
		}
		
		public virtual void OnTriggerEnter(Collider other)
		{
			if (other.transform.GetComponent<Enemy>() != null && !ignoreList.Contains(other.transform))
			{
				collidingEnemies.Add(other.transform);
			}
		}
		
		#endregion
		
		#region Private Methods
		
		#endregion
	}
}
