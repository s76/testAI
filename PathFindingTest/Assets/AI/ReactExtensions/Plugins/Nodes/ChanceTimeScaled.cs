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
    public class ChanceTimeScaled : DecoratorNode
    {
        [ReactVar]
        Reactor reactor;

        [ReactVar]
        public float
            probability = 0.5f;

        public override void Init(Reactor r)
        {
            base.Init(r);
            reactor = r;
        }

        public override IEnumerator<NodeResult> NodeTask ()
        {
            if (Child == null) {
                yield return NodeResult.Failure;
                yield break;
            }
            if (UnityEngine.Random.value <= probability * reactor.TickDuration) {
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
            return string.Format ("? {0}% scaled to TickDuration", probability*100);
        }

#if UNITY_EDITOR
        public new static string GetHelpText ()
        {
            return "Execute a child using a random chance scaled to current reactor tick duration.";
        }           
#endif
    }
    


}







