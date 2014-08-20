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
			{
				attack.AttackName = attackName;
				attack.Owner = owner;
			}

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
					string customClassName = "";
					bool doneSearching = false;
					XmlNode nodeToCheck = node;	

					while(!doneSearching && nodeToCheck != null)
					{
						if (nodeToCheck.SelectSingleNode("CustomClass") != null && nodeToCheck.SelectSingleNode("CustomClass").InnerText != "None")
						{
							customClassName = nodeToCheck.SelectSingleNode("CustomClass").InnerText;
							doneSearching = true;
						}
						else if (nodeToCheck.SelectSingleNode("Base") != null)
						{
							nodeToCheck = findAttackNode(xmlAttackList, nodeToCheck.SelectSingleNode("Base").InnerText);
						}
						else
						{
							doneSearching = true;
						}
					}

					if (customClassName != "")
					{
						attack = CreateCustomAttack(customClassName);
					}
					else
					{
						attack = new Attack();
					}
				}

				if (node.SelectSingleNode("Base") != null && node.SelectSingleNode("Base").InnerText != "None")
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
				attack.IsMelee = XmlConvert.ToBoolean(node.SelectSingleNode("IsMelee").InnerText);
				
				if (log) Debug.Log(attackName + " changed isRanged to " + attack.IsMelee);
			}
			
			if (node.SelectSingleNode("MinDamage") != null)
			{
				attack.Damage = new Vector2(XmlConvert.ToInt32(node.SelectSingleNode("MinDamage").InnerText), attack.Damage.y);

				if (log) Debug.Log(attackName + " changed minDamage to " + attack.Damage.x);
			}

			if (node.SelectSingleNode("MaxDamage") != null)
			{
				attack.Damage = new Vector2(attack.Damage.y, XmlConvert.ToInt32(node.SelectSingleNode("MaxDamage").InnerText));

				if (log) Debug.Log(attackName + " changed maxDamage to " + attack.Damage.y);
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

			attack.ReadCustomXmlProperties(node);
		}

		protected Attack CreateCustomAttack(string customClassName)
		{
			if (customClassName == "MGMultiplyAttack")
				return new MGMultiplyAttack();
			else if (customClassName == "RLPrecisionAttack")
				return new RLPrecisionAttack();
			else if (customClassName == "RLMultiplyAttack")
				return new RLMultiplyAttack();
			else if (customClassName == "LGPrecisionAttack")
				return new LGPrecisionAttack();
			else if (customClassName == "GCPrecisionAttack")
				return new GCPrecisionAttack();
			else if (customClassName == "MLNormalAttack")
				return new MLNormalAttack();
			else if (customClassName == "MLMultiplyAttack")
				return new MLMultiplyAttack();

			Debug.Log("Warning: Unrecognized custom attack class name " + customClassName);

			return new Attack();
		}

		#endregion
	}
}

