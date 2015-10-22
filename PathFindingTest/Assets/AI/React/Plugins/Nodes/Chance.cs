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
    public class Chance : DecoratorNode
    {
        [ReactVar]
        public float
            probability = 0.5f;

        public override IEnumerator<NodeResult> NodeTask ()
        {
            if (Child == null) {
                yield return NodeResult.Failure;
                yield break;
            }
            if (UnityEngine.Random.value <= probability) {
                var task = Child.GetNodeTask ();
                while (task.MoveNext ()) {
                    yield return task.Current;
                }
            } else {
                yield return NodeResult.Failure;
            }
            
        }

        public override string ToString ()
        {
            return string.Format ("? {0}%", probability*100);
        }

#if UNITY_EDITOR
        public new static string GetHelpText ()
        {
            return "Execute a child using a random chance.";
        }           
#endif
    }
    


}







