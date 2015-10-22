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
	public class Cooldown : DecoratorNode
	{
		[ReactVar]
		public float seconds;
		float lastRunTime = -1;
		
		public override IEnumerator<NodeResult> NodeTask ()
		{
			if (lastRunTime < 0)
				lastRunTime = Time.time;
			if(Child == null) {
				yield return NodeResult.Failure;
				yield break;
			}
			var T = Time.time - lastRunTime;
			if (T > seconds) {
				lastRunTime = Time.time;
				var task = Child.GetNodeTask ();
				while (task.MoveNext ()) {
					var t = task.Current;
					if (t == NodeResult.Continue) {
                        yield return NodeResult.Continue;
					} else if (t == NodeResult.Success) {
						yield return NodeResult.Success;
						break;
					} else if (t == NodeResult.Failure) {
						yield return NodeResult.Failure;
						break;
					}
				}
			} else {
				yield return NodeResult.Failure;
			}
		}
		
		public override string ToString ()
		{
			return string.Format ("Cooldown - {0} sec", seconds);
		}
		
#if UNITY_EDITOR
        public new static string GetHelpText ()
		{
			return "Will only run the child if N seconds have passed since the last time it was run.";
		}
			
#endif
	}
	


}







