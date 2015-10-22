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
	public class Sleep : LeafNode
	{
		[ReactVar]
		public float seconds;
		
		public override IEnumerator<NodeResult> NodeTask ()
		{
			var N = Time.time;
			while (Time.time - seconds <= N)
				yield return NodeResult.Continue;
			yield return NodeResult.Success;
		}
		
		public override string ToString ()
		{
			return string.Format ("Sleep - {0}", seconds);
		}
#if UNITY_EDITOR
        public new static string GetHelpText ()
		{
			return "Stops execution of the tree for a period of time.";
		}			
#endif		
	}
	

}







