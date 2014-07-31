using System;
using UnityEngine;

namespace AssemblyCSharp
{
	[RequireComponent(typeof(DamageTaker))]
	public class HealthBar : GUIBar
	{
		DamageTaker damageTakerComponent;
		
		void Start ()
		{
			damageTakerComponent = gameObject.GetComponent<DamageTaker>();

			maxAmount = damageTakerComponent.maxHP;
		}
		
		void Update()
		{
			currentAmount = damageTakerComponent.CurrentHP;
		}
	}
}

