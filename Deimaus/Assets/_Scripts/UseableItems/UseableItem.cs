using UnityEngine;
using System.Collections;

public class UseableItem : MonoBehaviour 
{
	public Transform startLocation;
	public int maxCharge = 8;
	protected int currentCharge = 8;
	public Movement_Controller controller;
	public int CurrentCharge
	{
		get{return currentCharge;}
		set{currentCharge = value;}
	}
	
	public virtual void Activate()
	{
		if(currentCharge == maxCharge)
		{
			//Allow the useage of the item.
		}
		else
		{
			return;
		}
	}
}
