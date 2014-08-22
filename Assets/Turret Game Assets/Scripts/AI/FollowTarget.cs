using System;
using UnityEngine;

namespace AssemblyCSharp
{
	[RequireComponent(typeof(MovingObject), typeof(Rigidbody), typeof(Entity))]
    public class FollowTarget : LookAtTarget
    {
		MovingObject movingObjectComponent;
		
		bool atTarget = false;

		public override void Start()
        {
			movingObjectComponent = (MovingObject)transform.GetComponent<MovingObject>();
        
			base.Start();
		}

        public override void FixedUpdate()
        {
			base.FixedUpdate();

			if (target == null)
				return;

			Vector3 targetDiff = target.transform.position - transform.position;

			if (ignoreVertical)
				targetDiff.y = 0.0f;

			float distToTarget = Vector3.Magnitude(targetDiff);
			float combinedRadius = transform.GetComponent<Entity>().radius + target.radius;

			if (!atTarget && distToTarget < combinedRadius)
			{
				atTarget = true;
				movingObjectComponent.enabled = false;

				if (transform.rigidbody.isKinematic)
					transform.GetComponent<MovingObject>().MoveSpeed = 0;
				else
					transform.rigidbody.velocity = Vector3.zero;
				
				// move so we are touching the target, based on the radii
				Vector3 direction = Vector3.Normalize(targetDiff);
				Vector3 finalPosition = target.transform.position;

				if (ignoreVertical)
					finalPosition.y = transform.position.y;

				finalPosition = finalPosition - ((combinedRadius + 0.1f) * direction);

				transform.rigidbody.MovePosition(finalPosition);
			}
			else if (!atTarget || (atTarget && distToTarget > combinedRadius + 0.1f))
			{
				atTarget = false;
				movingObjectComponent.enabled = true;
			}
        }
    }
}

