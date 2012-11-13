using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/** Simple GUIUtility functions */
public class GUIUtilityx {
	
	public static Color prevCol;
	public static void SetColor (Color col) {
		prevCol = GUI.color;
		GUI.color = col;
	}
	
	public static void ResetColor () {
		GUI.color = prevCol;
	}
}

/** Handles fading effects and also some custom GUI functions such as LayerMaskField */
public class EditorGUILayoutx {

	Rect fadeAreaRect;
	Rect lastAreaRect;
	
	//public static List<Rect> currentRects;
	//public static List<Rect> lastRects;
	//public static List<float> values;
	public Dictionary<string, FadeArea> fadeAreas;
	//public static Dictionary<string, Rect> lastRects;
	//public static Dictionary<string, float> values;
	
	
	//public static List<bool> open;
	
	public static int currentDepth = 0;
	public static int currentIndex = 0;
	public static bool isLayout = false;
	
	public static Editor editor;
	
	public static GUIStyle defaultAreaStyle;
	public static GUIStyle defaultLabelStyle;
	public static GUIStyle stretchStyle;
	public static GUIStyle stretchStyleThin;
	
	public static float speed = 6;
	public static bool fade = true;
	public static bool fancyEffects = true;
	
	public Stack<FadeArea> stack;
	
	public void RemoveID (string id) {
		if (fadeAreas == null) {
			return;
		}
		
		fadeAreas.Remove (id);
	}
	
	public bool DrawID (string id) {
		if (fadeAreas == null) {
			return false;
		}
		
		//Debug.Log ("Draw "+id+" "+fadeAreas[id].value.ToString ("0.00"));
		return fadeAreas[id].Show ();
		/*if (values == null) {
			return false;
		}
		
		return values[id] > 0.002F;*/
	}
	
	public FadeArea BeginFadeArea (bool open,string label, string id) {
		return BeginFadeArea (open,label,id, defaultAreaStyle);
	}
	
	public FadeArea BeginFadeArea (bool open,string label, string id, GUIStyle areaStyle) {
		return BeginFadeArea (open, label, id, areaStyle, defaultLabelStyle);
	}

	public FadeArea BeginFadeArea (bool open,string label, string id, GUIStyle areaStyle, GUIStyle labelStyle) {
		
		//Rect r = EditorGUILayout.BeginVertical (areaStyle);
		
		Color tmp1 = GUI.color;
		
		FadeArea fadeArea = BeginFadeArea (open,id, 20,areaStyle);
		//GUI.Box (r,"",areaStyle);
		
		Color tmp2 = GUI.color;
		GUI.color = tmp1;
		
		if (label != "") {
			if (GUILayout.Button (label,labelStyle)) {
				fadeArea.open = !fadeArea.open;
				editor.Repaint ();
			}
		}
		
		GUI.color = tmp2;
		
		//EditorGUILayout.EndVertical ();
		return fadeArea;
	}
	
	public class FadeArea {
		public Rect currentRect;
		public Rect lastRect;
		public float value;
		public float lastUpdate;
		public bool open;
		public Color preFadeColor;
		
		public void Switch () {
			lastRect = currentRect;
		}
		
		public FadeArea (bool open) {
			value = open ? 1 : 0;
		}
		
		public bool Show () {
			return open || value > 0F;
		}
		
		public static implicit operator bool (FadeArea o) {
			return o.open;
		}
	}
	public FadeArea BeginFadeArea (bool open, string id) {
		return BeginFadeArea (open,id,0);
	}
	
	//openMultiple is set to false if only 1 BeginVertical call needs to be made in the BeginFadeArea (open, id) function.
	//The EndFadeArea function always closes two BeginVerticals
	public FadeArea BeginFadeArea (bool open, string id, float minHeight) {
		return BeginFadeArea (open, id, minHeight, GUIStyle.none);
	}
	
