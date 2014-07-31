using UnityEngine;
using System.Collections;
using System.Xml;

namespace AssemblyCSharp
{
	public class AttackReader
	{
		#region Variables

		private string xmlDocName = "";

		private XmlDocument xmlDoc = null;

		#endregion
		
		#region Properties

		public string XmlDocumentName { get { return xmlDocName; } }

		#endregion

		#region Initialization

		public AttackReader()
		{
			xmlDocName = "Assets/Turret Game Assets/Data/Attacks.xml";
		}

		#endregion

		#region Public Methods

		public void SetAttackFile(string fileName)
		{
			xmlDocName = fileName;

			xmlDoc = new XmlDocument();
			xmlDoc.Load(fileName);
		}

		public virtual Attack LoadAttack(string attackName, Transform owner)
		{
			Attack attack = null;

			LoadAttack(attackName, ref attack);

			if (attack != null)
				attack.Owner = owner;

			return attack;
		}

		protected virtual void LoadAttack(string attackName, ref Attack attack)
		{
			if (xmlDoc == null)
			{
				xmlDoc = new XmlDocument();
				xmlDoc.Load(xmlDocName); 
			}
			
			XmlNode rootNode = xmlDoc.FirstChild;
			XmlNodeList xmlAttackList = rootNode.SelectNodes("//Attacks/Attack");

			XmlNode node = findAttackNode(xmlAttackList, attackName);

			if (node != null)
			{
				if (attack == null)
				{
					if (node.SelectSingleNode("CustomClass") != null)
					{
						attack = CreateCustomAttack(node.SelectSingleNode("CustomClass").InnerText);
					}
					else
					{
						attack = new Attack();
					}
				}

				attack.AttackName = attackName;

	            if (node.SelectSingleNode("Base") != null)
	            {
	                string baseNodeName = node.SelectSingleNode("Base").InnerText;

					LoadAttack(baseNodeName, ref attack);
	            }

				ReadValuesFromXmlNode(node, attack);
			}
			else
			{
				Debug.Log("Warning: Unrecognized attack name " + attackName);
			}
		}

		#endregion

		#region Private Methods

		private XmlNode findAttackNode(XmlNodeList attackList, string attackName)
		{
			for (int i = 0; i < attackList.Count; i++)
			{
				if (attackList[i].SelectSingleNode("Name").InnerText == attackName)
				{
					return attackList[i];
				}
			}
			
			return null;
		}

		private void ReadValuesFromXmlNode(XmlNode node, Attack attack)
		{
			string attackName = node.SelectSingleNode("Name").InnerText;
			
			bool log = false;
			
			if (node.SelectSingleNode("IsMelee") != null)
			{
				attack.IsMelee = node.SelectSingleNode("IsMelee").InnerText == "true";
				
				if (log) Debug.Log(attackName + " changed isRanged to " + attack.IsMelee);
			}
			
			if (node.SelectSingleNode("Damage") != null)
			{
				Vector2 damage = Vector2.zero;

				damage.x = XmlConvert.ToInt32(node.SelectSingleNode("Damage").SelectSingleNode("Min").InnerText);
				damage.y = XmlConvert.ToInt32(node.SelectSingleNode("Damage").SelectSingleNode("Max").InnerText);

				attack.Damage = damage;

				if (log) Debug.Log(attackName + " changed damage to " + attack.Damage.x + ", " + attack.Damage.y);
			}
			
			if (node.SelectSingleNode("Delay") != null)
			{
				attack.Delay = (float)XmlConvert.ToDouble(node.SelectSingleNode("Delay").InnerText);
				
				if (log) Debug.Log(attackName + " changed delay to " + attack.Delay);
			}
			
			if (node.SelectSingleNode("MinRange") != null)
			{
				attack.MinRange = (float)XmlConvert.ToDouble(node.SelectSingleNode("MinRange").InnerText);
				
				if (log) Debug.Log(attackName + " changed minRange to " + attack.MinRange);
			}
			
			if (node.SelectSingleNode("MaxRange") != null)
			{
				attack.MaxRange = (float)XmlConvert.ToDouble(node.SelectSingleNode("MaxRange").InnerText);
				
				if (log) Debug.Log(attackName + " changed maxRange to " + attack.MaxRange);
			}
			
			if (node.SelectSingleNode("Accuracy") != null)
			{
				attack.Accuracy = (float)XmlConvert.ToDouble(node.SelectSingleNode("Accuracy").InnerText);
				
				if (log) Debug.Log(attackName + " changed accuracy to " + attack.Accuracy);
			}

			if (node.SelectSingleNode("ProjectileSpeedMultiplier") != null)
			{
				attack.ProjectileSpeedMultiplier = (float)XmlConvert.ToDouble(node.SelectSingleNode("ProjectileSpeedMultiplier").InnerText);
				
				if (log) Debug.Log(attackName + " changed projectileSpeedMultiplier to " + attack.ProjectileSpeedMultiplier);
			}

			if (node.SelectSingleNode("ProjectilePrefab") != null)
			{
				attack.ProjectilePrefabName = node.SelectSingleNode("ProjectilePrefab").InnerText;
				
				if (log) Debug.Log(attackName + " changed projectilePrefabName to " + attack.ProjectilePrefabName);
			}
		}

		protected Attack CreateCustomAttack(string customClassName)
		{
			if (customClassName == "MGMultiplyAttack")
				return new MGMultiplyAttack();

			Debug.Log("Warning: Unrecognized custom attack class name " + customClassName);

			return new Attack();
		}

		#endregion
	}
}

