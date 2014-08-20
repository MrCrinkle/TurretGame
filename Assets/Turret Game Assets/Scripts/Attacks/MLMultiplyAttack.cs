using UnityEngine;
using System.Collections;
using System.Xml;

namespace AssemblyCSharp
{
	public class MLMultiplyAttack : MLNormalAttack
	{
		#region Variables
		
		Vector3 currentTargetPosition = Vector3.zero;
		int numMines = 3;
		float mineSpread = 2.0f;
		
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

			for (int i = 0; i < numMines; i++)
			{
				GameObject mine = SpawnProjectile(direction, position);
				MineProjectile mineComponent = mine.transform.GetComponent<MineProjectile>();

				float angle = ((360 / numMines) * (i + 1)) * Mathf.Deg2Rad;
				float x = Mathf.Cos(angle) * mineSpread;
				float y = Mathf.Sin(angle) * mineSpread;

				//mineComponent.TargetPosition = new Vector3(mineComponent.TargetPosition.x + x, mineComponent.TargetPosition.y, mineComponent.TargetPosition.z + y);

				mineComponent.TargetPosition = mineComponent.TargetPosition + new Vector3(x, 0.0f, y);

				//mineComponent.TargetPosition = mineComponent.TargetPosition + new Vector3(2.0f * i, 0.0f, 0.0f);

				projectiles.Add(mine);
			}
			
			return projectiles;
		}
		
		public override void ReadCustomXmlProperties(XmlNode node)
		{
			
		}
		
		#endregion
		
		#region Private Methods
		
		
		#endregion
	}
}
