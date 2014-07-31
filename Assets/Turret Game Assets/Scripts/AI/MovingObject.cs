using System;
using UnityEngine;

namespace AssemblyCSharp
{
    public class MovingObject : MonoBehaviour
    {
        public float maxMoveSpeed = 10.0f;
		public float directionalAcceleration = 0.0f;
		public Vector3 acceleration = Vector3.zero;
        public float turnSpeed = 400.0f;
		public float startMoveSpeed = 10;
		public Vector3 direction = Vector3.forward;
		public bool useLookDirection = true;
		public bool allowNegativeSpeed = false;

        float moveSpeed = 0.0f;
		Vector3 velocity = Vector3.zero;

        public float MoveSpeed 
        {
			get { return moveSpeed; }
			set { moveSpeed = value; }
        }

		public Vector3 Velocity
		{
			get { return velocity; }
			set { velocity = value; }
		}

		void Awake()
		{
			ResetVelocity();
		}
		
		void FixedUpdate()
		{ 
			if (transform.GetComponent<MineProjectile>() != null)
			{
				if (moveSpeed == 0.0f && velocity == Vector3.zero)
					return;
			}

			if (useLookDirection)
				velocity = VelocityFromMoveSpeed();

			// add acceleration to velocity
			velocity = velocity + acceleration * Time.deltaTime;

			// recalculate direction if velocity is not 0
			if (velocity != Vector3.zero)
				direction = velocity.normalized;

			// convert velocity to move speed to apply directional accelerations and check bounds
			moveSpeed = MoveSpeedFromVelocity();

			moveSpeed += directionalAcceleration * Time.deltaTime;
         
			if (maxMoveSpeed > 0 && moveSpeed > maxMoveSpeed)
				moveSpeed = maxMoveSpeed;

			if (!allowNegativeSpeed && moveSpeed < 0.0f)
				moveSpeed = 0.0f;

			// convert move speed back to velocity
			velocity = VelocityFromMoveSpeed();

			// set rotation if we are using it
			//if (useLookDirection && direction != Vector3.zero)
				//rigidbody.MoveRotation(Quaternion.LookRotation(direction));

			// move the object based on whether it is kinematic or not
			if (transform.rigidbody != null)
			{
				if (transform.rigidbody.isKinematic)
				{
					rigidbody.MovePosition(transform.position + (velocity * Time.deltaTime));
				}
				else
				{
					rigidbody.velocity = velocity;
				}
			}
		}
		
        void Update()
        {
            
        }

		public void ResetVelocity()
		{
			moveSpeed = startMoveSpeed;
			
			velocity = VelocityFromMoveSpeed();
		}

		Vector3 VelocityFromMoveSpeed()
		{
			Vector3 moveDirection = direction;

			Quaternion rot = transform.rotation;
			Vector3 dir = rot * Vector3.forward;

			if (useLookDirection)
				moveDirection = transform.forward;

			return moveDirection.normalized * moveSpeed;
		}

		float MoveSpeedFromVelocity()
		{
			return velocity.magnitude;
		}
    }
}

