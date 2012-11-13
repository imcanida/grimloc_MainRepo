using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Knight))]
public class KnightInspector : SmoothMoves.TextureFunctionInspector {

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
	}
}
