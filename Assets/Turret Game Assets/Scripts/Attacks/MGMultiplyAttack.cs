using UnityEngine;
using System.Collections;
using System.Xml;

namespace AssemblyCSharp
{
	public class MGMultiplyAttack : Attack
	{
		#region Variables
		
		float multiplyAngle;
		float multiplyDamage;
		
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
				
				projectiles.Add(SpawnProjectile(direction, position));

				float xDist = Mathf.Tan(Mathf.Deg2Rad * multiplyAngle);

				GameObject leftShot = SpawnProjectile(direction, position);
				leftShot.transform.localScale = new Vector3(1.2f, 1.0f, 1.2f);
				leftShot.transform.rotation = Quaternion.LookRotation(direction * new Vector3(-xDist, 0.0f, 1.0f));
				((DamageDealer)leftShot.GetComponent<DamageDealer>()).MultiplyDamage(multiplyDamage);
				
				GameObject rightShot = SpawnProjectile(direction, position);
				rightShot.transform.localScale = new Vector3(1.2f, 1.0f, 1.2f);
				rightShot.transform.rotation = Quaternion.LookRotation(direction * new Vector3(xDist, 0.0f, 1.0f));
				((DamageDealer)rightShot.GetComponent<DamageDealer>()).MultiplyDamage(multiplyDamage);

				return projectiles;
			}
			
			return null;
		}

		public override void ReadCustomXmlProperties(XmlNode node)
		{
			bool log = false;

			if (node.SelectSingleNode("MultiplyAngle") != null)
			{
				multiplyAngle = (float)XmlConvert.ToDouble(node.SelectSingleNode("MultiplyAngle").InnerText);
				
				if (log) Debug.Log(attackName + " changed multiplyAngle to " + multiplyAngle);
			}

			if (node.SelectSingleNode("MultiplyDamage") != null)
			{
				multiplyDamage = (float)XmlConvert.ToDouble(node.SelectSingleNode("MultiplyDamage").InnerText);
				
				if (log) Debug.Log(attackName + " changed multiplyDamage to " + multiplyDamage);
			}
		}

		#endregion
		
		#region Private Methods
		
		#endregion
	}
}

