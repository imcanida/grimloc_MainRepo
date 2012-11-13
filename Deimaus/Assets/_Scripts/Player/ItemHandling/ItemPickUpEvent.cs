using UnityEngine;
using System.Collections;

public class ItemPickUpEvent : MonoBehaviour 
{
	public Stats playerStats;
	
	#region Variables
	//Heal
	public bool isHeal = false;
	public float healAmount = 0;
	
	//Damage
	public bool isDamage = false;
	public float damageAmount = 0;
	
	//Sheilds
	public bool addShields = false;
	public int shieldsToAdd = 0;
	
	public bool subShields = false;
	public int shieldsToSub = 0;
	
	//Health
	public bool addHealthContainers = false;
	public int healthToAdd = 0;
	
	public bool subHealthContainers = false;
	public int healthToSub = 0;
	
	//Movement Speed;
	public bool addMovementSpeed = false;
	public int movementSpeedToAdd = 0;
	
	public bool subMovementSpeed = false;
	public int movementSpeedToSub = 0;
	
	//Damage
	public bool addDamage = false;
	public int damageToAdd = 0;
	
	public bool subDamage = false;
	public int damageToSub = 0;
	
	//Projectile Speed?
	public bool addProjectileSpeed = false;
	public int projectileSpeedToAdd = 0;
	
	public bool subProjectileSpeed = false;
	public int projectileSpeedtoSub = 0;
	
	//Range
	public bool addRange = false;
	public int rangeToAdd = 0;
	
	public bool subRange = false;
	public int rangeToSub = 0;
	
	//Attack Speed
	public bool addAttackSpeed = false;
	public int attackSpeedToAdd = 0;
	
	public bool subAttackSpeed = false;
	public int attackSpeedToSub = 0;
	
	//Penentration
	public bool addPenetration = false;
	public int penetrationToAdd = 0;
	
	public bool subPenetration= false;
	public int penetrationToSub = 0;
	
	//Coins
	public bool addCoins = false;
	public int coinsToAdd = 0;
	
	public bool subCoins = false;
	public int coinsToSub = 0;
	
	//Keys
	public bool addKeys = false;
	public int keysToAdd = 0;
	
	public bool subKeys = false;
	public int keysToSub = 0;
	#endregion
	
	public void ItemEvent(Stats pStats)
	{
		playerStats = pStats;
			
		#region Damage or Heal
		if(isHeal)
			playerStats.AdjustHealth(healAmount);
		
		if(isDamage)
			playerStats.ApplyDamage(damageAmount);
		#endregion
		
		#region Health Containers
		if(addHealthContainers)
		{
			for(int i = 0; i < healthToAdd; i++)
			{
				playerStats.AddHealth();
			}
		}
		
		if(subHealthContainers)
		{
			for(int i = 0; i < healthToSub; i++)
			{
				playerStats.SubHealth();
			}
		}
		#endregion
		
		#region Shields
		if(addShields)
		{
			for(int i = 0; i < shieldsToAdd; i++)
			{
				playerStats.AddShield();
			}
		}
		
		if(subShields)
		{
			for(int i = 0; i < shieldsToSub; i++)
			{
				playerStats.SubShield();
			}
		}
		#endregion
		
		#region MovementSpeed
		if(addMovementSpeed)
		{
			for(int i = 0; i < movementSpeedToAdd; i++)
			{
				playerStats.AddMovementSpeed();
			}
		}
		
		if(subMovementSpeed)
		{
			for(int i = 0; i < movementSpeedToSub; i++)
			{
				playerStats.SubMovementSpeed();
			}
		}
		#endregion
		
		#region Damage
		if(addDamage)
		{
			for(int i = 0; i < damageToAdd; i++)
			{
				playerStats.AddDamage();
			}
		}
		
		if(subDamage)
		{
			for(int i = 0; i < damageToSub; i++)
			{
				playerStats.SubDamage();
			}
		}
		#endregion
		
		#region ProjectileSpeed
		if(addProjectileSpeed)
		{
			for(int i = 0; i < projectileSpeedToAdd; i++)
			{
				playerStats.AddProjectileSpeed();
			}
		}
		
		if(subProjectileSpeed)
		{
			for(int i = 0; i < projectileSpeedtoSub; i++)
			{
				playerStats.SubProjectileSpeed();
			}
		}
		#endregion
		
		#region Range
		if(addRange)
		{
			for(int i = 0; i < rangeToAdd; i++)
			{
				playerStats.AddRange();
			}
		}
		
		if(subShields)
		{
			for(int i = 0; i < rangeToSub; i++)
			{
				playerStats.SubRange();
			}
		}
		#endregion
		
		#region Attack Speed
		if(addAttackSpeed)
		{
			for(int i = 0; i < attackSpeedToAdd; i++)
			{
				playerStats.AddAttackSpeed();
			}
		}
		
		if(subAttackSpeed)
		{
			for(int i = 0; i < attackSpeedToSub; i++)
			{
				playerStats.SubAttackSpeed();
			}
		}
		#endregion	
		
		#region Penetration
		if(addPenetration)
		{
			for(int i = 0; i < penetrationToAdd; i++)
			{
				playerStats.AddPenetration();
			}
		}
		
		if(subPenetration)
		{
			for(int i = 0; i < penetrationToSub; i++)
			{
				playerStats.SubPenetration();
			}
		}
		#endregion	
		
		#region Coins
		if(addCoins)
		{
			playerStats.AddCoins(coinsToAdd);
		}
		
		if(subCoins)
		{
			playerStats.SubCoins(coinsToSub);
		}
		#endregion	
		
		#region Keys
		if(addKeys)
		{
			for(int i = 0; i < keysToAdd; i++)
			{
				playerStats.AddKey();
			}
		}
		
		if(subKeys)
		{
			for(int i = 0; i < keysToSub; i++)
			{
				playerStats.SubKey();
			}
		}
		#endregion	
		playerStats.UpdateLabels();
		
	}
}
