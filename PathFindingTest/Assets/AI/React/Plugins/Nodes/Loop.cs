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
	public class Loop : DecoratorNode
	{
		[ReactVar]
		public int loops;
		
		public override IEnumerator<NodeResult> NodeTask ()
		{
			if(Child == null) {
				yield return NodeResult.Failure;
				yield break;
			}
			for (int i = 0; i < loops; i++) {
				var task = Child.GetNodeTask();
				while (task.MoveNext ()) {
					
					var t = task.Current;
					if (t == NodeResult.Continue) {
						yield return NodeResult.Continue;
					} else if (t == NodeResult.Failure) {
						yield return NodeResult.Continue;
						break;
					} else if (t == NodeResult.Success) {
						yield return NodeResult.Continue;
						break;
					}
				}
			}
			yield return NodeResult.Success;
		}

		public override string ToString ()
		{
			return "Loop - " + loops.ToString() + "x";
		}
#if UNITY_EDITOR
        public new static string GetHelpText ()
		{
			return "Run the child N times, restarting the loop if any child succeeds or fails.";
		}			
#endif		
	}
	


}







