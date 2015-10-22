using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class Reactor : MonoBehaviour, IPrewarmable
{
	
	public Reactable reactable;
    [SerializeField]
	private float tickDuration = 0.1f;
    public float TickDuration { get { return tickDuration; }
        set
        {
            tickDuration = value;
            delay = new WaitForSeconds(tickDuration);
            if (tickDuration <= 0) delay = null;
        }
    }
	React.Root root;
	IEnumerator<React.NodeResult> task;
	Dictionary<string,ComponentMethod> methods = new Dictionary<string, ComponentMethod> ();
	[HideInInspector]
	public bool pause = false;
	[HideInInspector]
	public bool step = false;


    #region modified by PanDenat
    public bool awakeEnabled = false;
    bool isInited = false;

    public void Initialize() {
        if (isInited) return;
        try {
            Profiler.BeginSample("Reactor.Initialize", this);
            LoadMethods();
            if (reactable != null) {
                AddComponentsIfMissing();
                root = (React.Root)React.JsonSerializer.Decode(reactable.json);
                root.PreProcess(gameObject, this);
                task = root.NodeTask();                
                isInited = true;
            }
            Profiler.EndSample();
        } catch (Exception e) {
            Debug.LogError("[Reactor] Error in initialization.\n" + e, this);
        }
    }

    void OnEnable() {
        if (isInited == false) Initialize();
        if (isInited == false) { Debug.LogWarning("[Reactor] Not initialized (empty reactable?): " + name, this); return; }
        StartCoroutine(RunReactable());
    }

    void OnDisable() {
        StopAllCoroutines();
    }

    void Awake() {
        if (awakeEnabled == false)
            enabled = false;
    }

    public void AddComponentsIfMissing(Reactable reactable = null) {
        if (reactable == null) reactable = this.reactable;

		if(reactable == null)
			Debug.LogError("Unable to add fill code with proper init component");
        /*if (reactable != null) {
            foreach (var b in reactable.behaviourTypes) {
                if (gameObject.GetComponent(b) == false) {
                    var c = UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(gameObject, "Assets/AI/React/Scripts/Reactor.cs (74,29)", b);
                    AddMethodsFromComponent(c);
                    var behaviour = c as Behaviour;
                    if (behaviour != null)
                        behaviour.Initialize();
                }
            }
        }*/
    }
    #endregion

    //void Start ()
    //{
    //    LoadMethods ();
    //    if (reactable != null) {
    //        root = (React.Root)React.JsonSerializer.Decode (reactable.json);
    //        root.PreProcess (gameObject, this);
    //        task = root.NodeTask ();
    //        StartCoroutine (RunReactable ());
    //    }
    //}
	
	public ComponentMethod FindMethod(string name) {
		if(methods.ContainsKey(name)) return methods[name];
		return null;
	}

    static List<string> _skip = new List<string> { "Component", "Transform", "MonoBehaviour", "Object" };
	void LoadMethods ()
	{		
		foreach (var c in gameObject.GetComponents<Component> ()) {
            AddMethodsFromComponent(c);
		}
	}

    public void AddMethodsFromComponent(Component c) 
    {
        foreach (var i in c.GetType().GetMethods()) 
        {
            if (_skip.Contains(i.DeclaringType.Name))
            {
                continue;
            }
            if ( Reactable.CanBeUsedAsAction( i, true ) ) 
            {
                methods[string.Format("{0}.{1}", i.DeclaringType.Name, i.Name)] = new ComponentMethod() { component = c, methodInfo = i };
            }
        }
    }
	
#if UNITY_EDITOR	
	[ContextMenu("Add reactable components")]
	void AddComponents() {
		if(reactable != null) {
			foreach(var c in reactable.Behaviours) {
				gameObject.AddComponent(c);	
			}
		}
	}
#endif

#if UNITY_EDITOR
    public bool isInDebug;
#endif

    WaitForSeconds delay;
	IEnumerator RunReactable ()
	{
		delay = new WaitForSeconds (tickDuration);
		if (tickDuration <= 0)
			delay = null;
		while (true) {
#if UNITY_EDITOR
			            
			if(step) {
				step = false;
				pause = true;
				yield return delay;
				task.MoveNext ();
			}

			if(pause) {
				yield return null;
				continue;
			} 
			
			yield return delay;
            if (isInDebug)
            {                
                root.ResetActivity();
            }
            Profiler.BeginSample("Reactor.TreeStep");
			task.MoveNext ();
		
#else
			yield return delay;
            Profiler.BeginSample("Reactor.TreeStep");
			task.MoveNext ();
#endif
            Profiler.EndSample();
		}
	}
	
	public React.Root GetRoot() {
		return root;	
	}

    void IPrewarmable.Prewarm()
    {
        Initialize();
    }

#if UNITY_EDITOR	
    
    public static HashSet<React.BaseNode> s_breakePoints = new HashSet<React.BaseNode>();
    public static React.BaseNode s_breakepointHit        = null;

    public static void AddBreakePoint(React.BaseNode beakePoint)
    {       
           s_breakePoints.Add(beakePoint);    
    }

    public static void RemoveBreakePoint(React.BaseNode beakePoint)
    {
        s_breakePoints.Remove(beakePoint);        
    }

    public static void ToggleBreakePoint(React.BaseNode breakePoint)
    {
        if (s_breakePoints.Contains(breakePoint))
        {
            if (s_breakepointHit == breakePoint)
            {
                s_breakepointHit = null;
            }
            s_breakePoints.Remove(breakePoint);
        }
        else
        {
            s_breakePoints.Add(breakePoint);
        }     
    }

    public static void CheckBreakePoint(React.BaseNode breakePoint)
    {
        if (breakePoint.ownerReactor!=null && breakePoint.ownerReactor.isInDebug && s_breakePoints.Contains(breakePoint))
        {
            Selection.activeTransform = breakePoint.ownerReactor.transform;
            Debug.Break();
            s_breakepointHit = breakePoint;
        }
    }
    
#endif
}
