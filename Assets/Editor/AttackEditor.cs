using UnityEngine;
using UnityEditor;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharpEditor
{
	public class AttackEditor : EditorWindow
	{
		private static string xmlDocName = "";
		private static XmlDocument xmlDoc = null;

		private const string defaultAttackName = "DefaultAttack";
		private Vector2 scrollPosition = Vector3.zero;

		private static ArrayList foldoutStates; 

		private string customPropertyName = "CustomPropertyName";

		[MenuItem("Custom/Attack Editor")]
		static void Init()
		{
			AttackEditor window = EditorWindow.GetWindow<AttackEditor>(true, "Attack Editor");

			window.minSize = new Vector2(400, 200);

			xmlDocName = "Assets/Turret Game Assets/Data/Attacks.xml";

			xmlDoc = new XmlDocument();
			xmlDoc.Load(xmlDocName);

			foldoutStates = new ArrayList();
		}

		void OnGUI()
		{
			float buttonWidth = 80;
			float buttonHeight = 20;

			EditorGUILayout.BeginVertical();

			EditorGUILayout.BeginHorizontal();

			GUILayout.Space(position.width - 190);

			if (GUILayout.Button("Revert", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
				onRevertButtonPressed();

			GUILayout.Space(20);

			if(GUILayout.Button("Save", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
				onSaveButtonPressed();

			EditorGUILayout.EndHorizontal();

			GUILayout.Space(20);

			scrollPosition =  EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height - 100));

			XmlNode rootNode = xmlDoc.FirstChild;
			XmlNodeList xmlAttackList = rootNode.SelectNodes("//Attacks/Attack");

			string name = "";
			XmlNode node = null;

			for (int i = 0; i < xmlAttackList.Count; i++)
			{
				node = xmlAttackList[i];
				name = node.SelectSingleNode("Name").InnerText;

				while(foldoutStates.Count - 1 < xmlAttackList.Count)
				{
					foldoutStates.Add(false);
				}

				EditorGUILayout.BeginHorizontal();

				GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
				foldoutStyle.fontStyle = FontStyle.Bold;

				foldoutStates[i] = (bool)EditorGUILayout.Foldout((bool)foldoutStates[i], name, foldoutStyle);

				if(GUILayout.Button(new GUIContent("X", "Delete attack"), GUILayout.Width(buttonHeight - 2), GUILayout.Height(buttonHeight - 2)))
					onDeleteAttackButtonPressed(node);

				EditorGUILayout.EndHorizontal();

				if ((bool)foldoutStates[i])
				{
					createAttackGUI(node);
				}
			}
			
			EditorGUILayout.EndScrollView();

			GUILayout.Space(20);

			EditorGUILayout.BeginHorizontal();

			GUILayout.Space((int)(position.width / 2) - 50);

			if (GUILayout.Button("Add Attack", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
				onAddAttackPressed();

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndVertical();
		}

		private void createAttackGUI(XmlNode node)
		{
			EditorGUI.indentLevel++;

			createPropertyGUI(node, "Name", "string");
			createPropertyGUI(node, "Base", "string");
			createPropertyGUI(node, "IsMelee", "bool");
			createPropertyGUI(node, "ProjectilePrefab", "string");
			createPropertyGUI(node, "MinDamage", "int");
			createPropertyGUI(node, "MaxDamage", "int");
			createPropertyGUI(node, "Delay", "float");
			createPropertyGUI(node, "MinRange", "float");
			createPropertyGUI(node, "MaxRange", "float");
			createPropertyGUI(node, "Accuracy", "float");
			createPropertyGUI(node, "ProjectileSpeedMultiplier", "float");
			createPropertyGUI(node, "CustomClass", "string");

			if (node.SelectSingleNode("CustomClass") != null)
				createCustomPropertyGUI(node);

			EditorGUILayout.BeginHorizontal();

			GUILayout.Space(20);

			if (node.SelectSingleNode("Name").InnerText != defaultAttackName && node.SelectSingleNode("CustomClass") != null)
			{
				customPropertyName = GUILayout.TextArea(customPropertyName, GUILayout.Width(170));

				if (GUILayout.Button("Add Custom Property", GUILayout.Width(160), GUILayout.Height(20)))
					onAddCustomPropertyPressed(node);
			}

			EditorGUILayout.EndHorizontal();

			EditorGUI.indentLevel--;
		}

		private void createPropertyGUI(XmlNode node, string propertyName, string dataType)
		{
			string nodeName = node.SelectSingleNode("Name").InnerText;
			string newValue = "";
			XmlNode nodeToCheck = node;
			bool isBaseNode = false;
			bool doneSearch = false;

			if (nodeName == "DefaultAttack")
			{
				if (propertyName == "Base")
					return;

				if (propertyName == "Name" || propertyName == "CustomClass")
				{
					isBaseNode = true;
					doneSearch = true;
				}
			}

			while(!doneSearch)
			{
				if (nodeToCheck.SelectSingleNode(propertyName) != null)
				{
					doneSearch = true;
				}
				else
				{
					isBaseNode = true;

					if (nodeToCheck.SelectSingleNode("Base") != null || nodeToCheck.SelectSingleNode("Base").InnerText != "None")
					{
						string baseNodeName = nodeToCheck.SelectSingleNode("Base").InnerText;

						nodeToCheck = findAttackNode(xmlDoc.FirstChild.SelectNodes("//Attacks/Attack"), baseNodeName);

						if (nodeToCheck == null)
						{
							Debug.Log("Warning: Unable to find base attack " + baseNodeName);
							doneSearch = true;
						}
					}
					else
					{
						doneSearch = true;
					}
				}
			}

			EditorGUILayout.BeginHorizontal();

			if (nodeToCheck != null)
			{
				if (isBaseNode)
					GUI.enabled = false;

				if (dataType == "string")
				{
					newValue = EditorGUILayout.TextField(propertyName, nodeToCheck.SelectSingleNode(propertyName).InnerText, GUILayout.Width(350));
				}
				else if (dataType == "int")
				{
					newValue = EditorGUILayout.IntField(propertyName, XmlConvert.ToInt32(nodeToCheck.SelectSingleNode(propertyName).InnerText), GUILayout.Width(350)).ToString();
				}
				else if (dataType == "float")
				{
					newValue = EditorGUILayout.FloatField(propertyName, (float)XmlConvert.ToDouble(nodeToCheck.SelectSingleNode(propertyName).InnerText), GUILayout.Width(350)).ToString();;
				}
				else if (dataType == "bool")
				{
					if (EditorGUILayout.Toggle(propertyName, nodeToCheck.SelectSingleNode(propertyName).InnerText == "true", GUILayout.Width(350)))
						newValue = "true";
					else
						newValue = "false";
				}

				if (isBaseNode)
				{
					GUI.enabled = true;

					if (nodeName != defaultAttackName)
					{
						if (GUILayout.Button(new GUIContent("/", "Edit property"), GUILayout.Width(18), GUILayout.Height(18)))
							onEditPropertyPressed(node, propertyName, newValue);
					}
				}
				else
				{
					if (propertyName != "Name" && propertyName != "Base" && nodeName != defaultAttackName)
					{
						if (GUILayout.Button(new GUIContent("X", "Use Base value"), GUILayout.Width(18), GUILayout.Height(18)))
							onRemovePropertyPressed(node, propertyName);
					}
				}

				onPropertyValueChanged(nodeToCheck, propertyName, newValue);
			}

			EditorGUILayout.EndHorizontal();
		}

		void createCustomPropertyGUI(XmlNode node)
		{
			XmlNodeList properties = node.ChildNodes;
			string propertyName = "";
			XmlNode property = null;

			for (int i = 0; i < properties.Count; i++)
			{
				property = properties[i];
				propertyName = property.Name;

				if (propertyName == "Name" || propertyName == "Base" || propertyName == "IsMelee" || propertyName == "MinDamage"
				    || propertyName == "MaxDamage" || propertyName == "Delay" || propertyName == "MinRange" || propertyName == "MaxRange"
				    || propertyName == "Accuracy" || propertyName == "ProjectileSpeedMultiplier"  || propertyName == "ProjectilePrefab" 
				    || propertyName == "CustomClass")
				{
					continue;
				}

				GUILayout.BeginHorizontal();

				string newValue = EditorGUILayout.TextField(propertyName, property.InnerText, GUILayout.Width(350));

				onPropertyValueChanged(node, propertyName, newValue);

				if (GUILayout.Button(new GUIContent("X", "Remove Custom Property"), GUILayout.Width(18), GUILayout.Height(18)))
					onRemovePropertyPressed(node, propertyName);

				GUILayout.EndHorizontal();
			}
		}

		void onRevertButtonPressed()
		{
			xmlDoc.Load(xmlDocName);
		}

		void onSaveButtonPressed()
		{
			xmlDoc.Save(xmlDocName);
		}

		void onDeleteAttackButtonPressed(XmlNode node)
		{
			xmlDoc.FirstChild.RemoveChild(node);
		}

		void onAddAttackPressed()
		{
			XmlNode attackNode = xmlDoc.CreateElement("Attack");

			XmlNode nameNode = xmlDoc.CreateElement("Name");
			nameNode.InnerText = "NewAttack";

			XmlNode baseNode = xmlDoc.CreateElement("Base");
			baseNode.InnerText = defaultAttackName;

			attackNode.AppendChild(nameNode);
			attackNode.AppendChild(baseNode);

			xmlDoc.FirstChild.AppendChild(attackNode);
		}

		void onPropertyValueChanged(XmlNode node, string propertyName, string value)
		{
			if (node.SelectSingleNode(propertyName) != null)
			{
				node.SelectSingleNode(propertyName).InnerText = value;
			}
		}

		void onEditPropertyPressed(XmlNode node, string propertyName, string value)
		{
			XmlNode newNode = xmlDoc.CreateElement(propertyName);
			newNode.InnerText = value;

			node.AppendChild(newNode);
		}

		void onRemovePropertyPressed(XmlNode node, string propertyName)
		{
			node.RemoveChild(node.SelectSingleNode(propertyName));
		}

		void onAddCustomPropertyPressed(XmlNode node)
		{
			XmlNode newNode = xmlDoc.CreateElement(customPropertyName);
			newNode.InnerText = "value";

			node.AppendChild(newNode);
		}

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
	}
}

