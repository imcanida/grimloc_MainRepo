using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(_PoolingManager))]
public class PoolManagerEditor : Editor 
{
	bool creationOption;
	GameObject tempObj;
	int poolNum;
	
	public override void OnInspectorGUI()
	{
		_PoolingManager self = (_PoolingManager)target;	
		GUILayout.Space(20);
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Open Creation Options", GUILayout.Height(20)) )
		{
			creationOption = !creationOption;
		}
		GUILayout.EndHorizontal();
		
		GUILayout.Space(15f);
		DropAreaGUI();
		GUILayout.Space(15f);
		
		/*
		if(creationOption)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("", GUILayout.Width(50));
			GUILayout.Label("Add Options", EditorStyles.boldLabel);
			GUILayout.EndHorizontal();
			GUILayout.Space(5);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Name", GUILayout.Width(50));
			objStr = EditorGUILayout.TextField(objStr);
			GUILayout.EndHorizontal();
			GUILayout.Space(5);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Pool #", GUILayout.Width(50));
			poolNum = EditorGUILayout.IntField(poolNum);
			GUILayout.EndHorizontal();
			GUILayout.Space(5);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Object", GUILayout.Width(50));
			tempObj = (GameObject)EditorGUILayout.ObjectField(tempObj, typeof(GameObject), true);
			GUILayout.EndHorizontal();
			GUILayout.Space(5);
			GUILayout.BeginHorizontal();
			if(objStr.Equals(""))
				GUILayout.Label(errorFound, GUILayout.Width(100));
			GUILayout.EndHorizontal();
			if(GUILayout.Button("Add Item for Pooling", GUILayout.Height(20), GUILayout.Width(125)) && !objStr.Equals("") )
			{
				bool wasMade = false;
				for(int i = 0; i < self.myPool.Count; i++)
				{
					if(self.myPool[i].labelName.Equals(objStr))
					{
						errorFound = "Name was already found";
					}
					else if(i == self.myPool.Count-1)
						self.myPool.Add(new ObjectPool((i)+1, objStr, tempObj, poolNum) );
					wasMade = true;
				}
				if(!wasMade)
				{
					self.myPool.Add(new ObjectPool(0, objStr, tempObj, poolNum) );
				}
			}
			else 
				errorFound = "";
		}
		*/
		if(self.myPool.Count > 0)
		{
			for(int i = 0; i < self.myPool.Count; i++)
			{
				if(self.myPool[i].foldOutObject = EditorGUILayout.Foldout(self.myPool[i].foldOutObject, self.myPool[i].labelName ) )
				{
					//Create Stats
					GUILayout.BeginHorizontal();
					GUILayout.Label("", GUILayout.Width(200));
					GUILayout.Label("Pool Options", EditorStyles.boldLabel);
					GUILayout.EndHorizontal();
					GUILayout.Space(10);
					GUILayout.BeginHorizontal();
					GUILayout.Label("Parent For Objects:", GUILayout.Width(100));
					self.myPool[i].parent = (Transform)EditorGUILayout.ObjectField(self.myPool[i].parent, typeof(Transform), true);	
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					GUILayout.Label("Activation Name:", GUILayout.Width(100));
					self.myPool[i].labelName = EditorGUILayout.TextField(self.myPool[i].labelName);	
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					GUILayout.Label("Activation ID:", GUILayout.Width(100));
					EditorGUILayout.LabelField(""+self.myPool[i].objectID);	
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					GUILayout.Label("# of Objects:", GUILayout.Width(100));
					self.myPool[i].numberPooled = (int)EditorGUILayout.IntField((int)self.myPool[i].numberPooled );	
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					GUILayout.Label("Object:", GUILayout.Width(100));
					self.myPool[i].pooledObject = (GameObject)EditorGUILayout.ObjectField(self.myPool[i].pooledObject, typeof(GameObject), true);	
					GUILayout.EndHorizontal();
			
					GUILayout.Space(10);
					GUILayout.BeginHorizontal();
					if(GUILayout.Button("Remove Item") )
						self.myPool.RemoveAt(i);
					GUILayout.EndHorizontal();
					GUILayout.Space(20);
				}
			}
		}
	}
	
	/// <summary>
	/// Drop Area GUI.
	/// </summary>
	private void DropAreaGUI()
	{
		var evt = Event.current;
		var dropArea = GUILayoutUtility.GetRect(0f, 50f, GUILayout.ExpandWidth(true));
		GUI.Box(dropArea, "Drop a Prefab or GameObject here to add it to PoolManager");
		_PoolingManager self = (_PoolingManager)target;	
		
		switch (evt.type)
		{
			case EventType.DragUpdated:
			case EventType.DragPerform:
				if (!dropArea.Contains(evt.mousePosition))
					break;
				
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				
				if (evt.type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();
					foreach(var draggedObject in DragAndDrop.objectReferences)
					{
						var go = draggedObject as GameObject;
						if (!go)
							continue;
						
						var poolableObject = go.GetComponent<_PoolManagedItem>();
						if (!poolableObject)
						{
							go.AddComponent<_PoolManagedItem>();
						}
						self.myPool.Add(new ObjectPool(self.myPool.Count+1, go.name, go, 10, self.transform));
					}
				}
				
				Event.current.Use();
				break;
		}
	}
}
