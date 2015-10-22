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
    public class DebugResults : DecoratorNode
    {
        [ReactVar]
        public string debugText;

        public override void Init(Reactor r)
        {
            base.Init(r);
        }

        public override IEnumerator<NodeResult> NodeTask()
        {
            if (Child == null)
            {
                yield return NodeResult.Failure;
                yield break;
            }

            var task = Child.GetNodeTask();
            while (task.MoveNext())
            {
                var t = task.Current;
                Debug.Log(debugText);
                yield return t;
            }
        }

        public override string ToString()
        {
            return "DebugResult - " + debugText;
        }
#if UNITY_EDITOR
        public new static string GetHelpText()
        {
            return "Run the child N times, restarting the loop if any child succeeds or fails.";
        }
#endif
    }



}







