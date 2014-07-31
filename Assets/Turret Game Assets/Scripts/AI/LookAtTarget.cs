using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class LookAtTarget : MonoBehaviour
	{
		#region Variables
		
		public Transform target = null;
		public bool ignoreVertical = false;
		public bool ignoreHorizontal = false;
		public float maxTurnSpeed = 200; // in degrees per second
		public bool useLocalRotation = false;

		#endregion
		
		#region Properties
		
		#endregion
		
		#region initialization
		
		public virtual void Start()
		{

		}
		
		#endregion
		
		#region Game Loop
		
		public virtual void FixedUpdate ()
		{
			if (target == null || (ignoreVertical && ignoreHorizontal))
				return;

			Vector3 targetDirection = target.position - transform.position;

			// ignore y component of position
			if (ignoreVertical)
				targetDirection.y = 0.0f;

			Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

			// ignore horizontal rotation
			if (ignoreHorizontal)
			{
				// convert to vector3 and zero out rotation around y axis, then convert back to quaternion
				Vector3 rotation = targetRotation.eulerAngles;
				rotation.y = 0.0f;
				targetRotation = Quaternion.Euler(rotation);
			}

			if (maxTurnSpeed == 0.0f)
			{
				rigidbody.rotation = targetRotation;
			}
			else if (useLocalRotation)
			{
				//rigidbody.MoveRotation(Quaternion.RotateTowards(transform.localRotation, targetRotation, maxTurnSpeed * Time.deltaTime));
			}
			else
			{
				rigidbody.MoveRotation(Quaternion.RotateTowards(rigidbody.rotation, targetRotation, maxTurnSpeed * Time.deltaTime));
				//rigidbody.MoveRotation(Quaternion.LookRotation(Vector3.back));
			}
		}
		
		#endregion
		
		#region Public Methods
		
		#endregion
		
		#region Private Methods
		
		#endregion
	}
}