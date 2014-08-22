using UnityEngine;
using System.Collections;
using System.Xml;

namespace AssemblyCSharp
{
	public class RLPrecisionAttack : Attack
	{
		#region Variables
		
		float precisionRocketSpeed;
		
		#endregion
		
		#region Properties
		
		
		
		#endregion
		
		#region Initialization
		
		#endregion
		
		#region Game Loop
		
		#endregion
		
		#region Public Methods

		protected override GameObject SpawnProjectile(Quaternion direction, Vector3 position)
		{
			GameObject newProjectile = base.SpawnProjectile(direction, position);

			FollowTarget followTarget = (FollowTarget)newProjectile.AddComponent(typeof(FollowTarget));
			followTarget.Target = target;
			followTarget.ignoreVertical = true;
			followTarget.maxTurnSpeed = 200.0f;

			MovingObject movingObject = newProjectile.GetComponent<MovingObject>();

			if (movingObject) 
				movingObject.maxMoveSpeed = precisionRocketSpeed;

			return newProjectile;
		}

		public override void ReadCustomXmlProperties(XmlNode node)
		{
			bool log = false;
			
			if (node.SelectSingleNode("PrecisionRocketSpeed") != null)
			{
				precisionRocketSpeed = (float)XmlConvert.ToDouble(node.SelectSingleNode("PrecisionRocketSpeed").InnerText);
				
				if (log) Debug.Log(attackName + " changed precisionRocketSpeed to " + precisionRocketSpeed);
			}
		}
		
		#endregion
		
		#region Private Methods
		
		#endregion
	}
}
