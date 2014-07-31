using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class ProjectileManager : MonoBehaviour
	{
		#region Variables
		
		private static ProjectileManager instance;
		ArrayList projectileList;
		
		#endregion
		
		#region Properties
		
		public static ProjectileManager Instance
		{
			get;
			private set;
		}
		
		public ArrayList ProjectileList
		{
			get { return projectileList; }
		}
		
		public int ProjectileCount
		{
			get { return projectileList.Count; }
		}
		
		#endregion
		
		#region initialization
		
		public void Start ()
		{

		}
		
		public void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
			}
			
			Instance = this;
			DontDestroyOnLoad(gameObject);
			
			projectileList = new ArrayList();
		}
		
		#endregion
		
		#region Game Loop
		
		public void Update ()
		{

		}
		
		#endregion
		
		#region Public Methods
		
		public void OnApplicationQuit()
		{
			instance = null;
		}
		
		public void AddProjectile(Projectile projectile)
		{
			if (projectileList.IndexOf(projectile) == -1)
				projectileList.Add(projectile);
		}
		
		public void RemoveProjectile(Projectile projectile, bool cleanUp)
		{
			if (projectileList.IndexOf(projectile) != -1)
				projectileList.Remove(projectile);
			
			if (cleanUp)
				Destroy(projectile.gameObject);
		}
		
		public void RemoveAllProjectiles(bool cleanUp)
		{
			if (cleanUp)
			{
				for (int i = 0; i < projectileList.Count; i++)
					Destroy(((Projectile)projectileList[i]).gameObject);
			}
			
			projectileList.Clear();
		}

		public ArrayList GetAllProjectilesOfType(ProjectileType type)
		{
			ArrayList list = new ArrayList();
			Projectile projectile = null;

			for (int i = 0; i < projectileList.Count; i++)
			{
				projectile = (Projectile)projectileList[i];

				if (projectile.ProjectileType == type)
					list.Add(projectile);
			}

			return list;
		}
		
		#endregion
		
		#region Private Methods
		
		
		
		#endregion
	}
}