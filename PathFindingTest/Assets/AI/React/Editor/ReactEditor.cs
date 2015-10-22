using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using React;
using System.Linq;

public class ReactEditor : EditorWindow
{
    
    Root root;
    Reactable reactable;

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
    
    void OnSelectionChange ()
    {
        if (root != null && reactable != null) {
            Save ();
        }
    }
    
    void OnEnable ()
    {
        EditorApplication.playmodeStateChanged += Save; 
        EditorApplication.projectWindowChanged += Reload;
    }
    
    void OnDisable ()
    {
        EditorApplication.playmodeStateChanged -= Save;
        EditorApplication.projectWindowChanged -= Reload;
        Save ();
    }
    
    void Reload ()
    {
        Load (reactable);
    }
    
    void Save ()
    {
        if (reactable == null || root == null) {
        } else {
            reactable.json = React.JsonSerializer.Encode (root);
            EditorUtility.SetDirty (reactable);
        }
    }
    
    void Load (Reactable asset)
    {
        reactable = asset;
        if (reactable != null) {
            if (reactable.json != null && reactable.json.Length > 0) {
                root = (Root)React.JsonSerializer.Decode (reactable.json);
                BaseNode.hotNode = root;
                BaseNode.editorReactable = reactable;
            } else {
                root = new Root ();
                BaseNode.hotNode = root;
                BaseNode.editorReactable = reactable;
            }
        }
    }
    
    void OnPaste ()
    {
        if (BaseNode.hotNode != null) {
            reactable.RegisterUndo ("Paste", root);
            var newNode = (BaseNode)React.JsonSerializer.Decode (EditorGUIUtility.systemCopyBuffer);
            BaseNode.hotNode.Add (newNode);
            BaseNode.hotNode = newNode;
            Save ();
        }
    }

    void OnCut ()
    {
        OnCopy ();
        OnDelete ();
    }

    void OnCopy ()
    {
        if (BaseNode.hotNode != null && BaseNode.hotNode.GetType () != typeof(Root)) {
            EditorGUIUtility.systemCopyBuffer = React.JsonSerializer.Encode (BaseNode.hotNode);
        }
    }

    void OnDuplicate ()
    {
        if (BaseNode.hotNode != null) {
            reactable.RegisterUndo ("Duplicate", root);
            var s = React.JsonSerializer.Encode (BaseNode.hotNode);
            var p = root.FindParent (BaseNode.hotNode);
            if (p != null) {
                var newNode = (BaseNode)React.JsonSerializer.Decode (s);
                p.Add (newNode);
                BaseNode.hotNode = newNode;
            }
            Save ();
        }
    }

    void OnHideChildren()
    {
        if( BaseNode.hotNode != null )
        {
            BaseNode.hotNode.hideChildren = true;
        }        
    }

    void OnShowChildren()
    {
        if (BaseNode.hotNode != null)
        {
            BaseNode.hotNode.hideChildren = false;
        } 
    }    

    void OnDelete ()
    {
        if (BaseNode.hotNode != null) {
            reactable.RegisterUndo ("Delete", root);
            var p = root.FindParent (BaseNode.hotNode);
            if (p != null) {
                p.Remove (BaseNode.hotNode);
                BaseNode.hotNode = p;
            }
            Save ();
        }
    }

    void OnKey (Event e)
    {
        if(GUI.GetNameOfFocusedControl() != "") return; //something has focus, so don't navigate the tree!
        if (BaseNode.hotNode != null) {
            var p = root.FindParent (BaseNode.hotNode);
            if (p == null)
                p = root;
            var c = p.Children ().ToList ();
            var i = c.IndexOf (BaseNode.hotNode);
            var children = BaseNode.hotNode.Children ().ToArray ();

            switch (e.keyCode) {
            case KeyCode.UpArrow:
                i -= 1;
                if (i < 0)
                    i = c.Count - 1;
                if(e.shift) {
                    p.Remove(BaseNode.hotNode);
                    p.Insert(i, BaseNode.hotNode);
                } else {
                    BaseNode.hotNode = c [i];
                }
                e.Use ();
                break;
            case KeyCode.DownArrow:
                i += 1;
                if (i >= c.Count)
                    i = 0;
                if(e.shift) {
                    p.Remove(BaseNode.hotNode);
                    p.Insert(i, BaseNode.hotNode);
                } else {
                    BaseNode.hotNode = c [i];
                }
                e.Use ();
                break;
            case KeyCode.LeftArrow:
                BaseNode.hotNode = p;
                e.Use ();
                break;
            case KeyCode.RightArrow:
                if (children.Count () > 0) {
                    BaseNode.hotNode = children [0];
                    e.Use ();
                }
                
                break;
            default:
                break;
            }

        }
    }

