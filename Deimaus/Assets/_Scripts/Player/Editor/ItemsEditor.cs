using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Item))]
public class ItemsEditor : SmoothMoves.TextureFunctionInspector {

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
	}
}
