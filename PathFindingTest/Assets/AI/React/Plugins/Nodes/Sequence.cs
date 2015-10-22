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
	public class Sequence : BranchNode
	{
		public override IEnumerator<NodeResult> NodeTask ()
		{
			//Succeed if all nodes succeed.
			foreach (var child in ActiveChildren()) {
				var task = child.GetNodeTask();
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
			}
			yield return NodeResult.Success;
		}

		public override string ToString ()
		{
			return "Sequence";
		}

#if UNITY_EDITOR
        public new static string GetHelpText ()
		{
			return "Runs each child in turn, succeeding if all children succeed, but failing on the first child that fails.";
		}			
#endif		
		
	}

}







