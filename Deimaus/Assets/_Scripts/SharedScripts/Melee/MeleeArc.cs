using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeArc : MonoBehaviour
{
	//public CharacterStats info;
	public float range = 5;
	public float spread = 80;
	public float forwardSpread = 30;
	public bool displayForwardDirectionSpread = false;
    public Transform holderCenter;
    private Collider[] hit;
		
	private int playerLayer = 1<<8;
	private int enemyLayer = 1<<15;
	//private PopUpHealthBar hb;
	
	void Awake()
	{
		holderCenter = transform;
	}
	
	public bool CanHitPlayer()
	{
		hit = Physics.OverlapSphere(holderCenter.position, range, playerLayer);
        foreach (Collider c in hit)
		{ 
            if (Vector3.Angle(holderCenter.forward, c.transform.position - holderCenter.position) <= spread)
            {
				return true;
			}
		}
		return false;
	}
	
	public bool CanSeePlayer(float distance, float angle)
	{
		hit = Physics.OverlapSphere(holderCenter.position, distance, playerLayer);
        foreach (Collider c in hit)
		{ 
            if (Vector3.Angle(holderCenter.forward, c.transform.position - holderCenter.position) <= angle)
            {
				return true;
			}
		}
		return false;
	}
	
	public bool CanHitEnemy()
	{
		hit = Physics.OverlapSphere(holderCenter.position, range, enemyLayer);
        foreach (Collider c in hit)
        {    
            if (Vector3.Angle(holderCenter.forward, c.transform.position - holderCenter.position) <= forwardSpread)
            {
				return true;
			}
		}
		return false;
	}
	
	public void ApplyDamage(Stats myStats)
	{
		StartCoroutine(AttackEnemies(myStats));
	}
    public IEnumerator AttackEnemies(Stats myStats)
    {
        //Check hit
        hit = Physics.OverlapSphere(holderCenter.position, range, enemyLayer);
        foreach (Collider c in hit)
		{
			
			EnemyAI eScript = c.GetComponent(typeof(EnemyAI)) as EnemyAI;
			if(eScript != null)
			{
				if (Vector3.Angle(holderCenter.forward, c.transform.position - holderCenter.position) <= spread)
                {
					Stats stats = c.GetComponent(typeof(Stats)) as Stats;
					
				}
			}
		}
		
		yield return null;
	}
	
	public void ApplyDamageToPlayers(Stats myStats)
	{
		StartCoroutine(AttackPlayers(myStats) );
	}
	public IEnumerator AttackPlayers(Stats myStats)
    {
        hit = Physics.OverlapSphere(holderCenter.position, range, playerLayer);
        foreach (Collider c in hit)
        {
			//Check if the Player is currently blocking
			Movement_Controller pMovement = c.GetComponent(typeof(Movement_Controller)) as Movement_Controller;
			
			if(pMovement != null)
			{
				 if (Vector3.Angle(holderCenter.forward, c.transform.position - holderCenter.position) <= spread)
	             {
					Stats pStats = c.GetComponent(typeof(Stats)) as Stats;
					pStats.ApplyDamage(-0.5f);
				}
			}
			yield return null;          
        }
    }
	
}
