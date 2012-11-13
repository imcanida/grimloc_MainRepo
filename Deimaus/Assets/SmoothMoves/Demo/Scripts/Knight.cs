using UnityEngine;
using System.Collections;

public class Knight : SmoothMoves.TextureFunctionMonoBehaviour {
	
	private float _blinkTimeLeft;
	
	public SmoothMoves.BoneAnimation knight;
	public AudioSource swishSound;
	public AudioSource hitSound;
	
	public float minBlinkTime;
	public float maxBlinkTime;
	
	public Material oldMaterial;
	public Material newMaterial;
	
	public float speed;
	
	public Transform knightShadow;
	
	public SmoothMoves.BoneAnimation sparks;
	
	// Use this for initialization
	void Start () {
		
		// knight.SwapMaterial(oldMaterial, newMaterial); <-- This is how you change characters
		
		knight.RegisterColliderTriggerDelegate(SwordHit);
		knight.RegisterUserTriggerDelegate(SwordSwish);
		
		
		
		knightShadow.parent = knight.GetBoneTransform("Root");
		knightShadow.localPosition = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		
		//Enhanced Animation and Movement by Kyle LeMaster, Enigma Factory Games
        //If either movement key is released, crossfade to Stand animation
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            knight.CrossFade("Stand");
        }
       
        if (Input.GetKey(KeyCode.RightArrow))
        {
            //If the opposite direction is held, crossfade Stand and exit
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                knight.CrossFade("Stand");
                return;
            }
            //Check if Walk is playing, if it is not, play it
            if (!knight.IsPlaying("Walk"))
            {
                knight["Walk"].speed = 1.0f;
                knight.CrossFade("Walk");
            }
            //Move the player character
            knight.mLocalTransform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        }
 
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //If the opposite direction is held, crossfade Stand and exit
            if (Input.GetKey(KeyCode.RightArrow))
            {
                knight.CrossFade("Stand");
                return;
            }
            //Check if Walk is playing, if it is not, play it
            if (!knight.IsPlaying("Walk"))
            {
                knight["Walk"].speed = -1.0f;
                knight.CrossFade("Walk");
            }
            //Move the player character
            knight.mLocalTransform.position -= new Vector3(speed * Time.deltaTime, 0, 0);
        }
	
		// Attack
		if (Input.GetKeyDown(KeyCode.A))
		{
			knight.CrossFade("Attack");
		}

		// switch to the axe
		if (Input.GetKeyDown(KeyCode.X))
		{
			knight.ReplaceBoneTexture("Weapon", textureSearchReplaceList[0]);
			knight.ReplaceBoneTexture("Weapon", textureSearchReplaceList[1]);
		}
		
		// switch to the mace
		if (Input.GetKeyDown(KeyCode.M))
		{
			knight.ReplaceBoneTexture("Weapon", textureSearchReplaceList[2]);
			knight.ReplaceBoneTexture("Weapon", textureSearchReplaceList[3]);
		}

		// switch to the sword
		if (Input.GetKeyDown(KeyCode.S))
		{
			// the sword was the original weapon set up in the animation,
			// so we just restore to the original instead of replacing
			knight.RestoreBoneTexture("Weapon");
		}
		
		// Hide and Show the weapon
		if (Input.GetKeyDown(KeyCode.H))
		{
			knight.HideBone("Weapon", !knight.IsBoneHidden("Weapon"));
		}
		
		_blinkTimeLeft -= Time.deltaTime;
		if (_blinkTimeLeft <= 0)
		{
			knight.Play("Blink");
			
			_blinkTimeLeft = UnityEngine.Random.Range(minBlinkTime, maxBlinkTime);
		}
	}
	
	public void SwordHit(SmoothMoves.ColliderTriggerEvent triggerEvent)
	{
		if (triggerEvent.boneName == "Weapon" && triggerEvent.triggerType == SmoothMoves.ColliderTriggerEvent.TRIGGER_TYPE.Enter)
		{
			hitSound.Play();
			
			sparks.mLocalTransform.position = triggerEvent.otherColliderClosestPointToBone;
			sparks.Play("Hit");
		}
	}
	
	public void SwordSwish(SmoothMoves.UserTriggerEvent triggerEvent)
	{
		if (triggerEvent.boneName == "Weapon")
		{
			swishSound.Play();
		}
	}
}