    void OnDrawGUI ()
    {
        if (root != null) {
            EditorGUI.BeginChangeCheck ();
            root.DrawEditorWidget ();   
            if (EditorGUI.EndChangeCheck ()) {
                Save ();    
            }
        }
    }

    void OnDrawHeader ()
    {
    }

    void OnDrawTools ()
    {
        if (reactable == null)
            return;
        var back = GUI.backgroundColor;
        GUI.backgroundColor = Color.white;
        GUILayout.BeginVertical ();
        GUILayout.BeginHorizontal ("box");
        var e = GUI.enabled;
        GUI.enabled = true;
        GUILayout.Label ("React AI Asset: ");
        var oldReactable = reactable;
        var r = (Reactable)EditorGUILayout.ObjectField (reactable, typeof(Reactable), false);
        if (oldReactable != r) {
            if (r != null) {
                if (reactable != null) {
                    Save ();
                }
                Load (r);   
            }
        }

        GUILayout.EndHorizontal();

        if (reactable.Behaviours.Count() == 0) {
            GUILayout.BeginHorizontal ("box");
            GUI.enabled = true;
            var color = GUI.color;
            GUI.color = Color.white;
            GUILayout.Label ("Warning: There are no behaviours assigned to this reactable asset.");
            GUI.color = color;
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal ("box");
        GUI.enabled = reactable.history.Count > 0;
        var undoLabel = reactable.history.Count > 0 ? "Undo " + reactable.history.Last ().desc : "Undo";
        if (GUILayout.Button (undoLabel, GUILayout.MinWidth (128))) {
            reactable.PerformUndo ();
            Load (reactable);
            Save ();
        }
        GUI.enabled = reactable.future.Count > 0;
        var redoLabel = reactable.future.Count > 0 ? "Redo " + reactable.future.Last ().desc : "Redo";
        if (GUILayout.Button (redoLabel, GUILayout.MinWidth (128))) {
            reactable.PerformRedo ();
            Load (reactable);
            Save ();
        }
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

        GUI.SetNextControlName ("Inspector");
        toolWindowRect = GUILayout.Window (1, toolWindowRect.Value, DrawToolWindow, "Inspector", GUILayout.MinHeight (420), GUILayout.Width (256));
    }

    void AddToHotNodeButton(System.Type t) {
        var tip = "";
        var method = t.GetMethod("GetHelpText");
        tip = (string)method.Invoke(null,null);
        var label = new GUIContent(t.Name, tip);
        if (GUILayout.Button (label, GUILayout.Width (100)))
            AddToHotNode (t);
    }
        
    void DrawToolWindow (int id)
    {
        if (nodeTypes == null)
            nodeTypes = ReactNodeAttribute.GetTypes ().OrderBy ((arg) => arg.Name).ToArray ();
        GUILayout.BeginHorizontal ();
        GUILayout.BeginVertical (GUILayout.Width (64));
        
        var back = GUI.backgroundColor;
        
        GUI.backgroundColor = back;
        GUI.enabled = true;
        GUILayout.EndVertical ();
        GUILayout.BeginVertical ();
        if (BaseNode.hotNode != null) {
            BaseNode.hotNode.DrawInspector ();  
        }
        
        GUILayout.FlexibleSpace ();  
        var isParent = BaseNode.hotNode.IsParent ();
        GUI.enabled = isParent;
        GUILayout.Label ("Add a Child Node:");
        GUILayout.BeginHorizontal ();
        
        if (nodeTypes != null) {
            
            GUILayout.BeginVertical ();
            GUILayout.Label ("Branch");
            var sequenceTypes = (from i in nodeTypes orderby i.Name where i.BaseType == typeof(React.BranchNode) select i).ToArray ();
            foreach (var t in sequenceTypes) {
                AddToHotNodeButton(t);
            }
            GUILayout.EndVertical ();
            GUILayout.BeginVertical (); 
            GUILayout.Label ("Leaf");
            var leafTypes = (from i in nodeTypes orderby i.Name where i.BaseType == typeof(React.LeafNode) select i).ToArray ();
            foreach (var t in leafTypes) {
                AddToHotNodeButton(t);
            }
            GUILayout.EndVertical ();
            GUILayout.BeginVertical ();
            GUILayout.Label ("Decorator");
            var decoratorTypes = (from i in nodeTypes orderby i.Name where i.BaseType == typeof(React.DecoratorNode) select i).ToArray ();
            foreach (var t in decoratorTypes) {
                AddToHotNodeButton(t);
            }
            GUILayout.EndVertical ();
        }
        GUILayout.EndHorizontal ();
        GUILayout.EndVertical ();
        
        GUILayout.EndHorizontal ();
    }

    void OnValidateGUIState ()
    {
        if (window == null) {
            window = (ReactEditor)EditorWindow.GetWindow (typeof(ReactEditor));
        }
        if (root == null) {
            if (reactable != null)
                Load (reactable);
            if (root == null) {
                root = new Root ();
            }
        }
    }  

    void OnGUIHasBeenDrawn ()
    {
        if (BaseNode.hotNode == null)
            return;
        if (BaseNode.hotNode.activateContext) {
            // Now create the menu, add items and show it
            var menu = new GenericMenu ();
            var isParent = BaseNode.hotNode.IsParent ();
            var sequenceTypes = (from i in nodeTypes orderby i.Name where i.BaseType == typeof(React.BranchNode) select i).ToArray ();
            foreach (var t in sequenceTypes) {
                if (isParent)
                    menu.AddItem (new GUIContent ("Add/Branch/" + t.Name), false, AddToHotNode, t);
                menu.AddItem (new GUIContent ("Convert/Branch/" + t.Name), false, ConvertHotNode, t);
            }
            
            var leafTypes = (from i in nodeTypes orderby i.Name where i.BaseType == typeof(React.LeafNode) select i).ToArray ();
            foreach (var t in leafTypes) {
                if (isParent)
                    menu.AddItem (new GUIContent ("Add/Leaf/" + t.Name), false, AddToHotNode, t);
                menu.AddItem (new GUIContent ("Convert/Leaf/" + t.Name), false, ConvertHotNode, t);
            }
            
            var decoratorTypes = (from i in nodeTypes orderby i.Name where i.BaseType == typeof(React.DecoratorNode) select i).ToArray ();
            foreach (var t in decoratorTypes) {
                if (isParent)
                    menu.AddItem (new GUIContent ("Add/Decorator/" + t.Name), false, AddToHotNode, t);
                menu.AddItem (new GUIContent ("Convert/Decorator/" + t.Name), false, ConvertHotNode, t);
                menu.AddItem (new GUIContent ("Decorate/" + t.Name), false, DecorateHotNode, t);
            }

            menu.AddSeparator ("");
            Action hotNodeAction = BaseNode.hotNode as Action;
            if (hotNodeAction != null)
            {
                Dictionary< string, List< string > > actionsMap = BaseNode.editorReactable.CollectActionsMap();

                foreach (KeyValuePair<string, List<string> > kvp in actionsMap)
                {
                    foreach (string method in kvp.Value )
                    {
                        menu.AddItem(new GUIContent("Set Action/" + kvp.Key + "/" + method), false, SetActionMethod, new KeyValuePair<string, string>( kvp.Key, method ) );                    
                    }                    
                }
            }
            
            Condition hotNodeCondition = BaseNode.hotNode as Condition;
            If hotNodeIf = BaseNode.hotNode as If;
            if (hotNodeCondition != null || hotNodeIf != null )
            {                
                Dictionary< string, List< string > > actionsMap = BaseNode.editorReactable.CollectConditionsMap();

                foreach (KeyValuePair<string, List<string> > kvp in actionsMap)
                {
                    foreach (string method in kvp.Value )
                    {
                        menu.AddItem(new GUIContent("Set Condition/" + kvp.Key + "/" + method), false, SetConditionMethod, new KeyValuePair<string, string>(kvp.Key, method));                    
                    }                    
                }
            }
            
            menu.AddSeparator("");

            menu.AddItem (new GUIContent ("Cut"), false, OnCut);
            menu.AddItem (new GUIContent ("Copy"), false, OnCopy);
            if (isParent)
                menu.AddItem (new GUIContent ("Paste"), false, OnPaste);
            else
                menu.AddDisabledItem (new GUIContent ("Paste"));
            menu.AddItem (new GUIContent ("Duplicate"), false, OnDuplicate);
            menu.AddItem (new GUIContent ("Delete"), false, OnDelete);

            if (BaseNode.hotNode.HasAnyChildren())
            {
                if (BaseNode.hotNode.hideChildren)
                {
                    menu.AddItem(new GUIContent("Show"), false, OnShowChildren );
                }
                else
                {
                    menu.AddItem(new GUIContent("Hide"), false, OnHideChildren );
                }
            }

            menu.ShowAsContext ();
        }
        
    }
    
    void ConvertHotNode (object t)
    {
        var type = (System.Type)t;
        var parent = root.FindParent (BaseNode.hotNode);
        var children = BaseNode.hotNode.Children ().ToArray ();
        var index = parent.Children ().ToList ().IndexOf (BaseNode.hotNode);
        
        reactable.RegisterUndo ("Convert to " + type.Name + " node", root);
        
        parent.Remove (BaseNode.hotNode);
        var newNode = System.Activator.CreateInstance (type) as BaseNode;
        parent.Insert (index, newNode);
        foreach (var c in children) {
            newNode.Add (c);    
        }
        BaseNode.hotNode = newNode;
        Save ();
    }

    void SetActionMethod( object t )
    {
        KeyValuePair<string, string> method = ( KeyValuePair<string, string> ) t;

         Action hotNodeAction = BaseNode.hotNode as Action;
         if (hotNodeAction == null)
             return;

         hotNodeAction.actionMethod = string.Format("{0}.{1}", method.Key, method.Value);
    }

    void SetConditionMethod(object t)
    {
        KeyValuePair<string, string> method = (KeyValuePair<string, string>)t;

        Condition hotNodeCondition = BaseNode.hotNode as Condition;
        If hotNodeIf = BaseNode.hotNode as If;
        if (hotNodeCondition != null )
        {
            hotNodeCondition.conditionMethod = string.Format("{0}.{1}", method.Key, method.Value);
        }
        else if (hotNodeIf != null )
        {
            hotNodeIf.conditionMethod = string.Format("{0}.{1}", method.Key, method.Value);
        }
    }

    void DecorateHotNode (object t)
    {
        var type = (System.Type)t;
        var parent = root.FindParent (BaseNode.hotNode);
        var decoratedNode = BaseNode.hotNode;

        var index = parent.Children ().ToList ().IndexOf (decoratedNode);
        
        reactable.RegisterUndo ("Decorate with " + type.Name + " node", root);
        
        parent.Remove (decoratedNode);
        var newNode = System.Activator.CreateInstance (type) as DecoratorNode;
        parent.Insert (index, newNode);
        newNode.Add (decoratedNode);
        BaseNode.hotNode = newNode;
        Save ();
    }
    
    void AddToHotNode (object t)
    {
        var type = (System.Type)t;
        reactable.RegisterUndo ("Add " + type.Name + " node", root);
        var child = System.Activator.CreateInstance (type) as BaseNode;
        BaseNode.hotNode.Add (child);
        BaseNode.hotNode = child;
        Save ();
    }
    
    bool wasCompiling = false;


    void OnGUI ()
    { 
        if (EditorApplication.isCompiling) {
            wasCompiling = true;
        } else {
            if (wasCompiling) {
                Reload ();
                wasCompiling = false;
            }
        }
        if (Event.current.type == EventType.DragUpdated) {
            BaseNode.hoverNode = null;
        }
        
        OnValidateGUIState ();
        if (BaseNode.hotNode == null) {
            GUILayout.Label("Please select an asset in the project window, right click and choose Edit Reactable.","box", GUILayout.ExpandWidth(true));
            return;
        }
        Event e = Event.current;
        
        
        if (e.type == EventType.ValidateCommand) {
            switch (e.commandName) {
            case "Paste":
                e.Use ();
                break;
            case "Duplicate":
                e.Use ();
                break;
            case "Copy":
                e.Use ();
                break;
            case "Cut":
                e.Use ();
                break;
            case "Delete":
                e.Use ();
                break;
            }
        }
        
        if (e.type == EventType.ExecuteCommand) {
            switch (e.commandName) {
            case "Paste":
                OnPaste ();
                return;
            case "Duplicate":
                OnDuplicate ();
                Repaint ();
                return;
            case "Copy":
                OnCopy ();
                Repaint ();
                return;
            case "Cut":
                OnCut ();
                Repaint ();
                return;
            case "Delete":
                OnDelete ();
                Repaint ();
                return;
            }
            
        }
        
        if (e.isKey && e.type == EventType.KeyDown) {
            OnKey (e);
        }
        
        GUILayout.BeginVertical ();
        GUILayout.BeginHorizontal ();
        OnDrawHeader ();
        GUILayout.EndHorizontal ();
        GUILayout.BeginHorizontal();
        scrollPosition = EditorGUILayout.BeginScrollView (scrollPosition);
        GUILayout.BeginVertical ();
        OnDrawGUI ();
        GUILayout.EndVertical ();
        
        if (BaseNode.hoverNode != null) {
            if (BaseNode.hoverNode.IsParent ()) {
                var hrect = BaseNode.hoverNode.rect;
                hrect.xMin -= 6;
                hrect.yMin -= 3;
                hrect.yMax += 6;
                GUI.Box (hrect, "");
            }
        }
        
        EditorGUILayout.EndScrollView ();

        GUILayout.BeginVertical("box", GUILayout.MaxWidth(256));
        OnDrawTools ();
        DrawToolWindow(0);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical ();

        BeginWindows ();
        //OnDrawWindows ();
        EndWindows ();
        OnGUIHasBeenDrawn ();        

        if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform) {
            
            
            if (BaseNode.hotNode != null && BaseNode.hoverNode != null && BaseNode.hoverNode != BaseNode.hotNode) {

                if (BaseNode.hoverNode.IsParent ()) {
                    
                    if (Event.current.shift) {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    } else {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;    
                    }
                    if (e.type == EventType.DragPerform) {
                        DragAndDrop.AcceptDrag ();
                        var serial = React.JsonSerializer.Encode (BaseNode.hotNode);
                        var newNode = (BaseNode)React.JsonSerializer.Decode (serial);   
                        if (!e.shift) {
                            var parent = root.FindParent (BaseNode.hotNode);
                            if (parent != null) {
                                reactable.RegisterUndo ("Move " + BaseNode.hotNode.ToString (), root);
                                BaseNode.hoverNode.Insert (0, newNode);
                            
                                parent.Remove (BaseNode.hotNode);
                                BaseNode.hotNode = BaseNode.hoverNode;
                                Save ();
                            }
                        } else {
                            reactable.RegisterUndo ("Copy " + BaseNode.hotNode.ToString (), root);
                            BaseNode.hoverNode.Insert (0, newNode);
                            Save ();
                        }
                    }
                    
                } else {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                }
            }
        }
        if (e.type == EventType.DragExited) {
            BaseNode.hoverNode = null;
        }

    }
    
