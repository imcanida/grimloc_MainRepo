using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;


public class Stats : MonoBehaviour 
{
	private int maxStat = 8;
	public CharacterSelect selectedCharacter = CharacterSelect.Grimloc;
	public BoneAnimation me;
	public Color startColor;
	public Color endColor;
	private List<string> boneList = new List<string>();

	void Start()
	{
		boneList.Add("Head");
		boneList.Add("Helmet_Back");
		boneList.Add("Helmet_Front");
		boneList.Add("Helmet_Side");
		
		boneList.Add("Body");
		
		boneList.Add("LeftArm");
		boneList.Add("RightArm");
		
		boneList.Add("LeftLeg");
		boneList.Add("RightLeg");
		UpdateLabels();
		switch(selectedCharacter)	
		{
		case CharacterSelect.Grimloc:
			health = 3;
			armor = 0;
			movementSpeed = 3;
			attackSpeed = 3;
			projectileSpeed = 3;
			damage = 1;
			range = 3;
			currentArmor = 0;
			currentHealth = 2.5f;
			penetration = 0;
			coins = 0;
			keys = 0;
			maxStat = 8;
			break;
		}
	}
	
	public float weight = 0.8f;
	public float duration = 1f;
	public int iterations = 4;
	private bool isInvunerable = false;
	public IEnumerator TurnInvincible(float invunerabilityTimer)
	{
		for(int i = 0; i < boneList.Count; i++)
			me.FlashBoneColor(boneList[i], startColor, weight, endColor, duration, weight, iterations, true);
		isInvunerable = true;
		yield return new WaitForSeconds(invunerabilityTimer);
		isInvunerable = false;
		me.StopAllFlashingBoneColors();
	}
	
	public void ApplyDamage(float amount)
	{
		if(isInvunerable)
			return;
		StartCoroutine(TurnInvincible(0.5f) );			//Turn invunerable for 1 second.
		float tempDamageRemaining = amount;
		if(currentArmor > 0)
		{
			float tempArmor = currentArmor - amount;
			if(tempArmor > 0)
			{
				AdjustArmor(amount);
				if(currentArmor <= 0 && currentHealth <= 0)
					playerIsDead = true;
				return;
			}
			else
			{
				tempDamageRemaining -= currentArmor;
				currentArmor = 0;
				armor = 0;
			}		
		}
		float tempHealth = currentHealth - tempDamageRemaining;
		if(tempHealth > 0)
		{
			AdjustHealth(tempDamageRemaining);
			return;
		}
		else
		{
			currentHealth = 0;
		}
		
		if(currentArmor <= 0 && currentHealth <= 0)
			playerIsDead = true;
	}
	
	public void AddShield()
	{
		if(armor >= maxStat)
			return;
		currentArmor++;
		armor++;
	}
	
	public bool SubShield()
	{
		if(armor <= 1 && health == 0)
			return false;
		if(armor <= 0)
			return false;
		armor--;
		if(currentArmor > armor)
			currentArmor = armor;
		return true;
	}
	
	public bool SubShield(int amount)
	{
		if(armor <= amount && health == 0)
			return false;
		if(armor <= amount)
			return false;
		armor-= amount;
		if(currentArmor > armor)
			currentArmor = armor;
		return true;
	}
	
	public void AddHealth()
	{
		if(health >= maxStat)
			return;
		currentHealth++;
		health++;
	}
	
	public bool SubHealth()
	{
		if(health <= 1 && armor == 0)
			return false;
		if(health <= 0)
			return false;
		health--;
		if(currentHealth > health)
			currentHealth = health;
		return true;
	}
	
	public bool SubHealth(int amount)
	{
		if(health <= amount && armor == 0)
			return false;
		if(health <= amount)
			return false;
		health-=amount;
		if(currentHealth > health)
			currentHealth = health;
		
		return true;
	}
	
	public void HealHalfHeart()
	{
		AdjustHealth(0.5f);
	}
	
	public void TakeDamageHit()
	{
		ApplyDamage(-0.25f);
	}	
	private bool playerIsDead = false;
	public bool PlayerIsDead 
	{
		get{return playerIsDead;}
		set{playerIsDead = value;}
	}
	/** Health  **/
	public bool tradeHealth(int amount)
	{
		int tempHealth = health-amount;
		if(tempHealth <= 0 && armor <= 0)
		{
			return false;
		}
		else
		{
			health = tempHealth;
			return true;
		}
	}
	
	public int Health 
	{
		get{return health;}
		set{health = value;}
	}
	private int health = 3;					//Actually health that can be replenished
	private int maxHealth = 8;
	
	private float currentHealth;
	public float GetHealth()
	{
		return currentHealth;
	}
	public bool AdjustHealth(float amount)
	{
		float tempHealth = currentHealth+amount;
		if(tempHealth > health)
		{
			currentHealth = health;
		}
		else if(tempHealth <= 0)
		{
			currentHealth = 0;
			if(currentArmor == 0)
				playerIsDead = true;
		}
		else
		{
			currentHealth = tempHealth;
		}
		return playerIsDead;
	}
	
	/** Armor  **/	
	public bool tradeArmor(int amount)
	{
		int tempArmor = armor - amount;
		if(tempArmor <= 0 && health <= 0)
		{
			return false;
		}
		else
		{
			armor = tempArmor;
			return true;
		}
	}
	private int armor = 0;					//Temp health items.
	public int Armor 
	{
		get{return armor;}
		set{armor = value;}
	}
	private int maxArmor = 8;
	
