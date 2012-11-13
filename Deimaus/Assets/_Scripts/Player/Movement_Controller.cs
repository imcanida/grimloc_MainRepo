using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;
[RequireComponent(typeof(CharacterController))]

/**
 * Clean This Script up-
 * 
 * Currently Handling Movement with DPad as well as Input keys.
 * 
 * Takes in Attack and Block Commands. Sends Attack to MeleeArc Script.
 * 
 * Handling Animations -> idle, walk, run, attack, block
 * Handling Sound -> Attack Swoosh
 * */
public class Movement_Controller : MonoBehaviour 
{
	public float movementCheck = 1f;		//Needed to get what buttons are being pressed.
	public Vector3 preMovement;
	public float movementSpeedModifier = 20f;
    public Vector3 moveDirection = Vector3.zero;
	CharacterController controller;
	
	public bool usingGun = false;
	public bool HoldingGun 
	{
		get{return usingGun;}
		set{usingGun = value;}
	}
	
	private bool grabbedItem = false;
	public bool GrabbingItem 
	{
		get{return grabbedItem;}
		set{grabbedItem = value;}
	}
	
	void Start()
	{
		StartCoroutine(CoUpdate());
	}
		
	public BoneAnimation player;
	public Transform faceDirection;
	public PlayerItems myItems;
	public Transform myTrans;
	public Stats myStats;
	public KeyCode lastKeyPress = KeyCode.UpArrow;
	public DoorLocation PressedDirection
	{
		get{return pressedDirection;}
		set{pressedDirection = value;}
	}
	private DoorLocation pressedDirection;
	public float baseYPosition = 0;
	
	private UseableItem currentUseableItem;
	public UseableItem SpacebarItem
	{
		get{return currentUseableItem;}
		set{currentUseableItem = value;}
	}
	public bool pauseMovement = false;
    IEnumerator CoUpdate() 
	{
		myTrans = this.transform;
		//baseYPosition = myTrans.localPosition.y;
		myItems = this.GetComponent<PlayerItems>();
		controller = this.GetComponent<CharacterController>();
		Physics.IgnoreLayerCollision( 30, 31 );
		while(true)
		{
			if(grabbedItem || pauseMovement)
			{
				//Do nothing.
			}
			else
			{
				//Useable Item
				if(Input.GetKeyDown(KeyCode.Space))
				{
					if(currentUseableItem != null)
					{
						currentUseableItem.startLocation = myTrans;
						currentUseableItem.Activate();
					}
				}
				
				//Movement
				pressedDirection = DoorLocation.None;
				moveDirection = Vector3.zero;
				if(Input.GetKey(KeyCode.D))
				{
					moveDirection += new Vector3(movementCheck, 0, 0);
					pressedDirection = DoorLocation.Right;
				}
				
				if(Input.GetKey(KeyCode.A))
				{
					moveDirection += new Vector3(-movementCheck, 0, 0);
					pressedDirection = DoorLocation.Left;
				}
				
				if(Input.GetKey(KeyCode.S))
				{
					moveDirection += new Vector3(0, 0, -movementCheck);
					pressedDirection = DoorLocation.Down;
				}
				
				if(Input.GetKey(KeyCode.W))
				{
					moveDirection += new Vector3(0, 0, movementCheck);
					pressedDirection = DoorLocation.Up;
				}
				
				if(moveDirection == Vector3.zero)
					pressedDirection = DoorLocation.None;
				
				preMovement = moveDirection;
				
	            moveDirection = transform.TransformDirection(moveDirection);
				float tempMovementMod = 0;
				if(myStats.MovementSpeed < 5)
					tempMovementMod = movementSpeedModifier/1.3f;
				else
					tempMovementMod = movementSpeedModifier/1.5f;
				
	            moveDirection *= myStats.MovementSpeed*movementSpeedModifier;
	
			
				if(canAttack)
				{
					//Handling Movement Animations
					if(Input.GetKey(KeyCode.UpArrow))
					{
						lastKeyPress = KeyCode.UpArrow;
						Shoot(Vector3.forward);
					}
					else if(Input.GetKey(KeyCode.DownArrow))
					{
						lastKeyPress = KeyCode.DownArrow;
						Shoot(Vector3.back);
					}
					else if(Input.GetKey(KeyCode.LeftArrow))
					{
						lastKeyPress = KeyCode.LeftArrow;
						Shoot(Vector3.left);
					}
					else if(Input.GetKey(KeyCode.RightArrow))
					{
						lastKeyPress = KeyCode.RightArrow;
						Shoot(Vector3.right);
					}
					
				switch(lastKeyPress)
					{
					case KeyCode.UpArrow:
						if(moveDirection.magnitude > 0.1f)
							player.Play("BackWalking");
						else
							player.Play("BackIdle");
						break;
					case KeyCode.DownArrow:
						if(moveDirection.magnitude > 0.1f)
							player.Play("ForwardWalking");
						else
							player.Play("ForwardIdle");
					break;
					case KeyCode.LeftArrow:
						if(moveDirection.magnitude > 0.1f)
						{
							if(usingGun)
								player.Play("SideGunWalking");
							else
								player.Play("SideWalking");
						}
						else
						{
							if(usingGun)
								player.Play("SideGunIdle");
							else
								player.Play("SideIdle");
						}
						faceDirection.localRotation = Quaternion.Euler(0, -90, -90);
					break;
					case KeyCode.RightArrow:
						if(moveDirection.magnitude > 0.1f)
						{
							if(usingGun)
								player.Play("SideGunWalking");
							else
								player.Play("SideWalking");
						}
						else
						{
							if(usingGun)
								player.Play("SideGunIdle");
							else
								player.Play("SideIdle");
						}
						faceDirection.localRotation = Quaternion.Euler(0, 90, 90);
					break;
					}
				}
				myTrans.localPosition = new Vector3(myTrans.localPosition.x, baseYPosition, myTrans.localPosition.z);
				myTrans.localPosition += new Vector3(0, myTrans.localPosition.z*zOffset, 0);
				
				controller.height = 32;
				controller.height += myTrans.localPosition.z*zOffset;
				
	       		controller.Move(moveDirection * Time.deltaTime);
			}
			yield return new WaitForSeconds(0.01f);
		}
    }
	