    void ContextCallback (object node)
    {
        
    }
    
    [MenuItem("Window/React Editor")]
    static void Init ()
    {
        window = (ReactEditor)EditorWindow.GetWindow (typeof(ReactEditor));

        
        window.Show ();
        var n = Selection.activeObject as Reactable;
        if (n) {
            window.Load (n);
        } else {
            window.root = new Root ();  
        }
    }
    
    [MenuItem("Assets/Edit Reactable", false, 100)]
    public static void EditReactable ()
    {
        var r = Selection.activeObject as Reactable;
        if (r != null) {
            Init ();    
        }
    }
    
    [MenuItem("Assets/Edit Reactable", true, 100)]
    public static bool EditReactAIValidate ()
    {
        var r = Selection.activeObject as Reactable;
        return !(r == null);
    }
    
    [MenuItem("Assets/Create/Reactable")]
    public static void CreateReactAI ()
    {
        var root = "Assets";
        if (Selection.activeObject != null) {
            root = AssetDatabase.GetAssetPath (Selection.activeObject);
            if (!System.IO.Directory.Exists (root))
                root = System.IO.Path.GetDirectoryName (root);
        }
        var path = AssetDatabase.GenerateUniqueAssetPath (root + "/reactable.asset");
        var asset = ScriptableObject.CreateInstance<Reactable> ();
        AssetDatabase.CreateAsset (asset, path);
        
    }
    
    static ReactEditor window;
    Vector2 scrollPosition;
    Rect? toolWindowRect;
    System.Type[] nodeTypes = null;
}

