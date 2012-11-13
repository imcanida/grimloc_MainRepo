using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;
public class Item : TextureFunctionMonoBehaviour 
{
	//Have an Item Pool so the player never can get the same item twice
	public BoneAnimation myItem;
	public string myItemAnimationName = "FloatingGun";
	void Start()
	{
		myItem = GetComponent(typeof(BoneAnimation)) as BoneAnimation;
		StartCoroutine(PlayAnimation());
	}
	
	IEnumerator PlayAnimation()
	{
		yield return new WaitForSeconds(0.2f);
		myItem.Play(myItemAnimationName);
	}
}
