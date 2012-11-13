using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class _PoolingManager : MonoBehaviour 
{
	public List<ObjectPool> myPool = new List<ObjectPool>();
	public GameObject myObj;	
	public float startTime;
	public Transform parentObject;
	GameObject temp;
	private static _PoolingManager s_instance;
	public static _PoolingManager Instance
	{
		get { return s_instance; }
		private set { s_instance = value; }
	}
	
	void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}
	}
	
	void Start() 
	{
		StartCoroutine(DelayedDeactivation() );
	}
	
	IEnumerator DelayedDeactivation()
	{
		for(int i = 0; i < myPool.Count; i++)
		{
			if(myPool[i].pooledObject == null)
				yield return null;
			for(int j = 0; j < myPool[i].numberPooled; j++)
			{
				temp = (GameObject)Instantiate(myPool[i].pooledObject, Vector3.zero, Quaternion.identity);
				temp.name = ""+j;
				temp.transform.parent = myPool[i].parent;
				temp.transform.localPosition = new Vector3(0, 0, 0);
				temp.AddComponent<_PoolManagedItem>();
				myPool[i].unUsedObjects.Add(temp);
			}			
		}
		
		yield return new WaitForSeconds(0.5f);
		for(int i = 0; i < myPool.Count; i++)
		{
			if(myPool[i].pooledObject == null)
				yield return null;
			for(int j = 0; j < myPool[i].unUsedObjects.Count; j++)
			{
				myPool[i].unUsedObjects[j].SetActiveRecursively(false);
			}
		}
		
		Debug.Log("Finished loading Pool");
	}
	
	public GameObject ActivatePooledItem(int id)
	{
		GameObject found = null;
		for(int i = 0; i < myPool.Count; i++)
		{
			if(myPool[i].objectID == id)
			{
				if(myPool[i].unUsedObjects.Count > 0 && found == null)
				{
					found = myPool[i].unUsedObjects[0];
					found.SetActiveRecursively(true);
					found.GetComponent<_PoolManagedItem>().myName = "i_"+Time.realtimeSinceStartup;
					myPool[i].usedObjects.Add(found);
					myPool[i].unUsedObjects.RemoveAt(0);
				}
			}
		}
		return found;
	}
	
	public GameObject ActivatePooledItem(string activationName)
	{
		GameObject found = null;
		for(int i = 0; i < myPool.Count; i++)
		{
			if(myPool[i].labelName.Equals(activationName))
			{
				if(myPool[i].unUsedObjects.Count > 0 && found == null)
				{
					found = myPool[i].unUsedObjects[0];
					found.SetActiveRecursively(true);
					found.GetComponent<_PoolManagedItem>().myName = "IanCanida_"+Time.realtimeSinceStartup;
					myPool[i].usedObjects.Add(found);
					myPool[i].unUsedObjects.RemoveAt(0);
				}
			}
		}
		return found;
	}
	
	public GameObject GetPooledItemToActivate(string activationName)
	{
		GameObject found = null;
		for(int i = 0; i < myPool.Count; i++)
		{
			if(myPool[i].labelName.Equals(activationName))
			{
				if(myPool[i].unUsedObjects.Count > 0 && found == null)
				{
					found = myPool[i].unUsedObjects[0];
					found.GetComponent<_PoolManagedItem>().myName = "IanCanida_"+Time.realtimeSinceStartup;
					myPool[i].usedObjects.Add(found);
					myPool[i].unUsedObjects.RemoveAt(0);
				}
			}
		}
		return found;
	}
	
	public GameObject ActivatePooledItemWithCheck(string activationName, GameObject me)
	{
		GameObject found = null;
		
		for(int i = 0; i < myPool.Count; i++)
		{
			for(int j = 0; j < myPool[i].usedBy.Count; j++)
			{
				if(myPool[i].usedBy[j] == me)
					return null;
			}
		}
		
		for(int i = 0; i < myPool.Count; i++)
		{
			if(myPool[i].labelName.Equals(activationName))
			{
				if(myPool[i].unUsedObjects.Count > 0 && found == null)
				{
					found = myPool[i].unUsedObjects[0];
					found.SetActiveRecursively(true);
					found.GetComponent<_PoolManagedItem>().myName = "IanCanida_"+Time.realtimeSinceStartup;
					myPool[i].usedObjects.Add(found);
					myPool[i].unUsedObjects.RemoveAt(0);
					myPool[i].usedBy.Add(me);
				}
			}
		}
		return found;
	}
	
	public bool DeactivatePooledItem(int id, GameObject deactivate)
	{
		if(deactivate.GetComponent<_PoolManagedItem>() == null)
			return true;
		for(int i = 0; i < myPool.Count; i++)
		{
			if(myPool[i].objectID == id)
			{
				if(myPool[i].usedObjects.Count > 0)
				{
					for(int j = 0; j < myPool[i].usedObjects.Count; j++)
					{
						if(myPool[i].usedObjects[j].Equals(deactivate) )
						{
							deactivate.transform.parent = myPool[i].parent;
							deactivate.transform.position = Vector3.zero;
							deactivate.SetActiveRecursively(false);
							myPool[i].unUsedObjects.Add(deactivate);
							myPool[i].usedObjects.RemoveAt(j);
							return false;
						}
					}
				}
			}
		}
		return true;
	}
	
	public bool DeactivatePooledItem(string deactivationName, GameObject deactivate)
	{
		if(deactivate.GetComponent<_PoolManagedItem>() == null)
			return true;
		for(int i = 0; i < myPool.Count; i++)
		{
			if(myPool[i].labelName.Equals(deactivationName))
			{
				if(myPool[i].usedObjects.Count > 0)
				{
					for(int j = 0; j < myPool[i].usedObjects.Count; j++)
					{
						if(myPool[i].usedObjects[j].Equals(deactivate) )
						{
							deactivate.transform.parent = myPool[i].parent;
							deactivate.transform.position = Vector3.zero;
							deactivate.SetActiveRecursively(false);
							myPool[i].unUsedObjects.Add(deactivate);
							myPool[i].usedObjects.RemoveAt(j);
							return false;
						}
					}
				}
			}
		}
		return true;
	}
	
	public bool DeactivatePooledItem(string deactivationName, GameObject deactivate, GameObject usedBy)
	{
		if(deactivate.GetComponent<_PoolManagedItem>() == null)
			return true;
		
		for(int i = 0; i < myPool.Count; i++)
		{
			if(myPool[i].labelName.Equals(deactivationName))
			{
				if(myPool[i].usedObjects.Count > 0)
				{
					for(int j = 0; j < myPool[i].usedObjects.Count; j++)
					{
						if(myPool[i].usedObjects[j].Equals(deactivate) )
						{
							deactivate.transform.position = Vector3.zero;
							deactivate.SetActiveRecursively(false);
							myPool[i].unUsedObjects.Add(deactivate);
							myPool[i].usedObjects.RemoveAt(j);
							myPool[i].usedBy.RemoveAt(j);
							return false;
						}
					}
				}
			}
		}
		return true;
	}

}


[System.Serializable]
public class ObjectPool
{
	public ObjectPool(int objectID, string label, GameObject obj, int num, Transform parent)
	{
		this.objectID = objectID;
		this.labelName = label;
		this.pooledObject = obj;
		this.numberPooled = num;
		this.parent = parent;
	}
	public int objectID;
	public string labelName;
	public GameObject pooledObject;
	public Transform parent;
	public int numberPooled;
	public bool foldOutObject = true;
	public List<GameObject> unUsedObjects = new List<GameObject>();
	public List<GameObject> usedObjects = new List<GameObject>();
	
	public List<GameObject> usedBy = new List<GameObject>();
}
