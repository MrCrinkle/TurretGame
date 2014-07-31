using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	class Line
	{
		public Vector3 pointA;
		public Vector3 pointB;
		public float thickness;
	
		public Line(Vector3 pointA, Vector3 pointB, float thickness)
		{
			this.pointA = pointA;
			this.pointB = pointB;
			this.thickness = thickness;
		}
	}
	
	[ExecuteInEditMode, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class Lightning : MonoBehaviour
	{
		ArrayList lineList;
		
		public Vector3 startPosition = Vector3.zero;
		public Vector3 endPosition = new Vector3(0.0f, 0.0f, 10.0f);
		
		public float startThickness = 1.5f;
		public float thicknessGrowth = 0.0f;
		public float segmentCount = 15.0f;
		public float jaggedness = 1.0f;
		public float sway = 5.0f;
		public float branchiness = 2.5f;
		public float branchLengthMultiplier = 0.85f;
		public bool useDistForBranchLength = true;
		public int maxBranchDepth = 3;
		public float branchAngle = 30.0f;
		public float branchAngleRange = 0.0f;
		public float branchThicknessMultiplier = 0.5f;
		public float spawnTime = 0.0f;
		public float fadeTime = 0.0f;

		Vector2 branchCountBounds = new Vector2(2.0f, 4.0f);
		
		int maxBranches = 15;
		int totalBranches = 0;

		Mesh mesh;
		Line rootLine;
		
		float segmentEndSize = 0.6f;
		
		float timer = 0.0f;
		float initialAlpha = 1.0f;
		bool fadeOutComplete = true;
		
		Vector3 worldStartPosition = Vector3.zero;
		Vector3 worldEndPosition = Vector3.zero;

		#region Properties
		
		public float StartThickness
		{
			get { return startThickness; }
			set { startThickness = Mathf.Max(value, 0.0f); }
		}

		public float ThicknessGrowth
		{
			get { return thicknessGrowth; }
			set { thicknessGrowth = value; }
		}
		
		public float SegmentCount
		{
			get { return segmentCount; }
			set { segmentCount = Mathf.Max(value, 1.0f); }
		}
		
		public float Sway
		{
			get { return sway; }
			set { sway = Mathf.Max(value, 0.0f); }
		}
		
		public float Branchiness
		{
			get { return branchiness; }
			set
			{
				branchiness = Mathf.Max(value, 0.0f);;
				
				float diff = branchiness / 3.0f;
				branchCountBounds = new Vector2(branchiness - diff, branchiness + diff);
				maxBranches = Mathf.Max((int)branchiness * 5, 1);
			}
		}
		
		public float BranchLengthMultiplier
		{
			get { return branchLengthMultiplier; }
			set { branchLengthMultiplier = Mathf.Max (value, 0.0f); }
		}
		
		public bool UseDistForBranchLength
		{
			get { return useDistForBranchLength; }
			set { useDistForBranchLength = value; }
		}

		public int MaxBranchDepth
		{
			get { return maxBranchDepth; }
			set { maxBranchDepth = Mathf.Max (value, 1); }
		}

		public float BranchAngle
		{
			get { return branchAngle; }
			set { branchAngle = Mathf.Clamp(branchAngle, 0.0f, 180.0f); }
		}

		public float BranchAngleRange
		{
			get { return branchAngleRange; }
			set { branchAngleRange = Mathf.Clamp(branchAngleRange, 0.0f, 180.0f); }
		}

		public float BranchThicknessMultiplier
		{
			get { return branchThicknessMultiplier; }
			set { branchThicknessMultiplier = Mathf.Max(value, 0.0f); }
		}
		
		public float SpawnTime
		{
			get { return spawnTime; }
			set { spawnTime = Mathf.Max(value, 0.0f); }
		}
		
		public float FadeTime
		{
			get { return fadeTime; }
			set { fadeTime = Mathf.Max(value, 0.0f); }
		}
		
		public Vector3 WorldStartPosition
		{
			get { return worldStartPosition; }
		}
		
		public Vector3 WorldEndPosition
		{
			get { return worldEndPosition; }
		}

		public float TotalLength
		{
			get { return Vector3.Distance(endPosition, startPosition); }
		}

		#endregion
		
		#region Initialization and Destruction
		
		void Start ()
		{
			if(Application.isPlaying)
			{
				initialAlpha = renderer.material.GetColor("_Color").a;
			}
			ApplyProperties();
			
			CalculateWorldPositions();
			
			GenerateLightning();
		}
		
		void OnEnable()
		{
			ApplyProperties();
			
			GenerateLightning();
		}
		
		void OnDisable()
		{
			if (Application.isEditor)
			{
				GetComponent<MeshFilter>().mesh = null;
				DestroyImmediate(mesh);
			}
		}
		
		#endregion
		
		#region Game Loop
		
		public void Update ()
		{
			timer += Time.deltaTime;
			
			if (spawnTime != 0.0f && timer > spawnTime)
			{
				GenerateLightning();
				timer = 0.0f;
			}
			else if (fadeTime != 0.0f && timer != 0.0f && !fadeOutComplete)
			{
				float alpha = Mathf.Min(1.0f, timer / fadeTime);
				
				if(Application.isPlaying)
				{
					Color color = renderer.material.GetColor("_Color");
					color.a = initialAlpha - alpha;
					renderer.material.SetColor("_Color", color);
				}
				if (timer > fadeTime)
					fadeOutComplete = true;
			}
		}
		
		#endregion
		
		#region Public Methods
		
		public void GenerateLightning()
		{
			CalculateWorldPositions();

			timer = 0.0f;
			fadeOutComplete = false;
			
			if (mesh == null)
			{
				mesh = new Mesh();
				mesh.hideFlags = HideFlags.HideAndDontSave;
				GetComponent<MeshFilter>().mesh = mesh;
			}
			
			totalBranches = 0;
			
			if (lineList != null)
				lineList.Clear();
			else
				lineList = new ArrayList();
			
			if (fadeTime > 0.0f)
				fadeOutComplete = false;
			
			if(Application.isPlaying)
			{
				Color color = renderer.material.GetColor("_Color");
				color.a = initialAlpha;
				renderer.material.SetColor("_Color", color);
			}
			
			lineList = GenerateBranch(startPosition, endPosition, 1);
			CreateMeshFromLines();
		}
		
		#endregion
		
		#region Private Methods
		
		void ApplyProperties()
		{
			StartThickness = startThickness;
			ThicknessGrowth = thicknessGrowth;
			SegmentCount = segmentCount;
			Branchiness = branchiness;
			BranchLengthMultiplier = branchLengthMultiplier;
			UseDistForBranchLength = useDistForBranchLength;
			MaxBranchDepth = maxBranchDepth;
			BranchAngle = branchAngle;
			BranchAngleRange = branchAngleRange;
			BranchThicknessMultiplier = branchThicknessMultiplier;
		}
		
		void CalculateWorldPositions()
		{
			worldStartPosition = transform.rotation * startPosition;
			worldStartPosition += transform.position;
			
			worldEndPosition = transform.rotation * endPosition;
			worldEndPosition += transform.position;
		}
		
		ArrayList GenerateBranch(Vector3 startPoint, Vector3 endPoint, int branchDepth)
		{
			if (totalBranches >= maxBranches)
				return new ArrayList();
			
			totalBranches++;
			
			ArrayList positionList = new ArrayList();
			ArrayList branchLineList = new ArrayList();
				
			Vector3 pointDiff = endPoint - startPoint;
			
			Vector3 lineDir = Vector3.Normalize(pointDiff);
			Vector3 perpDir = Vector3.Normalize(Vector3.Cross(lineDir, Vector3.up));
			float lineLength = Vector3.Magnitude(pointDiff);
			
			float lengthMultiplier = lineLength * 0.1f;
			
			int numPoints = (int)(segmentCount * lengthMultiplier);
			
			float segmentLength = (10.0f / numPoints) * 0.1f;
			float halfSegmentLength = segmentLength * 0.5f;
				
			Vector2 segmentLengthBounds = new Vector2(segmentLength - halfSegmentLength, segmentLength + halfSegmentLength);
			
			float totalLength = 0.0f;
			int numToShorten = 0;

			for (int i = 0; i < numPoints; i++)
			{
				float position = Random.Range(segmentLengthBounds.x, segmentLengthBounds.y);
				positionList.Add(position);
				totalLength += position;
				
				if (position > segmentLength)
					numToShorten++;
			}
			
			if (totalLength > 1.0f - segmentLengthBounds.x)
			{
				float extraLength = totalLength - 1.0f + segmentLengthBounds.x;

				float shortenAmount = extraLength / numToShorten;
				float position = 0.0f;

				for (int i = 0; i < numPoints; i++)
				{
					position = (float)positionList[i];
					
					if (position > segmentLength)
					{
						position -= shortenAmount;
						positionList[i] = position;
					}
				}
			}
			else if (totalLength < 1.0f - segmentLengthBounds.y)
			{
				float extraLength = 1.0f - segmentLengthBounds.y - totalLength;
				float lengthenAmount = extraLength / (numPoints - numToShorten);
				float position = 0.0f;
				
				for (int i = 0; i < numPoints; i++)
				{
					position = (float)positionList[i];
					
					if (position < segmentLength)
					{
						position += lengthenAmount;
						positionList[i] = position;
					}
				}
			}
			
			for (int i = 1; i < numPoints; i++)
			{
				positionList[i] = (float)positionList[i] + (float)positionList[i - 1];
			}
	
			Vector3 prevPoint = startPoint;
			float displacement;
			float prevDisplacement = 0.0f;
			float thicknessMultiplier = 1 - ((1 - branchThicknessMultiplier) * (branchDepth - 1));
			float thickness = 0.0f;

			// loop through ponts and create lines
			for (int i = 1; i < numPoints; i++)
			{
				float pos = (float)positionList[i];
				float shiftRange = (jaggedness * 0.5f) / (1 + ((branchDepth - 1) * 0.5f));
				float displacementAmount = Random.Range(-shiftRange, shiftRange);
				float maxDist = (1.0f - Mathf.Abs(pos * 2.0f - 1.0f)) * sway;
				maxDist = Mathf.Max(maxDist, shiftRange * 0.5f);

				thickness = startThickness + (thicknessGrowth * (lineLength / 10.0f) * pos);
				thickness = Mathf.Max(thickness, 0.0f);
				displacement = prevDisplacement + displacementAmount;
				
				if (i == numPoints - 1)
				{
					if (displacement > shiftRange * 0.5f)
						displacement = shiftRange * 0.5f;
					else if (displacement < -shiftRange * 0.5f)
						displacement = -shiftRange * 0.5f;
				}
				else if (displacement > maxDist || displacement < -maxDist)
				{
					displacement = prevDisplacement - displacementAmount;
				}
				
				Vector3 point = startPoint + (pos * pointDiff) + (displacement * perpDir);
				
				branchLineList.Add(new Line(prevPoint, point, thickness * thicknessMultiplier));
				
				prevPoint = point;
				prevDisplacement = displacement;
			}
			
			branchLineList.Add(new Line(prevPoint, endPoint, thickness * thicknessMultiplier));
					
			if (branchDepth < maxBranchDepth && totalBranches < maxBranches)
			{
				int numLines = branchLineList.Count;
				
				int minBranchNum = Mathf.Max(0, (int)branchCountBounds.x - branchDepth + 1);
				int maxBranchNum = Mathf.Max(0, (int)branchCountBounds.y - branchDepth + 1);
				
				int numBranches = Random.Range(minBranchNum, maxBranchNum);
				
				if (numBranches == 0)
					return branchLineList;
				
				ArrayList possibleLines = new ArrayList();
				Line branchLine = null;
				
				for (int i = 0; i < numLines - 3; i++)
					possibleLines.Add(i);
				
				for (int i = 0; i < numBranches; i++)
				{
					if (possibleLines.Count == 0)
						break;
					
					int pos = Random.Range(0, possibleLines.Count);
					branchLine = (Line)branchLineList[pos];
					
					int removeCount = 1;
					int removeStartIndex = pos;
					
					if (pos != 0 && (int)possibleLines[pos - 1] == pos - 1)
					{
						removeCount++;
						removeStartIndex = pos - 1;
					}

					if (pos != (int)possibleLines.Count - 1 && (int)possibleLines[pos + 1] == pos + 1)
						removeCount++;
					
					possibleLines.RemoveRange(removeStartIndex, removeCount);
					
					Vector3 branchDir = lineDir;
					int side = Random.Range(0, 2);
					float angle = Mathf.Clamp(branchAngle + Random.Range(0.0f, branchAngleRange) - branchAngleRange * 0.5f, 0.0f, 180.0f);
					
					if (side == 0)
						angle = -angle;
					
					branchDir = Quaternion.Euler(0.0f, angle, 0.0f) * branchDir;
					
					float branchLength = 0.0f;
					
					if (useDistForBranchLength)
						branchLength = (lineLength * (1.0f - (float)positionList[pos])) * branchLengthMultiplier;
					else
						branchLength = lineLength * branchLengthMultiplier;
					
					Vector3 branchPos = branchDir * branchLength;
					
					ArrayList branchLines = GenerateBranch(branchLine.pointA, branchLine.pointA + branchPos, branchDepth + 1);
					branchLineList.AddRange(branchLines);
					
					if (totalBranches >= maxBranches)
							break;
				}
			}
			
			return branchLineList;
		}
		
		void CreateMeshFromLines()
		{
			Line line = null;
			int numLines = lineList.Count;
			Vector3 lineDirection;
			Vector3 perpLineDirection;
			float lineLength;
			float halfLineWidth;
			float capSize;
			
			Quaternion lineRotation;
			
			Vector3[] vertices = new Vector3[numLines * 8];
			Vector2[] uvs = new Vector2[numLines * 8];
			int[] triangles = new int[numLines * 6 * 3];
			
			int indexOffset = 0;
			
			for (int i = 0; i < numLines; i++)
			{	
				line = (Line)lineList[i];
				indexOffset = 8 * i;
				
				halfLineWidth = line.thickness * 0.5f;
				capSize = line.thickness * segmentEndSize;
				
				lineDirection = line.pointB - line.pointA;
				lineLength = Vector3.Magnitude(lineDirection);
				
				lineDirection = Vector3.Normalize(lineDirection);
				perpLineDirection = Vector3.Cross(lineDirection, Vector3.up);
				
				lineRotation = Quaternion.FromToRotation(Vector3.forward, lineDirection);

				vertices[indexOffset]   = lineRotation * (new Vector3(-halfLineWidth, 0.0f, -capSize * 0.8f)) + line.pointA;
				vertices[indexOffset+1] = lineRotation * (new Vector3( halfLineWidth, 0.0f, -capSize * 0.8f)) + line.pointA;
				vertices[indexOffset+2] = lineRotation * (new Vector3(-halfLineWidth, 0.0f, capSize * 0.2f)) + line.pointA;
				vertices[indexOffset+3] = lineRotation * (new Vector3( halfLineWidth, 0.0f, capSize * 0.2f)) + line.pointA;
				
				vertices[indexOffset+4] = lineRotation * (new Vector3(-halfLineWidth, 0.0f, lineLength - capSize * 0.2f)) + line.pointA;
				vertices[indexOffset+5] = lineRotation * (new Vector3( halfLineWidth, 0.0f, lineLength - capSize * 0.2f)) + line.pointA;
				vertices[indexOffset+6] = lineRotation * (new Vector3(-halfLineWidth, 0.0f, lineLength + capSize * 0.8f)) + line.pointA;
				vertices[indexOffset+7] = lineRotation * (new Vector3( halfLineWidth, 0.0f, lineLength + capSize * 0.8f)) + line.pointA;
				
				uvs[indexOffset]   = new Vector2(0.0f, 1.0f);
				uvs[indexOffset+1] = new Vector2(0.0f, 0.0f);
				uvs[indexOffset+2] = new Vector2(0.8f, 1.0f);
				uvs[indexOffset+3] = new Vector2(0.8f, 0.0f);
				
				uvs[indexOffset+4] = new Vector2(0.81f, 1.0f);
				uvs[indexOffset+5] = new Vector2(0.81f, 0.0f);
				uvs[indexOffset+6] = new Vector2(0.0f, 1.0f);
				uvs[indexOffset+7] = new Vector2(0.0f, 0.0f);
			}
			
			/*  Smooth
			 *  6-----7
			 * 	|  B  |
			 *  4-----5
			 *  |     |
			 *  |     |
			 *  |     |
			 *  2-----3	
			 *  |  A  |
			 * 	0-----1
			 */
			
			indexOffset = 0; // offset for indexes in the array
			int quadOffset = 0; // offset for the quad
			int lineOffset = 0; // offset for the 
			
			for (int i = 0; i < triangles.Length / 6; i++)
			{
				indexOffset = i * 6;
				quadOffset = (i % 3) * 2;
				lineOffset = (int)Mathf.Floor(i / 3) * 8;
				
				triangles[0 + indexOffset] = 0 + quadOffset + lineOffset;
				triangles[1 + indexOffset] = 2 + quadOffset + lineOffset;
				triangles[2 + indexOffset] = 3 + quadOffset + lineOffset;
				
				triangles[3 + indexOffset] = 3 + quadOffset + lineOffset;
				triangles[4 + indexOffset] = 1 + quadOffset + lineOffset;
				triangles[5 + indexOffset] = 0 + quadOffset + lineOffset;
			}
			
			mesh.Clear();
			
			mesh.vertices = vertices;
			mesh.uv = uvs;
			mesh.triangles = triangles;
		}
		
		#endregion
	}
}