	public float zOffset = -0.15f;
	private float gravity = 1850;
	private float attackSpeedMod = 0.45f;
	float positionShift = 2f;
	Vector3 temp = new Vector3();
	public void Shoot(Vector3 direction)
	{
		float calcModAttackSpeed = (myStats.AttackSpeed)*attackSpeedMod;
		
		if(player.IsPlaying("SideSwing")  || player.IsPlaying("SideGunShoot") ) //|| player.IsPlaying("") )
			return;
		
			positionShift *= -1;	//Switch it from pos to neg and vice versa.
		
		if(direction.x <= -1) //Left
		{
			direction = new Vector3(-1, 0, 0);
			if(HoldingGun)
			{
				showProjectileBeforeShot = false;
				if(player.IsBoneHidden("Weapon_Side"))
					player.HideBone("Weapon_Side", !player.IsBoneHidden("Weapon_Side"));
				temp = sideFireLocation.position;
				temp = new Vector3(temp.x, temp.y, temp.z+positionShift);
				StartCoroutine(ThrowProjectile(direction, "SideGunShoot", temp, gunShotProjectile) );
				player["SideGunShoot"].wrapMode = WrapMode.Once;
				player["SideGunShoot"].speed = calcModAttackSpeed;
				player.Play("SideGunShoot");
			}
			else // Use Throwing
			{
				showProjectileBeforeShot = true;
				if(!player.IsBoneHidden("Weapon_Side"))
					player.HideBone("Weapon_Side", !player.IsBoneHidden("Weapon_Side"));
				temp = sideThrowLocationNeg.position;
				temp = new Vector3(temp.x, temp.y, temp.z+positionShift);
				StartCoroutine(ThrowProjectile(direction, "SideSwing", temp, thrownProjectile) );
				player["SideSwing"].wrapMode = WrapMode.Once;
				player["SideSwing"].speed = calcModAttackSpeed;
				player.Play("SideSwing");
			}

		}
		else if(direction.x >= 1) //Right
		{
			direction = new Vector3(1, 0, 0);
			if(HoldingGun)
			{
				showProjectileBeforeShot = false;
				if(player.IsBoneHidden("Weapon_Side"))
					player.HideBone("Weapon_Side", !player.IsBoneHidden("Weapon_Side"));
				temp = sideFireLocation.position;
				temp = new Vector3(temp.x, temp.y, temp.z+positionShift);
				StartCoroutine(ThrowProjectile(direction, "SideGunShoot", temp,gunShotProjectile ) );
				player["SideGunShoot"].wrapMode = WrapMode.Once;
				player["SideGunShoot"].speed = calcModAttackSpeed;
				player.Play("SideGunShoot");
			}
			else // Use Throwing
			{
				showProjectileBeforeShot = true;
				if(!player.IsBoneHidden("Weapon_Side"))
					player.HideBone("Weapon_Side", !player.IsBoneHidden("Weapon_Side"));
				temp = sideThrowLocation.position;
				temp = new Vector3(temp.x, temp.y, temp.z+positionShift);
				StartCoroutine(ThrowProjectile(direction, "SideSwing", temp, thrownProjectile) );
				player["SideSwing"].wrapMode = WrapMode.Once;
				player["SideSwing"].speed = calcModAttackSpeed;
				player.Play("SideSwing");
			}
		}
		else if(direction.z <= -1) //Back
		{
			//player["BackShooting"].wrapMode = WrapMode.Once;
			//player.CrossFade("BackShooting");
			showProjectileBeforeShot = true;
			direction = new Vector3(0, 0, -1);
			temp = backFireLocation.position;
			temp = new Vector3(temp.x+positionShift, temp.y, temp.z);
			StartCoroutine(ThrowProjectile(direction, "SideSwing", temp, thrownProjectile) );
		}
		else if(direction.z >= 1) //Forward
		{
			//player["FrontShooting"].wrapMode = WrapMode.Once;
			//player.CrossFade("FrontShooting");
			showProjectileBeforeShot = true;
			direction = new Vector3(0, 0, 1);
			temp = frontFireLocation.position;
			temp = new Vector3(temp.x+positionShift, temp.y, temp.z);
			StartCoroutine(ThrowProjectile(direction, "SideSwing", temp, thrownProjectile) );
		}
	}
	
