using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
    public class DamageDealer : MonoBehaviour
	{
        public float minDamage = 20;
        public float maxDamage = 20;

        void Start()
        {

        }

        void update()
        {

        }

        public float GetDamage()
        {
            return Random.Range(minDamage, maxDamage);
        }

		public void SetDamage(float damage)
		{
			SetDamage(damage, damage);
		}

		public void SetDamage(float min, float max)
		{
			minDamage = min;
			maxDamage = max;
		}

		public void MultiplyDamage(float multiplyAmount)
		{
			minDamage = minDamage * multiplyAmount;
			maxDamage = maxDamage * multiplyAmount;
		}
	}
}
