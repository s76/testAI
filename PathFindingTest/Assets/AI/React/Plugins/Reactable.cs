using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;



[System.Serializable]
public class ReactableEditorOperation
{
    public string desc;
    public string json;
}

#endif


public class Reactable : ScriptableObject
{
    public String[] behaviourTypes = new String[0];

    [HideInInspector]
    public string
        json;

    public static bool CanBeUsedAsAction(MethodInfo methodInfo, bool ignoreReturnType)
    {
        if (!methodInfo.IsPublic)
            return false;
        if (!ignoreReturnType && methodInfo.ReturnType != typeof(IEnumerator<React.NodeResult>))
            return false;
        if (methodInfo.GetParameters().Length != 0)
        {
            if (methodInfo.GetCustomAttributes(typeof(React.ReactActionAttribute), true).Length == 0)
                return false;
        }

        return true;
    }
#if UNITY_EDITOR
    public IEnumerable<Type> Behaviours { get { return behaviourTypes.Select(s => Type.GetType(s)); } }
    //public MonoScript[] behaviours = new MonoScript[0];
	
    [HideInInspector]
    public List<ReactableEditorOperation>
        history;
    [HideInInspector]
    public List<ReactableEditorOperation>
        future;
	
    public void RegisterUndo (string desc, React.Root root)
    {
        if (!(history.Count > 0 && history.Last ().json == json)) {
            history.Add (new ReactableEditorOperation () { desc = desc, json = React.JsonSerializer.Encode(root)});
            future.Clear ();
        }
    }
	
    public void PerformRedo ()
    {
        if (future.Count > 0) {
            var op = future.Last ();
            future.RemoveAt (future.Count - 1);
            history.Add (new ReactableEditorOperation () { desc = op.desc, json = json });
            json = op.json;
        }
    }
	
    public void PerformUndo ()
    {
        if (history.Count > 0) {
            var op = history.Last ();
            history.RemoveAt (history.Count - 1);
            future.Add (new ReactableEditorOperation () { desc = op.desc, json = json });
            json = op.json;
			
        }
    }

    // method def in format "DeclaringTypeName.MethodName"
    public MethodInfo FindMethod( string methodDef )
    {                
        string typeName = methodDef.Substring (0, methodDef.IndexOf ('.') );
        Type type = Type.GetType(typeName);
        return type.GetMethod( methodDef.Substring( methodDef.IndexOf( '.') + 1 ));
    }

    public List<string> Actions {
        get {
            var actions = new List<string> ();
            foreach (var b in Behaviours) {
                if (b == null)
                    continue;
                foreach (var i in b.GetMethods().OrderBy((arg) => arg.Name)) {
                    if( CanBeUsedAsAction( i, false ) )
                    {
                        actions.Add (string.Format ("{0}.{1}", i.DeclaringType.Name, i.Name));
                    }
                }
            }
            return actions;
        }
    }

    public Dictionary< string, List< string > > CollectActionsMap()
    {
        Dictionary< string, List<string> > actions = new Dictionary< string, List<string> >();
        foreach (var b in Behaviours) 
        {
            if (b == null)
            {
                continue;
            }            
            foreach ( var i in b.GetMethods() ) 
            {                
                if( CanBeUsedAsAction( i, false ) )
                {
                    List<string> methods = null;
                    actions.TryGetValue( i.DeclaringType.Name, out methods );                    
                    if( !actions.TryGetValue( i.DeclaringType.Name, out methods ) || methods == null )
                    {
                        methods = new List< string >();
                        actions[i.DeclaringType.Name] = methods;
                    }
                    methods.Add(i.Name);                    
                }
            }
        }
        return actions;
    }

    public List<string> Conditions {
        get {
            var actions = new List<string> ();
            foreach (var b in Behaviours) {
                if (b == null)
                    continue;
                foreach (var i in b.GetMethods().OrderBy((arg) => arg.Name)) {
                    if (i.IsPublic && i.ReturnType == typeof(bool) && i.GetParameters ().Length == 0) {
                        actions.Add (string.Format ("{0}.{1}", i.DeclaringType.Name, i.Name));
                    }
                }
            }
            return actions;
        }
    }

    public Dictionary<string, List<string>> CollectConditionsMap()
    {
        Dictionary<string, List<string>> conditions = new Dictionary<string, List<string>>();
        foreach (var b in Behaviours)
        {
            if (b == null)
            {
                continue;
            }
            foreach (var i in b.GetMethods())
            {
                if (i.IsPublic && i.ReturnType == typeof(bool) && i.GetParameters().Length == 0)
                {
                    List<string> methods = null;
                    conditions.TryGetValue(i.DeclaringType.Name, out methods);
                    if (!conditions.TryGetValue(i.DeclaringType.Name, out methods) || methods == null)
                    {
                        methods = new List<string>();
                        conditions[i.DeclaringType.Name] = methods;
                    }
                    methods.Add(i.Name);
                }
            }
        }
        return conditions;
    }

    public List<string> Functions {
        get {
            var actions = new List<string> ();
            foreach (var b in Behaviours) {
                if (b == null)
                    continue;
                foreach (var i in b.GetMethods().OrderBy((arg) => arg.Name)) {
                    if (i.IsPublic && i.GetParameters ().Length == 0) {
                        actions.Add (string.Format ("{0}.{1}", i.DeclaringType.Name, i.Name));
                    }
                }
            }
            return actions;
        }
    }
	
    public string DocText (string methodName)
    {
		
        var text = "";
        var parts = methodName.Split ('.');
        foreach (var b in Behaviours) {
            if (b == null)
                continue;
            MethodInfo mi = null;
            if (b.Name == parts [0]) {
                try {
                    mi = b.GetMethod (parts [1]);
                } catch (AmbiguousMatchException) {
                    mi = null;
                } 
                if (mi != null) {
                    foreach (React.ReactDocAttribute doc in mi.GetCustomAttributes(typeof(React.ReactDocAttribute), true)) {
                        text = text + "\r\n" + doc.docText + "\r\n";
                    }
                }

				

            }
        }
        return text;
	
    }
#endif
}
