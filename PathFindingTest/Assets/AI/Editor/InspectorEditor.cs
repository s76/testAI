using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using System.Collections;
using System.Linq;

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#pragma warning disable 0618 // obsolete
#pragma warning disable 0649 // null value

public class InspectorEditor : Editor 
{
	protected static readonly float valueWidth = 18, valueHeight = 18;
	private static Dictionary<string, Vector2> scrollPoses = new Dictionary<string,Vector2>();
	private static Dictionary<string, bool> isShowns = new Dictionary<string,bool>();
    /*
    public override void OnInspectorGUI()
    {
        var stack = new StackFrame(0);
        Debug.Log("[" + stack.GetType().Name + "] Invoked method: " + stack.GetMethod().Name);
        base.OnInspectorGUI();
    }

    public void OnEnable()//this makes errors on start
    {
        var stack = new StackFrame(0);
        Debug.Log("[" + stack.GetType().Name + "] Invoked method: " + stack.GetMethod().Name);
        base.OnInspectorGUI();
    }

    public void OnAwake()
    {
        var stack = new StackFrame(0);
        Debug.Log("[" + stack.GetType().Name + "] Invoked method: " + stack.GetMethod().Name);
        base.OnInspectorGUI();
    }*/

	public Vector2 GetScrollPos(string id)
	{
		Vector2 v = Vector2.zero;
		scrollPoses.TryGetValue(id, out v);
		return v;
	}

	public void SetScrollPos(string id, Vector2 pos)
	{
		scrollPoses[id] = pos;
	}

	public bool GetIsShown(string id, bool defaultVal = false)
	{
		bool v = false;
		if(!isShowns.TryGetValue(id, out v))
			v = defaultVal;
		return v;
	}

	public void SetIsShown(string id, bool isShown)
	{
		isShowns[id] = isShown;
	}

	public static void AddSpace()
	{
		EditorGUILayout.Space();
	}
	
	public static Rect CalcLayoutSimpleRect(float fixedWidth = -1f)
	{
		return GUILayoutUtility.GetRect(fixedWidth > 0 ? fixedWidth : valueWidth, valueHeight, "TextField");
	}

	public static void ProgressBar(float value, string label) 
	{
        Rect rect = GUILayoutUtility.GetRect(valueWidth, valueHeight, "TextField");
        EditorGUI.ProgressBar(rect, value, label);
    }
	
	public static float Slider(float value, float minVal, float maxVal, string label) 
	{
		Rect rect = GUILayoutUtility.GetRect(valueWidth, valueHeight, "TextField");
		return EditorGUI.Slider(rect, label, value, minVal, maxVal);
	}
	
	public static void Label(string value, GUIStyle style = null)
	{
        Rect rect = GUILayoutUtility.GetRect(valueWidth, valueHeight, "TextField");
		if(style == null)
			EditorGUI.LabelField(rect, value);
		else
			EditorGUI.LabelField(rect, value, style);
	}
	
	public static string TextField(string value, string label = "")
	{
		Rect rect = GUILayoutUtility.GetRect(valueWidth, valueHeight, "TextField");
		return EditorGUI.TextField(rect, label, value);
	}
	public static string TextField(string value, string label, Vector2 size)
	{
		Rect rect = GUILayoutUtility.GetRect(size.x, size.y, "TextField");
		if(string.IsNullOrEmpty(label))
		   return EditorGUI.TextField(rect, value);
		return EditorGUI.TextField(rect, label, value);
	}

	public static string TextField(string value, string label, float width)
	{
		Rect rect = GUILayoutUtility.GetRect(width, valueHeight, "TextField");
		if(string.IsNullOrEmpty(label))
		   return EditorGUI.TextField(rect, value);
		return EditorGUI.TextField(rect, label, value);
	}
	
	public static string TextArea(string value, string label = "", float heightFactor = 4f)
	{
		if(label.Length > 0)
			Label(label);
        Rect rect = GUILayoutUtility.GetRect(valueWidth, valueHeight * heightFactor, "TextArea");
		return EditorGUI.TextArea(rect, value);
	}
		
