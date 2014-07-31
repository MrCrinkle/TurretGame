using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class GUIBar : MonoBehaviour
	{
		public Texture2D barBackTexture;
		public Texture2D barTexture;
		public Vector2 barSize = new Vector2(40, 4);
		public Vector2 barOffset = new Vector2(0, -15);
		public float maxAmount = 100.0f;
		public float currentAmount = 100.0f;
		public bool hideWhenFull = true;
		public bool hideWhenEmpty = true;
		
		public virtual void Start ()
		{

		}
		
		public virtual void Update()
		{
			
		}
		
		public virtual void OnGUI () 
		{
			Vector3 targetScreenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
			
			float fullPercent = currentAmount / maxAmount;
			
			if ((fullPercent == 1.0f && hideWhenFull) || (fullPercent == 0.0f && hideWhenEmpty))
				return;
			
			Vector2 barPos = targetScreenPos;
			
			// convert world coords to screen coords
			barPos.y -= Screen.height / 2.0f;
			barPos.y = -barPos.y;
			barPos.y += Screen.height / 2.0f;
			
			barPos.x += barOffset.x - (barSize.x * 0.5f);
			barPos.y += barOffset.y - (barSize.y * 0.5f);
			
			GUI.DrawTexture(new Rect(barPos.x, barPos.y, barSize.x, barSize.y), barBackTexture);
			GUI.DrawTexture(new Rect(barPos.x, barPos.y, barSize.x * fullPercent, barSize.y), barTexture);
		}
	}
}