	public FadeArea BeginFadeArea (bool open, string id, float minHeight, GUIStyle areaStyle) {
		
		if (editor == null) {
			Debug.LogError ("You need to set the 'EditorGUIx.editor' variable before calling this function");
			return null;
		}
		
		if (stretchStyle == null) {
			
			stretchStyle = new GUIStyle ();
			stretchStyle.stretchWidth = true;
			//stretchStyle.padding = new RectOffset (0,0,4,14);
			//stretchStyle.margin = new RectOffset (0,0,4,4);
		}
		
		if (stack == null) {
			stack = new Stack<FadeArea>();
		}
		
		if (fadeAreas == null) {
			fadeAreas = new Dictionary<string, FadeArea> ();
		}
		
		if (!fadeAreas.ContainsKey (id)) {
			fadeAreas.Add (id,new FadeArea (open));
		}
		
		FadeArea fadeArea = fadeAreas[id];
		
		stack.Push (fadeArea);
		
		fadeArea.open = open;
		
		//Make sure the area fills the full width
		areaStyle.stretchWidth = true;
		
		Rect lastRect = fadeArea.lastRect;
		
		if (!fancyEffects) {
			fadeArea.value = open ? 1F : 0F;
			lastRect.height -= minHeight;
			lastRect.height = open ? lastRect.height : 0;
			lastRect.height += minHeight;
		} else {
		
			//GUILayout.Label (lastRect.ToString ()+"\n"+fadeArea.tmp.ToString (),EditorStyles.miniLabel);
			lastRect.height = lastRect.height < minHeight ? minHeight : lastRect.height;
			lastRect.height -= minHeight;
			float faded = Hermite (0F,1F,fadeArea.value);
			lastRect.height *= faded;
			lastRect.height += minHeight;
			lastRect.height = Mathf.Round (lastRect.height);
			//lastRect.height *= 2;
			//if (Event.current.type == EventType.Repaint) {
			//	isLayout = false;
			//}
		}
		
		//Rect stretchWidthRect = GUILayoutUtility.GetRect (new GUIContent (), stretchStyle,GUILayout.Height (0));
		
		
		Rect gotLastRect = GUILayoutUtility.GetRect (new GUIContent (),areaStyle,GUILayout.Height (lastRect.height));
		
		//Debug.Log (Event.current.type +" "+lastRect);
		
		//The clipping area, also drawing background
		GUILayout.BeginArea (lastRect,areaStyle);
		
		Rect newRect = EditorGUILayout.BeginVertical ();
		
		if (Event.current.type == EventType.Repaint || Event.current.type == EventType.ScrollWheel) {
			newRect.x = gotLastRect.x;
			newRect.y = gotLastRect.y;
			newRect.width = gotLastRect.width;//stretchWidthRect.width;
			newRect.height += areaStyle.padding.top+ areaStyle.padding.bottom;
			fadeArea.currentRect = newRect;
			
			if (fadeArea.lastRect != newRect) {
				//@Fix - duplicate
				//fadeArea.lastUpdate = Time.realtimeSinceStartup;
				editor.Repaint ();
			}
			
			fadeArea.Switch ();
		}
		if (Event.current.type == EventType.Repaint) {
			float value = fadeArea.value;
			float targetValue = open ? 1F : 0F;
			
			float newRectHeight = fadeArea.lastRect.height;
			float deltaHeight = 400F / newRectHeight;
			
			float deltaTime = Mathf.Clamp (Time.realtimeSinceStartup-fadeAreas[id].lastUpdate,0.00001F,0.05F);
			
			deltaTime *= Mathf.Lerp (deltaHeight*deltaHeight*0.01F, 0.8F, 0.9F);
			
			fadeAreas[id].lastUpdate = Time.realtimeSinceStartup;
			
			//Useless, but fun feature
			if (Event.current.shift) {
				deltaTime *= 0.05F;
			}
			
			if (Mathf.Abs(targetValue-value) > 0.001F) {
				
				float time = Mathf.Clamp01 (deltaTime*speed);
				value += time*Mathf.Sign (targetValue-value);
				editor.Repaint ();
			} else {
				value = Mathf.Round (value);
			}
			
			fadeArea.value = Mathf.Clamp01 (value);
			
			//if (oldValue != value) {
			//	editor.Repaint ();
			//}
		}
		
		if (fade) {
			Color c = GUI.color;
			fadeArea.preFadeColor = c;
			c.a *= fadeArea.value;
			GUI.color = c;
		}
		
		fadeArea.open = open;
		
		//GUILayout.Label ("Alpha : "+fadeArea.value);
		//GUILayout.Label ("Alpha : "+fadeArea.value);GUILayout.Label ("Alpha : "+fadeArea.value);GUILayout.Label ("Alpha : "+fadeArea.value);GUILayout.Label ("Alpha : "+fadeArea.value);
		/*GUILayout.Label ("Testing");
		GUILayout.Label ("Testing");
			GUILayout.Label ("Testing");
				GUILayout.Label ("Testing");*/
				
			
		return fadeArea;
	}
	
