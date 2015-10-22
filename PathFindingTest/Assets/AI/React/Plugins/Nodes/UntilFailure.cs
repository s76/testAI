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
	public class UntilFailure : DecoratorNode
	{
		public override IEnumerator<NodeResult> NodeTask ()
		{
			if (Child == null) {
				yield return NodeResult.Failure;
				yield break;
			}
			while (true) {
				var task = Child.GetNodeTask ();
				while (task.MoveNext ()) {
					var t = task.Current;
                    if (t == NodeResult.Continue) {
						yield return NodeResult.Continue;
					} else if (t == NodeResult.Failure) {
						yield return NodeResult.Failure;
						yield break;
					} else if (t == NodeResult.Success) {
						yield return NodeResult.Continue;
						break;
					}
				}
				yield return NodeResult.Continue;
			}
		}
		
		public override string ToString ()
		{
			return "UntilFailure";
		}
#if UNITY_EDITOR
        public new static string GetHelpText ()
		{
			return "Will run the child repeaatedly until a failure is received.";
		}			
#endif		
	}
	


}







