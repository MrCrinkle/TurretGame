using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class TopDownAim : MonoBehaviour
	{
		public Transform target = null;
		public Vector3 targetOffset = Vector3.zero;
		public float angleOffet = 0.0f;
		public float maxRotationSpeed = 800.0f;
		public float deadZoneValue = 0.3f;

		public bool allowMouse = true;
		public bool allowController = true;

		bool usingMouse = true;

		public bool UsingMouse { get { return usingMouse; } }

		void Start()
		{
			if (!allowMouse && allowController)
				usingMouse = false;
		}
		
		void Update()
		{
			if (!allowMouse && !allowController)
				return;

			float controllerXValue = Input.GetAxis("RightStickX");
			float controllerYValue = Input.GetAxis("RightStickY");

			if (allowMouse && (Mathf.Abs(Input.GetAxis("Mouse X")) > 5 || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.3f))
				usingMouse = true;
			else if (allowController && ((Mathf.Abs(controllerXValue) >= deadZoneValue || Mathf.Abs(controllerYValue) >= deadZoneValue)))
				usingMouse = false;

			if (usingMouse)
			{
				Plane targetPlane = new Plane(Vector3.up, target.position + targetOffset);
				Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
				
				float dist = 0.0f;
				if (targetPlane.Raycast(mouseRay, out dist))
				{
					Vector3 mousePoint = mouseRay.GetPoint(dist);
				
					Vector3 mouseDir = mousePoint - (target.position + targetOffset);

					Quaternion targetRotation = Quaternion.LookRotation(mouseDir);
					targetRotation = targetRotation * Quaternion.AngleAxis(angleOffet, Vector3.up);
					target.rotation = Quaternion.RotateTowards(target.rotation, targetRotation, maxRotationSpeed * Time.deltaTime);
				}
			}
			else
			{
				if (Mathf.Abs(controllerXValue) >= deadZoneValue || Mathf.Abs(controllerYValue) >= deadZoneValue)
				{
					float angle = (Mathf.Atan2(controllerXValue, controllerYValue) * Mathf.Rad2Deg) + 180 + angleOffet;
					Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.up);

					target.rotation = Quaternion.RotateTowards(target.rotation, targetRotation, maxRotationSpeed * Time.deltaTime);
				}
			}
		}
	}
}

