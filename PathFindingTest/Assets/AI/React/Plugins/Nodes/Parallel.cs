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
	[Serializable]
	public enum ParallelPolicy
	{
		FailIfOneFails,
		SucceedIfOneSucceeds
	}
	
	[ReactNode]	
	public class Parallel : BranchNode
	{
		
		[ReactVar("Policy")]
		public ParallelPolicy policy;
		
		public override IEnumerator<NodeResult> NodeTask ()
		{
			var tasks = new List<IEnumerator<NodeResult>> ();
			var childMap = new Dictionary<IEnumerator<NodeResult>,BaseNode>();
			foreach (var c in ActiveChildren()) {
				var t = c.GetNodeTask ();
				childMap[t] = c; 
				tasks.Add (t);
			}
			
			while (true) {
				if (tasks.Count == 0) {
					if (policy == ParallelPolicy.SucceedIfOneSucceeds) {
						yield return NodeResult.Failure;
					}
					if (policy == ParallelPolicy.FailIfOneFails) {
						yield return NodeResult.Success;
					}
				}
				var toKill = new List<int> ();
				for (var i=0; i<tasks.Count; i++) {
					var task = tasks [i];
					if (task.MoveNext ()) {
						if (policy == ParallelPolicy.SucceedIfOneSucceeds) {
                            if (task.Current != NodeResult.Continue) {
								if (task.Current == NodeResult.Success) {
									foreach(var c in childMap.Keys) {
										if(c != task) childMap[c].Abort();	
									}
									yield return NodeResult.Success;
								} else if (task.Current == NodeResult.Failure) {
									toKill.Add (i);
								}
							}
						}
						if (policy == ParallelPolicy.FailIfOneFails) {
                            if (task.Current != NodeResult.Continue) {
								if (task.Current == NodeResult.Failure) {
									foreach(var c in childMap.Keys) {
										if(c != task) childMap[c].Abort();	
									}
									yield return NodeResult.Failure;
								} else if (task.Current == NodeResult.Success)
									toKill.Add (i);
							}
						}
                        if (task.Current == NodeResult.Continue) {
						
						}
					} else {
						toKill.Add (i);
					}
				}
				
				toKill.Reverse ();
				foreach (var i in toKill) {					
					childMap.Remove(tasks[i]);
					tasks.RemoveAt (i);
				}
				toKill.Clear ();
			
				yield return NodeResult.Continue;
			}
		}
		
		public override string ToString ()
		{
			return string.Format ("Parallel - {0}", policy);
		}
		
#if UNITY_EDITOR
        public new static string GetHelpText ()
		{
			return "Will run all children in parallel, returning when the policy is met.";
		}			
#endif
	}
	


}







