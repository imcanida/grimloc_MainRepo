using UnityEngine;
using System.Collections;

public class ItemColliderWall : MonoBehaviour 
{

	void Start () 
	{
		StartCoroutine(CoUpdate() );
	}
	
	
	IEnumerator CoUpdate()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.layer == 27 || col.gameObject.layer == 26)
		{
			gameObject.rigidbody.velocity = -1*(gameObject.rigidbody.velocity);
		}
	}
}
