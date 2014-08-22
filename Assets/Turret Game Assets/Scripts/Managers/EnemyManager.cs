using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class EnemyManager : MonoBehaviour
	{
		#region Variables

		private static EnemyManager instance;

		public float enemySpawnDist = 45.0f;
		public int totalValueToSpawn = 20; // amount of enemies to spawn based on the enemies' value

		public GameObject[] enemyPrefabList;

		ArrayList enemyList;

		float spawnChance = 0.5f; // chance to spawn an enemy per second
		float maxSpawnDelay = 2.0f; // maximum seconds between spawns
		int maxSpawnsPerSecond = 1; // maximum number of enemies that can spawn per second


		int totalValueSpawned = 0;
		float timeSinceSpawn = 0.0f;
		int totalSpawnedThisSecond = 0;
		float secondTimer = 0.0f;

		int numEnemyTypes = 0;

		#endregion
		
		#region Properties
		
		public static EnemyManager Instance
		{
			get { return instance; }
			private set { instance = value; }
		}

		public ArrayList EnemyList
		{
			get { return enemyList; }
		}

		public int EnemyCount
		{
			get { return enemyList.Count; }
		}

		public float SpawnChance
		{
			get { return spawnChance; }
			set { spawnChance = Mathf.Clamp(value, 0.0f, 1.0f); }
		}

		public float MaxSpawnDelay
		{
			get { return maxSpawnDelay; }
			set { maxSpawnDelay = Mathf.Max(0.0f, value); }
		}

		public int MaxSpawnsPerSecond
		{
			get { return maxSpawnsPerSecond; }
			set { maxSpawnsPerSecond = Mathf.Max(0, value); }
		}

		public int TotalValueToSpawn
		{
			get { return totalValueToSpawn; }
			set { totalValueToSpawn = (int)Mathf.Max(0, value); }
		}

		public int TotalValueSpawned
		{
			get { return totalValueSpawned; }
		}

		public int NumEnemyTypes
		{
			get { return numEnemyTypes; }
		}
		
		#endregion
		
		#region initialization
		
		public void Start ()
		{
			numEnemyTypes = enemyPrefabList.Length;
		}
		
		public void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
			}
			
			Instance = this;
			DontDestroyOnLoad(gameObject);
			
			enemyList = new ArrayList();
		}
		
		#endregion
		
		#region Game Loop
		
		public void Update ()
		{
			if (totalValueSpawned < totalValueToSpawn) 
			{
				bool spawnEnemy = false;

				if (secondTimer >= 0.0f)
					secondTimer += Time.deltaTime;

				if (secondTimer >= 1.0f)
				{
					totalSpawnedThisSecond = 0;
					secondTimer = -1.0f;
				}

				if (totalSpawnedThisSecond < maxSpawnsPerSecond)
				{
					timeSinceSpawn += Time.deltaTime;

					// force spawn if over max spawn delay
					if (timeSinceSpawn >= maxSpawnDelay)
						spawnEnemy = true;
				
					// if we haven't spawned, check for random spawn chance
					if (!spawnEnemy)
					{
						float frameSpawnChance = spawnChance * Time.deltaTime;
						float num = Random.Range (0.0f, 1.0f);

						if (frameSpawnChance > num) {
							spawnEnemy = true;
						}
					}
				}

				if (spawnEnemy)
				{
					int enemyType = Random.Range(0, numEnemyTypes);

					SpawnEnemy(enemyType);
				}
			}
		}
		
		#endregion
		
		#region Public Methods
		
		public void OnApplicationQuit()
		{
			instance = null;	
		}
		
		public void AddEnemy(Enemy enemy)
		{
			if (enemyList.IndexOf(enemy) == -1)
				enemyList.Add(enemy);
		}
		
		public void RemoveEnemy(Enemy enemy, bool cleanUp)
		{
			if (enemyList.IndexOf(enemy) != -1)
				enemyList.Remove(enemy);
			
			if (cleanUp)
				Destroy(enemy.gameObject);
		}
		
		public void RemoveAllEnemies(bool cleanUp)
		{
			if (cleanUp)
			{
				for (int i = 0; i < enemyList.Count; i++)
					Destroy(((Enemy)enemyList[i]).gameObject);
			}
			
			enemyList.Clear();
		}

		public Enemy CheckForEnemyAtMousePosition()
		{
			// get a ray from the mouse position
			Vector3 mousePos = Input.mousePosition;
			Ray mouseVector = Camera.main.ScreenPointToRay(mousePos);

			Collider enemyCollider = null;
			Transform enemy = null;
			RaycastHit raycastHit;
			
			ArrayList enemiesHit = new ArrayList();
			Transform enemyHit = null;
			
			for (int i = 0; i < enemyList.Count; i++)
			{
				enemy = ((Enemy)enemyList[i]).transform;
				enemyCollider = enemy.collider;
				
				if (enemyCollider == null)
					continue;
				
				// check raycast against enemy and add it to the list if it hits
				if (enemyCollider.Raycast(mouseVector, out raycastHit, 1000.0f))
					enemiesHit.Add(enemy);
			}
			
			if (enemiesHit.Count != 0)
				enemyHit = (Transform)enemiesHit[0];

			if (enemyHit != null)
				return enemyHit.GetComponent<Enemy>();

			return null;
		}

		GameObject SpawnEnemy(int type)
		{
			if (enemyPrefabList.Length - 1 < type || enemyPrefabList [type] == null)
				return null;

			float angle = Random.Range (0.0f, Mathf.PI * 2.0f);
			Vector3 position = new Vector3 (Mathf.Cos (angle) * enemySpawnDist, 0.0f, Mathf.Sin (angle) * enemySpawnDist);
			Vector3 eulerDirection = Vector3.zero - position;
			eulerDirection.y = 0.0f;
			Quaternion direction = Quaternion.LookRotation (eulerDirection);

			GameObject newEnemy = (GameObject)GameObject.Instantiate(enemyPrefabList[type], position, direction);
			newEnemy.GetComponent<Enemy>().Target = World.Instance.baseRef.GetComponent<Entity>();

			Enemy enemyComponent = newEnemy.GetComponent<Enemy>();

			totalValueSpawned += enemyComponent.spawnValue;
			totalSpawnedThisSecond++;
			secondTimer = 0.0f;
			timeSinceSpawn = 0.0f;

			return newEnemy;
		}
		
		#endregion
		
		#region Private Methods
		
		
		
		#endregion
	}
}