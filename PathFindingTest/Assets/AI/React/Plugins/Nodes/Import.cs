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
	public class Import : LeafNode
	{
		[ReactVar]
		public Reactable reactable;
		
		BaseNode importedChild;
		
		public override void Init (Reactor reactor)
		{
            base.Init(reactor);
            if (reactable != null) {
                #region Edited by PanDenat
                reactor.AddComponentsIfMissing(reactable);
                #endregion
                var root = (React.Root)React.JsonSerializer.Decode(reactable.json);
                root.PreProcess(gameObject, reactor);
                importedChild = root.children[0];

                #region Log if null
                if (importedChild == null) {
                    Debug.LogWarning("Imported root: " + root + " \n\n"
                        + "Imported reactable: " + reactable + "\n\n"
                        + "Imported reactable behaviours: " + reactable.behaviourTypes + "\n\n"
                        + "Reactor: " + reactor);
                    Debug.Log(reactable.json);
                }
                #endregion
            } else Debug.LogError("[Import Reactable Node] no reactable to import!\n Reactor.reactable = " + reactor.reactable);
		}
		
		public override IEnumerator<NodeResult> NodeTask ()
		{
			var task = importedChild.GetNodeTask();
			while(task.MoveNext()) {
				yield return task.Current;
			}
		}
		
		public override string ToString ()
		{
			return string.Format ("Import - {0}", (reactable == null ? "" : reactable.name));
		}
#if UNITY_EDITOR
        public new static string GetHelpText ()
		{
			return "Run a sub-tree from another Reactable asset. Allows modular and re-usable trees.";
		}
        
        public override void ResetActivity()
        {
            base.ResetActivity();
            if (importedChild != null)
            {
                importedChild.ResetActivity();
            }
        }

        public override void DrawChildren()
        {
            if (importedChild != null)
            {
                importedChild.parentNode = this;
                Rect cell = new Rect(0, 0, 0, 0);
                GUILayout.BeginVertical();
                cell = importedChild.DrawEditorWidget();
                bool activeBranch = isActive && importedChild.isActive;
                Color originalColor = Handles.color;
                if (activeBranch)
                {
                    Handles.color = Color.blue;
                }
                var A = new Vector2(rect.xMin + 17, rect.yMax);
                var B = new Vector2(rect.xMin + 17, cell.yMin + 10);
                var C = new Vector2(cell.xMin, cell.yMin + 10);               
                Handles.lighting = false;
                Handles.DrawPolyLine(A, B, C);
                if (activeBranch)
                {
                    Handles.color = originalColor;
                }
                GUILayout.EndVertical();
            }
        }

		public override void DrawInspector ()
		{
			base.DrawInspector ();
			if(reactable != null) {
				GUILayout.Space(10);
				if(GUILayout.Button("Edit")) {
					Selection.activeObject = reactable;				
					EditorApplication.ExecuteMenuItem("Assets/Edit Reactable");
				}
			}
		}
#endif
	}
	


}







