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
	public class RandomSelector : BranchNode
	{
		public override IEnumerator<NodeResult> NodeTask ()
		{
			//Succeed if one node succeeds.
			var nodes = ActiveChildren().ToList();
			
			while(nodes.Count > 0) {
				var index = UnityEngine.Random.Range(0, nodes.Count-1);
				var child = nodes[index];
				nodes.RemoveAt(index);
				var task = child.GetNodeTask ();
				while (task.MoveNext ()) {
					var t = task.Current;
                    if (t == NodeResult.Continue) {
						yield return NodeResult.Continue;
					} else if (t == NodeResult.Failure) {
						yield return NodeResult.Continue;
						break;
					} else if (t == NodeResult.Success) {
						yield return NodeResult.Success;
						yield break;
					}
				}
			}
			yield return NodeResult.Failure;
		}
		
		public override string ToString ()
		{
			return "Random";
		}
#if UNITY_EDITOR
        public new static string GetHelpText ()
		{
			return "Pick a random child and return if it succeeds, or fail if none succeed.";
		}			
#endif		
	}
	

}







