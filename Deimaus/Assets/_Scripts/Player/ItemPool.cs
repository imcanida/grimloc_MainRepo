using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class ItemPool : TextureFunctionMonoBehaviour 
{
	//Have an Item Pool so the player never can get the same item twice
	public BoneAnimation player;
	
	void Start()
	{
		//player.(boneNames[i], textureSearchReplaceList[ locations[i] ]);
	}
}