	public static Vector2 Vector2View(Vector2 v, string label, bool addSpace = true)
	{
        Rect rect = GUILayoutUtility.GetRect(valueWidth, valueHeight, "TextField");
		Vector2 ret = EditorGUI.Vector2Field(rect, label, v);	
		if(addSpace)
			AddSpace();
		return ret;
	}
		
	public static Vector3 Vector3View(Vector3 v, string label)
	{
        Rect rect = GUILayoutUtility.GetRect(valueWidth, valueHeight * 2f, "TextField");
		Vector3 ret = EditorGUI.Vector3Field(rect, label, v);	
		AddSpace();
		return ret;
	}
	
	public static Vector4 Vector4View(Vector4 v, string label)
	{
        Rect rect = GUILayoutUtility.GetRect(valueWidth, valueHeight * 2f, "TextField");
		Vector4 ret = EditorGUI.Vector4Field(rect, label, v);	
		AddSpace();
		return ret;
	}
	
	public static void TransformView(Transform t, string label)
	{
        Rect rect = GUILayoutUtility.GetRect(valueWidth, valueHeight, "TextField");
		EditorGUI.ObjectField(rect, label, t, typeof(Transform));	
	}
	
	public static UnityEngine.Object ObjectView(UnityEngine.Object o, Type type, string label = "")
	{
        Rect rect = GUILayoutUtility.GetRect(valueWidth, valueHeight, "TextField");
		if(label.Length > 0)
			return EditorGUI.ObjectField(rect, label, o, type);	
		else
			return EditorGUI.ObjectField(rect, o, type, true);	
	}
	
	public static bool Button(string label, float width = -1f)
	{
		if(width < 0f)
			width = valueWidth;
        Rect rect = GUILayoutUtility.GetRect(width, valueHeight, "TextField");
		return GUI.Button(rect, label);
	}

	public static bool Button(string label, bool enabled)
	{
		GUI.enabled = enabled;
		try{
			return Button(label);
		}
		finally{

			GUI.enabled = true;
		}
	}
	public static bool Button(string label,Color c, bool enabled=true)
	{
		GUI.enabled = enabled;
		Color def = GUI.color;
		GUI.color = c;
		try{
			return Button(label);
		}
		finally{
			GUI.color = def;
			GUI.enabled = true;
		}
	}

