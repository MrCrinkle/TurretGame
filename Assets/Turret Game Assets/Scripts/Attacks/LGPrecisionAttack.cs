using UnityEngine;
using System.Collections;
using System.Xml;

namespace AssemblyCSharp
{
	public class LGPrecisionAttack : Attack
	{
		#region Variables

		public float hitDelay = 0.2f;

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
			ArrayList projectiles = new ArrayList();

			projectiles = base.StartAttack(direction, position);

			((GameObject)projectiles[0]).GetComponent<LightningProjectile>().PrecisionModHitDelay = hitDelay;

			return projectiles;
		}
		
		public override void ReadCustomXmlProperties(XmlNode node)
		{
			bool log = false;
			
			if (node.SelectSingleNode("HitDelay") != null)
			{
				hitDelay = (float)XmlConvert.ToDouble(node.SelectSingleNode("HitDelay").InnerText);
				
				if (log) Debug.Log(attackName + " changed hitDelay to " + hitDelay);
			}
		}
		
		#endregion
		
		#region Private Methods
		
		#endregion
	}
}
