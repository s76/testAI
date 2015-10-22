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
	public class Root : DecoratorNode
	{
		public override IEnumerator<NodeResult> NodeTask ()
		{
			if (Child == null) {
				yield return NodeResult.Failure;
				yield break;
			}
			while (true) {
				var task = Child.GetNodeTask ();
				while (true) {
					task.MoveNext ();
                    if (task.Current != NodeResult.Continue)
						break;
					yield return NodeResult.Continue;
				}
				yield return NodeResult.Continue;
			}
		}
		
		public override string ToString ()
		{
			return "Root";
		}
#if UNITY_EDITOR
        public new static string GetHelpText ()
		{
			return "The root node must have one child, which is executed repeatedly.";
		}	
#endif
		
	}
	
	

}







