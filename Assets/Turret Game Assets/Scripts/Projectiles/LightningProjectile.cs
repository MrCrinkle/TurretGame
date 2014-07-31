using System;
using System.Collections;
using UnityEngine;

namespace AssemblyCSharp
{
	public class LightningProjectile : Projectile
	{
		public Transform lightningEffectRef = null;
		LightningGun lightningGun = null;
		
		private DamageDealer damageDealer = null;

		private bool calculatingPrecisionTargets = false;
		private ArrayList precisionModTargets;
		private GameObject precisionModCollisionObject;	
		public float precisionModHitDelay = 0.2f;
		private float precisionModHitTimer = 0.0f;
		private bool isFirstPrecisionModTarget = true;
		private int lightningConnectionIndex = 0;

		private ArrayList lightningEffects;

		private bool drawDebugColliders = false;
		
		public override void Start ()
		{
			projectileType = ProjectileType.Lightning;

			lightningGun = source.GetComponent<LightningGun>();

			damageDealer = transform.GetComponent<DamageDealer>();
			if (damageDealer)
			{
				// lightning has custom damage model
				// DamageDealer properties are still used (splash radius, damage etc.)
				damageDealer.enabled = false;
			}
			
			lightningEffects = new ArrayList();
			
			lightningEffects.Add(transform.Find("LightningEffect"));

			BoxCollider collider = transform.GetComponent<BoxCollider>();
			Lightning lightning = ((Transform)lightningEffects[0]).GetComponent<Lightning>();

			if (collider != null)
			{
				float colliderWidth = splashAOE;
				float colliderLength = lightning.TotalLength;
				
				if (ModifierType == TurretModifierType.Precision)
					colliderWidth = colliderWidth * 0.5f;
				
				collider.size = new Vector3(colliderWidth, collider.size.y, colliderLength);
			}

			checkForObstacles();

			if (ModifierType == TurretModifierType.Precision)
			{
				precisionModTargets = new ArrayList();
				calculatingPrecisionTargets = true;
				CreateNextCollider();

				((Transform)lightningEffects[0]).renderer.enabled = false;
			}
			else if (ModifierType == TurretModifierType.Piercing)
			{
				lightning.thicknessGrowth = 0.0f;
			}
			
			base.Start();
		}
		
		public override void Update ()
		{
			base.Update();
			
			float fadeTime = ((Transform)lightningEffects[0]).GetComponent<Lightning>().FadeTime;

			if (ModifierType != TurretModifierType.Precision || (ModifierType == TurretModifierType.Precision && !calculatingPrecisionTargets))
			{
				if (fadeTime != 0.0f && timeAlive > fadeTime)
				{
					Destroy(gameObject);
				}	
			}

			if (calculatingPrecisionTargets)
			{
				if (precisionModHitTimer >= precisionModHitDelay)
				{
					CreateNextCollider();
					
					precisionModHitTimer = 0.0f;
				}
				else
				{
					precisionModHitTimer += Time.deltaTime;
				}
			}
		}
		
		public void FixedUpdate()
		{
			if (ModifierType == TurretModifierType.Precision && precisionModCollisionObject != null)
			{
				LightningCollider lightningCollider = (LightningCollider)precisionModCollisionObject.transform.GetComponent<LightningCollider>();
					
				if (lightningCollider.HasCheckedCollision)
				{
					bool isHittingEnemies = false;

					if (lightningCollider.HasHitEnemies)
					{
						addClosestEnemyToList(lightningCollider);

						if (precisionModTargets.Count > 1)
						{
							Vector3 start = ((Transform)precisionModTargets[precisionModTargets.Count - 2]).position;
							Vector3 end = ((Transform)precisionModTargets[precisionModTargets.Count - 1]).position;

							DamageTaker damageTaker = ((Transform)precisionModTargets[precisionModTargets.Count - 1]).GetComponent<DamageTaker>();

							if (damageTaker)
								damageTaker.TakeDamage(damageDealer.GetDamage());

							createLightningConnection(start, end);

							Destroy(precisionModCollisionObject);

							isHittingEnemies = true;
						}
					}

					if (!isHittingEnemies)
					{
						calculatingPrecisionTargets = false;
						Destroy(precisionModCollisionObject);
						((Transform)lightningEffects[0]).renderer.enabled = true;
					}
				}
			}
		}
		
		protected void addClosestEnemyToList(LightningCollider lightningCollider)
		{
			ArrayList enemiesHit = lightningCollider.CollidingEnemies;
			
			if (enemiesHit.Count != 0 && precisionModTargets.Count != 0)
			{
				float dist = 0.0f;
				float closestDist = 0.0f;
				Transform closestEnemy = null;
					
				for (int i = 0; i < enemiesHit.Count; i++)
				{
					dist = Vector3.Distance(((Transform)enemiesHit[i]).position, ((Transform)precisionModTargets[precisionModTargets.Count - 1]).position);
					
					if ((closestDist == 0.0f || dist < closestDist) && !precisionModTargets.Contains(enemiesHit[i]))
					{
						DamageTaker damageTaker = ((Transform)enemiesHit[i]).GetComponent<DamageTaker>();
						
						if (damageTaker && damageTaker.IsAlive)
						{
							closestDist = dist;
							closestEnemy = (Transform)enemiesHit[i];
						}
					}
				}
				
				if (closestEnemy != null)
				{
					precisionModTargets.Add(closestEnemy);
				}
			}
		}
		
