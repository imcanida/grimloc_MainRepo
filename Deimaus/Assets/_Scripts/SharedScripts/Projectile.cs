using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class Projectile : MonoBehaviour 
{
	private BoneAnimation anims;
	public bool playerShot = false;
	
	private int rangeMod = 26;
	public float shotRange = 5;
	
	private Vector3 directionShot;
	private Transform myTrans;
	
	public string projectileAnimation = "GreenOrbThrow";
	public string endProjectileAnimation = "GreenOrbEnd";
	// Get a reference of the shadow of the projectile so we can change the depth as it passes over the range.
	public bool onlyPlayEndAnimationOnCollision = false;
	private bool wasInitialized = false;
	private Vector3 myInitalSize;
	public void OnEnable()
	{
		Physics.IgnoreLayerCollision( 9, 9 );
		Physics.IgnoreLayerCollision( 29, 31 );
		StartCoroutine( InitializeAnimations() );
		myTrans = this.transform;
	}
	
	IEnumerator InitializeAnimations()
	{
		//yield return new WaitForSeconds(0.005f);
		anims = GetComponentInChildren(typeof(BoneAnimation)) as BoneAnimation;
		//anims.Play(projectileAnimation);
		if(!wasInitialized)
		{
			yield return new WaitForSeconds(0.02f);
			wasInitialized = true;
			myInitalSize = myTrans.parent.localScale;
		}
		yield return null;
	}
	
	private Vector3 shotLocation;
	private string projectileActivationName = "";
	private Stats myStats;
	public void EnableProjectile(Collider col, bool isPlayer, Vector3 myShotLocation, Vector3 directionShot, string projectileActivationName, Stats playerStats)
	{
		//Player Stats.
		myStats = playerStats;
		
		//Rotation of the Projectile.
		myTrans.eulerAngles = new Vector3(myTrans.eulerAngles.x, 180, myTrans.eulerAngles.z);
		
		//Projectile Size based on Damage.
		myTrans.parent.localScale = myInitalSize;
		float projectileSizeRatio = (playerStats.Damage*0.1f) + 1;
		myTrans.parent.localScale = new Vector3(myTrans.parent.localScale.x, myTrans.parent.localScale.y, myTrans.parent.localScale.z)*projectileSizeRatio;
		
		//Ignore collision based on who shot it.
		if(isPlayer)
		{
			Physics.IgnoreLayerCollision( 8, 9);
		}
		else
		{
			Physics.IgnoreLayerCollision( 9, 10 );
		}
		//Ignore this collider so we don't collide with ourselves.
		Physics.IgnoreCollision(this.collider, col);
		//Direction
		this.directionShot = directionShot;
		//Range
		shotRange = playerStats.Range;
		//How many objects the projectile can penetrate through.
		this.penentrationCount = playerStats.ProjectilePenetration;
		//A Player shot the projectile
		playerShot = isPlayer;
		//Our activationName so we can deactivate the pooled object
		this.projectileActivationName = projectileActivationName;
		
		shotLocation = myShotLocation;
			
		if(anims == null)
			anims = GetComponentInChildren(typeof(BoneAnimation)) as BoneAnimation;
		anims.Play(projectileAnimation);

		myTrans.position = myTrans.parent.position;
		StartCoroutine(CheckDistance() );
	}
	
	private Collider[] hit;
	private float maxRangeMag;
	public float colliderSize = 3.25f;
	public float distanceRatio = -1.75f;
	public float shadowProjectileDiff = -1;
	public Transform shadow;
	
	private List<GameObject> hitObjects = new List<GameObject>();
	int penentrationCount = 0;
	IEnumerator CheckDistance()
	{		
		maxRangeMag = (shotRange*rangeMod);
		bool checkDistance = true;
		float endLocation = 0;
		bool setDistance = false;
		if(myStats.Range > 4 && (myStats.MovementSpeed < 5 || myStats.ProjectileSpeed < 5) )
			shadowProjectileDiff = -0.65f;
		else
			shadowProjectileDiff = -1;
		
		while(checkDistance)
		{			
			int layerMask;
			if(playerShot)
			{
				layerMask = 1 << 8 | 1<<9 | 1<< 31 | 1 << 29;
				layerMask = ~layerMask;
			}
			else
			{
				layerMask = 1 << 10 | 1<<9 | 1<< 31 | 1 << 29;;
				layerMask = ~layerMask;
			}
			
			hit = Physics.OverlapSphere(this.transform.position, colliderSize, layerMask);
			foreach (Collider c in hit)
			{ 			
				//We hit a trigger or Item
				if(c.gameObject.layer == 31 || c.gameObject.layer == 29)
				{
					yield return null;
				}
				int wallLayer = 27;
				if(c.gameObject.layer == wallLayer)
				{
					StartCoroutine(EndProjectile(true) );
					yield return null;
				}
				
				if(hitObjects.Contains(c.gameObject) )
				{
					//Ignore it
				}
				else
				{
					if(c.gameObject.GetComponent<Enemy>() != null)
					{
						c.gameObject.GetComponent<Enemy>().ApplyDamage(myStats.Damage);
					}
					
					hitObjects.Add(c.gameObject);
					penentrationCount--;
					if(penentrationCount < 0)
					{
						StartCoroutine(EndProjectile(true) );
						checkDistance = false;
						yield return null;
					}
				}
			}
			yield return new WaitForSeconds(0.02f);
			if(directionShot.x <= -1) //Left
			{
				if(!setDistance)
				{
					setDistance = true;
					endLocation = shotLocation.x - maxRangeMag;
				}
				distanceRatio = Mathf.Abs(myTrans.position.x - shotLocation.x) / maxRangeMag;
				if( myTrans.position.x <= endLocation )
				{
					StartCoroutine(EndProjectile(false) );
					checkDistance = false;
				}
				myTrans.position = new Vector3(myTrans.position.x, myTrans.position.y, myTrans.position.z+shadowProjectileDiff*(distanceRatio) ); 
			}
			else if(directionShot.x >= 1) //Right
			{			
				if(!setDistance)
				{
					setDistance = true;
					endLocation = shotLocation.x + maxRangeMag;
				}
				distanceRatio = (myTrans.position.x - shotLocation.x) / maxRangeMag;
				if( myTrans.parent.position.x >= endLocation )
				{
					StartCoroutine(EndProjectile(false) );
					checkDistance = false;
				}
				myTrans.position = new Vector3(myTrans.position.x, myTrans.position.y, myTrans.position.z+shadowProjectileDiff*(distanceRatio)); 
			}
			else if(directionShot.z <= -1) //Back
			{
				if(!setDistance)
				{
					setDistance = true;
					endLocation = shotLocation.z - maxRangeMag;
				}
				distanceRatio = Mathf.Abs(myTrans.position.z - shotLocation.z) / maxRangeMag;
				if( myTrans.position.z <= endLocation )
				{
					StartCoroutine(EndProjectile(false) );
					checkDistance = false;
				}
				myTrans.position = new Vector3(myTrans.position.x, myTrans.position.y, myTrans.position.z+shadowProjectileDiff*(distanceRatio) ); 
			}
			else if(directionShot.z >= 1) //Forward
			{
				if(!setDistance)
				{
					setDistance = true;
					endLocation = shotLocation.z + maxRangeMag;
				}
				distanceRatio = (myTrans.position.z - shotLocation.z) / maxRangeMag;
				if( myTrans.position.z >= endLocation )
				{
					StartCoroutine(EndProjectile(false) );
					checkDistance = false;
				}
				myTrans.position = new Vector3(myTrans.position.x, myTrans.position.y, myTrans.position.z+shadowProjectileDiff*(distanceRatio) ); 
			}
			yield return new WaitForSeconds(0.01f);
		}
	}
	
	void OnTriggerEnter(Collider c)
	{
		if(c.gameObject.layer == 31 || c.gameObject.layer == 29)
		{
			return;
		}
		int wallLayer = 27;
		if(c.gameObject.layer == wallLayer)
		{
			StartCoroutine(EndProjectile(true) );
			return;
		}
		
		if(hitObjects.Contains(c.gameObject) )
		{
			//Ignore it
		}
		else
		{
			if(c.gameObject.GetComponent<Enemy>() != null)
			{
				c.gameObject.GetComponent<Enemy>().ApplyDamage(myStats.Damage);
			}
			
			hitObjects.Add(c.gameObject);
			penentrationCount--;
			if(penentrationCount < 0)
			{
				StartCoroutine(EndProjectile(true) );
				return;
			}
		}
	}
	
	IEnumerator EndProjectile(bool collided)
	{
		shadow.gameObject.active = false;
		hitObjects = new List<GameObject>();
		myTrans.parent.rigidbody.isKinematic = true;
		if(onlyPlayEndAnimationOnCollision && collided || !onlyPlayEndAnimationOnCollision)
		{
			anims.Play(endProjectileAnimation);
			yield return new WaitForSeconds(anims[endProjectileAnimation].length/2);
		}
		myTrans.parent.rigidbody.isKinematic = false;
		myTrans.parent.rigidbody.velocity = new Vector3(0,0,0);
		anims.Play(projectileAnimation);
		_PoolingManager.Instance.DeactivatePooledItem(projectileActivationName, myTrans.parent.gameObject);
		yield return null;
	}
	
	void OnDrawGizmosSelected() 
	{
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, colliderSize);
    }

}
