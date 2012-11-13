using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class Enemy : MonoBehaviour 
{
	public BoneAnimation me;
	private float health;
	
	public Color startColor;
	public Color endColor;
	
	public List<string> bones = new List<string>();
	
	public void Start()
	{
		SetHealth(hitsEnemyCanTake);
		bones.Add("Head");
		bones.Add("LArm");
		bones.Add("RArm");
		bones.Add("RLeg");
		bones.Add("LLeg");
		bones.Add("Body");
	}
	
	public void ApplyDamage(float damage)
	{
		float tempHealth = health - damage;
		if(tempHealth > 0)
		{
			health = tempHealth; 
			StartCoroutine(FlashColor() );
		
		}
		else
		{
			health = 0;
			isDead = true;
		}
	}
	private IEnumerator FlashColor()
	{
		for(int i = 0; i < bones.Count; i++)
			me.FlashBoneColor(bones[i], startColor, 0.5f, endColor, 0.5f, 0.5f, 3, true);
		yield return new WaitForSeconds(0.5f);
		me.StopAllFlashingBoneColors();
	}
	
	private bool isDead;
	public bool IsDead {
		get{return isDead;}
		set{isDead = value;}
	}
	
	
	public float medianDamage = 4.5f;
	public int hitsEnemyCanTake = 4;
	public void SetHealth(int hitsEnemyCanTake)
	{
		health = (int) hitsEnemyCanTake*medianDamage;
	}
}
