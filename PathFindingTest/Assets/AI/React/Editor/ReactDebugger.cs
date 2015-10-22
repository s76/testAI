using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using React;
using System.Linq;

public class ReactDebugger : EditorWindow
{
	
	Root root;
	Reactor reactor;
	
	
	
	string NiceName (string s)
	{
		var n = "";
		for (int i = 0; i < s.Length; i++) {
			if (n.Length > 0 && char.IsUpper (s [i]) && (!char.IsUpper (n.Last ()))) {
				n += " ";                    
			}
			n += (i == 0 ? s [i].ToString ().ToUpper () : s [i].ToString ());
		}
		return n;
	}
	
	
	

	
	void Reload ()
	{
		Load (reactor);
	}
	
	
	void Load (Reactor asset)
	{
        Reactor.s_breakePoints.Clear();
        if( reactor != null )
        {
            reactor.isInDebug = false;
        }

		reactor = asset;
		if (reactor != null) {            
			if(EditorApplication.isPlaying) {
                reactor.isInDebug = true;
				root = reactor.GetRoot();
			}
		}       
	}
	
	
	void OnKey (Event e)
	{
		
	}

	void OnDrawGUI ()
	{
		if (root != null) {
			root.DrawEditorWidget ();	
		}
	}

	void OnDrawHeader ()
	{
	}

	void OnDrawTools ()
	{
		
		var back = GUI.backgroundColor;
		GUI.backgroundColor = Color.white;
		GUILayout.BeginVertical ();
		GUILayout.BeginHorizontal ("box");
		var e = GUI.enabled;
		GUI.enabled = true;
		GUILayout.Label ("Reactor GameObject: ");
		var oldReactable = reactor;
		var r = (Reactor)EditorGUILayout.ObjectField (reactor, typeof(Reactor), true);
		if (oldReactable != r) {
			if (r != null) {
				Load (r);	
			}
		}
		if(reactor != null) {
			reactor.pause = GUILayout.Toggle(reactor.pause, reactor.pause?"Resume":"Pause", "button");
			reactor.step = GUILayout.Toggle(reactor.step, reactor.step?"Running":"Step", "button");
		}
		
		GUILayout.FlexibleSpace ();
		
		
		GUILayout.EndHorizontal ();
	
		GUILayout.EndVertical ();
		GUI.backgroundColor = back;
		GUI.enabled = e;
	}

	void OnDrawWindows ()
	{
		
		if (toolWindowRect == null) {
			toolWindowRect = new Rect (window.position.width - 340, 64, 0, 0);
		} 
		if (toolWindowRect.Value.x > window.position.width - 10) {
			toolWindowRect = new Rect (window.position.width - 340, 64, 0, 0);
		}
		if (toolWindowRect.Value.yMax < 0) {
			toolWindowRect = new Rect (window.position.width - 340, 64, 0, 0);
		}
		
		toolWindowRect = GUILayout.Window (1, toolWindowRect.Value, DrawToolWindow, "Inspector", GUILayout.MinHeight (420), GUILayout.Width (256));
	}
		
	
	Reactor[] runningReactors = null;
	void DrawToolWindow (int id)
	{
		inspectorScrollPosition = EditorGUILayout.BeginScrollView (inspectorScrollPosition);
		GUILayout.BeginVertical ();
		if(GUILayout.Button("Get Active Reactors")) {
			runningReactors = GameObject.FindObjectsOfType(typeof(Reactor)) as Reactor[];
		}
		
		GUILayout.Space(22);
        if (runningReactors != null) {
            foreach (var i in runningReactors)
            {
                if (i != null && GUILayout.Button((i.transform.parent != null ? i.transform.parent.gameObject.name : "") + "." + i.gameObject.name))
                {
                    Selection.activeGameObject = i.gameObject;
                    Load(i);
                }
            }
		}
		GUILayout.EndVertical ();
		EditorGUILayout.EndScrollView();
		GUI.DragWindow ();
	}

	void OnValidateGUIState ()
	{
		if (window == null) {
			window = (ReactDebugger)EditorWindow.GetWindow (typeof(ReactDebugger));
		}
		
	}

    void DrawBreakePoints()
    {        
        Color originalColor = Handles.color;
        Handles.color = Color.red;
        float circleR = 10;
        foreach (BaseNode breakePoint in Reactor.s_breakePoints)
        {
            Vector2 bpPosition = breakePoint.rect.center;
            bpPosition.x -= breakePoint.rect.width / 2 + circleR;
            bool bpWasHit = Reactor.s_breakepointHit == breakePoint;
            if (bpWasHit)
            {
                Handles.color = Color.yellow;
            }

            Handles.DrawSolidDisc(bpPosition, new Vector3(0, 0, 1), circleR);
            //Handles.CircleCap(counter, bpPosition, Quaternion.identity, 10);
            if (bpWasHit)
            {
                Handles.color = Color.red;
            }            
        }
        Handles.color = originalColor;
    }

	void OnGUIHasBeenDrawn ()
	{
        if (BaseNode.hotNode == null)
            return;
        if (BaseNode.hotNode.activateContext)
        {                        
            bool isAction = BaseNode.hotNode is React.Action;
            if (isAction)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Toggle Breake Point"), false, OnToggleBreakepoint);
                menu.ShowAsContext();
            }                        
        }
		
	}

    void OnToggleBreakepoint()
    {
        if (BaseNode.hotNode != null)
        {
            Reactor.ToggleBreakePoint(BaseNode.hotNode);
        }        
    }
	
	void Update() {
		if(EditorApplication.isPaused || EditorApplication.isPlaying) {
			Repaint();
		}
        if (!EditorApplication.isPaused && EditorApplication.isPlaying )
        {
            Reactor.s_breakepointHit = null;
        }
	}

	void OnDestroy()
	{
		Reactor.s_breakePoints.Clear();
	}

	void OnGUI ()
	{
		if(!(EditorApplication.isPlaying || EditorApplication.isPaused)) {
			GUILayout.Label("Game must be running or paused to use the React Debugger");
			runningReactors = null;
			return;
		}
		OnValidateGUIState();
		
		
		
		GUILayout.BeginVertical ();
		GUILayout.BeginHorizontal ();
		OnDrawHeader ();
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
		OnDrawTools ();
		GUILayout.EndHorizontal ();
		scrollPosition = EditorGUILayout.BeginScrollView (scrollPosition);
		GUILayout.BeginVertical ();
		OnDrawGUI ();
        DrawBreakePoints();
		GUILayout.EndVertical ();
		
		
		EditorGUILayout.EndScrollView ();
		GUILayout.EndVertical ();
		BeginWindows ();
		OnDrawWindows ();
		EndWindows ();
		OnGUIHasBeenDrawn ();
		
		
	}
	
	
	
	[MenuItem("Window/React Debugger")]
	static void Init ()
	{
		window = (ReactDebugger)EditorWindow.GetWindow (typeof(ReactDebugger));        
		window.Show ();        
	}

   

	static ReactDebugger window;
	Vector2 scrollPosition;
	Vector2 inspectorScrollPosition;
	Rect? toolWindowRect;
	
}

