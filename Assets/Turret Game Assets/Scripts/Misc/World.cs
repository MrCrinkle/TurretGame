using System;
using System.Collections;
using UnityEngine;

namespace AssemblyCSharp
{
	public class World : MonoBehaviour
	{
		private static World instance;

		public Transform terrainRef = null;
		public Transform baseRef = null;

		public float standardCollisionHeight = 1.15f;
		
		public static World Instance
		{
			get;
			private set;
		}
		
		public void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
			}
			
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}
}