using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace React
{
    [ReactNode]
    public class Function : LeafNode
    {
        public string  filter = "";
        public string functionName;
        Component component;
        System.Reflection.MethodInfo methodInfo;
		
        public override void Init (Reactor reactor)
        {
            base.Init(reactor);
            var cm = reactor.FindMethod (functionName);
            if (cm == null) {
                Debug.LogError ("Could not load function method: " + functionName);	
            } else {
                component = cm.component;
                methodInfo = cm.methodInfo;
            }
        }
		
        public override IEnumerator<NodeResult> NodeTask ()
        {
            if (component != null) {
                methodInfo.Invoke (component, null);
            }
            yield return NodeResult.Success;
        }
		
        public override string ToString ()
        {
            var actionMethodName = "";
            if (functionName != null && functionName.Length > 0) {
                actionMethodName = functionName.Substring (functionName.IndexOf ('.') + 1);
            }
            return string.Format ("Fn {0}{1}", actionMethodName, actionMethodName != "" ? "()" : "");
        }
	
	
#if UNITY_EDITOR
        public new static string GetHelpText ()
        {
            return "Runs a function from a MonoBehaviour, which is any method with no arguments. Discards the result.";
        }
		
        public override void DrawInspector ()
        {
            base.DrawInspector ();
            if (editorReactable != null) {
                List<string> methods;
                if (filter == null)
                    filter = "";
                if (filter != "") {
                    methods = (from i in editorReactable.Functions where i.ToLower ().Contains (filter.ToLower ()) select i).ToList ();
                } else {
                    methods = editorReactable.Functions;
                }
                if (methods.Count > 0) {
                    var idx = 0;
                    if (methods.Contains (functionName)) 
                        idx = methods.IndexOf (functionName);
                    GUILayout.BeginHorizontal ();
                    GUILayout.Label ("Function");
                    GUILayout.BeginVertical ();
                    GUILayout.BeginHorizontal ();
                    GUILayout.Label ("Filter", GUILayout.ExpandWidth (false));
                    filter = EditorGUILayout.TextField (filter, GUILayout.ExpandWidth (true));
                    GUILayout.EndHorizontal ();
                    idx = EditorGUILayout.Popup (idx, methods.ToArray (), "button");
                    GUILayout.EndVertical ();
                    GUILayout.EndHorizontal ();
                    functionName = methods [idx];
                    GUILayout.Space (8);
                    GUILayout.Label (editorReactable.DocText (functionName), "textarea");
                }
            }
        }
#endif
    }

}







