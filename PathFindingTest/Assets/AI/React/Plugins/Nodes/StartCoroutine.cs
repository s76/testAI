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
    public class StartCoroutine : LeafNode
    {
        [ReactVar]
        public string
            coroutineName;
		
        Reactor target;

        public override void Init (Reactor r)
        {
            target = r;
        }
		
        public override IEnumerator<NodeResult> NodeTask ()
        {
            target.StartCoroutine (coroutineName);
            yield return NodeResult.Success;
        }
		
        public override string ToString ()
        {
            return string.Format ("StartCoroutine - {0}", coroutineName);
        }
	
	
#if UNITY_EDITOR

		
        public new static string GetHelpText ()
        {
            return "Starts a Coroutine on the gameobject.";
        }
		
		
#endif
    }

}