	public bool showProjectileBeforeShot = true;
	public string gunShotProjectile = "GreenOrbProjectile";
	public string thrownProjectile = "KnifeProjectile";
	private int projectsPerShot = 1;
	
	public Transform backFireLocation;
	public Transform frontFireLocation;
	public Transform sideFireLocation;
	public Transform sideThrowLocation;
	public Transform sideThrowLocationNeg;
	
	bool canAttack = true;
	IEnumerator ThrowProjectile(Vector3 direction, string animationName, Vector3 location, string projectileActivationName)
	{
		canAttack = false;
		float calcModAttackSpeed = (myStats.AttackSpeed)*attackSpeedMod;
		
		Vector3 addedForce;
		addedForce = MovingInDirectionOfFire(direction);
		GameObject myProjectile = null;
	
		yield return new WaitForSeconds( (player[animationName].length/calcModAttackSpeed)/ 2);

		myProjectile = CreateProjectile(direction, location, projectileActivationName);

		//Rigidbody projectileRig = myProjectile.GetComponentInChildren(typeof(Rigidbody)) as Rigidbody;
		myProjectile.rigidbody.AddForce(addedForce);
		yield return new WaitForSeconds( (player[animationName].length/calcModAttackSpeed)/ 2);
		canAttack = true;
	}
	
	public GameObject CreateProjectile(Vector3 direction, Vector3 location, string projectileActivationName)
	{
		GameObject myProjectile = _PoolingManager.Instance.ActivatePooledItem(projectileActivationName);
		Projectile projectileStats = myProjectile.GetComponentInChildren(typeof(Projectile)) as Projectile;
		
		projectileStats.EnableProjectile(this.collider, true, myTrans.localPosition, direction, 
			projectileActivationName, myStats);
		
		myProjectile.transform.position = new Vector3(location.x, location.y+5, location.z);
		myProjectile.SetActiveRecursively(true);
		return myProjectile;
	}
	
	public Vector3 MovingInDirectionOfFire(Vector3 directionShot)
	{
		/*
		if(moveDirection.x <= -1) //Left
		{
			if(directionShot.x >= -1)	//Dont effect it if were moving the opposite direction of the shot..
				directionShot = new Vector3(directionShot.x-0.5f, directionShot.y, directionShot.z);
		}
		else if(moveDirection.x >= 1) //Right
		{
			if(directionShot.x <= 1)
				directionShot = new Vector3(directionShot.x+0.5f, directionShot.y, directionShot.z);
		}
		else if(moveDirection.z <= -1) //Back
		{
			if(directionShot.z >= -1)
				directionShot = new Vector3(directionShot.x, directionShot.y, directionShot.z-0.5f);
		}
		else if(moveDirection.z >= 1) //Forward
		{
			if(directionShot.z <= 1)
				directionShot = new Vector3(directionShot.x, directionShot.y, directionShot.z+0.5f);
		}
		*/
		return (directionShot* (myStats.ProjectileSpeed+myStats.MovementSpeed) * 1000);	//Add in the movedirection
	}
	
	public void DisableHead()
	{
		if(!player.IsBoneHidden("Head"))
			player.HideBone("Head", !player.IsBoneHidden("Head"));
	}
}
