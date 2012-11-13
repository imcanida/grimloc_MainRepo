using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PlayerItems))]
public class PlayerItemsEditor : SmoothMoves.TextureFunctionInspector {

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
	}
}
