using UnityEngine;
using System.Collections;

public class MoveTiles : MonoBehaviour 
{
	public bool doorIsLocked = false;
	public MapGenerator mapGen;
	public DoorLocation moveLocation;
	private Movement_Controller controller;
	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.GetComponent(typeof(Movement_Controller)) as Movement_Controller != null && mapGen.DoorsOpen)
		{
			controller = col.gameObject.GetComponent(typeof(Movement_Controller)) as Movement_Controller;
			DoorLocation pressed = controller.PressedDirection;
			
			
				if(doorIsLocked)
				{
					if(controller.myStats.SubKey() )
					{
						doorIsLocked = false;
					}
					else
						return;
				}
				switch(moveLocation)
				{
				case DoorLocation.Up:
					mapGen.MoveUp();
					break;
				case DoorLocation.Down:
					mapGen.MoveDown();
					break;
				case DoorLocation.Right:
					mapGen.MoveRight();
					break;
				case DoorLocation.Left:
					mapGen.MoveLeft();
					break;
				}
			
		}
	}

}

public enum DoorLocation
{
	Left, Right, Up, Down, None
}
			