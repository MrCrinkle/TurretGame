using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class EnemyController : MonoBehaviour
	{
		GameObject enemy;
		Enemy enemyComponent;
		
		bool atTarget = false;
		
		void Start ()
		{
				enemy = transform.gameObject;
				enemyComponent = enemy.GetComponent<Enemy>();
		}
	
		void Update ()
		{
			
		}
	}
}
