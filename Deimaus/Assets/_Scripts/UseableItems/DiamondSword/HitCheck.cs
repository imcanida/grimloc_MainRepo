using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitCheck : MonoBehaviour 
{
	private Collider[] hit;
	public float colliderSize = 10;
	private int layerMask;
	public int damage = 10;
	public bool isPlayer = true;
	public Transform hitLocation;
	private List<GameObject> hitObjects = new List<GameObject>();
	
	public void Start()
	{
		Physics.IgnoreLayerCollision( 9, 9 );
	}
	public void Update()
	{
		CheckCollision();
	}
	public void CheckCollision()
	{
		layerMask = 0;
		hitObjects = new List<GameObject>();
		//Ignore collision based on who shot it.
		if(isPlayer)
		{
			layerMask = 1 << 8 | 1<<9 | 1<< 31 | 1 << 29;
			layerMask = ~layerMask;
			Physics.IgnoreLayerCollision( 8, 9);
		}
		else
		{
			layerMask = 1 << 10 | 1<<9 | 1<< 31 | 1 << 29;;
			layerMask = ~layerMask;
			Physics.IgnoreLayerCollision( 9, 10 );
		}
		hit = Physics.OverlapSphere(this.transform.position, colliderSize, layerMask);
		foreach (Collider c in hit)
		{ 			
			if(hitObjects.Contains(c.gameObject) )
			{
				//Ignore it
			}
			else
			{
				if(c.gameObject.GetComponent<Enemy>() != null)
				{
					c.gameObject.GetComponent<Enemy>().ApplyDamage(damage);
					hitObjects.Add(c.gameObject);
				}
			}
		}
	}
	
	void OnDrawGizmosSelected() 
	{
        Gizmos.color = Color.white;
		if(hitLocation != null)
        	Gizmos.DrawWireSphere(hitLocation.position, colliderSize);
    }
	
	public bool isObsticalInWay()
	{
		return hitObstical;
	}
	
	private bool hitObstical = false;
	public void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.layer == 26 || col.gameObject.layer == 27)
		{
			hitObstical = true;
			Debug.Log("We hit something");
		}
	}
}