	public void EndFadeArea () {
		
		if (stack.Count <= 0) {
			Debug.LogError ("You are popping more Fade Areas than you are pushing, make sure they are balanced");
			return;
		}
		
		FadeArea fadeArea = stack.Pop ();
		//Debug.Log (r);
		//fadeArea.tmp = r;
		
		//r.width = 10;
		//r.height = 10;
		//GUI.Box (r,"");
		//GUILayout.Label ("HEllo : ");
		EditorGUILayout.EndVertical ();
		GUILayout.EndArea ();
		
		if (fade) {
			GUI.color = fadeArea.preFadeColor;
		}
		//GUILayout.Label ("Outside");
		/*currentDepth--;
		
		Rect r = GUILayoutUtility.GetRect (new GUIContent (),stretchStyle,GUILayout.Height (0));
		
		if (Event.current.type == EventType.Repaint || Event.current.type == EventType.ScrollWheel) {
			Rect currentRect = currentRects[id];
			Rect newRect = new Rect (currentRect.x,currentRect.y,currentRect.width,r.y-minHeight);
			
			currentRects[id] = newRect;
			
			if (lastRects[id] != newRect) {
				changedDelta = true;
				lastUpdate = Time.realtimeSinceStartup;
				editor.Repaint ();
			}
			
		}
		
		GUILayout.EndArea ();*/
		
	}
	
	/*public static bool BeginFadeAreaSimple (bool open, string id) {
		
		if (editor == null) {
			Debug.LogError ("You need to set the 'EditorGUIx.editor' variable before calling this function");
			return open;
		}
		
		if (stretchStyleThin == null) {
			
			stretchStyleThin = new GUIStyle ();
			stretchStyleThin.stretchWidth = true;
			//stretchStyle.padding = new RectOffset (0,0,4,14);
			//stretchStyleThin.margin = new RectOffset (0,0,4,4);
		}
		
		if (Event.current.type == EventType.Layout && !isLayout) {
			if (currentRects == null) {
				currentRects = new Dictionary<string, Rect> ();//new List<Rect>();
				lastRects = new Dictionary<string, Rect> ();//new List<Rect>();
				values = new Dictionary<string, float> ();//new List<float>();
				//open = new List<bool>();
			}
			
			if (changedDelta) {
				deltaTime = Mathf.Min (Time.realtimeSinceStartup-lastUpdate,0.1F);
			} else {
				deltaTime = 0.01F;
			}
			changedDelta = false;
			
			isLayout = true;
			currentDepth = 0;
			currentIndex = 0;
			
			Dictionary<string, Rect> tmp = lastRects;
			lastRects = currentRects;
			
			currentRects = tmp;
			currentRects.Clear ();
			
		}
		
		if (Event.current.type == EventType.Layout) {
			if (!currentRects.ContainsKey (id)) {
				
				currentRects.Add (id,new Rect ());
				
			}
		}
		
		if (!lastRects.ContainsKey (id)) {
			lastRects.Add (id,new Rect ());
		}
		if (!values.ContainsKey (id)) {
			values.Add (id, open ? 1.0F : 0.0F);	
			//open.Add (false);
		}
				
		Rect newRect = GUILayoutUtility.GetRect (new GUIContent (),stretchStyleThin,GUILayout.Height (0));
		
		Rect lastRect = lastRects[id];
		
		lastRect.height *= Hermite (0F,1F,values[id]);
		
		GUILayoutUtility.GetRect (lastRect.width,lastRect.height);
		GUI.depth+= 10;
		GUILayout.BeginArea (lastRect);
		
		if (Event.current.type == EventType.Repaint) {
			isLayout = false;
			currentRects[id] = newRect;
		}
			
		if (Event.current.type == EventType.Layout) {
			float value = values[id];
			float oldValue = value;
			float targetValue = open ? 1F : 0;
			
			if (Mathf.Abs(targetValue-value) > 0.001F) {
				
				float time = Mathf.Clamp01 (deltaTime*speed);
				value += time*Mathf.Sign (targetValue-value);
			}
			
			values[id] = Mathf.Clamp01 (value);
			
			if (oldValue != value) {
				changedDelta = true;
				lastUpdate = Time.realtimeSinceStartup;
				editor.Repaint ();
			}
		}
		
		return open;
	}*/
	
	/*public static void EndFadeAreaSimple (string id) {
		
		if (editor == null) {
			Debug.LogError ("You need to set the 'EditorGUIx.editor' variable before calling this function");
			return;
		}
		
		Rect r = GUILayoutUtility.GetRect (new GUIContent (),stretchStyleThin,GUILayout.Height (0));
		
		if (Event.current.type == EventType.Repaint || Event.current.type == EventType.ScrollWheel) {
			Rect currentRect = currentRects[id];
			Rect newRect = new Rect (currentRect.x,currentRect.y,currentRect.width,r.y);
			
			currentRects[id] = newRect;
			
			if (lastRects[id] != newRect) {
				changedDelta = true;
				lastUpdate = Time.realtimeSinceStartup;
				editor.Repaint ();
			}
			
		}
		
		GUILayout.EndArea ();
		
	}*/
	
