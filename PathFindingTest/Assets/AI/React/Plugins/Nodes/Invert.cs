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
	public class Invert : DecoratorNode
	{
		public override IEnumerator<NodeResult> NodeTask ()
		{
			//invert result.
			if(Child == null) {
				yield return NodeResult.Failure;
				yield break;
			}
			var task = Child.GetNodeTask ();
			
			while (task.MoveNext ()) {
				var t = task.Current;
				if (t == NodeResult.Continue) {
					yield return NodeResult.Continue;
				} else if (t == NodeResult.Failure) {
					yield return NodeResult.Success;
					yield break;
				} else if (t == NodeResult.Success) {
					yield return NodeResult.Failure;
					yield break;
				}
			}
		}

		public override string ToString ()
		{
			return "Invert";
		}
#if UNITY_EDITOR
        public new static string GetHelpText ()
		{
			return "Run the child and invert the result before returning to the parent.";
		}			
#endif
	}
	


}







