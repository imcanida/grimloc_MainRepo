using UnityEngine;
using System.Collections;
using SmoothMoves;

public class TestAnimation : MonoBehaviour 
{
	public bool playAnimation = false;
	public string animationName = "";
	public BoneAnimation animation;
	// Update is called once per frame
	void Update () 
	{
		if(playAnimation)
		{
			animation.Play(animationName);
		}
	}
}
