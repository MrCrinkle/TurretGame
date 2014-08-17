using UnityEngine;
using System.Collections;
using System.Xml;

namespace AssemblyCSharp
{
	public class GCPrecisionAttack : Attack
	{
		#region Variables

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
			Transform currentTarget = null;
			Transform target = EnemyManager.Instance.CheckForEnemyAtMousePosition();
			
			if (target != null && target.GetComponent<Enemy>() != null)
			{
				DamageTaker damageTaker = target.GetComponent<DamageTaker>();
				
				if(damageTaker != null && damageTaker.IsAlive)
					currentTarget = target;
			}
			
			ArrayList projectiles = new ArrayList();
			
			projectiles = base.StartAttack(direction, position);
			
			((GameObject)projectiles[0]).GetComponent<GaussProjectile>().Target = currentTarget;
			
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
