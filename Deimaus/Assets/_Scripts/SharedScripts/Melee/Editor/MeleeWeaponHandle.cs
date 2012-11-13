using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MeleeArc))]
public class MeleeWeaponHandle : Editor 
{
	void OnSceneGUI () 
	{
		var tar = (MeleeArc)target;
		if(!tar.displayForwardDirectionSpread)
		{
			Handles.color = new Color(1,1,1,0.2f);
		    Handles.DrawSolidArc(tar.transform.position, 
		            tar.transform.up, 
		            tar.transform.forward, 
		            tar.spread/2, 
		            tar.range);
			
		    Handles.DrawSolidArc(tar.transform.position, 
		            tar.transform.up, 
		            tar.transform.forward, 
		            -tar.spread/2, 
		            tar.range);
		    Handles.color = Color.white;
			
		    tar.range = Handles.ScaleValueHandle(
					tar.range,
					tar.transform.position + tar.transform.forward*tar.range,
					tar.transform.rotation, 2, Handles.ConeCap, 1);
			
			 tar.spread = Handles.ScaleSlider(tar.spread,
	                       tar.transform.position,
	                        -Vector3.right, 
	                        Quaternion.identity, 
	                        2, 
	                        HandleUtility.GetHandleSize(tar.transform.position));
		}
		else
		{
			Handles.color = new Color(1,1,1,0.2f);
	        Handles.DrawSolidArc(tar.transform.position, 
	                tar.transform.up, 
	                tar.transform.forward, 
	                tar.forwardSpread/2, 
	                tar.range);
			
	        Handles.DrawSolidArc(tar.transform.position, 
	                tar.transform.up, 
	                tar.transform.forward, 
	                -tar.forwardSpread/2, 
	                tar.range);
	        Handles.color = Color.white;
			
	        tar.range = Handles.ScaleValueHandle(
					tar.range,
					tar.transform.position + tar.transform.forward*tar.range,
					tar.transform.rotation, 2, Handles.ConeCap, 1);
			 Handles.color = Color.red;
			
			 tar.forwardSpread = Handles.ScaleSlider(tar.forwardSpread,
	                       	tar.transform.position,
	                        Vector3.right, 
	                        Quaternion.identity, 
	                        2, 
	                        HandleUtility.GetHandleSize(tar.transform.position));
		}
        if (GUI.changed)
            EditorUtility.SetDirty (tar);
	}
}
