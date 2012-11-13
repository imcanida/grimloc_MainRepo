using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class DiamondSword : UseableItem 
{
	public string animationName = "SwordFall";
	public int damage = 10;
	private Vector3 currentLocation = Vector3.zero;
	public override void Activate ()
	{
		for(int i = 0; i < swordObj.Count; i++)
		{
			_PoolingManager.Instance.DeactivatePooledItem("SwordFall", swordObj[i]);
		}
		swordObj = new List<GameObject>();
		if(currentCharge == maxCharge)
		{
			int layerMask = 1 << 26 | 1 << 27;
			currentLocation = startLocation.localPosition;
			currentLocation += new Vector3(xLocationAddition, -5, -5);
			checkResult = Physics.OverlapSphere( currentLocation, 3, layerMask );
			if (checkResult.Length > 0) 
			{
				//Do not allow.
			}
			else
			{
				//Allow the useage of the item.
				Debug.Log("Activated");
				StartCoroutine(SwordSpawn() );
			}

		}
		else
		{
			return;
		}
	}
	
	public int xLocationAddition = 15;
	private bool hitObstical = false;
	private RaycastHit hit;
	private List<GameObject> swordObj = new List<GameObject>();
	private HitCheck check;
	private BoneAnimation anims;
	
	public Collider[] checkResult;
	IEnumerator SwordSpawn()
	{
		swordObj = new List<GameObject>();
		currentLocation = startLocation.localPosition;
		currentLocation += new Vector3(xLocationAddition, 0, -5);
		GameObject temp;
		int layerMask = 1 << 26 | 1 << 27;
		while(!hitObstical)
		{
			Vector3 testPos = new Vector3(currentLocation.x, currentLocation.y-6, currentLocation.z);
			checkResult = Physics.OverlapSphere( currentLocation, 3, layerMask );
			if (checkResult.Length > 0) 
			{
				hitObstical = true;
			}
			else
			{
				temp = _PoolingManager.Instance.ActivatePooledItem("SwordFall");
				temp.transform.position = currentLocation;
				temp.transform.eulerAngles = new Vector3(90, 0, 0);
				swordObj.Add(temp);
				anims = temp.GetComponent(typeof(BoneAnimation)) as BoneAnimation;
				anims.Play(animationName);
				yield return new WaitForSeconds(0.1f);
				currentLocation += new Vector3(xLocationAddition, 0, 0);
			}
			
		}
		
		yield return new WaitForSeconds(anims[animationName].length);
		for(int i = 0; i < swordObj.Count; i++)
		{
			_PoolingManager.Instance.DeactivatePooledItem("SwordFall", swordObj[i]);
		}
		hitObstical = false;
	}
}