		protected void createLightningConnection(Vector3 start, Vector3 end)
		{
			while (lightningConnectionIndex > lightningEffects.Count - 1)
			{
				Transform newLightning = (Transform)Instantiate(lightningEffectRef);
				lightningEffects.Add(newLightning);
			}

			Transform lightningObject = (Transform)lightningEffects[lightningConnectionIndex];
			Lightning lightningComponent = (Lightning)lightningObject.GetComponent<Lightning>();

			lightningObject.renderer.enabled = true;
			lightningObject.position = start;
			lightningObject.rotation = Quaternion.identity;

			lightningComponent.startPosition = Vector3.zero;
			lightningComponent.endPosition = end - start;

			lightningComponent.GenerateLightning();
			
			lightningConnectionIndex++;
		}
		
		protected void CreateNextCollider()
		{
			if (precisionModCollisionObject != null)
				Destroy(precisionModCollisionObject);

			precisionModCollisionObject = new GameObject();
			
			if (isFirstPrecisionModTarget)
			{
				precisionModCollisionObject.transform.position = source.position;
				precisionModCollisionObject.transform.rotation = source.Find("Turret").transform.rotation;
				
				BoxCollider collider = (BoxCollider)precisionModCollisionObject.AddComponent<BoxCollider>();
				collider.size = new Vector3(4.0f, 0.5f, 40.0f);
				collider.center = new Vector3(0.0f, 0.0f, 20.0f);
				collider.isTrigger = true;

				precisionModTargets.Add(source.Find("Turret").Find("FirePoint").transform);
				
				isFirstPrecisionModTarget = false;
			}
			else
			{
				MeshCollider collider = (MeshCollider)precisionModCollisionObject.AddComponent<MeshCollider>();
				
				Mesh colliderMesh = new Mesh();
				Vector3[] vertices = { new Vector3(0.0f, 0.0f, 0.0f), new Vector3(-7.5f, 0.0f, 15.0f), new Vector3(7.5f, 0.0f, 15.0f)};
				int[] triangles = { 0, 1, 2 };
				colliderMesh.vertices = vertices;
				colliderMesh.triangles = triangles;
				colliderMesh.RecalculateBounds();
				collider.sharedMesh = colliderMesh;
				collider.isTrigger = true;

				if (drawDebugColliders)
				{
					MeshRenderer r = (MeshRenderer)precisionModCollisionObject.AddComponent<MeshRenderer>();
					MeshFilter f = (MeshFilter)precisionModCollisionObject.AddComponent<MeshFilter>();

					f.mesh = colliderMesh;
				}

				Vector3 direction = ((Transform)precisionModTargets[precisionModTargets.Count - 1]).position - ((Transform)precisionModTargets[precisionModTargets.Count - 2]).position;
				direction = direction.normalized;
				direction.y = 0.0f;
				
				collider.transform.rotation = Quaternion.LookRotation(direction);

				Vector3 position = ((Transform)precisionModTargets[precisionModTargets.Count - 1]).position;
				position.y = 0.5f;
				collider.transform.position = position;
			}
			
			LightningCollider lightningCollider = (LightningCollider)precisionModCollisionObject.AddComponent(typeof(LightningCollider));
			lightningCollider.AddIgnoreEnemy((Transform)precisionModTargets[precisionModTargets.Count - 1]);
		}
		
		protected override void OnCollision(Collision collision, Collider other, Transform hitObject)
		{
			LightningGun lightningGun = source.GetComponent<LightningGun>();

			float distFromTurret = Vector3.Distance(transform.position, hitObject.position);
			float lightningLength = ((Transform)lightningEffects[0]).GetComponent<Lightning>().TotalLength;

			if (ModifierType == TurretModifierType.Precision || distFromTurret > lightningLength)
				return;

			DamageTaker otherDamageTaker = hitObject.GetComponent<DamageTaker>();
			
			if (otherDamageTaker != null && otherDamageTaker.IsAlive)
			{
				Lightning lightning = ((Transform)lightningEffects[0]).GetComponent<Lightning>();
				
				if (lightning == null)
					return;
				
				//  ||(a - p) - ((a - p) . n)n||
				
				// convert position to vector2 because we don't want to use vertical distance
				Vector2 a = new Vector2(lightning.WorldStartPosition.x, lightning.WorldStartPosition.z);
				Vector2 b = new Vector2(lightning.WorldEndPosition.x, lightning.WorldEndPosition.z);
				Vector2 p = new Vector2(other.transform.position.x, other.transform.position.z);
				Vector2 n = (b - a).normalized;
				Vector2 result = Vector2.Dot((a - p), n) * n;
				result = (a - p) - result;
				float distance = result.magnitude;
				
				if(distance < splashAOE)
				{
					float damageMult = 1.0f - (distance / splashAOE);

					float distMult = 1.0f;

					if (ModifierType != TurretModifierType.Piercing)
					{
						distMult = distFromTurret / lightningLength;
						distMult = 1 - Mathf.Max(0.0f, distMult);
					}

					float finalDamage = damageMult * distMult * damageDealer.GetDamage();

					otherDamageTaker.TakeDamage(finalDamage);
				}
			}
		}

		protected void checkForObstacles()
		{
			RaycastHit hitInfo;
			int layerMask = 1 << 11;
			if (Physics.Raycast(transform.position, transform.forward, out hitInfo, 100.0f, layerMask))
			{
				((Transform)lightningEffects[0]).GetComponent<Lightning>().endPosition = new Vector3(0.0f, 0.0f, hitInfo.distance);

				BoxCollider boxCollider = transform.GetComponent<BoxCollider>();
				boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y, hitInfo.distance);

				if (hitInfo.transform.GetComponent<Base>() != null)
				{
					hitInfo.transform.GetComponent<DamageTaker>().TakeDamage(damageDealer.GetDamage() * 0.6f);
				}
			}
		}
	}
}