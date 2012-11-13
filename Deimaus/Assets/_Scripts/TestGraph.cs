using UnityEngine;
using System.Collections;

public class TestGraph : MonoBehaviour 
{

	AstarPath graph;
	
	void Start () 
	{
		
		StartCoroutine(CoUpdate() );
		graph = GetComponent<AstarPath>();
	}
	
	IEnumerator CoUpdate()
	{
		yield return new WaitForSeconds(2f);
		while(true)
		{
			graph.Scan();
			yield return new WaitForSeconds(0.5f);
		}
	}
	
}
