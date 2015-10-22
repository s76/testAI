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
	public class DecoratorNode : BranchNode
	{
		public BaseNode Child {
			get {
				if(children.Count == 0 || !children[0].enabled)
					return null;
				return children[0];
			}
		}
		public override void Add (BaseNode child)
		{
			base.Add(child);
			while(children.Count > 1) {
				children.RemoveAt(0);
			}
		}
		
		public override void Add (BaseNode child, BaseNode after)
		{
			Add (child);
		}
		
		public override void Insert (int index, BaseNode child)
		{
			Add (child);
		}

        public override bool HasAnyChildren()
        {
            return Child != null;
        }

#if UNITY_EDITOR	

        public override void ResetActivity()
        {
            base.ResetActivity();
            if (Child != null)
            {
                Child.ResetActivity();
            }            
        }

		public override void DrawGUI ()
		{
			base.DrawGUI();
			var erect = rect;
			if(children.Count == 0) {
				erect.x += erect.width;
				erect.y += 2;
				erect.width = 256;
				var c = GUI.color;
				GUI.color = Color.red;
				GUI.Label(erect, "Missing Child!");
				GUI.color = c;
			}
		}
		
		public override Color NodeColor ()
		{
			return new Color(0.7f,0.7f,1);
		}
#endif
	}
	

}







