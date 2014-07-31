using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public enum UpgradeType { TurretModifier, TurretEnhancement, BaseEnhancement, BaseAddOn, Max }
	public enum TurretModifierType { None, Precision, Piercing, Elemental, Multiply, Max }
	public enum BaseEnhancementType { None, Radar, ShieldCapacity, ShieldRecharge, Durability, BaseDamage, Max }
	public enum BaseAddOnType { None, AutoTurret, Max, SpotLight , ShockShield, Nuke, Repulse }
	
	public class Upgrade
	{
		#region Variables
		
		int level = 0;
		int maxLevel = 0;
		
		UpgradeType type = UpgradeType.TurretModifier;
		int subType = 0;
		
		#endregion
		
		#region Properties
		
		public UpgradeType Type{ get { return type; } }
		public int SubType{ get { return subType; } }
		
		#endregion
		
		#region initialization
		
		public Upgrade(UpgradeType type)
		{
			this.type = type;
		}
		
		#endregion

		#region Public Methods
		
		public void setUpgradeSubType(TurretModifierType newSubType)
		{
			if (type != UpgradeType.TurretModifier)
			{
				Utils.Assert(false, "Upgrade::setUpgradeSubType | Upgrade type for sub type does not match assigned type UpgradeType.TurretModifier");
				return;
			}
			
			subType = (int)newSubType;
		}
		
		public void setUpgradeSubType(BaseEnhancementType newSubType)
		{
			if (type != UpgradeType.BaseEnhancement)
			{
				Utils.Assert(false, "Upgrade::setUpgradeSubType | Upgrade type for sub type does not match assigned type UpgradeType.BaseEnhancement");
				return;
			}
			
			subType = (int)newSubType;
		}
		
		public void setUpgradeSubType(BaseAddOnType newSubType)
		{
			if (type != UpgradeType.BaseAddOn)
			{
				Utils.Assert(false, "Upgrade::setUpgradeSubType | Upgrade type for sub type does not match assigned type UpgradeType.BaseAddOn");
				return;
			}
			
			subType = (int)newSubType;
		}
		
		#endregion
		
		#region Private Methods
		
		#endregion
	}
}