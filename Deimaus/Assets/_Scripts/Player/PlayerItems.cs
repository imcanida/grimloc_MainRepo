using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class PlayerItems : SmoothMoves.TextureFunctionMonoBehaviour 
{
	/**
	 * Hide Bone
	 * if (Input.GetKeyDown(KeyCode.H))
		{
			knight.HideBone("Weapon", !knight.IsBoneHidden("Weapon"));
		}
	 * 
	 */ 
	public List<ItemLocations> Items = new List<ItemLocations>();
	public BoneAnimation player;
	
	public void SwapItem(List<string> boneNames, List<int> locations)
	{
		for(int i = 0; i < boneNames.Count;i++)
		{
			player.ReplaceBoneTexture(boneNames[i], textureSearchReplaceList[ locations[i] ]);
			//Handle the item and location.
		}
	}
}

[System.Serializable]
public class ItemLocations
{
	public string locationName = "";
	public ItemType type;
	public bool isEquipped = false;
	public GameObject equippedObject = null;
	
}
