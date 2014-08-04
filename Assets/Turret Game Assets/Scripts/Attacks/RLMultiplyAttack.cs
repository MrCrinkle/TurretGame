using UnityEngine;
using System.Collections;
using System.Xml;

namespace AssemblyCSharp
{
	public class RLMultiplyAttack : Attack
	{
		#region Variables
		
		float rocketSpacing;
		
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
			if (!isMelee && projectilePrefabName != "null" && projectilePrefabName != "")
			{
				ArrayList projectiles = new ArrayList();

				GameObject rocket1 = SpawnProjectile(direction, position);
				GameObject rocket2 = SpawnProjectile(direction, position);

				Vector3 dir = direction * Vector3.right;

				rocket1.transform.position = rocket1.transform.position + (dir * rocketSpacing);
				rocket2.transform.position = rocket2.transform.position + (dir * -rocketSpacing);

				projectiles.Add(rocket1);
				projectiles.Add(rocket2);

				return projectiles;
			}
			
			return null;
		}
		
		public override void ReadCustomXmlProperties(XmlNode node)
		{
			bool log = false;
			
			if (node.SelectSingleNode("RocketSpacing") != null)
			{
				rocketSpacing = (float)XmlConvert.ToDouble(node.SelectSingleNode("RocketSpacing").InnerText);
				
				if (log) Debug.Log(attackName + " changed rocketSpacing to " + rocketSpacing);
			}
		}
		
		#endregion
		
		#region Private Methods
		
		#endregion
	}
}