	private float currentArmor;
	public float GetArmor()
	{
		return currentArmor;
	}
	
	public float AdjustArmor(float amount)
	{
		float tempArmor = currentArmor+amount;
		if(tempArmor > armor)
		{
			currentArmor = armor;
		}
		else if(tempArmor <= 0)
		{
			currentArmor = 0;
		}
		else
		{
			currentArmor = tempArmor;
		}
		
		if(Mathf.CeilToInt(currentArmor) < armor)
		{
			armor--;
		}
		return currentArmor;
	}
	
	#region Projectile Penetration
	public int ProjectilePenetration 
	{
		get{return penetration;}
		set{penetration = value;}
	}
	private int penetration = 0;		
	public void AddPenetration()
	{
		penetration+= 1;
		if(penetration >= maxStat)
			penetration = maxStat;
	}
	public void SubPenetration()
	{
		penetration--;
		if(penetration <= 0)
			penetration = 0;
	}
	#endregion
	
	#region Coins
	public int Coins
	{
		get{return coins;}
		set{coins = value;}
	}
	private int coins = 0;		
	public void AddCoins(int amount)
	{
		coins+= amount;
		UpdateLabels();
	}
	
	//Can be used for item purchases. If the user has the money it will return true to pick it up.
	public bool SubCoins(int amount)
	{
		int tempCoins = coins-amount;
		if(tempCoins < 0)
		{
			return false;
		}
		else
		{
			coins = tempCoins;
			UpdateLabels();
			return true;
		}
	}
	#endregion
	
	#region Crystal Keys
	public int Keys
	{
		get{return keys;}
		set{keys = value;}
	}
	private int keys = 0;		
	public void AddKey()
	{
		keys++;
		UpdateLabels();
	}
	
	public bool SubKey()
	{
		int tempKeys = keys-1;
		if(tempKeys < 0)
		{
			return false;
		}
		else
		{
			keys = tempKeys;
			UpdateLabels();
			return true;
		}
	}
	#endregion
	
	/** Movement Speed  **/
	public int MovementSpeed 
	{
		get{return movementSpeed;}
		set{movementSpeed = value;}
	}
	private int movementSpeed = 3;			//How fast he can move
	public void AddMovementSpeed()
	{
		movementSpeed+= 1;
		if(movementSpeed >= maxStat)
			movementSpeed = maxStat;
	}
	public void SubMovementSpeed()
	{
		movementSpeed-= 1;
		if(movementSpeed <= 2)
			movementSpeed = 2;
	}
	
	/** Attack Speed  **/
	public int AttackSpeed 
	{
		get{return attackSpeed;}
		set{attackSpeed = value;}
	}
	private int attackSpeed = 3;			//How fast he can attack
	public void AddAttackSpeed()
	{
		attackSpeed+= 1;
		if(attackSpeed >= maxStat)
			attackSpeed = maxStat;
	}
	public void SubAttackSpeed()
	{
		attackSpeed-= 1;
		if(attackSpeed <= 2)
			attackSpeed = 2;
	}
	
	/** Projectile Speed  **/
	public int ProjectileSpeed 
	{
		get{return projectileSpeed;}
		set{projectileSpeed = value;}
	}
	private int projectileSpeed = 3;		//How fast he can attack
	public void AddProjectileSpeed()
	{
		projectileSpeed+= 1;
		if(projectileSpeed >= maxStat)
			projectileSpeed = maxStat;
	}
	public void SubProjectileSpeed()
	{
		projectileSpeed-= 1;
		if(projectileSpeed <= 1)
			projectileSpeed = 1;
	}
	
	/** Damage  **/
	public int Damage 
	{
		get{return damage;}
		set{damage = value;}
	}
	private int damage = 1;					//How much damage grimloc deals per hit
	public void AddDamage()
	{
		damage+= 1;
		if(damage >= maxStat)
			damage = maxStat;
	}
	public void SubDamage()
	{
		damage-= 1;
		if(damage <= 1)
			damage = 1;
	}
	
	/** Range  **/
	public int Range 
	{
		get{return range;}
		set{range = value;}
	}
	private int range = 3;					//How far a shot can go
	public void AddRange()
	{
		range+= 1;
		if(range >= maxStat)
			range = maxStat;
	}
	public void SubRange()
	{
		range-= 1;
		if(range <= 2)
			range = 2;
	}
	
	public UILabel lblMoveSpeed;
	public UILabel lblAttackSpeed;
	public UILabel lblRange;
	public UILabel lblProjectileSpeed;
	public UILabel lblDamage;
	public UILabel lblPenentration;
	public UILabel lblCoins;
	public UILabel lblKeys;
	
	public UILabel lblHealth;
	public UILabel lblShield;
	public void UpdateLabels()
	{
		lblMoveSpeed.text = "MoveSpeed: "+movementSpeed;
		lblAttackSpeed.text = "Attack Speed: "+attackSpeed;
		lblRange.text = "Range: "+range;
		lblProjectileSpeed.text = "Projectile Speed: "+projectileSpeed;
		lblDamage.text = "Damage: "+damage;
		
		lblHealth.text = "Health: "+health;
		lblShield.text = "Shields: "+armor;
		
		
		lblPenentration.text = "Penetration: "+penetration;
		lblKeys.text = ""+keys;
		lblCoins.text = ""+coins;
		
		
	}
	
}

public enum CharacterSelect
{
	Grimloc,
	GenericMob_01,
	
}
