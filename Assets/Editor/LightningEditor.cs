using UnityEngine;
using UnityEditor;
using AssemblyCSharp;

namespace AssemblyCSharpEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Lightning))]
	public class LightningEditor : Editor
	{
		private Lightning lightning;
		
		private SerializedObject lightningObject;
		private SerializedProperty startPos;
		private SerializedProperty endPos;
		private SerializedProperty startThickness;
		private SerializedProperty thicknessGrowth;
		private SerializedProperty segmentCount;
		private SerializedProperty jaggedness;
		private SerializedProperty sway;
		private SerializedProperty branchiness;
		private SerializedProperty branchLengthMultiplier;
		private SerializedProperty useDistForBranchLength;
		private SerializedProperty maxBranchDepth;
		private SerializedProperty branchAngle;
		private SerializedProperty branchAngleRange;
		private SerializedProperty branchThicknessMultiplier;
		private SerializedProperty spawnTime;
		private SerializedProperty fadeTime;
				
		public void OnEnable()
		{
			lightning = (Lightning)target;
			
			lightningObject = new SerializedObject(target);
			startPos = lightningObject.FindProperty("startPosition");
			endPos = lightningObject.FindProperty("endPosition");
			startThickness = lightningObject.FindProperty("startThickness");
			thicknessGrowth = lightningObject.FindProperty("thicknessGrowth");
			segmentCount = lightningObject.FindProperty("segmentCount");
			jaggedness = lightningObject.FindProperty("jaggedness");
			sway = lightningObject.FindProperty("sway");
			branchiness = lightningObject.FindProperty("branchiness");
			branchLengthMultiplier = lightningObject.FindProperty("branchLengthMultiplier");
			useDistForBranchLength = lightningObject.FindProperty("useDistForBranchLength");
			maxBranchDepth = lightningObject.FindProperty("maxBranchDepth");
			branchAngle = lightningObject.FindProperty("branchAngle");
			branchAngleRange = lightningObject.FindProperty("branchAngleRange");
			branchThicknessMultiplier = lightningObject.FindProperty("branchThicknessMultiplier");
			spawnTime = lightningObject.FindProperty("spawnTime");
			fadeTime = lightningObject.FindProperty("fadeTime");
		}
		
		public override void OnInspectorGUI()
		{
			if (lightning == null)
				return;
			
			lightningObject.Update();
			
			EditorGUILayout.PropertyField(startPos);
			EditorGUILayout.PropertyField(endPos);
			EditorGUILayout.PropertyField(startThickness);
			EditorGUILayout.PropertyField(thicknessGrowth);
			EditorGUILayout.PropertyField(segmentCount);
			EditorGUILayout.PropertyField(jaggedness);
			EditorGUILayout.PropertyField(sway);
			EditorGUILayout.PropertyField(branchiness);
			EditorGUILayout.PropertyField(branchLengthMultiplier);
			EditorGUILayout.PropertyField(useDistForBranchLength);
			EditorGUILayout.PropertyField(maxBranchDepth);
			EditorGUILayout.PropertyField(branchAngle);
			EditorGUILayout.PropertyField(branchAngleRange);
			EditorGUILayout.PropertyField(branchThicknessMultiplier);
			EditorGUILayout.PropertyField(spawnTime);
			EditorGUILayout.PropertyField(fadeTime);
			
			if(lightningObject.ApplyModifiedProperties())
			{
				lightning.GenerateLightning();
			}
		}
	}
}