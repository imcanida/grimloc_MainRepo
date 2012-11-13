using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ItemPickUpEvent))]
public class ItemEventsEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		ItemPickUpEvent tar = (ItemPickUpEvent)target;
		
		float thirdOfScreen = Screen.width/3-10;
		
		#region Health Containers
		GUILayout.BeginHorizontal();
		GUILayout.Label("Health Containers | Add: " + tar.healthToAdd + " | Sub: " + tar.healthToSub, GUILayout.Width(thirdOfScreen*2) );
		if(GUILayout.Button("+", GUILayout.Width(thirdOfScreen/2) ) )
		{
			if(tar.healthToAdd == 0 && tar.healthToSub > 0)
			{
				tar.healthToSub--;
			}
			else
			{
				tar.addHealthContainers = true;
				tar.healthToAdd++;
				if(tar.healthToAdd > 8)
					tar.healthToAdd = 8;
				tar.subHealthContainers = false;
			}
		}
		if(GUILayout.Button("-", GUILayout.Width(thirdOfScreen/2) ) )
		{
			
			if(tar.healthToSub == 0 && tar.healthToAdd > 0)
			{
				tar.healthToAdd--;
			}
			else
			{
				tar.addHealthContainers = false;
				tar.healthToSub++;
				if(tar.healthToSub > 8)
					tar.healthToSub = 8;
				tar.subHealthContainers = true;
			}
			
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(5f);
		#endregion
		
		#region Shields
		GUILayout.BeginHorizontal();
		GUILayout.Label("Shields | Add: " + tar.shieldsToAdd + " | Sub: " + tar.shieldsToSub, GUILayout.Width(thirdOfScreen*2) );
		if(GUILayout.Button("+", GUILayout.Width(thirdOfScreen/2) ) )
		{
			if(tar.shieldsToAdd == 0 && tar.shieldsToSub > 0)
			{
				tar.shieldsToSub--;
			}
			else
			{
				tar.addShields = true;
				tar.shieldsToAdd++;
				if(tar.shieldsToAdd > 8)
					tar.shieldsToAdd = 8;
				tar.subShields = false;
			}
		}
		if(GUILayout.Button("-", GUILayout.Width(thirdOfScreen/2) ) )
		{
			
			if(tar.shieldsToSub == 0 && tar.shieldsToAdd > 0)
			{
				tar.shieldsToAdd--;
			}
			else
			{
				tar.addShields = false;
				tar.shieldsToSub++;
				if(tar.shieldsToSub > 8)
					tar.shieldsToSub = 8;
				tar.subShields = true;
			}
			
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(5f);
		#endregion
		
		#region MoveSpeed
		GUILayout.BeginHorizontal();
		GUILayout.Label("Move Speed | Add: " + tar.movementSpeedToAdd + " | Sub: " + tar.movementSpeedToSub, GUILayout.Width(thirdOfScreen*2) );
		if(GUILayout.Button("+", GUILayout.Width(thirdOfScreen/2) ) )
		{
			if(tar.movementSpeedToAdd == 0 && tar.movementSpeedToSub > 0)
			{
				tar.movementSpeedToSub--;
			}
			else
			{
				tar.addMovementSpeed = true;
				tar.movementSpeedToAdd++;
				if(tar.movementSpeedToAdd > 8)
					tar.movementSpeedToAdd = 8;
				tar.subMovementSpeed = false;
			}
		}
		if(GUILayout.Button("-", GUILayout.Width(thirdOfScreen/2) ) )
		{
			
			if(tar.movementSpeedToSub == 0 && tar.movementSpeedToAdd > 0)
			{
				tar.movementSpeedToAdd--;
			}
			else
			{
				tar.addMovementSpeed = false;
				tar.movementSpeedToSub++;
				if(tar.movementSpeedToSub > 8)
					tar.movementSpeedToSub = 8;
				tar.subMovementSpeed = true;
			}
			
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(5f);
		#endregion
		
		#region Attack Speed
		GUILayout.BeginHorizontal();
		GUILayout.Label("Attack Speed | Add: " + tar.attackSpeedToAdd + " | Sub: " + tar.attackSpeedToSub, GUILayout.Width(thirdOfScreen*2) );
		if(GUILayout.Button("+", GUILayout.Width(thirdOfScreen/2) ) )
		{
			if(tar.attackSpeedToAdd == 0 && tar.attackSpeedToSub > 0)
			{
				tar.attackSpeedToSub--;
			}
			else
			{
				tar.addAttackSpeed = true;
				tar.attackSpeedToAdd++;
				if(tar.attackSpeedToAdd > 8)
					tar.attackSpeedToAdd = 8;
				tar.subAttackSpeed = false;
			}
		}
		if(GUILayout.Button("-", GUILayout.Width(thirdOfScreen/2) ) )
		{
			
			if(tar.attackSpeedToSub == 0 && tar.attackSpeedToAdd > 0)
			{
				tar.attackSpeedToAdd--;
			}
			else
			{
				tar.addAttackSpeed = false;
				tar.attackSpeedToSub++;
				if(tar.attackSpeedToSub > 8)
					tar.attackSpeedToSub = 8;
				tar.subAttackSpeed = true;
			}
			
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(5f);
		#endregion
		
		#region Projectile Speed
		GUILayout.BeginHorizontal();
		GUILayout.Label("Projectile Speed | Add: " + tar.projectileSpeedToAdd + " | Sub: " + tar.projectileSpeedtoSub, GUILayout.Width(thirdOfScreen*2) );
		if(GUILayout.Button("+", GUILayout.Width(thirdOfScreen/2) ) )
		{
			if(tar.projectileSpeedToAdd == 0 && tar.projectileSpeedtoSub > 0)
			{
				tar.projectileSpeedtoSub--;
			}
			else
			{
				tar.addProjectileSpeed = true;
				tar.subProjectileSpeed = false;
				
				tar.projectileSpeedToAdd++;
				if(tar.projectileSpeedToAdd > 8)
					tar.projectileSpeedToAdd = 8;
			}
		}
		if(GUILayout.Button("-", GUILayout.Width(thirdOfScreen/2) ) )
		{
			
			if(tar.projectileSpeedtoSub == 0 && tar.projectileSpeedToAdd > 0)
			{
				tar.projectileSpeedToAdd--;
			}
			else
			{
				tar.addProjectileSpeed = false;
				tar.subProjectileSpeed = true;
				
				tar.projectileSpeedtoSub++;
				if(tar.projectileSpeedtoSub > 8)
					tar.projectileSpeedtoSub = 8;
			}
			
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(5f);
		#endregion
		
		#region Range
		GUILayout.BeginHorizontal();
		GUILayout.Label("Range | Add: " + tar.rangeToAdd + " | Sub: " + tar.rangeToSub, GUILayout.Width(thirdOfScreen*2) );
		if(GUILayout.Button("+", GUILayout.Width(thirdOfScreen/2) ) )
		{
			if(tar.rangeToAdd == 0 && tar.rangeToSub > 0)
			{
				tar.rangeToSub--;
			}
			else
			{
				tar.addRange = true;
				tar.subRange = false;
				
				tar.rangeToAdd++;
				if(tar.rangeToAdd > 8)
					tar.rangeToAdd = 8;
			}
		}
		if(GUILayout.Button("-", GUILayout.Width(thirdOfScreen/2) ) )
		{
			
			if(tar.rangeToSub == 0 && tar.rangeToAdd > 0)
			{
				tar.rangeToAdd--;
			}
			else
			{
				tar.addRange = false;
				tar.subRange = true;
				
				tar.rangeToSub++;
				if(tar.rangeToSub > 8)
					tar.rangeToSub = 8;
			}
			
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(5f);
		#endregion
		
		#region Damage
		GUILayout.BeginHorizontal();
		GUILayout.Label("Projectile Damange | Add: " + tar.damageToAdd + " | Sub: " + tar.damageToSub, GUILayout.Width(thirdOfScreen*2) );
		if(GUILayout.Button("+", GUILayout.Width(thirdOfScreen/2) ) )
		{
			if(tar.damageToAdd == 0 && tar.damageToSub > 0)
			{
				tar.damageToSub--;
			}
			else
			{
				tar.addDamage = true;
				tar.subDamage = false;
				
				tar.damageToAdd++;
				if(tar.damageToAdd > 8)
					tar.damageToAdd = 8;
			}
		}
		if(GUILayout.Button("-", GUILayout.Width(thirdOfScreen/2) ) )
		{
			
			if(tar.damageToSub == 0 && tar.damageToAdd > 0)
			{
				tar.damageToAdd--;
			}
			else
			{
				tar.addDamage = false;
				tar.subDamage = true;
				
				tar.damageToSub++;
				if(tar.damageToSub > 8)
					tar.damageToSub = 8;
			}
			
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(5f);
		#endregion
		
		#region Penetration
		GUILayout.BeginHorizontal();
		GUILayout.Label("Projectile Penetration | Add: " + tar.penetrationToAdd + " | Sub: " + tar.penetrationToSub, GUILayout.Width(thirdOfScreen*2) );
		if(GUILayout.Button("+", GUILayout.Width(thirdOfScreen/2) ) )
		{
			if(tar.penetrationToAdd == 0 && tar.penetrationToSub > 0)
			{
				tar.penetrationToSub--;
			}
			else
			{
				tar.addPenetration = true;
				tar.subPenetration = false;
				
				tar.penetrationToAdd++;
				if(tar.penetrationToAdd > 8)
					tar.penetrationToAdd = 8;
			}
		}
		if(GUILayout.Button("-", GUILayout.Width(thirdOfScreen/2) ) )
		{
			
			if(tar.penetrationToSub == 0 && tar.penetrationToAdd > 0)
			{
				tar.penetrationToAdd--;
			}
			else
			{
				tar.addPenetration = false;
				tar.subPenetration = true;
				
				tar.penetrationToSub++;
				if(tar.penetrationToSub > 8)
					tar.penetrationToSub = 8;
			}
			
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(5f);
		#endregion
		
		#region Coins
		GUILayout.BeginHorizontal();
		GUILayout.Label("Coins | Add: " + tar.coinsToAdd + " | Sub: " + tar.coinsToSub, GUILayout.Width(thirdOfScreen*2) );
		if(GUILayout.Button("+", GUILayout.Width(thirdOfScreen/2) ) )
		{
			if(tar.coinsToAdd == 0 && tar.coinsToSub > 0)
			{
				tar.coinsToSub--;
			}
			else
			{
				tar.addCoins = true;
				tar.subCoins = false;
				
				tar.coinsToAdd++;
				if(tar.coinsToAdd > 8)
					tar.coinsToAdd = 8;
			}
		}
		if(GUILayout.Button("-", GUILayout.Width(thirdOfScreen/2) ) )
		{
			
			if(tar.coinsToSub == 0 && tar.coinsToAdd > 0)
			{
				tar.coinsToAdd--;
			}
			else
			{
				tar.addCoins = false;
				tar.subCoins = true;
				
				tar.coinsToSub++;
				if(tar.coinsToSub > 8)
					tar.coinsToSub = 8;
			}
			
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(5f);
		#endregion
		
		#region Keus
		GUILayout.BeginHorizontal();
		GUILayout.Label("Keys | Add: " + tar.keysToAdd + " | Sub: " + tar.keysToSub, GUILayout.Width(thirdOfScreen*2) );
		if(GUILayout.Button("+", GUILayout.Width(thirdOfScreen/2) ) )
		{
			if(tar.keysToAdd == 0 && tar.keysToSub > 0)
			{
				tar.keysToSub--;
			}
			else
			{
				tar.addKeys = true;
				tar.subKeys = false;
				
				tar.keysToAdd++;
				if(tar.keysToAdd > 8)
					tar.keysToAdd = 8;
			}
		}
		if(GUILayout.Button("-", GUILayout.Width(thirdOfScreen/2) ) )
		{
			
			if(tar.keysToSub == 0 && tar.keysToAdd > 0)
			{
				tar.keysToAdd--;
			}
			else
			{
				tar.addKeys = false;
				tar.subKeys = true;
				
				tar.keysToSub++;
				if(tar.keysToSub > 8)
					tar.keysToSub = 8;
			}
			
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(5f);
		#endregion
		
		#region Healing
		GUILayout.BeginHorizontal();
		GUILayout.Label("Healing | Amount: " + tar.healAmount, GUILayout.Width(thirdOfScreen*2) );
		if(GUILayout.Button("+", GUILayout.Width(thirdOfScreen/2) ) )
		{
			tar.isHeal = true;
			tar.healAmount+= 0.25f;
			if(tar.healAmount > 10)
			{
				tar.healAmount = 10;
			}
		}
		if(GUILayout.Button("-", GUILayout.Width(thirdOfScreen/2) ) )
		{
			tar.healAmount-= 0.25f;
			if(tar.healAmount <= 0)
			{
				tar.healAmount = 0;
				tar.isHeal = false;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(5f);
		#endregion
		
		#region Deal Damage
		GUILayout.BeginHorizontal();
		GUILayout.Label("Deal Damage | Amount: " + tar.damageAmount, GUILayout.Width(thirdOfScreen*2) );
		if(GUILayout.Button("+", GUILayout.Width(thirdOfScreen/2) ) )
		{
			tar.isDamage = true;
			tar.damageAmount+= 0.25f;
			if(tar.damageAmount > 10)
			{
				tar.damageAmount = 10;
			}
		}
		if(GUILayout.Button("-", GUILayout.Width(thirdOfScreen/2) ) )
		{
			tar.damageAmount-= 0.25f;
			if(tar.damageAmount <= 0)
			{
				tar.damageAmount = 0;
				tar.isDamage = false;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(5f);
		#endregion
	}
}