	public static int UpDownArrows (GUIContent label, int value, GUIStyle labelStyle, GUIStyle upArrow, GUIStyle downArrow) {
		
		GUILayout.BeginHorizontal ();
		GUILayout.Space (EditorGUI.indentLevel*10);
		GUILayout.Label (label,labelStyle,GUILayout.Width (170));
		
		if (downArrow == null || upArrow == null) {
			upArrow = GUI.skin.FindStyle ("Button");//EditorStyles.miniButton;//
			downArrow = upArrow;//GUI.skin.FindStyle ("Button");
		}
		
		//GUILayout.BeginHorizontal ();
		//GUILayout.FlexibleSpace ();
		if (GUILayout.Button ("",upArrow,GUILayout.Width (16),GUILayout.Height (12))) {
			value++;
		}
		if (GUILayout.Button ("",downArrow,GUILayout.Width (16),GUILayout.Height (12))) {
			value--;
		}
		//GUILayout.EndHorizontal ();
		GUILayout.Space (100);
		GUILayout.EndHorizontal ();
		return value;
	}
	
	public static LayerMask LayerMaskField (string label, LayerMask selected) {
		return LayerMaskField (label,selected,true);
	}
	
	public static List<string> layers;
	public static List<int> layerNumbers;
	
	/** Displays a LayerMask field.
	 * \param showSpecial Use the Nothing and Everything selections
	 */
	public static LayerMask LayerMaskField (string label, LayerMask selected, bool showSpecial) {
		
		if (layers == null) {
			layers = new List<string>();
			layerNumbers = new List<int>();
		} else {
			layers.Clear ();
			layerNumbers.Clear ();
		}
		
		string selectedLayers = "";
		
		for (int i=0;i<32;i++) {
			
			string layerName = LayerMask.LayerToName (i);
			
			if (layerName != "") {
				if (selected == (selected | (1 << i))) {
					
					if (selectedLayers == "") {
						selectedLayers = layerName;
					} else {
						selectedLayers = "Mixed";
					}
				}
			}
		}
		
		if (Event.current.type != EventType.MouseDown && Event.current.type != EventType.ExecuteCommand) {
			if (selected.value == 0) {
				layers.Add ("Nothing");
			} else if (selected.value == -1) {
				layers.Add ("Everything");
			} else {
				layers.Add (selectedLayers);
			}
			layerNumbers.Add (-1);
		}
		
		if (showSpecial) {
			layers.Add ((selected.value == 0 ? "[X] " : "     ") + "Nothing");
			layerNumbers.Add (-2);
			
			layers.Add ((selected.value == -1 ? "[X] " : "     ") + "Everything");
			layerNumbers.Add (-3);
		}
		
		for (int i=0;i<32;i++) {
			
			string layerName = LayerMask.LayerToName (i);
			
			if (layerName != "") {
				if (selected == (selected | (1 << i))) {
					layers.Add ("[X] "+layerName);
				} else {
					layers.Add ("     "+layerName);
				}
				layerNumbers.Add (i);
			}
		}
		
		bool preChange = GUI.changed;
		
		GUI.changed = false;
		
		int newSelected = 0;
		
		if (Event.current.type == EventType.MouseDown) {
			newSelected = -1;
		}
		
		newSelected = EditorGUILayout.Popup (label,newSelected,layers.ToArray(),EditorStyles.layerMaskField);
		
		if (GUI.changed && newSelected >= 0) {
			
			int preSelected = selected;
			
			if (showSpecial && newSelected == 0) {
				selected = 0;
			} else if (showSpecial && newSelected == 1) {
				selected = -1;
			} else {
				
				if (selected == (selected | (1 << layerNumbers[newSelected]))) {
					selected &= ~(1 << layerNumbers[newSelected]);
					//Debug.Log ("Set Layer "+LayerMask.LayerToName (LayerNumbers[newSelected]) + " To False "+selected.value);
				} else {
					//Debug.Log ("Set Layer "+LayerMask.LayerToName (LayerNumbers[newSelected]) + " To True "+selected.value);
					selected = selected | (1 << layerNumbers[newSelected]);
				}
			}
			
			if (selected == preSelected) {
				GUI.changed = false;
			} else {
				Debug.Log ("Difference made");
			}
		}
		
		GUI.changed = preChange || GUI.changed;
		
		return selected;
	}
	
	public static float Hermite(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));
    }
    
    public static float Sinerp(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
    }

    public static float Coserp(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f));
    }
 
    public static float Berp(float start, float end, float value)
    {
        value = Mathf.Clamp01(value);
        value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
        return start + (end - start) * value;
    }
}
