using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

[RequireComponent(typeof(ItemPickUpEvent))]
public class ItemPickUpTrigger : MonoBehaviour 
{
	Transform playerItemPickUp;
	BoneAnimation player;
	Movement_Controller control;
	public GameObject myItem;
	public PlayerItems pItem;
	public Stats playerStats;
	
	public ItemType type = ItemType.Gun;
	
	public List<int> mySwapNumber = new List<int>();	//The replace texture location in the array
	public List<string> boneName = new List<string>();	//The name of the bone that we need to replace
	
	private ItemPickUpEvent iEvent;
	public void OnEnable()
	{
		iEvent = GetComponent(typeof(ItemPickUpEvent)) as ItemPickUpEvent;
	}
	
	private Vector3 direction;
	public void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.GetComponent(typeof(Movement_Controller)) as Movement_Controller != null) 
		{
			playerItemPickUp = col.transform.FindChild("ItemPickUp").transform;
			control = col.gameObject.GetComponent(typeof(Movement_Controller)) as Movement_Controller;
			
			if(type == ItemType.Gun)
				control.usingGun = true;
			
			if(type == ItemType.Useable)
			{
				//The useable item script should be attached to this Object.
				this.gameObject.transform.parent = control.transform;
				control.SpacebarItem = GetComponent(typeof(UseableItem)) as UseableItem;
				control.SpacebarItem.controller = control;
			}
			playerStats = col.gameObject.GetComponent(typeof(Stats)) as Stats;
			
			if(playerStats.GetHealth() == playerStats.Health && type == ItemType.HealthPotion)
			{
				direction = control.preMovement;
				rigidbody.AddForce(direction * 1000f);
				return;
			}
			player = control.player;	//Bone animation reference
			pItem = col.gameObject.GetComponent(typeof(PlayerItems)) as PlayerItems;
			iEvent.ItemEvent(playerStats);
			myItem.SetActiveRecursively(false);
			StartCoroutine(GrabItem() );
		}
	}
	
	public bool isOnStand;
	public IEnumerator GrabItem()
	{
		myItem.transform.parent = playerItemPickUp;
		myItem.transform.localPosition = new Vector3(0,0,0);
		if(!isOnStand)
			this.gameObject.SetActiveRecursively(false);
		if(type != ItemType.HealthPotion && type != ItemType.Consumable)
		{
			myItem.SetActiveRecursively(true);
			control.GrabbingItem = true;
			player.Play("ItemGrab");
			yield return new WaitForSeconds(player["ItemGrab"].length);
			
			//Wait to Disable Head until here because he will have no head other wise..
			if(type == ItemType.Helmet)
					control.DisableHead();
			control.GrabbingItem = false;
			myItem.SetActiveRecursively(false);
			control.lastKeyPress = KeyCode.DownArrow;	//Set the press for down so the animation doesn't get stuck
			this.collider.enabled = false;
			pItem.SwapItem(boneName, mySwapNumber);	//Call the replace
		}
		yield return null;
	}
}

public enum ItemType
{
	Helmet, Body, Head, 
	Melee, Throwing, Gun, Consumable, HealthPotion, Useable
}
