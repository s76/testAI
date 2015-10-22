using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

namespace React
{
	[ReactNode]
	public class Action : LeafNode
	{
		public string           actionMethod;
        public MethodParams     methodParams = new MethodParams();        

		Component component;
		System.Reflection.MethodInfo methodInfo;
		
		public override void Init (Reactor reactor)
		{
            base.Init(reactor);

            ComponentMethod cm = null;
            try {
                cm = reactor.FindMethod(actionMethod);
            } catch (ArgumentNullException e) {
                Debug.LogError("[Action Node] Could not find method :" + actionMethod + "\n" + NodeNamesInStack());
                throw e;
            }
			if(cm == null) {
				Debug.LogError("Could not load action method: " + actionMethod);	
			} else {
				if(cm.methodInfo.ReturnType == typeof(IEnumerator<NodeResult>)) {
					component = cm.component;
					methodInfo = cm.methodInfo;
                    if( methodParams != null )
                    {
                        methodParams.ConvertParams(methodInfo);
                    }                    
				} else {
					Debug.LogError("Action method has invalid signature: " + actionMethod);	
				}
			}
		}
		
#if UNITY_EDITOR
        private void SetIsActiveInHierarchy()
        {
            isActive = true;
            
            BaseNode parent = parentNode;
            while (parent != null)
            {
                parent.isActive = true;
                parent = parent.parentNode;
            }
        }
#endif

		public override IEnumerator<NodeResult> NodeTask ()
		{
            if (component != null)
            {
                IEnumerator<NodeResult> task = (IEnumerator<NodeResult>)methodInfo.Invoke(component, methodParams != null ? methodParams.ConvertedParams : null );                
				while (true) 
                {
                    Profiler.BeginSample(actionMethod, component);
                    if (!task.MoveNext()) 
                    { 
                        Profiler.EndSample(); 
                        break; 
                    }
                    else 
                    { 
                        Profiler.EndSample(); 
                    }

#if UNITY_EDITOR
                    if (task.Current == NodeResult.Continue || task.Current == NodeResult.Success )
                    {
                        SetIsActiveInHierarchy();
                        Reactor.CheckBreakePoint(this);
                    }
#endif
                    yield return task.Current;
                   
				}
			}
			yield return NodeResult.Failure;
		}
		
		public override string ToString ()
		{
			var actionMethodName = "";
			if (actionMethod != null && actionMethod.Length > 0) {
				actionMethodName = actionMethod.Substring (actionMethod.IndexOf ('.') + 1);
			}
			return string.Format ("! {0}", actionMethodName);
		}
		
		
#if UNITY_EDITOR
		public new static string GetHelpText ()
		{
			return "Runs an action from a MonoBehaviour, which has the method signature 'public IEnumerator<NodeResult> MethodName()'.";
		}
		
		private bool searchShow = false;
		private string searchString = "";
		private Vector2 searchScrollPos;
		public override void DrawInspector ()
		{
			base.DrawInspector ();
			if(editorReactable != null) {
				var methods = editorReactable.Actions;
				if(methods.Count > 0) {
					var idx = 0;
					if(methods.Contains(actionMethod)) 
						idx = methods.IndexOf(actionMethod);
					GUILayout.BeginHorizontal();
					GUILayout.Label("Action");
					GUILayout.BeginVertical();
					var methodsArray = methods.ToArray();
					idx = EditorGUILayout.Popup(idx, methodsArray, "button");
					
					if(searchShow = EditorGUILayout.Foldout(searchShow, "Search for:"))
					{
						searchScrollPos = EditorGUILayout.BeginScrollView(searchScrollPos, GUILayout.Height(128));
						searchString = EditorGUILayout.TextField(searchString);
						if(!string.IsNullOrEmpty(searchString))
						{
							for(int i = 0; i < methodsArray.Length; i++)
							{
								var methodName = methodsArray[i];
								if(!string.IsNullOrEmpty(methodName) && methodName.Contains(searchString))
								{
									if(GUILayout.Button(methodName))
										idx = i;
								}
							}
						}
						EditorGUILayout.EndScrollView();
					}
					GUILayout.EndVertical();
					GUILayout.EndHorizontal();
                    
					actionMethod = methods[idx];
					GUILayout.Space (8);
					GUILayout.Label(editorReactable.DocText(actionMethod), "textarea");

                    if (methodParams == null)
                    {
                        methodParams = new MethodParams();
                    }
                    MethodInfo methodInfo = editorReactable.FindMethod( actionMethod );
                    methodParams.ReInit( methodInfo );
                    ReactParamsDrawer.DrawParamsInspector( methodInfo, methodParams );
                    
				}
			}
		}
		
		public override Color NodeColor ()
		{
			return Color.yellow;
		}
#endif
	}
	
	

}







