using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class MineLauncher : Turret
	{
		#region Variables

		public GameObject multiplyProjectilePrefab = null;

		Vector3 currentTargetPosition = Vector3.zero;

		#endregion
		
		#region Properties
		
		#endregion
		
		#region initialization
	
		public override void Start ()
		{
			base.Start();
			
			turretType = TurretType.Mine;
			fireType = FireType.SingleFire;
		}
	
		#endregion
		
		#region Game Loop
	
		public override void Update ()
		{
			base.Update();
		}
	
		#endregion
		
		#region Public Methods

		public override bool Shoot()
		{
			FindTargetPosition();

			bool canShoot = base.Shoot();

			if (canShoot && modifier.SubType == (int)TurretModifierType.Multiply)
			{

			}


			return false;
		}

		/*
		protected override GameObject SpawnProjectile()
		{
			GameObject newProjectile = null;

			if (modifier.SubType == (int)TurretModifierType.Multiply)
			{
				newProjectile = (GameObject)GameObject.Instantiate(multiplyProjectilePrefab, firePoint.position, container.rotation);
			}
			else
			{
				newProjectile = (GameObject)GameObject.Instantiate(projectilePrefab, firePoint.position, container.rotation);

			}

			newProjectile = (GameObject)GameObject.Instantiate(projectilePrefab, firePoint.position, container.rotation);
			newProjectile.transform.GetComponent<Projectile>().Source = transform;	

			MovingObject movingObject = newProjectile.GetComponent<MovingObject>();

			movingObject.direction = (transform.rotation * Quaternion.Euler(new Vector3(-30.0f, 0.0f, 0.0f))) * Vector3.forward;
			movingObject.useLookDirection = false;

			movingObject.acceleration = new Vector3(0.0f, -15.0f, 0.0f);

			newProjectile.transform.rotation = Quaternion.LookRotation(container.rotation * Vector3.forward);

			newProjectile.GetComponent<MineProjectile>().TargetPosition = currentTargetPosition;

			return newProjectile;
		}
		*/

		#endregion
		
		#region Private Methods

		void FindTargetPosition()
		{
			Vector3 mousePos = Input.mousePosition;
			Ray mouseVector = Camera.main.ScreenPointToRay(mousePos);
			RaycastHit raycastHit;
			
			if (World.Instance.terrainRef.collider.Raycast(mouseVector, out raycastHit, 1000.0f))
			{
				currentTargetPosition = raycastHit.point;	
			}
		}

		#endregion
	}
}