	public static void DictionaryField<TKey, TValue>(Dictionary<TKey, TValue> dict, ref Vector2 scrollPosition, float expectedHeight = 100f)
	{
		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(expectedHeight));
		EditorGUILayout.BeginVertical();
		foreach(var pair in dict)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(pair.Key.ToString());
			GUILayout.Label(pair.Value.ToString());
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndScrollView();

	}

	public static void ListField<T>(IEnumerable list, ref bool show, ref Vector2 scrollPosition, string label = "", float expectedHeight = 100f) where T:UnityEngine.Object
	{
        Rect rect = GUILayoutUtility.GetRect(valueWidth, valueHeight);
        show = EditorGUI.Foldout(rect, show, label);
		if(show)
		{
			int count = 0;
			foreach(var val in list)
				count++;
			float height = Mathf.Min(expectedHeight, valueHeight * count);
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, EditorStyles.helpBox, GUILayout.Height(height));
			EditorGUILayout.BeginVertical();
			int id = 0;
			foreach(var val in list)
			{
				EditorGUILayout.ObjectField(id.ToString(), val as T, typeof(T));
				id++;
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();
		}
	}

	public static void ListField<T>(ICollection<T> list, ref Vector2 scrollPosition, ref bool show, string label = "", float expectedHeight = 100f)
	{
        Rect rect = GUILayoutUtility.GetRect(valueWidth, valueHeight);
        show = EditorGUI.Foldout(rect, show, label);
		float height = Mathf.Min(expectedHeight, valueHeight * list.Count);
		if(show)
		{
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, EditorStyles.helpBox, GUILayout.Height(height));
			EditorGUILayout.BeginVertical();
			int id = 0;
			foreach(var val in list)
			{
				GUILayout.Label(id + ": " + val);
				id++;
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();
		}
	}

	public static void ListFieldWithButtons<T>(ICollection<T> list, ref Vector2 scrollPosition, ref bool show, string buttonText, Action<T> action, string label = "", float expectedHeight = 100f)
	{
        Rect rect = GUILayoutUtility.GetRect(valueWidth, valueHeight);
        show = EditorGUI.Foldout(rect, show, label);
		float height = Mathf.Min(expectedHeight, valueHeight * list.Count);
		if(show)
		{
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, EditorStyles.helpBox, GUILayout.Height(height));
			EditorGUILayout.BeginVertical();
			int i = 0;
			foreach(var elem in list)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(i + ": " + elem);
				if(GUILayout.Button(buttonText))
					action(elem);
				EditorGUILayout.EndHorizontal();
				i++;
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();
		}
	}

	public void ListField(List<string> list, string scrollId, string showId, string label = "", bool showAddRemove = true, float expectedHeight = 100f)
	{
		bool isShown = GetIsShown(showId);
		Vector2 scrollPos = GetScrollPos(scrollId);
		ListField(list, ref scrollPos, ref isShown, label, showAddRemove, expectedHeight);
		SetIsShown(showId, isShown);
		SetScrollPos(scrollId, scrollPos);
	}

	public static void ListField(List<string> list, ref Vector2 scrollPosition, ref bool show, string label = "", bool showAddRemove = true, float expectedHeight = 100f)
	{
        Rect rect = GUILayoutUtility.GetRect(valueWidth, valueHeight);
        show = EditorGUI.Foldout(rect, show, label);
		float height = Mathf.Min(expectedHeight, valueHeight * list.Count) + 5f;//5 is for border
		if(showAddRemove)
			height += valueHeight;
		if(show)
		{
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, EditorStyles.helpBox, GUILayout.Height(height));
			EditorGUILayout.BeginVertical();
			
			for(int i = 0; i < list.Count; i++)
			{
				if(showAddRemove)
				{
					EditorGUILayout.BeginHorizontal();
					Label(i + ":");
					list[i] = TextField(list[i], "");
					if(Button("Remove", 20f))
						list.RemoveAt(i--);
					EditorGUILayout.EndHorizontal();
				}
				else
				{
					list[i] = TextField(list[i], i + ":");
				}
			}

			if(showAddRemove)
			{
				if(list.Count > 0)
					list.Add(list.Last());
				else
					list.Add("");
				scrollPosition.y = Mathf.Infinity;
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();
		}
	}

	public static void ListField(List<Vector2> list, ref Vector2 scrollPosition, ref bool show, string label = "", float expectedHeight = 100f)
	{
        Rect rect = GUILayoutUtility.GetRect(valueWidth, valueHeight);
        show = EditorGUI.Foldout(rect, show, label);
		float height = Mathf.Min(expectedHeight, valueHeight * list.Count);
		if(show)
		{
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, EditorStyles.helpBox, GUILayout.Height(height));
			EditorGUILayout.BeginVertical();
			
			for(int i = 0; i < list.Count; i++)
			{
				list[i] = Vector2View(list[i], i + ":");
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();
		}
	}
	
	public static Enum EnumField(string label, Enum enumVal)
	{
		return EnumField(enumVal, label);
	}

	public static Enum EnumField(Enum enumVal, string label = "")
	{
		if(string.IsNullOrEmpty(label))
			return EditorGUILayout.EnumPopup(enumVal);
		return EditorGUILayout.EnumPopup(label, enumVal);
	}
	
	public static int IntField(string label, int val)
	{
		return IntField(val, label);
	}
	
	public static int IntField(int val, string label = "")
	{
		if(string.IsNullOrEmpty(label))
			return EditorGUILayout.IntField(val);
		return EditorGUILayout.IntField(label, val);
	}

	public static float FloatField(string label, float val)
	{
		return FloatField(val, label);
	}
	
	public static float FloatField(float val, string label = "")
	{	
		if(string.IsNullOrEmpty(label))
			return EditorGUILayout.FloatField(val);
		return EditorGUILayout.FloatField(label, val);
	}
	
	public static void TextureView(Texture2D tex, float width, float height)
	{
		Rect rect = GUILayoutUtility.GetRect(width, height, "TextField");
		GUI.DrawTexture(rect, tex, ScaleMode.StretchToFill, true);
	}

	public static void HelpBox(string message, MessageType type = MessageType.Info)
	{
		EditorGUILayout.HelpBox(message, type);
	}

	public static bool FoldOut(bool show, string label)
    {
        Rect rect = GUILayoutUtility.GetRect(valueWidth, valueHeight);
        return EditorGUI.Foldout(rect, show, label);
    }
	
	public static bool BoolField(string label, bool value)
	{
		Rect rect = GUILayoutUtility.GetRect(valueWidth, valueHeight, "TextField");
		return GUI.Toggle(rect, value, label);
	}

	//call it in method OnSceneGUI();
	public static int DrawEditablePoints(Vector3[] points,Color color,float size = 0.09f)
	{
		Handles.color = color;
		if (Tools.current != Tool.View && Event.current.type == EventType.Layout)
		{
			for (int i = 0; i < points.Length; i++)
			{
				HandleUtility.AddControl(-i - 1, HandleUtility.DistanceToLine(points[i], points[i]));
			}
		}
		int wasMoved = -1;

		if (Tools.current != Tool.View)
			HandleUtility.AddDefaultControl(0);

		for (int i = 0; i < points.Length; i++)
		{

			if (Tools.current == Tool.Move)
			{
#if UNITY_LE_4_3
				//Undo.SetSnapshotTarget(script, "Moved Point");
#else
				//Undo.RecordObject(target, "Moved Point");
#endif
				Handles.SphereCap(-i - 1, points[i], Quaternion.identity, HandleUtility.GetHandleSize(points[i]) * size * 2);
				Vector3 pre = points[i];
				Vector3 post = Handles.PositionHandle(points[i], Quaternion.identity);
				if (pre != post)
				{
					points[i] = post;
					wasMoved = i;
				}
			}
			else
			{
				Handles.SphereCap(-i - 1, points[i], Quaternion.identity, HandleUtility.GetHandleSize(points[i]) * size);
			}
		}
		return wasMoved;
	}

	public class SearchablePopupData
	{
		public int id;
		public string label;
		public string[] strings;
		public bool searchShow;
		public Vector2 searchScrollPos;
		public string searchString;

	}

	public static SearchablePopupData SearchablePopup(SearchablePopupData data)
	{
		data.id = SearchablePopup(data.id, data.label, data.strings, ref data.searchShow, ref data.searchScrollPos,
			ref data.searchString);
		return data;
	}
	public static int SearchablePopup(int id, string label, string[] strings, ref bool searchShow, ref Vector2 searchScrollPos, ref string searchString)
	{
		if(searchString == null)
			searchString = "";
		GUILayout.BeginVertical();
		if(string.IsNullOrEmpty(label))
			id = EditorGUILayout.Popup(id, strings);
		else		
			id = EditorGUILayout.Popup(label, id, strings);
		if(searchShow = EditorGUILayout.Foldout(searchShow, "Search for:"))
		{
			searchScrollPos = EditorGUILayout.BeginScrollView(searchScrollPos, GUILayout.Height(searchString.Length > 0 ? 128 : valueHeight * 2f));
			searchString = EditorGUILayout.TextField(searchString);
			if(!string.IsNullOrEmpty(searchString))
			{
				for(int i = 0; i < strings.Length; i++)
				{
					var str = strings[i];
					if(!string.IsNullOrEmpty(str) && str.Contains(searchString))
					{
						if(GUILayout.Button(str))
							id = i;
					}
				}
			}
			EditorGUILayout.EndScrollView();
		}
		GUILayout.EndVertical();
		
		return id;
	}
	

	public static int Popup(int id, string label, string[] strings)
	{
		if(string.IsNullOrEmpty(label))
			id = EditorGUILayout.Popup(id, strings);
		else		
			id = EditorGUILayout.Popup(label, id, strings);
		
		return id;
	}
}
