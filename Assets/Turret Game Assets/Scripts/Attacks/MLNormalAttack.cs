using UnityEngine;
using System.Collections;
using System.Xml;

namespace AssemblyCSharp
{
	public class MLNormalAttack : Attack
	{
		#region Variables

		Vector3 currentTargetPosition = Vector3.zero;

		#endregion
		
		#region Properties
		
		#endregion
		
		#region Initialization
		
		#endregion
		
		#region Game Loop
		
		#endregion
		
		#region Public Methods
		
		public override ArrayList StartAttack(Quaternion direction, Vector3 position)
		{
			FindTargetPosition();

			ArrayList projectiles = new ArrayList();
			
			projectiles.Add(SpawnProjectile(direction, position));

			return projectiles;
		}

		protected override GameObject SpawnProjectile(Quaternion direction, Vector3 position)
		{
			GameObject newProjectile = (GameObject)GameObject.Instantiate(Resources.Load(projectilePrefabName));
			newProjectile.transform.GetComponent<Projectile>().Source = owner;

			MineProjectile mine = newProjectile.transform.GetComponent<MineProjectile>();

			mine.StartPosition = new Vector3(currentTargetPosition.x, 5.0f, currentTargetPosition.z);
			mine.TargetPosition = new Vector3(currentTargetPosition.x, 0.0f, currentTargetPosition.z);

			newProjectile.transform.rotation = direction;

			return newProjectile;
		}

		public override void ReadCustomXmlProperties(XmlNode node)
		{
			
		}
		
		#endregion
		
		#region Private Methods

		protected void FindTargetPosition()
		{
			Vector3 mousePos = Input.mousePosition;
			Ray mouseVector = Camera.main.ScreenPointToRay(mousePos);
			RaycastHit raycastHit;
			
			if (World.Instance.terrainRef.collider.Raycast(mouseVector, out raycastHit, 1000.0f))
			{
				currentTargetPosition = raycastHit.point;	
			}
		}


		#endregion
	}
}
