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
	public class TimeLimit : DecoratorNode
	{
		[ReactVar]
		public float seconds = 0;
		
		public override IEnumerator<NodeResult> NodeTask ()
		{
			//fail if something takes too long.
			if(Child == null) {
				yield return NodeResult.Failure;
				yield break;
			}
			var start = Time.time;
			var task = Child.GetNodeTask();
			
			while (task.MoveNext ()) {
				var t = task.Current;
				if ((Time.time - start) > seconds) {
					Child.Abort();
					yield return NodeResult.Failure;
					break;
				}
                if (t == NodeResult.Continue) {
					yield return NodeResult.Continue;
				} else {
					if (t == NodeResult.Failure) {
						yield return NodeResult.Failure;
						break;
					}
					if (t == NodeResult.Success) {
						yield return NodeResult.Success;
						break;
					}
				}
			}
			
		}
		
		public override string ToString ()
		{
			return string.Format ("TimeLimit - {0} sec", seconds);
		}
#if UNITY_EDITOR
        public new static string GetHelpText ()
		{
			return "Will run the child, but will fail if the child takes too long to succeed or fail.";
		}			
#endif		
		
	}
	


}







