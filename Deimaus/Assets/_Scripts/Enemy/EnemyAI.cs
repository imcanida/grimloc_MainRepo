using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class EnemyAI : MonoBehaviour {

	public AIBehavior behave = AIBehavior.idle;	//Only public for debugging purposes
	public AttackState attackState = AttackState.aggressive;
	
	public Transform myTarget;
	public Stats myInfo;
	Transform myTransform;
	public Transform forwardDirection; 
	public Enemy myHealth;
	public BoneAnimation animations;
	void OnEnable()
	{		
		StartCoroutine(CoUpdate() );
	}
	
	public float yRotationMod = 1;
	public float updateDelayTime = 0.1f;
	public float zMoveSpeed = 1;
	bool switchedDirections = true;
	bool stillLooking = false;
	
	public string walkAnimation = "walk";
	public string runAnimation = "run";
	public string attackAnimation = "attack";
	public string idleAninimation = "idle";
	public string deathAnimation = "idle";
	
	private float baseYPosition = 0;
	
	public bool onceHasTargetNeverLoses = true;
	IEnumerator CoUpdate()
	{
		myTransform = this.transform;
		baseYPosition = myTransform.localPosition.y;
		if(meleeRange == null)
			meleeRange = myTransform.GetComponentInChildren(typeof(MeleeArc)) as MeleeArc;
		
		if(seek == null)
				seek = GetComponent(typeof(AIFollow)) as AIFollow;
		
		if(movement == null)
			movement = GetComponent(typeof(CharacterController)) as CharacterController;
		
		yield return new WaitForSeconds(2f);
		//Initialize AI
		while(true)
		{		
			if(myHealth.IsDead)
			{
				behave = AIBehavior.die;
			}
			else
			{
				Vector3 horizontalVelocity = new Vector3(movement.velocity.x, 0, movement.velocity.z);
				if(horizontalVelocity.magnitude > 0.1f )
				{
					if(horizontalVelocity.magnitude > 2.5f)
						animations.CrossFade(runAnimation);
					else
						animations.CrossFade(walkAnimation);
				}
				else
					animations.CrossFade(idleAninimation);
				
				if(CanSeePlayer() || myTarget != null)
				{
					stillLooking = true;
					if(CanSeePlayer() )
					{
						countTilLostTarget = 0;
					}
					if(!CanSeePlayer() || !InMeleeRange())
						FlipForward();
					else
						behave = AIBehavior.attack;
				}
				if(!CanSeePlayer() && myTarget != null && stillLooking && switchedDirections)
				{
					StartCoroutine(FlipDirectionDelay() );
					switchedDirections = false;
					forwardDirection.eulerAngles = new Vector3(0, forwardDirection.eulerAngles.y*yRotationMod, 0);
					if( (forwardDirection.eulerAngles.y < -80 || forwardDirection.eulerAngles.y > 240) && forwardDirection.eulerAngles.z < 100)
					{
						forwardDirection.eulerAngles = new Vector3(0, forwardDirection.eulerAngles.y, 180);
					}
					else
					{
						forwardDirection.eulerAngles = new Vector3(0, forwardDirection.eulerAngles.y, 0);
					}
				}
				else if(countTilLostTarget >= lostTargetNumber)
				{
					myTarget = null;
				}
				
				forwardDirection.position = myTransform.position;
				
			}
			//Debug.Log("State: "+ ((AIBehavior) behave).ToString("F") );
			switch(behave)
			{
			case AIBehavior.idle:
				StartCoroutine(Idle() );
				break;
			case AIBehavior.pursue:
				StartCoroutine(Pursue() );
				break;
			case AIBehavior.attack:
				StartCoroutine(Attack() );
				break;
			case AIBehavior.flee:
				StartCoroutine(Flee() );
				break;
			case AIBehavior.die:
				StartCoroutine(Die() );
				break;
			}			
			yield return new WaitForSeconds(updateDelayTime);
		}
	}
	
	bool canSwitch = false;
	public int lostTargetNumber = 10;	//5 seconds of search time 2times/per second.
	int countTilLostTarget = 0;
	public float switchForwardTime = 0.2f;
	
	public IEnumerator FlipDirectionDelay()
	{
		canSwitch = false;
		yield return new WaitForSeconds(switchForwardTime);
		canSwitch = true;
	}
	public void FlipForward()
	{
		if(canSwitch)
		{
			countTilLostTarget++;
			yRotationMod *= -1;
			switchedDirections = true;
		}
	}
	IEnumerator Idle()
	{
		if(CanSeePlayer() || myTarget != null)
		{
			behave = AIBehavior.pursue;
		}
		yield return null;
	}
	
	IEnumerator Pursue()
	{
		if(InMeleeRange() &&  !inAttackDelay )
		{
			behave = AIBehavior.attack;
			yield return null;
		}
		else
		{
			TraverseToTarget();
			behave = AIBehavior.idle;
		}
	}
	
	//Pull Attack delay from Melee Script.
	bool inAttackDelay = false;
	bool finishedAttack = false;
	
	public float attackDelay = 0.5f;
	public float attackSpeed = 1;
	private Stats pStats;
	private bool hasMelee = false;
	public float dmgAmount = -0.5f;
	IEnumerator Attack()
	{
		if(!inAttackDelay)
		{
			//animations[attackAnimation].speed = 1*attackSpeed;
			//animations.CrossFade(attackAnimation);
			if(hasMelee)
				StartCoroutine(CheckAttackHit());
			else
			{
				pStats = myTarget.GetComponent(typeof(Stats)) as Stats;
				pStats.ApplyDamage(dmgAmount);
			}
			StartCoroutine(AttackDelay(attackDelay));
			behave = AIBehavior.pursue;
		}
		behave = AIBehavior.idle;
		yield return null;
	}
	
	IEnumerator CheckAttackHit()
	{
		yield return new WaitForSeconds(((animations[attackAnimation].length)/attackSpeed) / 2);
		meleeRange.ApplyDamageToPlayers(myInfo);
	}
	IEnumerator AttackDelay(float time)
	{
		inAttackDelay = true;
		yield return new WaitForSeconds(time);
		inAttackDelay = false;
	}
		
	IEnumerator Flee()
	{
		yield return null;
	}
	
	IEnumerator Die()
	{
		//Death Animation
		animations.CrossFade(deathAnimation);
		myTarget = null;
		seek.Stop();
		movement.enabled = false;
		yield return null;
	}
	
	
	AIFollow seek;
	CharacterController movement;
	Vector3 moveDirection;
	public float movementSpeed = 5f;
	public void TraverseToTarget()
	{
		seek.speed = movementSpeed;
		seek.target = myTarget;
       	movement.Move(moveDirection * Time.deltaTime);
	}
		
    private Collider[] hit;
	public float viewRange = 8;
	public float viewAngle = 80;	//Forward.	
	

	private int playerLayer = 1<<8;
	public bool CanSeePlayer()
	{
		hit = Physics.OverlapSphere(forwardDirection.position, viewRange, playerLayer);
        foreach (Collider c in hit)
		{ 
            if (Vector3.Angle(forwardDirection.forward, c.transform.position - forwardDirection.position) <= viewAngle)
            {
				myTarget = c.gameObject.transform;
				return true;
			}
		}
		return false;
	}
	
	//Need to access melee arc.
	public MeleeArc meleeRange;
	public bool InMeleeRange()
	{
		if(meleeRange.CanHitPlayer() )
		{
			//Debug.Log("In melee range");
			return true;
		}
		return false;
	}
	
	void OnDrawGizmosSelected() 
	{
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRange);
    }
}

public enum AttackState
{
	aggressive, defensive, standground
}
public enum AIBehavior
{
	idle, pursue, attack, die, flee
}
