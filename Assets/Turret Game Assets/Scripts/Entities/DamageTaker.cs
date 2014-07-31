using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public class DamageTaker : MonoBehaviour
	{
		public float maxHP = 100;
		public float maxArmor = 0;
		public float regen = 0.0f;
		public float regenDelay = 1.0f;
		public bool regenWhenDead = false;
		public bool solidWhenDead = false;
		
		float currentHP = 100;
		float currentArmor = 0.0f;
		bool alive = true;
		float regenDelayTimer = 0.0f;
		
		public float CurrentHP { get { return currentHP; } }
		public float CurrentArmor { get { return currentArmor; } }
		public bool IsAlive { get { return alive; } }
		
		public delegate void OnDeathDelegate(GameObject obj);
		public static event OnDeathDelegate OnDeathEvent;
		
		void Start()
		{
			currentHP = maxHP;
			regenDelayTimer = regenDelay;
		}
		
		void Update()
		{
			if (alive || (!alive && regenWhenDead))
			{
				if (regenDelayTimer < regenDelay)
				{
					regenDelayTimer += Time.deltaTime;

					if (regenDelayTimer > regenDelay)
						regenDelayTimer = regenDelay;
				}

				if (regenDelayTimer == regenDelay && regen > 0.0f && currentHP < maxHP)
				{
					currentHP += regen * Time.deltaTime;

					if (currentHP > maxHP)
						currentHP = maxHP;
				}
			}
		}
		
		void OnTriggerEnter(Collider other)
		{

		}
		
		public bool TakeDamage(float damage)
		{
			bool tookDamage = false;

			if (damage < 0.0f)
				return tookDamage;

			if (alive)
			{
				currentHP -= damage;
				
				if (currentHP <= 0.0f)
				{
					currentHP = 0.0f;
					alive = false;
					OnDeath();
				}
				
				tookDamage = true;

				regenDelayTimer = 0.0f;
			}
			
			return tookDamage;
		}
		
		void OnDeath()
		{
			OnDeathEvent(gameObject);
		}
	}
}
