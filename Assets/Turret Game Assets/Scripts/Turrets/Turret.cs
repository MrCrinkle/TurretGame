using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public enum TurretType { MachineGun, RocketLauncher, Lightning, Gauss, Mine, Max };
	public enum FireType { Auto, SemiAuto, SingleFire, Max };

	public abstract class Turret : MonoBehaviour
	{	
		#region Variables
		
		public GameObject projectilePrefab = null;
		public string[] attackNameList;
		protected Dictionary<TurretModifierType, Attack> attackList;

		protected Attack currentAttack = null;

		public int maxAmmo = 0; // if this is 0 then ammo isn't being used
		public int startAmmo = 0;
		protected int currentAmmo = 0;
		public float ammoRechargeRate = 0.0f;
		protected float ammoRechargeTimer = 0.0f;
	
		protected TurretType turretType = TurretType.MachineGun;
		protected FireType fireType = FireType.Auto;
		protected Transform firePoint;
		protected Transform container;
		protected Transform aimer;
		protected Transform aimerModel;

		protected float shotTimer = 0.0f;
		protected bool readyToShoot = true;
		protected bool isSelected = false;
		protected bool isAimerHitting = false;
		protected GameObject lastProjectileSpawned = null;
		
		protected Upgrade modifier;
		
		#endregion
		
		#region Properties

		public int CurrentAmmo { get { return currentAmmo; } }
		public TurretType TurretType { get { return turretType; } }
		public FireType FireType { get { return fireType; } }
		public bool IsSelected { get { return isSelected; } set { isSelected = value; } }
		public Upgrade Modifier { get {return modifier; } }
		public float AmmoRechargeTimer { get { return ammoRechargeTimer; } }
		public GameObject LastProjectileSpawned { get { return lastProjectileSpawned; } }

		#endregion
		
		#region Initialization

		public virtual void Awake()
		{
			currentAmmo = startAmmo;
			shotTimer = 0.0f;
			ammoRechargeTimer = 0.0f;
			firePoint = transform.Find("FirePoint");
			container = transform.parent;

			if (container != null)
			{
				aimer = container.Find("Aimer");
				aimerModel = aimer.Find("AimerModel");
			}

			SetAimerVisible(false);

			modifier = new Upgrade(UpgradeType.TurretModifier);

			InputManager.OnControlTypeChangedEvent += OnControlTypeChanged;

			attackList = new Dictionary<TurretModifierType, Attack>();
			
			AttackReader attackReader = new AttackReader();
			
			for (int i = 0; i < attackNameList.Length; i++)
			{
				Attack newAttack = attackReader.LoadAttack(attackNameList[i], this.transform);
				
				TurretModifierType key = TurretModifierType.None;
				
				if (attackNameList[i].IndexOf("Precision") != -1)
					key = TurretModifierType.Precision;
				else if (attackNameList[i].IndexOf("Piercing") != -1)
					key = TurretModifierType.Piercing;
				else if (attackNameList[i].IndexOf("Elemental") != -1)
					key = TurretModifierType.Elemental;
				else if (attackNameList[i].IndexOf("Multiply") != -1)
					key = TurretModifierType.Multiply;
				
				attackList.Add(key, newAttack);
			}
			
			currentAttack = (Attack)attackList[TurretModifierType.None];
		}

		public virtual void Start() 
		{

		}
		
		#endregion
		
		#region Game Loop
		
		public virtual void Update() 
		{
			if (container == null)
			{
				container = transform.parent;

				if (container != null)
				{
					aimer = container.Find("Aimer");
					aimerModel = aimer.Find("AimerModel");
				}
			}

			if (Time.timeScale == 0.0f)
				return;

			if (!readyToShoot)
			{
				shotTimer += Time.deltaTime;

				if (shotTimer >= currentAttack.Delay)
				{
					shotTimer = 0.0f;
					readyToShoot = true;
				}
			}

			if (maxAmmo > 0)
			{
				if (currentAmmo < maxAmmo)
					ammoRechargeTimer += Time.deltaTime;

				if (ammoRechargeTimer >= ammoRechargeRate)
				{
					ammoRechargeTimer = 0.0f;
					currentAmmo++;
				}
			}

			if (isSelected)
			{
				if (Input.GetButtonDown("Fire1") || Input.GetAxis("Fire1") > 0.0f)
				{
					if (maxAmmo == 0 || currentAmmo > 0)
						Shoot();
				}
			}

			CheckAimerCollision();
		}
		
		#endregion
		
		#region Public Methods
		
		public virtual bool Shoot()
		{
			if (readyToShoot && (maxAmmo == 0 || currentAmmo > 0))
			{
				ArrayList projectiles = currentAttack.StartAttack(container.rotation, firePoint.position);
				SetupProjectiles(projectiles);

				readyToShoot = false;
				shotTimer = 0.0f;
				currentAmmo--;

				return true;
			}

			return false;
		}

		protected virtual void SetupProjectiles(ArrayList projectiles)
		{
			/*
			GameObject projectile = null;

			for (int i = 0; i < projectiles.Count; i++)
			{
				projectile = (GameObject)projectiles[i];

				projectile.transform.position = firePoint.position;
				projectile.transform.rotation = container.rotation;
				projectile.transform.GetComponent<Projectile>().Source = transform;

				if (currentAttack.Accuracy > 0.0f)
				{
					float angleModifier = Random.Range(0.0f, currentAttack.Accuracy);
					float newRotation = container.rotation.eulerAngles.y;
					newRotation = newRotation + angleModifier - (currentAttack.Accuracy * 0.5f);
					
					Vector3 newDirection = new Vector3(Mathf.Sin(newRotation * Mathf.Deg2Rad), 0.0f, Mathf.Cos(newRotation * Mathf.Deg2Rad));
					projectile.transform.rotation = Quaternion.LookRotation(newDirection);
				}
				else
				{
					projectile.transform.rotation = Quaternion.LookRotation(container.rotation * Vector3.forward);
				}
				
				lastProjectileSpawned = projectile;
			}
			*/

			lastProjectileSpawned = (GameObject)projectiles[0];
		}
		
		public virtual void OnSelectTurret()
		{
			isSelected = true;

			if (!InputManager.Instance.UsingMouse)
				SetAimerVisible(true);
		}
		
		public virtual void OnDeselectTurret()
		{
			isSelected = false;
			SetAimerVisible(false);
		}
		
		public virtual void SetTurretModifierType(TurretModifierType type)
		{
			modifier.setUpgradeSubType(type);

			currentAttack = attackList[type];
		}

		public void OnControlTypeChanged(bool changedToMouse)
		{
			if (isSelected)
				SetAimerVisible(!changedToMouse);
		}

		public void SetAimerVisible(bool visible)
		{
			if (aimerModel != null)
				aimerModel.renderer.enabled = visible;
		}

		public void CheckAimerCollision()
		{
			if (aimerModel.renderer.enabled)
			{
				RaycastHit hitInfo;
				int layerMask = 1 << 11;

				if(Physics.Raycast(firePoint.position, transform.forward, out hitInfo, 1000.0f, layerMask))
				{
					float length = hitInfo.distance * 0.5f;

					aimerModel.localScale = new Vector3(aimerModel.localScale.x, length, aimerModel.localScale.z);
					aimer.localPosition = new Vector3(aimer.localPosition.x, aimer.localPosition.y, length + firePoint.localPosition.z);

					isAimerHitting = true;
				}
				else
				{
					// only resize if we were hitting something previously
					if (isAimerHitting)
					{
						aimerModel.localScale = new Vector3(aimerModel.localScale.x, 20.0f, aimerModel.localScale.z);
						aimer.localPosition = new Vector3(aimer.localPosition.x, aimer.localPosition.y, 20.0f + firePoint.localPosition.z);
					}

					isAimerHitting = false;
				}
			}
		}
		
		#endregion
	}
}